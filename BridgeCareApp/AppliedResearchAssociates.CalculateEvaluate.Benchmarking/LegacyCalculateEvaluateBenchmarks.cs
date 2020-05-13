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

        private static readonly IReadOnlyDictionary<ParameterType, string> AnnotationPerType = new Dictionary<ParameterType, string>
        {
            [ParameterType.Number] = "",
            [ParameterType.Text] = "@",
            [ParameterType.Timestamp] = "$",
        };

        private static readonly Regex ParameterReferencePattern = new Regex(@"\[([a-zA-Z_][a-zA-Z_0-9]*)]", RegexOptions.Compiled);

        private static readonly IReadOnlyDictionary<string, ParameterType> TypePerParameter = new Dictionary<string, ParameterType>(StringComparer.OrdinalIgnoreCase)
        {
            ["district"] = ParameterType.Text,
            ["family_id"] = ParameterType.Text,
        };

        private readonly LegacyCalculateEvaluate LegacyCompiler = new LegacyCalculateEvaluate();
    }
}
