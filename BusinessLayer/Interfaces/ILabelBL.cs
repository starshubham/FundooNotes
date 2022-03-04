using CommonLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface ILabelBL
    {
        public bool AddLabel(LabelModel labelModel);
    }
}
