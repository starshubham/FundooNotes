using BusinessLayer.Interfaces;
using CommonLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INoteBL Nbl;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Nbl"></param>
        /// <param name="funContext"></param>
        public NotesController(INoteBL Nbl, FundooContext funContext)
        {
            this.Nbl = Nbl;
        }

        /// <summary>
        /// Create Note api
        /// </summary>
        /// <param name="noteModel"></param>
        /// <returns></returns>
        [HttpPost("CreateNote")]
        public IActionResult CreateNote(NoteModel noteModel)
        {
            try
            {
                long userId = Convert.ToInt32(User.Claims.FirstOrDefault(e => e.Type == "Id").Value);
                if (this.Nbl.CreateNote(noteModel, userId))
                {
                    return this.Ok(new { status = 200, isSuccess = true, message = " Note created successfully " });
                }
                else
                {
                    return this.BadRequest(new { status = 401, isSuccess = false, message = "Unsuccessful! Notes not Added" });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("DisplayAllNotes")]
        public IActionResult GetAllNotes(long userId)
        {
            try
            {
                var notes = Nbl.GetAllNotes(userId);
                if (notes != null)
                {
                    return this.Ok(new { isSuccess = true, message = " All notes found Successfully", data = notes});

                }
                else
                {
                    return this.NotFound(new { isSuccess = false, message = "No Notes Found" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.Message, InnerException= e.InnerException });
            }
        }

        [HttpGet("DisplaySpecificNote")]
        public IActionResult GetNote(int NotesId)
        {
            try
            {
                long note = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                List<Note> notes = this.Nbl.GetNote(NotesId);
                if (notes != null)
                {
                    return this.Ok(new { isSuccess = true, message = " Specific Note found Successfully", data = notes });
                }
                else
                    return this.NotFound(new { isSuccess = false, message = "Specific Note not Found" });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.Message, InnerException = e.InnerException });
            }
        }

        [HttpPut("UpdateNote")]
        public IActionResult UpdateNote(NoteModel noteUpdateModel)
        {
            try
            {
                long userid = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var result = this.Nbl.UpdateNote(noteUpdateModel, userid);
                if (result != null)
                {
                    return this.Ok(new { success = true, message = "Notes Updated Successfully", data = result });
                }
                else
                {
                    return this.NotFound(new { isSuccess = false, message = "No Notes Found" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.Message, InnerException = e.InnerException });
            }
        }

        [HttpDelete("DeleteNotes")]
        public IActionResult DeleteNotes(long NoteId)
        {
            try
            {
                long userid = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var delete = this.Nbl.DeleteNotes(NoteId);
                if (delete != null)
                {
                    return this.Ok(new { success = true, message = "Notes Deleted Successfully" });
                }
                else
                {
                    return this.NotFound(new { isSuccess = false, message = "No Notes Deleted" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.Message, InnerException = e.InnerException });
            }
        }
    }
}
