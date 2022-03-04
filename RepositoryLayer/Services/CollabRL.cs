using CommonLayer.Models;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositoryLayer.Services
{
    public class CollabRL : ICollabRL
    {
        public readonly FundooContext fundooContext; //context class is used to query or save data to the database.
        
        public CollabRL(FundooContext fundooContext)
        {
            this.fundooContext = fundooContext;            
        }

        public bool AddCollab(CollabModel collabModel)
        {
            try
            {               
                var noteData = this.fundooContext.NotesTable.Where(x => x.NoteId == collabModel.NoteId).FirstOrDefault();
                var userData = this.fundooContext.UserTables.Where(x => x.Email == collabModel.CollabEmail).FirstOrDefault();
                if(noteData != null && userData != null)
                {
                    Collaborator collab = new Collaborator();
                    collab.CollabEmail = collabModel.CollabEmail;
                    collab.NoteId = collabModel.NoteId;
                    collab.UserId = userData.Id;
                    //Adding the data to database
                    this.fundooContext.CollabsTable.Add(collab);
                }
                               
                //Save the changes in database
                int result = this.fundooContext.SaveChanges();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }            
        }

        public IEnumerable<Collaborator> GetCollabsByNoteId(long noteId)
        {
            try
            {
                var result = this.fundooContext.CollabsTable.ToList().Where(x => x.NoteId == noteId);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
