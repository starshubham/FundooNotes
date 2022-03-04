using CommonLayer.Models;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface ILabelRL
    {
        public bool AddLabel(LabelModel labelModel);
        public IEnumerable<Label> GetAllLabels(long userId);
        public List<Label> GetByLabelID(long labelID);
        public string UpdateLabel(LabelModel labelModel, long labelID);
    }
}
