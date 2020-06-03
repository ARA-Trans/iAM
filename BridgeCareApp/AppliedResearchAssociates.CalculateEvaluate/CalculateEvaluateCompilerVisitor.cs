using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal sealed class CalculateEvaluateCompilerVisitor : CalculateEvaluateParserBaseVisitor<Expression>
    {
        public CalculateEvaluateCompilerVisitor(IReadOnlyDictionary<string, CalculateEvaluateParameterType> parameterTypes) => ParameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));

        #region "Calculate"

        static CalculateEvaluateCompilerVisitor()
        {
            var mathMethods = typeof(Math).GetMethods(BindingFlags.Public | BindingFlags.Static);
            var numberMethods = mathMethods.Where(method => method.ReturnType == typeof(double) && method.GetParameters().All(parameter => parameter.ParameterType == typeof(double))).ToArray();
            NumberFunctions = numberMethods.Select(method => new NumberFunctionDescription(method.Name, method.GetParameters().Select(parameter => parameter.Name))).ToArray();
            MethodPerSignature = numberMethods.ToDictionary(method => (method.Name, method.GetParameters().Length), ValueTupleEqualityComparer.Create<string, int>(StringComparer.OrdinalIgnoreCase));
        }

        public static IReadOnlyDictionary<string, double> NumberConstants { get; } = GetNumberConstants();

        public static IReadOnlyCollection<NumberFunctionDescription> NumberFunctions { get; }

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

        public override Expression VisitInvocation(CalculateEvaluateParser.InvocationContext context)
        {
            var identifierText = context.IDENTIFIER().GetText();
            var arguments = context.arguments().calculation();

            if (!MethodPerSignature.TryGetValue((identifierText, arguments.Length), out var method))
            {
                throw new CalculateEvaluateCompilationException("Unknown function or invalid number of arguments.");
            }

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
            var result = Number.ParseLiteral(numberText);
            return result;
        }

        public override Expression VisitNumberParameterReference(CalculateEvaluateParser.NumberParameterReferenceContext context)
        {
            var identifierText = context.IDENTIFIER().GetText();

            if (!TryVisitNumberParameterReference(identifierText, out var result))
            {
                throw UnknownReference;
            }

            return result;
        }

        public override Expression VisitNumberReference(CalculateEvaluateParser.NumberReferenceContext context)
        {
            var identifierText = context.IDENTIFIER().GetText();

            Expression result;
            if (NumberConstants.TryGetValue(identifierText, out var constant))
            {
                result = Expression.Constant(constant);
            }
            else if (!TryVisitNumberParameterReference(identifierText, out result))
            {
                throw UnknownReference;
            }

            return result;
        }

        private static readonly IReadOnlyDictionary<(string, int), MethodInfo> MethodPerSignature;

        private static IReadOnlyDictionary<string, double> GetNumberConstants()
        {
            var mathFields = typeof(Math).GetFields(BindingFlags.Public | BindingFlags.Static);
            var numberFields = mathFields.Where(field => field.FieldType == typeof(double));
            var result = numberFields.ToDictionary(field => field.Name, field => (double)field.GetValue(null), StringComparer.OrdinalIgnoreCase);
            return result;
        }

        private bool TryVisitNumberParameterReference(string identifierText, out Expression result)
        {
            if (!ParameterTypes.TryGetValue(identifierText, out var parameterType))
            {
                result = default;
                return false;
            }

            if (parameterType != CalculateEvaluateParameterType.Number)
            {
                throw new CalculateEvaluateCompilationException("Parameter is not a number.");
            }

            var identifierString = Expression.Constant(identifierText);
            result = Expression.Call(ArgumentParameter, Number.GetterInfo, identifierString);
            return true;
        }

        #endregion "Calculate"

        #region "Evaluate"

        public override Expression VisitEqual(CalculateEvaluateParser.EqualContext context)
        {
            var result = GetComparisonExpression(context.parameterReference(), context.literal(), Expression.Equal, true);
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
            var result = GetComparisonExpression(context.parameterReference(), context.literal(), Expression.GreaterThan, false);
            return result;
        }

        public override Expression VisitGreaterThanOrEqual(CalculateEvaluateParser.GreaterThanOrEqualContext context)
        {
            var result = GetComparisonExpression(context.parameterReference(), context.literal(), Expression.GreaterThanOrEqual, false);
            return result;
        }

        public override Expression VisitLessThan(CalculateEvaluateParser.LessThanContext context)
        {
            var result = GetComparisonExpression(context.parameterReference(), context.literal(), Expression.LessThan, false);
            return result;
        }

        public override Expression VisitLessThanOrEqual(CalculateEvaluateParser.LessThanOrEqualContext context)
        {
            var result = GetComparisonExpression(context.parameterReference(), context.literal(), Expression.LessThanOrEqual, false);
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
            var result = GetComparisonExpression(context.parameterReference(), context.literal(), Expression.NotEqual, true);
            return result;
        }

        private Expression GetComparisonExpression(CalculateEvaluateParser.ParameterReferenceContext parameterReference, CalculateEvaluateParser.LiteralContext evaluationLiteral, Func<MethodCallExpression, ConstantExpression, BinaryExpression> getComparison, bool allowStrings)
        {
            var identifierText = parameterReference.IDENTIFIER().GetText();

            if (!ParameterTypes.TryGetValue(identifierText, out var parameterType))
            {
                throw UnknownReference;
            }

            ArgumentInfo argumentInfo;
            switch (parameterType)
            {
            case CalculateEvaluateParameterType.Number:
                argumentInfo = Number;
                break;

            case CalculateEvaluateParameterType.Text:
                if (!allowStrings)
                {
                    throw new CalculateEvaluateCompilationException("Ordering comparisons do not support text parameters.");
                }
                argumentInfo = Text;
                break;

            case CalculateEvaluateParameterType.Timestamp:
                argumentInfo = Timestamp;
                break;

            default:
                throw new InvalidOperationException("Invalid parameter type.");
            }

            var identifierString = Expression.Constant(identifierText);
            var reference = Expression.Call(ArgumentParameter, argumentInfo.GetterInfo, identifierString);

            var literalContent = evaluationLiteral.content?.Text ?? "";
            var literal = argumentInfo.ParseLiteral(literalContent);

            var result = getComparison(reference, literal);
            return result;
        }

        #endregion "Evaluate"

        private static readonly ParameterExpression ArgumentParameter = Expression.Parameter(typeof(CalculateEvaluateArgument), "arg");

        private static readonly ArgumentInfo Number = GetArgumentInfo(nameof(CalculateEvaluateArgument.GetNumber), double.Parse);

        private static readonly ArgumentInfo Text = GetArgumentInfo(nameof(CalculateEvaluateArgument.GetText), Static.Identity);

        private static readonly ArgumentInfo Timestamp = GetArgumentInfo(nameof(CalculateEvaluateArgument.GetTimestamp), Convert.ToDateTime);

        private readonly IReadOnlyDictionary<string, CalculateEvaluateParameterType> ParameterTypes;

        private static CalculateEvaluateCompilationException UnknownReference => new CalculateEvaluateCompilationException("Unknown reference.");

        private static ArgumentInfo GetArgumentInfo<T>(string argumentGetterName, Func<string, T> parse)
        {
            var getterInfo = typeof(CalculateEvaluateArgument).GetMethod(argumentGetterName);
            return new ArgumentInfo(getterInfo, literal =>
            {
                T value;

                try
                {
                    value = parse(literal);
                }
                catch (Exception e)
                {
                    throw new CalculateEvaluateCompilationException("Failed to parse literal.", e);
                }

                return Expression.Constant(value);
            });
        }

        private sealed class ArgumentInfo
        {
            public ArgumentInfo(MethodInfo getterInfo, Func<string, ConstantExpression> parseLiteral)
            {
                GetterInfo = getterInfo ?? throw new ArgumentNullException(nameof(getterInfo));
                ParseLiteral = parseLiteral ?? throw new ArgumentNullException(nameof(parseLiteral));
            }

            public MethodInfo GetterInfo { get; }

            public Func<string, ConstantExpression> ParseLiteral { get; }
        }
    }
}
