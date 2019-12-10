using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppliedResearchAssociates.PciDistress.Tests
{
    [TestClass]
    public class PciDistress_Tests
    {
        [TestMethod]
        public void ComputePCIValue_runs_without_exception()
        {
            _ = PciDistress.ComputePCIValue(null, null);
        }

        private static IPciDistress PciDistress => PciDistressCppCli.Instance;
    }
}
