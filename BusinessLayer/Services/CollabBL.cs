using BusinessLayer.Interfaces;
using CommonLayer.Models;
using RepositoryLayer.Interface;
using RepositoryLayer.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Services
{
    public class CollabBL : ICollabBL
    {
        private readonly ICollabRL collabRL;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collabRL"></param>
        public CollabBL(ICollabRL collabRL)
        {
            this.collabRL = collabRL;
        }

        public bool AddCollab(CollabModel collabModel)
        {
            try
            {
                return collabRL.AddCollab(collabModel);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
