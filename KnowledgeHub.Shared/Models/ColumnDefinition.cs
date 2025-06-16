namespace KnowledgeHub.Shared.Models
{
    public class ColumnDefinition
    {
        public string Name { get; set; } = "";
        public ColumnDataType DataType { get; set; } = ColumnDataType.Text;
    }
} 