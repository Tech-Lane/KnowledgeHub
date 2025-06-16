using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeHub.Shared.Models
{
    public class Folder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Folder name cannot be longer than 100 characters.")]
        public required string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
