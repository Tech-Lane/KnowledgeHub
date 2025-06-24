using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace KnowledgeHubV2.Models
{
    /// <summary>
    /// Represents a single note, which contains content and can have associated data tables.
    /// </summary>
    public class Note : BaseEntity, IFileSystemItem
    {
        /// <summary>
        /// The unique identifier for the note.
        /// </summary>
        [Key]
        public int NoteID { get; set; }

        /// <summary>
        /// The title of the note.
        /// </summary>
        [Required(ErrorMessage = "Note title is required.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The main content of the note, can contain rich text and references.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the folder that contains this note. Null if it's a root note.
        /// </summary>
        public int? FolderID { get; set; }

        /// <summary>
        /// Navigation property for the parent folder.
        /// </summary>
        [ForeignKey("FolderID")]
        public virtual Folder? Folder { get; set; }

        /// <summary>
        /// A collection of data tables associated with this note.
        /// </summary>
        public virtual ICollection<NoteTable> Tables { get; set; } = new List<NoteTable>();

        // Explicit interface implementation
        public int ID => NoteID;
        public string Name => Title;
        public int? ParentID => FolderID;
    }
} 