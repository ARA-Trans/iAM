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

        public PerformanceLibraryEquationModel(PerformanceEntity performanceEntity)
        {
            Id = performanceEntity.PERFORMANCEID.ToString();
            Attribute = performanceEntity.ATTRIBUTE_;
            EquationName = performanceEntity.EQUATIONNAME;
            Criteria = performanceEntity.CRITERIA;
            Equation = performanceEntity.EQUATION;
            Shift = performanceEntity.SHIFT ?? false;
            Piecewise = performanceEntity.PIECEWISE ?? false;
            IsFunction = performanceEntity.ISFUNCTION ?? false;
        }

        public void UpdatePerformance(PerformanceEntity performanceEntity)
        {
            performanceEntity.ATTRIBUTE_ = Attribute;
            performanceEntity.EQUATIONNAME = EquationName;
            performanceEntity.CRITERIA = Criteria;
            performanceEntity.EQUATION = Equation;
            performanceEntity.SHIFT = Shift ?? false;
            performanceEntity.PIECEWISE = Piecewise ?? false;
            performanceEntity.ISFUNCTION = IsFunction ?? false;
        }
    }
}