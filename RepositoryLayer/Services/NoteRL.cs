using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CommonLayer.Models;
using Microsoft.AspNetCore.Http;
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
        IConfiguration _Appsettings;  //IConfiguration interface is used to read Settings and Connection Strings from AppSettings.
        public NoteRL(FundooContext fundooContext, IConfiguration Appsettings)
        {
            this.fundooContext = fundooContext;
            _Appsettings = Appsettings;
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
        /// Show all notes of all users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Note> GetEveryoneNotes()
        {
            try
            {
                var result = this.fundooContext.NotesTable.ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Show all his notes of particular user
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
                var update = fundooContext.NotesTable.Where(X => X.NoteId == NoteId).FirstOrDefault();
                if (update != null && update.NoteId == NoteId)
                {
                    update.Title = noteUpdateModel.Title;
                    update.Body = noteUpdateModel.Body;
                    update.ModifiedAt = DateTime.Now;
                    update.Color = noteUpdateModel.Color;
                    update.BGImage = noteUpdateModel.BGImage;

                    this.fundooContext.SaveChanges();
                    return "Note is Modified";
                }
                else
                {
                    return "Note Not Modified";
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

        public string ArchiveNote(long NoteId)
        {
            var response = this.fundooContext.NotesTable.Where(a => a.NoteId == NoteId).SingleOrDefault();
            if (response != null)
            {
                response.IsArchived = true;
                this.fundooContext.SaveChanges();
                return "Note is Archived";
            }
            else
            {
                return null;
            }
        }
        public string UnArchiveNote(long NoteId)
        {
            var response = this.fundooContext.NotesTable.Where(a => a.NoteId == NoteId && a.IsArchived == true).SingleOrDefault();
            if (response != null)
            {
                response.IsArchived = false;
                this.fundooContext.SaveChanges();
                return "Note is Unarchived";
            }
            else
            {
                return null;
            }
        }

        public string PinNote(long NotesId)
        {
            var pin = this.fundooContext.NotesTable.Where(p => p.NoteId == NotesId).FirstOrDefault();
            if (pin.IsPinned == false)
            {
                pin.IsPinned = true;
                this.fundooContext.SaveChanges();
                return "Note is Pinned";
            }
            else
            {
                pin.IsPinned = false;
                this.fundooContext.SaveChanges();
                return "Note is Unpinned";
            }
        }

        public string TrashNote(long NotesId)
        {
            var trashed = this.fundooContext.NotesTable.Where(p => p.NoteId == NotesId).FirstOrDefault();
            if (trashed.IsDeleted == true)
            {
                trashed.IsDeleted = false;
                this.fundooContext.SaveChanges();
                return "notes recoverd";
            }
            else
            {
                trashed.IsDeleted = true;
                this.fundooContext.SaveChanges();
                return "note is in trashed";
            }
        }

        public string NoteColor(long NoteId, string addcolor)
        {

            var note = this.fundooContext.NotesTable.Where(c => c.NoteId == NoteId).SingleOrDefault();
            if (note != null)
            {
                if (addcolor != null)
                {
                    note.Color = addcolor;
                    this.fundooContext.NotesTable.Update(note);
                    return this.fundooContext.SaveChanges().ToString();
                }
                else
                {
                    return null;
                }
            }
            return "Failed! NoteId is not Present";
        }

        public bool AddBGImage(IFormFile imageURL, long noteid)
        {
            try
            {
                if (noteid > 0)
                {
                    var note = this.fundooContext.NotesTable.Where(b => b.NoteId == noteid).SingleOrDefault();
                    if (note != null)
                    {
                        Account acc = new Account(
                            _Appsettings["Cloudinary:cloud_name"],
                            _Appsettings["Cloudinary:api_key"],
                            _Appsettings["Cloudinary:api_secret"]
                            );
                        Cloudinary Cld = new Cloudinary(acc);
                        var path = imageURL.OpenReadStream();
                        ImageUploadParams upLoadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(imageURL.FileName, path)
                        };
                        var UploadResult = Cld.Upload(upLoadParams);
                        note.BGImage = UploadResult.Url.ToString();
                        note.ModifiedAt = DateTime.Now;
                        this.fundooContext.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteBGImage(long noteid)
        {
            try
            {
                if (noteid > 0)
                {
                    var note = this.fundooContext.NotesTable.Where(x => x.NoteId == noteid).SingleOrDefault();
                    if (note != null)
                    {
                        note.BGImage = "";
                        note.ModifiedAt = DateTime.Now;
                        this.fundooContext.SaveChangesAsync();
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
    }
}
