using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculatorCompiler : ICompiler
    {
        public void Compile(string expression, Dictionary<string, ParameterType> parameters)
        {

        }

        public Func<CalculationArguments, double> Calculator { get; private set; }
    }
    public interface ICompiler
    {
        void Compile(string expression, Dictionary<string, ParameterType> parameters);
    }
    public enum ParameterType
    {
        Number,
        String,
        Date,
    }
    public class CalculateEvaluateCompiler
    {
        public static Func<CalculationArguments, double> GetCalculator(string calculationExpression)
        {
            var compiler = GetCompiler(calculationExpression, parser => parser.r());
            return compiler.Calculator;
        }

        public static Func<EvaluationArguments, bool> GetEvaluator(string evaluationExpression)
        {
            var compiler = GetCompiler(evaluationExpression, parser => parser.r());
            return compiler.Evaluator;
        }

        private static CalculateEvaluateCompilerListener GetCompiler(string expression, Func<CalculateEvaluateParser, IParseTree> getParseTree)
        {
            var input = new AntlrInputStream(expression);
            var lexer = new CalculateEvaluateLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CalculateEvaluateParser(tokens);
            var tree = getParseTree(parser);
            var compiler = new CalculateEvaluateCompilerListener();
            ParseTreeWalker.Default.Walk(compiler, tree);
            return compiler;
        }
    }
}
