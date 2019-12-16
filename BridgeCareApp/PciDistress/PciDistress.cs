using static AppliedResearchAssociates.PciDistress.Constants;
using static AppliedResearchAssociates.PciDistress.Math;

namespace AppliedResearchAssociates.PciDistress
{
    public sealed class PciDistress : IPciDistress
    {
        public static PciDistress Instance { get; } = new PciDistress();

        public double ComputePCIValue(string sDeductValues, string sMethodology) => throw new System.NotImplementedException();

        public bool IsWASHCLKMethod(string s) => throw new System.NotImplementedException();

        public double pvt_ComputeNonPCIDeduct(string sMethod, int nDistress, string sSeverity, double dExtent) => throw new System.NotImplementedException();

        public double pvt_ComputePCIDeduct(int nDistress, string sSeverity, double dAmount, double dSamsiz) => throw new System.NotImplementedException();

        private PciDistress()
        {
        }
    }
}
