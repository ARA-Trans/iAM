using System;
using NUnit.Framework;

namespace AppliedResearchAssociates.CalculateEvaluate.Testing
{
    public class CalculationTests
    {
        [Test]
        public void Invocation()
        {
            var compiler = new CalculateEvaluateCompiler();
            var calculator = compiler.GetCalculator("log(42)");
            var result = calculator(null);
            Assert.That(result, Is.EqualTo(Math.Log(42)));
        }
    }
}
