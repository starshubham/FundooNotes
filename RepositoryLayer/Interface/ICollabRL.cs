using CommonLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface ICollabRL
    {
        public bool AddCollab(CollabModel collabModel);
    }
}
