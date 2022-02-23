using CommonLayer.Models;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        /// <summary>
        /// Interface for User Registration
        /// </summary>
        /// <param name="userRegModel"></param>
        /// <returns></returns>
        public User Registration(UserRegModel userRegModel);

        //public string Login(UserLoginModel userLogin);

        /// <summary>
        /// Interface for UserLogin
        /// </summary>
        /// <param name="userLog"></param>
        /// <returns></returns>
        public LoginResponseModel UserLogin(UserLoginModel userLog);

    }
}
