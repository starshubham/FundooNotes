using BusinessLayer.Interfaces;
using CommonLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollabsController : ControllerBase
    {
        private readonly ICollabBL collabBL;
        private readonly FundooContext fundooContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collabBL"></param>
        public CollabsController(ICollabBL collabBL, FundooContext fundooContext)
        {
            this.collabBL = collabBL;
            this.fundooContext = fundooContext;
        }

        [HttpPost("AddCollab")]
        public IActionResult AddCollab(CollabModel collabModel)
        {
            try
            {
                long userId = Convert.ToInt32(User.Claims.FirstOrDefault(e => e.Type == "Id").Value);
                var collab = this.fundooContext.NotesTable.Where(X => X.NoteId == collabModel.NoteId).SingleOrDefault();
                if (collab.UserId == userId)
                {
                    var result = this.collabBL.AddCollab(collabModel);
                    if (result)
                    {
                        return this.Ok(new { status = 200, isSuccess = true, message = "Collaboration stablish successfully", data = result });
                    }
                    return this.BadRequest(new { status = 400, isSucess = false, message = "Failed to stablish collaboration" });
                }
                else
                {
                    return this.Unauthorized(new { status = 401, isSucess = false, Message = "Not authorized to add collaboration" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.Message, InnerException = e.InnerException });
            }
        }
    }
}
