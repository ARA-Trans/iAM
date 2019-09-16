namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using OfficeOpenXml;
    using Reports.MDSHA;

    public partial class Report : ReportBase
    {
        public const string GenericTitle =
            "Input Summary Report (Configurable)";

        private readonly List<ISheet> Sheets;

        public Report(string simulationId, string simulation)
        {
            this.SimulationId = simulationId;
            this.Simulation = simulation;
            this.OutputFileBaseName =
                string.Format("{0} - {1}", GenericTitle, simulation);

            this.Sheets = new List<ISheet>();
        }

        public interface ISheet
        {
            string Caption { get; }

            void Insert();

            void AddTo(Report report);
        }

        public override string Title
        {
            get { return GenericTitle; }
        }

        protected override void Open()
        {
            this.OutputFileExtension = "xlsx";
            var outputFile = this.GetNewOutputFile();
            this.OutputPackage = new ExcelPackage(outputFile);
        }

        protected override void Fill()
        {
            foreach (var sheet in this.Sheets)
            {
                sheet.Insert();
            }
        }

        protected override void Save()
        {
            this.OutputPackage.Save();
        }
    }
}
