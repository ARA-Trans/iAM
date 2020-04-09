using System;
using BenchmarkDotNet.Attributes;

namespace AppliedResearchAssociates.CalculateEvaluate.Benchmarking
{
    public class CalculateEvaluateCompilerBenchmarks
    {
        public CalculateEvaluateCompilerBenchmarks()
        {
            Compiler.ParameterTypes["deck_area"] = ParameterType.Number;
            Compiler.ParameterTypes["district"] = ParameterType.String;
            Compiler.ParameterTypes["family_id"] = ParameterType.String;
        }

        [Params(.2, .5, .8)]
        public double ProbabilityOfDuplicate { get; set; }

        [Benchmark]
        public Calculator GetCalculator()
        {
            var expression = GetCalculationExpression();
            return Compiler.GetCalculator(expression);
        }

        [Benchmark]
        public Evaluator GetEvaluator()
        {
            var expression = GetEvaluationExpression();
            return Compiler.GetEvaluator(expression);
        }

        private const string FIXED_CALCULATION = "0*[DECK_AREA]";
        private const string FIXED_EVALUATION = "[DISTRICT]=|03| AND [FAMILY_ID]=|0|";

        private readonly CalculateEvaluateCompiler Compiler = new CalculateEvaluateCompiler();
        private readonly Random Random = new Random(56245299);

        private int Constant = 1;

        private string GetCalculationExpression() => Random.NextDouble() < ProbabilityOfDuplicate ? FIXED_CALCULATION : $"{NextConstant()}*[DECK_AREA]";

        private string GetEvaluationExpression() => Random.NextDouble() < ProbabilityOfDuplicate ? FIXED_EVALUATION : $"[DISTRICT]=|03| AND [FAMILY_ID]=|{NextConstant()}|";

        private int NextConstant()
        {
            unchecked
            {
                return Constant++;
            }
        }
    }
}
