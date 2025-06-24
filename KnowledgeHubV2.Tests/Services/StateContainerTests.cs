using KnowledgeHubV2.Models;
using KnowledgeHubV2.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace KnowledgeHubV2.Tests.Services
{
    public class StateContainerTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly StateContainer _stateContainer;

        public StateContainerTests()
        {
            var services = new ServiceCollection();
            services.AddScoped<NoteRepository>();
            _serviceProvider = services.BuildServiceProvider();
            _stateContainer = new StateContainer(_serviceProvider);
        }

        [Fact]
        public void SetSelection_ShouldUpdateSelectedTypeAndId()
        {
            // Arrange
            var expectedType = SelectedItemType.Note;
            var expectedId = 123;

            // Act
            _stateContainer.SetSelection(expectedType, expectedId);

            // Assert
            Assert.Equal(expectedType, _stateContainer.SelectedType);
            Assert.Equal(expectedId, _stateContainer.SelectedId);
        }

        [Fact]
        public void SetSelection_ShouldTriggerOnChangeEvent()
        {
            // Arrange
            var eventTriggered = false;
            _stateContainer.OnChange += () => eventTriggered = true;

            // Act
            _stateContainer.SetSelection(SelectedItemType.Folder, 1);

            // Assert
            Assert.True(eventTriggered);
        }

        [Fact]
        public void SetProperty_ShouldStoreAndRetrieveValue()
        {
            // Arrange
            var key = "testKey";
            var value = "testValue";

            // Act
            _stateContainer.SetProperty(key, value);
            var retrievedValue = _stateContainer.GetProperty(key);

            // Assert
            Assert.Equal(value, retrievedValue);
        }

        [Fact]
        public void StartDrag_ShouldSetDraggedItems()
        {
            // Arrange
            var folder = new Folder { FolderID = 1, Name = "Test Folder" };
            var note = new Note { NoteID = 2, Title = "Test Note" };

            // Act
            _stateContainer.StartDrag(folder, note);

            // Assert
            Assert.Equal(folder, _stateContainer.DraggedFolder);
            Assert.Equal(note, _stateContainer.DraggedNote);
        }

        [Fact]
        public async Task NotifyStateChanged_ShouldTriggerOnChangeEvent()
        {
            // Arrange
            var eventTriggered = false;
            _stateContainer.OnChange += () => eventTriggered = true;

            // Act
            await _stateContainer.NotifyStateChanged();

            // Assert
            Assert.True(eventTriggered);
        }
    }
} 