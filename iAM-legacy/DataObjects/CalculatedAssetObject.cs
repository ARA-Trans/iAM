using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalculateEvaluate;
using System.Data;

namespace DataObjects
{
//    ASSET	varchar(50)	Unchecked
//CALCULATED_PROPERTY	varchar(50)	Unchecked
//EQUATION	varchar(MAX)	Unchecked
//BINARY_EQUATION	varbinary(MAX)	Checked
//CRITERIA	varchar(MAX)	Checked
//BINARY_CRITERIA	varbinary(MAX)	Checked
//RETURN_TYPE	varchar(50)	Checked
    
    public class CalculatedAssetObject
    {
        int m_nID;
        String m_strAsset;
        String m_strCalculatedProperty;
        String m_strEquation;
        String m_strCriteria;
        String m_strReturnType;
        CalculateEvaluate.CalculateEvaluate m_Calculate;
        CalculateEvaluate.CalculateEvaluate m_Evaluate;


        public int PrimaryKey
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public CalculateEvaluate.CalculateEvaluate Calculate
        {
            get { return m_Calculate; }
            set { m_Calculate = value; }
        }

        public CalculateEvaluate.CalculateEvaluate Evaluate
        {
            get { return m_Evaluate; }
            set { m_Evaluate = value; }
            
        }
        


        /// <summary>
        /// RoadCare Asset (native or non-native)
        /// </summary>
        public String Asset
        {
            get { return m_strAsset; }
            set { m_strAsset = value; }
        }
        /// <summary>
        /// Equation for calculating RoadCare calculated Asset
        /// </summary>
        public String Equation
        {
            get { return m_strEquation; }
            set { m_strEquation = value; }
        }
        /// <summary>
        /// Criteria when equation applies.  Blank is all cases
        /// </summary>
        public String Criteria
        {
            get { return m_strCriteria; }
            set { m_strCriteria = value; }
        }

        /// <summary>
        /// Type of return from equation (STRING or NUMBER(double))
        /// </summary>
        public String Type
        {
            get { return m_strReturnType; }
            set { m_strReturnType = value; }
        }
        /// <summary>
        /// Calculated Property
        /// </summary>
        public String CalculatedProperty
        {
            get { return m_strCalculatedProperty; }
            set { m_strCalculatedProperty = value; }
        }
        public CalculatedAssetObject()
        {

        }

        public CalculatedAssetObject(DataRow row)
        {
            this.PrimaryKey = (int) row["ID_"];
            this.Asset = row["ASSET"].ToString();
            this.CalculatedProperty = row["CALCULATED_PROPERTY"].ToString();
            this.Equation = row["EQUATION"].ToString();
            this.Criteria = row["CRITERIA"].ToString();
            this.Type = row["RETURN_TYPE"].ToString();
        }




    }
}
