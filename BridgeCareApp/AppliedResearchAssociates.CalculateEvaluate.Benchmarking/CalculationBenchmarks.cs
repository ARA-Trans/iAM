using System.CodeDom.Compiler;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using LegacyCalculateEvaluate = CalculateEvaluate.CalculateEvaluate;

namespace AppliedResearchAssociates.CalculateEvaluate.Benchmarking
{
    public class CalculationBenchmarks
    {
        public IEnumerable<string> CalculationExpressions
        {
            get
            {
                yield return "250*[DECK_AREA]";
            }
        }

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(CalculationExpressions))]
        public CompilerResults CompileAssembly(string expression)
        {
            LegacyCompiler.BuildTemporaryClass(expression, true);
            return LegacyCompiler.CompileAssembly();
        }

        [Benchmark]
        [ArgumentsSource(nameof(CalculationExpressions))]
        public Calculator GetCalculator(string expression) => Compiler.GetCalculator(expression);

        [GlobalSetup]
        public void Setup()
        {
            Compiler = new CalculateEvaluateCompiler();
            Compiler.ParameterTypes["deck_area"] = ParameterType.Number;

            LegacyCompiler = new LegacyCalculateEvaluate();
        }

        private CalculateEvaluateCompiler Compiler;
        private LegacyCalculateEvaluate LegacyCompiler;
    }
}
