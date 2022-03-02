using CommonLayer.Models;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface INoteRL
    {
        public bool CreateNote(NoteModel noteModel, long userId);
        public IEnumerable<Note> GetAllNotes();
    }
}
