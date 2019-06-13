using BridgeCare.DataAccessLayer;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Services;
using BridgeCare.Services.SummaryReport;
using System;

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

            container.RegisterType<INetwork, Network>();
            container.RegisterType<ISimulation, Simulations>();
            container.RegisterType<ISections, Sections>();
            container.RegisterType<ISectionLocator, SectionLocator>();
            container.RegisterType<IDetailedReport, DetailedReport>();
            container.RegisterType<IAttributeNames, Attributes>();
            container.RegisterType<IAttributesByYear, AttributesByYear>();
            container.RegisterType<IPerformanceLibrary, PerformanceLibrary>();
            container.RegisterType<BridgeCareContext>();
            container.RegisterType<IBudgetReport, BudgetReport>();
            container.RegisterType<IValidation, Validation>();
            container.RegisterType<IInventory, Inventory>();
            container.RegisterType<ISimulationAnalysis, SimulationAnalysis>();
            container.RegisterType<IDeficientReport, DeficientReport>();
            container.RegisterType<IInvestmentLibrary, InvestmentLibrary>();
            container.RegisterType<ITreatmentLibrary, TreatmentLibrary>();
            container.RegisterType<CostDetails>();
            container.RegisterType<ITarget, Targets>();
            container.RegisterType<IReportCreator, ReportCreator>();
            container.RegisterType<FillDetailedSheet>();
            container.RegisterType<TargetsMet>();
            container.RegisterType<TargetResults>();
            container.RegisterType<Target>();
            container.RegisterType<Deficient>();
            container.RegisterType<Detailed>();
            container.RegisterType<Budget>();
            container.RegisterType<CellAddress>();
            container.RegisterType<IRunSimulation, RunSimulation>();

            //Summary Report types
            container.RegisterType<ISummaryReportGenerator, SummaryReportGenerator>();
            container.RegisterType<IBridgeData, BridgeData>();
            container.RegisterType<SummaryReportBridgeData>();
            container.RegisterType<ICommonSummaryReportData, CommonSummaryReportData>();
            container.RegisterType<IBridgeWorkSummaryData, BridgeWorkSummaryData>();

            container.RegisterType<IInventoryItemDetailModelGenerator, InventoryItemDetailModelGenerator>();
        }
    }
}