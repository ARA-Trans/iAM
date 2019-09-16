using System;
using System.Data.Common;
using System.IO;
using Microsoft.VisualBasic.FileIO; // File recycling
using OfficeOpenXml;

namespace Reports.MDSHA
{
    /// <summary>
    ///     Encapsulates some common functionality for all MDSHA reports (those
    ///     added in Spring/Summer 2014). Inheritors are expected to provide
    ///     implementations for the report's Title property and
    ///     OutputFileExtension property, as well as several methods that
    ///     constitute the template method Generate.
    /// </summary>
    public abstract class ReportBase
    {
        private const string RptGenErrorMsgFmt =
            "Report generation for \"{0}\" encountered error:{1}{2}{1}";

        protected ReportBase()
        {
            Tags = new MetaData();
        }

        public abstract string Title { get; }

        public DirectoryInfo OutputDirectory { get; set; }

        public string OutputFileBaseName { get; set; }

        protected string OutputFileExtension { private get; set; }

        protected ExcelPackage OutputPackage { get; set; }

        public string NetworkId
        {
            get
            {
                return Tags.NetworkId;
            }
            protected set
            {
                Tags.NetworkId = value;
            }
        }

        public string Network
        {
            get
            {
                return Tags.Network;
            }
            protected set
            {
                Tags.Network = value;
            }
        }

        public string SimulationId
        {
            get
            {
                return Tags.SimulationId;
            }
            protected set
            {
                Tags.SimulationId = value;
            }
        }

        public string Simulation
        {
            get
            {
                return Tags.Simulation;
            }
            protected set
            {
                Tags.Simulation = value;
            }
        }

        private MetaData Tags { get; set; }

        public void Generate()
        {
            try
            {
                using (OutputPackage)
                {
                    Open();
                    Fill();
                    Save();
                }

                OutputPackage = null;
            }
            catch (DbException ex)
            {
                if (OutputPackage != null)
                {
                    OutputPackage.Stream.Dispose();
                }

                var msg = string.Format(
                    RptGenErrorMsgFmt,
                    Title,
                    Environment.NewLine,
                    ex.Message);

                throw new ReportGenerationException(msg, ex);
            }
        }

        protected abstract void Open();

        protected abstract void Fill();

        protected abstract void Save();

        protected FileInfo GetNewOutputFile()
        {
            var outputFileName =
                OutputFileBaseName + "." + OutputFileExtension;

            var outputFilePath = Path.Combine(
                OutputDirectory.FullName,
                outputFileName);

            var outputFile = new FileInfo(outputFilePath);
            if (outputFile.Exists)
            {
                try
                {
                    // Be nice. Recycle, don't delete.
                    FileSystem.DeleteFile(
                        outputFile.FullName,
                        UIOption.OnlyErrorDialogs,
                        RecycleOption.SendToRecycleBin);
                }
                catch (OperationCanceledException e)
                {
                    throw new ReportGenerationException(e.Message, e);
                }

                outputFile = new FileInfo(outputFilePath);
            }

            return outputFile;
        }

        protected class MetaData
        {
            public string NetworkId { get; set; }

            public string Network { get; set; }

            public string SimulationId { get; set; }

            public string Simulation { get; set; }
        }
    }
}
