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
        private readonly FundooContext fundooContext;
        IConfiguration _Appsettings;
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

        ///// <summary>
        ///// show only Email
        ///// </summary>
        ///// <param name="userLogin"></param>
        ///// <returns></returns>
        //public string Login(UserLoginModel userLogin)
        //{
        //    var LoginResult = this.fundooContext.UserTables.Where(X => X.Email == userLogin.Email && X.Password == userLogin.Password).FirstOrDefault();
        //    if (LoginResult != null)
        //    {
        //        return LoginResult.Email;
        //    }
        //    else
        //        return null;
        //}

        /// <summary>
        /// Show All Registerd Login Data
        /// </summary>
        /// <param name="userLog"></param>
        /// <returns></returns>
        public LoginResponseModel UserLogin(UserLoginModel userLog)
        {
            try
            {
                var existingLogin = this.fundooContext.UserTables.Where(X => X.Email == userLog.Email && X.Password == userLog.Password).FirstOrDefault();
                if (existingLogin != null)
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
        /// Generating Security Token
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
    }
}
