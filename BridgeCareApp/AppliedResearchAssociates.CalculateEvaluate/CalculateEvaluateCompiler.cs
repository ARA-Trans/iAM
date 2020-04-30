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
        public Dictionary<string, ParameterType> ParameterTypes { get; } = new Dictionary<string, ParameterType>(StringComparer.OrdinalIgnoreCase);

        public string AnnotateParameterReferenceTypes(string expression)
        {
            var input = new AntlrInputStream(expression);
            var lexer = new CalculateEvaluateBailingLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CalculateEvaluateBailingParser(tokens);

            IParseTree tree;
            try
            {
                tree = parser.root();
            }
            catch (ParseCanceledException e)
            {
                throw new CalculateEvaluateParsingException(null, e);
            }

            var listener = new ParameterReferenceTypeAnnotatorListener(ParameterTypes, tokens);
            ParseTreeWalker.Default.Walk(listener, tree);
            var annotatedExpression = listener.Rewriter.GetText();
            return annotatedExpression;
        }

        public Calculator GetCalculator(string expression) => Compile<Calculator>(expression);

        public Evaluator GetEvaluator(string expression) => Compile<Evaluator>(expression);

        private static readonly IEqualityComparer<IEnumerable<(int, string)>> CacheComparer = SequenceEqualityComparer.Create(ValueTupleEqualityComparer.Create<int, string>(t2: StringComparer.OrdinalIgnoreCase));

        private readonly ConcurrentDictionary<(int, string)[], WeakReference<FinalActor<Delegate>>> Cache = new ConcurrentDictionary<(int, string)[], WeakReference<FinalActor<Delegate>>>(CacheComparer);

        private readonly ConcurrentBag<(int, string)[]> InvalidatedCacheKeys = new ConcurrentBag<(int, string)[]>();

        private T Compile<T>(string expression) where T : Delegate
        {
            while (InvalidatedCacheKeys.TryTake(out var invalidatedCacheKey))
            {
                _ = Cache.TryRemove(invalidatedCacheKey, out _);
            }

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

            _ = Cache.TryAdd(cacheKey, ((Delegate)lambda).WithFinalAction(_ => InvalidatedCacheKeys.Add(cacheKey)).GetWeakReference());

            return lambda;
        }
    }
}
