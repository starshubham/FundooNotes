﻿using CommonLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositoryLayer.Services
{
    public class LabelRL : ILabelRL
    {
        public readonly FundooContext fundooContext; //context class is used to query or save data to the database.
        public LabelRL(FundooContext fundooContext)
        {
            this.fundooContext = fundooContext;
        }

        /// <summary>
        /// Created AddLabel Method
        /// </summary>
        /// <param name="labelModel"></param>
        /// <returns></returns>
        public bool AddLabel(LabelModel labelModel)
        {
            try
            {
                // checking with the notestable db to find NoteId
                var note = fundooContext.NotesTable.Where(x => x.NoteId == labelModel.NoteId).FirstOrDefault();
                if (note != null)
                {
                    // Entity class Instance
                    Label label = new Label();
                    label.LabelName = labelModel.LabelName;
                    label.NoteId = note.NoteId;
                    label.UserId = note.UserId;
                   
                    this.fundooContext.LabelsTable.Add(label);
                    int result = this.fundooContext.SaveChanges();
                    if (result > 0)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Method to get all Labels
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<Label> GetAllLabels(long userId)
        {
            try
            {
                var result = this.fundooContext.LabelsTable.ToList().Where(x => x.UserId == userId);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Method to get labels by NotesId
        /// </summary>
        /// <param name="NotesId"></param>
        /// <returns></returns>
        public List<Label> GetlabelByNotesId(long NotesId)
        {
            try
            {
                var response = this.fundooContext.LabelsTable.Where(x => x.NoteId == NotesId).ToList();
                return response;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Method to updateLabel by labelID
        /// </summary>
        /// <param name="labelModel"></param>
        /// <param name="labelID"></param>
        /// <returns></returns>
        public string UpdateLabel(LabelModel labelModel, long labelID)
        {
            try
            {
                var update = fundooContext.LabelsTable.Where(X => X.LabelID == labelID).FirstOrDefault();
                if (update != null && update.LabelID == labelID)
                {
                    update.LabelName = labelModel.LabelName;
                    update.NoteId = labelModel.NoteId;

                    this.fundooContext.SaveChanges();
                    return "Label is modified";
                }
                else
                {
                    return "Label is not modified";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Method to DeleteLabel By LabelID
        /// </summary>
        /// <param name="labelID"></param>
        /// <returns></returns>
        public string DeleteLabel(long labelID)
        {
            var deleteLabel = fundooContext.LabelsTable.Where(X => X.LabelID == labelID).SingleOrDefault();
            if (deleteLabel != null)
            {
                fundooContext.LabelsTable.Remove(deleteLabel);
                this.fundooContext.SaveChanges();
                return "Label Deleted Successfully";
            }
            else
            {
                return null;
            }
        }
    }
}
