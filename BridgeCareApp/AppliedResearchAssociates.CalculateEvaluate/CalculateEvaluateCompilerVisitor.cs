using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Antlr4.Runtime.Tree;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal sealed class CalculateEvaluateCompilerVisitor : CalculateEvaluateParserBaseVisitor<Expression>
    {
        public CalculateEvaluateCompilerVisitor(IReadOnlyDictionary<string, ParameterType> parameterTypes) => ParameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));

        #region "Calculate"

        public override Expression VisitAdditionOrSubtraction(CalculateEvaluateParser.AdditionOrSubtractionContext context)
        {
            var leftOperand = Visit(context.left);
            var rightOperand = Visit(context.right);

            Expression result;
            switch (context.operation.Type)
            {
            case CalculateEvaluateLexer.PLUS:
                result = Expression.AddChecked(leftOperand, rightOperand);
                break;

            case CalculateEvaluateLexer.MINUS:
                result = Expression.SubtractChecked(leftOperand, rightOperand);
                break;

            default:
                throw new InvalidOperationException("Operation is neither addition nor subtraction.");
            }

            return result;
        }

        public override Expression VisitCalculationGrouping(CalculateEvaluateParser.CalculationGroupingContext context)
        {
            var result = Visit(context.calculation());
            return result;
        }

        public override Expression VisitCalculationRoot(CalculateEvaluateParser.CalculationRootContext context)
        {
            var body = Visit(context.calculation());
            var lambda = Expression.Lambda<Calculator>(body, ArgumentParameter);
            return lambda;
        }

        public override Expression VisitConstantReference(CalculateEvaluateParser.ConstantReferenceContext context)
        {
            var identifierText = context.IDENTIFIER().GetText();
            var constant = MathConstants[identifierText];
            var result = Expression.Constant(constant);
            return result;
        }

        public override Expression VisitInvocation(CalculateEvaluateParser.InvocationContext context)
        {
            var identifierText = context.IDENTIFIER().GetText();
            var arguments = context.arguments().calculation();
            var method = MathFunctions[(identifierText, arguments.Length)];
            var result = Expression.Call(method, arguments.Select(Visit));
            return result;
        }

        public override Expression VisitMultiplicationOrDivision(CalculateEvaluateParser.MultiplicationOrDivisionContext context)
        {
            var leftOperand = Visit(context.left);
            var rightOperand = Visit(context.right);

            Expression result;
            switch (context.operation.Type)
            {
            case CalculateEvaluateLexer.TIMES:
                result = Expression.MultiplyChecked(leftOperand, rightOperand);
                break;

            case CalculateEvaluateLexer.DIVIDED_BY:
                result = Expression.Divide(leftOperand, rightOperand);
                break;

            default:
                throw new InvalidOperationException("Operation is neither multiplication nor division.");
            }

            return result;
        }

        public override Expression VisitNegation(CalculateEvaluateParser.NegationContext context)
        {
            var operand = Visit(context.calculation());
            var result = Expression.NegateChecked(operand);
            return result;
        }

        public override Expression VisitNumberLiteral(CalculateEvaluateParser.NumberLiteralContext context)
        {
            var numberText = context.NUMBER().GetText();
            var result = Numbers.ParseLiteral(numberText);
            return result;
        }

        public override Expression VisitNumberParameterReference(CalculateEvaluateParser.NumberParameterReferenceContext context)
        {
            var identifierText = context.calculationParameterReference().IDENTIFIER().GetText();
            var parameterType = ParameterTypes[identifierText];

            if (parameterType != ParameterType.Number)
            {
                throw new InvalidOperationException("Parameter is not a number.");
            }

            var identifierString = Expression.Constant(identifierText);
            var result = Expression.Property(Numbers.DictionaryExpression, Numbers.IndexerInfo, identifierString);
            return result;
        }

        private static readonly IReadOnlyDictionary<string, double> MathConstants = GetMathConstants();

        private static readonly IReadOnlyDictionary<(string, int), MethodInfo> MathFunctions = GetMathFunctions();

        private static IReadOnlyDictionary<string, double> GetMathConstants()
        {
            var fields = typeof(Math).GetFields(BindingFlags.Public | BindingFlags.Static);
            var numberFields = fields.Where(field => field.FieldType == typeof(double));
            var result = numberFields.ToDictionary(field => field.Name, field => (double)field.GetValue(null), StringComparer.OrdinalIgnoreCase);
            return result;
        }

        private static IReadOnlyDictionary<(string, int), MethodInfo> GetMathFunctions()
        {
            var methods = typeof(Math).GetMethods(BindingFlags.Public | BindingFlags.Static);
            var numberMethods = methods.Where(method => method.ReturnType == typeof(double) && method.GetParameters().All(parameter => parameter.ParameterType == typeof(double)));
            var result = numberMethods.ToDictionary(method => (method.Name, method.GetParameters().Length), ValueTupleEqualityComparer.Create<string, int>(StringComparer.OrdinalIgnoreCase));
            return result;
        }

        #endregion "Calculate"

        #region "Evaluate"

        public override Expression VisitEqual(CalculateEvaluateParser.EqualContext context)
        {
            var result = GetComparisonExpression(context.evaluationParameterReference(), context.EVALUATION_LITERAL(), Expression.Equal, true);
            return result;
        }

        public override Expression VisitEvaluationGrouping(CalculateEvaluateParser.EvaluationGroupingContext context)
        {
            var result = Visit(context.evaluation());
            return result;
        }

        public override Expression VisitEvaluationRoot(CalculateEvaluateParser.EvaluationRootContext context)
        {
            var body = Visit(context.evaluation());
            var lambda = Expression.Lambda<Evaluator>(body, ArgumentParameter);
            return lambda;
        }

        public override Expression VisitGreaterThan(CalculateEvaluateParser.GreaterThanContext context)
        {
            var result = GetComparisonExpression(context.evaluationParameterReference(), context.EVALUATION_LITERAL(), Expression.GreaterThan, false);
            return result;
        }

        public override Expression VisitGreaterThanOrEqual(CalculateEvaluateParser.GreaterThanOrEqualContext context)
        {
            var result = GetComparisonExpression(context.evaluationParameterReference(), context.EVALUATION_LITERAL(), Expression.GreaterThanOrEqual, false);
            return result;
        }

        public override Expression VisitLessThan(CalculateEvaluateParser.LessThanContext context)
        {
            var result = GetComparisonExpression(context.evaluationParameterReference(), context.EVALUATION_LITERAL(), Expression.LessThan, false);
            return result;
        }

        public override Expression VisitLessThanOrEqual(CalculateEvaluateParser.LessThanOrEqualContext context)
        {
            var result = GetComparisonExpression(context.evaluationParameterReference(), context.EVALUATION_LITERAL(), Expression.LessThanOrEqual, false);
            return result;
        }

        public override Expression VisitLogicalConjunction(CalculateEvaluateParser.LogicalConjunctionContext context)
        {
            var leftOperand = Visit(context.left);
            var rightOperand = Visit(context.right);
            var result = Expression.AndAlso(leftOperand, rightOperand);
            return result;
        }

        public override Expression VisitLogicalDisjunction(CalculateEvaluateParser.LogicalDisjunctionContext context)
        {
            var leftOperand = Visit(context.left);
            var rightOperand = Visit(context.right);
            var result = Expression.OrElse(leftOperand, rightOperand);
            return result;
        }

        public override Expression VisitNotEqual(CalculateEvaluateParser.NotEqualContext context)
        {
            var result = GetComparisonExpression(context.evaluationParameterReference(), context.EVALUATION_LITERAL(), Expression.NotEqual, true);
            return result;
        }

        private Expression GetComparisonExpression(CalculateEvaluateParser.EvaluationParameterReferenceContext evaluationParameterReference, ITerminalNode evaluationLiteral, Func<IndexExpression, ConstantExpression, BinaryExpression> getComparison, bool allowStrings)
        {
            var identifierText = evaluationParameterReference.IDENTIFIER().GetText();
            var parameterType = ParameterTypes[identifierText];

            ArgumentInfo argumentInfo;
            switch (parameterType)
            {
            case ParameterType.Number:
                argumentInfo = Numbers;
                break;

            case ParameterType.Text:
                if (!allowStrings)
                {
                    goto default;
                }
                argumentInfo = Strings;
                break;

            case ParameterType.Timestamp:
                argumentInfo = Dates;
                break;

            default:
                throw new InvalidOperationException("Invalid parameter type.");
            }

            var identifierString = Expression.Constant(identifierText);
            var reference = Expression.Property(argumentInfo.DictionaryExpression, argumentInfo.IndexerInfo, identifierString);

            var literalText = evaluationLiteral.GetText();
            var literalContent = literalText.Substring(1, literalText.Length - 2);
            var literal = argumentInfo.ParseLiteral(literalContent);

            var result = getComparison(reference, literal);
            return result;
        }

        #endregion "Evaluate"

        private static readonly ParameterExpression ArgumentParameter = Expression.Parameter(typeof(CalculateEvaluateArgument), "arg");

        private static readonly ArgumentInfo Dates = GetArgumentInfo(nameof(CalculateEvaluateArgument.Timestamp), Convert.ToDateTime);

        private static readonly ArgumentInfo Numbers = GetArgumentInfo(nameof(CalculateEvaluateArgument.Number), double.Parse);

        private static readonly ArgumentInfo Strings = GetArgumentInfo(nameof(CalculateEvaluateArgument.Text), Static.Identity);

        private readonly IReadOnlyDictionary<string, ParameterType> ParameterTypes;

        private static ArgumentInfo GetArgumentInfo<T>(string argumentPropertyName, Func<string, T> parse)
        {
            var dictionaryExpression = Expression.Property(ArgumentParameter, argumentPropertyName);
            var indexerInfo = (PropertyInfo)dictionaryExpression.Type.GetDefaultMembers().Single();
            return new ArgumentInfo(dictionaryExpression, indexerInfo, literal => Expression.Constant(parse(literal)));
        }

        private class ArgumentInfo
        {
            public ArgumentInfo(Expression dictionaryExpression, PropertyInfo indexerInfo, Func<string, ConstantExpression> parseLiteral)
            {
                DictionaryExpression = dictionaryExpression;
                IndexerInfo = indexerInfo;
                ParseLiteral = parseLiteral;
            }

            public Expression DictionaryExpression { get; }

            public PropertyInfo IndexerInfo { get; }

            public Func<string, ConstantExpression> ParseLiteral { get; }
        }
    }
}
