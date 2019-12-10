using _ = PCI.Distress;

namespace AppliedResearchAssociates.PciDistress
{
    public sealed class PciDistressCppCli : IPciDistress
    {
        public static PciDistressCppCli Instance { get; } = new PciDistressCppCli();

        public double ComputePCIValue(string sDeductValues, string sMethodology) => _.ComputePCIValue(sDeductValues, sMethodology);

        public bool IsWASHCLKMethod(string s) => _.IsWASHCLKMethod(s);

        public double pciCorrectedDeductValue(string sMethod, string sDeduct, double dLargeDeductLimit) => _.pciCorrectedDeductValue(sMethod, sDeduct, dLargeDeductLimit);

        public double pvt_ComputeNonPCIDeduct(string sMethod, int nDistress, string sSeverity, double dExtent) => _.pvt_ComputeNonPCIDeduct(sMethod, nDistress, sSeverity, dExtent);

        public double pvt_ComputePCIDeduct(int nDistress, string sSeverity, double dAmount, double dSamsiz) => _.pvt_ComputePCIDeduct(nDistress, sSeverity, dAmount, dSamsiz);

        private PciDistressCppCli()
        {
        }
    }
}
