using BenchmarkDotNet.Running;

namespace AppliedResearchAssociates.PciDistress.Benchmarks
{
    internal class Program
    {
        private static void Main() => _ = BenchmarkRunner.Run<CppCliVsCs>();
    }
}
