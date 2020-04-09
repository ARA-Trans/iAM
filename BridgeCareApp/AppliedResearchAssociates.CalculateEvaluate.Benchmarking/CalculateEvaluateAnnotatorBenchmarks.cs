using BenchmarkDotNet.Attributes;

namespace AppliedResearchAssociates.CalculateEvaluate.Benchmarking
{
    public class CalculateEvaluateAnnotatorBenchmarks
    {
        public CalculateEvaluateAnnotatorBenchmarks()
        {
            Compiler.ParameterTypes["district"] = ParameterType.String;
            Compiler.ParameterTypes["family_id"] = ParameterType.String;
        }

        [Benchmark]
        public string AnnotateParameterReferenceTypes() => Compiler.AnnotateParameterReferenceTypes("[DISTRICT]=|03| AND [FAMILY_ID]=|1|");

        private readonly CalculateEvaluateCompiler Compiler = new CalculateEvaluateCompiler();
    }
}
