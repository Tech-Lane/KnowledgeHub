using System.ComponentModel.DataAnnotations;

namespace KnowledgeHub.Shared.Models
{
    public class Table
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int NoteId { get; set; }

        public Note Note { get; set; }

        public string ColumnsAsJson { get; set; } // Storing column definitions as a JSON string

        public string DataAsJson { get; set; } // Storing table data as a JSON string
    }
} 