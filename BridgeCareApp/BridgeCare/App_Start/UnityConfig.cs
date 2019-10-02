using BridgeCare.DataAccessLayer;
using BridgeCare.DataAccessLayer.CriteriaDrivenBudgets;
using BridgeCare.Interfaces;
using BridgeCare.Interfaces.CriteriaDrivenBudgets;
using BridgeCare.Models;
using BridgeCare.Services;
using BridgeCare.Services.SummaryReport;
using System;
using BridgeCare.DataAccessLayer.Inventory;
using BridgeCare.DataAccessLayer.SummaryReport;
using Unity;

namespace BridgeCare
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container

        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;

        #endregion Unity Container

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container"> The unity container to configure. </param>
        /// <remarks> 
        /// There is no need to register concrete types such as controllers or API controllers
        /// (unless you want to change the defaults), as Unity allows resolving a concrete type even
        /// if it was not previously registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a
            // Unity.Configuration to the using statements. container.LoadConfiguration();

            container.RegisterType<INetwork, NetworkDAL>();
            container.RegisterType<ISimulation, SimulationDAL>();
            container.RegisterType<ISections, SectionsDAL>();
            container.RegisterType<ISectionLocator, SectionLocatorDAL>();
            container.RegisterType<IDetailedReport, DetailedReportDAL>();
            container.RegisterType<IAttributeRepo, AttributesDAL>();
            container.RegisterType<IPerformanceLibrary, PerformanceLibraryDAL>();
            container.RegisterType<BridgeCareContext>();
            container.RegisterType<IBudgetReport, BudgetReportDAL>();
            container.RegisterType<IValidation, Validation>();
            container.RegisterType<IInventory, InventoryDAL>();
            container.RegisterType<ISimulationAnalysis, SimulationAnalysisDAL>();
            container.RegisterType<IDeficientReport, DeficientReportDAL>();
            container.RegisterType<IInvestmentLibrary, InvestmentLibraryDAL>();
            container.RegisterType<ITreatmentLibrary, TreatmentLibraryDAL>();
            container.RegisterType<CostDetails>();
            container.RegisterType<ITarget, TargetsDAL>();
            container.RegisterType<IReportCreator, ReportCreator>();
            container.RegisterType<FillDetailedSheet>();
            container.RegisterType<TargetsMetDAL>();
            container.RegisterType<TargetResultsDAL>();
            container.RegisterType<Target>();
            container.RegisterType<Deficient>();
            container.RegisterType<Detailed>();
            container.RegisterType<Budget>();
            container.RegisterType<CellAddress>();
            container.RegisterType<IPriority, PriorityDAL>();
            container.RegisterType<IDeficient, DeficientDAL>();
            container.RegisterType<IRemainingLifeLimit, RemainingLifeLimitDAL>();
            container.RegisterType<IRunRollup, RunRollupDAL>();

            //Summary Report types
            container.RegisterType<ISummaryReportGenerator, SummaryReportGenerator>();
            container.RegisterType<IBridgeData, BridgeDataDAL>();
            container.RegisterType<SummaryReportBridgeData>();
            container.RegisterType<ICommonSummaryReportData, CommonSummaryReportDataDAL>();
            container.RegisterType<IBridgeWorkSummaryData, BridgeWorkSummaryDataDAL>();

            container.RegisterType<IInventoryItemDetailModelGenerator, InventoryItemDetailModelGenerator>();
            container.RegisterType<ICommittedProjects, CommittedProjects>();
            container.RegisterType<ICommitted, CommittedDAL>();

            container.RegisterType<ICriteriaDrivenBudgets, CriteriaDrivenBudgetsDAL>();
        }
    }
}