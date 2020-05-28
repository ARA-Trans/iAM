using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public delegate double Calculator(CalculateEvaluateArgument argument);

    public delegate bool Evaluator(CalculateEvaluateArgument argument);

    public sealed class CalculateEvaluateCompiler
    {
        public static IEnumerable<string> NumberConstantNames => CalculateEvaluateCompilerVisitor.NumberConstants.Keys;

        public static IEnumerable<NumberFunctionDescription> NumberFunctionDescriptors => CalculateEvaluateCompilerVisitor.NumberFunctions;

        public Dictionary<string, CalculateEvaluateParameterType> ParameterTypes { get; } = new Dictionary<string, CalculateEvaluateParameterType>(StringComparer.OrdinalIgnoreCase);

        public Calculator GetCalculator(string expression) => Compile<Calculator>(expression);

        public Evaluator GetEvaluator(string expression) => Compile<Evaluator>(expression);

        private static readonly IEqualityComparer<IEnumerable<(int, string)>> CacheComparer = SequenceEqualityComparer.Create(ValueTupleEqualityComparer.Create<int, string>(comparer2: StringComparer.OrdinalIgnoreCase));

        private readonly ConcurrentDictionary<(int, string)[], WeakReference<FinalActor<Delegate>>> Cache = new ConcurrentDictionary<(int, string)[], WeakReference<FinalActor<Delegate>>>(CacheComparer);

        private T Compile<T>(string expression) where T : Delegate
        {
            var input = new AntlrInputStream(expression);
            var lexer = new CalculateEvaluateBailingLexer(input);

            var tokenList = lexer.GetAllTokens();

            var cacheKey = tokenList
                .Where(token => token.Channel != Lexer.Hidden)
                .Select(token => (token.Type, token.Text))
                .ToArray();

            if (Cache.TryGetValue(cacheKey, out var weakReference))
            {
                if (weakReference.TryGetTarget(out var cachedLambda))
                {
                    return (T)cachedLambda.Value;
                }
            }

            var tokenSource = new ListTokenSource(tokenList);
            var tokens = new CommonTokenStream(tokenSource);

            var parser = new CalculateEvaluateBailingParser(tokens);
            parser.Interpreter.PredictionMode = PredictionMode.SLL; // Fast mode. Not as robust as default, but almost always works.

            IParseTree tree;
            try
            {
                tree = parser.root();
            }
            catch (ParseCanceledException)
            {
                tokens.Reset();
                parser.Reset();
                parser.Interpreter.PredictionMode = PredictionMode.LL; // Default, robust mode. Fails only when successful parse is impossible (for ANTLR, anyway).

                try
                {
                    tree = parser.root();
                }
                catch (ParseCanceledException e)
                {
                    throw new CalculateEvaluateParsingException(null, e);
                }
            }

            var visitor = new CalculateEvaluateCompilerVisitor(ParameterTypes);
            var lambdaExpression = (Expression<T>)visitor.Visit(tree);
            var lambda = lambdaExpression.Compile();

            _ = Cache.TryAdd(cacheKey, ((Delegate)lambda).WithFinalAction(actor => Cache.TryRemove(cacheKey, out _)).GetWeakReference());

            return lambda;
        }
    }
}
