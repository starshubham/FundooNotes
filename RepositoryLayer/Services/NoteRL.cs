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
        public readonly FundooContext fundooContext; //context class is used to query or save data to the database.

        public NoteRL(FundooContext fundooContext)
        {
            this.fundooContext = fundooContext;
        }

        /// <summary>
        /// Creating a CreateNote Method
        /// </summary>
        /// <param name="noteModel"></param>
        /// <returns></returns>
        public bool CreateNote(NoteModel noteModel, long userId)
        {
            try
            {
                Note newNotes = new Note();
                newNotes.UserId = userId;
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
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Show all his notes to user
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Note> GetAllNotes(long userId)
        {
            try
            {
                var result = this.fundooContext.NotesTable.ToList().Where(x => x.UserId == userId);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<Note> GetNote(int NotesId)
        {
            var listNote = fundooContext.NotesTable.Where(X => X.NoteId == NotesId).SingleOrDefault();
            if (listNote != null)
            {
                return fundooContext.NotesTable.Where(list => list.NoteId == NotesId).ToList();
            }
            return null;
        }

        public string UpdateNote(NoteModel noteUpdateModel, long NoteId)
        {
            try
            {
                var update = fundooContext.NotesTable.Where(X => X.NoteId == noteUpdateModel.NoteId).SingleOrDefault();
                if (update != null)
                {
                    update.Title = noteUpdateModel.Title;
                    update.Body = noteUpdateModel.Body;
                    update.ModifiedAt = DateTime.Now;
                    update.Color = noteUpdateModel.Color;
                    update.BGImage = noteUpdateModel.BGImage;

                    this.fundooContext.SaveChanges();
                    return "Modified";
                }
                else
                {
                    return "Not Modified";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DeleteNotes(long NoteId)
        {
            var deleteNote = fundooContext.NotesTable.Where(X => X.NoteId == NoteId).SingleOrDefault();
            if (deleteNote != null)
            {
                fundooContext.NotesTable.Remove(deleteNote);
                this.fundooContext.SaveChanges();
                return "Notes Deleted Successfully";
            }
            else
            {
                return null;
            }
        }
    }
}
