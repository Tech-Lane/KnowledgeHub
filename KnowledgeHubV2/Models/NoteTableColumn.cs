using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnowledgeHubV2.Models
{
    /// <summary>
    /// Defines the type of data a column can hold.
    /// </summary>
    public enum ColumnDataType
    {
        Text,
        Number,
        Date,
        Boolean
    }

    /// <summary>
    /// Represents a single column in a NoteTable.
    /// </summary>
    public class NoteTableColumn
    {
        /// <summary>
        /// The unique identifier for the column.
        /// </summary>
        [Key]
        public int ColumnID { get; set; }

        /// <summary>
        /// The name of the column header.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The data type that this column stores (e.g., Text, Number).
        /// </summary>
        public ColumnDataType DataType { get; set; }

        /// <summary>
        /// The ID of the table this column belongs to.
        /// </summary>
        public int NoteTableID { get; set; }

        /// <summary>
        /// Navigation property for the parent table.
        /// </summary>
        [ForeignKey("NoteTableID")]
        public virtual NoteTable NoteTable { get; set; } = null!;
    }
} 