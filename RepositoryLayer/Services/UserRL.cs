using CommonLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly FundooContext fundooContext;
        public UserRL(FundooContext fundooContext)
        {
            this.fundooContext = fundooContext;
        }
        public User Registration(UserRegModel userRegModel)
        {
            try
            {
                User newUser = new User();
                newUser.FirstName = userRegModel.FirstName;
                newUser.LastName = userRegModel.LastName;
                newUser.Email = userRegModel.Email;
                newUser.Password = userRegModel.Password;
                fundooContext.UserTables.Add(newUser);
                int result = fundooContext.SaveChanges();
                if (result > 0)
                {
                    return newUser;
                }
                else
                    return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
