using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace AppliedResearchAssociates.PciDistress.Benchmarks
{
    [LegacyJitX86Job]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    public class CppCliVsCs
    {
        #region ComputePCIValue

        public IEnumerable<object[]> ComputePCIValue_Source
        {
            get
            {
                yield return new object[] { "30.8336372375488,50.5481834411621,53.7909088134766,4.73999977111816,27.3861827850342,21.6018180847168,2.73818182945251,21.4327259063721", "ac.mpr" };
            }
        }

        [BenchmarkCategory(nameof(IPciDistress.ComputePCIValue))]
        [Benchmark]
        [ArgumentsSource(nameof(ComputePCIValue_Source))]
        public double ComputePCIValue(string sDeductValues, string sMethodology) => PciDistress.Instance.ComputePCIValue(sDeductValues, sMethodology);

        [BenchmarkCategory(nameof(IPciDistress.ComputePCIValue))]
        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(ComputePCIValue_Source))]
        public double ComputePCIValue_CppCli(string sDeductValues, string sMethodology) => PciDistressCppCli.Instance.ComputePCIValue(sDeductValues, sMethodology);

        #endregion ComputePCIValue

        #region IsWASHCLKMethod

        public IEnumerable<object> IsWASHCLKMethod_Source
        {
            get
            {
                yield return "ac.mpr";
            }
        }

        [BenchmarkCategory(nameof(IPciDistress.IsWASHCLKMethod))]
        [Benchmark]
        [ArgumentsSource(nameof(IsWASHCLKMethod_Source))]
        public bool IsWASHCLKMethod(string s) => PciDistress.Instance.IsWASHCLKMethod(s);

        [BenchmarkCategory(nameof(IPciDistress.IsWASHCLKMethod))]
        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(IsWASHCLKMethod_Source))]
        public bool IsWASHCLKMethod_CppCli(string s) => PciDistressCppCli.Instance.IsWASHCLKMethod(s);

        #endregion IsWASHCLKMethod

        #region pvt_ComputePCIDeduct

        public IEnumerable<object[]> pvt_ComputePCIDeduct_Source
        {
            get
            {
                yield return new object[] { 10, "L", 5.5, 2200 };
            }
        }

        [BenchmarkCategory(nameof(IPciDistress.pvt_ComputePCIDeduct))]
        [Benchmark]
        [ArgumentsSource(nameof(pvt_ComputePCIDeduct_Source))]
        public double pvt_ComputePCIDeduct(int nDistress, string sSeverity, double dAmount, double dSamsiz) => PciDistress.Instance.pvt_ComputePCIDeduct(nDistress, sSeverity, dAmount, dSamsiz);

        [BenchmarkCategory(nameof(IPciDistress.pvt_ComputePCIDeduct))]
        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(pvt_ComputePCIDeduct_Source))]
        public double pvt_ComputePCIDeduct_CppCli(int nDistress, string sSeverity, double dAmount, double dSamsiz) => PciDistressCppCli.Instance.pvt_ComputePCIDeduct(nDistress, sSeverity, dAmount, dSamsiz);

        #endregion pvt_ComputePCIDeduct
    }
}
