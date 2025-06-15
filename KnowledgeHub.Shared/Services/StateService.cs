using KnowledgeHub.Shared.Services;

namespace KnowledgeHub.Shared.Services
{
    public class StateService : IStateService
    {
        public object? SelectedItem { get; private set; }
        public string? CurrentViewType { get; private set; }

        public event Action? OnChange;

        public void SetSelectedItem(string viewType, object? item)
        {
            CurrentViewType = viewType;
            SelectedItem = item;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
