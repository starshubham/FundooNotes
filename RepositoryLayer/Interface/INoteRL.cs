﻿using CommonLayer.Models;
using Microsoft.AspNetCore.Http;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface INoteRL
    {
        public bool CreateNote(NoteModel noteModel, long userId);
        public IEnumerable<Note> GetEveryoneNotes();
        public IEnumerable<Note> GetAllNotes(long userId);
        public List<Note> GetNote(int NotesId);
        public string UpdateNote(NoteModel noteUpdateModel, long NoteId);
        public string DeleteNotes(long NoteId);
        public string ArchiveNote(long NoteId);
        public string UnArchiveNote(long NoteId);
        public string PinNote(long NotesId);
        public string TrashNote(long NotesId);
        public string NoteColor(long NoteId, string addcolor);
        public bool AddBGImage(IFormFile imageURL, long noteid);
        public bool DeleteBGImage(long noteid);
    }
}
