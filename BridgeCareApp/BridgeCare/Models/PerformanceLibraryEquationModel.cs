using System;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class PerformanceLibraryEquationModel : CrudModel
    {
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string EquationName { get; set; }
        public string Criteria { get; set; }
        public string Equation { get; set; }
        public bool? Shift { get; set; }  
        public bool? Piecewise { get; set; }
        public bool? IsFunction { get; set; }

        public PerformanceLibraryEquationModel() { }

        public PerformanceLibraryEquationModel(PerformanceEntity entity)
        {
            Id = entity.PERFORMANCEID.ToString();
            Attribute = entity.ATTRIBUTE_;
            EquationName = entity.EQUATIONNAME;
            Criteria = entity.CRITERIA;
            Equation = entity.EQUATION;
            Shift = entity.SHIFT ?? false;
            Piecewise = entity.PIECEWISE ?? false;
            IsFunction = entity.ISFUNCTION ?? false;
        }

        public void UpdatePerformance(PerformanceEntity entity)
        {
            entity.ATTRIBUTE_ = Attribute;
            entity.EQUATIONNAME = EquationName;
            entity.CRITERIA = Criteria;
            entity.EQUATION = Equation;
            entity.SHIFT = Shift ?? false;
            entity.PIECEWISE = Piecewise ?? false;
            entity.ISFUNCTION = IsFunction ?? false;
        }
    }
}