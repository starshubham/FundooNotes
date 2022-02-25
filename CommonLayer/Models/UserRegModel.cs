using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLayer.Models
{
    /// <summary>
    /// Model class for registering all the details required to (sign up)
    /// </summary>
    public class UserRegModel
    {
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

        public string ConfirmPassword { get; set; }
    }
}
