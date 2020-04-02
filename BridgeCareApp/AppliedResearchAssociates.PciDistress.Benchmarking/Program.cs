using BenchmarkDotNet.Running;

namespace AppliedResearchAssociates.PciDistress.Benchmarks
{
    internal static class Program
    {
        private static void Main() => _ = BenchmarkRunner.Run<CharacterizationTestInputs>();
    }
}
