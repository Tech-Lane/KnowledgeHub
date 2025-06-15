using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeHub.Shared.Models
{
    public class TreeItem
    {
        public string Id { get; set; }
        public string Type { get; set; } // "folder" or "note"
        public string Name { get; set; }
        public object? Data { get; set; }
        public List<TreeItem> Children { get; set; } = new();
    }
}
