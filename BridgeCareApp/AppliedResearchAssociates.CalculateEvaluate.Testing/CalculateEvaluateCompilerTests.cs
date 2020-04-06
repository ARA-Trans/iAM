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
            compiler.ParameterTypes["param2"] = ParameterType.String;
            compiler.ParameterTypes["param3"] = ParameterType.Date;

            var expression = "[param1]=|1| and [param2]<>|foo| or [param3]<|2000-01-01|";
            var annotatedExpression = compiler.AnnotateParameterReferenceTypes(expression);

            Assert.That(annotatedExpression, Is.EqualTo("[param1]=|1| and [@param2]<>|foo| or [$param3]<|2000-01-01|"));
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
            argument.Numbers["PaRaM"] = n1;
            var result = calculator(argument);
            Assert.That(result, Is.EqualTo(argument.Numbers["pArAm"]));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Subtraction() => ParameterlessCalculation($"{n1} - {n2}", n1 - n2);

        #endregion "Calculate"

        #region "Evaluate"

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void DateEqual() => SingleDateParameterEvaluation($"[param]=|{d2}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void DateGreaterThan() => SingleDateParameterEvaluation($"[param]>|{d2}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void DateGreaterThanOrEqual() => SingleDateParameterEvaluation($"[param]>=|{d2}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void DateLessThan() => SingleDateParameterEvaluation($"[param]<|{d2}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void DateLessThanOrEqual() => SingleDateParameterEvaluation($"[param]<=|{d2}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void DateNotEqual() => SingleDateParameterEvaluation($"[param]<>|{d2}|", Assert.IsTrue);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberEqual() => SingleNumberParameterEvaluation($"[param]=|{n2}|", Assert.IsFalse);

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
        public void StringEqual() => SingleStringParameterEvaluation($"[param]=|{s2}|", Assert.IsFalse);

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void StringNotEqual() => SingleStringParameterEvaluation($"[param]<>|{s2}|", Assert.IsTrue);

        #endregion "Evaluate"

        private const string CATEGORY_CALCULATE = "Calculate";

        private const string CATEGORY_EVALUATE = "Evaluate";

        private const double n1 = 19.123, n2 = 23.456;

        private const string s1 = "foo", s2 = "bar";

        private static readonly DateTime d1 = new DateTime(2000, 1, 1), d2 = new DateTime(2020, 1, 1);

        private void ParameterlessCalculation(string inputExpression, object expectedOutput)
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator(inputExpression);
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        private void SingleDateParameterEvaluation(string inputExpression, Action<bool> assert)
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = ParameterType.Date;
            var calculator = compiler.GetEvaluator(inputExpression);
            var argument = new CalculateEvaluateArgument();
            argument.Dates["PaRaM"] = d1;
            var result = calculator(argument);
            assert(result);
        }

        private void SingleNumberParameterEvaluation(string inputExpression, Action<bool> assert)
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = ParameterType.Number;
            var calculator = compiler.GetEvaluator(inputExpression);
            var argument = new CalculateEvaluateArgument();
            argument.Numbers["PaRaM"] = n1;
            var result = calculator(argument);
            assert(result);
        }

        private void SingleStringParameterEvaluation(string inputExpression, Action<bool> assert)
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = ParameterType.String;
            var calculator = compiler.GetEvaluator(inputExpression);
            var argument = new CalculateEvaluateArgument();
            argument.Strings["PaRaM"] = s1;
            var result = calculator(argument);
            assert(result);
        }
    }
}
