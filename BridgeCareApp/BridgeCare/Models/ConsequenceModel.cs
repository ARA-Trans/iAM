using System;
using System.Runtime.Serialization;


namespace BridgeCare.Models
{
    [DataContract]
    public class ConsequenceModel
    {
        [DataMember(Name = "id")]
        public int ConsequenceId { get; set; }

        [DataMember(Name = "treatmentId")]
        public int TreatmentId { get; set; }

        [DataMember(Name = "attribute")]
        public string Attribute_ { get; set; }

        [DataMember(Name = "change")]
        public string Change { get; set; }

        [DataMember(Name = "criteria")]
        public string Criteria { get; set; }

        [DataMember(Name = "equation")]
        public string Equation { get; set; }

        [DataMember(Name = "isFunction")]
        public Nullable<bool> IsFunction { get; set; }
    }
}