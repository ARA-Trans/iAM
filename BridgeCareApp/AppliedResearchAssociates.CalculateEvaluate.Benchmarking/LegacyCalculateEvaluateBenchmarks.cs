using System.CodeDom.Compiler;
using BenchmarkDotNet.Attributes;
using LegacyCalculateEvaluate = CalculateEvaluate.CalculateEvaluate;

namespace AppliedResearchAssociates.CalculateEvaluate.Benchmarking
{
    public class LegacyCalculateEvaluateBenchmarks
    {
        public LegacyCalculateEvaluateBenchmarks()
        {
            Compiler.ParameterTypes["district"] = ParameterType.Text;
            Compiler.ParameterTypes["family_id"] = ParameterType.Text;
        }

        [Benchmark]
        public CompilerResults CompileAssembly_Calculation()
        {
            var expression = FIXED_CALCULATION;
            LegacyCompiler.BuildTemporaryClass(expression, true);
            return LegacyCompiler.CompileAssembly();
        }

        [Benchmark]
        public CompilerResults CompileAssembly_Evaluation()
        {
            var expression = FIXED_EVALUATION;
            var annotatedExpression = Compiler.AnnotateParameterReferenceTypes(expression);
            LegacyCompiler.BuildTemporaryClass(annotatedExpression, false);
            return LegacyCompiler.CompileAssembly();
        }

        private const string FIXED_CALCULATION = "0*[DECK_AREA]";
        private const string FIXED_EVALUATION = "[DISTRICT]=|03| AND [FAMILY_ID]=|0|";

        private readonly CalculateEvaluateCompiler Compiler = new CalculateEvaluateCompiler();
        private readonly LegacyCalculateEvaluate LegacyCompiler = new LegacyCalculateEvaluate();
    }
}
