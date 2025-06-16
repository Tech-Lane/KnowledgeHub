using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeHub.Shared.Models
{
    public class TreeItem
    {
        public required string Id { get; set; }
        public required string Type { get; set; } // "folder" or "note"
        public required string Name { get; set; }
        public object? Data { get; set; }
        public List<TreeItem> Children { get; set; } = new();
    }
}
