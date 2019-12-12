// PCI.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;


namespace LegacyPCI {

	public ref class Distress
	{
	public:
	
		static double ComputePCIValue( System::String^ sDeductValues, System::String^ sMethodology);
		static double pvt_ComputePCIDeduct(int nDistress,System::String^ sSeverity, double dAmount, double dSamsiz);
		static int pvt_nSevFromSev(System::String^ sSev);
		static double pciCorrectedDeductValue(System::String^ sMethod, System::String^ sDeduct,double dLargeDeductLimit);
		static double pciPrivateLargeDeductCorrection(System::String^ sMethod, int nNumLargeDeducts, double nTotalDeduct);
		static void   SortNumericDescending(double * pArray, int nLength);
		static  int	GetLengthNonNegativeArray(double * pArray);
		static double SafePercentShare(double dShare, double dTotal);
	    static bool IsWASHCLKMethod(System::String^ s);
		static double pvt_ComputeNonPCIDeduct(	System::String^ sMethod, int nDistress, System::String^ sSeverity, System::String^ sExtent);
		static double pvt_ComputeNonPCIDeduct(	System::String^ sMethod, int nDistress, System::String^ sSeverity, double dExtent);

		static double pvt_ComputeWASHCLKDeduct(int nDistress, System::String^ sSeverity, System::String^ sExtent);	

		static int pvt_SeverityExtent2Deduct(System::String^ sRawSeverity, System::String^ sRawExtent, int n1, int n2, int n3, int n4, int n5, int n6, int n7, int n8, int n9);
		static int TotalDeducts(System::String ^ sDeductValues);
//
// Routines added to get various values from the PCI module so that the PCI
// module is the one source for such information.  LRR - 11/19/2008
//
		static array<System::String^, 1>^ GetMethodologies();
		static array<System::String^, 1>^ GetMPACDistressNames();

// PCI routines used to calculate Q values and deduct values.  CalcWCI is for the
// WCI procedure only.
//
		static  float Q(int i, float x);
		static  float  DeductEval(int, int, float);
		static double CalcWCI_CDV(int iQuantity, double fDeductValue);
		static int    dis2row(int, int);


//		static float DeductEval(int nDistress, double dSeverity, double dPercent);
//		static double pvt_ComputePCIDeduct(int nDistress, System::String^ sSeverity, double dAmount, double dSamsiz);
//		static  bool IsWNDMethod(System::String^ s);
//		static bool IsWASHCLKMethod(System::String^ s);
//		static double pvt_ComputeNonPCIDeduct(System::String^ sMethod, int nDistress, CString sSeverity, CString sExtent);
//		static double pvt_ComputeWNDDeduct(int nDistress, System::String^ sSeverity, System::String^ sExtent);
//		static double pvt_DeductEvalExtent(int nDistress, int nSeverity, System::String^ sExtent);
//		static double pvt_FlatDeduct(double dMultiplier, int nSeverity, System::String^ sRawExtent);
//		static double pvt_ComputeWASHCLKDeduct(int nDistress, System::String^ sSeverity, System::String^ sExtent);
//		static int pvt_SeverityExtent2Deduct(System::String^ sRawSeverity, System::String^ sRawExtent, int n1, int n2, int n3, int n4, int n5, 
//								 int n6, int n7, int n8, int n9);
//		static double ComputeNonPCIQuantity(System::String^ sRawExtent, double dArea);
//		static double Estimate(	double nTot, double nRan, double nAdd, double nMeanRan, double nMeanAdd);
//		static  bool HasSampleUnits(System::String^ sTestMethod);
//		static void ComputeVCIValues(double dLoadDeducts, double dClimateDeducts, double dOtherDeducts, double dTotalDeduct,
//							 System::String^ sDeductValues,
//							 bool bIsPCIMethod, System::String^ sMethodology, double * pTotalDeductLoad, 
//							 double * pdTotalDeductClimate, double * pdTotalDeductOther, double * pVCI);
//		static double CalcPCIDistressDeductValue(int dDisNum, System::String^ sSev, double dPCIArea, double dSampleArea);




//		static double ConvertUnit(int nDestUnit, int nUnitType, double dOldValue);


	};

//
// Constants
//
#define NUMPTS  13  /* first&last pts must be out of range sentinels */
#define NUMQ    40  /* Any change will effect the initializers, */
#define SALDISTRESSRANGE 200  /* Any change will effect the initializers, */
#define QROWS	8		// Selected by the number of large deducts (deducts > 5)
#define CVALUES 7		// values for c1 through C6 in the formula
#define NUMDISTRESS       200  /* must agree with # in dis2row  */
#define NUMCURVES       300  /* Any change will effect the initializers, */
//
// These constants vary slightly from the normal "C" language conventions as they
// were copied from the old version written in Centura Team Developer.  They were
// copied verbatim in order to reduce confusion during the conversion.
//
#define UNIT_ENGLISH			1
#define UNIT_SI					2
#define UNIT_TYPE_FOOT_M		1		// foot or meter
#define UNIT_TYPE_FOOT_M_2		2		// Unit type square-foot square-meter
#define UNIT_TYPE_INCH_CM		3		// Unit type inch or centimeter
//
// ------------------ MicroPAVER distress numbers agree with the numbers MicroPAVER uses.
// ------------------ MicroPAVER Roadway Asphalt Cement Distresses
#define nMPar_AlliCrack 		 1
#define nMPar_Bleeding   		 2
#define nMPar_BlockCrack 		 3
#define nMPar_BumpAndSag		 4
#define nMPar_Corrugation 		 5
#define nMPar_Depression  		 6
#define nMPar_EdgeCrack   		 7
#define nMPar_JRefCrack 		 8
#define nMPar_LSDropOff 		 9
#define nMPar_LTCrack 		 10
#define nMPar_Patching 		 11
#define nMPar_PolishAgg 	 12
#define nMPar_Potholes 		 13
#define nMPar_RRXing 		 14
#define nMPar_Rutting 		 15
#define nMPar_Shoving 		 16
#define nMPar_SlipCrack 	 17
#define nMPar_Swell 		 18
#define nMPar_WeaRavel 		 19
// ------------------ MicroPAVER Roadway Portland Concrete Cement Distresses
#define nMPpr_Blowup       21
#define nMPpr_CBreak       22
#define nMPpr_DivSlab      23
#define nMPpr_DCrack       24
#define nMPpr_Fault        25
#define nMPpr_JSDamage     26
#define nMPpr_LSDropOff    27
#define nMPpr_LCrack       28
#define nMPpr_LPatch       29
#define nMPpr_SPatch       30
#define nMPpr_PolAgg       31
#define nMPpr_Popout       32
#define nMPpr_Pump         33
#define nMPpr_Punch        34
#define nMPpr_RRXing       35
#define nMPpr_Scale        36
#define nMPpr_Shrink       37
#define nMPpr_CSpall       38
#define nMPpr_JSpall       39
//------------------ MicroPAVER Air Pavement Asphalt Cement Distresses
#define nMPaa_AlliCrack    41
#define nMPaa_Bleeding     42
#define nMPaa_BlockCrack   43
#define nMPaa_Corrugation  44
#define nMPaa_Depression   45
#define nMPaa_JetBlast     46
#define nMPaa_JRefCrack    47
#define nMPaa_LTCrack      48
#define nMPaa_OilSpill     49
#define nMPaa_Patching     50
#define nMPaa_PolishAgg    51
#define nMPaa_WeaRavel     52
#define nMPaa_Rutting      53
#define nMPaa_Shoving      54
#define nMPaa_SlipCrack    55
#define nMPaa_Swell        56
// ------------------ an extra distress for use with ilfaa.ac
#define nMPaa_illaero        57
//------------------ MicroPAVER Air Pavement Portland Concrete Cement Distresses
#define nMPpa_Blowup       61
#define nMPpa_CBreak       62
#define nMPpa_LCrack       63
#define nMPpa_DCrack       64
#define nMPpa_JSDamage     65
#define nMPpa_SPatch       66
#define nMPpa_LPatch       67
#define nMPpa_Popout       68
#define nMPpa_Pump         69
#define nMPpa_Scale        70
#define nMPpa_Fault        71
#define nMPpa_Shatter      72
#define nMPpa_Shrink       73
#define nMPpa_JSpall       74
#define nMPpa_CSpall       75
// ------------------ Rigid windshield distress numbers
#define nwndrgd_DCrack           200
#define nwndrgd_JointSealFail    201
#define nwndrgd_LongCrack        202
#define nwndrgd_TransCrack       203
#define nwndrgd_LongJointSpall   204
#define nwndrgd_TranJointSpall   205
#define nwndrgd_Fault            206
#define nwndrgd_BrokenSlab       207
#define nwndrgd_Patch            208
#define nwndrgd_CornerSpall      209
// ------------------ Flexible windshield distress numbers
#define nwndflx_AlliCrack      230
#define nwndflx_Bleed          231
#define nwndflx_LongCrack      232
#define nwndflx_TransCrack     233
#define nwndflx_BlockCrack     234
#define nwndflx_RavelWeather   235
#define nwndflx_BitPatch       236
#define nwndflx_Rutting        237
// ------------------ CRCP windshield distress numbers
#define nwndcrc_DCrack              250
#define nwndcrc_LongJointSpall      251
#define nwndcrc_LongCrack           252
#define nwndcrc_TransCrack          253
#define nwndcrc_TransJointSpall     254
#define nwndcrc_BitPatch            255
#define nwndcrc_ConcretePatchDet    256
#define nwndcrc_Blowup              257
// ------------------ Clark Co distress numbers (4 methodologies)
#define washacAlliCrackPREV  270
#define washacAlliCrack  271
#define washacLongCrack  272
#define washacTransCrack  273
#define washacPatching  274
#define washacRaveling  275
#define washacFlushing  276
#define washacRuttingAndPavementWear  277
#define washacEdgeRaveling  278
#define washacEdgePatching  279
#define washacWavesAndSags  280
#define washpccCracking  290
#define washpccSpalling  291
#define washpccFaulting  292
#define washpccPumping  293
#define washpccPatching  294
#define washpccRaveling  295
#define washpccRutting  296
#define washpccBlowUp  297
#define washpccEdgeRaveling  298

#define clkacRutting  310
#define clkacEdgePatch  311
#define clkacWavesAndSags  312
#define clkacAlliCrackPREV  313
#define clkacAlliCrack  314
#define clkacRaveling  315
#define clkacLongCrack  316
#define clkacTranCrack  317
#define clkacPatching  318
#define clkacEdgeRaveling  319
#define clkacDepression  320

#define clkbitAlliCrackPREV  330
#define clkbitAlliCrack  331
#define clkbitRutting  332
#define clkbitLongCrack  333
#define clkbitRaveling  334
#define clkbitTransCrack  335
#define clkbitPatching  336
#define clkbitBlockCrack  337
#define clkbitCrackSeal  338
#define clkbitEdgeRaveling  339
#define clkbitEdgePatch  340
// ------------------------------------- symbolic names for the PCI correction curve indices
#define MPACRDQ1  0
#define MPACRDQ2  1
#define MPACRDQ3  2
#define MPACRDQ4  3
#define MPACRDQ5  4
#define MPACRDQ6  5
#define MPACRDQ7  6
#define FAA82PCCQ1  7
#define FAA82PCCQ2  8
#define FAA82PCCQ3  9
#define FAA82PCCQ4  10
#define FAA82PCCQ5  11
#define FAA82PCCQ6  12
#define FAA82PCCQ7  13
#define FAA82PCCQ8  14
#define FAA82ACQ1  15
#define FAA82ACQ2  16
#define FAA82ACQ3  17
#define FAA82ACQ4  18
#define FAA82ACQ5  19
#define FAA82ACQ6  20
#define MPPCCRDQ1  21
#define MPPCCRDQ2  22
#define MPPCCRDQ3  23
#define MPPCCRDQ4  24
#define MPPCCRDQ5  25
#define MPPCCRDQ6  26
#define MPPCCRDQ7  27
#define MPPCCRDQ8  28
#define MPPCCRDQ9  29
#define min(a,b) ((a) <= (b) ? (a) : (b))

}
