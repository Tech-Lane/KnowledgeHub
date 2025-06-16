// In KnowledgeHub.Web/Services/ServerNoteDataService.cs
using KnowledgeHub.Shared.Models;
using KnowledgeHub.Shared.Services;
using KnowledgeHub.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeHub.Web.Services
{
    public class ServerNoteDataService : INoteDataService
    {
        private readonly ApplicationDbContext _context;

        public ServerNoteDataService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Folder>?> GetFoldersAsync()
        {
            return await _context.Folders.ToListAsync();
        }

        public async Task<List<Note>?> GetNotesAsync()
        {
            return await _context.Notes.ToListAsync();
        }

        public async Task<Note?> GetNoteAsync(int id)
        {
            return await _context.Notes
                .Include(n => n.Tables)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task UpdateNoteAsync(Note note)
        {
            // Also update the 'UpdatedAt' timestamp
            note.UpdatedAt = DateTime.UtcNow;
            _context.Update(note);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNoteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Folder?> CreateFolderAsync(Folder newFolder)
        {
            _context.Folders.Add(newFolder);
            await _context.SaveChangesAsync();
            return newFolder;
        }

        public async Task<Note?> CreateNoteAsync(Note newNote)
        {
            _context.Notes.Add(newNote);
            await _context.SaveChangesAsync();
            return newNote;
        }

        public async Task<Table> UpdateTableAsync(Table table)
        {
            var existingTable = await _context.Tables.FindAsync(table.Id);
            if (existingTable != null)
            {
                existingTable.DataAsJson = table.DataAsJson;
                await _context.SaveChangesAsync();
                return existingTable;
            }
            return null;
        }

        public async Task<Table> GetTableAsync(int id)
        {
            return await _context.Tables.FindAsync(id);
        }
    }
}