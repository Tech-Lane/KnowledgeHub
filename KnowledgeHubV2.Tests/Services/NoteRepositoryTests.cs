using KnowledgeHubV2.Data;
using KnowledgeHubV2.Models;
using KnowledgeHubV2.Services;
using KnowledgeHubV2.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace KnowledgeHubV2.Tests.Services
{
    public class NoteRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public NoteRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAllDataAsync_ShouldReturnNotesAndFolders()
        {
            // Arrange
            var notes = new List<Note>
            {
                new Note { Title = "Note 1" },
                new Note { Title = "Note 2" }
            };
            var folders = new List<Folder>
            {
                new Folder { Name = "Folder 1" },
                new Folder { Name = "Folder 2" }
            };

            _fixture.Context.Notes.AddRange(notes);
            _fixture.Context.Folders.AddRange(folders);
            await _fixture.Context.SaveChangesAsync();

            // Act
            var (returnedNotes, returnedFolders) = await _fixture.NoteRepository.GetAllDataAsync();

            // Assert
            Assert.Equal(2, returnedNotes.Count());
            Assert.Equal(2, returnedFolders.Count());
        }
    }
} 