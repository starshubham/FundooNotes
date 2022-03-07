﻿using BusinessLayer.Interfaces;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  //user to grant and restrict permissions on Web pages.
    public class CollabsController : ControllerBase
    {
        private readonly ICollabBL collabBL;
        private readonly FundooContext fundooContext;
        private readonly IMemoryCache memoryCache;
        private readonly IDistributedCache distributedCache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collabBL"></param>
        public CollabsController(ICollabBL collabBL, FundooContext fundooContext, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            this.collabBL = collabBL;
            this.fundooContext = fundooContext;
            this.memoryCache = memoryCache;
            this.distributedCache = distributedCache;
        }

        [HttpPost("Add")]
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

        [HttpGet("GetAll")]
        public IActionResult GetAllCollabs()
        {
            try
            {
                long userId = Convert.ToInt32(User.Claims.FirstOrDefault(e => e.Type == "Id").Value);
                var collabs = collabBL.GetAllCollabs();
                if (collabs != null)
                {
                    return this.Ok(new { isSuccess = true, message = " All Collaborators found Successfully", data = collabs });

                }
                else
                {
                    return this.NotFound(new { isSuccess = false, message = "No Collaborator  Found" });
                }
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = ex.InnerException.Message });
            }
        }

        [HttpGet("redis")]
        public async Task<IActionResult> GetAllCollaboratorUsingRedisCache()
        {
            var cacheKey = "CollabsList";
            string serializedList;
            var CollabsList = new List<Collaborator>();
            var redisCollabsList = await distributedCache.GetAsync(cacheKey);
            if (redisCollabsList != null)
            {
                serializedList = Encoding.UTF8.GetString(redisCollabsList);
                CollabsList = JsonConvert.DeserializeObject<List<Collaborator>>(serializedList);
            }
            else
            {
                CollabsList = await fundooContext.CollabsTable.ToListAsync();  // Comes from Microsoft.EntityFrameworkCore Namespace
                serializedList = JsonConvert.SerializeObject(CollabsList);
                redisCollabsList = Encoding.UTF8.GetBytes(serializedList);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                await distributedCache.SetAsync(cacheKey, redisCollabsList, options);
            }
            return Ok(CollabsList);
        }

        [HttpGet("GetByNoteId")]
        public IActionResult GetCollabsByNoteId(long noteId)
        {
            try
            {
                long userId = Convert.ToInt32(User.Claims.FirstOrDefault(e => e.Type == "Id").Value);
                var collabs = collabBL.GetCollabsByNoteId(noteId);
                if (collabs != null)
                {
                    return this.Ok(new { isSuccess = true, message = " All Collaborators found Successfully", data = collabs });

                }
                else
                {
                    return this.NotFound(new { isSuccess = false, message = "No Collaborator  Found" });
                }
            }
            catch (Exception ex)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, message = ex.InnerException.Message });
            }
        }

        [HttpDelete("Remove")]
        public IActionResult ReomoveCollab(long collabID)
        {
            try
            {
                long userid = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var delete = this.collabBL.ReomoveCollab(collabID);
                if (delete != null)
                {
                    return this.Ok(new { status = 200, isSuccess = true, message = "Member removed from collaboration successfully", data = collabID });
                }
                else
                {
                    return this.NotFound(new { isSuccess = false, message = "Member not removed from collaboration." });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.Message, InnerException = e.InnerException });
            }
        }
    }
}
