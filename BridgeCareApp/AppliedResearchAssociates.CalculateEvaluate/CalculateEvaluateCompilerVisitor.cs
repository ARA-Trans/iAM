using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal class CalculateEvaluateCompilerVisitor : CalculateEvaluateBaseVisitor<Expression>
    {
        public CalculateEvaluateCompilerVisitor(Dictionary<string, ParameterType> parameters) => Parameters = new Dictionary<string, ParameterType>(parameters, parameters.Comparer);

        public override Expression VisitCalculation(CalculateEvaluateParser.CalculationContext context)
        {
            var parameter = Expression.Parameter(typeof(CalculationArguments));

            Numbers = GetArgumentsInfo(parameter, nameof(CalculationArguments.Numbers));
            Dates = null;
            Strings = null;

            var body = Visit(context.calc());
            var lambda = Expression.Lambda(body, parameter);
            return lambda;
        }

        public override Expression VisitNumericLiteral(CalculateEvaluateParser.NumericLiteralContext context)
        {
            var numberText = context.NUMBER().GetText();
            var number = double.Parse(numberText);
            var result = Expression.Constant(number);
            return result;
        }

        public override Expression VisitParameterReference(CalculateEvaluateParser.ParameterReferenceContext context)
        {
            var idText = context.ID().GetText();
            var id = Expression.Constant(idText);

            ArgumentsInfo argumentsInfo;
            switch (Parameters[idText])
            {
            case ParameterType.Number:
                argumentsInfo = Numbers;
                break;

            case ParameterType.Date:
                argumentsInfo = Dates;
                break;

            case ParameterType.String:
                argumentsInfo = Strings;
                break;

            default:
                throw new InvalidOperationException("Invalid parameter type.");
            }

            var result = Expression.Property(argumentsInfo.DictionaryExpression, argumentsInfo.IndexerInfo, id);
            return result;
        }

        private readonly IReadOnlyDictionary<string, ParameterType> Parameters;

        private ArgumentsInfo Dates;

        private ArgumentsInfo Numbers;

        private ArgumentsInfo Strings;

        private ArgumentsInfo GetArgumentsInfo(Expression argumentsParameter, string argumentsPropertyName)
        {
            var dictionaryExpression = Expression.Property(argumentsParameter, argumentsPropertyName);
            var indexerInfo = (PropertyInfo)dictionaryExpression.Type.GetDefaultMembers().Single();
            return new ArgumentsInfo(dictionaryExpression, indexerInfo);
        }

        private class ArgumentsInfo
        {
            public ArgumentsInfo(Expression dictionaryExpression, PropertyInfo indexerInfo)
            {
                DictionaryExpression = dictionaryExpression ?? throw new ArgumentNullException(nameof(dictionaryExpression));
                IndexerInfo = indexerInfo ?? throw new ArgumentNullException(nameof(indexerInfo));
            }

            public Expression DictionaryExpression { get; }

            public PropertyInfo IndexerInfo { get; }
        }
    }
}
