namespace AppliedResearchAssociates.PciDistress
{
    internal static class Constants
    {
        #region

        public static int NUMPTS { get; } = 13; /* first&last pts must be out of range sentinels */

        public static int NUMQ { get; } = 40; /* Any change will effect the initializers, */

        public static int SALDISTRESSRANGE { get; } = 200; /* Any change will effect the initializers, */

        public static int QROWS { get; } = 8; // Selected by the number of large deducts (deducts > 5)

        public static int CVALUES { get; } = 7; // values for c1 through C6 in the formula

        public static int NUMDISTRESS { get; } = 200; /* must agree with # in dis2row  */

        public static int NUMCURVES { get; } = 300; /* Any change will effect the initializers, */

        #endregion

        #region

        // These constants vary slightly from the normal "C" language conventions as they were
        // copied from the old version written in Centura Team Developer. They were copied verbatim
        // in order to reduce confusion during the conversion.

        public static int UNIT_ENGLISH { get; } = 1;

        public static int UNIT_SI { get; } = 2;

        public static int UNIT_TYPE_FOOT_M { get; } = 1; // foot or meter

        public static int UNIT_TYPE_FOOT_M_2 { get; } = 2; // Unit type square-foot square-meter

        public static int UNIT_TYPE_INCH_CM { get; } = 3; // Unit type inch or centimeter

        #endregion

        // ------------------ MicroPAVER distress numbers agree with the numbers MicroPAVER uses.

        #region ------------------ MicroPAVER Roadway Asphalt Cement Distresses

        public static int nMPar_AlliCrack { get; } = 1;

        public static int nMPar_Bleeding { get; } = 2;

        public static int nMPar_BlockCrack { get; } = 3;

        public static int nMPar_BumpAndSag { get; } = 4;

        public static int nMPar_Corrugation { get; } = 5;

        public static int nMPar_Depression { get; } = 6;

        public static int nMPar_EdgeCrack { get; } = 7;

        public static int nMPar_JRefCrack { get; } = 8;

        public static int nMPar_LSDropOff { get; } = 9;

        public static int nMPar_LTCrack { get; } = 10;

        public static int nMPar_Patching { get; } = 11;

        public static int nMPar_PolishAgg { get; } = 12;

        public static int nMPar_Potholes { get; } = 13;

        public static int nMPar_RRXing { get; } = 14;

        public static int nMPar_Rutting { get; } = 15;

        public static int nMPar_Shoving { get; } = 16;

        public static int nMPar_SlipCrack { get; } = 17;

        public static int nMPar_Swell { get; } = 18;

        public static int nMPar_WeaRavel { get; } = 19;

        #endregion

        #region ------------------ MicroPAVER Roadway Portland Concrete Cement Distresses

        public static int nMPpr_Blowup { get; } = 21;

        public static int nMPpr_CBreak { get; } = 22;

        public static int nMPpr_DivSlab { get; } = 23;

        public static int nMPpr_DCrack { get; } = 24;

        public static int nMPpr_Fault { get; } = 25;

        public static int nMPpr_JSDamage { get; } = 26;

        public static int nMPpr_LSDropOff { get; } = 27;

        public static int nMPpr_LCrack { get; } = 28;

        public static int nMPpr_LPatch { get; } = 29;

        public static int nMPpr_SPatch { get; } = 30;

        public static int nMPpr_PolAgg { get; } = 31;

        public static int nMPpr_Popout { get; } = 32;

        public static int nMPpr_Pump { get; } = 33;

        public static int nMPpr_Punch { get; } = 34;

        public static int nMPpr_RRXing { get; } = 35;

        public static int nMPpr_Scale { get; } = 36;

        public static int nMPpr_Shrink { get; } = 37;

        public static int nMPpr_CSpall { get; } = 38;

        public static int nMPpr_JSpall { get; } = 39;

        #endregion

        #region ------------------ MicroPAVER Air Pavement Asphalt Cement Distresses

        public static int nMPaa_AlliCrack { get; } = 41;

