using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models
{
    public class UserRegModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
