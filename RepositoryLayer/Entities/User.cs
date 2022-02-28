using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{1}[a-z]{2,}$")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{1}[a-z]{2,}$")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression("^[a-z]{3,}[.]*[a-z0-9]*[@]{1}[a-z]{2,}[.]{1}[co]{2}[m]*[.]*[a-z]*$", ErrorMessage = "Enter a valid email.")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{1}[A-Z a-z]{3,}[!*@#$%^&+=]?[0-9]{1,}$")]
        public string Password { get; set; }

        public DateTime? CreatedAt { get; set; }  // ? Allow the Nullable value
        public DateTime? ModifiedAt { get; set; }

        public ICollection<Note> NotesTable { get; set; }
    }
}
