using Microsoft.EntityFrameworkCore;
using KnowledgeHub.Shared.Models;

namespace KnowledgeHub.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Folder> Folders { get; set; }
        public DbSet<Note> Notes { get; set; }
    }
}