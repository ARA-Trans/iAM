using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal sealed class CalculateEvaluateCompilerVisitor<T> : CalculateEvaluateBaseVisitor<Expression>
    {
        public CalculateEvaluateCompilerVisitor(IReadOnlyDictionary<string, ParameterType> parameters, Func<string, T> parseNumber)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            ParseNumber = parseNumber ?? throw new ArgumentNullException(nameof(parseNumber));
        }

        public override Expression VisitAddition(CalculateEvaluateParser.AdditionContext context)
        {
            var leftOperand = Visit(context.left);
            var rightOperand = Visit(context.right);
            var result = Expression.AddChecked(leftOperand, rightOperand);
            return result;
        }

        public override Expression VisitCalculation(CalculateEvaluateParser.CalculationContext context)
        {
            var parameter = Expression.Parameter(typeof(CalculatorArgument<T>));
            Numbers = ArgumentInfo.Of(parameter, nameof(CalculatorArgument<T>.Numbers));
            var body = Visit(context.calc());
            var lambda = Expression.Lambda<Calculator<T>>(body, parameter);
            return lambda;
        }

        public override Expression VisitConstantReference(CalculateEvaluateParser.ConstantReferenceContext context)
        {
            var idText = context.ID().GetText();
            var constant = Constants[idText];
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

        public override Expression VisitEvaluation(CalculateEvaluateParser.EvaluationContext context)
        {
            var parameter = Expression.Parameter(typeof(EvaluatorArgument<T>));
            Numbers = ArgumentInfo.Of(parameter, nameof(EvaluatorArgument<T>.Numbers));
            Strings = ArgumentInfo.Of(parameter, nameof(EvaluatorArgument<T>.Strings));
            Dates = ArgumentInfo.Of(parameter, nameof(EvaluatorArgument<T>.Dates));
            var body = Visit(context.eval());
            var lambda = Expression.Lambda<Evaluator<T>>(body, parameter);
            return lambda;
        }

        public override Expression VisitGrouping(CalculateEvaluateParser.GroupingContext context)
        {
            var result = Visit(context.calc());
            return result;
        }

        public override Expression VisitInvocation(CalculateEvaluateParser.InvocationContext context)
        {
            var idText = context.ID().GetText();
            var arguments = context.args().calc();
            var method = Functions[(idText, arguments.Length)];
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
            var operand = Visit(context.calc());
            var result = Expression.NegateChecked(operand);
            return result;
        }

        public override Expression VisitNumericLiteral(CalculateEvaluateParser.NumericLiteralContext context)
        {
            var numberText = context.NUMBER().GetText();
            var number = ParseNumber(numberText);
            var result = Expression.Constant(number);
            return result;
        }

        public override Expression VisitParameterReference(CalculateEvaluateParser.ParameterReferenceContext context)
        {
            var idText = context.ID().GetText();

            ArgumentInfo argumentInfo;
            switch (Parameters[idText])
            {
            case ParameterType.Number:
                argumentInfo = Numbers;
                break;

            case ParameterType.String:
                argumentInfo = Strings;
                break;

            case ParameterType.Date:
                argumentInfo = Dates;
                break;

            default:
                throw new InvalidOperationException("Invalid parameter type.");
            }

            var id = Expression.Constant(idText);
            var result = Expression.Property(argumentInfo.DictionaryExpression, argumentInfo.IndexerInfo, id);
            return result;
        }

        public override Expression VisitSubtraction(CalculateEvaluateParser.SubtractionContext context)
        {
            var leftOperand = Visit(context.left);
            var rightOperand = Visit(context.right);
            var result = Expression.SubtractChecked(leftOperand, rightOperand);
            return result;
        }

        private static readonly IReadOnlyDictionary<string, T> Constants =
            typeof(Math).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(T))
            .ToDictionary(field => field.Name, field => (T)field.GetValue(null), StringComparer.OrdinalIgnoreCase);

        private static readonly IReadOnlyDictionary<(string, int), MethodInfo> Functions =
            typeof(Math).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.ReturnType == typeof(T) && method.GetParameters().All(parameter => parameter.ParameterType == typeof(T)))
            .ToDictionary(method => (method.Name, method.GetParameters().Length), ValueTupleEqualityComparer.Create<string, int>(StringComparer.OrdinalIgnoreCase));

        private readonly IReadOnlyDictionary<string, ParameterType> Parameters;

        private readonly Func<string, T> ParseNumber;

        private ArgumentInfo Dates;

        private ArgumentInfo Numbers;

        private ArgumentInfo Strings;

        private class ArgumentInfo
        {
            public Expression DictionaryExpression { get; }

            public PropertyInfo IndexerInfo { get; }

            public static ArgumentInfo Of(Expression argumentParameter, string argumentPropertyName)
            {
                var dictionaryExpression = Expression.Property(argumentParameter, argumentPropertyName);
                var indexerInfo = (PropertyInfo)dictionaryExpression.Type.GetDefaultMembers().Single();
                return new ArgumentInfo(dictionaryExpression, indexerInfo);
            }

            private ArgumentInfo(Expression dictionaryExpression, PropertyInfo indexerInfo)
            {
                DictionaryExpression = dictionaryExpression ?? throw new ArgumentNullException(nameof(dictionaryExpression));
                IndexerInfo = indexerInfo ?? throw new ArgumentNullException(nameof(indexerInfo));
            }
        }
    }
}
