﻿using CommonLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RepositoryLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly FundooContext fundooContext; //context class is used to query or save data to the database.
        IConfiguration _Appsettings;  //IConfiguration interface is used to read Settings and Connection Strings from AppSettings.
        public UserRL(FundooContext fundooContext, IConfiguration Appsettings)
        {
            this.fundooContext = fundooContext;
            _Appsettings = Appsettings;
        }

        /// <summary>
        /// User Registration
        /// </summary>
        /// <param name="userRegModel"></param>
        /// <returns></returns>
        public User Registration(UserRegModel userRegModel)
        {
            try
            {
                User newUser = new User();
                newUser.FirstName = userRegModel.FirstName;
                newUser.LastName = userRegModel.LastName;
                newUser.Email = userRegModel.Email;
                newUser.Password = EncryptPassword(userRegModel.Password);
                newUser.CreatedAt = DateTime.Now;

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

        /// <summary>
        /// Method for EncryptPassword
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string EncryptPassword(string password)
        {
            string enteredpassword = "";
            byte[] hide = new byte[password.Length];
            hide = Encoding.UTF8.GetBytes(password);
            enteredpassword = Convert.ToBase64String(hide);
            return enteredpassword;
        }

        /// <summary>
        /// Method for DecryptPassword
        /// </summary>
        /// <param name="encryptpwd"></param>
        /// <returns></returns>
        private string DecryptPassword(string encryptpwd)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }

        /// <summary>
        /// Show All Registerd Login Data
        /// </summary>
        /// <param name="userLog"></param>
        /// <returns></returns>
        public LoginResponseModel UserLogin(UserLoginModel userLog)
        {
            try
            {
                var existingLogin = this.fundooContext.UserTables.Where(X => X.Email == userLog.Email).FirstOrDefault();
                if (DecryptPassword(existingLogin.Password) == userLog.Password)
                {
                    LoginResponseModel login = new LoginResponseModel();
                    string token = GenerateSecurityToken(existingLogin.Email, existingLogin.Id);
                    login.Id = existingLogin.Id;
                    login.FirstName = existingLogin.FirstName;
                    login.LastName = existingLogin.LastName;
                    login.Email = existingLogin.Email;
                    login.Password = existingLogin.Password;
                    login.CreatedAt = existingLogin.CreatedAt;
                    login.Token = token;
                    
                    return login;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Method for Generating Security Token
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        private string GenerateSecurityToken(string Email, long Id)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Appsettings["Jwt:SecKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(ClaimTypes.Email,Email),
                new Claim("Id",Id.ToString())
            };
            var token = new JwtSecurityToken(_Appsettings["Jwt:Issuer"],
              _Appsettings["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(60),
              signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Method for forget password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string ForgetPassword(string email)
        {
            try
            {
                var existingLogin = this.fundooContext.UserTables.Where(X => X.Email == email).FirstOrDefault();
                if (existingLogin != null)
                {
                    var token = GenerateSecurityToken(email, existingLogin.Id);
                    new MSMQ_Model().MSMQSender(token);
                    return token;
                }
                else
                    return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Method for Reset password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <returns></returns>
        public bool ResetPassword(string email, string password, string confirmPassword)
        {
            try
            {
                if (password.Equals(confirmPassword))
                {
                    User user = fundooContext.UserTables.Where(e => e.Email == email).FirstOrDefault();
                    user.Password = confirmPassword;
                    fundooContext.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Method for Getting all users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetAllUsers()
        {
            try
            {
                var result = this.fundooContext.UserTables.ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
