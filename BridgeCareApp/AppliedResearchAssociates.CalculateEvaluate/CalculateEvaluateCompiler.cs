using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Antlr4.Runtime;
using MonetaryCalculator = AppliedResearchAssociates.CalculateEvaluate.Calculator<decimal?>;
using MonetaryEvaluator = AppliedResearchAssociates.CalculateEvaluate.Evaluator<decimal?>;
using NumericCalculator = AppliedResearchAssociates.CalculateEvaluate.Calculator<double?>;
using NumericEvaluator = AppliedResearchAssociates.CalculateEvaluate.Evaluator<double?>;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public delegate T Calculator<T>(CalculatorArgument<T> argument);

    public delegate T Evaluator<T>(EvaluatorArgument<T> argument);

    public sealed class CalculateEvaluateCompiler
    {
        public Dictionary<string, ParameterType> Parameters { get; } = new Dictionary<string, ParameterType>(StringComparer.OrdinalIgnoreCase);

        public NumericCalculator GetCalculator(string expression) => (NumericCalculator)Compile(expression, _ => double.Parse(_).AsNullable());

        public NumericEvaluator GetEvaluator(string expression) => (NumericEvaluator)Compile(expression, _ => double.Parse(_).AsNullable());

        public MonetaryCalculator GetMonetaryCalculator(string expression) => (MonetaryCalculator)Compile(expression, _ => decimal.Parse(_).AsNullable());

        public MonetaryEvaluator GetMonetaryEvaluator(string expression) => (MonetaryEvaluator)Compile(expression, _ => decimal.Parse(_).AsNullable());

        private Delegate Compile<T>(string expression, Func<string, T> parseNumber)
        {
            var input = new AntlrInputStream(expression);
            var lexer = new CalculateEvaluateLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CalculateEvaluateParser(tokens);
            var tree = parser.root();
            var visitor = new CalculateEvaluateCompilerVisitor<T>(Parameters, parseNumber);
            var lambdaExpression = (LambdaExpression)visitor.Visit(tree);
            var lambda = lambdaExpression.Compile();
            return lambda;
        }
    }
}
