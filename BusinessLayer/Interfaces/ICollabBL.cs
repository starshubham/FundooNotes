using CommonLayer.Models;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface ICollabBL
    {
        public bool AddCollab(CollabModel collabModel);
        public IEnumerable<Collaborator> GetCollabsByNoteId(long noteId);
        public string ReomoveCollab(long collabID);
    }
}
