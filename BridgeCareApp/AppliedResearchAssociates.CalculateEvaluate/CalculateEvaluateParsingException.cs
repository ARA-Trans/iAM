using System;
using System.Runtime.Serialization;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculateEvaluateParsingException : CalculateEvaluateException
    {
        public CalculateEvaluateParsingException()
        {
        }

        public CalculateEvaluateParsingException(string message) : base(message)
        {
        }

        public CalculateEvaluateParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CalculateEvaluateParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
