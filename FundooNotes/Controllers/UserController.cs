using BusinessLayer.Interfaces;
using CommonLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]  // Route is for matching incoming HTTP requests.
    [ApiController]  // To Enable Routing Requirements.
    public class UserController : ControllerBase
    {
        private readonly IUserBL userBL;  // can only be assigned a value from within the constructor(s) of a class.
        public UserController(IUserBL userBL)
        {
            this.userBL = userBL;
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
