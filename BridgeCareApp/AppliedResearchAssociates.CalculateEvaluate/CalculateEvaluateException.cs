using System;
using System.Runtime.Serialization;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculateEvaluateException : Exception
    {
        public CalculateEvaluateException()
        {
        }

        public CalculateEvaluateException(string message) : base(message)
        {
        }

        public CalculateEvaluateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CalculateEvaluateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
