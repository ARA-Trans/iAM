using System;
using System.Runtime.Serialization;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculateEvaluateCompilationException : CalculateEvaluateException
    {
        public CalculateEvaluateCompilationException()
        {
        }

        public CalculateEvaluateCompilationException(string message) : base(message)
        {
        }

        public CalculateEvaluateCompilationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CalculateEvaluateCompilationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
