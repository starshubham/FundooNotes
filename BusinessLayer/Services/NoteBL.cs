using BusinessLayer.Interfaces;
using CommonLayer.Models;
using RepositoryLayer.Entities;
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
        private readonly INoteBL noteBL;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="noteBL"></param>
        public NoteBL(INoteBL noteBL)
        {
            this.noteBL = noteBL;
        }

        /// <summary>
        /// Adding a new Note Method
        /// </summary>
        /// <param name="noteModel"></param>
        /// <returns></returns>
        public bool CreateNote(NoteModel noteModel)
        {
            try
            {
                return this.noteBL.CreateNote(noteModel);
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
                return this.noteBL.GetAllNotes();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
