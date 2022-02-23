using CommonLayer.Models;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface IUserBL
    {
        public User Registration(UserRegModel userRegModel);
        //public string Login(UserLoginModel userLogin);
        public LoginResponseModel UserLogin(UserLoginModel userLog);
    }
}
