﻿using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Context
{
    // A DbContext instance represents a session with the database and can be used to query and save the intances of  your entities.
    // DbContext is a combination of the Unit of Work and Repository patterns.
    public class FundooContext : DbContext    // used for data accessibility
    {
        public FundooContext(DbContextOptions options)
            : base(options)
        {
        }
        public DbSet<User> UserTables { get; set; }
        public DbSet<Note> NotesTable { get; set; }
        public DbSet<Collaborator> CollabsTable { get; set; }
    }
}
