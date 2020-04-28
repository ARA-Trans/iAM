using System;
using System.Runtime.Serialization;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculateEvaluateLexingException : CalculateEvaluateException
    {
        public CalculateEvaluateLexingException()
        {
        }

        public CalculateEvaluateLexingException(string message) : base(message)
        {
        }

        public CalculateEvaluateLexingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CalculateEvaluateLexingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
