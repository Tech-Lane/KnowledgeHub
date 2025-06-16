using KnowledgeHub.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeHub.Shared.Services
{
    public interface INoteDataService
    {
        Task<List<Folder>?> GetFoldersAsync();
        Task<List<Note>?> GetNotesAsync();

        Task<Note?> GetNoteAsync(int id);
        Task UpdateNoteAsync(Note note);
        Task DeleteNoteAsync(int id);
        Task<Folder?> CreateFolderAsync(Folder newFolder);
        Task<Note?> CreateNoteAsync(Note newNote);
        Task<Table> UpdateTableAsync(Table table);
        Task<Table> GetTableAsync(int id);
    }
}
