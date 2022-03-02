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

        [HttpGet("DisplayNotes")]
        public IActionResult GetAllNotes()
        {
            try
            {
                IEnumerable<Note> notes = Nbl.GetAllNotes();
                if (notes != null)
                {
                    return this.Ok(new { status = 200, isSuccess = true, message = " Successful" });

                }
                else
                {
                    return this.NotFound(new { status = 404, isSuccess = false, message = "No Notes Found" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.InnerException.Message });
            }
        }
    }
}
