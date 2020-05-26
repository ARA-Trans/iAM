using System;
using static System.Math;
using static AppliedResearchAssociates.PciDistress.Constants;

namespace AppliedResearchAssociates.PciDistress
{
    public static class PciDistress
    {
        #region Transliteration of public members

        /// <summary>
        ///     A reduced version of the original ComputeVCIValues from DSS which computes only the VCI.
        /// </summary>
        public static double ComputePCIValue(string sDeductValues, string sMethodology)
        {
            double dVCI = 0.0;
            // for PCI call function to get the residue in 100
            double dLargeDeductLimit = 5.0;
            if (sMethodology == "ac.mpr" || sMethodology == "pcc.mpr") dLargeDeductLimit = 2.0;
            if (IsWASHCLKMethod(sMethodology))
            {
                dVCI = 100.0 - TotalDeducts(sDeductValues);
            }
            else
            {
                dVCI = 100.0 - pciCorrectedDeductValue(sMethodology, sDeductValues, dLargeDeductLimit);
            }

            // Correct for overflow or underflow of the value
            if (dVCI < 0.0)
                dVCI = 0.0;
            else if (dVCI > 100.0)
                dVCI = 100.0;

            return dVCI;
        }

        public static bool IsWASHCLKMethod(string s)
        {
            if (s == "pcc.wash" || s == "ac.wash" || s == "ac.clk" || s == "bit.clk")
                return true;
            else
                return false;
        }

        /// <summary>
        ///     Returns the deduct value for a non PCI distress
        /// </summary>
        public static double pvt_ComputeNonPCIDeduct(string sMethod, int nDistress, string sSeverity, double dExtent)
        {
            string sExtent = "";
            if (dExtent < 0.40) sExtent = "L";
            else if (dExtent >= 0.40 && dExtent > 0.8) sExtent = "M";
            else sExtent = "H";
            return pvt_ComputeNonPCIDeduct(sMethod, nDistress, sSeverity, sExtent);
        }

        public static double pvt_ComputePCIDeduct(int nDistress, string sSeverity, double dAmount, double dSamsiz)
        {
            double dPCIDeduct = 0.0;
            double dPercentDensity = 0.0;
            int iSeverity = pvt_nSevFromSev(sSeverity);

            if (dSamsiz < .0001)
                dPCIDeduct = 0.0;
            if (dAmount < .0001)
                dPCIDeduct = 0.0;
            if (dSamsiz < dAmount)
                dPercentDensity = 100.0;
            else
                dPercentDensity = 100.0 * (dAmount / dSamsiz);
            // Calculate the deduct only if there is something to calculate. Don't calculate zeros!
            //
            // Gregg and Alice investaged and found out, even the density is 0, micropaver is still
            // using 0.1 After consulted with Mike Darter, we decide to match MicroPaver if
            // (dPercentDensity > 0.0)
            dPCIDeduct = DeductEval(nDistress, iSeverity, (float)dPercentDensity);

            return dPCIDeduct;
        }

        #endregion Transliteration of public members

        #region Transliteration of private members

        /// <summary>
        ///     Returns the deduct value for a non PCI distress
        /// </summary>
        private static double pvt_ComputeNonPCIDeduct(string sMethod, int nDistress, string sSeverity, string sExtent)
        {
            double dVal = 0.0;
            //if (IsWNDMethod(sMethod))
            //{
            //	dVal = pvt_ComputeWNDDeduct(nDistress, sSeverity, sExtent);
            //	return dVal;
            //}
            if (IsWASHCLKMethod(sMethod))
            {
                dVal = pvt_ComputeWASHCLKDeduct(nDistress, sSeverity, sExtent);
                return dVal;
            }
            // default to the WNDMethod
            //dVal = pvt_ComputeWNDDeduct(nDistress, sSeverity, sExtent);
            return dVal;
        }

        /// <summary>
        ///     Returns the deduct value for a single distress. Ok to pass in '-' 'nun' or whatever
        ///     for sev or extent level, for distresses without defined levels.
        /// </summary>
        private static double pvt_ComputeWASHCLKDeduct(int nDistress, string sSeverity, string sExtent)
        {
            double dDistress = 0.0;
            switch (nDistress)
            {
                // Have the spreadsheet that links number to distress in your hand before touching
                // this code. When '-' 'nun' or whatever is passed in, parameters must be replicated
                // in the call to the sub. severity varies fastest, deducts for the same extent are contiguous
                case 270:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 17, 27, 35, 35, 50, 61, 57, 75, 87);
                    break;

                case 271:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 90, 100, 100, 90, 100, 100, 90, 100, 100);
                    break;

                case 272:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 12, 20, 27, 32, 46, 57, 47, 64, 76);
                    break;

