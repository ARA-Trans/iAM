//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AspWebApi
{
    using System;
    using System.Collections.Generic;
    
    public partial class PERFORMANCE
    {
        public int PERFORMANCEID { get; set; }
        public int SIMULATIONID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public string EQUATIONNAME { get; set; }
        public string CRITERIA { get; set; }
        public string EQUATION { get; set; }
        public Nullable<bool> SHIFT { get; set; }
        public byte[] BINARY_EQUATION { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }
        public Nullable<bool> PIECEWISE { get; set; }
        public Nullable<bool> ISFUNCTION { get; set; }
    
        public virtual ATTRIBUTES_ ATTRIBUTES_ { get; set; }
        public virtual SIMULATION SIMULATION { get; set; }
    }
}
