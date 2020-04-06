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

        public string AnnotateParameterReferenceTypes(string expression)
        {
            try
            {
                var parsingInfo = new ParsingInfo(expression);
                var listener = new ParameterReferenceTypeAnnotatorListener(ParameterTypes, parsingInfo.Tokens);
                ParseTreeWalker.Default.Walk(listener, parsingInfo.Tree);
                var annotatedExpression = listener.Rewriter.GetText();
                return annotatedExpression;
            }
            catch (Exception e)
            {
                throw new CalculateEvaluateException("Error during annotation of expression.", e);
            }
        }

        public Calculator GetCalculator(string expression) => Compile<Calculator>(expression);

        public Evaluator GetEvaluator(string expression) => Compile<Evaluator>(expression);

        private T Compile<T>(string expression) where T : Delegate
        {
            try
            {
                var parsingInfo = new ParsingInfo(expression);
                var visitor = new CalculateEvaluateCompilerVisitor(ParameterTypes);
                var lambdaExpression = (Expression<T>)visitor.Visit(parsingInfo.Tree);
                var lambda = lambdaExpression.Compile();
                return lambda;
            }
            catch (Exception e)
            {
                throw new CalculateEvaluateException("Error during compilation of expression.", e);
            }
        }

        private sealed class ParameterReferenceTypeAnnotatorListener : CalculateEvaluateBaseListener
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

        private sealed class ParsingInfo
        {
            public ParsingInfo(string expression)
            {
                var input = new AntlrInputStream(expression);
                var lexer = new CalculateEvaluateLexer(input);
                Tokens = new CommonTokenStream(lexer);
                var parser = new CalculateEvaluateParser(Tokens);
                Tree = parser.root();
            }

            public ITokenStream Tokens { get; }

            public IParseTree Tree { get; }
        }
    }
}
