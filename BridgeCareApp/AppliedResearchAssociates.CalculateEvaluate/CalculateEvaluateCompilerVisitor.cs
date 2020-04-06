using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal sealed class CalculateEvaluateCompilerVisitor : CalculateEvaluateBaseVisitor<Expression>
    {
        public CalculateEvaluateCompilerVisitor(IReadOnlyDictionary<string, ParameterType> parameterTypes) => ParameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));

        #region "Calculate"

        public override Expression VisitAddition(CalculateEvaluateParser.AdditionContext context)
        {
            var leftOperand = Visit(context.left);
            var rightOperand = Visit(context.right);
            var result = Expression.AddChecked(leftOperand, rightOperand);
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

        public override Expression VisitDivision(CalculateEvaluateParser.DivisionContext context)
        {
            var leftOperand = Visit(context.left);
            var rightOperand = Visit(context.right);
            var result = Expression.Divide(leftOperand, rightOperand);
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

        public override Expression VisitMultiplication(CalculateEvaluateParser.MultiplicationContext context)
        {
            var leftOperand = Visit(context.left);
            var rightOperand = Visit(context.right);
            var result = Expression.MultiplyChecked(leftOperand, rightOperand);
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
            var number = Numbers.Parse(numberText);
            var result = Expression.Constant(number);
            return result;
        }

        public override Expression VisitNumberParameterReference(CalculateEvaluateParser.NumberParameterReferenceContext context)
        {
            var identifierText = context.parameterReference().IDENTIFIER().GetText();

            //ArgumentInfo argumentInfo;
            //switch (ParameterTypes[identifierText])
            //{
            //case ParameterType.Number:
            //    argumentInfo = Numbers;
            //    break;

            //case ParameterType.String:
            //    argumentInfo = Strings;
            //    break;

            //case ParameterType.Date:
            //    argumentInfo = Dates;
            //    break;

            //default:
            //    throw new InvalidOperationException("Invalid parameter type.");
            //}

            if (ParameterTypes[identifierText] != ParameterType.Number)
            {
                throw new InvalidOperationException("Parameter is not a number.");
            }

            var identifierString = Expression.Constant(identifierText);
            var result = Expression.Property(Numbers.DictionaryExpression, Numbers.IndexerInfo, identifierString);
            return result;
        }

        public override Expression VisitSubtraction(CalculateEvaluateParser.SubtractionContext context)
        {
            var leftOperand = Visit(context.left);
            var rightOperand = Visit(context.right);
            var result = Expression.SubtractChecked(leftOperand, rightOperand);
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

        public override Expression VisitEvaluationRoot(CalculateEvaluateParser.EvaluationRootContext context)
        {
            var body = Visit(context.evaluation());
            var lambda = Expression.Lambda<Evaluator>(body, ArgumentParameter);
            return lambda;
        }

        #endregion "Evaluate"

        private static readonly ParameterExpression ArgumentParameter = Expression.Parameter(typeof(CalculateEvaluateArgument));

        private static readonly ArgumentInfo<DateTime> Dates = GetArgumentInfo(nameof(CalculateEvaluateArgument.Dates), Convert.ToDateTime);

        private static readonly ArgumentInfo<double> Numbers = GetArgumentInfo(nameof(CalculateEvaluateArgument.Numbers), double.Parse);

        private static readonly ArgumentInfo<string> Strings = GetArgumentInfo(nameof(CalculateEvaluateArgument.Strings), Static.Identity);

        private readonly IReadOnlyDictionary<string, ParameterType> ParameterTypes;

        private static ArgumentInfo<T> GetArgumentInfo<T>(string argumentPropertyName, Func<string, T> parse)
        {
            var dictionaryExpression = Expression.Property(ArgumentParameter, argumentPropertyName);
            var indexerInfo = (PropertyInfo)dictionaryExpression.Type.GetDefaultMembers().Single();
            return new ArgumentInfo<T>(dictionaryExpression, indexerInfo, parse);
        }

        private class ArgumentInfo<T>
        {
            public ArgumentInfo(Expression dictionaryExpression, PropertyInfo indexerInfo, Func<string, T> parse)
            {
                DictionaryExpression = dictionaryExpression;
                IndexerInfo = indexerInfo;
                Parse = parse;
            }

            public Expression DictionaryExpression { get; }

            public PropertyInfo IndexerInfo { get; }

            public Func<string, T> Parse { get; }
        }
    }
}
