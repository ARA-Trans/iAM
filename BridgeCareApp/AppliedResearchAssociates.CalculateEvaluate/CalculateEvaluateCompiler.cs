using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculateEvaluateCompiler
    {
        public Dictionary<string, Type> Parameters { get; } = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        // Maps a canonical lexed representation of an expression to its post-walk listener. Is this really necessary? Does it actually boost performance?
        private readonly ConcurrentDictionary<string, WeakReference<CalculateEvaluateCompilerListener>> Cache = new ConcurrentDictionary<string, WeakReference<CalculateEvaluateCompilerListener>>();

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

        private CalculateEvaluateCompilerListener GetListener(string expression, Func<CalculateEvaluateParser, IParseTree> getParseTree)
        {
            var input = new AntlrInputStream(expression);
            var lexer = new CalculateEvaluateLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CalculateEvaluateParser(tokens);
            var tree = getParseTree(parser);
            var listener = new CalculateEvaluateCompilerListener(Parameters);
            ParseTreeWalker.Default.Walk(listener, tree);
            return listener;
        }
    }
}
