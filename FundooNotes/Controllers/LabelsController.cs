using BusinessLayer.Interfaces;
using CommonLayer.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]  //user to grant and restrict permissions on Web pages.
    public class LabelsController : ControllerBase
    {
        private readonly ILabelBL labelBL;
        private readonly FundooContext fundooContext;

        public LabelsController(ILabelBL labelBL, FundooContext fundooContext)
        {
            this.labelBL = labelBL;
            this.fundooContext = fundooContext;
        }

        [HttpPost("Create")]
        public IActionResult AddLabel(LabelModel labelModel)
        {
            try
            {
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
                        return this.BadRequest(new { status = 400, isSuccess = false, Message = "Label not created" });
                    }                   
                }
                return this.Unauthorized(new { status = 401, isSuccess = false, Message = "Unauthorized User!" });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { status = 400, isSuccess = false, Message = e.InnerException.Message });
            }
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllLabels(long userId)
        {
            try
            {
                var labels = labelBL.GetAllLabels(userId);
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
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.InnerException.Message });
            }
        }

        [HttpGet("GetByLabelID")]
        public IActionResult GetByLabelID(long labelID)
        {
            try
            {
                long userId = Convert.ToInt32(User.Claims.FirstOrDefault(X => X.Type == "Id").Value);
                var labels = this.labelBL.GetByLabelID(labelID);
                if (labels != null)
                {
                    return this.Ok(new { status = 200, isSuccess = true, message = " Specific label found Successfully", data = labels });
                }
                else
                    return this.NotFound(new { isSuccess = false, message = "Specific label not Found" });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.InnerException.Message });
            }
        }

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
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.InnerException.Message });
            }
        }

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
                return this.BadRequest(new { Status = 401, isSuccess = false, Message = e.InnerException.Message });
            }
        }
    }
}
