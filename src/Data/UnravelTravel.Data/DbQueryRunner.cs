﻿namespace UnravelTravel.Data
{
    using System;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common;

    public class DbQueryRunner : IDbQueryRunner
    {
        public DbQueryRunner(ApplicationDbContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ApplicationDbContext Context { get; set; }

        public void RunQuery(string query, params object[] parameters)
        {
            this.Context.Database.ExecuteSqlCommand(query, parameters);
        }

        public void Dispose()
        {
            this.Context?.Dispose();
        }
    }
}
