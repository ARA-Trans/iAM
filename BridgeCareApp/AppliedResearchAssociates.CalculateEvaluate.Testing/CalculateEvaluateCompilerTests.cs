using System;
using NUnit.Framework;

namespace AppliedResearchAssociates.CalculateEvaluate.Testing
{
    public class CalculateEvaluateCompilerTests
    {
        [Test]
        public void AnnotateParameterReferenceTypes()
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["param1"] = ParameterType.Number;
            compiler.ParameterTypes["param2"] = ParameterType.Text;
            compiler.ParameterTypes["param3"] = ParameterType.Timestamp;

            var expression = "[param1]=|1| and ([param2]<>|| or [param3]<'2000-01-01')";
            var annotatedExpression = compiler.AnnotateParameterReferenceTypes(expression);

            Assert.That(annotatedExpression, Is.EqualTo("[param1]=|1| and ([@param2]<>|| or [$param3]<'2000-01-01')"));
        }

        #region "Calculate"

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Addition() => ParameterlessCalculation($"{n1} + {n2}", n1 + n2);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void CalculationGrouping() => ParameterlessCalculation($"{n1} * ({n1} + {n2})", n1 * (n1 + n2));

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void ConstantReference() => ParameterlessCalculation("pi", Math.PI);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Division() => ParameterlessCalculation($"{n1} / {n2}", n1 / n2);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Invocation() => ParameterlessCalculation($"log({n1})", Math.Log(n1));

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Multiplication() => ParameterlessCalculation($"{n1} * {n2}", n1 * n2);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Negation() => ParameterlessCalculation($"-{n1}", -n1);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void NumberLiteral() => ParameterlessCalculation($"{n1}", n1);

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void NumberParameterReference()
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = ParameterType.Number;
            var expression = "[param]";
            var calculator = compiler.GetCalculator(expression);
            var argument = new CalculateEvaluateArgument();
            argument.Number["PaRaM"] = n1;
            var result = calculator(argument);
            Assert.That(result, Is.EqualTo(argument.Number["pArAm"]));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Subtraction() => ParameterlessCalculation($"{n1} - {n2}", n1 - n2);

        #endregion "Calculate"

        #region "Evaluate"

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberEqual() => SingleNumberParameterEvaluation($"[param]='{n2}'", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberGreaterThan() => SingleNumberParameterEvaluation($"[param]>|{n2}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberGreaterThanOrEqual() => SingleNumberParameterEvaluation($"[param]>=|{n2}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberLessThan() => SingleNumberParameterEvaluation($"[param]<|{n2}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberLessThanOrEqual() => SingleNumberParameterEvaluation($"[param]<=|{n2}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberNotEqual() => SingleNumberParameterEvaluation($"[param]<>|{n2}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TextEqual() => SingleTextParameterEvaluation($"[param]='{s2}'", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TextNotEqual() => SingleTextParameterEvaluation($"[param]<>|{s2}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampEqual() => SingleTimestampParameterEvaluation($"[param]='{d2}'", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampGreaterThan() => SingleTimestampParameterEvaluation($"[param]>|{d2}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampGreaterThanOrEqual() => SingleTimestampParameterEvaluation($"[param]>=|{d2}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampLessThan() => SingleTimestampParameterEvaluation($"[param]<|{d2}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampLessThanOrEqual() => SingleTimestampParameterEvaluation($"[param]<=|{d2}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void TimestampNotEqual() => SingleTimestampParameterEvaluation($"[param]<>|{d2}|", Assert.IsTrue);

        #endregion "Evaluate"

        private const string CATEGORY_CALCULATE = "Calculate";

        private const string CATEGORY_EVALUATE = "Evaluate";

        private const double n1 = 19.123, n2 = 23.456;

        private const string s1 = "foo", s2 = "bar";

        private static readonly DateTime d1 = new DateTime(2000, 1, 1), d2 = new DateTime(2020, 1, 1);

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
            compiler.ParameterTypes["PARAM"] = ParameterType.Number;
            var calculator = compiler.GetEvaluator(inputExpression);
            var argument = new CalculateEvaluateArgument();
            argument.Number["PaRaM"] = n1;
            var result = calculator(argument);
            assert(result);
        }

        private void SingleTextParameterEvaluation(string inputExpression, Action<bool> assert)
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = ParameterType.Text;
            var calculator = compiler.GetEvaluator(inputExpression);
            var argument = new CalculateEvaluateArgument();
            argument.Text["PaRaM"] = s1;
            var result = calculator(argument);
            assert(result);
        }

        private void SingleTimestampParameterEvaluation(string inputExpression, Action<bool> assert)
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = ParameterType.Timestamp;
            var calculator = compiler.GetEvaluator(inputExpression);
            var argument = new CalculateEvaluateArgument();
            argument.Timestamp["PaRaM"] = d1;
            var result = calculator(argument);
            assert(result);
        }
    }
}
