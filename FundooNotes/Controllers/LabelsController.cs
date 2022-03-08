//-----------------------------------------------------------------------
// <copyright file="LabelsController.cs" company="CompanyName">
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
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using RepositoryLayer.Context;
    using RepositoryLayer.Entities;   

    /// <summary>
    /// LabelsController connected with BaseController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // user to grant and restrict permissions on Web pages.
    public class LabelsController : ControllerBase
    {
        private readonly ILabelBL labelBL;
        private readonly FundooContext fundooContext;
        private readonly IDistributedCache distributedCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelsController"/> class.
        /// </summary>
        /// <param name="labelBL">labelBL parameter</param>
        /// <param name="fundooContext">fundooContext parameter</param>
        /// <param name="distributedCache">distributedCache parameter</param>
        public LabelsController(ILabelBL labelBL, FundooContext fundooContext, IDistributedCache distributedCache)
        {
            this.labelBL = labelBL;
            this.fundooContext = fundooContext;
            this.distributedCache = distributedCache;
        }

        /// <summary>
        /// API for adding a new label
        /// </summary>
        /// <param name="labelModel">labelModel parameter</param>
        /// <returns>returns a new label</returns>
        [HttpPost("Create")]
        public IActionResult AddLabel(LabelModel labelModel)  // IActionResult -- how the server should respond to the request.
        {
            try
            {
                // checking if the user has a claim to access.
                long userid = Convert.ToInt32(User.Claims.FirstOrDefault(e => e.Type == "Id").Value);
                var labelNote = this.fundooContext.NotesTable.Where(x => x.NoteId == labelModel.NoteId).SingleOrDefault();
                if (labelNote.UserId == userid)
                {
                    var result = this.labelBL.AddLabel(labelModel);
                    if (result)
                    {
                        return this.Ok(new { status = 200, isSuccess = true, Message = "Label created successfully!", data = result });
                    }
                    else
                    {
                        return this.BadRequest(new { status = 400, isSuccess = false, message = "Label not created" });
                    }                   
                }

                return this.Unauthorized(new { status = 401, isSuccess = false, message = "Unauthorized User!" });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { status = 400, isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for getting all labels
        /// </summary>
        /// <returns>returns all labels</returns>
        [HttpGet("GetAll")]
        public IActionResult GetAllLabels()
        {
            try
            {
                var labels = this.labelBL.GetAllLabels();
                if (labels != null)
                {
                    return this.Ok(new { status = 200, isSuccess = true, Message = " All labels found Successfully", data = labels });
                }
                else
                {
                    return this.NotFound(new { isSuccess = false, Message = "No label found" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for Getting all notes using redis cache
        /// </summary>
        /// <returns>returns all notes</returns>
        [HttpGet("Redis")]
        public async Task<IActionResult> GetAllNotesUsingRedisCache()
        {
            var cacheKey = "LabelsList";
            string serializedLabelsList;
            var labelsList = new List<Label>();
            var redisLabelsList = await this.distributedCache.GetAsync(cacheKey);
            if (redisLabelsList != null)
            {
                serializedLabelsList = Encoding.UTF8.GetString(redisLabelsList);
                labelsList = JsonConvert.DeserializeObject<List<Label>>(serializedLabelsList);
            }
            else
            {
                labelsList = await this.fundooContext.LabelsTable.ToListAsync();  // Comes from Microsoft.EntityFrameworkCore Namespace
                serializedLabelsList = JsonConvert.SerializeObject(labelsList);
                redisLabelsList = Encoding.UTF8.GetBytes(serializedLabelsList);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                await this.distributedCache.SetAsync(cacheKey, redisLabelsList, options);
            }

            return this.Ok(labelsList);
        }

        /// <summary>
        /// API for Get Labels by noteId
        /// </summary>
        /// <param name="notesId">notesId parameter</param>
        /// <returns>returns all labels using notesId</returns>
        [HttpGet("{NotesId}/Get")]
        public IActionResult GetlabelByNotesId(long notesId)
        {
            try
            {
                long userId = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var labels = this.labelBL.GetlabelByNotesId(notesId);
                if (labels != null)
                {
                    return this.Ok(new { status = 200, isSuccess = true, message = " Specific label found Successfully", data = labels });
                }
                else
                {
                    return this.NotFound(new { isSuccess = false, message = "Specific label not Found" });
                }                   
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for update a label by labelId
        /// </summary>
        /// <param name="labelModel">labelModel parameter</param>
        /// <param name="labelID">labelID parameter</param>
        /// <returns>returns for updated labels</returns>
        [HttpPut("Update")]
        public IActionResult UpdateLabel(LabelModel labelModel, long labelID)
        {
            try
            {
                long userid = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var result = this.labelBL.UpdateLabel(labelModel, labelID);
                if (result != null)
                {
                    return this.Ok(new { status = 200, isSuccess = true, message = "Label Updated Successfully", data = result });
                }
                else
                {
                    return this.NotFound(new { isSuccess = false, message = "No Label Found" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = e.InnerException.Message });
            }
        }

        /// <summary>
        /// API for delete a label by labelID
        /// </summary>
        /// <param name="labelID">labelID parameter</param>
        /// <returns>Delete a label</returns>
        [HttpDelete("Delete")]
        public IActionResult DeleteLabel(long labelID)
        {
            try
            {
                long userid = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var delete = this.labelBL.DeleteLabel(labelID);
                if (delete != null)
                {
                    return this.Ok(new { status = 200, isSuccess = true, message = "Label Deleted Successfully" });
                }
                else
                {
                    return this.NotFound(new { isSuccess = false, message = "Label not found" });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = e.InnerException.Message });
            }
        }
    }
}
