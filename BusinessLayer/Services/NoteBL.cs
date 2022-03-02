using BusinessLayer.Interfaces;
using CommonLayer.Models;
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

        public IEnumerable<Note> GetAllNotes()
        {
            try
            {
                return this.noteRL.GetAllNotes();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
