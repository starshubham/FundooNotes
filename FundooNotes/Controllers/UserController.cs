using BusinessLayer.Interfaces;
using CommonLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL userBL;
        public UserController(IUserBL userBL)
        {
            this.userBL = userBL;
        }
        [HttpPost("Register")]
        public IActionResult addUser(UserRegModel userRegModel)
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
        /// Only for Email Login
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        //[HttpPost("Login")]
        //public IActionResult LoginUser(UserLoginModel userLogin)
        //{
        //    try
        //    {
        //        var result = userBL.Login(userLogin);
        //        if (result != null)
        //        {
        //            return this.Ok(new { success = true, message = "Login Successful", data = result });
        //        }
        //        else
        //            return this.BadRequest(new { success = false, message = "Login Unsuccessful" });
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        [HttpPost("AllLogin")]
        public IActionResult UserLogin(UserLoginModel userLog)
        {
            try
            {
                var result = userBL.UserLogin(userLog);
                if (result != null)
                {
                    return this.Ok(new { success = true, message = "Login Successful", data = result });
                }
                else
                    return this.BadRequest(new { success = false, message = "Login Unsuccessful" });
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
