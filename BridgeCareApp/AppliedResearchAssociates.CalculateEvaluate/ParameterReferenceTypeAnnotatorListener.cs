using System;
using System.Collections.Generic;
using Antlr4.Runtime;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal sealed class ParameterReferenceTypeAnnotatorListener : CalculateEvaluateParserBaseListener
    {
        public ParameterReferenceTypeAnnotatorListener(IReadOnlyDictionary<string, ParameterType> parameterTypes, ITokenStream tokens)
        {
            ParameterTypes = parameterTypes;
            Rewriter = new TokenStreamRewriter(tokens);
        }

        public TokenStreamRewriter Rewriter { get; }

        public override void EnterEvaluationParameterReference(CalculateEvaluateParser.EvaluationParameterReferenceContext context)
        {
            var annotationNode = context.TYPE_ANNOTATION();
            var identifierNode = context.IDENTIFIER();
            var identifierText = identifierNode.GetText();
            var parameterType = ParameterTypes[identifierText];

            switch (parameterType)
            {
            case ParameterType.Number:
                if (annotationNode != null)
                {
                    Rewriter.Delete(annotationNode.Symbol);
                }
                break;

            case ParameterType.Text:
                update(CalculateEvaluateLexer.TEXT_TYPE_ANNOTATION);
                break;

            case ParameterType.Timestamp:
                update(CalculateEvaluateLexer.TIMESTAMP_TYPE_ANNOTATION);
                break;

            default:
                throw new InvalidOperationException("Invalid parameter type.");
            }

            void update(int annotationTokenType)
            {
                var annotationTokenLiteral = CalculateEvaluateLexer.DefaultVocabulary.GetLiteralName(annotationTokenType);
                var annotationTokenText = annotationTokenLiteral.Substring(1, 1);

                if (annotationNode != null)
                {
                    if (annotationNode.Symbol.Type != annotationTokenType)
                    {
                        Rewriter.Replace(annotationNode.Symbol, annotationTokenText);
                    }
                }
                else
                {
                    Rewriter.InsertBefore(identifierNode.Symbol, annotationTokenText);
                }
            }
        }

        private readonly IReadOnlyDictionary<string, ParameterType> ParameterTypes;
    }
}
