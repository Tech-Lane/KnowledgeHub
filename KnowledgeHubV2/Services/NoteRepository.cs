using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KnowledgeHubV2.Data;
using KnowledgeHubV2.Models;
using Microsoft.EntityFrameworkCore;
using HtmlAgilityPack;
using System.Text;
using System.Web;
using System;

namespace KnowledgeHubV2.Services
{
    /// <summary>
    /// Repository for handling data operations for Notes and Folders.
    /// </summary>
    public class NoteRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public NoteRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Retrieves all root folders (folders with no parent).
        /// </summary>
        /// <returns>A list of root Folder objects with their children and notes pre-loaded.</returns>
        public virtual async Task<List<Folder>> GetRootFoldersAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            // This is a classic hierarchical data problem. A simple approach is to load all folders
            // and build the hierarchy in memory. For larger datasets, a recursive CTE would be better,
            // but that's more complex to set up with EF Core and SQLite.
            var allFolders = await context.Folders
                .Include(f => f.Notes)
                .ToListAsync();
            
            var folderDict = allFolders.ToDictionary(f => f.FolderID);
            var rootFolders = new List<Folder>();

            foreach (var folder in allFolders)
            {
                if (folder.ParentFolderID.HasValue && folderDict.TryGetValue(folder.ParentFolderID.Value, out var parent))
                {
                    parent.ChildFolders.Add(folder);
                }
                else
                {
                    rootFolders.Add(folder);
                }
            }
            return rootFolders;
        }

        /// <summary>
        /// Retrieves all notes that do not belong to any folder.
        /// </summary>
        /// <returns>A list of root Note objects.</returns>
        public virtual async Task<List<Note>> GetRootNotesAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Notes
                .Where(n => n.FolderID == null)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new folder.
        /// </summary>
        /// <param name="name">The name of the new folder.</param>
        /// <param name="parentFolderId">The ID of the parent folder, if any.</param>
        /// <returns>The newly created Folder object.</returns>
        public virtual async Task<Folder> CreateFolderAsync(string name, int? parentFolderId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var newFolder = new Folder
            {
                Name = name,
                ParentFolderID = parentFolderId
            };
            context.Folders.Add(newFolder);
            await context.SaveChangesAsync();

            // Detach the entity to avoid accessing a disposed context
            context.Entry(newFolder).State = EntityState.Detached;

            return newFolder;
        }

        /// <summary>
        /// Creates a new note.
        /// </summary>
        /// <param name="title">The title of the new note.</param>
        /// <param name="folderId">The ID of the parent folder, if any.</param>
        /// <returns>The newly created Note object.</returns>
        public virtual async Task<Note> CreateNoteAsync(string title, int? folderId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var newNote = new Note
            {
                Title = title,
                FolderID = folderId,
                Content = ""
            };
            context.Notes.Add(newNote);
            await context.SaveChangesAsync();
            return newNote;
        }

        public virtual async Task<NoteTable> CreateTableForNoteAsync(int noteId, NoteTable table)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            table.NoteID = noteId;
            context.NoteTables.Add(table);
            await context.SaveChangesAsync();
            return table;
        }

        public virtual async Task<Note?> GetNoteByIdAsync(int noteId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Notes
                .Include(n => n.Tables)
                .ThenInclude(t => t.Columns)
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NoteID == noteId);
        }

        public virtual async Task<NoteTable?> GetTableByIdAsync(int tableId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.NoteTables.Include(t => t.Columns).FirstOrDefaultAsync(t => t.NoteTableID == tableId);
        }

        public virtual async Task<decimal> GetAggregateValueAsync(int tableId, string columnName, string aggregateType)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var table = await context.NoteTables.FindAsync(tableId);
            if (table == null)
            {
                return 0;
            }

            var rows = table.Rows;
            if (!rows.Any())
            {
                return 0;
            }

            switch (aggregateType.ToLower())
            {
                case "sum":
                    return rows.Sum(r => r.TryGetValue(columnName, out var val) && val != null && decimal.TryParse(val.ToString(), out var result) ? result : 0);
                case "count":
                    return rows.Count;
                case "avg":
                    return rows.Average(r => r.TryGetValue(columnName, out var val) && val != null && decimal.TryParse(val.ToString(), out var result) ? result : 0);
                default:
                    return 0;
            }
        }

        public virtual async Task<Note> AddNoteAsync(string title, string content, int? folderId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var newNote = new Note
            {
                Title = title,
                Content = content,
                FolderID = folderId
            };
            context.Notes.Add(newNote);
            await context.SaveChangesAsync();
            return newNote;
        }

        public virtual async Task UpdateNoteAsync(Note note, List<NoteTable> tables)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            
            // This is a more robust "disconnected" update pattern.
            // It treats the incoming 'note' object as the source of truth and applies changes accordingly.
            context.Update(note);

            // Get the IDs of tables currently in the database for this note
            var existingTableIds = await context.NoteTables
                .Where(t => t.NoteID == note.NoteID)
                .Select(t => t.NoteTableID)
                .ToListAsync();
            
            var incomingTableIds = tables.Select(t => t.NoteTableID);
            var tablesToDelete = existingTableIds.Except(incomingTableIds).ToList();

            if (tablesToDelete.Any())
            {
                await Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteDeleteAsync(context.NoteTables.Where(t => tablesToDelete.Contains(t.NoteTableID)));
            }

            foreach (var table in tables)
            {
                // Ensure the foreign key is set
                table.NoteID = note.NoteID;

                // If the table has an ID, it's an update. If not, it's an add.
                // EF's Update method handles both cases correctly for disconnected entities.
                context.Update(table);
            }

            await context.SaveChangesAsync();
        }

        public virtual async Task<List<NoteTable>> GetTablesForNoteAsync(int noteId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var note = await context.Notes
                .Include(n => n.Tables)
                .ThenInclude(t => t.Columns)
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NoteID == noteId);

            return note?.Tables.ToList() ?? new List<NoteTable>();
        }

        public virtual async Task<IEnumerable<NoteTable>> GetAllTablesAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.NoteTables.Include(t => t.Columns).ToListAsync();
        }

        public virtual async Task MoveFolderAsync(int folderId, int? targetFolderId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var folderToMove = await context.Folders.FindAsync(folderId);
            if (folderToMove != null)
            {
                // Basic check to prevent cyclical dependencies
                if (folderToMove.FolderID == targetFolderId) return;

                folderToMove.ParentFolderID = targetFolderId;
                await context.SaveChangesAsync();
            }
        }

        public virtual async Task MoveNoteAsync(int noteId, int? targetFolderId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var noteToMove = await context.Notes.FindAsync(noteId);
            if (noteToMove != null)
            {
                noteToMove.FolderID = targetFolderId;
                await context.SaveChangesAsync();
            }
        }

        public virtual async Task RenameFolderAsync(int folderId, string newName)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var folder = await context.Folders.FindAsync(folderId);
            if (folder != null)
            {
                folder.Name = newName;
                await context.SaveChangesAsync();
            }
        }

        public virtual async Task RenameNoteAsync(int noteId, string newTitle)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var note = await context.Notes.FindAsync(noteId);
            if (note != null)
            {
                note.Title = newTitle;
                await context.SaveChangesAsync();
            }
        }

        public virtual async Task DeleteNoteAsync(int noteId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var note = await context.Notes.FindAsync(noteId);
            if (note != null)
            {
                context.Notes.Remove(note);
                await context.SaveChangesAsync();
            }
        }

        public virtual async Task DeleteFolderAsync(int folderId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var folder = await context.Folders.FindAsync(folderId);
            if (folder != null)
            {
                await DeleteFolderRecursive(context, folder);
                await context.SaveChangesAsync();
            }
        }

        private async Task DeleteFolderRecursive(ApplicationDbContext context, Folder folder)
        {
            var childFolders = await context.Folders.Where(f => f.ParentFolderID == folder.FolderID).ToListAsync();
            foreach (var child in childFolders)
            {
                await DeleteFolderRecursive(context, child);
            }

            var notes = await context.Notes.Where(n => n.FolderID == folder.FolderID).ToListAsync();
            context.Notes.RemoveRange(notes);

            context.Folders.Remove(folder);
        }

        public virtual async Task<List<IFileSystemItem>> SearchAsync(string searchTerm)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var lowercasedTerm = searchTerm.ToLower();

            var foundFolders = await context.Folders
                .Where(f => f.Name.ToLower().Contains(lowercasedTerm))
                .ToListAsync();

            var foundNotes = await context.Notes
                .Where(n => n.Title.ToLower().Contains(lowercasedTerm) || n.Content.ToLower().Contains(lowercasedTerm))
                .ToListAsync();

            var results = new List<IFileSystemItem>();
            results.AddRange(foundFolders);
            results.AddRange(foundNotes);

            return results;
        }

        public virtual async Task<List<Note>> GetAllNotesWithFolderAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Notes.Include(n => n.Folder).ToListAsync();
        }

        public virtual async Task<(IEnumerable<Note> Notes, IEnumerable<Folder> Folders)> GetAllDataAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var notes = await context.Notes.Include(n => n.Tables).ThenInclude(t => t.Columns).ToListAsync();
            var folders = await context.Folders.ToListAsync();
            return (notes, folders);
        }

        public virtual async Task WipeAndReplaceDataAsync(IEnumerable<Note> notes, IEnumerable<Folder> folders)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            // Using ExecuteDeleteAsync for efficient bulk deletion
            await Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteDeleteAsync(context.Notes);
            await Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteDeleteAsync(context.Folders);

            // It's often necessary to reset identity columns after a wipe,
            // but EF Core with SQLite handles this reasonably well. For other DBs,
            // a raw SQL command might be needed (e.g., TRUNCATE or DBCC CHECKIDENT).

            await context.Folders.AddRangeAsync(folders);
            await context.Notes.AddRangeAsync(notes);

            await context.SaveChangesAsync();
        }
    }
} 