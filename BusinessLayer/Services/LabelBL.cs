using BusinessLayer.Interfaces;
using CommonLayer.Models;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Services
{
    public class LabelBL : ILabelBL
    {
        private readonly ILabelRL labelRL;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collabRL"></param>
        public LabelBL(ILabelRL labelRL)
        {
            this.labelRL = labelRL;
        }

        public bool AddLabel(LabelModel labelModel)
        {
            try
            {
                return labelRL.AddLabel(labelModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
