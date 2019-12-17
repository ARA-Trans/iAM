namespace AppliedResearchAssociates.PciDistress
{
    public interface IPciDistress
    {
        double ComputePCIValue(string sDeductValues, string sMethodology);

        bool IsWASHCLKMethod(string s);

        double pvt_ComputeNonPCIDeduct(string sMethod, int nDistress, string sSeverity, double dExtent);

        double pvt_ComputePCIDeduct(int nDistress, string sSeverity, double dAmount, double dSamsiz);
    }
}
