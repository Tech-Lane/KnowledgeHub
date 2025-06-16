using KnowledgeHub.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace KnowledgeHub.Shared.Services
{
    public class StateService : IStateService
    {
        private Dispatcher? _dispatcher;
        public object? SelectedItem { get; private set; }
        public string? CurrentViewType { get; private set; }

        public event Action? OnChange;

        public void Initialize(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void SetSelectedItem(string viewType, object? item)
        {
            CurrentViewType = viewType;
            SelectedItem = item;
            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            // if (_dispatcher != null)
            // {
            //     _dispatcher.InvokeAsync(() => OnChange?.Invoke());
            // }
            OnChange?.Invoke();
        }
    }
}
