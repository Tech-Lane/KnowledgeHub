using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Linq;

namespace KnowledgeHubV2.Models
{
    /// <summary>
    /// Represents a table of data associated with a note.
    /// </summary>
    public class NoteTable
    {
        /// <summary>
        /// The unique identifier for the table.
        /// </summary>
        [Key]
        public int NoteTableID { get; set; }

        /// <summary>
        /// The name of the table.
        /// </summary>
        [Required(ErrorMessage = "Table name is required.")]
        [StringLength(50, ErrorMessage = "Table name cannot be longer than 50 characters.")]
        public string Name { get; set; } = "";

        /// <summary>
        /// The ID of the note this table belongs to.
        /// </summary>
        public int NoteID { get; set; }

        /// <summary>
        /// Navigation property for the parent note.
        /// </summary>
        [ForeignKey("NoteID")]
        public virtual Note Note { get; set; } = null!;

        /// <summary>
        /// A collection of column definitions for this table.
        /// </summary>
        public List<NoteTableColumn> Columns { get; set; } = new();

        /// <summary>
        /// The rows of data for this table, stored as a JSON string.
        /// Each row is a dictionary of [ColumnID, Value].
        /// Example: "[{\"1\":\"Some Text\",\"2\":123},{\"1\":\"More Text\",\"2\":456}]"
        /// </summary>
        public string RowsJson { get; set; } = "[]";
        
        [NotMapped]
        public List<Dictionary<string, object>> Rows
        {
            get => JsonSerializer.Deserialize<List<Dictionary<string, object>>>(RowsJson) ?? new List<Dictionary<string, object>>();
            set => RowsJson = JsonSerializer.Serialize(value);
        }

        public void AddRow(Dictionary<string, object> newRow)
        {
            var currentRows = Rows;
            // Ensure all columns exist in the new row dictionary
            foreach (var col in Columns)
            {
                if (!newRow.ContainsKey(col.Name))
                {
                    newRow[col.Name] = GetDefaultValue(col.DataType);
                }
            }
            currentRows.Add(newRow);
            Rows = currentRows;
        }

        private object GetDefaultValue(ColumnDataType dataType)
        {
            return dataType switch
            {
                ColumnDataType.Number => 0,
                ColumnDataType.Date => System.DateTime.Now,
                ColumnDataType.Boolean => false,
                _ => ""
            };
        }
    }
} 