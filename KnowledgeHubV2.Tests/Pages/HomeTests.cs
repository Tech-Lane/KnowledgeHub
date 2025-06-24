using Bunit;
using KnowledgeHubV2.Data;
using KnowledgeHubV2.Models;
using KnowledgeHubV2.Pages;
using KnowledgeHubV2.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace KnowledgeHubV2.Tests.Pages
{
    public class HomeTests : Bunit.TestContext
    {
        private Mock<NoteRepository> _noteRepositoryMock;
        private Mock<StateContainer> _stateContainerMock;

        public HomeTests()
        {
            var dbContextFactoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            _noteRepositoryMock = new Mock<NoteRepository>(dbContextFactoryMock.Object);
            _stateContainerMock = new Mock<StateContainer>(Mock.Of<IServiceProvider>());
            
            Services.AddSingleton(_noteRepositoryMock.Object);
            Services.AddSingleton(_stateContainerMock.Object);
            Services.AddSingleton<NavigationManager>(new MockNavigationManager());
        }

        [Fact]
        public void Home_ShouldRenderWelcomeMessage()
        {
            // Arrange
            SetupMockData();

            // Act
            var component = RenderComponent<Home>();

            // Assert
            Assert.Contains("Welcome to KnowledgeHub", component.Markup);
            Assert.Contains("Your central place for organizing thoughts", component.Markup);
        }

        [Fact]
        public void Home_ShouldDisplayRecentNotes()
        {
            // Arrange
            var recentNotes = new List<Note>
            {
                new Note { NoteID = 1, Title = "Note 1", UpdatedAt = DateTime.Now },
                new Note { NoteID = 2, Title = "Note 2", UpdatedAt = DateTime.Now.AddDays(-1) }
            };
            
            _noteRepositoryMock
                .Setup(x => x.GetAllDataAsync())
                .ReturnsAsync((recentNotes, new List<Folder>()));

            // Act
            var component = RenderComponent<Home>();

            // Assert
            Assert.Contains("Note 1", component.Markup);
            Assert.Contains("Note 2", component.Markup);
            Assert.Contains("Recent Notes", component.Markup);
        }

        [Fact]
        public void Home_ShouldDisplayStatistics()
        {
            // Arrange
            var notes = Enumerable.Range(1, 5).Select(i => new Note { NoteID = i }).ToList();
            var folders = Enumerable.Range(1, 3).Select(i => new Folder { FolderID = i }).ToList();
            
            _noteRepositoryMock
                .Setup(x => x.GetAllDataAsync())
                .ReturnsAsync((notes, folders));

            // Act
            var component = RenderComponent<Home>();

            // Assert
            Assert.Contains("Total Notes:", component.Markup);
            Assert.Contains("5", component.Markup);
            Assert.Contains("Total Folders:", component.Markup);
            Assert.Contains("3", component.Markup);
        }

        [Fact]
        public void Home_ShouldShowNoRecentNotesMessage_WhenNoNotesExist()
        {
            // Arrange
            _noteRepositoryMock
                .Setup(x => x.GetAllDataAsync())
                .ReturnsAsync((new List<Note>(), new List<Folder>()));

            // Act
            var component = RenderComponent<Home>();

            // Assert
            Assert.Contains("No recent notes found", component.Markup);
        }

        [Fact]
        public void CreateNewNote_ShouldNavigateToNoteEditor()
        {
            // Arrange
            SetupMockData();
            var newNote = new Note { NoteID = 99, Title = "New Note" };
            _noteRepositoryMock
                .Setup(x => x.CreateNoteAsync("New Note", null))
                .ReturnsAsync(newNote);

            var component = RenderComponent<Home>();
            var navigationManager = Services.GetRequiredService<NavigationManager>() as MockNavigationManager;

            // Act
            var createButton = component.Find("button:contains('New Note')");
            createButton.Click();

            // Assert
            Assert.Equal("/note-editor/99", navigationManager?.LastNavigatedTo);
        }

        private void SetupMockData()
        {
            _noteRepositoryMock
                .Setup(x => x.GetAllDataAsync())
                .ReturnsAsync((new List<Note>(), new List<Folder>()));
        }

        private class MockNavigationManager : NavigationManager
        {
            public string? LastNavigatedTo { get; private set; }

            public MockNavigationManager()
            {
                Initialize("http://localhost/", "http://localhost/");
            }

            protected override void NavigateToCore(string uri, bool forceLoad)
            {
                LastNavigatedTo = uri;
            }
        }
    }
} 