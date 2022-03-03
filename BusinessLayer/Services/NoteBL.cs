using BusinessLayer.Interfaces;
using CommonLayer.Models;
using Microsoft.AspNetCore.Http;
using RepositoryLayer.Entities;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Services
{
    public class NoteBL : INoteBL
    {
        /// <summary>
        /// Variable
        /// </summary>
        private readonly INoteRL noteRL;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="noteBL"></param>
        public NoteBL(INoteRL noteRL)
        {
            this.noteRL = noteRL;
        }

        /// <summary>
        /// Adding a new Note Method
        /// </summary>
        /// <param name="noteModel"></param>
        /// <returns></returns>
        public bool CreateNote(NoteModel noteModel, long userId)
        {
            try
            {
                return this.noteRL.CreateNote(noteModel, userId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Note> GetAllNotes(long userId)
        {
            try
            {
                return this.noteRL.GetAllNotes(userId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Note> GetNote(int NotesId)
        {
            try
            {
                return this.noteRL.GetNote(NotesId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string UpdateNote(NoteModel noteUpdateModel, long NoteId)
        {
            try
            {
                return noteRL.UpdateNote(noteUpdateModel, NoteId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DeleteNotes(long NoteId)
        {
            try
            {
                return noteRL.DeleteNotes(NoteId);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ArchiveNote(long NoteId)
        {
            try
            {
                return noteRL.ArchiveNote(NoteId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string UnArchiveNote(long NoteId)
        {
            try
            {
                return noteRL.UnArchiveNote(NoteId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string PinNote(long NotesId)
        {
            try
            {
                return noteRL.PinNote(NotesId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string TrashNote(long NotesId)
        {
            try
            {
                return noteRL.TrashNote(NotesId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string NoteColor(long NoteId, string addcolor)
        {
            try
            {
                return noteRL.NoteColor(NoteId, addcolor);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool AddBGImage(IFormFile imageURL, long noteid)  // IFormFile comes from using Microsoft.AspNetCore.Http namespace
        {
            try
            {
                return noteRL.AddBGImage(imageURL, noteid);
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
                return noteRL.DeleteBGImage(noteid);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
