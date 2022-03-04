using CommonLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface ICollabBL
    {
        public bool AddCollab(CollabModel collabModel);
    }
}