        public static int nMPaa_Bleeding { get; } = 42;

        public static int nMPaa_BlockCrack { get; } = 43;

        public static int nMPaa_Corrugation { get; } = 44;

        public static int nMPaa_Depression { get; } = 45;

        public static int nMPaa_JetBlast { get; } = 46;

        public static int nMPaa_JRefCrack { get; } = 47;

        public static int nMPaa_LTCrack { get; } = 48;

        public static int nMPaa_OilSpill { get; } = 49;

        public static int nMPaa_Patching { get; } = 50;

        public static int nMPaa_PolishAgg { get; } = 51;

        public static int nMPaa_WeaRavel { get; } = 52;

        public static int nMPaa_Rutting { get; } = 53;

        public static int nMPaa_Shoving { get; } = 54;

        public static int nMPaa_SlipCrack { get; } = 55;

        public static int nMPaa_Swell { get; } = 56;

        public static int nMPaa_illaero { get; } = 57; // ------------------ an extra distress for use with ilfaa.ac

        #endregion

        #region ------------------ MicroPAVER Air Pavement Portland Concrete Cement Distresses

        public static int nMPpa_Blowup { get; } = 61;

        public static int nMPpa_CBreak { get; } = 62;

        public static int nMPpa_LCrack { get; } = 63;

        public static int nMPpa_DCrack { get; } = 64;

        public static int nMPpa_JSDamage { get; } = 65;

        public static int nMPpa_SPatch { get; } = 66;

        public static int nMPpa_LPatch { get; } = 67;

        public static int nMPpa_Popout { get; } = 68;

        public static int nMPpa_Pump { get; } = 69;

        public static int nMPpa_Scale { get; } = 70;

        public static int nMPpa_Fault { get; } = 71;

        public static int nMPpa_Shatter { get; } = 72;

        public static int nMPpa_Shrink { get; } = 73;

        public static int nMPpa_JSpall { get; } = 74;

        public static int nMPpa_CSpall { get; } = 75;

        #endregion

        #region ------------------ Rigid windshield distress numbers

        public static int nwndrgd_DCrack { get; } = 200;

        public static int nwndrgd_JointSealFail { get; } = 201;

        public static int nwndrgd_LongCrack { get; } = 202;

        public static int nwndrgd_TransCrack { get; } = 203;

        public static int nwndrgd_LongJointSpall { get; } = 204;

        public static int nwndrgd_TranJointSpall { get; } = 205;

        public static int nwndrgd_Fault { get; } = 206;

        public static int nwndrgd_BrokenSlab { get; } = 207;

        public static int nwndrgd_Patch { get; } = 208;

        public static int nwndrgd_CornerSpall { get; } = 209;

        #endregion

        #region ------------------ Flexible windshield distress numbers

        public static int nwndflx_AlliCrack { get; } = 230;

        public static int nwndflx_Bleed { get; } = 231;

        public static int nwndflx_LongCrack { get; } = 232;

        public static int nwndflx_TransCrack { get; } = 233;

        public static int nwndflx_BlockCrack { get; } = 234;

        public static int nwndflx_RavelWeather { get; } = 235;

        public static int nwndflx_BitPatch { get; } = 236;

        public static int nwndflx_Rutting { get; } = 237;

        #endregion

        #region ------------------ CRCP windshield distress numbers

        public static int nwndcrc_DCrack { get; } = 250;

        public static int nwndcrc_LongJointSpall { get; } = 251;

        public static int nwndcrc_LongCrack { get; } = 252;

        public static int nwndcrc_TransCrack { get; } = 253;

        public static int nwndcrc_TransJointSpall { get; } = 254;

        public static int nwndcrc_BitPatch { get; } = 255;

        public static int nwndcrc_ConcretePatchDet { get; } = 256;

        public static int nwndcrc_Blowup { get; } = 257;

        #endregion

        #region ------------------ Clark Co distress numbers (4 methodologies)

        public static int washacAlliCrackPREV { get; } = 270;

        public static int washacAlliCrack { get; } = 271;

