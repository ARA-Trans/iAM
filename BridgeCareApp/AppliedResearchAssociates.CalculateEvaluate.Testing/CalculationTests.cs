using System;
using NUnit.Framework;

namespace AppliedResearchAssociates.CalculateEvaluate.Testing
{
    public class CalculationTests
    {
        private const double A = 23, B = 19;

        [Test]
        public void Addition()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"{A} + {B}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(A + B));
        }

        [Test]
        public void ConstantReference()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator("pi");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(Math.PI));
        }

        [Test]
        public void Division()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"{A} / {B}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(A / B));
        }

        [Test]
        public void Grouping()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"({A})");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(A));
        }

        [Test]
        public void Invocation()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"log({A})");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(Math.Log(A)));
        }

        [Test]
        public void Multiplication()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"{A} * {B}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(A * B));
        }

        [Test]
        public void Negation()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"-{A}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(-A));
        }

        [Test]
        public void NumberLiteral()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"{A}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(A));
        }

        [Test]
        public void NumberParameterReference()
        {
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["PARAM"] = ParameterType.Number;
            var calculator = compiler.GetCalculator("[param]");
            var argument = new CalculateEvaluateArgument();
            argument.Numbers["PaRaM"] = A;
            var result = calculator(argument);
            Assert.That(result, Is.EqualTo(argument.Numbers["pArAm"]));
        }

        [Test]
        public void Subtraction()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator($"{A} - {B}");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(A - B));
        }
    }
}
