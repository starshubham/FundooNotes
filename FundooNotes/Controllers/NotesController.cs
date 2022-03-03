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
    [Authorize]  //user to grant and restrict permissions on Web pages.
    public class NotesController : ControllerBase
    {
        private readonly INoteBL noteBL;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Nbl"></param>
        /// <param name="funContext"></param>
        public NotesController(INoteBL noteBL)
        {
            this.noteBL = noteBL;
        }

        /// <summary>
        /// Create Note api
        /// </summary>
        /// <param name="noteModel"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public IActionResult CreateNote(NoteModel noteModel)
        {
            try
            {
                long userId = Convert.ToInt32(User.Claims.FirstOrDefault(e => e.Type == "Id").Value);
                var note = this.noteBL.CreateNote(noteModel, userId);
                if (note)
                {
                    return this.Ok(new { status = 200, isSuccess = true, message = " Note created successfully ", data = note });
                }
                else
                {
                    return this.BadRequest(new { status = 401, isSuccess = false, message = "Unsuccessful! Notes not Added" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.Message, InnerException = e.InnerException });
            }
        }

        [HttpGet("DisplayAll")]
        public IActionResult GetAllNotes(long userId)
        {
            try
            {
                var notes = noteBL.GetAllNotes(userId);
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

        [HttpGet("{Id}/Display")]
        public IActionResult GetNote(int NotesId)
        {
            try
            {
                long note = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                List<Note> notes = this.noteBL.GetNote(NotesId);
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

        [HttpPut("Update")]
        public IActionResult UpdateNote(NoteModel noteUpdateModel, long NoteId)
        {
            try
            {
                long userid = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var result = this.noteBL.UpdateNote(noteUpdateModel, NoteId);
                if (result != null)
                {
                    return this.Ok(new { isSuccess = true, message = "Notes Updated Successfully", data = result });
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

        [HttpDelete("Delete")]
        public IActionResult DeleteNotes(long NoteId)
        {
            try
            {
                long userid = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var delete = this.noteBL.DeleteNotes(NoteId);
                if (delete != null)
                {
                    return this.Ok(new { isSuccess = true, message = "Notes Deleted Successfully" });
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

        [HttpPut("Archive")]
        public IActionResult ArchiveNote(long NoteId)
        {
            try
            {
                long userid = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var archieve = this.noteBL.ArchiveNote(NoteId);
                return this.Ok(new { isSuccess = true, data = archieve });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.Message, InnerException = e.InnerException });
            }
        }

        [HttpPut("UnArchive")]
        public IActionResult UnArchiveNote(long NoteId)
        {
            try
            {
                long userid = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var unArchieve = this.noteBL.UnArchiveNote(NoteId);
                return this.Ok(new { isSuccess = true, data = unArchieve });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.Message, InnerException = e.InnerException });
            }
        }

        [HttpPut("Pin")]
        public IActionResult PinNote(long NotesId)
        {
            try
            {
                long userId = Convert.ToInt32(User.Claims.FirstOrDefault(p => p.Type == "Id").Value);
                var result = this.noteBL.PinNote(NotesId);
                if (result != null)
                {
                    return this.Ok(new { isSuccess = true, message = result });
                }
                return this.BadRequest(new { isSuccess = false, message = result });
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = ex.InnerException.Message });
            }
        }

        [HttpPut("Trash")]
        public IActionResult TrashNote(long NotesId)
        {
            try
            {
                long userId = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var result = this.noteBL.TrashNote(NotesId);
                if (result != null)
                {
                    return this.Ok(new { isSuccess = true, message = result });
                }
                return this.BadRequest(new { isSuccess = false, message = result });
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = ex.InnerException.Message });
            }
        }

        [HttpPut("Color")]
        public IActionResult NoteColor(long NoteId, string addcolor)
        {
            try
            {
                long userId = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var color = this.noteBL.NoteColor(NoteId, addcolor);
                if (color != null)
                {
                    return this.Ok(new { isSuccess = true, message = "Color Added!", data = color });
                }
                else
                    return this.BadRequest(new { isSuccess = false, message = " Color not Added!" });
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = ex.InnerException.Message });
            }
        }
    }
}
