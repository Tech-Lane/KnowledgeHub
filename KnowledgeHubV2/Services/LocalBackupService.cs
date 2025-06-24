using KnowledgeHubV2.Data;
using KnowledgeHubV2.Models;
using Microsoft.JSInterop;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO.Compression;

namespace KnowledgeHubV2.Services;

public class LocalBackupService
{
    private readonly NoteRepository _noteRepository;
    private readonly IJSRuntime _jsRuntime;
    private readonly StateContainer _stateContainer;

    public LocalBackupService(NoteRepository noteRepository, IJSRuntime jsRuntime, StateContainer stateContainer)
    {
        _noteRepository = noteRepository;
        _jsRuntime = jsRuntime;
        _stateContainer = stateContainer;
    }

    public async Task<(byte[] data, string fileName)> ExportDataAsync(string? password = null)
    {
        var (notes, folders) = await _noteRepository.GetAllDataAsync();
        var backupData = new BackupModel { Notes = notes.ToList(), Folders = folders.ToList() };
        var jsonData = JsonSerializer.Serialize(backupData, new JsonSerializerOptions { WriteIndented = true });
        var byteData = Encoding.UTF8.GetBytes(jsonData);

        if (!string.IsNullOrWhiteSpace(password))
        {
            var encryptedBase64 = await _jsRuntime.InvokeAsync<string>("cryptoFunctions.encrypt", byteData, password);
            byteData = Convert.FromBase64String(encryptedBase64);
            return (byteData, "knowledgehub_backup.khb-enc");
        }

        return (byteData, "knowledgehub_backup.khb");
    }

    public async Task ImportDataAsync(byte[] fileContent, string? password = null)
    {
        byte[] jsonData;
        if (!string.IsNullOrWhiteSpace(password))
        {
            var encryptedBase64 = Convert.ToBase64String(fileContent);
            var decryptedBytes = await _jsRuntime.InvokeAsync<byte[]?>("cryptoFunctions.decrypt", encryptedBase64, password);

            if (decryptedBytes == null)
            {
                throw new InvalidOperationException("Decryption failed. Invalid password or corrupted file.");
            }
            jsonData = decryptedBytes;
        }
        else
        {
            jsonData = fileContent;
        }

        var jsonString = Encoding.UTF8.GetString(jsonData);
        var backupData = JsonSerializer.Deserialize<BackupModel>(jsonString);

        if (backupData?.Notes != null && backupData.Folders != null)
        {
            await _noteRepository.WipeAndReplaceDataAsync(backupData.Notes, backupData.Folders);
            _stateContainer.NotifyStateChanged();
            await _jsRuntime.InvokeVoidAsync("alert", "Import successful!");
        }
        else
        {
            throw new JsonException("Invalid backup file format.");
        }
    }
}

public class BackupModel
{
    public List<Note> Notes { get; set; } = new();
    public List<Folder> Folders { get; set; } = new();
} 