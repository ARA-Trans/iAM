using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Antlr4.Runtime;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public delegate T Calculator<T>(CalculatorArgument<T> argument);

    public delegate T Evaluator<T>(EvaluatorArgument<T> argument);

    public sealed class CalculateEvaluateCompiler
    {
        public Calculator<double> GetCalculator(string expression) => (Calculator<double>)Compile(expression, double.Parse);

        public Evaluator<double> GetEvaluator(string expression) => (Evaluator<double>)Compile(expression, double.Parse);

        public Calculator<decimal> GetMonetaryCalculator(string expression) => (Calculator<decimal>)Compile(expression, decimal.Parse);

        public Evaluator<decimal> GetMonetaryEvaluator(string expression) => (Evaluator<decimal>)Compile(expression, decimal.Parse);

        public Dictionary<string, ParameterType> Parameters { get; } = new Dictionary<string, ParameterType>(StringComparer.OrdinalIgnoreCase);

        private Delegate Compile<T>(string expression, Func<string, T> parseNumber)
        {
            var input = new AntlrInputStream(expression);
            var lexer = new CalculateEvaluateLexer(input);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CalculateEvaluateParser(tokenStream);
            var tree = parser.root();
            var visitor = new CalculateEvaluateCompilerVisitor<T>(Parameters, parseNumber);
            var lambdaExpression = (LambdaExpression)visitor.Visit(tree);
            var lambda = lambdaExpression.Compile();
            return lambda;
        }
    }
}
