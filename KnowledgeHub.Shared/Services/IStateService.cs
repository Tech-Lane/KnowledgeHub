using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeHub.Shared.Services
{
    public interface IStateService
    {
        object? SelectedItem { get; }
        string? CurrentViewType { get; }
        event Action? OnChange;
        void SetSelectedItem(string viewType, object? item);
        void Initialize(Dispatcher dispatcher);
    }
}
