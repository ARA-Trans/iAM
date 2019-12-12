using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _ = LegacyPCI.Distress;

namespace AppliedResearchAssociates.PciDistress
{
    public sealed class PciDistressCppCli : IPciDistress
    {
        #region IPciDistress

        public double ComputePCIValue(string sDeductValues, string sMethodology)
        {
            var result = _.ComputePCIValue(sDeductValues, sMethodology);
            Log(nameof(ComputePCIValue), result, sDeductValues, sMethodology);
            return result;
        }

        public bool IsWASHCLKMethod(string s)
        {
            var result = _.IsWASHCLKMethod(s);
            Log(nameof(IsWASHCLKMethod), result, s);
            return result;
        }

        public double pciCorrectedDeductValue(string sMethod, string sDeduct, double dLargeDeductLimit)
        {
            var result = _.pciCorrectedDeductValue(sMethod, sDeduct, dLargeDeductLimit);
            Log(nameof(pciCorrectedDeductValue), result, sMethod, sDeduct, dLargeDeductLimit);
            return result;
        }

        public double pvt_ComputeNonPCIDeduct(string sMethod, int nDistress, string sSeverity, double dExtent)
        {
            var result = _.pvt_ComputeNonPCIDeduct(sMethod, nDistress, sSeverity, dExtent);
            Log(nameof(pvt_ComputeNonPCIDeduct), result, sMethod, nDistress, sSeverity, dExtent);
            return result;
        }

        public double pvt_ComputePCIDeduct(int nDistress, string sSeverity, double dAmount, double dSamsiz)
        {
            var result = _.pvt_ComputePCIDeduct(nDistress, sSeverity, dAmount, dSamsiz);
            Log(nameof(pvt_ComputePCIDeduct), result, nDistress, sSeverity, dAmount, dSamsiz);
            return result;
        }

        #endregion IPciDistress

        public static PciDistressCppCli Instance { get; } = new PciDistressCppCli();

        public void WriteLog()
        {
            var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var logFolderPath = Path.Combine(docsPath, "RoadCare PCI");
            Directory.CreateDirectory(logFolderPath);

            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");

            var logLines = new List<string>();
            foreach (var (method, logEntries) in InvocationLog.Select(kv => (kv.Key, kv.Value)))
            {
                var csxPath = Path.Combine(docsPath, $"{method}_{timestamp}.csx");
                File.WriteAllLines(csxPath, logEntries.Select(formatDataRow).Distinct());

                logLines.AddRange(logEntries.Select(formatLogLine));

                string formatDataRow(object[] logEntry)
                {
                    var values = logEntry.Select(value =>
                    {
                        if (value is double d) return d.ToString("G17");
                        if (value is string s) return $"\"{s}\"";
                        if (value is bool b) return b.ToString().ToLowerInvariant();
                        if (value is int i) return i.ToString();
                        throw new InvalidOperationException("Unsupported argument type: " + value.GetType());
                    });

                    var arguments = string.Join(", ", values);
                    return $"[DataRow({arguments})]";
                }

                string formatLogLine(object[] logEntry)
                {
                    var fields = logEntry.Select(formatValue).Prepend(method);
                    return string.Join("\t", fields);

                    object formatValue(object value) => value is double d ? d.ToString("G17") : value;
                }
            }

            var logPath = Path.Combine(docsPath, $"PCI Invocations {timestamp}.log");
            File.WriteAllLines(logPath, logLines.Distinct());
        }

        private readonly SortedDictionary<string, List<object[]>> InvocationLog = new SortedDictionary<string, List<object[]>>();

        private PciDistressCppCli()
        {
        }

        private void Log(string method, params object[] row)
        {
            if (!InvocationLog.TryGetValue(method, out var rows))
            {
                rows = new List<object[]>();
                InvocationLog.Add(method, rows);
            }

            rows.Add(row);
        }
    }
}
