using CommonLayer.Models;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositoryLayer.Services
{
    public class NoteRL : INoteRL
    {
        /// <summary>
        /// Defining Variables
        /// </summary>
        public readonly FundooContext fundooContext;
        IConfiguration _configure;

        public NoteRL(FundooContext fundooContext, IConfiguration configure)
        {
            this.fundooContext = fundooContext;
            _configure = configure;
        }

        /// <summary>
        /// Creating a CreateNote Method
        /// </summary>
        /// <param name="noteModel"></param>
        /// <returns></returns>
        public bool CreateNote(NoteModel noteModel)
        {
            try
            {
                Note newNotes = new Note();
                newNotes.NoteId = noteModel.NoteId;
                newNotes.Title = noteModel.Title;
                newNotes.Body = noteModel.Body;
                newNotes.Reminder = noteModel.Reminder;
                newNotes.Color = noteModel.Color;
                newNotes.BGImage = noteModel.BGImage;
                newNotes.IsArchived = noteModel.IsArchived;
                newNotes.IsPinned = noteModel.IsPinned;
                newNotes.IsDeleted = noteModel.IsDeleted;
                newNotes.CreatedAt = DateTime.Now;
                //Adding the data to database
                this.fundooContext.NotesTable.Add(newNotes);
                //Save the changes in database
                int result = this.fundooContext.SaveChanges();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Show all his notes to user
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Note> GetAllNotes()
        {
            try
            {
                return this.fundooContext.NotesTable.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
