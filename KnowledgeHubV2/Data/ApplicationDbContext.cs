using KnowledgeHubV2.Models;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeHubV2.Data
{
    /// <summary>
    /// The Entity Framework Core database context for the application.
    /// It provides access to the underlying SQLite database and its tables.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DbSet for Folders.
        /// </summary>
        public DbSet<Folder> Folders { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for Notes.
        /// </summary>
        public DbSet<Note> Notes { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for NoteTables.
        /// </summary>
        public DbSet<NoteTable> NoteTables { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for NoteTableColumns.
        /// </summary>
        public DbSet<NoteTableColumn> NoteTableColumns { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Configures the model relationships for the database context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Note and Folder relationship
            modelBuilder.Entity<Folder>()
                .HasMany(f => f.ChildFolders)
                .WithOne(f => f.ParentFolder)
                .HasForeignKey(f => f.ParentFolderID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Folder>()
                .HasMany(f => f.Notes)
                .WithOne(n => n.Folder)
                .HasForeignKey(n => n.FolderID)
                .OnDelete(DeleteBehavior.SetNull);

            // Note and NoteTable relationship
            modelBuilder.Entity<Note>()
                .HasMany(n => n.Tables)
                .WithOne(t => t.Note)
                .HasForeignKey(t => t.NoteID)
                .OnDelete(DeleteBehavior.Cascade);
            
            // NoteTable and NoteTableColumn relationship
            modelBuilder.Entity<NoteTable>()
                .HasMany(t => t.Columns)
                .WithOne(c => c.NoteTable)
                .HasForeignKey(c => c.NoteTableID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 