using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnowledgeHubV2.Models
{
    /// <summary>
    /// Represents a folder in the knowledge hub, which can contain notes and other folders.
    /// </summary>
    public class Folder : BaseEntity, IFileSystemItem
    {
        /// <summary>
        /// The unique identifier for the folder.
        /// </summary>
        [Key]
        public int FolderID { get; set; }

        /// <summary>
        /// The name of the folder.
        /// </summary>
        [Required(ErrorMessage = "Folder name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the parent folder. A null value indicates a root folder.
        /// </summary>
        public int? ParentFolderID { get; set; }

        /// <summary>
        /// Navigation property for the parent folder.
        /// </summary>
        [ForeignKey("ParentFolderID")]
        public virtual Folder? ParentFolder { get; set; }

        /// <summary>
        /// A collection of child folders contained within this folder.
        /// </summary>
        public virtual ICollection<Folder> ChildFolders { get; set; } = new List<Folder>();

        /// <summary>
        /// A collection of notes contained within this folder.
        /// </summary>
        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

        // Explicit interface implementation
        public int ID => FolderID;
        public int? ParentID => ParentFolderID;
    }
} 