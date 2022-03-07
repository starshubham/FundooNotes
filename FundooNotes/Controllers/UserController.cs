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
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]  // Route is for matching incoming HTTP requests.
    [ApiController]  // To Enable Routing Requirements.
    public class UserController : ControllerBase
    {
        private readonly IUserBL userBL;  // can only be assigned a value from within the constructor(s) of a class.
        private readonly FundooContext fundooContext;
        private readonly IMemoryCache memoryCache;
        private readonly IDistributedCache distributedCache;

        public UserController(IUserBL userBL, FundooContext fundooContext, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            this.userBL = userBL;
            this.fundooContext = fundooContext;
            this.memoryCache = memoryCache;
            this.distributedCache = distributedCache;
        }
        [HttpPost("Register")]
        public IActionResult addUser(UserRegModel userRegModel)  //IActionResult lets you return both data and HTTP codes.
        {
            try
            {
                var result = userBL.Registration(userRegModel);
                if (result != null)
                {
                    return this.Ok(new { success = true, message = "Registration Successful", data = result });
                }
                else
                    return this.BadRequest(new { success = false, message = "Registration Unsuccessful" });
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get all Login Data
        /// </summary>
        /// <param name="userLog"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public IActionResult UserLogin(UserLoginModel userLog)
        {
            try
            {
                var result = userBL.UserLogin(userLog);
                if (result != null)
                {
                    return this.Ok(new { isSuccess = true, message = "Login Successful", data = result });
                }
                else
                    return this.BadRequest(new { isSuccess = false, message = "Login Unsuccessful" });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = userBL.GetAllUsers();
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
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.InnerException.Message });
            }
        }

        [HttpGet("redis")]
        public async Task<IActionResult> GetAllUsersUsingRedisCache()
        {
            var cacheKey = "UsersList";
            string serializedUsersList;
            var UsersList = new List<User>();
            var redisUsersList = await distributedCache.GetAsync(cacheKey);
            if (redisUsersList != null)
            {
                serializedUsersList = Encoding.UTF8.GetString(redisUsersList);
                UsersList = JsonConvert.DeserializeObject<List<User>>(serializedUsersList);
            }
            else
            {
                UsersList = await fundooContext.UserTables.ToListAsync();  // Comes from Microsoft.EntityFrameworkCore Namespace
                serializedUsersList = JsonConvert.SerializeObject(UsersList);
                redisUsersList = Encoding.UTF8.GetBytes(serializedUsersList);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                await distributedCache.SetAsync(cacheKey, redisUsersList, options);
            }
            return Ok(UsersList);
        }

        [HttpPost("ForgotPassword")]
        public IActionResult ForgotPassword(string email)
        {
            try
            {
                var result = userBL.ForgetPassword(email);
                if (result != null)
                {
                    return this.Ok(new { isSuccess = true, message = "Send Forget Password Link"});
                }
                else
                    return this.BadRequest(new { isSuccess = false, message = "Email not Found" });
            }
            catch (Exception e)
            {

                return this.BadRequest(new { isSuccess = false, message = e.InnerException.Message });
            }
        }

        [Authorize]
        [HttpPost("ResetPassword")]
        
        public IActionResult ResetPassword(string password, string confirmPassword)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email).Value.ToString();
                var result = userBL.ResetPassword(email, password, confirmPassword);
                return this.Ok(new { isSuccess = true, message = "Reset Password Successfully" });
                            
            }
            catch (Exception e)
            {

                return this.BadRequest(new { isSuccess = false, message = e.InnerException.Message });
            }
        }
    }
}
