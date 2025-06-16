using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KnowledgeHub.Shared.Models
{
    public class Dashboard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Widget> Widgets { get; set; } = new List<Widget>();
    }
} 