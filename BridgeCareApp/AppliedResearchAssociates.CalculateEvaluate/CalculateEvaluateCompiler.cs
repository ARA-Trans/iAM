using System;
using System.Collections.Generic;
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

        public Calculator GetCalculator(string expression) => Compile<Calculator>(expression, parser => parser.calculationRoot());

        public Evaluator GetEvaluator(string expression) => Compile<Evaluator>(expression, parser => parser.evaluationRoot());

        private T Compile<T>(string expression, Func<CalculateEvaluateParser, IParseTree> getParseTree) where T : Delegate
        {
            var input = new AntlrInputStream(expression);
            var lexer = new CalculateEvaluateLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CalculateEvaluateParser(tokens);
            var tree = getParseTree(parser);
            var visitor = new CalculateEvaluateCompilerVisitor(ParameterTypes);
            var lambdaExpression = (Expression<T>)visitor.Visit(tree);
            var lambda = lambdaExpression.Compile();
            return lambda;
        }
    }
}
