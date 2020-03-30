using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculateEvaluateCompiler
    {
        public Dictionary<string, Type> Parameters { get; } = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public Func<CalculationArguments, double> GetCalculator(string expression)
        {
            var listener = GetListener(expression, parser => parser.r());
            return listener.Calculator;
        }

        public Func<EvaluationArguments, bool> GetEvaluator(string expression)
        {
            var listener = GetListener(expression, parser => parser.r());
            return listener.Evaluator;
        }

        private readonly ConcurrentDictionary<TokenKey[], WeakReference<CalculateEvaluateCompilerListener>> Cache = new ConcurrentDictionary<TokenKey[], WeakReference<CalculateEvaluateCompilerListener>>(SequenceEqualityComparer<TokenKey>.Instance);

        private readonly ConcurrentBag<TokenKey[]> InvalidatedKeys = new ConcurrentBag<TokenKey[]>();

        private CalculateEvaluateCompilerListener GetListener(string expression, Func<CalculateEvaluateParser, IParseTree> getParseTree)
        {
            var input = new AntlrInputStream(expression);
            var lexer = new CalculateEvaluateLexer(input);
            var tokens = lexer.GetAllTokens();

            var tokenKeys = tokens.Select(TokenKey.Of).ToArray();

            //var containsKey = false;
            //if (Cache.TryGetValue(tokenKeys, out var value))
            //{
            //    if (value.TryGetTarget(out var listener))
            //    {
            //        return listener;
            //    }
            //}

            var tokenSource = new ListTokenSource(tokens);
            var tokenStream = new CommonTokenStream(tokenSource);
            var parser = new CalculateEvaluateParser(tokenStream);
            var tree = getParseTree(parser);
            var listener = new CalculateEvaluateCompilerListener(Parameters);
            ParseTreeWalker.Default.Walk(listener, tree);

            //Cache[tokenKeys] = listener.GetWeakReference();

            return listener;
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
