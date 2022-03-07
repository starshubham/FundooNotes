using CommonLayer.Models;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface ICollabRL
    {
        public bool AddCollab(CollabModel collabModel);
        public IEnumerable<Collaborator> GetAllCollabs();
        public IEnumerable<Collaborator> GetCollabsByNoteId(long noteId);
        public string ReomoveCollab(long collabID);
    }
}
