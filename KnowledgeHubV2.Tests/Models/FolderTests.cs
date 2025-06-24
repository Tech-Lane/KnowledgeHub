using KnowledgeHubV2.Models;
using Xunit;

namespace KnowledgeHubV2.Tests.Models
{
    public class FolderTests
    {
        [Fact]
        public void Folder_ShouldInitializeWithDefaultValues()
        {
            // Act
            var folder = new Folder();

            // Assert
            Assert.Equal(0, folder.FolderID);
            Assert.Equal(string.Empty, folder.Name);
            Assert.Null(folder.ParentFolderID);
            Assert.NotNull(folder.ChildFolders);
            Assert.Empty(folder.ChildFolders);
            Assert.NotNull(folder.Notes);
            Assert.Empty(folder.Notes);
        }

        [Fact]
        public void Folder_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var folder = new Folder();
            var expectedName = "Test Folder";
            var expectedParentId = 5;

            // Act
            folder.FolderID = 1;
            folder.Name = expectedName;
            folder.ParentFolderID = expectedParentId;

            // Assert
            Assert.Equal(1, folder.FolderID);
            Assert.Equal(expectedName, folder.Name);
            Assert.Equal(expectedParentId, folder.ParentFolderID);
        }

        [Fact]
        public void Folder_IFileSystemItemInterface_ShouldReturnCorrectValues()
        {
            // Arrange
            var folder = new Folder
            {
                FolderID = 10,
                Name = "Interface Test Folder",
                ParentFolderID = 20
            };

            // Act
            IFileSystemItem fileSystemItem = folder;

            // Assert
            Assert.Equal(10, fileSystemItem.ID);
            Assert.Equal("Interface Test Folder", fileSystemItem.Name);
            Assert.Equal(20, fileSystemItem.ParentID);
        }

        [Fact]
        public void Folder_ShouldAllowAddingSubFolders()
        {
            // Arrange
            var parentFolder = new Folder { FolderID = 1, Name = "Parent" };
            var childFolder = new Folder { FolderID = 2, Name = "Child", ParentFolderID = 1 };

            // Act
            parentFolder.ChildFolders.Add(childFolder);

            // Assert
            Assert.Single(parentFolder.ChildFolders);
            Assert.Contains(childFolder, parentFolder.ChildFolders);
            Assert.Equal(parentFolder.FolderID, childFolder.ParentFolderID);
        }

        [Fact]
        public void Folder_ShouldAllowAddingNotes()
        {
            // Arrange
            var folder = new Folder { FolderID = 1, Name = "Notes Container" };
            var note = new Note { NoteID = 1, Title = "Test Note", FolderID = 1 };

            // Act
            folder.Notes.Add(note);

            // Assert
            Assert.Single(folder.Notes);
            Assert.Contains(note, folder.Notes);
            Assert.Equal(folder.FolderID, note.FolderID);
        }
    }
} 