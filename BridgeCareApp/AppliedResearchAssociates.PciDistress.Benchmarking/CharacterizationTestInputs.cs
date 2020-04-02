using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace AppliedResearchAssociates.PciDistress.Benchmarks
{
    public class CharacterizationTestInputs // For now, only a small sample of all characterization test inputs.
    {
        #region ComputePCIValue

        public IEnumerable<object[]> ComputePCIValue_Source
        {
            get
            {
                yield return new object[] { "30.8336372375488,50.5481834411621,53.7909088134766,4.73999977111816,27.3861827850342,21.6018180847168,2.73818182945251,21.4327259063721", "ac.mpr" };
                yield return new object[] { "35.9527282714844,33.6872749328613,47.0909080505371,3.13863611221313,28.3967266082764", "ac.mpr" };
            }
        }

        [Benchmark]
        [ArgumentsSource(nameof(ComputePCIValue_Source))]
        public double ComputePCIValue(string sDeductValues, string sMethodology) => PciDistress.ComputePCIValue(sDeductValues, sMethodology);

        #endregion ComputePCIValue

        #region IsWASHCLKMethod

        public IEnumerable<object> IsWASHCLKMethod_Source
        {
            get
            {
                yield return "ac.mpr";
            }
        }

        [Benchmark]
        [ArgumentsSource(nameof(IsWASHCLKMethod_Source))]
        public bool IsWASHCLKMethod(string s) => PciDistress.IsWASHCLKMethod(s);

        #endregion IsWASHCLKMethod

        #region pvt_ComputePCIDeduct

        public IEnumerable<object[]> pvt_ComputePCIDeduct_Source
        {
            get
            {
                yield return new object[] { 10, "L", 5.5, 2200 };
                yield return new object[] { 10, "L", 10, 2200 };
            }
        }

        [Benchmark]
        [ArgumentsSource(nameof(pvt_ComputePCIDeduct_Source))]
        public double pvt_ComputePCIDeduct(int nDistress, string sSeverity, double dAmount, double dSamsiz) => PciDistress.pvt_ComputePCIDeduct(nDistress, sSeverity, dAmount, dSamsiz);

        #endregion pvt_ComputePCIDeduct
    }
}
