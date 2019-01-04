namespace UnravelTravel.Data
{
    using System;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common;

    public class DbQueryRunner : IDbQueryRunner
    {
        public DbQueryRunner(UnravelTravelDbContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UnravelTravelDbContext Context { get; set; }

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
