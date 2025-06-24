using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace KnowledgeHubV2.Services
{
    /// <summary>
    /// Provides methods to interact with the browser's File System Access API
    /// through JavaScript interop. This service is responsible for all file I/O.
    /// </summary>
    public class FileSystemService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemService"/> class.
        /// </summary>
        /// <param name="jsRuntime">The JSRuntime instance for JS interop.</param>
        public FileSystemService(IJSRuntime jsRuntime)
        {
            // Lazily load the JavaScript module to improve startup performance.
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./js/fileSystem.js").AsTask());
        }

        /// <summary>
        /// Prompts the user to select a database file and reads its content.
        /// </summary>
        /// <returns>A byte array representing the file's content, or null if no file was selected.</returns>
        public async Task<byte[]> OpenFileAsync()
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<byte[]>("openFile");
        }

        /// <summary>
        /// Saves the given data to the last opened file. If no file has been opened,
        /// it will prompt the user to select a new file location.
        /// </summary>
        /// <param name="data">The byte array content to save.</param>
        /// <returns>True if the file was saved successfully, false otherwise.</returns>
        public async Task<bool> SaveFileAsync(byte[] data)
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<bool>("saveFile", data);
        }

        /// <summary>
        /// Prompts the user to select a new file location to save the data.
        /// </summary>
        /// <param name="data">The byte array content to save.</param>
        /// <returns>True if the file was saved successfully, false otherwise.</returns>
        public async Task<bool> SaveFileAsAsync(byte[] data)
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<bool>("saveFileAs", data);
        }

        /// <summary>
        /// Disposes of the JavaScript module reference.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
} 