using AspWebApi.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace AspWebApi
{
    public partial class BridgeCareEntities : DbContext
    {
        public DbSet<DetailedReportModel> DetailedReportModels { get; set; }
    }
}