                case 273:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 8, 14, 20, 18, 28, 37, 26, 39, 49);
                    break;

                case 274:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 14, 23, 35, 29, 42, 61, 47, 63, 87);
                    break;

                case 275:
                    dDistress = 0;
                    break;

                case 276:
                    dDistress = 0;
                    break;

                case 277:
                    dDistress = 0;
                    break;

                case 278:
                    dDistress = 0;
                    break;

                case 279:
                    dDistress = 0;
                    break;

                case 280:
                    dDistress = 0;
                    break;

                case 290:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 7, 22, 37, 17, 39, 60, 29, 55, 80);
                    break;

                case 291:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 3, 5, 13, 5, 9, 23, 7, 13, 32);
                    break;

                case 292:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 5, 18, 37, 15, 36, 60, 30, 56, 80);
                    break;

                case 293:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 6, 6, 6, 12, 12, 12, 18, 18, 18);
                    break;

                case 294:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 4, 7, 16, 8, 14, 28, 13, 22, 40);
                    break;

                case 295:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 2, 7, 13, 4, 12, 23, 6, 18, 34);
                    break;

                case 296:
                    dDistress = 0;
                    break;

                case 297:
                    dDistress = 0;
                    break;

                case 298:
                    dDistress = 0;
                    break;

                case 310:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 15, 20, 30, 20, 30, 45, 30, 40, 50);
                    break;

                case 311:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 35, 50, 55, 35, 50, 55, 35, 50, 55);
                    break;

                case 312:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 3, 6, 9, 3, 6, 9, 3, 6, 9);
                    break;

                case 313:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 5, 15, 20, 10, 15, 25, 15, 20, 30);
                    break;

                case 314:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 2, 4, 6, 4, 6, 8, 6, 8, 12);
                    break;

                case 315:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 5, 10, 15, 10, 15, 20, 15, 20, 25);
                    break;

                case 316:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 20, 15, 0, 25, 20, 5, 30, 30, 10);
                    break;

                case 317:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 5, 15, 20, 10, 15, 25, 15, 20, 30);
                    break;

                case 318:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 5, 15, 25, 5, 15, 25, 5, 15, 25);
                    break;

                case 319:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 5, 5, 5, 10, 10, 10, 15, 15, 15);
                    break;

                case 320:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 2, 2, 2, 5, 5, 5, 10, 10, 10);
                    break;

                case 321:
                    dDistress = 0;
                    break;

                case 330:
                    dDistress = 0;
                    break;

                case 331:
                    dDistress = 0;
                    break;

                case 332:
                    dDistress = 0;
                    break;

                case 333:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 35, 50, 65, 35, 50, 65, 35, 50, 65);
                    break;

                case 334:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 20, 35, 50, 25, 40, 55, 30, 45, 60);
                    break;

                case 335:
                    dDistress = 0;
                    break;

                case 336:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 5, 15, 30, 15, 30, 45, 30, 45, 60);
                    break;

                case 337:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 5, 10, 15, 10, 15, 20, 15, 20, 25);
                    break;

                case 338:
                    dDistress = pvt_SeverityExtent2Deduct(sSeverity, sExtent, 20, 25, 30, 25, 30, 35, 30, 40, 50);
                    break;

                case 339:
                    dDistress = 0;
                    break;

                default:
                    dDistress = 0;
                    break;
            }
            return dDistress;
        }

        /// <summary>
        ///     PRIVATE for WASH CLK deduct calculation. pvt_SeverityExtent2Deduct('M', 'M', 2, 3,
        ///     4, 32, 33, 34, 88, 89, 90) returns 3. A 4th value severity or extent is silently
        ///     mapped to an un-specified one of the other levels. For those distresses call the
        ///     function with replicated parameters
        /// </summary>
        private static int pvt_SeverityExtent2Deduct(string sRawSeverity, string sRawExtent, int n1, int n2, int n3, int n4, int n5, int n6, int n7, int n8, int n9)
        {
            string sSeverity;
            string sExtent;
            sSeverity = sRawSeverity;
            sExtent = sRawExtent;

            if (sSeverity != "L" && sSeverity != "M" && sSeverity != "H") sSeverity = "H";
            if (sExtent != "L" && sExtent != "M" && sExtent != "H") sExtent = "H";

            if (sSeverity == "L" && sExtent == "L") return n1;
            if (sSeverity == "M" && sExtent == "L") return n2;
            if (sSeverity == "H" && sExtent == "L") return n3;

            if (sSeverity == "L" && sExtent == "M") return n4;
            if (sSeverity == "M" && sExtent == "M") return n5;
            if (sSeverity == "H" && sExtent == "M") return n6;

            if (sSeverity == "L" && sExtent == "H") return n7;
            if (sSeverity == "M" && sExtent == "H") return n8;
            if (sSeverity == "H" && sExtent == "H") return n9;

            return n9;
        }

        private static int pvt_nSevFromSev(string sSev)
        {
            int iSev = 0;
            sSev = sSev.ToUpper();
            if (sSev == "L")
                iSev = 2;
            else if (sSev == "M")
                iSev = 1;
            else if (sSev == "H")
                iSev = 0;

            return iSev;
        }

        private static float DeductEval(int distress, int degree, float x)
        {
            /*
             * DeductEval(int a, int b, float x) -- this is nothing more than an
             *   doubly indexed library of functions f(a,b): [0,100]->R+.
             *
             *   (this comment describes what should be, not what is)
             *   Helper function dis2row(a,b) returns an index that DeductEval uses
             *   to locate a row of data points.
             *   The return value, f(a,b)(x), is computed by
             *   interpolating the data.
             *   Error conditions are handled as follows:
             *     1.  if (a,b) does not map to a function,       return 0
             *     2.  if (a,b) does map to a function, but x<0   return f(a,b)(0)
             *     3.  if (a,b) does map to a function, but x>100 return f(a,b)(100)
             *     4.  any other case is not an error f(a,b)(x) is returned
             *
             *   The implementation may assume that a>=0, b=0,1, or 2
             *   The library needs to contain approximately 200 micropaver distress
             *   deduct curves, 100 more FAA curves, and have capacity to hold another
             *   200 curves.  These curves currently exist as printed graphs, the routine
             *   needs to be accurate to within a value of 2, and the error should
             *   be negative or positive with equal likelyhood.  Some of the curves
             *   are plotted as x vs y and some are plotted as log(x) vs y.
             */

            var d = DeductEval_d;

            int n;

            if ((x > 999) || (x < 0))
            {
                return 0;
            }
            else if ((distress < 0) || (distress >= NUMDISTRESS))
            {  /*?*/
                return 0;
            }
            else
            {
                int i;
                n = dis2row(distress, degree);
                for (i = 1; d[n, i].x < x; i++)
                    ;
                {
                    float x1 = d[n, i - 1].x;
                    float y1 = d[n, i - 1].y;
                    float x2 = d[n, i].x;
                    float y2 = d[n, i].y;
                    float val = ((x - x1) * (y2 - y1)) / (x2 - x1) + y1;
                    return val;
                }
            }
        }

        /// <summary>
        ///     dis2row -- given the SAL distress number from [0-SALDISTRESSRANGE] and the severity
        ///     number (0=high, 1=medium, 2=low) dis2row returns the index of the row of
        ///     interpolation data in the interpolation table to use for this distress severity
        ///     combination. SEE also the related function q2row.
        /// </summary>
        private static int dis2row(int distress, int degree) => p[degree + 3 * distress];

        /// <summary>
        ///     Clark County uses the average of the deduct values in a section in order to compute
        ///     the PCI. This function provides the averaged totals for the PCI deductes.
        /// </summary>
        private static int TotalDeducts(string sDeductValues)
        {
            double dValue = 0.0;
            double dTotalDeducts = 0.0;
            var split = sDeductValues.Split(',');
            for (int i = 0; i < split.Length; i++)
            {
                dValue = double.Parse(split[i].ToString());
                dTotalDeducts += dValue;
            }

            dTotalDeducts = dTotalDeducts / (double)split.Length;
            return (int)Floor(dTotalDeducts);
        }

        /// <summary>
        ///     Implements ASTM method of computing the CDV from individual deducts
        /// </summary>
        private static unsafe double pciCorrectedDeductValue(string sMethod, string sDeduct, double dLargeDeductLimit)
        {
            var split = sDeduct.Split(',');

            var adDeductValue = new double[100];
            for (int i = 0; i < 100; i++) adDeductValue[i] = 0;

            System.Diagnostics.Debug.Assert(split.Length < 100);
            for (int i = 0; i < split.Length; i++)
            {
                adDeductValue[i] = double.Parse(split[i].ToString());
            }

            fixed (double* pDeductValues = adDeductValue)
            {
                int nLength = split.Length;
                int nNumLargeDeducts = 0;
                double nLargestDeduct = 0.0;
                double nMaxNumOfAllowDis = 0.0;
                int nMaxNumOfAllowDisIntegerPart = 0;
                double nMaxNumOfAllowDisFractionalPart = 0.0;
                double nTrialCDV = 0.0;
                double nMaxCDV = 0.0;
                double nSumOfDeducts = 0.0;

                double* pDV = pDeductValues;

                // ====================== Algorithm specified in an ASTM circular (we use it for roads too).
                //nLength = GetLengthNonNegativeArray(pDeductValues);

                Array.Sort(adDeductValue);
                Array.Reverse(adDeductValue);
                // ---------------------- compute the largest deduct
                nLargestDeduct = 0.0;

                while (*pDV > 0.0)
                {
                    if (*pDV > nLargestDeduct)
                        nLargestDeduct = *pDV;
                    pDV++;
                }
                // ---------------------- return 0 right now if there are no appreciable deduct values
                if (nLargestDeduct < 0.01)
                    return 0.0;
                // ---------------------- filter the distress list by the maximum number of
                //                        allowable distresses
                // ---------------------- compute the maximum number of allowable distresses
                nMaxNumOfAllowDis = Min(1.0 + (9.0 / 95.0) * (100.0 - nLargestDeduct), 10.0);
                if (sMethod == "ac.mpr" || sMethod == "pcc.mpr")
                    nMaxNumOfAllowDis = Min(1.0 + (9.0 / 98.0) * (100.0 - nLargestDeduct), 10.0);
                if (sMethod == "ac.faa" || sMethod == "pcc.faa" ||
                    sMethod == "ac.astm" || sMethod == "pcc.astm")
                    nMaxNumOfAllowDis = Min(1.0 + (9.0 / 95.0) * (100.0 - nLargestDeduct), 10.0);
                // ---------------------- scale the last allowable distress
                nMaxNumOfAllowDisIntegerPart = (int)nMaxNumOfAllowDis;
                nMaxNumOfAllowDisFractionalPart = nMaxNumOfAllowDis - (double)nMaxNumOfAllowDisIntegerPart;

                pDV = pDeductValues;
                if (nMaxNumOfAllowDisIntegerPart < nLength)
                    *(pDV + nMaxNumOfAllowDisIntegerPart) =
                                                      *(pDV + nMaxNumOfAllowDisIntegerPart) * nMaxNumOfAllowDisFractionalPart;
                // ---------------------- truncate the list after the last allowable distress
                *(pDV + (nMaxNumOfAllowDisIntegerPart + 1)) = -1;
                nLength = Min(nLength, nMaxNumOfAllowDisIntegerPart);
                // ---------------------- count the number of large deducts (The new sentinal may
                //                        have decreased this count.)
                nNumLargeDeducts = 99;
                nMaxCDV = 0.0;

                while (nNumLargeDeducts > 0)
                {
                    nNumLargeDeducts = 0;
                    pDV = pDeductValues;
                    while (*pDV > 0.0)
                    {
                        if (*pDV > dLargeDeductLimit)// 5.0)
                            nNumLargeDeducts = nNumLargeDeducts + 1;
                        pDV++;
                    }
                    //
                    // ---------------------- compute and return the final corrected deduct value (CDV)
                    //----------------------- as the max of a set of old method CDV's
                    //

                    // -------------------- compute the sum of the deducts
                    nSumOfDeducts = 0.0;
                    pDV = pDeductValues;
                    while (*pDV > 0.0)
                    {
                        nSumOfDeducts = nSumOfDeducts + *pDV;
                        pDV++;
                    }
                    // -------------------- apply the old "q" large deduct correction, remembering
                    //                      the largest one as the CDV, as all but the largest of
                    // the large deducts are successively and in increasing order reduced to 5.0

                    nTrialCDV = pciPrivateLargeDeductCorrection(sMethod, nNumLargeDeducts, nSumOfDeducts);
                    if (nTrialCDV > nMaxCDV)
                        nMaxCDV = nTrialCDV;

                    pDV = pDeductValues;
                    if (nNumLargeDeducts > 0)
                    {
                        nNumLargeDeducts = nNumLargeDeducts - 1;
                        *(pDV + nNumLargeDeducts) = dLargeDeductLimit;//5.0;
                    }
                }

                return nMaxCDV;
            }
        }

        /// <summary>
        ///     Applies the original (before ASTM) large deduct correction
        ///     pciLargeDeductCorrection(sMethod, nNumLargeDeducts, nTotalDeduct)
        /// </summary>
        private static double pciPrivateLargeDeductCorrection(string sMethod, int nNumLargeDeducts, double dTotalDeduct)
        {
            float nTotalDeduct = (float)dTotalDeduct;
            float fCorrDeduct = 0.0f;

            if (sMethod == "ac.mpr")
            {
                if (nNumLargeDeducts == 0)
                    fCorrDeduct = nTotalDeduct;
                else if (nNumLargeDeducts == 1)
                    fCorrDeduct = Q(MPACRDQ1, nTotalDeduct);
                else if (nNumLargeDeducts == 2)
                    fCorrDeduct = Q(MPACRDQ2, nTotalDeduct);
                else if (nNumLargeDeducts == 3)
                    fCorrDeduct = Q(MPACRDQ3, nTotalDeduct);
                else if (nNumLargeDeducts == 4)
                    fCorrDeduct = Q(MPACRDQ4, nTotalDeduct);
                else if (nNumLargeDeducts == 5)
                    fCorrDeduct = Q(MPACRDQ5, nTotalDeduct);
                else if (nNumLargeDeducts == 6)
                    fCorrDeduct = Q(MPACRDQ6, nTotalDeduct);
                else if (nNumLargeDeducts == 7)
                    fCorrDeduct = Q(MPACRDQ7, nTotalDeduct);
                else
                    fCorrDeduct = Q(MPACRDQ7, nTotalDeduct);
            }
            else if (sMethod == "pcc.mpr")
            {
                if (nNumLargeDeducts == 0)
                    fCorrDeduct = nTotalDeduct;
                else if (nNumLargeDeducts == 1)
                    fCorrDeduct = Q(MPPCCRDQ1, nTotalDeduct);
                else if (nNumLargeDeducts == 2)
                    fCorrDeduct = Q(MPPCCRDQ2, nTotalDeduct);
                else if (nNumLargeDeducts == 3)
                    fCorrDeduct = Q(MPPCCRDQ3, nTotalDeduct);
                else if (nNumLargeDeducts == 4)
                    fCorrDeduct = Q(MPPCCRDQ4, nTotalDeduct);
                else if (nNumLargeDeducts == 5)
                    fCorrDeduct = Q(MPPCCRDQ5, nTotalDeduct);
                else if (nNumLargeDeducts == 6)
                    fCorrDeduct = Q(MPPCCRDQ6, nTotalDeduct);
                else if (nNumLargeDeducts == 7)
                    fCorrDeduct = Q(MPPCCRDQ7, nTotalDeduct);
                else if (nNumLargeDeducts == 8)
                    fCorrDeduct = Q(MPPCCRDQ8, nTotalDeduct);
                else if (nNumLargeDeducts == 9)
                    fCorrDeduct = Q(MPPCCRDQ9, nTotalDeduct);
                else
                    fCorrDeduct = Q(MPPCCRDQ8, nTotalDeduct);
            }
            else if (sMethod == "pcc.faa")
            {
                if (nNumLargeDeducts == 0)
                    fCorrDeduct = nTotalDeduct;
                else if (nNumLargeDeducts == 1)
                    fCorrDeduct = Q(FAA82PCCQ1, nTotalDeduct);
                else if (nNumLargeDeducts == 2)
                    fCorrDeduct = Q(FAA82PCCQ2, nTotalDeduct);
                else if (nNumLargeDeducts == 3)
                    fCorrDeduct = Q(FAA82PCCQ3, nTotalDeduct);
                else if (nNumLargeDeducts == 4)
                    fCorrDeduct = Q(FAA82PCCQ4, nTotalDeduct);
                else if (nNumLargeDeducts == 5)
                    fCorrDeduct = Q(FAA82PCCQ5, nTotalDeduct);
                else if (nNumLargeDeducts == 6)
                    fCorrDeduct = Q(FAA82PCCQ6, nTotalDeduct);
                else if (nNumLargeDeducts == 7)
                    fCorrDeduct = Q(FAA82PCCQ7, nTotalDeduct);
                else if (nNumLargeDeducts == 8)
                    fCorrDeduct = Q(FAA82PCCQ8, nTotalDeduct);
                else
                    fCorrDeduct = Q(FAA82PCCQ8, nTotalDeduct);
            }
            else if (sMethod == "pcc.astm")
            {
                if (nNumLargeDeducts == 0)
                    fCorrDeduct = nTotalDeduct;
                else if (nNumLargeDeducts == 1)
                    fCorrDeduct = Q(FAA82PCCQ1, nTotalDeduct);
                else if (nNumLargeDeducts == 2)
                    fCorrDeduct = Q(FAA82PCCQ2, nTotalDeduct);
                else if (nNumLargeDeducts == 3)
                    fCorrDeduct = Q(FAA82PCCQ3, nTotalDeduct);
                else if (nNumLargeDeducts == 4)
                    fCorrDeduct = Q(FAA82PCCQ4, nTotalDeduct);
                else if (nNumLargeDeducts == 5)
                    fCorrDeduct = Q(FAA82PCCQ5, nTotalDeduct);
                else if (nNumLargeDeducts == 6)
                    fCorrDeduct = Q(FAA82PCCQ6, nTotalDeduct);
                else if (nNumLargeDeducts == 7)
                    fCorrDeduct = Q(FAA82PCCQ7, nTotalDeduct);
                else if (nNumLargeDeducts == 8)
                    fCorrDeduct = Q(FAA82PCCQ8, nTotalDeduct);
                else
                    fCorrDeduct = Q(FAA82PCCQ8, nTotalDeduct);
            }
            else if (sMethod == "ac.faa")
            {
                if (nNumLargeDeducts == 0)
                    fCorrDeduct = nTotalDeduct;
                else if (nNumLargeDeducts == 1)
                    fCorrDeduct = Q(FAA82ACQ1, nTotalDeduct);
                else if (nNumLargeDeducts == 2)
                    fCorrDeduct = Q(FAA82ACQ2, nTotalDeduct);
                else if (nNumLargeDeducts == 3)
                    fCorrDeduct = Q(FAA82ACQ3, nTotalDeduct);
                else if (nNumLargeDeducts == 4)
                    fCorrDeduct = Q(FAA82ACQ4, nTotalDeduct);
                else if (nNumLargeDeducts == 5)
                    fCorrDeduct = Q(FAA82ACQ5, nTotalDeduct);
                else if (nNumLargeDeducts == 6)
                    fCorrDeduct = Q(FAA82ACQ6, nTotalDeduct);
                else
                    fCorrDeduct = Q(FAA82ACQ6, nTotalDeduct);
            }
            else if (sMethod == "ac.astm")
            {
                if (nNumLargeDeducts == 0)
                    fCorrDeduct = nTotalDeduct;
                else if (nNumLargeDeducts == 1)
                    fCorrDeduct = Q(FAA82ACQ1, nTotalDeduct);
                else if (nNumLargeDeducts == 2)
                    fCorrDeduct = Q(FAA82ACQ2, nTotalDeduct);
                else if (nNumLargeDeducts == 3)
                    fCorrDeduct = Q(FAA82ACQ3, nTotalDeduct);
                else if (nNumLargeDeducts == 4)
                    fCorrDeduct = Q(FAA82ACQ4, nTotalDeduct);
                else if (nNumLargeDeducts == 5)
                    fCorrDeduct = Q(FAA82ACQ5, nTotalDeduct);
                else if (nNumLargeDeducts == 6)
                    fCorrDeduct = Q(FAA82ACQ6, nTotalDeduct);
                else
                    fCorrDeduct = Q(FAA82ACQ6, nTotalDeduct);
            }
            else if (sMethod == "ac.wci")
            {
                if (nNumLargeDeducts < 2)
                    fCorrDeduct = nTotalDeduct;
                else if (nNumLargeDeducts < 10)
                    fCorrDeduct = (float)CalcWCI_CDV(nNumLargeDeducts, nTotalDeduct);
                else
                    fCorrDeduct = (float)CalcWCI_CDV(9, nTotalDeduct);
            }
            else if (sMethod == "pcc.wci")
            {
                if (nNumLargeDeducts < 2)
                    fCorrDeduct = nTotalDeduct;
                else if (nNumLargeDeducts < 10)
                    fCorrDeduct = (float)CalcWCI_CDV(nNumLargeDeducts, nTotalDeduct);
                else
                    fCorrDeduct = (float)CalcWCI_CDV(9, nTotalDeduct);
            }

            return fCorrDeduct;
        }

        /// <summary>
        ///     this is nothing more than an indexed library of functions f(a): [0,1000]-&gt;R+.
        ///     Semantically Q(i,x) is the a'th q-curve evaluated at pt x.
        /// </summary>
        private static float Q(int i, float x)
        {
            var d = Q_d;

            float val = 0.0f;
            if ((x > 1000) || (x < 0))
            {
                val = 0.0f;
            }
            else if ((i < 0) || (i >= NUMQ))
            {
                val = 0.0f;
            }
            else
            {
                int k;
                for (k = 1; (float)d[i, k].x < x; k++)
                    ;
                {
                    float x1 = d[i, k - 1].x;
                    float y1 = d[i, k - 1].y;
                    float x2 = d[i, k].x;
                    float y2 = d[i, k].y;
                    val = ((x - x1) * (y2 - y1)) / (x2 - x1) + y1;
                }
            }
            return val;
        }

        private static double CalcWCI_CDV(int iQuantity, double dDeductValue)
        {
            /*
            // Asphalt curves
            168	3.119215	0.1584799	0.0269957	-0.000527892	5.06317E-06	-2.41185E-08
            182	-12.42725	1.436821	-0.02498516	0.000428474	-3.89542E-06	1.7264E-08
            200	-6.187479	0.4353937	0.009474656	-0.000153209	1.1821E-06	-4.6033E-09
            200	-15.532	1.098929	-0.01238574	0.000173124	-1.3396E-06	5.09637E-09
            200	4.90E+01	-3.083841	9.13E-02	-1.14E-03	7.50E-06	-2.52E-08
            200	9.674662	-5.93E-01	2.92E-02	-3.58E-04	2.31E-06	-7.75E-09

            */
            // The index is smaller than the quantity fed into this program.
            int iIndex = iQuantity - 2;
            double dCDV = 0.0;
            var dMaxDV = new double[QROWS] { 163.0, 179.0, 198.0, 200.0, 200.0, 200.0, 200.0, 0 };
            double C0 = Params[iIndex, 0];
            double C1 = Params[iIndex, 1];
            double C2 = Params[iIndex, 2];
            double C3 = Params[iIndex, 3];
            double C4 = Params[iIndex, 4];
            double C5 = Params[iIndex, 5];
            double C6 = Params[iIndex, 6];

            if (iQuantity > (QROWS - 1))
                return 0.0;
            if ((dDeductValue > 1000.0) || (dDeductValue < 0.0))
            {
                return 0.0;
            }

            if (dDeductValue < dMaxDV[iIndex])
            {
                dCDV = C0 + (C1 * dDeductValue) + (C2 * Pow(dDeductValue, 2)) + (C3 * Pow(dDeductValue, 3)) +
                    (C4 * Pow(dDeductValue, 4)) + (C5 * Pow(dDeductValue, 5)) + (C6 * Pow(dDeductValue, 6));
            }
            else
            {
                dCDV = 0.0;
            }

            return dCDV;
        }

        #endregion Transliteration of private members

        #region Data

        /// <summary>
        ///     the array Params[] [] contains the values used in the calculation routine. These
        ///     parameters are the constants that make the function computed the proper curve. We
        ///     use this information to compute the curve directly, not approximate as done in other
        ///     parts of this routine.
        /// </summary>
        private static double[,] Params { get; } = new double[QROWS, CVALUES]
        {
            {3.415421, 0.2246326, 0.02872284, -0.0006318123, 0.000006380303, -0.00000003067465, 0.00000000005644553},
            {-1.201932, 0.4330228, 0.01165469, -0.0002100998, 0.000001709584, -0.000000006750956, 0.00000000001035905},
            {12.20614, -0.502545 , .03301571, -0.00046876, 3.38797E-06, -1.22595E-08, 1.75135E-11},
            {18.0909, -0.9794627, 0.04278762, -0.000558333, 3.76568E-06, -1.28973E-08, 1.77034E-11},
            {26.05572, -1.261192, 0.04101644, -0.000438443, 2.35762E-06, -6.13409E-09, 5.87181E-12},
            {52.13357, -2.445436, 0.06009657, -0.000580392, 2.79808E-06, -6.28999E-09, 4.72022E-12},
            {18.83555, -0.8038614, 0.02964717, -0.000338495, 2.09418E-06, -6.81047E-09, 9.07303E-12},
            {12.78057, -0.4164763, 0.01818463, -0.000176542, 8.95002E-07, -2.36785E-09, 2.59654E-12}
        };

        private static int[] p { get; } = new int[SALDISTRESSRANGE * 3]
        {
        /* distress 	 HIGH	   MEDIUM      LOW     */
        /* # in SAL 	  =0	     =1 	=2     */

         /*   0  */ 	  0,         0,     0,     /* 0 unused in SAL*/

         /*   1  */ 	  0,         1,     2,     /* mpr alligator cracking, SAL uses 1, we will use 0 */
         /*   2  */ 	  3,         4,     5,     /* mp ac road bleeding */
         /*   3  */ 	  6,         7,     8,     /* mp ac road block cracking */
         /*   4  */ 	  9,        10,        11,     /* mp ac road bumps adn sags */
         /*   5  */ 	 12,        13,        14,     /* mp ac road corrugation */
         /*   6  */ 	 15,        16,        17,     /* mp ac road depression */
         /*   7  */ 	 18,        19,        20,     /* mp ac road edge cracking */
         /*   8  */ 	 21,        22,        23,     /* mp ac road joint refelection */
         /*   9  */ 	 24,        25,        26,     /* mp ac road ls dropoff */
         /*  10  */ 	 27,        28,        29,     /* mp ac road lon and tra cracking */
         /*  11  */ 	 30,        31,        32,     /* mp ac road patching */
         /*  12  */ 	 33,        34,        35,     /* mp ac road polished agg */
         /*  13  */ 	 36,        37,        38,     /* mp ac road potholes */
         /*  14  */ 	 39,        40,        41,     /* mp ac road rr xing */
         /*  15  */ 	 42,        43,        44,     /* mp ac road rutting */
         /*  16  */ 	 45,        46,        47,     /* mp ac road shoving */
         /*  17  */ 	 48,        49,        50,     /* mp ac road slippage cracking */
         /*  18  */ 	 51,        52,        53,     /* mp ac road swell */
         /*  19  */ 	 54,        55,        56,     /* mp ac road weathering and raveling */

         /*  20  */ 	  0,         0,     0,     /* 20 unused in SAL*/

         /*  21  */ 	 57,        58,        59,     /* mp pcc roadway distresses */
         /*  22  */ 	 60,        61,        62,     /* mp pcc roadway distresses */
         /*  23  */ 	 63,        64,        65,     /* mp pcc roadway distresses */
         /*  24  */ 	 66,        67,        68,     /* mp pcc roadway distresses */
         /*  25  */ 	 69,        70,        71,     /* mp pcc roadway distresses */
         /*  26  */ 	 72,        73,        74,     /* mp pcc roadway distresses */
         /*  27  */ 	 75,        76,        77,     /* mp pcc roadway distresses */
         /*  28  */ 	 78,        79,        80,     /* mp pcc roadway distresses */
         /*  29  */ 	 81,        82,        83,     /* mp pcc roadway distresses */
         /*  30  */ 	 84,        85,        86,     /* mp pcc roadway distresses */
         /*  31  */ 	 87,        88,        89,     /* mp pcc roadway distresses */
         /*  32  */ 	 90,        91,        92,     /* mp pcc roadway distresses */
         /*  33  */ 	 93,        94,        95,     /* mp pcc roadway distresses */
         /*  34  */ 	 96,        97,        98,     /* mp pcc roadway distresses */
         /*  35  */ 	 99,       100,       101,     /* mp pcc roadway distresses */
         /*  36  */ 	102,       103,       104,     /* mp pcc roadway distresses */
         /*  37  */ 	105,       106,       107,     /* mp pcc roadway distresses */
         /*  38  */ 	108,       109,       110,     /* mp pcc roadway distresses */
         /*  39  */ 	111,       112,       113,     /* mp pcc roadway distresses */

         /*  40  */ 	  0,         0,     0,     /* 40 unused in SAL */

         /* distress	 HIGH	   MEDIUM      LOW     */
         /* # in SAL	  =0	     =1 	=2     */

         /*  41  */ 	 120,       121,       122,	/* mpaver & faa82 ac AIRFIELDS ALLIGATOR CRACKING */
         /*  42  */ 	 123,       124,       125,	/* mpaver & faa82 ac AIRFIELDS bleeding  */
         /*  43  */ 	 126,       127,       128,	/* mpaver & faa82 ac AIRFIELDS block cracking */
         /*  44  */ 	 129,       130,       131,	/* mpaver & faa82 ac AIRFIELD corrugation */
         /*  45  */ 	 132,       133,       134,	/* mpaver & faa82 ac AIRFIELDS depression */
         /*  46  */ 	 135,       136,       137,	/* mpaver & faa82 ac AIRFIELDS jet blast erosion  */
         /*  47  */ 	 138,       139,       140,	/* mpaver & faa82 ac AIRFIELDS joint reflection cracking  */
         /*  48  */ 	 141,       142,       143,	/* mpaver & faa82 ac AIRFIELDS longitudinal and transverse cracking */
         /*  49  */ 	 144,       145,       146,	/* mpaver & faa82 ac AIRFIELDS oil spillage  */
         /*  50  */ 	 147,       148,       149,	/* mpaver & faa82 ac AIRFIELDS patching and utility cut  */
         /*  51  */ 	 150,       151,       152,	/* mpaver & faa82 ac AIRFIELDS polished aggregate  */
         /*  52  */ 	 153,       154,       155,	/* mpaver & faa82 ac AIRFIELDS raveling/weathering  */
         /*  53  */ 	 156,       157,       158,	/* mpaver & faa82 ac AIRFIELDS rutting */
         /*  54  */ 	 159,       160,       161,	/* mpaver & faa82 ac AIRFIELDS shoving of flexible pavement by PCC slabs */
         /*  55  */ 	 162,       163,       164,	/* mpaver & faa82 ac AIRFIELDS slippage cracking */
         /*  56  */ 	 165,       166,       167,	/* mpaver & faa82 ac AIRFIELDS swell */

         /*  57  */ 	  0,         0,     0,     /* 57 unused in SAL */
         /*  58  */ 	  0,         0,     0,     /* 58 unused in SAL */
         /*  59  */ 	  0,         0,     0,     /* 59 unused in SAL */
         /*  60  */ 	  0,         0,     0,     /* 60 unused in SAL */

         /*  61  */ 	 180,       181,       182,	/* mpaver & faa82 pcc AIRFIELDS  BLOWUPS */
         /*  62  */ 	 183,       184,       185,	/* mpaver & faa82 pcc AIRFIELDS corner break */
         /*  63  */ 	 186,       187,       188,	/* mpaver & faa82 pcc AIRFIELDS longitudinal/transverse/diagonal cracking */
         /*  64  */ 	 189,       190,       191,	/* mpaver & faa82 pcc AIRFIELDS durability cracking */
         /*  65  */ 	 192,       193,       194,
         /*  66  */ 	 195,       196,       197,	/* mpaver & faa82 pcc AIRFIELDS small patch */
         /*  67  */ 	 198,       199,       200,	/* mpaver & faa82 pcc AIRFIELDS patching/utility cut defect */
         /*  68  */ 	 201,       202,       203,	/* mpaver & faa82 pcc AIRFIELDS popouts */
         /*  69  */ 	 204,       205,       206,	/* mpaver & faa82 pcc AIRFIELDS pumping */
         /*  70  */ 	 207,       208,       209,	/* mpaver & faa82 pcc AIRFIELDS scaling */
         /*  71  */ 	 210,       211,       212,	/* mpaver & faa82 pcc AIRFIELDS settlement */
         /*  72  */ 	 213,       214,       215,	/* mpaver & faa82 pcc AIRFIELDS shattered slab */
         /*  73  */ 	 216,       217,       218,	/* mpaver & faa82 pcc AIRFIELDS shrinkage cracks */
         /*  74  */ 	 219,       220,       221,	/* mpaver & faa82 pcc AIRFIELDS spalling along the joints */
         /*  75  */ 	 222,       223,       224,	/* mpaver & faa82 pcc AIRFIELDS spalling corner */

         /*  76  */ 	  225,       226,   227,     /* wci ac Linear Cracking */
         /*  77  */ 	  228,       229,   230,     /* wci ac Pattern Cracking */
         /*  78  */ 	  231,       232,   233,     /* wci ac Surface Deterioration */
         /*  79  */ 	  234,       235,   236,     /* wci ac Surface Distortion */
         /*  80  */ 	  237,       238,   239,     /* wci ac Surface Defects */
         /*  81  */ 	  240,       241,   242,     /* wci pcc Linear Cracking */
         /*  82  */ 	  243,       244,   245,     /* wci pcc Pattern Cracking */
         /*  83  */ 	  246,       247,   248,     /* wci pcc Surface Deterioration */
         /*  84  */ 	  249,       250,   251,     /* wci pcc Surface Defects */

         /*  85  */ 	  0,         0,     0,     /* unused */
         /*  86  */ 	  0,         0,     0,     /* unused */
         /*  87  */ 	  0,         0,     0,     /* unused */
         /*  88  */ 	  0,         0,     0,     /* unused */
         /*  89  */ 	  0,         0,     0,     /* unused */
         /*  90  */ 	  0,         0,     0,     /* unused */
         /*  91  */ 	  0,         0,     0,     /* unused */
         /*  92  */ 	  0,         0,     0,     /* unused */
         /*  93  */ 	  0,         0,     0,     /* unused */
         /*  94  */ 	  0,         0,     0,     /* unused */
         /*  95  */ 	  0,         0,     0,     /* unused */
         /*  96  */ 	  0,         0,     0,     /* unused */
         /*  97  */ 	  0,         0,     0,     /* unused */
         /*  98  */ 	  0,         0,     0,     /* unused */
         /*  99  */ 	  0,         0,     0,     /* unused */

        /* distress 	 HIGH	   MEDIUM      LOW     */
        /* # in SAL 	  =0	     =1 	=2     */

         /* 100  */ 	  120,       121,       122,     /* faa82 ac AIRFIELDS ALLIGATOR CRACKING */
         /* 101  */ 	  123,       124,   125,     /* faa82 ac AIRFIELDS bleeding  */
         /* 102  */ 	  126,       127,   128,     /* faa82 ac AIRFIELDS block cracking */
         /* 103  */ 	  129,       130,   131,     /* faa82 ac AIRFIELD corrugation */
         /* 104  */ 	  132,       133,   134,     /* faa82 ac AIRFIELDS depression */
         /* 105  */ 	  135,       136,   137,     /* faa82 ac AIRFIELDS jet blast erosion  */
         /* 106  */ 	  138,       139,   140,     /* faa82 ac AIRFIELDS joint reflection cracking  */
         /* 107  */ 	  141,       142,       143,     /* faa82 ac AIRFIELDS longitudinal and transverse cracking */
         /* 108  */ 	  144,       145,   146,     /* faa82 ac AIRFIELDS oil spillage  */
         /* 109  */ 	  147,       148,   149,     /* faa82 ac AIRFIELDS patching and utility cut  */
         /* 110  */ 	  150,       151,   152,     /* faa82 ac AIRFIELDS polished aggregate  */
         /* 111  */ 	  153,       154,   155,     /* faa82 ac AIRFIELDS raveling/weathering  */
         /* 112  */ 	  156,       157,   158,     /* faa82 ac AIRFIELDS rutting */
         /* 113  */ 	  159,       160,   161,     /* faa82 ac AIRFIELDS shoving of flexible pavement by PCC slabs */
         /* 114  */ 	  162,       163,   164,     /* faa82 ac AIRFIELDS slippage cracking */
         /* 115  */ 	  165,       166,   167,	 /* faa82 ac AIRFIELDS swell */

         /* 116  */ 	  0,         0,     0,     /* unused */
         /* 117  */ 	  0,         0,     0,     /* unused */
         /* 118  */ 	  0,         0,     0,     /* unused */
         /* 119  */ 	  0,         0,     0,     /* unused */

        /* distress 	 HIGH	   MEDIUM      LOW     */
        /* # in SAL 	  =0	     =1 	=2     */

         /* 120  */ 	  180,       181,   182,	 /* faa82 pcc AIRFIELDS  BLOWUPS */
         /* 121  */ 	  183,       184,   185,	 /* faa82 pcc AIRFIELDS corner break */
         /* 122  */ 	  186,       187,   188,     /* faa82 pcc AIRFIELDS longitudinal/transverse/diagonal cracking */
         /* 123  */ 	  189,       190,   191,     /* faa82 pcc AIRFIELDS durability cracking */
         /* 124  */ 	  192,       193,   194,     /* faa82 pcc AIRFIELDS small patch */
         /* 125  */ 	  195,       196,   197,     /* faa82 pcc AIRFIELDS patching/utility cut defect */
         /* 126  */ 	  198,       199,   200,     /* faa82 pcc AIRFIELDS popouts */
         /* 127  */ 	  201,       202,   203,     /* faa82 pcc AIRFIELDS pumping */
         /* 128  */ 	  204,       205,   206,     /* faa82 pcc AIRFIELDS scaling */
         /* 129  */ 	  207,       208,   209,     /* faa82 pcc AIRFIELDS settlement */
         /* 130  */ 	  210,       211,   212,     /* faa82 pcc AIRFIELDS shattered slab */
         /* 131  */ 	  213,       214,   215,     /* faa82 pcc AIRFIELDS shrinkage cracks */
         /* 132  */ 	  216,       217,   218,     /* faa82 pcc AIRFIELDS spalling along the joints */
         /* 133  */ 	  219,       220,   221,	 /* faa82 pcc AIRFIELDS spalling corner */

         /* ASTM D53040-10 method for pcc  (pcc.astm) */
         /* 134  */ 	  180,       181,   182,     /* D534010 pcc AIRFIELDS blowups */
         /* 135  */ 	  183,       184,   185,     /* D534010 pcc AIRFIELDS corner break */
         /* 136  */ 	  186,       187,   188,     /* D534010 pcc AIRFIELDS longitudinal/transverse/diagonal cracking*/
         /* 137  */ 	  261,       262,   263,     /* D534010 pcc AIRFIELDS durability cracking */
         /* 138  */ 	  192,       193,   194,     /* D534010 pcc AIRFIELDS joint seal damage*/
         /* 139  */ 	  195,       196,   197,     /* D534010 pcc AIRFIELDS small patch */
         /* 140  */ 	  198,       199,   200,     /* D534010 pcc AIRFIELDS large patching/utility cut */
         /* 141  */ 	  201,       202,   203,     /* D534010 pcc AIRFIELDS popouts */
         /* 142  */ 	  204,       205,   206,	 /* D534010 pcc AIRFIELDS pumping */
         /* 143  */ 	  252,       253,   254,     /* D534010 pcc AIRFIELDS scaling */
         /* 144  */ 	  210,       211,   212,     /* D534010 pcc AIRFIELDS settlement (faulting) */
         /* 145  */ 	  213,       214,   215,     /* D534010 pcc AIRFIELDS shattered slab/Intersecting cracks */
         /* 146  */ 	  216,       217,   218,     /* D534010 pcc AIRFIELDS shrinkage cracks */
         /* 147  */ 	  219,       220,   221,     /* D534010 pcc AIRFIELDS spalling (Transverse and longitudinal joint) */
         /* 148  */		  222,       223,   224,     /* D534010 pcc AIRFIELDS spalling (corner) */
         /* 149  */ 	  255,       256,   257,     /* D534010 pcc AIRFIELDS alkali silica reaction (ASR) */

         /* ASTM D53040-10 method for ac (ac.astm) */
         /*  150 */ 	 120,       121,       122,	/* D534010 ac AIRFIELDS ALLIGATOR CRACKING */
         /*  151 */ 	 123,       124,       125,	/* D534010 ac AIRFIELDS bleeding  */
         /*  152 */ 	 126,       127,       128,	/* D534010 ac AIRFIELDS block cracking */
         /*  153 */ 	 129,       130,       131,	/* D534010 ac AIRFIELD corrugation */
         /*  154 */ 	 132,       133,       134,	/* D534010 ac AIRFIELDS depression */
         /*  155 */ 	 135,       136,       137,	/* D534010 ac AIRFIELDS jet blast erosion  */
         /*  156 */ 	 138,       139,       140,	/* D534010 ac AIRFIELDS joint reflection cracking  */
         /*  157 */ 	 141,       142,       143,	/* D534010 ac AIRFIELDS longitudinal and transverse cracking */
         /*  158  */ 	 144,       145,       146,	/* D534010 ac AIRFIELDS oil spillage  */
         /*  159 */ 	 147,       148,       149,	/* D534010 ac AIRFIELDS patching and utility cut  */
         /*  160 */ 	 150,       151,       152,	/* D534010 ac AIRFIELDS polished aggregate  */
         /*  161 */ 	 153,       154,       155,	/* D534010 ac AIRFIELDS raveling  */
         /*  162 */ 	 156,       157,       158,	/* D534010 ac AIRFIELDS rutting */
         /*  163 */ 	 159,       160,       161,	/* D534010 ac AIRFIELDS shoving of flexible pavement by PCC slabs */
         /*  164 */ 	 162,       163,       164,	/* D534010 ac AIRFIELDS slippage cracking */
         /*  165 */ 	 165,       166,       167,	/* D534010 ac AIRFIELDS swell */
         /* 166  */ 	 258,       259,       260, /* d534010 ac AIRFIELDS weathering */

         /* 167  */ 	  0,         0,     0,     /* unused */
         /* 168  */ 	  0,         0,     0,     /* unused */
         /* 169  */ 	  0,         0,     0,     /* unused */
         /* 170  */ 	  0,         0,     0,     /* unused */
         /* 171  */ 	  0,         0,     0,     /* unused */
         /* 172  */ 	  0,         0,     0,     /* unused */
         /* 173  */ 	  0,         0,     0,     /* unused */
         /* 174  */ 	  0,         0,     0,     /* unused */
         /* 175  */ 	  0,         0,     0,     /* unused */
         /* 176  */ 	  0,         0,     0,     /* unused */
         /* 177  */ 	  0,         0,     0,     /* unused */
         /* 178  */ 	  0,         0,     0,     /* unused */
         /* 179  */ 	  0,         0,     0,     /* unused */
         /* 180  */ 	  0,         0,     0,     /* unused */
         /* 181  */ 	  0,         0,     0,     /* unused */
         /* 182  */ 	  0,         0,     0,     /* unused */
         /* 183  */ 	  0,         0,     0,     /* unused */
         /* 184  */ 	  0,         0,     0,     /* unused */
         /* 185  */ 	  0,         0,     0,     /* unused */
         /* 186  */ 	  0,         0,     0,     /* unused */
         /* 187  */ 	  0,         0,     0,     /* unused */
         /* 188  */ 	  0,         0,     0,     /* unused */
         /* 189  */ 	  0,         0,     0,     /* unused */
         /* 190  */ 	  0,         0,     0,     /* unused */
         /* 191  */ 	  0,         0,     0,     /* unused */
         /* 192  */ 	  0,         0,     0,     /* unused */
         /* 193  */ 	  0,         0,     0,     /* unused */
         /* 194  */ 	  0,         0,     0,     /* unused */
         /* 195  */ 	  0,         0,     0,     /* unused */
         /* 196  */ 	  0,         0,     0,     /* unused */
         /* 197  */ 	  0,         0,     0,     /* unused */
         /* 198  */ 	  0,         0,     0,     /* unused */
         /* 199  */ 	  0,         0,     0     /* unused */
        };

        private static (float x, float y)[,] DeductEval_d { get; } = new (float, float)[NUMCURVES, NUMPTS]
        {
/*******************************************************************************************************************************************************************/
/* begin paver ac airport distresses */

/* AC Road alligator cracking */
/* H = 0*/ {(0,13),(0.1f,13),(0.2f,15),(0.3f,17),(0.5f,22),(1,30),(2,40),(5,53),(10,61.7f),(20,70.63f),(50,81.83f),(100,90.19f),(1100,91) }, /* H AC Road alligator cracking */
/* M = 1*/ {(0, 7),(0.1f, 7.4f),(0.2f, 9.32f),(0.3f,11.47f),(0.5f,15.04f),(1,21.06f),(2,28.08f),(5,38.36f),(10,46.63f),(20,55.25f),(50,67.21f),(100,78.88f),(1100,79) }, /* M AC Road alligator cracking */
/* L = 2*/ {(0, 4),(0.1f, 4),(0.2f, 4),(0.3f, 5),(0.5f, 6),(1,11),(2,16),(5,25),(10,32.13f),(20,40.54f),(50,52.42f),(100,61.27f),(1100,62) }, /* L AC Road alligator cracking */

/* AC Bleeding */
/* H = 3*/ {(0,2),(0.1f, 2),(0.2f, 2),(0.3f, 3),(0.5f, 4),(1, 6),(2, 8),(5,15),(10,22.87f),(20,33.96f),(50,54.08f),(100,74),(1100,74) }, /* H AC Bleeding */
/* M = 4*/ {(0,1),(0.1f, 1),(0.2f, 1),(0.3f, 1),(0.5f, 2),(1, 3),(2, 5),(5, 9),(10,12.78f),(20,18.4f),(50,28.84f),(100,39.65f),(1100,40) }, /* M AC Bleeding */
/* L = 5*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 0),(1, 0),(2, 1),(5, 1),(10, 3.02f),(20, 5.81f),(50,12.25f),(100,20),(1100,20) }, /* L AC Bleeding */

/* AC Block Cracking */
/* H = 6*/ {(0,0),(0.1f, 0),(0.2f, 1),(0.3f, 2),(0.5f, 4),(1, 6),(2,10),(5,20),(10,29.48f),(20,41.06f),(50,58.3f),(100,71.72f),(1100,72) }, /* H AC Block Cracking */
/* M = 7*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 0),(1, 2),(2, 5),(5,11),(10,15.75f),(20,22.53f),(50,33.51f),(100,43.22f),(1100,44) }, /* M AC Block Cracking */
/* L = 8*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 0),(1, 0),(2, 1),(5, 5),(10, 8),(20,12.55f),(50,20.36f),(100,27.56f),(1100,28) }, /* L AC Block Cracking */

/*  AC Bumps and Sags */
/* H = 9*/ {(0,20),(0.1f,20),(0.2f,30),(0.3f,35),(0.5f,43),(1,52.44f),(2,64.01f),(5,82.23f),(10,99.92f),(20,99.92f),(50,99.92f),(100,99.92f),(1100,100) }, /* H Road AC Bumps and Sags */
/* M =10*/ {(0, 7),(0.1f, 7),(0.2f,10),(0.3f,13),(0.5f,17),(1,23.67f),(2,32.7f),(5,52.85f),(10, 79.35f),(20, 79.35f),(50, 79.35f),(100, 79.35f),(1100, 79.35f) }, /* M Road AC Bumps and Sags */
/* L =11*/ {(0, 1),(0.1f, 1),(0.2f, 1),(0.3f, 2),(0.5f, 4),(1, 6.8f),(2,12.0f),(5,24.29f),(10, 40),(20, 40),(50, 40),(100, 40),(1100, 40) }, /* L Road AC Bumps and Sags */

/*  AC Corrugation */
/* H =12*/ {(0,20),(0.1f,10),(0.2f,18),(0.3f,23),(0.5f,28),(1,33.74f),(2,40.94f),(5,51.8f),(10,61.1f),(20,71.13f),(50,84.75f),(100,94.42f),(1100,95) }, /* H AC Road Corrugation */
/* M =13*/ {(0, 7),(0.1f, 5),(0.2f, 6),(0.3f, 8),(0.5f,10),(1,15.53f),(2,21.71f),(5,31.37f),(10,39.67f),(20,48.79f),(50,62.2f),(100,73.52f),(1100,74) }, /* M AC Road Corrugation */
/* L =14*/ {(0, 1),(0.1f, 1),(0.2f, 1),(0.3f, 1),(0.5f, 1),(1, 1.72f),(2, 3.55f),(5, 8.06f),(10,13.21f),(20,19.83f),(50,30.35f),(100,39.66f),(1100,39) }, /* L AC Road Corrugation */

/*  AC Depression */
/* H =15*/ {(0,12),(0.1f,12),(0.2f,13),(0.3f,14),(0.5f,15),(1,16.25f),(2,19.9f),(5,31.04f),(10,43.59f),(20,56.76f),(50,69.7f),(100,74.32f),(1100,75) }, /* H AC Road Depression */
/* M =16*/ {(0, 8),(0.1f, 8),(0.2f, 8),(0.3f, 8),(0.5f, 8),(1, 9.18f),(2,11.54f),(5,20.04f),(10,30.6f),(20,42.88f),(50,56.47f),(100,60.31f),(1100,60.31f) }, /* M AC Road Depression */
/* L =17*/ {(0, 4),(0.1f, 4),(0.2f, 4),(0.3f, 4),(0.5f, 4),(1, 4.58f),(2, 5.78f),(5, 10.97f),(10,18.54f),(20,28.84f),(50,42.85f),(100,47.87f),(1100,48) }, /* L AC Road Depression */

/*  AC Edge Cracking */
/* H =18*/ {(0,7),(0.1f, 7),(0.2f, 8),(0.3f, 9),(0.5f,10),(1,13.04f),(2,19.02f),(5,30.11f),(10,39.1f),(20,45.92f),(50,46),(100,46),(1100,46) }, /* H AC Road Edge Cracking */
/* M =19*/ {(0,4),(0.1f, 4),(0.2f, 4),(0.3f, 5),(0.5f, 6),(1, 8.12f),(2,11.61f),(5,17.99f),(10,23.4f),(20,28.21f),(50,28.21f),(100,28.21f),(1100,28.21f) }, /* M AC Road Edge Cracking */
/* L =20*/ {(0,0),(0.1f, 0),(0.2f, 1),(0.3f, 2),(0.5f, 3),(1, 2.68f),(2, 4.01f),(5, 7.27f),(10,10.84f),(20,15.03f),(50,15.03f),(100,15.03f),(1100,15.03f) }, /* L AC Road Edge Cracking */

/*  AC Joint Reflection  Cracking  */
/* H =21*/ {(0,2),(0.1f, 2),(0.2f, 5.04f),(0.3f, 7),(0.5f,10),(1,14.01f),(2,22.51f),(5,42.72f),(10,60.03f),(20,71),(30,72.18f),(100,72.18f),(1100,73) }, /* H AC Road Joint Reflection Cracking */
/* M =22*/ {(0,1),(0.1f, 0.1f),(0.2f, 2.01f),(0.3f, 3),(0.5f, 3.61f),(1, 6.62f),(2,12.18f),(5,23.09f),(10,32.48f),(20,40.35f),(30,42.98f),(100,43),(1100,43) }, /* M AC Road Joint Reflection Cracking */
/* L =23*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 1),(1, 2.37f),(2, 4.48f),(5, 8.93f),(10,13.998f),(20,20.93f),(30,26.06f),(100,26.06f),(1100,26) }, /* L AC Road Joint Reflection Cracking */

/*  AC Lane/Shoulder Drop-off */
/* H =24*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 6),(1, 7),(2,10),(5,19),(10,34),(15,44),(50,44),(100,44),(1100,44) }, /* H AC Road Lane/Shoulder Drop-off */
/* M =25*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 4),(1, 4),(2, 6),(5,11),(10,19),(15,28),(50,28),(100,28),(1100,28) }, /* M AC Road Lane/Shoulder Drop-off */
/* L =26*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 2),(1, 2),(2, 4),(5, 7),(10,12),(15,16),(50,16),(100,16),(1100,16) }, /* L AC Road Lane/Shoulder Drop-off */

/*  AC Longitudinal and Transverse Cracking */
/* H =27*/ {(0,3.4f),(0.1f, 3.4f),(0.2f, 6.45f),(0.3f, 9.01f),(0.5f,12.51f),(1,18.19f),(2,26.53f),(5,44.24f),(10,62.26f),(20,79.78f),(30, 86.26f),(100,86.26f),(1100,87) }, /* H AC Road Longitudinal and Transverse Cracking */
/* M =28*/ {(0,0),(0.1f, 0),(0.2f, 0.92f),(0.3f, 2.13f),(0.5f, 4.33f),(1, 8.43f),(2,13.75f),(5,22.55f),(10,30.49f),(20,39.52f),(30,45.3f),(100,45.3f),(1100,45.3f) }, /* M AC Road Longitudinal and Transverse Cracking */
/* L =29*/ {(0,0),(0.1f, 0),(0.2f, 0.03f),(0.3f, 0.14f),(0.5f, 0.58f),(1, 2.03f),(2, 4.74f),(5,10.53f),(10,16.58f),(20,23.9f),(30,28.64f),(100,28.64f),(1100,28.64f) }, /* L AC Road Longitudinal and Transverse Cracking */

/*  AC Large Patching and Utility  Cut Patching */
/* H =30*/ {(0,6),(0.1f, 6),(0.2f, 8.62f),(0.3f,11),(0.5f,14.58f),(1,18.66f),(2,24.37f),(5,37.6f),(10,51.73f),(20,66.6f),(50,80.64f),(100,80.64f),(1100,80.64f) }, /* H AC Road Large Patching and Utility Cut Patching */
/* M =31*/ {(0,3),(0.1f, 3),(0.2f, 4.21f),(0.3f, 5),(0.5f, 6.6f),(1, 9.57f),(2,13.95f),(5,22.35f),(10,30.89f),(20,41.41f),(50,58.34f),(100,58.34f),(1100,58.34f) }, /* M AC Road Large Patching and Utility Cut Patching */
/* L =32*/ {(0,0),(0.1f, 0),(0.2f, 0.52f),(0.3f, 0),(0.5f, 1.08f),(1, 2.32f),(2, 4.78f),(5,10.28f),(10,16.1f),(20,23.06f),(50,33.12f),(100,33.12f),(1100,33.12f) }, /* L AC Road Large Patching and Utility Cut Patching */

/*  AC Polished Aggregate (dup)  */
/* X =33*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 0),(1, 0),(2, 0),(5, 1),(10, 3),(20, 8),(50,12),(100,20),(1100,20) }, /* - AC Road Polished Aggregate (dup) */
/* X =34*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 0),(1, 0),(2, 0),(5, 1),(10, 3),(20, 8),(50,12),(100,20),(1100,20) }, /* - AC Road Polished Aggregate (dup) */
/* X =35*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 0),(1, 0),(2, 0),(5, 1),(10, 3),(20, 8),(50,12),(100,20),(1100,20) }, /* - AC Road Polished Aggregate ----- */

/*  AC Potholes */
/* H =36*/ {(0,21),(0.01f,21),(0.02f,28),(0.03f,33),(0.05f,41),(0.1f,54.89f),(0.2f,69.8f),(0.5f,91.58f),(0.7f,100),(2.0f,100),(5,100),(10,100),(1100,100) }, /* H AC Road Potholes */
/* M =37*/ {(0, 6),(0.01f, 6),(0.02f,10),(0.03f,15),(0.05f,22),(0.1f,34),(0.2f,48.77f),(0.5f,71.38f),(1.0f, 89.72f),(1.5f,100),(5,100),(10,100),(1100,100) }, /* M AC Road Potholes */
/* L =38*/ {(0, 2),(0.01f, 2),(0.02f, 5),(0.03f, 8),(0.05f,13),(0.1f,21.39f),(0.2f,31),(0.5f,45.75f),(1.0f, 58),(2.0f, 70.87f),(5, 87.77f),(10,100),(1100,100) }, /* L AC Road Potholes */

/*  AC Railroad Crosing */
/* H =39*/ {(0,20),(0.1f,20),(0.2f,20),(0.3f,20),(0.5f,20),(1,20),(2,29),(5,50),(10,68),(20,77),(50,80),(100,80),(1100,80) }, /* H AC Road Railroad Crosing */
/* M =40*/ {(0, 8),(0.1f, 8),(0.2f, 8),(0.3f, 8),(0.5f, 8),(1, 8),(2,12),(5,26),(10,39),(20,46),(50,51),(100,51),(1100,51) }, /* M AC Road Railroad Crosing */
/* L =41*/ {(0, 2),(0.1f, 2),(0.2f, 2),(0.3f, 2),(0.5f, 2),(1, 2),(2, 2),(5, 6),(10,12),(20,17),(50,20),(100,20),(1100,20) }, /* L AC Road Railroad Crosing */

/*  AC Rutting */
/* H =42*/ {(0,6),(0.1f, 6.4f),(0.2f,12.14f),(0.3f,16),(0.5f,20.42f),(1,27.35f),(2,35.34f),(5,48.19f),(10,59.67f),(20,71.82f),(50,85.67f),(100,90),(1100,90) }, /* H AC Road Rutting */
/* M =43*/ {(0,4),(0.1f, 4),(0.2f, 6.97f),(0.3f, 9),(0.5f,12.46f),(1,17.91f),(2,24.58f),(5,35.17f),(10,44.14f),(20,53.16f),(50,63),(100,67),(1100,67) }, /* M AC Road Rutting */
/* L =44*/ {(0,1),(0.1f, 1),(0.2f, 1.71f),(0.3f, 2),(0.5f, 4.35f),(1, 7.86f),(2,12.72f),(5,21),(10,28.13f),(20,35.65f),(50,45.04f),(100,50.73f),(1100,50.73f) }, /* L AC Road Rutting */

/*  AC Shoving */
/* H =45*/ {(0,7),(0.1f, 7),(0.2f,10),(0.3f,12),(0.5f,15),(1,19),(2,25),(5,37),(10,53),(20,67),(50,80),(100,80),(1100,80) }, /* H AC Road Shoving */
/* M =46*/ {(0,3),(0.1f, 3),(0.2f, 3),(0.3f, 4),(0.5f, 6),(1,10),(2,15),(5,25),(10,35),(20,49),(50,64),(100,64),(1100,64) }, /* M AC Road Shoving */
/* L =47*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 1),(1, 4),(2, 8),(5,14),(10,20),(20,27),(50,36),(100,36),(1100,36) }, /* L AC Road Shoving */

/*  AC Slippage Cracking */
/* H =48*/ {(0,4),(0.1f, 4),(0.2f, 7),(0.3f, 9),(0.5f,13),(1,19),(2,30),(5,52),(10,67),(20,78),(50,86),(100,90),(1100,90) }, /* H AC Road Slippage Cracking */
/* M =49*/ {(0,2),(0.1f, 2),(0.2f, 4),(0.3f, 5),(0.5f, 7),(1,11),(2,17),(5,33),(10,44),(20,53),(50,64),(100,70),(1100,70) }, /* M AC Road Slippage Cracking */
/* L =50*/ {(0,0),(0.1f, 0),(0.2f, 1),(0.3f, 2),(0.5f, 2),(1, 4),(2, 8),(5,19),(10,27),(20,35),(50,46),(100,54),(1100,54) }, /* L AC Road Slippage Cracking */

/*  H AC Swell */
/* H =51*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 0),(1,34),(2,37),(5,45),(10,54),(20,64),(30,71),(100,71),(1100,71) }, /* H AC Road Swell */
/* M =52*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 0),(1,12),(2,18),(5,27),(10,35),(20,45),(30,51),(100,51),(1100,51) }, /* M AC Road Swell */
/* L =53*/ {(0,0),(0.1f, 0),(0.2f, 0),(0.3f, 0),(0.5f, 0),(1, 2),(2, 5),(5, 9),(10,12),(20,17),(30,20),(100,20),(1100,20) }, /* L AC Road Swell */

/*  H AC Weathering and Raveling */
/* H =54*/ {(0,7),(0.1f, 7),(0.2f, 9),(0.3f,11),(0.5f,13),(1,16),(2,21),(5,30),(10,41),(20,56),(50,70),(100,78),(1100,78) }, /* H AC Road Weathering and Raveling */
/* M =55*/ {(0,4),(0.1f, 4),(0.2f, 6),(0.3f, 7),(0.5f, 8),(1, 9),(2,10),(5,13),(10,18),(20,25),(50,35),(100,44),(1100,44) }, /* M AC Road Weathering and Raveling */
/* L =56*/ {(0,0),(0.1f, 0),(0.2f, 1),(0.3f, 1),(0.5f, 1),(1, 2),(2, 2),(5, 3),(10, 5),(20, 8),(50,12),(100,16),(1100,16) }, /* L AC Road Weathering and Raveling */

/* end of paver ac roadway distresses ------------------------------------------------------------------------------------------------------------------------*/
/*******************************************************************************************************************************************************************/
/* begin paver pcc roadway distresses ------------------------------------------------------------------------------------------------------------------------*/

/*  PCC Blow-Ups */
/* H =57*/ {(-1,0),(0, 0),( 5,50),(10,60),(20,73),(30,81), (40,88),(50,94),(60,100),(80,100),(90,100),(100,100),(1100,100) }, /* H Road PCC Blow-Ups */
/* M =58*/ {(-1,0),(0, 0),(10,18),(20,34),(30,47),(40,58), (50,67),(60,73), (70,77), (80,80), (90,83), (100,84), (1100,84) }, /* M Road PCC Blow-Ups */
/* L =59*/ {(-1,0),(0, 0),(10, 8),(20,17),(30,24),(40,31), (50,35),(60,38), (70,41), (80,42), (90,43), (100,44), (1100,44) }, /* L Road PCC Blow-Ups */

/*  PCC Corner Breaks */
/* H =60*/ {(-1,0),(0, 0),(10,27),(20,41),(30,52),(40,61), (50,67),(60,72),(70,75),(80,77),(90,78),(100,79),(1100,79) }, /* H PCC Road Corner Breaks */
/* M =61*/ {(-1,0),(0, 0),(10,16),(20,28),(30,40),(40,47), (50,53),(60,56),(70,58),(80,61),(90,62),(100,64),(1100,64) }, /* M PCC Road Corner Breaks */
/* L =62*/ {(-1,0),(0, 0),(10, 8),(20,17),(30,25),(40,32), (50,38),(60,42),(70,45),(80,47),(90,49),(100,50),(1100,50) }, /* L PCC Road Corner Breaks */

/*  PCC Divided Slab */
/* H =63*/ {(-1,0),(0, 0),(10,37),(20,52),(30,62),(40,69), (50,75),(60,79),(70,83),(80,86),(90,89),(100,92),(1100,92) }, /* H PCC Road Divided Slab */
/* M =64*/ {(-1,0),(0, 0),(10,24),(20,36),(30,44),(40,52), (50,57),(60,62),(70,67),(80,70),(90,73),(100,75),(1100,75) }, /* M PCC Road Divided Slab */
/* L =65*/ {(-1,0),(0, 0),(10,11),(20,20),(30,28),(40,34), (50,39),(60,43),(70,46),(80,48),(90,49),(100,51),(1100,51) }, /* L PCC Road Divided Slab */

/*  PCC Durability "D" Cracking */
/* H =66*/ {(-1,0),(0, 0),(10,24),(20,40),(30,50),(40,56), (50,60),(60,63),(70,66),(80,68),(90,69),(100,70),(1100,70) }, /* H PCC Road Durability "D" Cracking */
/* M =67*/ {(-1,0),(0, 0),(10,10),(20,18),(30,25),(40,30), (50,34),(60,37),(70,39),(80,41),(90,43),(100,44),(1100,44) }, /* M PCC Road Durability "D" Cracking */
/* L =68*/ {(-1,0),(0, 0),(10, 4),(20, 8),(30,11),(40,13), (50,16),(60,18),(70,20),(80,21),(90,22),(100,23),(1100,23) }, /* L PCC Road Durability "D" Cracking */

/*  PCC Faulting */
/* H =69*/ {(-1,0),(0, 0),(10,17),(20,30),(30,42),(40,52), (50,62),(60,68),(70,74),(80,77),(90,81),(100,84),(1100,84) }, /* H PCC Road Faulting */
/* M =70*/ {(-1,0),(0, 0),(10, 8),(20,16),(30,24),(40,33), (50,40),(60,45),(70,47),(80,51),(90,53),(100,54),(1100,54) }, /* M PCC Road Faulting */
/* L =71*/ {(-1,0),(0, 0),(10, 2),(20, 7),(30,13),(40,19), (50,22),(60,24),(70,26),(80,28),(90,29),(100,30),(1100,30) }, /* L PCC Road Faulting */

/*  PCC Joint Seal Damage */
/* H =72*/ {(-1,0),(0, 8),(10, 8),(20, 8),(30, 8),(40, 8), (50, 8),(60, 8),(70, 8),(80, 8),(90, 8),(100, 8),(1100, 8) }, /* H PCC Road Joint Seal Damage */
/* M =73*/ {(-1,0),(0, 4),(10, 4),(20, 4),(30, 4),(40, 4), (50, 4),(60, 4),(70, 4),(80, 4),(90, 4),(100, 4),(1100, 4) }, /* M PCC Road Joint Seal Damage */
/* L =74*/ {(-1,0),(0, 2),(10, 2),(20, 2),(30, 2),(40, 2), (50, 2),(60, 2),(70, 2),(80, 2),(90, 2),(100, 2),(1100, 2) }, /* L PCC Road Joint Seal Damage */

/*  PCC Lane/Shoulder Drop-Off */
/* H =H =75*/ {(-1,0),(0, 0),(10, 9),(20,15),(30,19),(40,23), (50,26),(60,28),(70,29),(80,30),(90,32),(100,33),(1100,33) }, /* H PCC Road Lane/Shoulder Drop-Off */
/* M =M =76*/ {(-1,0),(0, 0),(10, 5),(20, 9),(30,12),(40,14), (50,16),(60,17),(70,18),(80,18),(90,18),(100,18),(1100,18) }, /* M PCC Road Lane/Shoulder Drop-Off */
/* L =L =77*/ {(-1,0),(0, 0),(10, 1),(20, 2),(30, 2),(40, 3), (50, 4),(60, 4),(70, 5),(80, 5),(90, 5),(100, 5),(1100, 5) }, /* L PCC Road Lane/Shoulder Drop-Off */

/*  PCC Linear Cracking */
/* H =H =78*/ {(-1,0),(0, 0),(10,21),(20,30),(30,36),(40,42), (50,46),(60,50),(70,54),(80,58),(90,61),(100,64),(1100,64) }, /* H PCC Road Linear Cracking */
/* M =M =79*/ {(-1,0),(0, 0),(10, 9),(20,15),(30,20),(40,24), (50,27),(60,30),(70,33),(80,35),(90,36),(100,38),(1100,38) }, /* M PCC Road Linear Cracking */
/* L =L =80*/ {(-1,0),(0, 0),(10, 6),(20,11),(30,14),(40,17), (50,20),(60,21),(70,22),(80,23),(90,23),(100,23),(1100,23) }, /* L PCC Road Linear Cracking */

/*  PCC Large Patching and Utility Cut Patching  */
/* H =81*/ {(-1,0),(0, 0),(10,19),(20,31),(30,39),(40,47), (50,53),(60,57),(70,62),(80,66),(90,69),(100,72),(1100,72) }, /* H PCC Road Large Patching and Utility Cut Patching   */
/* M =82*/ {(-1,0),(0, 0),(10, 5),(20,12),(30,21),(40,28), (50,34),(60,38),(70,42),(80,44),(90,47),(100,48),(1100,48) }, /* M PCC Road Large Patching and Utility Cut Patching   */
/* L =83*/ {(-1,0),(0, 0),(10, 2),(20, 7),(30,13),(40,16), (50,19),(60,21),(70,23),(80,25),(90,27),(100,28),(1100,28) }, /* L PCC Road Large Patching and Utility Cut Patching   */

/*  PCC Small Patching */
/* H =84*/ {(-1,0),(0, 0),(10, 5),(20,10),(30,13),(40,16), (50,18),(60,21),(70,23),(80,24),(90,25),(100,25),(1100,25) }, /* H PCC Road Small Patching */
/* M =85*/ {(-1,0),(0, 0),(10, 2),(20, 4),(30, 6),(40, 9), (50,11),(60,13),(70,14),(80,15),(90,15),(100,15),(1100,15) }, /* M PCC Road Small Patching */
/* L =86*/ {(-1,0),(0, 0),(10, 0),(20, 1),(30, 2),(40, 2), (50, 3),(60, 3),(70, 3),(80, 3),(90, 4),(100, 4),(1100, 4) }, /* L PCC Road Small Patching */

/*  PCC Polished Aggregates  (dup) */
/* X =87*/ {(-1,0),(0, 0),(10, 2),(20, 3),(30, 5),(40, 6), (50, 7),(60, 8),(70, 8),(80, 9),(90, 9),(100, 9),(1100, 9) }, /* - PCC Road Polished Aggregates  (dup) */
/* X =88*/ {(-1,0),(0, 0),(10, 2),(20, 3),(30, 5),(40, 6), (50, 7),(60, 8),(70, 8),(80, 9),(90, 9),(100, 9),(1100, 9) }, /* - PCC Road Polished Aggregates  (dup) */
/* X =89*/ {(-1,0),(0, 0),(10, 2),(20, 3),(30, 5),(40, 6), (50, 7),(60, 8),(70, 8),(80, 9),(90, 9),(100, 9),(1100, 9) }, /* - PCC Road Polished Aggregates  ----- */

/*  PCC Popouts (dup) */
/* X =90*/ {(-1,0),(0, 0),(10, 2),(20, 3),(30, 5),(40, 7), (50, 9),(60,11),(70,13),(80,14),(90,14),(100,14),(1100,14) }, /* - PCC Road Popouts (dup) */
/* X =91*/ {(-1,0),(0, 0),(10, 2),(20, 3),(30, 5),(40, 7), (50, 9),(60,11),(70,13),(80,14),(90,14),(100,14),(1100,14) }, /* - PCC Road Popouts (dup) */
/* X =92*/ {(-1,0),(0, 0),(10, 2),(20, 3),(30, 5),(40, 7), (50, 9),(60,11),(70,13),(80,14),(90,14),(100,14),(1100,14) }, /* - PCC Road Popouts ----- */

/*  PCC Pumping (dup) */
/* X =93*/ {(-1,0),(0, 0),(10, 7),(20,12),(30,17),(40,22), (50,26),(60,30),(70,33),(80,35),(90,37),(100,38),(1100,38) }, /* - PCC Road Pumping (dup) */
/* X =94*/ {(-1,0),(0, 0),(10, 7),(20,12),(30,17),(40,22), (50,26),(60,30),(70,33),(80,35),(90,37),(100,38),(1100,38) }, /* - PCC Road Pumping (dup) */
/* X =95*/ {(-1,0),(0, 0),(10, 7),(20,12),(30,17),(40,22), (50,26),(60,30),(70,33),(80,35),(90,37),(100,38),(1100,38) }, /* - PCC Road Pumping ----- */

/*  PCC Punch Outs*/
/* H =96*/ {(-1,0),(0, 0),(10,35),(20,50),(30,60),(40,66), (50,72),(60,77),(70,80),(80,83),(90,85),(100,87),(1100,87) }, /* H PCC Road Punch Outs*/
/* M =97*/ {(-1,0),(0, 0),(10,25),(20,40),(30,48),(40,55), (50,61),(60,65),(70,68),(80,70),(90,71),(100,71),(1100,71) }, /* M PCC Road Punch Outs*/
/* L =98*/ {(-1,0),(0, 0),(10,16),(20,28),(30,36),(40,42), (50,47),(60,50),(70,52),(80,54),(90,55),(100,55),(1100,55) }, /* L PCC Road Punch Outs*/

/*  PCC Railroad Crossing */
/*H = 99*/ {(-1,0),(0, 0),(10,56),(20,72),(30,80),(40,87), (50,92),(60,95),(70,95),(80,95),(90,95),(100,95),(1100,95) }, /* H PCC Road Railroad Crossing */
/*M =100*/ {(-1,0),(0, 0),(10,21),(20,30),(30,36),(40,41), (50,46),(60,50),(70,50),(80,50),(90,50),(100,50),(1100,50) }, /* M PCC Road Railroad Crossing */
/*L =101*/ {(-1,0),(0, 0),(10,10),(20,16),(30,20),(40,24), (50,27),(60,29),(70,29),(80,29),(90,29),(100,29),(1100,29) }, /* L PCC Road Railroad Crossing */

/*  PCC Scaling/Map Cracking/Crazing */
/*H =102*/ {(-1,0),(0, 0),(10,18),(20,28),(30,36),(40,41), (50,45),(60,49),(70,53),(80,57),(90,61),(100,65),(1100,65) }, /* H PCC Road Scaling/Map Cracking/Crazing */
/*M =103*/ {(-1,0),(0, 0),(10, 9),(20,15),(30,18),(40,21), (50,24),(60,25),(70,27),(80,29),(90,30),(100,31),(1100,31) }, /* M PCC Road Scaling/Map Cracking/Crazing */
/*L =104*/ {(-1,0),(0, 0),(10, 3),(20, 4),(30, 6),(40, 8), (50, 9),(60,10),(70,10),(80,11),(90,11),(100,11),(1100,11) }, /* L PCC Road Scaling/Map Cracking/Crazing */

/*  PCC Shrinkage Cracks (dup) */
/*H =105*/ {(-1,0),(0, 0),(10, 0),(20, 0),(30, 1),(40, 2), (50, 3),(60, 3),(70, 3),(80, 3),(90, 4),(100, 4),(1100, 4) }, /* - PCC Road Shrinkage Cracks (dup) */
/*M =106*/ {(-1,0),(0, 0),(10, 0),(20, 0),(30, 1),(40, 2), (50, 3),(60, 3),(70, 3),(80, 3),(90, 4),(100, 4),(1100, 4) }, /* - PCC Road Shrinkage Cracks (dup) */
/*L =107*/ {(-1,0),(0, 0),(10, 0),(20, 0),(30, 1),(40, 2), (50, 3),(60, 3),(70, 3),(80, 3),(90, 4),(100, 4),(1100, 4) }, /* - PCC Road Shrinkage Cracks ----- */

/*  PCC Corner Spalling */
/*H =108*/ {(-1,0),(0, 0),(10, 7),(20,13),(30,17),(40,20), (50,23),(60,25),(70,27),(80,29),(90,29),(100,30),(1100,30) }, /* H PCC Road Corner Spalling */
/*M =109*/ {(-1,0),(0, 0),(10, 3),(20, 6),(30,10),(40,13), (50,15),(60,18),(70,20),(80,21),(90,22),(100,22),(1100,22) }, /* M PCC Road Corner Spalling */
/*L =110*/ {(-1,0),(0, 0),(10, 1),(20, 3),(30, 5),(40, 8), (50,10),(60,11),(70,13),(80,14),(90,14),(100,14),(1100,14) }, /* L PCC Road Corner Spalling */

/*  PCC Joint Spalling */
/*H =111*/ {(-1,0),(0, 0),(10,15),(20,25),(30,32),(40,37), (50,42),(60,45),(70,48),(80,52),(90,54),(100,56),(1100,56) }, /* H PCC Road Joint Spalling */
/*M =112*/ {(-1,0),(0, 0),(10, 4),(20, 8),(30,13),(40,16), (50,19),(60,21),(70,23),(80,24),(90,25),(100,25),(1100,25) }, /* M PCC Road Joint Spalling */
/*L =113*/ {(-1,0),(0, 0),(10, 2),(20, 4),(30, 6),(40, 8), (50,10),(60,12),(70,12),(80,12),(90,12),(100,12),(1100,12) }, /* L PCC Road Joint Spalling */

/* end of paver pcc roadway distresses ---------------------------------------------------------------------------------------------------------------------*/
/*******************************************************************************************************************************************************************/
/* begin paver ac roadway scaling curves (q=1 no scaling, omitted) ---------------------------------------------------------------------------------------------------------*/

/*q=2*/ { (-10, 0), (0, 0), (13,  9), (40, 30), (60, 44), (80, 57), (100, 70), (120, 82), (150, 94), (170,  100), (1000, 100), (1000, 100), (1000, 100) },     /*38H for q=2 */
/*q=3*/ { (-10, 0), (0, 0), (19,  9), (40, 25), (60, 38), (80, 52), (100, 63), (120, 74), (150, 87), (170,   95), ( 183, 100), (1000, 100), (1000, 100) },     /*38M for q=3 */
/*q=4*/ { (-10, 0), (0, 0), (25,  9), (100,57), (120,68), (150,83), (170, 90), (185, 95), (200, 97), (1000, 100), (1000, 100), (1000, 100), (1000, 100) },     /*38L for q=4 */
/*q=5*/ { (-10, 0), (0, 0), (28,  9), (100,52), (150,77), (170,85), (190, 91), (200,  93),  (1000, 100), (1000, 100), (1000, 100), (1000, 100), (1000, 100) },      /*39H for q=5 */
/*q=6*/ { (-10, 0), (0, 0), (42, 16), (120,58), (160,77), (180,84), (200, 89), (1000, 100), (1000, 100), (1000, 100), (1000, 100), (1000, 100), (1000, 100) },      /*39M for q=6 */
/*q=7*/ { (-10, 0), (0, 0), (42, 16), (120,58), (160,75), (180,79), (200, 82), (1000, 100), (1000, 100), (1000, 100), (1000, 100), (1000, 100), (1000, 100) },      /*39L for q=7 */

/* end paver ac roadway scaling curves (q=1 no scaling, omitted) ---------------------------------------------------------------------------------------------------------*/
/*******************************************************************************************************************************************************************/
/* begin faa82 ac airfield distreses. */

/* AC alligator cracking #41 */
/* H=120 */ { (0,16.25f), (0.1f,16.25f), (0.2f,21.5f), (0.3f,25), (0.5f,30), (1,37), (2,45), (5,58), (10,70), (20,84), (50,96.25f), (100,100), (1100, 100) },     /* H FAA82  alligator cracking */
/* M=121 */ { (0,10), (0.1f,10), (0.2f,15), (0.3f,18.75f), (0.5f,23), (1,29.5f), (2,36.5f), (5,47), (10,56), (20,66), (50,77.5f), (100,84), (1100,84) },      /* M FAA82  alligator cracking */
/* L=122 */ { (0,7.5f), (0.1f,7.5f), (0.2f,8.75f), (0.3f,11.25f), (0.5f,15), (1,21), (2,27.5f), (5,36.25f), (10,43.5f), (20,51), (50,63), (100,74), (1100,74) },     /* L FAA82  alligator cracking */

/* AC bleeding */
/* H=123 */ { (0,0), (0.1f,0), (0.2f,1.25f), (0.3f,2), (0.5f,3.75f), (1,6.5f), (2,11.25f), (5,25), (10,40), (20,51), (50,62), (100,67), (1100,67) },     /* H AC  bleeding */
/* M=124 */ { (0,0), (0.1f,0), (0.2f,1.25f), (0.3f,2), (0.5f,3.75f), (1,6.5f), (2,11.25f), (5,25), (10,40), (20,51), (50,62), (100,67), (1100,67) },     /* M AC cracking */
/* L=125 */ { (0,0), (0.1f,0), (0.2f,1.25f), (0.3f,2), (0.5f,3.75f), (1,6.5f), (2,11.25f), (5,25), (10,40), (20,51), (50,62), (100,67), (1100,67) },     /* L AC  bleeding */

/* AC block cracking */
/* H=126 */ { (0,10), (0.1f,10), (0.2f,11.25f), (0.3f,12.5f), (0.5f,15), (1,19), (2,24.5f), (5,33.75f), (10,41.25f), (20,50), (50,65), (100,78.75f), (1100,78.75f) },     /* H AC  block cracking */
/* M=127 */ { (0,8), (0.1f,8), (0.2f,8), (0.3f,8.5f), (0.5f,9), (1,11.5f), (2,14), (5,18.75f), (10,23.75f), (20,29), (50,40), (100,55), (1100,55) },          /* M AC  block cracking */
/* L=128 */ { (0,5), (0.1f,5), (0.2f,5), (0.3f,5.5f), (0.5f,6.25f), (1,7.5f), (2,9), (5,13), (10,16.5f), (20,20.5f), (50,28), (100,35), (1100,35) },          /* L AC  block cracking */

/* AC corrugation */
/* H=129 */ { (0,11.25f), (0.1f,11.25f), (0.2f,16.25f), (0.3f,19.5f), (0.5f,25), (1,32.5f), (2,41.25f), (5,55), (10,67.5f), (20,82.5f), (40,100), (100,100), (1100,100) },     /* H AC  corrugation */
/* M=130 */ { (0,6), (0.1f,6), (0.2f,7.5f), (0.3f,9.5f), (0.5f,12.5f), (1,18.75f), (2,26), (5,37.5f), (10,48.75f), (20,61.25f), (80,100), (100,100), (1100,100) },     /* M AC  corrugation */
/* L=131 */ { (0,1.75f), (0.1f,1.75f), (0.2f,2.5f), (0.3f,4), (0.5f,6), (1,9), (2,14), (5,22), (10,30), (20,39), (50,55), (100,72.5f), (1100,72.5f) },           /* L AC  corrugation */

/* AC depression */
/* H=132 */ { (0,12), (0.1f,12), (0.2f,15), (0.3f,17.5f), (0.5f,21), (1,26), (2,32.5f), (5,42.5f), (10,52.5f), (20,62.5f), (50,80), (100,97.5f), (1100,97.5f) },     /* H AC  depression */
/* M=133 */ { (0,5), (0.1f,5), (0.2f,6.25f), (0.3f,7.5f), (0.5f,10), (1,15), (2,21.5f), (5,31.25f), (10,40), (20,50), (50,67.5f), (100,85), (1100,85) },       /* M AC  depression */
/* L=134 */ { (0,0), (0.1f,0), (0.2f,0.5f), (0.3f,1), (0.5f,2.5f), (1,6.5f), (2,11.5f), (5,20), (10,27.5f), (20,36), (50,49), (100,62), (1100,62) },          /* L AC  depression */

/* AC jet blast erosion */
/* H=135 */ { (0,0), (0.1f,0), (0.2f,0.5f), (0.3f,1.25f), (0.5f,3), (1,6), (2,9), (5,16), (10,25), (20,35), (50,40), (100,41.5f), (1100,41.5f) },     /* H AC  jet blast erosion */
/* M=136 */ { (0,0), (0.1f,0), (0.2f,0.5f), (0.3f,1.25f), (0.5f,3), (1,6), (2,9), (5,16), (10,25), (20,35), (50,40), (100,41.5f), (1100,41.5f) },     /* M AC  jet blast erosion */
/* L=137 */ { (0,0), (0.1f,0), (0.2f,0.5f), (0.3f,1.25f), (0.5f,3), (1,6), (2,9), (5,16), (10,25), (20,35), (50,40), (100,41.5f), (1100,41.5f) },     /* L AC  jet blast erosion */

/* AC joint reflection cracking */
#if ACCURATE_BUT_PROBABLY_NOT_IN_MICROPAVER
/* H=138 */ { (0,0), (0.1f,1), (0.2f,3.75f), (0.3f,   5), (0.5f,   8), (1,15.5f), (2,    25), (5,40), (10,   55), (20,68.75f), (50,   85), (100,93), (1100,93) },     /* H AC  joint refection cracking */
/* M=139 */ { (0,0), (0.1f,0), (0.2f,    1), (0.3f,2.5f), (0.5f,4.5f), (1,    9), (2,16.25f), (5,28), (10,37.5f), (20, 47.5f), (50,57.5f), (100,63), (1100,63) },     /* M AC  joint refection cracking */
/* L=140 */ { (0,0), (0.1f,0), (0.2f,    0), (0.3f,   0), (0.5f,   1), (1, 2.5f), (2,     6), (5,11), (10,   16), (20,21.25f), (50,   29), (100,34), (1100,34) },     /* L AC  joint refection cracking */
#else
/* H=138 */ { (0,0), (0.1f,1), (0.2f,3.75f), (0.3f,   5), (0.5f,   8), (1,15.5f), (2,    26), (5,42), (10,   58), (20,     72), (50,   90), (100,100), (1100,100) },   /* H AC  joint refection cracking */
/* M=139 */ { (0,0), (0.1f,0), (0.2f,    1), (0.3f,2.5f), (0.5f,4.5f), (1,    9), (2,16.25f), (5,29), (10,   39), (20,     50), (50,   61), (100, 68), (1100, 68) },   /* M AC  joint refection cracking */
/* L=140 */ { (0,0), (0.1f,0), (0.2f,    0), (0.3f,   0), (0.5f,   1), (1, 2.5f), (2,     6), (5,11), (10,   17), (20,     23), (50,   32), (100, 37), (1100, 37) },   /* L AC  joint refection cracking */
#endif

/* AC longitudinal and transverse cracking */
/* H=141 */ { (0,7), (0.1f,7), (0.2f,9), (0.3f,11), (0.5f,14), (1,20), (2,27.5f), (5,40), (10,54), (20,69), (50,87.5f), (100,97.5f), (1100,97.5f) },     /* H AC  longitudinal and transverse cracking */
/* M=142 */ { (0,3.75f), (0.1f,3.75f), (0.2f,5), (0.3f,6), (0.5f,7.5f), (1,11), (2,16), (5,25), (10,36), (20,50), (50,64), (100,72.5f), (1100,72.5f) },    /* M AC  longitudinal and transverse cracking */
/* L=143 */ { (0,2.5f), (0.1f,2.5f), (0.2f,2.5f), (0.3f,2.5f), (0.5f,3.5f), (1,5), (2,7.5f), (5,14), (10,23), (20,33), (50,44), (100,50), (1100,50) },    /* L AC  longitudinal and transverse cracking */

/* AC oil spillage */
/* H=144 */ { (0,2), (0.1f,2), (0.2f,2), (0.3f,2.5f), (0.5f,3), (1,4), (2,6.25f), (5,10), (10,16.5f), (20,24), (50,27.5f), (100,28), (1100,28) },     /* H AC oil spillage */
/* M=145 */ { (0,2), (0.1f,2), (0.2f,2), (0.3f,2.5f), (0.5f,3), (1,4), (2,6.25f), (5,10), (10,16.5f), (20,24), (50,27.5f), (100,28), (1100,28) },     /* M AC oil spillage */
/* L=146 */ { (0,2), (0.1f,2), (0.2f,2), (0.3f,2.5f), (0.5f,3), (1,4), (2,6.25f), (5,10), (10,16.5f), (20,24), (50,27.5f), (100,28), (1100,28) },     /* L AC oil spillage */

/* AC patching and utility cut */
/* H=147 */ { (0,15), (0.1f,15), (0.2f,16), (0.3f,16.25f), (0.5f,17), (1,19), (2,23), (5,32.5f), (10,42.5f), (20,56.25f), (50,80), (100,100), (1100,100) },     /* H AC patching and utility cut */
/* M=148 */ { (0,6.5f), (0.1f,6.5f), (0.2f,7), (0.3f,7.5f), (0.5f,8), (1,10), (2,12.5f), (5,20), (10,27.5f), (20,38), (50,55), (100,70), (1100,70) },          /* M AC patching and utility cut */
/* L=149 */ { (0,2), (0.1f,2), (0.2f,2), (0.3f,2.5f), (0.5f,3), (1,4), (2,6), (5,10), (10,15), (20,20.5f), (50,30), (100,39), (1100,39) },             /* L AC patching and utility cut */

/* AC polished aggregate */
/* H=150 */ { (0,1.25f), (0.1f,1.25f), (0.2f,1.25f), (0.3f,1.25f), (0.5f,1.5f), (1,3), (2,5.5f), (5,10.5f), (10,17), (20,25), (50,37.5f), (100,50), (1100,50) },     /* H AC polished aggregate */
/* M=151 */ { (0,1.25f), (0.1f,1.25f), (0.2f,1.25f), (0.3f,1.25f), (0.5f,1.5f), (1,3), (2,5.5f), (5,10.5f), (10,17), (20,25), (50,37.5f), (100,50), (1100,50) },     /* M AC polished aggregate */
/* L=152 */ { (0,1.25f), (0.1f,1.25f), (0.2f,1.25f), (0.3f,1.25f), (0.5f,1.5f), (1,3), (2,5.5f), (5,10.5f), (10,17), (20,25), (50,37.5f), (100,50), (1100,50) },     /* L AC polished aggregate */

/* AC raveling/weathering */
/* H=153 */ { (0,6.5f), (0.1f,6.5f), (0.2f,8), (0.3f,9), (0.5f,11.25f), (1,16.25f), (2,26.25f), (5,42.5f), (10,53), (20,60.5f), (50,68), (100,70), (1100,70) },     /* H AC raveling/weathering */
/* M=154 */ { (0,4.95f), (0.1f,4.95f), (0.2f,5), (0.3f,5.5f), (0.5f,6.5f), (1,8), (2,11), (5,16), (10,20), (20,28.5f), (50,44), (100,57), (1100,57) },       /* M AC raveling/weathering */
/* L=155 */ { (0,1.5f), (0.1f,1.5f), (0.2f,1.5f), (0.3f,1.75f), (0.5f,2), (1,3), (2,4.9f), (5,7), (10,10), (20,14.5f), (50,21), (100,27.5f), (1100,27.5f) },    /* L AC raveling/weathering */

/* AC rutting */
/* H=156 */ { (0,19.5f), (0.1f,19.5f), (0.2f,23.75f), (0.3f,26), (0.5f,29), (1,34.5f), (2,41), (5,51.5f), (10,62), (20,74), (60,100), (100,100), (1100,100) },     /* H AC rutting */
/* M=157 */ { (0,13), (0.1f,13), (0.2f,16), (0.3f,17.5f), (0.5f,20), (1,24.5f), (2,29), (5,37), (10,44), (20,52), (50,65), (100,79), (1100,79) },          /* M AC rutting */
/* L=158 */ { (0,8.75f), (0.1f,8.75f), (0.2f,10), (0.3f,11), (0.5f,13), (1,16), (2,18.75f), (5,24), (10,29), (20,35), (50,44), (100,53.75f), (1100,53.75f) },      /* L AC rutting */

/* AC shoving of flexible pavement by PCC slabs */
/* H=159 */ { (0,10), (0.1f,10), (0.2f,13), (0.3f,15.5f), (0.5f,19), (1,25), (2,31.25f), (5,41.5f), (10,50), (20,60), (50,75), (100,87.5f), (1100,87.5f) },        /* H AC shoving of flexible pavement by PCC slabs */
/* M=160 */ { (0,6.25f), (0.1f,6.25f), (0.2f,7.5f), (0.3f,8.75f), (0.5f,11.25f), (1,15), (2,20), (5,27.5f), (10,33.75f), (20,41), (50,52.5f), (100,64), (1100,64) },     /* M AC shoving of flexible pavement by PCC slabs */
/* L=161 */ { (0,2.5f), (0.1f,2.5f), (0.2f,3), (0.3f,3.75f), (0.5f,5), (1,7), (2,10), (5,14), (10,18.75f), (20,24), (50,32), (100,39), (1100,39) },           /* L AC shoving of flexible pavement by PCC slabs */

/* AC slippage cracking */
/* H=162 */ { (0,5), (0.1f,5), (0.2f,5.5f), (0.3f,6.5f), (0.5f,8), (1,12.5f), (2,20), (5,35), (10,52), (20,66), (50,76), (100,80), (1100,80) },     /* H AC slippage cracking */
/* M=163 */ { (0,5), (0.1f,5), (0.2f,5.5f), (0.3f,6.5f), (0.5f,8), (1,12.5f), (2,20), (5,35), (10,52), (20,66), (50,76), (100,80), (1100,80) },     /* M AC slippage cracking */
/* L=164 */ { (0,5), (0.1f,5), (0.2f,5.5f), (0.3f,6.5f), (0.5f,8), (1,12.5f), (2,20), (5,35), (10,52), (20,66), (50,76), (100,80), (1100,80) },     /* L AC slippage cracking */

/* AC swell */
/* H=165 */ { (0,28.5f), (0.1f,28.5f), (0.2f,29), (0.3f,30), (0.5f,31.25f), (1,33.75f), (2,37.5f), (5,47.5f), (10,57.5f), (20,68.75f), (80,100), (100,100), (1100,100) },     /* H AC swell */
/* M=166 */ { (0,10), (0.1f,10), (0.2f,11), (0.3f,11.5f), (0.5f,12.5f), (1,14.5f), (2,17.5f), (5,25.5f), (10,34), (20,45), (50,62), (100,75), (1100,75) },        /* M AC swell */
/* L=167 */ { (0,1.25f), (0.1f,1.25f), (0.2f,1.25f), (0.3f,1.5f), (0.5f,2), (1,3.75f), (2,6), (5,11), (10,17.5f), (20,25), (50,37), (100,46.5f), (1100,46.5f) },        /* L AC swell */

/* end faa82 ac airfield distreses. */
/*******************************************************************************************************************************************************************/
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */

   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
/*******************************************************************************************************************************************************************/
/* begin faa82 PCC airfield distreses. */

/*  PCC airport blowup */
/* H = 180*/ {(-1,0),(0,100),(5,100),(10,100),(15,100),(20,100),(25,100),(30,100),(40,100),(60,100),(80,100),(100,100),(1100,100) }, /* H PCC airport blowup */
/* M = 181*/ {(-1,0),(0,0),(2,20),(5,32),(8,40),(10,42.5f),(15,48),(63,100),(70,100),(75,100),(80,100),(100,100),(1100,100) },        /* M PCC airport blowup */
/* L = 182*/ {(-1,0),(0,0),(3,10),(5,13),(8.5f,20),(10,22),(15,27.5f),(20,33),(50,65),(84,100),(90,100),(100,100),(1100,100) },      /* L PCC airport blowup */
/*   PCC corner break */
/* H = 183*/ {(-1,0),(0,0),(5,12),(10,22.5f),(15,30),(20,36),(25,41),(30,45.5f),(40,54),(60,69),(80,82),(100,91),(1100,91) },         /* H PCC airport corner break */
/* M = 184*/ {(-1,0),(0,0),(5,8),(10,15),(15,20),(20,24.5f),(25,28),(30,32),(40,38),(60,50.5f),(80,62),(100,72.5f),(1100,72.5f) },    /* M PCC airport corner break */
/* L = 185*/ {(-1,0),(0,0),(5,4),(10,8),(15,11.5f),(20,15),(25,18),(30,21),(40,25),(60,32.5f),(80,39),(100,42),(1100,42) },         /* L PCC airport corner break */
/*   PCC longitudinal cracking */
/* H = 186*/ {(-10,0),(0,0),(5,15.5f),(10,25),(15,33),(20,39.5f),(25,45),(30,50),(40,57),(60,71),(80,79),(100,83.5f),(1100,83.5f) },          /* H PCC airport longitudinal cracking */
/* M = 187*/ {(-10,0),(0,0),(5,11.25f),(10,18.75f),(15,23.75f),(20,27.5f),(25,31.25f),(30,34.7f),(40,40),(60,48.75f),(80,54),(100,57.5f),(1100,57.5f) }, /* M PCC airport longitudinal cracking */
/* L = 188*/ {(-10,0),(0,0),(5,4.77f),(10,8),(15,11.25f),(20,13.75f),(25,15),(30,17),(40,18.75f),(60,21.25f),(80,22),(100,22),(1100,22) },       /* L PCC airport longitudinal cracking */

/*   PCC durability cracking */
/* H = 189*/ {(-10,0),(0,0),(5, 14.87f),(10, 25.33f),(15, 34.01f),(20, 41.65f),(25,48.44f),(30, 54.45f),(40, 64.44f),(60, 77.98f),(80, 85.14f),(100, 87.67f),(1100, 87.67f) },  /* H PCC airport durability cracking */
/* M = 190*/ {(-10,0),(0,0),(5,  6.73f),(10, 12.25f),(15, 17.34f),(20, 21.8f),(25,25.71f),(30,29.15f),(40, 34.96f),(60, 43.63f),(80,49.83f),(100,  54.48f),(1100, 54.48f) },  /* M PCC airport durability cracking */
/* L = 191*/ {(-10,0),(0,0),(5,  1.64f),(10, 3.27f),(15,   5.09f),(20,  6.77f),(25, 8.27f),(30, 9.6f),(40, 11.84f),(60, 15.13f),(80, 17.38f),(100, 18.23f),(1100, 18.23f) },  /* L PCC airport durability cracking */
/*   PCC joint seal damage */
/* H = 192*/ {(-10,0),(0,0),(5,12),(10,12),(15,12),(20,12),(25,12),(30,12),(40,12),(60,12),(80,12),(100,12),(1100,12) }, /* H PCC airport small patch */
/* M = 193*/ {(-10,0),(0,0),(5, 7),(10, 7),(15, 7),(20, 7),(25, 7),(30, 7),(40, 7),(60, 7),(80, 7),(100, 7),(1100, 7) }, /* M PCC airport small patch */
/* L = 194*/ {(-10,0),(0,0),(5, 2),(10, 2),(15, 2),(20, 2),(25, 2),(30, 2),(40, 2),(60, 2),(80, 2),(100, 2),(1100, 2) }, /* L PCC airport small patch */
/*   PCC small patch */
/* H = 195*/ {(-10,0),(0,0),(5,6.9f),(10,11.25f),(15,14),(20,17.5f),(25,20),(30,22),(40,26.75f),(60,33),(80,38),(100,41),(1100,41) },     /* H PCC airport small patch */
/* M = 196*/ {(-10,0),(0,0),(5,3),(10,6),(15,7.8f),(20,10),(25,11.25f),(30,12.5f),(40,15),(60,19),(80,21.25f),(100,22),(1100,22) },     /* M PCC airport small patch */
/* L = 197*/ {(-10,0),(0,0),(5,1.25f),(10,1.75f),(15,2),(20,2.5f),(25,3.75f),(30,4),(40,6.75f),(60,8.5f),(80,9.9f),(100,10),(1100,10) },  /* L PCC airport small patch */
/*   PCC patch cut defect  */
/* H = 198*/ {(-10,0),(0,0),(5,18.75f),(10,28.75f),(15,36),(20,42.5f),(25,47.5f),(30,52),(40,60),(60,72.5f),(80,82.5f),(100,87.7f),(1100,87.7f) },  /* H PCC airport patch cut defect */
/* M = 199*/ {(-10,0),(0,0),(5,11.25f),(10,17),(15,20.5f),(20,23.75f),(25,26.25f),(30,29),(40,33.75f),(60,41.25f),(80,46.25f),(100,49),(1100,49) }, /* M PCC airport patch cut defect  */
/* L = 200*/ {(-10,0),(0,0),(5,3.75f),(10,6.25f),(15,8.75f),(20,10.5f),(25,12),(30,13.5f),(40,16),(60,19.5f),(80,21.25f),(100,22),(1100,22) },       /* L PCC airport patch cut defect  */
/*   PCC popouts  */
/* H = 201*/ {(-10,0),(0,0),(5,4),(10,7.5f),(15,10),(20,12),(25,13.75f),(30,15),(40,17.5f),(60,21.25f),(80,22),(100,22),(1100,22) }, /* H PCC airport popouts */
/* M = 202*/ {(-10,0),(0,0),(5,4),(10,7.5f),(15,10),(20,12),(25,13.75f),(30,15),(40,17.5f),(60,21.25f),(80,22),(100,22),(1100,22) }, /* M PCC airport popouts */
/* L = 203*/ {(-10,0),(0,0),(5,4),(10,7.5f),(15,10),(20,12),(25,13.75f),(30,15),(40,17.5f),(60,21.25f),(80,22),(100,22),(1100,22) }, /* L PCC airport popouts */
/*   PCC pumping  */
/* H = 204*/ {(-10,0),(0,0),(5,5),(10,10),(15,14),(20,18.75f),(25,22.5f),(30,26),(40,32.5f),(60,43),(80,50),(100,52),(1100,52) }, /* H PCC airport pumping */
/* M = 205*/ {(-10,0),(0,0),(5,5),(10,10),(15,14),(20,18.75f),(25,22.5f),(30,26),(40,32.5f),(60,43),(80,50),(100,52),(1100,52) }, /* M PCC airport pumping */
/* L = 206*/ {(-10,0),(0,0),(5,5),(10,10),(15,14),(20,18.75f),(25,22.5f),(30,26),(40,32.5f),(60,43),(80,50),(100,52),(1100,52) }, /* L PCC airport pumping */
/*   PCC scaling  */
/* H = 207*/ {(-10,0),(0,0),(5,15),(10,26),(15,34),(20,42),(25,48),(30,54),(40,65),(60,77.5f),(80,84),(100,87),(1100,87) },           /* H PCC airport scaling */
/* M = 208*/ {(-10,0),(0,0),(5,7.5f),(10,13),(15,17.5f),(20,22),(25,25),(30,28.75f),(40,34.9f),(60,43.75f),(80,50.5f),(100,52.5f),(1100,52.5f) }, /* M PCC airport scaling */
/* L = 209*/ {(-10,0),(0,0),(5,2.5f),(10,4),(15,6),(20,7.5f),(25,8),(30,9),(40,11.25f),(60,13.75f),(80,15),(100,16.5f),(1100,16.5f) },        /* L PCC airport scalingg */
/*   PCC settlement  */
/* H = 210*/ {(-10,0),(0,0),(5,15),(10,26),(15,33),(20,39),(25,44.9f),(30,49.5f),(40,57.5f),(60,71),(80,81.25f),(100,90),(1100,90) },          /* H PCC airport settlement */
/* M = 211*/ {(-10,0),(0,0),(5,8.75f),(10,14),(15,19),(20,23),(25,26.25f),(30,30),(40,35),(60,44.9f),(80,51.25f),(100,56.5f),(1100,56.5f) },   /* M PCC airport settlement */
/* L = 212*/ {(-10,0),(0,0),(5,4.9f),(10,8.75f),(15,11.5f),(20,14),(25,17),(30,19.9f),(40,23.75f),(60,30),(80,34),(100,37.5f),(1100,37.5f) },  /* L PCC airport settlement */
/*   PCC shattered slab  */
/* H = 213*/ {(-10,0),(0,0),(5,30),(10,40),(15,46),(20,51.25f),(25,56.25f),(30,61),(40,70),(60,83.5f),(90,100),(100,100),(1100,100) },        /* H PCC airport shattered slab */
/* M = 214*/ {(-10,0),(0,0),(5,20),(10,27.5f),(15,33),(20,38),(25,42.5f),(30,47),(40,54),(60,66),(80,75),(100,83),(1100,83) },           /* M PCC airport shattered slab */
/* L = 215*/ {(-10,0),(0,0),(5,10),(10,17.5f),(15,22.5f),(20,26.25f),(25,30),(30,32.5f),(40,37.5f),(60,46),(80,52.5f),(100,57.5f),(1100,57.5f) }, /* L PCC airport shattered slab */
/*   PCC shrinkage cracks  */
/* H = 216*/ {(-10,0),(0,0),(5,1.25f),(10,1.75f),(15,2.5f),(20,3),(25,3.75f),(30,4),(40,6),(60,9),(80,12),(100,13.75f),(1100,13.75f) }, /* H PCC airport shrinkage cracks */
/* M = 217*/ {(-10,0),(0,0),(5,1.25f),(10,1.75f),(15,2.5f),(20,3),(25,3.75f),(30,4),(40,6),(60,9),(80,12),(100,13.75f),(1100,13.75f) }, /* M PCC airport shrinkage cracks */
/* L = 218*/ {(-10,0),(0,0),(5,1.25f),(10,1.75f),(15,2.5f),(20,3),(25,3.75f),(30,4),(40,6),(60,9),(80,12),(100,13.75f),(1100,13.75f) }, /* L PCC airport shrinkage cracks */
/*   PCC spalling along the joints  */
/* H = 219*/ {(-10,0),(0,0),(5,12.5f),(10,21),(15,25),(20,29),(25,32),(30,34),(40,38.75f),(60,45),(80,49),(100,51.25f),(1100,51.25f) },     /* H PCC airport spalling along the joints */
/* M = 220*/ {(-10,0),(0,0),(5,4),(10,7.5f),(15,11),(20,13.75f),(25,16),(30,18.75f),(40,22.5f),(60,30),(80,34),(100,36),(1100,36) },       /* M PCC airport spalling along the joints */
/* L = 221*/ {(-10,0),(0,0),(5,2),(10,3.5f),(15,4.9f),(20,6),(25,7),(30,7.5f),(40,10),(60,12.5f),(80,13.75f),(100,13.76f),(1100,13.76f) },  /* L PCC airport spalling along the joints */
/*   PCC spalling corner  */
/* H = 222*/ {(-10,0),(0,0),(5,4.83f),(10,9.5f),(15,12.5f),(20,16.25f),(25,19.9f),(30,22.5f),(40,27.5f),(60,36),(80,42.5f),(100,46),(1100,46) }, /* H PCC airport spalling corner */
/* M = 223*/ {(-10,0),(0,0),(5,4),(10,7.5f),(15,10   ),(20,12    ),(25,14   ),(30,16),(40,18.75f),(60,22.5f),(80,26),(100,28),(1100,28) },          /* M PCC airport spalling corner */
/* L = 224*/ {(-10,0),(0,0),(5,1.83f),(10,4   ),(15,5.5f ),(20,7.5f  ),(25,8.75f),(30,10),(40,12),(60,15),(80,17.5f),(100,19),(1100,19) },         /* L PCC airport spalling corner */

/* end faa82 ac airfield distreses. */
/*******************************************************************************************************************************************************************/

/********** begin wci distresses *********************/

/*    AC Linear Cracking     */
/* H = 225*/ {(0,18.19f),(2,26.48f),(4,38.69f),(6,48.18f),(12,67.36f),(16,75.13f),(18,77.96f),(20,80.21f),(22,81.95f),(24,83.25f),(26,84.15f),(100,84.91f),(1000,100) },
/* M = 226*/ {(0,8.68f),(2,13.80f),(4,19.87f),(6,24.28f),(8,27.90f),(10,30.97f),(12,33.57f),(16,37.63f),(20,40.45f),(26,42.83f),(32,43.46f),(100,43.46f),(1000,100) },
/* L = 227*/ {(0,1.72f),(4,8.86f),(8,15.09f),(10,17.22f),(12,18.91f),(16,21.25f),(18,22.02f),(20,22.58f),(22,22.97f),(24,23.19f),(26,23.29f),(100,23.29f),(1000,100) },

/*    AC Pattern Cracking   */
/* H = 228*/ {(0,18.18f),(10,52.67f),(30,69.18f),(45,74.41f),(60,78.02f),(65,79.03f),(70,79.98f),(75,80.88f),(80,81.74f),(85,82.57f),(95,84.14f),(100,84.89f),(1000,100) },
/* M = 229*/ {(0,11.32f),(10,35.26f),(20,43.70f),(35,50.55f),(40,52.19f),(45,53.64f),(50,54.95f),(60,57.23f),(70,59.18f),(80,60.89f),(90,62.42f),(100,63.80f),(1000,100) },
/* L = 230*/ {(0,4.77f),(15,26.84f),(30,34.03f),(45,38.65f),(60,41.63f),(65,42.73f),(70,43.42f),(75,43.24f),(80,45.01f),(85,45.74f),(95,47.11f),(100,47.75f),(1000,100) },

/*    AC Surface Deterioration    */
/* H = 231*/ {(0,7.1259f),(1,36.553f),(5,64.135f),(7,71.053f),(10,75.78f),(17,82.44f),(25,85.873f),(40,88.9f),(45,89.29f),(50,89.56f),(70,89.99f),(100,90.07f),(1000,100) },
/* M = 232*/ {(0,1.6376f),(1,21.943f),(5,46.91f),(10,61.13f),(15,68.787f),(35,75.414f),(50,79.0f),(65,78.75f),(80,78.98f),(85,79.05f),(90,79.13f),(100,79.35f),(1000,100) },
/* L = 233*/ {(0,7.1259f),(5,27.57f),(10,36.78f),(15,42.59f),(30,52.69f),(45,58.24f),(55,60.74f),(65,62.63f),(75,64.08f),(80,64.68f),(90,65.65f),(100,66.39f),(1000,100) },

/*    AC Surface Distortion    */
/* H = 234*/ {(0,26.45f),(4,42.39f),(6,48.97f),(10,58.12f),(14,64.29f),(20,70.57f),(30,76.94f),(40,80.73f),(50,83.17f),(70,86.03f),(90,87.60f),(100,88.16f),(1000,100) },
/* M = 235*/ {(0,14.33f),(4,27.50f),(6,32.63f),(10,39.84f),(14,44.86f),(20,50.22f),(30,56.05f),(40,59.79f),(50,62.36f),(70,65.51f),(90,67.16f),(100,67.65f),(1000,100) },
/* L = 236*/ {(0,4.40f),(4,12.34f),(6,15.50f),(10,20.11f),(14,23.50f),(20,27.36f),(30,31.87f),(40,34.95f),(50,37.14f),(70,39.76f),(80,40.47f),(100,41.04f),(1000,100) },

/*    AC Surface Defects    */
/* H = 237*/ {(0,10.51f),(4,19.55f),(6,24.39f),(10,32.00f),(14,37.75f),(20,44.29f),(30,52.01f),(40,57.54f),(50,61.81f),(70,68.23f),(90,73.10f),(100,75.20f),(1000,100) },
/* M = 238*/ {(0,5.53f),(4,9.85f),(6,12.06f),(10,15.51f),(14,18.17f),(20,21.32f),(30,25.34f),(40,28.54f),(50,31.28f),(70,35.98f),(90,40.10f),(100,42.03f),(1000,100) },
/* L = 239*/ {(0,0.55f),(4,1.54f),(6,2.19f),(10,3.41f),(14,4.54f),(20,6.09f),(30,8.36f),(40,10.35f),(50,12.13f),(70,15.22f),(90,17.86f),(100,19.05f),(1000,100) },

/*    PCC Linear Cracking    */
/* H = 240*/ {(0,2.22f),(4,11.17f),(6,14.74f),(10,20.47f),(14,24.84f),(20,29.76f),(30,35.83f),(40,40.91f),(50,45.54f),(70,53.49f),(90,61.30f),(100,64.39f),(1000,100) },
/* M = 241*/ {(0,0.04f),(4,4.03f),(6,5.78f),(10,8.89f),(14,11.59f),(20,15.05f),(30,19.79f),(40,23.72f),(50,27.09f),(70,32.47f),(90,36.37f),(100,37.50f),(1000,100) },
/* L = 242*/ {(0,0.46f),(4,1.40f),(6,1.90f),(10,2.91f),(14,3.93f),(20,5.43f),(30,7.76f),(40,9.75f),(50,11.38f),(70,13.86f),(90,16.56f),(100,18.74f),(1000,100) },

/*    PCC Pattern Cracking    */
/* H = 243*/ {(0,5.28f),(1,18.33f),(4,37.35f),(10,52.59f),(20,63.81f),(30,68.93f),(40,72.59f),(50,75.8f),(60,78.32f),(70,80.09f),(90,83.42f),(100,84.45f),(1000,100) },
/* M = 244*/ {(0,1.56f),(1,11.54f),(4,24.1f),(10,35.39f),(20,43.71f),(30,48.49f),(40,52.13f),(50,55.06f),(60,57.28f),(70,59.15f),(90,62.46f),(100,63.64f),(1000,100) },
/* L = 245*/ {(0,0.65f),(4,14.14f),(10,23.01f),(20,30.10f),(30,33.48f),(40,37.04f),(50,39.59f),(60,41.81f),(70,43.56f),(80,44.99f),(90,46.37f),(100,47.66f),(1000,100) },

/*    PCC Surface Deterioration    */
/* H = 246*/ {(0,0.76f),(4,5.34f),(10,11.34f),(20,19.31f),(30,25.30f),(40,29.94f),(50,33.71f),(60,36.92f),(70,39.62f),(80,41.75f),(90,43.21f),(100,44.04f),(1000,100) },
/* M = 247*/ {(0,0.26f),(6,1.71f),(10,3.16f),(20,7.63f),(30,12.31f),(40,16.36f),(50,19.46f),(60,21.63f),(70,23.11f),(80,24.16f),(90,24.75f),(100,24.75f),(1000,100) },
/* L = 248*/ {(0,0.37f),(6,0.54f),(10,1.11f),(20,3.41f),(30,6.10f),(40,8.58f),(50,10.63f),(60,12.29f),(70,13.74f),(80,15.19f),(90,16.73f),(100,18.24f),(1000,100) },

/*    PCC Surface Defects    */
/* H = 249*/ {(0,0),(6,0.88f),(10,1.52f),(20,3.12f),(30,4.62f),(40,6.00f),(50,7.25f),(60,8.28f),(70,8.91f),(80,9.04f),(90,9.04f),(100,9.04f),(1000,100) },
/* M = 250*/ {(0,0),(6,0.88f),(10,1.52f),(20,3.12f),(30,4.62f),(40,6.00f),(50,7.25f),(60,8.28f),(70,8.91f),(80,9.04f),(90,9.04f),(100,9.04f),(1000,100) },
/* L = 251*/ {(0,0),(6,0.88f),(10,1.52f),(20,3.12f),(30,4.62f),(40,6.00f),(50,7.25f),(60,8.28f),(70,8.91f),(80,9.04f),(90,9.04f),(100,9.04f),(1000,100) },

/********** end wci distresses *********************/

/********** start ASTM D5340-10 distressses Apr. 6, 2010
/* PCC scaling */
/* H = 252*/ { (0, 0), (5, 9), (10, 30), (20, 42.5f), (30, 51.8f), (40, 59), (50, 65), (60, 69.5f), (70, 72.5f), (80, 76), (90, 78), (100, 81), (1100, 100) },     /* */
/* M = 253*/ { (0, 0), (5, 4.5f), (10, 9), (20, 17.0f), (30, 24), (40, 28), (50, 31.5f), (60, 33), (70, 34.5f), (80, 36), (90, 37.5f), (100, 39.5f), (1100, 45) },     /* */
/* L = 254*/ { (-10, 0), (5, 1), (10, 1.5f), (20, 2), (30, 2.5f), (40, 3), (50, 3.5f), (60, 4), (70, 5), (80, 5.5f), (90, 6), (100, 6.5f), (1100, 100) },     /* */
/* PCC Alkali Silica Reaction (ASR) */
/* H = 255*/ { (0, 0), (5, 22.0f), (7, 30.0f), (10, 38.0f), (20, 51.0f), (30, 59.0f), (40, 67.0f), (50, 72.0f), (60, 76.0f), (70, 79.5f), (80, 81.0f), (100, 82.0f), (1100, 85.0f) },     /* */
/* M = 256*/ { (0, 0), (5, 15), (10, 24), (20, 32), (30, 37), (40, 40), (50, 42), (60, 45), (70, 47), (80, 48.5f), (90, 49), (100, 50), (1100, 50) },     /* */
/* L = 257*/ { (0, 0), (5, 5), (10, 8), (20, 12), (30, 14), (40, 17), (50, 18), (60, 19.5f), (70, 20.5f), (80, 21), (90, 22), (100, 22), (1100, 22) },     /* */
/* AC weathering */
/* H = 258*/ { (0, 2), (1, 8), (5, 17.5f), (10, 22), (20, 29), (30, 35), (40, 39), (50, 42), (60, 47), (70, 49), (80, 52), (100, 57), (1100, 70) },     /* */
/* M = 259*/{ (0, 1), (1, 1), (10, 5), (20, 9), (30, 12), (40, 15), (50, 16.5f), (60, 17), (70, 18), (80, 18.7f), (90, 19.8f), (100, 20.5f), (1100, 30) },     /* */
/* L = 260*/ { (-1, 0), (0, 0), (10, 1.5f), (20, 2), (30, 2.4f), (40, 3), (50, 3.5f), (60, 4), (70, 4.5f), (80, 5), (90, 6), (100, 7), (1100, 8) },     /* */
/*  PCC D-Cracking */
/* H = 261*/ {(-10,0),(0,0),(5, 20.0f),(10, 29.0f),(15, 35.5f),(20, 42.0f),(25, 47.0f),(30, 51.2f),(40, 60.0f),(60, 73.0f),(80, 82.0f),(100, 88.0f),(1100, 87.67f) },  /* H PCC airport durability cracking */
/* M = 262*/ {(-10,0),(0,0),(5, 10.5f),(10, 16.20f),(15, 20.0f),(20, 24.0f),(25, 26.0f),(30,29.5f),(40, 34.00f),(60, 41.0f),(80,46.0f),(100,  49.0f),(1100, 54.48f) },  /* M PCC airport durability cracking */
/* L = 263*/ {(-10,0),(0,0),(5, 4.0f),(10, 5.5f),(15, 9.0f),(20,  10.0f),(25, 12.0f),(30, 14.0f),(40, 16.0f),(60, 20.00f),(80, 21.0f),(100, 22.0f),(1100, 18.23f) },  /* L PCC airport durability cracking */

/********** end ASTM D5340-10

/* 264 */   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */

/*270*/ { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */

/*280*/ { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */

/*290*/ { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) },     /* */
   { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (1100, 100) }      /* */
        };

        private static (int x, int y)[,] Q_d { get; } = new (int, int)[NUMQ, NUMPTS]
        {
/*  0 = MPACRDQ1   */ { (-10, 0), (0, 0), (10, 10), (20, 20), (30, 30), (40, 40), (50, 50), (60, 60), (70, 70), (80, 80), (90, 90), (100, 100), (11000, 100) },
/*  1 = MPACRDQ2   */ { (-10, 0), (0, 0), (13,  9), (40, 30), (60, 44), (80, 57), (100, 70), ( 120,  82), ( 150,  94), ( 170, 100), (1000, 100), (1000, 100), (10000, 100) },
/*  2 = MPACRDQ3   */ { (-10,  0), (0,  0), (19,   9), (40,  25), (60,  38), (80,  52), (100,  63), ( 120,   74), ( 150,   87), ( 170,   95), ( 183,  100), (1000,  100), (10000,  100) },
/*  3 = MPACRDQ4   */ { (-10,  0), (0,  0), (25,   9), (100, 57), (120, 68), (150, 83), (170,  90), ( 185,   95), ( 200,   97), ( 220,  100), (1000,  100), (1000,  100), (10000,  100) },
/*  4 = MPACRDQ5   */ { (-10,  0), (0,  0), (28,   9), (100, 52), (150, 77), (170, 85), (190,  91), ( 200,   93), ( 240,  100), (1000,  100), (1000,  100), (1000,  100), (10000,  100) },
/*  5 = MPACRDQ6   */ { (-10,  0), (0,  0), (42,  16), (120, 58), (160, 77), (180, 84), (200,  89), ( 260,  100), (1000,  100), (1000,  100), (1000,  100), (1000,  100), (10000,  100) },
/*  6 = MPACRDQ7   */ { (-10,  0), (0,  0), (42,  16), (120, 58), (160, 75), (180, 79), (200,  82), ( 280,  100), (1000,  100), (1000,  100), (1000,  100), (1000,  100), (10000,  100) },

/*  7 = FAA82PCCQ1 */ { (-10,  0), (0,  0), (10,  10), (20,  20), ( 30,  30), ( 40,   40), ( 50,   50), ( 60,  60), ( 70,   70), ( 80,   80), ( 90,   90), (100,  100), (11000,  100) },
/*  8 = FAA82PCCQ2 */ { (-10,  0), (2,  0), (60,  51), (90,  77), (110,  92), (127,  100), (148,  100), (170, 100), (180,  100), (200,  100), (210,  100), (300,  100), (11000,  100) },
/*  9 = FAA82PCCQ3 */ { (-10,  0), (2,  0), (60,  46), (90,  68), (110,  83), (127,   92), (148,  100), (170, 100), (180,  100), (200,  100), (210,  100), (300,  100), (11000,  100) },
/* 10 = FAA82PCCQ4 */ { (-10,  0), (2,  0), (60,  43), (90,  63), (110,  74), (127,   83), (148,   91), (170,  99), (180,  100), (200,  100), (210,  100), (300,  100), (11000,  100) },
/* 11 = FAA82PCCQ5 */ { (-10,  0), (2,  0), (60,  41), (90,  61), (110,  72), (127,   80), (148,   89), (170,  97), (180,   99), (200,  100), (210,  100), (300,  100), (11000,  100) },
/* 12 = FAA82PCCQ6 */ { (-10,  0), (2,  0), (60,  40), (90,  59), (110,  70), (127,   78), (148,   87), (170,  95), (180,   96), (200,  100), (210,  100), (300,  100), (11000,  100) },
/* 13 = FAA82PCCQ7 */ { (-10,  0), (2,  0), (60,  38), (90,  57), (110,  68), (127,   75), (148,   84), (170,  92), (180,   93), (200,   98), (210,  100), (300,  100), (11000,  100) },
/* 14 = FAA82PCCQ8 */ { (-10,  0), (2,  0), (60,  37), (90,  55), (110,  65), (127,   73), (148,   81), (170,  88), (180,   91), (200,   96), (210,  100), (300,  100), (11000,  100) },

/* 15 = FAA82ACQ1  */ { (-10,  0), (0,  0), (10,  10), (20,  20), (30,  30), (40,  40), (50,  50), (60,  60), ( 70,  70), ( 80,  80), ( 90,   90), (100,  100), (11000,  100) },
/* 16 = FAA82ACQ2  */ { (-10,  0), (0,  0), (10,   4), (20,  12), (30,  18), (40,  26), (70,  48), (85,  60), (100,  69), (120,  83), (157,  100), (180,  100), (11000,  100) },
/* 17 = FAA82ACQ3  */ { (-10,  0), (0,  0), (10,   0), (20,   7), (30,  14), (40,  22), (70,  44), (85,  55), (100,  63), (120,  74), (157,   89), (180,  100), (11000,  100) },
/* 18 = FAA82ACQ4  */ { (-10,  0), (0,  0), (10,   0), (20,   3), (30,   9), (40,  17), (70,  36), (85,  45), (100,  54), (120,  63), (157,   78), (205,  100), (11000,  100) },
/* 19 = FAA82ACQ5  */ { (-10,  0), (0,  0), (10,   0), (20,   3), (30,   9), (40,  17), (70,  36), (85,  45), (100,  54), (120,  61), (157,   75), (215,  100), (11000,  100) },
/* 20 = FAA82ACQ6  */ { (-10,  0), (0,  0), (10,   0), (20,   3), (30,   9), (40,  17), (70,  36), (85,  45), (100,  53), (120,  60), (157,   73), (225,  100), (11000,  100) },

/* Jan. 19. 2007. Gregg. Changed the one column in the following curve table, 130 used to be 110.
/* 21 = MPPCCRDQ1  */ { (-10,  0), (0,  0), (10,  10), (20,  20), (40,  40), (60,  60), (100,  100), (130,  100), (163,  100), (180,  100), (220,  100), (260,  100), (11000,  100) },
/* 22 = MPPCCRDQ2  */ { (-10,  0), (0,  0), (10,   7), (20,  15), (40,  32), (60,  45), (100,   70), (130,   86), (163,  100), (180,  100), (220,  100), (260,  100), (11000,  100) },
/* 23 = MPPCCRDQ3  */ { (-10,  0), (0,  0), (10,   5), (20,  12), (40,  25), (60,  38), (100,   63), (130,   78), (163,   93), (180,  100), (220,  100), (260,  100), (11000,  100) },
/* 24 = MPPCCRDQ4  */ { (-10,  0), (0,  0), (10,   4), (20,  10), (40,  23), (60,  35), (100,   57), (130,   72), (163,   87), (200,  100), (220,  100), (260,  100), (11000,  100) },
/* 25 = MPPCCRDQ5  */ { (-10,  0), (0,  0), (10,   0), (20,   7), (40,  21), (60,  33), (100,   54), (130,   69), (163,   83), (200,   97), (220,  100), (260,  100), (11000,  100) },
/* 26 = MPPCCRDQ6  */ { (-10,  0), (0,  0), (10,   0), (20,   4), (40,  18), (60,  30), (100,   51), (130,   66), (163,   79), (200,   92), (230,  100), (260,  100), (11000,  100) },
/* 27 = MPPCCRDQ7  */ { (-10,  0), (0,  0), (10,   0), (20,   0), (40,  17), (60,  29), (100,   49), (130,   63), (163,   76), (200,   90), (230,   97), (260,  100), (11000,  100) },
/* 28 = MPPCCRDQ8  */ { (-10,  0), (0,  0), (10,   0), (20,   0), (40,  16), (60,  28), (100,   47), (130,   61), (163,   74), (200,   88), (230,   96), (270,  100), (11000,  100) },
/* 29 = MPPCCRDQ9  */ { (-10,  0), (0,  0), (10,   0), (20,   0), (40,  15), (60,  26), (100,   45), (130,   58), (163,   72), (200,   84), (230,   94), (280,  100), (11000,  100) },

/* 30*/ 	      { (-10,  0), (0,  0), (10,  10), (20,  20), (30,  30), (40,  40), (50,  50), (60,  60), (70,  70), (80,  80), (90,  90), (100,  100), (11000,  100) },
/* 31*/ 	      { (-10,  0), (0,  0), (10,  10), (20,  20), (30,  30), (40,  40), (50,  50), (60,  60), (70,  70), (80,  80), (90,  90), (100,  100), (11000,  100) },
/* 32*/ 	      { (-10,  0), (0,  0), (10,  10), (20,  20), (30,  30), (40,  40), (50,  50), (60,  60), (70,  70), (80,  80), (90,  90), (100,  100), (11000,  100) },
/* 33*/ 	      { (-10,  0), (0,  0), (10,  10), (20,  20), (30,  30), (40,  40), (50,  50), (60,  60), (70,  70), (80,  80), (90,  90), (100,  100), (11000,  100) },
/* 34*/ 	      { (-10,  0), (0,  0), (10,  10), (20,  20), (30,  30), (40,  40), (50,  50), (60,  60), (70,  70), (80,  80), (90,  90), (100,  100), (11000,  100) },
/* 35*/ 	      { (-10,  0), (0,  0), (10,  10), (20,  20), (30,  30), (40,  40), (50,  50), (60,  60), (70,  70), (80,  80), (90,  90), (100,  100), (11000,  100) },
/* 36*/ 	      { (-10,  0), (0,  0), (10,  10), (20,  20), (30,  30), (40,  40), (50,  50), (60,  60), (70,  70), (80,  80), (90,  90), (100,  100), (11000,  100) },
/* 37*/ 	      { (-10,  0), (0,  0), (10,  10), (20,  20), (30,  30), (40,  40), (50,  50), (60,  60), (70,  70), (80,  80), (90,  90), (100,  100), (11000,  100) },
/* 38*/ 	      { (-10,  0), (0,  0), (10,  10), (20,  20), (30,  30), (40,  40), (50,  50), (60,  60), (70,  70), (80,  80), (90,  90), (100,  100), (11000,  100) },
/* 39*/ 	      { (-10,  0), (0,  0), (10,  10), (20,  20), (30,  30), (40,  40), (50,  50), (60,  60), (70,  70), (80,  80), (90,  90), (100,  100), (11000,  100) }
        };

        #endregion Data
    }
}
