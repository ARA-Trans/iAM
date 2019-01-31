﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class ConditionalData
    {
        public string Treatment { get; set; }
        public bool IsCommitted { get; set; }
        public int NumberTreatment { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
    }
}