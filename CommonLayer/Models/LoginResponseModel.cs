using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models
{
    /// <summary>
    /// model class for getting all the data when we enter (Email and Password )
    /// </summary>
    public class LoginResponseModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Token { get; set; }
    }
}
