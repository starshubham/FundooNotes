using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RepositoryLayer.Entities
{
    public class Collaborator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CollabID { get; set; }
        public string CollabEmail { get; set; }

        [ForeignKey("user")]
        public long UserId { get; set; }
        public virtual User user { get; set; }

        [ForeignKey("note")]
        public long NoteId { get; set; }
        public virtual Note note { get; set; }
    }
}