        public static int washacLongCrack { get; } = 272;

        public static int washacTransCrack { get; } = 273;

        public static int washacPatching { get; } = 274;

        public static int washacRaveling { get; } = 275;

        public static int washacFlushing { get; } = 276;

        public static int washacRuttingAndPavementWear { get; } = 277;

        public static int washacEdgeRaveling { get; } = 278;

        public static int washacEdgePatching { get; } = 279;

        public static int washacWavesAndSags { get; } = 280;

        public static int washpccCracking { get; } = 290;

        public static int washpccSpalling { get; } = 291;

        public static int washpccFaulting { get; } = 292;

        public static int washpccPumping { get; } = 293;

        public static int washpccPatching { get; } = 294;

        public static int washpccRaveling { get; } = 295;

        public static int washpccRutting { get; } = 296;

        public static int washpccBlowUp { get; } = 297;

        public static int washpccEdgeRaveling { get; } = 298;

        public static int clkacRutting { get; } = 310;

        public static int clkacEdgePatch { get; } = 311;

        public static int clkacWavesAndSags { get; } = 312;

        public static int clkacAlliCrackPREV { get; } = 313;

        public static int clkacAlliCrack { get; } = 314;

        public static int clkacRaveling { get; } = 315;

        public static int clkacLongCrack { get; } = 316;

        public static int clkacTranCrack { get; } = 317;

        public static int clkacPatching { get; } = 318;

        public static int clkacEdgeRaveling { get; } = 319;

        public static int clkacDepression { get; } = 320;

        public static int clkbitAlliCrackPREV { get; } = 330;

        public static int clkbitAlliCrack { get; } = 331;

        public static int clkbitRutting { get; } = 332;

        public static int clkbitLongCrack { get; } = 333;

        public static int clkbitRaveling { get; } = 334;

        public static int clkbitTransCrack { get; } = 335;

        public static int clkbitPatching { get; } = 336;

        public static int clkbitBlockCrack { get; } = 337;

        public static int clkbitCrackSeal { get; } = 338;

        public static int clkbitEdgeRaveling { get; } = 339;

        public static int clkbitEdgePatch { get; } = 340;

        #endregion

        #region ------------------------------------- symbolic names for the PCI correction curve indices

        public static int MPACRDQ1 { get; } = 0;

        public static int MPACRDQ2 { get; } = 1;

        public static int MPACRDQ3 { get; } = 2;

        public static int MPACRDQ4 { get; } = 3;

        public static int MPACRDQ5 { get; } = 4;

        public static int MPACRDQ6 { get; } = 5;

        public static int MPACRDQ7 { get; } = 6;

        public static int FAA82PCCQ1 { get; } = 7;

        public static int FAA82PCCQ2 { get; } = 8;

        public static int FAA82PCCQ3 { get; } = 9;

        public static int FAA82PCCQ4 { get; } = 10;

        public static int FAA82PCCQ5 { get; } = 11;

        public static int FAA82PCCQ6 { get; } = 12;

        public static int FAA82PCCQ7 { get; } = 13;

        public static int FAA82PCCQ8 { get; } = 14;

        public static int FAA82ACQ1 { get; } = 15;

        public static int FAA82ACQ2 { get; } = 16;

        public static int FAA82ACQ3 { get; } = 17;

        public static int FAA82ACQ4 { get; } = 18;

        public static int FAA82ACQ5 { get; } = 19;

        public static int FAA82ACQ6 { get; } = 20;

        public static int MPPCCRDQ1 { get; } = 21;

        public static int MPPCCRDQ2 { get; } = 22;

        public static int MPPCCRDQ3 { get; } = 23;

        public static int MPPCCRDQ4 { get; } = 24;

        public static int MPPCCRDQ5 { get; } = 25;

        public static int MPPCCRDQ6 { get; } = 26;

        public static int MPPCCRDQ7 { get; } = 27;

        public static int MPPCCRDQ8 { get; } = 28;

        public static int MPPCCRDQ9 { get; } = 29;

        #endregion
    }
}
