using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using LegacyCalculateEvaluate = CalculateEvaluate.CalculateEvaluate;

namespace AppliedResearchAssociates.CalculateEvaluate.Benchmarking
{
    public class LegacyCalculateEvaluateBenchmarks
    {
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
            var annotatedExpression = ParameterReferencePattern.Replace(expression, match => match.Value.Insert(1, AnnotationPerType[TypePerParameter[match.Groups[1].Value]]));
            LegacyCompiler.BuildTemporaryClass(annotatedExpression, false);
            return LegacyCompiler.CompileAssembly();
        }

        private const string FIXED_CALCULATION = "0*[DECK_AREA]";
        private const string FIXED_EVALUATION = "[DISTRICT]=|03| AND [FAMILY_ID]=|0|";

        private static readonly IReadOnlyDictionary<CalculateEvaluateParameterType, string> AnnotationPerType = new Dictionary<CalculateEvaluateParameterType, string>
        {
            [CalculateEvaluateParameterType.Number] = "",
            [CalculateEvaluateParameterType.Text] = "@",
            [CalculateEvaluateParameterType.Timestamp] = "$",
        };

        private static readonly Regex ParameterReferencePattern = new Regex(@"\[([a-zA-Z_][a-zA-Z_0-9]*)]", RegexOptions.Compiled);

        private static readonly IReadOnlyDictionary<string, CalculateEvaluateParameterType> TypePerParameter = new Dictionary<string, CalculateEvaluateParameterType>(StringComparer.OrdinalIgnoreCase)
        {
            ["district"] = CalculateEvaluateParameterType.Text,
            ["family_id"] = CalculateEvaluateParameterType.Text,
        };

        private readonly LegacyCalculateEvaluate LegacyCompiler = new LegacyCalculateEvaluate();
    }
}
