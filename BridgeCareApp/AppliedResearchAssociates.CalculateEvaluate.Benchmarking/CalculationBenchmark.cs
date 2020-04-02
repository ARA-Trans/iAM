using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace AppliedResearchAssociates.CalculateEvaluate.Benchmarking
{
    public class CalculationBenchmark
    {
        public IEnumerable<string> CalculationExpressions
        {
            get
            {
                yield return "250*[DECK_AREA]";
            }
        }

        [Benchmark]
        [ArgumentsSource(nameof(CalculationExpressions))]
        public Calculator<double> GetCalculator(string expression) => Compiler.GetCalculator(expression);

        [GlobalSetup]
        public void Setup()
        {
            Compiler = new CalculateEvaluateCompiler();
            Compiler.Parameters["deck_area"] = ParameterType.Number;
        }

        private CalculateEvaluateCompiler Compiler;
    }
}
