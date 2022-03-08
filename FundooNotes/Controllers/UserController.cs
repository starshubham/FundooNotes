//-----------------------------------------------------------------------
// <copyright file="UserController.cs" company="CompanyName">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------

namespace FundooNotes.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using BusinessLayer.Interfaces;
    using CommonLayer.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using RepositoryLayer.Context;
    using RepositoryLayer.Entities;
    
    /// <summary>
    /// UserController class
    /// </summary>
    [Route("api/[controller]")]  // Route is for matching incoming HTTP requests.
    [ApiController]  // To Enable Routing Requirements.
    public class UserController : ControllerBase
    {
        // can only be assigned a value from within the constructor(s) of a class.
        private readonly IUserBL userBL;  
        private readonly FundooContext fundooContext;
        private readonly IDistributedCache distributedCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userBL">userBL parameter</param>
        /// <param name="fundooContext">fundooContext parameter</param>
        /// <param name="distributedCache">distributedCache parameter</param>
        public UserController(IUserBL userBL, FundooContext fundooContext, IDistributedCache distributedCache)
        {
            this.userBL = userBL;
            this.fundooContext = fundooContext;
            this.distributedCache = distributedCache;
        }

        /// <summary>
        /// API for Register a User
        /// </summary>
        /// <param name="userRegModel">userRegModel parameter</param>
        /// <returns>returns registered user</returns>
        [HttpPost("Register")]
        public IActionResult Registration(UserRegModel userRegModel)  // IActionResult lets you return both data and HTTP codes.
        {
            try
            {
                var result = this.userBL.Registration(userRegModel);
                if (result != null)
                {
                    return this.Ok(new { success = true, message = "Registration Successful", data = result });
                }
                else
                {
                    return this.BadRequest(new { success = false, message = "Registration Unsuccessful" });
                }                  
            }
            catch (Exception e)
            {
                return this.BadRequest(new { isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for Get all Login Data
        /// </summary>
        /// <param name="userLog">userLog parameter</param>
        /// <returns>returns login data</returns>
        [HttpPost("Login")]
        public IActionResult UserLogin(UserLoginModel userLog)
        {
            try
            {
                var result = this.userBL.UserLogin(userLog);
                if (result != null)
                {
                    return this.Ok(new { isSuccess = true, message = "Login Successful", data = result.Token });
                }
                else
                {
                    return this.BadRequest(new { isSuccess = false, message = "Login Unsuccessful" });
                }                  
            }
            catch (Exception e)
            {
                return this.BadRequest(new { isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for get all the users
        /// </summary>
        /// <returns>returns all users</returns>
        [HttpGet("GetAll")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = this.userBL.GetAllUsers();
                if (users != null)
                {
                    return this.Ok(new { isSuccess = true, message = " All users found Successfully", data = users });
                }
                else
                {
                    return this.NotFound(new { isSuccess = false, message = "No user Found" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for Redis Cache
        /// </summary>
        /// <returns>returns all users</returns>
        [HttpGet("Redis")]
        public async Task<IActionResult> GetAllUsersUsingRedisCache()
        {
            var cacheKey = "UsersList";
            string serializedUsersList;
            var usersList = new List<User>();
            var redisUsersList = await this.distributedCache.GetAsync(cacheKey);
            if (redisUsersList != null)
            {
                serializedUsersList = Encoding.UTF8.GetString(redisUsersList);
                usersList = JsonConvert.DeserializeObject<List<User>>(serializedUsersList);
            }
            else
            {
                usersList = await this.fundooContext.UserTables.ToListAsync();  // Comes from Microsoft.EntityFrameworkCore Namespace
                serializedUsersList = JsonConvert.SerializeObject(usersList);
                redisUsersList = Encoding.UTF8.GetBytes(serializedUsersList);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                await this.distributedCache.SetAsync(cacheKey, redisUsersList, options);
            }

            return this.Ok(usersList);
        }

        /// <summary>
        /// API for Forget Password
        /// </summary>
        /// <param name="email">email parameter</param>
        /// <returns>returns a token</returns>
        [HttpPost("ForgotPassword")]
        public IActionResult ForgotPassword(string email)
        {
            try
            {
                var result = this.userBL.ForgetPassword(email);
                if (result != null)
                {
                    return this.Ok(new { isSuccess = true, message = "Send Forget Password Link" });
                }
                else
                {
                    return this.BadRequest(new { isSuccess = false, message = "Email not Found" });
                }                  
            }
            catch (Exception e)
            {
                return this.BadRequest(new { isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for Reset Password
        /// </summary>
        /// <param name="password">password parameter</param>
        /// <param name="confirmPassword">confirmPassword parameter</param>
        /// <returns>returns updated password</returns>
        [Authorize]
        [HttpPost("ResetPassword")]       
        public IActionResult ResetPassword(string password, string confirmPassword)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email).Value.ToString();
                var result = this.userBL.ResetPassword(email, password, confirmPassword);
                return this.Ok(new { isSuccess = true, message = "Reset Password Successfully" });                          
            }
            catch (Exception e)
            {
                return this.BadRequest(new { isSuccess = false, message = e.InnerException.Message });
            }
        }
    }
}
