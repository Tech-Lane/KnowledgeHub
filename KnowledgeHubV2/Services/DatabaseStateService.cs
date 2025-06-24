using System;
using System.IO;
using System.Threading.Tasks;
using KnowledgeHubV2.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace KnowledgeHubV2.Services
{
    /// <summary>
    /// Manages the state of the in-memory SQLite database.
    /// It handles creating, loading, and saving the database file,
    /// and provides a single point of access to the database connection.
    /// </summary>
    public class DatabaseStateService : IAsyncDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly ILogger<DatabaseStateService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private SqliteConnection? _connection;

        /// <summary>
        /// Event that is fired when the database has been loaded or created,
        /// signaling that the application is ready to be used.
        /// </summary>
        public event Action? OnDatabaseReady;
        
        /// <summary>
        /// Gets a value indicating whether a database has been loaded.
        /// </summary>
        public bool IsDatabaseLoaded { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseStateService"/> class.
        /// </summary>
        public DatabaseStateService(IServiceProvider serviceProvider, IDbContextFactory<ApplicationDbContext> dbContextFactory, ILogger<DatabaseStateService> logger, IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Opens a user-selected database file, loads it into memory, and applies migrations.
        /// </summary>
        public async Task LoadDatabaseAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var fileSystemService = scope.ServiceProvider.GetRequiredService<FileSystemService>();
            var dbBytes = await fileSystemService.OpenFileAsync();
            if (dbBytes == null) return;

            await InitializeDatabase(dbBytes);
        }

        /// <summary>
        /// Creates a new, empty in-memory database and applies migrations.
        /// </summary>
        public async Task CreateNewDatabaseAsync()
        {
            await InitializeDatabase(null);
        }

        /// <summary>
        /// Saves the current in-memory database to a file on the user's disk.
        /// This works by backing up the in-memory DB to a temporary file in the virtual filesystem,
        /// reading the bytes of that file, and then passing those bytes to the FileSystemService.
        /// </summary>
        public async Task SaveDatabaseAsync()
        {
            if (_connection == null) return;

            using var scope = _serviceProvider.CreateScope();
            var fileSystemService = scope.ServiceProvider.GetRequiredService<FileSystemService>();

            string tempPath = Path.GetTempFileName();
            try
            {
                using (var tempFileConnection = new SqliteConnection($"Data Source={tempPath}"))
                {
                    await tempFileConnection.OpenAsync();
                    // Backup from the main in-memory connection to the temporary file-based connection
                    _connection.BackupDatabase(tempFileConnection);
                } // The temp file is now written and the connection is closed.

                var dbBytes = await File.ReadAllBytesAsync(tempPath);
                await fileSystemService.SaveFileAsync(dbBytes);
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        /// <summary>
        /// Initializes the shared in-memory SQLite database from a byte array or creates a new one.
        /// </summary>
        /// <param name="dbBytes">The byte array of an existing .db file, or null to create a new database.</param>
        private async Task InitializeDatabase(byte[]? dbBytes)
        {
            // The connection string uses a shared in-memory data source.
            // This connection must be kept open for the lifetime of the application
            // for the in-memory database to persist.
            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
            _connection = new SqliteConnection("DataSource=file:memdb1?mode=memory&cache=shared");
            await _connection.OpenAsync();

            if (dbBytes != null && dbBytes.Length > 0)
            {
                // To load an existing DB into an in-memory one, we write the bytes to a temporary
                // file in the virtual filesystem and then use the Backup API to copy it.
                string tempPath = Path.GetTempFileName();
                try
                {
                    await File.WriteAllBytesAsync(tempPath, dbBytes);
                    using (var tempFileConnection = new SqliteConnection($"Data Source={tempPath}"))
                    {
                        await tempFileConnection.OpenAsync();
                        // Backup from the temporary file-based connection to the main in-memory connection
                        tempFileConnection.BackupDatabase(_connection);
                    }
                }
                finally
                {
                    if(File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
            }

            // Use the DbContext to ensure the schema is created (applies migrations)
            using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
            {
                await dbContext.Database.EnsureCreatedAsync();
            }

            IsDatabaseLoaded = true;
            NotifyReady();
        }

        /// <summary>
        /// Disposes the shared database connection.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
        }

        public async Task<SqliteConnection> GetConnectionAsync()
        {
            if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
            {
                _logger.LogWarning("Connection was requested before it was ready. Creating a new database.");
                await CreateNewDatabaseAsync(); // Try to initialize again by creating a new DB.
            }
            
            if (_connection == null)
            {
                _logger.LogError("Failed to establish a database connection.");
                throw new InvalidOperationException("Failed to establish a database connection.");
            }

            return _connection;
        }

        private void NotifyReady()
        {
            IsDatabaseLoaded = true;
            _logger.LogInformation("Database is ready.");
            OnDatabaseReady?.Invoke();
        }

        private async Task<bool> IsDatabaseDownloaded()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                using var request = new HttpRequestMessage(HttpMethod.Head, "db/main.db");
                var response = await client.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
} 