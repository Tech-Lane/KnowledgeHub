namespace KnowledgeHubV2.Models
{
    public interface IFileSystemItem
    {
        public int ID { get; }
        public string Name { get; }
        public int? ParentID { get; }
    }
} 