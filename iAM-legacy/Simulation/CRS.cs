using System;
using System.Collections.Generic;
using System.Text;
using DatabaseManager;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
namespace Simulation
{
    public class CRS
    {
        Hashtable m_hashPavmentCoefficient;// Key is pavement type.  Value is list of doubles.



        public CRS()
        {
            if (DBMgr.IsTableInDatabase("CRS_COEFFICIENT"))
            {
                String strSelect = "SELECT PAVEMENT,A,B,C,D FROM CRS_COEFFICIENT";
                try
                {
                    m_hashPavmentCoefficient = new Hashtable();
                    DataSet ds = DBMgr.ExecuteQuery(strSelect);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        List<double> listCoefficient = new List<double>();

                        double dCoefficient = 0;
                        double.TryParse(dr[1].ToString(), out dCoefficient);
                        listCoefficient.Add(dCoefficient);

                        dCoefficient = 0;
                        double.TryParse(dr[2].ToString(), out dCoefficient);
                        listCoefficient.Add(dCoefficient);

                        dCoefficient = 0;
                        double.TryParse(dr[3].ToString(), out dCoefficient);
                        listCoefficient.Add(dCoefficient);

                        dCoefficient = 0;
                        double.TryParse(dr[4].ToString(), out dCoefficient);
                        listCoefficient.Add(dCoefficient);

                        m_hashPavmentCoefficient.Add(dr[0].ToString(), listCoefficient);
                    }
                }
                catch (Exception exception)
                {
                    SimulationMessaging.AddMessage(new SimulationMessage("CRS attribute load error.  Make sure CRS_COEFFICIENT table exists: " + exception.Message));
                }
            }
            else
            {
                m_hashPavmentCoefficient = new Hashtable();
            }
        }
        /// <summary>
        /// Thickness adjustment factor
        /// </summary>
        /// <param name="dThick">Thickness</param>
        /// <returns></returns>
        private double CalculateTAF(double dThick)
        {
	        double	dTAF = 0;

            if (dThick > 0)
            {
                dTAF = 0.048 * Math.Pow(dThick, 4);
                dTAF -= 1.234 * Math.Pow(dThick, 3);
                dTAF += 10.88 * Math.Pow(dThick, 2);
                dTAF -= 29.75 * dThick;
                dTAF += 26.28;
                dTAF /= 3;
            }
            else
                dTAF = 1;
	        return dTAF;
        }

        public double CalculateCRSBenefit(bool bCalculateBenefit, String strPavementType, double dCurrentCRS, double dThick, double dESAL, double dGrowth, double dLimit, double dAge, double dDeficientLevel, out double dBenefit,out double dRSL)
        {
	        double dCRS = 0;
	        double dC1 = 0;
	        double dC2 = 0;
            dRSL = 0;
            dBenefit = 0;


	        double dInitialESAL = dESAL;
	        double dCumESALS = 0;
	        double dCurrentESAL = 0;
	        double dDeltaYear = 0;
	        double dCurCRS = dCurrentCRS;

	        //dGrowth /= 100;

	        double dA = 0;
	        double dB = 0;
	        double dC = 0;
	        double dD = 0;

            #region Set Analysis Variables
             if (m_hashPavmentCoefficient.Contains(strPavementType))
            {
                List<double> listCoefficient = (List<double>)m_hashPavmentCoefficient[strPavementType];
                dA = listCoefficient[0];
                dB = listCoefficient[1];
                dC = listCoefficient[2];
                dD = listCoefficient[3];
            }
            else
            {
                return 0;
            }
            #endregion

            #region Set TAF
	        double dTAF = 1;

        		
            if(strPavementType == "HMAC" || strPavementType == "ELAC" || strPavementType == "AC")
            {
	            if(dThick < 4) dTAF = 2.0;
	            if(dThick >= 4 && dThick < 8) dTAF = 1.75;
	            else dTAF = 1.0;
            }
            else if(strPavementType == "AC-JRCP" || strPavementType == "AC-JPCP" ||strPavementType == "AC-JPCP-ND"
	            ||strPavementType == "AC-CRCP" || strPavementType == "AC-JPOL")
            {
	            if(dThick <= 2.5)
		            dTAF = 1.0;
	            else
		            dTAF = CalculateTAF(dThick);
            }
            #endregion


            if (!bCalculateBenefit)
            {

                dDeltaYear = dAge;
                int nAge = (int)dAge;

                for (int iYear = 0; iYear <= nAge; iYear++)
                {
                    dCurrentESAL = dInitialESAL * Math.Pow((1 + dGrowth), (double)iYear);
                    dCumESALS += dCurrentESAL;
                }
                //	dCumESALS = dInitialESAL*(1+dGrowth)*pow(1+dGrowth,dDeltaYear)/dGrowth;			

                dC1 = Math.Pow((9 - dCurCRS) / (2 * dA * Math.Pow(dThick * dTAF, dB) * Math.Pow(dCurrentESAL / 1000000, dD)), 1 / (dC + dD));
                dC2 = dC1 * dCurrentESAL / 1000000;
                dCRS = 9 - (2 * dA * Math.Pow(dThick * dTAF, dB) * Math.Pow(dC1 + dDeltaYear, dC) * Math.Pow(dC2 + dCumESALS / 1000000, dD));
                return dCRS;
            }
            
            
            bool bRSL = true;
            for (int n = 1; n <= 100; n++)
            {
                dDeltaYear = (double) n;
                dCurrentESAL = dInitialESAL * Math.Pow((1 + dGrowth), dDeltaYear);
                dCumESALS += dCurrentESAL;
                //	dCumESALS = dInitialESAL*(1+dGrowth)*pow(1+dGrowth,dDeltaYear)/dGrowth;			

                dC1 = Math.Pow((9 - dCurCRS) / (2 * dA * Math.Pow(dThick * dTAF, dB) * Math.Pow(dCurrentESAL / 1000000, dD)), 1 / (dC + dD));
                dC2 = dC1 * dCurrentESAL / 1000000;
                dCRS = 9 - (2 * dA * Math.Pow(dThick * dTAF, dB) * Math.Pow(dC1 + dDeltaYear, dC) * Math.Pow(dC2 + dCumESALS / 1000000, dD));

                dBenefit += dCRS;

                if (bRSL && dDeficientLevel > dCRS)
                {
                    dRSL = dDeltaYear;
                    bRSL = false;
                }
                if (dCRS < 1) break;// Break this for loop
            }
            return 0;
        }
    }
}
