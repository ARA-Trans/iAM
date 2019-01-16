namespace BridgeCareCodeFirst
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using BridgeCareCodeFirst.EntityClasses;

    public partial class BridgeCareContext : DbContext
    {
        public BridgeCareContext()
            : base("name=BridgeCareContext")
        {
        }

        public virtual DbSet<NETWORK> NETWORKS { get; set; }
        public virtual DbSet<SIMULATION> SIMULATIONS { get; set; }
        public virtual DbSet<YEARLYINVESTMENT> YEARLYINVESTMENTs { get; set; }
        public virtual DbSet<REPORT_13_9> REPORT_13_9 { get; set; }
        public virtual DbSet<SECTION_13> SECTION_13 { get; set; }

        //public virtual DbSet<DetailedReportModel> DetailedReportModels { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NETWORK>()
                .Property(e => e.NETWORK_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<NETWORK>()
                .Property(e => e.DESCRIPTION)
                .IsUnicode(false);

            modelBuilder.Entity<NETWORK>()
                .Property(e => e.DESIGNER_USERID)
                .IsUnicode(false);

            modelBuilder.Entity<NETWORK>()
                .Property(e => e.DESIGNER_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<NETWORK>()
                .Property(e => e.NETWORK_DEFINITION_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<NETWORK>()
                .Property(e => e.NETWORK_AREA)
                .IsUnicode(false);

            modelBuilder.Entity<NETWORK>()
                .HasMany(e => e.SIMULATIONS)
                .WithOptional(e => e.NETWORK)
                .WillCascadeOnDelete();

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.SIMULATION1)
                .IsUnicode(false);

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.COMMENTS)
                .IsUnicode(false);

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.CREATOR_ID)
                .IsUnicode(false);

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.USERNAME)
                .IsUnicode(false);

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.JURISDICTION)
                .IsUnicode(false);

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.ANALYSIS)
                .IsUnicode(false);

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.BUDGET_CONSTRAINT)
                .IsUnicode(false);

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.WEIGHTING)
                .IsUnicode(false);

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.BENEFIT_VARIABLE)
                .IsUnicode(false);

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.SIMULATION_VARIABLES)
                .IsUnicode(false);

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.RUN_TIME)
                .IsUnicode(false);

            modelBuilder.Entity<YEARLYINVESTMENT>()
                .Property(e => e.BUDGETNAME)
                .IsUnicode(false);

            //modelBuilder.Entity<DetailedReportModel>()
            //    .Property(e => e.Facility)
            //    .IsUnicode(false);

            //modelBuilder.Entity<DetailedReportModel>()
            //    .Property(e => e.Section)
            //    .IsUnicode(false);

            //modelBuilder.Entity<DetailedReportModel>()
            //    .Property(e => e.Treatment)
            //    .IsUnicode(false);
        }
    }
}
