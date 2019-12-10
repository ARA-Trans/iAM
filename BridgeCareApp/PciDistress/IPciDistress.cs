namespace AppliedResearchAssociates.PciDistress
{
    public interface IPciDistress
    {
        double ComputePCIValue(string sDeductValues, string sMethodology);
        double pvt_ComputePCIDeduct(int nDistress, string sSeverity, double dAmount, double dSamsiz);
        //pvt_nSevFromSev
        double pciCorrectedDeductValue(string sMethod, string sDeduct, double dLargeDeductLimit);
        //pciPrivateLargeDeductCorrection
        //SortNumericDescending
        //GetLengthNonNegativeArray
        //SafePercentShare
        bool IsWASHCLKMethod(string s);
        //pvt_ComputeNonPCIDeduct(,,,string)
        double pvt_ComputeNonPCIDeduct(string sMethod, int nDistress, string sSeverity, double dExtent);
        //pvt_ComputeWASHCLKDeduct
        //pvt_SeverityExtent2Deduct
        //TotalDeducts
        //GetMethodologies
        //GetMPACDistressNames
        //Q
        //DeductEval
        //CalcWCI_CDV
        //dis2row
    }
}
