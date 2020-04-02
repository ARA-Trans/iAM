using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Antlr4.Runtime;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public delegate T Calculator<T>(CalculatorArgument<T> argument);

    public delegate T Evaluator<T>(EvaluatorArgument<T> argument);

    public sealed class CalculateEvaluateCompiler
    {
        public IDictionary<string, ParameterType> Parameters => _Parameters;

        public Calculator<double> GetCalculator(string expression) => (Calculator<double>)Compile(expression, double.Parse);

        public Evaluator<double> GetEvaluator(string expression) => (Evaluator<double>)Compile(expression, double.Parse);

        public Calculator<decimal> GetMonetaryCalculator(string expression) => (Calculator<decimal>)Compile(expression, decimal.Parse);

        public Evaluator<decimal> GetMonetaryEvaluator(string expression) => (Evaluator<decimal>)Compile(expression, decimal.Parse);

        private readonly ConcurrentDictionary<string, ParameterType> _Parameters = new ConcurrentDictionary<string, ParameterType>(StringComparer.OrdinalIgnoreCase);

        private readonly ConcurrentDictionary<(string, int)[], WeakReference<Delegate>> Cache = new ConcurrentDictionary<(string, int)[], WeakReference<Delegate>>(SequenceEqualityComparer.With(ValueTupleEqualityComparer.Create<string, int>(StringComparer.OrdinalIgnoreCase)));

        private readonly ConcurrentBag<(string, int)[]> InvalidatedKeys = new ConcurrentBag<(string, int)[]>(); // TODO: supercat's "finasposer"

        private Delegate Compile<T>(string expression, Func<string, T> parseNumber)
        {
            // allow cache toggle on/off, then benchmark.

            while (InvalidatedKeys.TryTake(out var invalidatedKey))
            {
                _ = Cache.TryRemove(invalidatedKey, out _);
            }

            var input = new AntlrInputStream(expression);
            var lexer = new CalculateEvaluateLexer(input);
            var tokens = lexer.GetAllTokens();

            var key = tokens.Select(token => (token.Text, token.Type)).ToArray();
            if (Cache.TryGetValue(key, out var weakReference))
            {
                if (weakReference.TryGetTarget(out var cachedLambda))
                {
                    return cachedLambda;
                }
            }

            var tokenSource = new ListTokenSource(tokens);
            var tokenStream = new CommonTokenStream(tokenSource);
            var parser = new CalculateEvaluateParser(tokenStream);
            var tree = parser.root();
            var visitor = new CalculateEvaluateCompilerVisitor<T>(_Parameters, parseNumber);
            var lambdaExpression = (LambdaExpression)visitor.Visit(tree);
            var lambda = lambdaExpression.Compile();

            _ = Cache.TryAdd(key, lambda.GetWeakReference());

            return lambda;
        }
    }
}
