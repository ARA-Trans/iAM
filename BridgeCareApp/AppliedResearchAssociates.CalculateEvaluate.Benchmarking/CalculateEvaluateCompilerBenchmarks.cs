using System.CodeDom.Compiler;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using LegacyCalculateEvaluate = CalculateEvaluate.CalculateEvaluate;

namespace AppliedResearchAssociates.CalculateEvaluate.Benchmarking
{
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    public class CalculateEvaluateCompilerBenchmarks
    {
        public CalculateEvaluateCompilerBenchmarks()
        {
            Compiler = new CalculateEvaluateCompiler();
            Compiler.ParameterTypes["deck_area"] = ParameterType.Number;
            Compiler.ParameterTypes["district"] = ParameterType.String;
            Compiler.ParameterTypes["family_id"] = ParameterType.String;

            LegacyCompiler = new LegacyCalculateEvaluate();
        }

        public IEnumerable<string> CalculationExpressions
        {
            get
            {
                yield return "250*[DECK_AREA]";
            }
        }

        public IEnumerable<string> EvaluationExpressions
        {
            get
            {
                yield return "[DISTRICT]=|03| AND [FAMILY_ID]=|1|";
            }
        }

        [Benchmark]
        [ArgumentsSource(nameof(EvaluationExpressions))]
        public string AnnotateParameterReferenceTypes(string expression) => Compiler.AnnotateParameterReferenceTypes(expression);

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(CATEGORY_CALCULATE)]
        [ArgumentsSource(nameof(CalculationExpressions))]
        public CompilerResults CompileAssembly_Calculation(string expression)
        {
            LegacyCompiler.BuildTemporaryClass(expression, true);
            return LegacyCompiler.CompileAssembly();
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(CATEGORY_EVALUATE)]
        [ArgumentsSource(nameof(EvaluationExpressions))]
        public CompilerResults CompileAssembly_Evaluation(string expression)
        {
            var annotatedExpression = Compiler.AnnotateParameterReferenceTypes(expression);
            LegacyCompiler.BuildTemporaryClass(annotatedExpression, false);
            return LegacyCompiler.CompileAssembly();
        }

        [Benchmark]
        [BenchmarkCategory(CATEGORY_CALCULATE)]
        [ArgumentsSource(nameof(CalculationExpressions))]
        public Calculator GetCalculator(string expression) => Compiler.GetCalculator(expression);

        [Benchmark]
        [BenchmarkCategory(CATEGORY_EVALUATE)]
        [ArgumentsSource(nameof(EvaluationExpressions))]
        public Evaluator GetEvaluator(string expression) => Compiler.GetEvaluator(expression);

        private const string CATEGORY_CALCULATE = "Calculate";
        private const string CATEGORY_EVALUATE = "Evaluate";
        private CalculateEvaluateCompiler Compiler;
        private LegacyCalculateEvaluate LegacyCompiler;
    }
}
