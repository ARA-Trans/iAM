using AppliedResearchAssociates.PciDistress;
using System;

namespace PCI
{
    /// <summary>
    ///     This class (and its namespace) provide a way to transparently drop the new <see
    ///     cref="IPciDistress"/> abstraction into existing callsites that have/had been directly
    ///     accessing the legacy C++/CLI <see cref="LegacyPCI.Distress"/> type.
    /// </summary>
    public static class Distress
    {
        public static Func<string, string, double> ComputePCIValue => _.ComputePCIValue;

        public static Func<string, bool> IsWASHCLKMethod => _.IsWASHCLKMethod;

        public static Func<string, string, double, double> pciCorrectedDeductValue => _.pciCorrectedDeductValue;

        public static Func<string, int, string, double, double> pvt_ComputeNonPCIDeduct => _.pvt_ComputeNonPCIDeduct;

        public static Func<int, string, double, double, double> pvt_ComputePCIDeduct => _.pvt_ComputePCIDeduct;

        private static IPciDistress _ => PciDistressCppCli.Instance;
    }
}
