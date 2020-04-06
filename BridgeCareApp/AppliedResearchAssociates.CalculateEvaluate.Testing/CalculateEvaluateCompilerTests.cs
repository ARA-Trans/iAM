using System;
using NUnit.Framework;

namespace AppliedResearchAssociates.CalculateEvaluate.Testing
{
    public class CalculateEvaluateCompilerTests
    {
        private const string CATEGORY_CALCULATE = "Calculate";
        private const string CATEGORY_EVALUATE = "Evaluate";
        private const double n1 = 19.123, n2 = 23.456;
        private const string s1 = "foo", s2 = "bar";
        private static readonly DateTime d1 = new DateTime(2000, 1, 1), d2 = new DateTime(2020, 1, 1);

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
        public void Addition()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"{n1} + {n2}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(n1 + n2));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void CalculationGrouping()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"({n1})");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(n1));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void ConstantReference()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator("pi");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(Math.PI));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Division()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"{n1} / {n2}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(n1 / n2));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Invocation()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"log({n1})");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(Math.Log(n1)));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Multiplication()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"{n1} * {n2}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(n1 * n2));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Negation()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"-{n1}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(-n1));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void NumberLiteral()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"{n1}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(n1));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void NumberParameterReference()
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = ParameterType.Number;
            var calculator = compiler.GetCalculator("[param]");
            var argument = new CalculateEvaluateArgument();
            argument.Numbers["PaRaM"] = n1;
            var result = calculator(argument);
            Assert.That(result, Is.EqualTo(argument.Numbers["pArAm"]));
        }

        [Test]
        [Category(CATEGORY_CALCULATE)]
        public void Subtraction()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"{n1} - {n2}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(n1 - n2));
        }

        #endregion "Calculate"

        #region "Evaluate"

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberEqual()
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = ParameterType.Number;

            var calculator = compiler.GetEvaluator($"[param]=|{n2}|");

            var argument = new CalculateEvaluateArgument();
            argument.Numbers["PaRaM"] = n1;

            var result = calculator(argument);
            Assert.IsFalse(result);
        }

        [Test]
        [Category(CATEGORY_EVALUATE)]
        public void NumberNotEqual()
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = ParameterType.Number;

            var calculator = compiler.GetEvaluator($"[param]<>|{n2}|");

            var argument = new CalculateEvaluateArgument();
            argument.Numbers["PaRaM"] = n1;

            var result = calculator(argument);
            Assert.IsTrue(result);
        }

        #endregion "Evaluate"
    }
}
