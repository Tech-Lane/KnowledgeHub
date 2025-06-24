using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using KnowledgeHubV2.Models;

namespace KnowledgeHubV2.Services
{
    public enum SelectedItemType { None, Folder, Note }

    /// <summary>
    /// A simple state container and event bus for the application.
    /// This allows disconnected components to notify each other of state changes.
    /// </summary>
    public class StateContainer
    {
        private readonly IServiceProvider _serviceProvider;
        private Dictionary<string, object?> _properties = new();

        /// <summary>
        /// This event is invoked when the notes/folders hierarchy has changed
        /// (e.g., a new note or folder was created). Components displaying this
        /// data should subscribe to this event to refresh their state.
        /// </summary>
        public event Action? OnChange;

        public SelectedItemType SelectedType { get; private set; }
        public int? SelectedId { get; private set; }

        public Folder? DraggedFolder => GetProperty("draggedFolder") as Folder;
        public Note? DraggedNote => GetProperty("draggedNote") as Note;

        public StateContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetSelection(SelectedItemType type, int? id)
        {
            SelectedType = type;
            SelectedId = id;
            OnChange?.Invoke();
        }

        public object? GetProperty(string key)
        {
            _properties.TryGetValue(key, out var value);
            return value;
        }

        public void SetProperty(string key, object? value)
        {
            _properties[key] = value;
        }

        public void StartDrag(Folder? folder, Note? note)
        {
            SetProperty("draggedFolder", folder);
            SetProperty("draggedNote", note);
        }

        public async Task HandleDrop(int? targetFolderId)
        {
            using var scope = _serviceProvider.CreateScope();
            var noteRepository = scope.ServiceProvider.GetRequiredService<NoteRepository>();

            if (DraggedFolder != null)
            {
                await noteRepository.MoveFolderAsync(DraggedFolder.FolderID, targetFolderId);
            }
            else if (DraggedNote != null)
            {
                await noteRepository.MoveNoteAsync(DraggedNote.NoteID, targetFolderId);
            }

            await NotifyStateChanged();
        }

        /// <summary>
        /// Notifies all subscribers that the note/folder hierarchy has changed.
        /// </summary>
        public async Task NotifyStateChanged()
        {
            OnChange?.Invoke();
            await Task.CompletedTask;
        }
    }
} 