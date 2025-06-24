using KnowledgeHubV2.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace KnowledgeHubV2.Tests.Models
{
    public class NoteTests
    {
        [Fact]
        public void Note_ShouldInitializeWithDefaultValues()
        {
            // Act
            var note = new Note();

            // Assert
            Assert.Equal(0, note.NoteID);
            Assert.Equal(string.Empty, note.Title);
            Assert.Equal(string.Empty, note.Content);
            Assert.Null(note.FolderID);
            Assert.NotNull(note.Tables);
            Assert.Empty(note.Tables);
        }

        [Fact]
        public void Note_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var note = new Note();
            var expectedTitle = "Test Note";
            var expectedContent = "This is test content";
            var expectedDate = DateTime.Now;
            var expectedFolderId = 5;

            // Act
            note.NoteID = 1;
            note.Title = expectedTitle;
            note.Content = expectedContent;
            note.UpdatedAt = expectedDate;
            note.FolderID = expectedFolderId;

            // Assert
            Assert.Equal(1, note.NoteID);
            Assert.Equal(expectedTitle, note.Title);
            Assert.Equal(expectedContent, note.Content);
            Assert.Equal(expectedDate, note.UpdatedAt);
            Assert.Equal(expectedFolderId, note.FolderID);
        }

        [Fact]
        public void Note_IFileSystemItemInterface_ShouldReturnCorrectValues()
        {
            // Arrange
            var note = new Note
            {
                NoteID = 10,
                Title = "Interface Test",
                FolderID = 20
            };

            // Act
            IFileSystemItem fileSystemItem = note;

            // Assert
            Assert.Equal(10, fileSystemItem.ID);
            Assert.Equal("Interface Test", fileSystemItem.Name);
            Assert.Equal(20, fileSystemItem.ParentID);
        }

        [Fact]
        public void Note_TitleValidation_ShouldRequireTitle()
        {
            // Arrange
            var note = new Note { Title = null! };
            var context = new ValidationContext(note) { MemberName = nameof(Note.Title) };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(note.Title, context, results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("required"));
        }

        [Fact]
        public void Note_TitleValidation_ShouldEnforceMaxLength()
        {
            // Arrange
            var note = new Note { Title = new string('a', 201) }; // 201 characters
            var context = new ValidationContext(note) { MemberName = nameof(Note.Title) };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(note.Title, context, results);

            // Assert
            Assert.False(isValid);
        }
    }
} 