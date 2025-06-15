// In KnowledgeHub.Web.Client/Services/ClientNoteDataService.cs
using KnowledgeHub.Shared.Models;
using KnowledgeHub.Shared.Services;
using System.Net.Http.Json;

namespace KnowledgeHub.Web.Client.Services
{
    public class ClientNoteDataService : INoteDataService
    {
        private readonly HttpClient _http;

        public ClientNoteDataService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Folder>?> GetFoldersAsync()
        {
            return await _http.GetFromJsonAsync<List<Folder>>("api/Folders");
        }

        public async Task<List<Note>?> GetNotesAsync()
        {
            return await _http.GetFromJsonAsync<List<Note>>("api/Notes");
        }

        public async Task<Note?> GetNoteAsync(int id)
        {
            return await _http.GetFromJsonAsync<Note>($"api/Notes/{id}");
        }

        public async Task UpdateNoteAsync(Note note)
        {
            await _http.PutAsJsonAsync($"api/Notes/{note.Id}", note);
        }

        public async Task DeleteNoteAsync(int id)
        {
            await _http.DeleteAsync($"api/Notes/{id}");
        }
    }
}