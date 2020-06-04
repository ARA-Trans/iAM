using System;
using System.Linq;
using NUnit.Framework;

namespace AppliedResearchAssociates.CalculateEvaluate.Testing
{
    public class CalculateEvaluateCompilerTests
    {
        [Test]
        public void BadLex() => Assert.Throws<CalculateEvaluateLexingException>(() => ParameterlessCalculation("2 # 2", 4));

        [Test]
        public void BadParse() => Assert.Throws<CalculateEvaluateParsingException>(() => ParameterlessCalculation("2 ( 2", 4));

        #region "Calculate"

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Addition() => ParameterlessCalculation($"{n0} + {n1}", n0 + n1);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void CalculationAssociativity() => MultipleParameterCalculation($"2 + 3 * 4 + 5", 19);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void CalculationGrouping() => ParameterlessCalculation("2 * (3 + 4) * 5", 70);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void ConstantReference() => ParameterlessCalculation("pi", Math.PI);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Division() => ParameterlessCalculation($"{n0} / {n1}", n0 / n1);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Invocation() => ParameterlessCalculation($"log({n0})", Math.Log(n0));

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Multiplication() => ParameterlessCalculation($"{n0} * {n1}", n0 * n1);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Negation() => ParameterlessCalculation($"-{n0}", -n0);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void NumberLiteral() => ParameterlessCalculation($"{n0}", n0);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void NumberParameterReference()
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = CalculateEvaluateParameterType.Number;
            var expression = "[param] * param";
            var calculator = compiler.GetCalculator(expression);
            var argument = new CalculateEvaluateArgument();
            argument.SetNumber("PaRaM", n0);
            var result = calculator(argument);
            Assert.That(result, Is.EqualTo(n0 * n0));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Subtraction() => ParameterlessCalculation($"{n0} - {n1}", n0 - n1);

        #endregion "Calculate"

        #region "Evaluate"

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void CompareWithNumberOperand() => SingleNumberParameterEvaluation($"[param]={n1}", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void CompareWithReferenceOperand() => MultipleNumberParameterEvaluation($"[param0]=param1", Assert.IsFalse, n0, n1);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void EvaluationAssociativity() => MultipleNumberParameterEvaluation($"param0='{n0}' or [param0]='{n1}' and [param1]='{n0}'", Assert.IsTrue, n0, n1);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void EvaluationGrouping() => MultipleNumberParameterEvaluation($"(param0='{n0}' or [param0]='{n1}') and [param1]='{n0}'", Assert.IsFalse, n0, n1);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void LogicalConjunction() => MultipleNumberParameterEvaluation($"[param0]='{n0}' and param1='{n0}'", Assert.IsFalse, n0, n1);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void LogicalDisjunction() => MultipleNumberParameterEvaluation($"[param0]='{n0}' or param1='{n0}'", Assert.IsTrue, n0, n1);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberEqual() => SingleNumberParameterEvaluation($"[param]='{n1}'", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberGreaterThan() => SingleNumberParameterEvaluation($"[param]>|{n1}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberGreaterThanOrEqual() => SingleNumberParameterEvaluation($"[param]>=|{n1}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberLessThan() => SingleNumberParameterEvaluation($"[param]<|{n1}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberLessThanOrEqual() => SingleNumberParameterEvaluation($"[param]<=|{n1}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberNotEqual() => SingleNumberParameterEvaluation($"[param]<>|{n1}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TextEqual() => SingleTextParameterEvaluation($"[param]=''", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TextNotEqual() => SingleTextParameterEvaluation($"[param]<>|{s1}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampEqual() => SingleTimestampParameterEvaluation($"[param]='{d1}'", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampGreaterThan() => SingleTimestampParameterEvaluation($"[param]>|{d1}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampGreaterThanOrEqual() => SingleTimestampParameterEvaluation($"[param]>=|{d1}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampLessThan() => SingleTimestampParameterEvaluation($"[param]<|{d1}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampLessThanOrEqual() => SingleTimestampParameterEvaluation($"[param]<=|{d1}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampNotEqual() => SingleTimestampParameterEvaluation($"[param]<>|{d1}|", Assert.IsTrue);

        #endregion "Evaluate"

        private const string CATEGORY_CALCULATE = "Calculate";

        private const string CATEGORY_EVALUATE = "Evaluate";

        private const double n0 = 19.123, n1 = 23.456;

        private const string s0 = "foo", s1 = "bar";

        private static readonly DateTime d0 = new DateTime(2000, 1, 1), d1 = new DateTime(2020, 1, 1);

        private void MultipleNumberParameterEvaluation(string inputExpression, Action<bool> assert, params double[] parameterValues)
        {
            var parameters = parameterValues.Select(ValueTuple.Create<double, int>).ToArray();
            var compiler = new CalculateEvaluateCompiler();
            foreach (var (_, i) in parameters)
            {
                compiler.ParameterTypes["PARAM" + i] = CalculateEvaluateParameterType.Number;
            }
            var evaluator = compiler.GetEvaluator(inputExpression);
            var argument = new CalculateEvaluateArgument();
            foreach (var (n, i) in parameters)
            {
                argument.SetNumber("PaRaM" + i, n);
            }
            var result = evaluator(argument);
            assert(result);
        }

        private void MultipleParameterCalculation(string inputExpression, double expectedOutput, params double[] parameterValues)
        {
            var parameters = parameterValues.Select(ValueTuple.Create<double, int>).ToArray();
            var compiler = new CalculateEvaluateCompiler();
            foreach (var (_, i) in parameters)
            {
                compiler.ParameterTypes["PARAM" + i] = CalculateEvaluateParameterType.Number;
            }
            var calculator = compiler.GetCalculator(inputExpression);
            var argument = new CalculateEvaluateArgument();
            foreach (var (n, i) in parameters)
            {
                argument.SetNumber("PaRaM" + i, n);
            }
            var result = calculator(argument);
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        private void ParameterlessCalculation(string inputExpression, double expectedOutput)
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator(inputExpression);
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        private void SingleNumberParameterEvaluation(string inputExpression, Action<bool> assert)
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = CalculateEvaluateParameterType.Number;
            var calculator = compiler.GetEvaluator(inputExpression);
            var argument = new CalculateEvaluateArgument();
            argument.SetNumber("PaRaM", n0);
            var result = calculator(argument);
            assert(result);
        }

        private void SingleTextParameterEvaluation(string inputExpression, Action<bool> assert)
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = CalculateEvaluateParameterType.Text;
            var calculator = compiler.GetEvaluator(inputExpression);
            var argument = new CalculateEvaluateArgument();
            argument.SetText("PaRaM", s0);
            var result = calculator(argument);
            assert(result);
        }

        private void SingleTimestampParameterEvaluation(string inputExpression, Action<bool> assert)
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = CalculateEvaluateParameterType.Timestamp;
            var calculator = compiler.GetEvaluator(inputExpression);
            var argument = new CalculateEvaluateArgument();
            argument.SetTimestamp("PaRaM", d0);
            var result = calculator(argument);
            assert(result);
        }
    }
}
