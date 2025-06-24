using KnowledgeHubV2.Services;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace KnowledgeHubV2.Tests.Services
{
    public class FileSystemServiceTests
    {
        private readonly Mock<IJSRuntime> _jsRuntimeMock;
        private readonly Mock<IJSObjectReference> _jsModuleMock;
        private readonly FileSystemService _fileSystemService;

        public FileSystemServiceTests()
        {
            _jsRuntimeMock = new Mock<IJSRuntime>();
            _jsModuleMock = new Mock<IJSObjectReference>();
            
            _jsRuntimeMock
                .Setup(x => x.InvokeAsync<IJSObjectReference>("import", It.Is<object[]>(args => args[0].ToString() == "./js/fileSystem.js")))
                .ReturnsAsync(_jsModuleMock.Object);

            _fileSystemService = new FileSystemService(_jsRuntimeMock.Object);
        }

        [Fact]
        public async Task OpenFileAsync_ShouldReturnFileContent()
        {
            // Arrange
            var expectedContent = new byte[] { 1, 2, 3, 4, 5 };
            _jsModuleMock
                .Setup(x => x.InvokeAsync<byte[]>("openFile", It.IsAny<object[]>()))
                .ReturnsAsync(expectedContent);

            // Act
            var result = await _fileSystemService.OpenFileAsync();

            // Assert
            Assert.Equal(expectedContent, result);
            _jsModuleMock.Verify(x => x.InvokeAsync<byte[]>("openFile", It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task SaveFileAsync_ShouldReturnTrue_WhenSaveSuccessful()
        {
            // Arrange
            var dataToSave = new byte[] { 1, 2, 3 };
            _jsModuleMock
                .Setup(x => x.InvokeAsync<bool>("saveFile", It.Is<object[]>(args => args[0] == dataToSave)))
                .ReturnsAsync(true);

            // Act
            var result = await _fileSystemService.SaveFileAsync(dataToSave);

            // Assert
            Assert.True(result);
            _jsModuleMock.Verify(x => x.InvokeAsync<bool>("saveFile", It.Is<object[]>(args => args[0] == dataToSave)), Times.Once);
        }

        [Fact]
        public async Task SaveFileAsync_ShouldReturnFalse_WhenSaveFails()
        {
            // Arrange
            var dataToSave = new byte[] { 1, 2, 3 };
            _jsModuleMock
                .Setup(x => x.InvokeAsync<bool>("saveFile", It.Is<object[]>(args => args[0] == dataToSave)))
                .ReturnsAsync(false);

            // Act
            var result = await _fileSystemService.SaveFileAsync(dataToSave);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SaveFileAsAsync_ShouldReturnTrue_WhenSaveSuccessful()
        {
            // Arrange
            var dataToSave = new byte[] { 1, 2, 3 };
            _jsModuleMock
                .Setup(x => x.InvokeAsync<bool>("saveFileAs", It.Is<object[]>(args => args[0] == dataToSave)))
                .ReturnsAsync(true);

            // Act
            var result = await _fileSystemService.SaveFileAsAsync(dataToSave);

            // Assert
            Assert.True(result);
            _jsModuleMock.Verify(x => x.InvokeAsync<bool>("saveFileAs", It.Is<object[]>(args => args[0] == dataToSave)), Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_ShouldDisposeModule_WhenModuleIsCreated()
        {
            // Arrange
            // Force module creation by calling a method
            await _fileSystemService.OpenFileAsync();

            // Act
            await _fileSystemService.DisposeAsync();

            // Assert
            _jsModuleMock.Verify(x => x.DisposeAsync(), Times.Once);
        }
    }
} 