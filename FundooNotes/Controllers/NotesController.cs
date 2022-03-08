//-----------------------------------------------------------------------
// <copyright file="NotesController.cs" company="CompanyName">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------
namespace FundooNotes.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BusinessLayer.Interfaces;
    using CommonLayer.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using RepositoryLayer.Context;
    using RepositoryLayer.Entities;
    
    /// <summary>
    /// NotesController Class for creating api
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // user to grant and restrict permissions on Web pages.
    public class NotesController : ControllerBase
    {
        private readonly INoteBL noteBL;
        private readonly FundooContext fundooContext;
        private readonly IMemoryCache memoryCache;
        private readonly IDistributedCache distributedCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotesController"/> class.
        /// </summary>
        /// <param name="noteBL">noteBL parameter</param>
        /// <param name="fundooContext">fundooContext parameter</param>
        /// <param name="memoryCache">memoryCache parameter</param>
        /// <param name="distributedCache">distributedCache parameter</param>
        public NotesController(INoteBL noteBL, FundooContext fundooContext, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            this.noteBL = noteBL;
            this.fundooContext = fundooContext;
            this.memoryCache = memoryCache;
            this.distributedCache = distributedCache;
        }

        /// <summary>
        /// API for creating a new note
        /// </summary>
        /// <param name="noteModel">noteModel parameter</param>
        /// <returns>return a note</returns>
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
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for Getting all Notes using UserId
        /// </summary>
        /// <param name="userId">userId parameter</param>
        /// <returns>returns all notes of a user</returns>
        [HttpGet("{UserId}/Get")]
        public IActionResult GetAllNotes(long userId)
        {
            try
            {
                var notes = this.noteBL.GetAllNotes(userId);
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
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for getting all Notes of all users
        /// </summary>
        /// <returns>returns all notes of every user</returns>
        [HttpGet("GetAll")]
        public IActionResult GetEveryoneNotes()
        {
            try
            {
                var notes = this.noteBL.GetEveryoneNotes();
                if (notes != null)
                {
                    return this.Ok(new { isSuccess = true, message = " All notes found Successfully", data = notes });
                }
                else
                {
                    return this.NotFound(new { isSuccess = false, message = "No Notes Found" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for getting all notes using redis cache
        /// </summary>
        /// <returns>returns all notes of all users</returns>
        [HttpGet("Redis")]
        public async Task<IActionResult> GetAllNotesUsingRedisCache()
        {
            var cacheKey = "NotesList";
            string serializedNotesList;
            var NotesList = new List<Note>();
            var redisNotesList = await this.distributedCache.GetAsync(cacheKey);
            if (redisNotesList != null)
            {
                serializedNotesList = Encoding.UTF8.GetString(redisNotesList);
                NotesList = JsonConvert.DeserializeObject<List<Note>>(serializedNotesList);
            }
            else
            {
                NotesList = await this.fundooContext.NotesTable.ToListAsync();  // Comes from Microsoft.EntityFrameworkCore Namespace
                serializedNotesList = JsonConvert.SerializeObject(NotesList);
                redisNotesList = Encoding.UTF8.GetBytes(serializedNotesList);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                await this.distributedCache.SetAsync(cacheKey, redisNotesList, options);
            }
            return this.Ok(NotesList);
        }

        /// <summary>
        /// API for getting a particular note using NoteId
        /// </summary>
        /// <param name="NotesId">NotesId parameter</param>
        /// <returns>returns a note using NotesId</returns>
        [HttpGet("{NotesId}/Get")]
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
                {
                    return this.NotFound(new { isSuccess = false, message = "Specific Note not Found" });
                }                  
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for update note using NoteId
        /// </summary>
        /// <param name="noteUpdateModel">noteUpdateModel parameter</param>
        /// <param name="NoteId">NoteId parameter</param>
        /// <returns>return a updated note</returns>
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
                return this.BadRequest(new { Status = 401, isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for deleting a note using NoteId
        /// </summary>
        /// <param name="NoteId">NoteId parameter</param>
        /// <returns>delete a note</returns>
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
                return this.BadRequest(new { Status = 401, isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for Archive a note
        /// </summary>
        /// <param name="NoteId">NoteId parameter</param>
        /// <returns>archive a note</returns>
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
                return this.BadRequest(new { Status = 401, isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API or Unarchive a note
        /// </summary>
        /// <param name="NoteId">NoteId parameter</param>
        /// <returns>Unarchive a note</returns>
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
                return this.BadRequest(new { Status = 401, isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for pin a note
        /// </summary>
        /// <param name="NotesId">NotesId parameter</param>
        /// <returns>pin a note</returns>
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

        /// <summary>
        /// API for put a note into trash and Untrash
        /// </summary>
        /// <param name="NotesId">NotesId parameter</param>
        /// <returns>put a note into trash and remove from trash</returns>
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

        /// <summary>
        /// API for adding a color
        /// </summary>
        /// <param name="NoteId">NoteId parameter</param>
        /// <param name="addcolor">addcolor parameter</param>
        /// <returns>add a color</returns>
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
                {
                    return this.BadRequest(new { isSuccess = false, message = " Color not Added!" });
                }                   
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = ex.InnerException.Message });
            }
        }

        /// <summary>
        /// API for add Image into a note
        /// </summary>
        /// <param name="imageURL">imageURL parameter</param>
        /// <param name="noteid">noteid parameter</param>
        /// <returns>add image</returns>
        [HttpPut("AddImage")]
        public IActionResult AddBGImage(IFormFile imageURL, long noteid)
        {
            try
            {
                long userId = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var result = this.noteBL.AddBGImage(imageURL, noteid);
                if (result == true)
                {
                    return this.Ok(new { isSuccess = true, message = "BGImage Added Successfully!", data = result });
                }
                else
                {
                    return this.BadRequest(new { isSuccess = false, message = " BGImage not Added!" });
                }                    
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = ex.InnerException.Message });
            }
        }

        /// <summary>
        /// API for remove image from note
        /// </summary>
        /// <param name="noteid">noteid parameter</param>
        /// <returns>remove image</returns>
        [HttpDelete("RemoveImage")]
        public IActionResult DeleteBGImage(long noteid)
        {
            try
            {
                long userid = Convert.ToInt32(User.Claims.FirstOrDefault(e => e.Type == "Id").Value);
                var NoteBgImage = this.fundooContext.NotesTable.Where(x => x.NoteId == noteid).SingleOrDefault();
                if(noteid == 0)
                {
                    return this.NotFound(new { status = 404, isSuccess = false, Message = "Noteid not entered, Please enter a note id!" });
                }
                if(NoteBgImage.UserId == userid)
                {
                    var result = this.noteBL.DeleteBGImage(noteid);
                    if (result)
                    {
                        return this.Ok(new { status = 200, isSuccess = true, Message = "BG image deleted successfully!" });
                    }

                    return this.BadRequest(new { status = 400, isSuccess = false, Message = "Image not deleted" });
                }
                else
                {
                    return this.Unauthorized(new { status = 401, isSuccess = false, Message = "Not logged in" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { status = 400, isSuccess = false, message = e.InnerException.Message });
            }
        }
    }
}
