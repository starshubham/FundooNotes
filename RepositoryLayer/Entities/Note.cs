using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RepositoryLayer.Entities
{
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long NoteId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsPinned { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsArchieved { get; set; }
        public DateTime? Reminder { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string Color { get; set; }
        public string BGImage { get; set; }

        //foreign Key
        public User User { get; set; }
    }
}
