using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Antlr4.Runtime;
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
            var lexer = new CalculateEvaluateLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CalculateEvaluateParser(tokens);
            var tree = parser.root();

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
            var lexer = new CalculateEvaluateLexer(input);
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
            var parser = new CalculateEvaluateParser(tokens);
            var tree = parser.root();

            var visitor = new CalculateEvaluateCompilerVisitor(ParameterTypes);
            var lambdaExpression = (Expression<T>)visitor.Visit(tree);
            var lambda = lambdaExpression.Compile();

            _ = Cache.TryAdd(cacheKey, ((Delegate)lambda).WithFinalAction(_ => InvalidatedCacheKeys.Add(cacheKey)).GetWeakReference());

            return lambda;
        }

        private sealed class ParameterReferenceTypeAnnotatorListener : CalculateEvaluateParserBaseListener
        {
            public ParameterReferenceTypeAnnotatorListener(IReadOnlyDictionary<string, ParameterType> parameterTypes, ITokenStream tokens)
            {
                ParameterTypes = parameterTypes;
                Rewriter = new TokenStreamRewriter(tokens);
            }

            public TokenStreamRewriter Rewriter { get; }

            public override void EnterParameterReference(CalculateEvaluateParser.ParameterReferenceContext context)
            {
                var identifierContext = context.IDENTIFIER();
                var identifierText = identifierContext.GetText();
                var parameterType = ParameterTypes[identifierText];

                switch (parameterType)
                {
                case ParameterType.Number:
                    break;

                case ParameterType.String:
                    insert("@");
                    break;

                case ParameterType.Date:
                    insert("$");
                    break;

                default:
                    throw new InvalidOperationException("Invalid parameter type.");
                }

                void insert(string typeAnnotation) => Rewriter.InsertBefore(identifierContext.Symbol, typeAnnotation);
            }

            private readonly IReadOnlyDictionary<string, ParameterType> ParameterTypes;
        }
    }
}
