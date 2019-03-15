namespace BridgeCare
{
    using BridgeCare.EntityClasses;
    using System.Data.Entity;

    public partial class BridgeCareContext : DbContext
    {
        public BridgeCareContext()
            : base("name=BridgeCareContext")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        //public virtual DbSet<DetailedReportModel> DetailedReportModels { get; set; }
        public virtual DbSet<NETWORK> NETWORKS { get; set; }

        public virtual DbSet<SIMULATION> SIMULATIONS { get; set; }
        public virtual DbSet<YEARLYINVESTMENT> YEARLYINVESTMENTs { get; set; }
        public virtual DbSet<INVESTMENTS> INVESTMENTs { get; set; }
        public virtual DbSet<Deficients> Deficient { get; set; }
        public virtual DbSet<Targets> Target { get; set; }
        public virtual DbSet<Attributes> Attributes { get; set; }
        public virtual DbSet<PennDotBridgeData> PennDotBridgeData { get; set; }
        public virtual DbSet<PennDotReportAData> PennDotReportAData { get; set; }
        public virtual DbSet<SdRisk> SdRisks { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NETWORK>()
                .Property(e => e.NETWORK_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<NETWORK>()
                .HasMany(e => e.SIMULATIONS)
                .WithOptional(e => e.NETWORK)
                .WillCascadeOnDelete();

            modelBuilder.Entity<SIMULATION>()
                .Property(e => e.SIMULATION1)
                .IsUnicode(false);

            modelBuilder.Entity<YEARLYINVESTMENT>()
                .Property(e => e.BUDGETNAME)
                .IsUnicode(false);

            modelBuilder.Entity<INVESTMENTS>()
                .Property(e => e.BUDGETORDER)
                .IsUnicode(false);
        }
    }
}