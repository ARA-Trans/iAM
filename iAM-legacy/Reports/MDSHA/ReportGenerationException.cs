using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports.MDSHA
{
    public class ReportGenerationException : Exception
    {
        public ReportGenerationException()
        {
        }

        public ReportGenerationException(string message)
            : base(message)
        {
        }

        public ReportGenerationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
