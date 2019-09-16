using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Logging
{
    public static class ElmahLog
    {
        public static void Write(Exception e, string customMessage)
        {
            Exception ex = new Exception(customMessage, e);
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }

    }
}
