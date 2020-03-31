using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculateEvaluateCompiler
    {
        public Dictionary<string, ParameterType> Parameters { get; } = new Dictionary<string, ParameterType>(StringComparer.OrdinalIgnoreCase);

        public Func<CalculationArguments, double> GetCalculator(string expressionText)
        {
            var calculator = Compile<Func<CalculationArguments, double>>(expressionText);
            return calculator;
        }

        public Func<EvaluationArguments, bool> GetEvaluator(string expressionText)
        {
            var evaluator = Compile<Func<EvaluationArguments, bool>>(expressionText);
            return evaluator;
        }

        // Cache-maintenance logic needs work.
        private readonly ConcurrentDictionary<TokenKey[], WeakReference<Delegate>> Cache = new ConcurrentDictionary<TokenKey[], WeakReference<Delegate>>(SequenceEqualityComparer<TokenKey>.Instance);

        private readonly ConcurrentBag<TokenKey[]> InvalidatedKeys = new ConcurrentBag<TokenKey[]>();

        private T Compile<T>(string expressionText) where T : Delegate
        {
            var input = new AntlrInputStream(expressionText);
            var lexer = new CalculateEvaluateLexer(input);
            // query cache

            //var tokens = lexer.GetAllTokens();
            //var tokenKeys = tokens.Select(TokenKey.Of).ToArray();
            //var containsKey = false;
            //if (Cache.TryGetValue(tokenKeys, out var value))
            //{
            //    if (value.TryGetTarget(out var listener))
            //    {
            //        return listener;
            //    }
            //}
            //var tokenSource = new ListTokenSource(tokens);
            //var tokenStream = new CommonTokenStream(tokenSource);

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CalculateEvaluateParser(tokenStream);
            var tree = parser.root();
            var visitor = new CalculateEvaluateCompilerVisitor(Parameters);
            var expression = (LambdaExpression)visitor.Visit(tree);

            //var listener = new CalculateEvaluateCompilerListener(Parameters);
            //ParseTreeWalker.Default.Walk(listener, tree);

            var compiledExpression = (T)expression.Compile();
            // update cache

            //Cache[tokenKeys] = listener.GetWeakReference();

            return compiledExpression;
        }

        private struct TokenKey : IEquatable<TokenKey>
        {
            public string Text { get; private set; }

            public int Type { get; private set; }

            public static TokenKey Of(IToken token) => new TokenKey { Text = token.Text, Type = token.Type };

            public bool Equals(TokenKey other) => Text == other.Text && Type == other.Type;

            public override bool Equals(object obj) => obj is TokenKey key && Equals(key);

            public override int GetHashCode() => HashCode.Combine(Text, Type);
        }
    }
}
