using System.ComponentModel.DataAnnotations;

namespace KnowledgeHub.Shared.Models
{
    public class Widget
    {
        [Key]
        public int Id { get; set; }

        public WidgetType WidgetType { get; set; }

        public string SettingsAsJson { get; set; }

        public int DashboardId { get; set; }
        public Dashboard Dashboard { get; set; }
    }

    public enum WidgetType
    {
        NotePreview,
        TablePreview,
        TableAggregate,
        QuickStats
    }
} 