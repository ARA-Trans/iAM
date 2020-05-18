using System;
using System.Runtime.Serialization;

namespace AppliedResearchAssociates.iAM
{
    public class SimulationException : Exception
    {
        public SimulationException()
        {
        }

        public SimulationException(string message) : base(message)
        {
        }

        public SimulationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SimulationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
