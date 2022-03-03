using CommonLayer.Models;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface INoteBL
    {
        public bool CreateNote(NoteModel noteModel, long userId);
        public IEnumerable<Note> GetAllNotes(long userId);
        public List<Note> GetNote(int NotesId);
        public string UpdateNote(NoteModel noteUpdateModel, long NoteId);
        public string DeleteNotes(long NoteId);
        public string ArchiveNote(long NoteId);
        public string UnArchiveNote(long NoteId);
        public string PinNote(long NotesId);
        public string TrashNote(long NotesId);
    }
}
