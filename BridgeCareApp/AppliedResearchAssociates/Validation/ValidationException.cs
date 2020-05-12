using System;
using System.Runtime.Serialization;

namespace AppliedResearchAssociates.Validation
{
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationException()
        {
        }

        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context) => Context = (ValidationContext)info.GetValue(nameof(Context), typeof(ValidationContext));

        public ValidationContext Context { get; set; }

        public override string Message => (base.Message + Environment.NewLine + Context).Trim();
    }
}
