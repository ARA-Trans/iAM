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
    
    public partial class TARGET_DEFICIENT
    {
        public int ID_ { get; set; }
        public int SIMULATIONID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public Nullable<int> YEARS { get; set; }
        public Nullable<double> TARGETMEAN { get; set; }
        public Nullable<double> DEFICIENT { get; set; }
        public Nullable<double> TARGETPERCENTDEFICIENT { get; set; }
        public string CRITERIA { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }
    
        public virtual ATTRIBUTES_ ATTRIBUTES_ { get; set; }
        public virtual SIMULATION SIMULATION { get; set; }
    }
}
