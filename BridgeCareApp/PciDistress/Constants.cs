namespace AppliedResearchAssociates.PciDistress
{
    internal static class Constants
    {
        #region

        public const int NUMPTS = 13; /* first&last pts must be out of range sentinels */

        public const int NUMQ = 40; /* Any change will effect the initializers, */

        public const int SALDISTRESSRANGE = 200; /* Any change will effect the initializers, */

        public const int QROWS = 8; // Selected by the number of large deducts (deducts > 5)

        public const int CVALUES = 7; // values for c1 through C6 in the formula

        public const int NUMDISTRESS = 200; /* must agree with # in dis2row  */

        public const int NUMCURVES = 300; /* Any change will effect the initializers, */

        #endregion

        #region

        // These constants vary slightly from the normal "C" language conventions as they were
        // copied from the old version written in Centura Team Developer. They were copied verbatim
        // in order to reduce confusion during the conversion.

        public const int UNIT_ENGLISH = 1;

        public const int UNIT_SI = 2;

        public const int UNIT_TYPE_FOOT_M = 1; // foot or meter

        public const int UNIT_TYPE_FOOT_M_2 = 2; // Unit type square-foot square-meter

        public const int UNIT_TYPE_INCH_CM = 3; // Unit type inch or centimeter

        #endregion

        // ------------------ MicroPAVER distress numbers agree with the numbers MicroPAVER uses.

        #region ------------------ MicroPAVER Roadway Asphalt Cement Distresses

        public const int nMPar_AlliCrack = 1;

        public const int nMPar_Bleeding = 2;

        public const int nMPar_BlockCrack = 3;

        public const int nMPar_BumpAndSag = 4;

        public const int nMPar_Corrugation = 5;

        public const int nMPar_Depression = 6;

        public const int nMPar_EdgeCrack = 7;

        public const int nMPar_JRefCrack = 8;

        public const int nMPar_LSDropOff = 9;

        public const int nMPar_LTCrack = 10;

        public const int nMPar_Patching = 11;

        public const int nMPar_PolishAgg = 12;

        public const int nMPar_Potholes = 13;

        public const int nMPar_RRXing = 14;

        public const int nMPar_Rutting = 15;

        public const int nMPar_Shoving = 16;

        public const int nMPar_SlipCrack = 17;

        public const int nMPar_Swell = 18;

        public const int nMPar_WeaRavel = 19;

        #endregion

        #region ------------------ MicroPAVER Roadway Portland Concrete Cement Distresses

        public const int nMPpr_Blowup = 21;

        public const int nMPpr_CBreak = 22;

        public const int nMPpr_DivSlab = 23;

        public const int nMPpr_DCrack = 24;

        public const int nMPpr_Fault = 25;

        public const int nMPpr_JSDamage = 26;

        public const int nMPpr_LSDropOff = 27;

        public const int nMPpr_LCrack = 28;

        public const int nMPpr_LPatch = 29;

        public const int nMPpr_SPatch = 30;

        public const int nMPpr_PolAgg = 31;

        public const int nMPpr_Popout = 32;

        public const int nMPpr_Pump = 33;

        public const int nMPpr_Punch = 34;

        public const int nMPpr_RRXing = 35;

        public const int nMPpr_Scale = 36;

        public const int nMPpr_Shrink = 37;

        public const int nMPpr_CSpall = 38;

        public const int nMPpr_JSpall = 39;

        #endregion

        #region ------------------ MicroPAVER Air Pavement Asphalt Cement Distresses

        public const int nMPaa_AlliCrack = 41;

        public const int nMPaa_Bleeding = 42;

        public const int nMPaa_BlockCrack = 43;

        public const int nMPaa_Corrugation = 44;

        public const int nMPaa_Depression = 45;

        public const int nMPaa_JetBlast = 46;

        public const int nMPaa_JRefCrack = 47;

        public const int nMPaa_LTCrack = 48;

        public const int nMPaa_OilSpill = 49;

        public const int nMPaa_Patching = 50;

        public const int nMPaa_PolishAgg = 51;

        public const int nMPaa_WeaRavel = 52;

        public const int nMPaa_Rutting = 53;

        public const int nMPaa_Shoving = 54;

        public const int nMPaa_SlipCrack = 55;

        public const int nMPaa_Swell = 56;

        public const int nMPaa_illaero = 57; // ------------------ an extra distress for use with ilfaa.ac

        #endregion

        #region ------------------ MicroPAVER Air Pavement Portland Concrete Cement Distresses

        public const int nMPpa_Blowup = 61;

        public const int nMPpa_CBreak = 62;

        public const int nMPpa_LCrack = 63;

        public const int nMPpa_DCrack = 64;

        public const int nMPpa_JSDamage = 65;

        public const int nMPpa_SPatch = 66;

        public const int nMPpa_LPatch = 67;

        public const int nMPpa_Popout = 68;

        public const int nMPpa_Pump = 69;

        public const int nMPpa_Scale = 70;

        public const int nMPpa_Fault = 71;

        public const int nMPpa_Shatter = 72;

        public const int nMPpa_Shrink = 73;

        public const int nMPpa_JSpall = 74;

        public const int nMPpa_CSpall = 75;

        #endregion

        #region ------------------ Rigid windshield distress numbers

        public const int nwndrgd_DCrack = 200;

        public const int nwndrgd_JointSealFail = 201;

        public const int nwndrgd_LongCrack = 202;

        public const int nwndrgd_TransCrack = 203;

        public const int nwndrgd_LongJointSpall = 204;

        public const int nwndrgd_TranJointSpall = 205;

        public const int nwndrgd_Fault = 206;

        public const int nwndrgd_BrokenSlab = 207;

        public const int nwndrgd_Patch = 208;

        public const int nwndrgd_CornerSpall = 209;

        #endregion

        #region ------------------ Flexible windshield distress numbers

        public const int nwndflx_AlliCrack = 230;

        public const int nwndflx_Bleed = 231;

        public const int nwndflx_LongCrack = 232;

        public const int nwndflx_TransCrack = 233;

        public const int nwndflx_BlockCrack = 234;

        public const int nwndflx_RavelWeather = 235;

        public const int nwndflx_BitPatch = 236;

        public const int nwndflx_Rutting = 237;

        #endregion

        #region ------------------ CRCP windshield distress numbers

        public const int nwndcrc_DCrack = 250;

        public const int nwndcrc_LongJointSpall = 251;

        public const int nwndcrc_LongCrack = 252;

        public const int nwndcrc_TransCrack = 253;

        public const int nwndcrc_TransJointSpall = 254;

        public const int nwndcrc_BitPatch = 255;

        public const int nwndcrc_ConcretePatchDet = 256;

        public const int nwndcrc_Blowup = 257;

        #endregion

        #region ------------------ Clark Co distress numbers (4 methodologies)

        public const int washacAlliCrackPREV = 270;

        public const int washacAlliCrack = 271;

        public const int washacLongCrack = 272;

        public const int washacTransCrack = 273;

        public const int washacPatching = 274;

        public const int washacRaveling = 275;

        public const int washacFlushing = 276;

        public const int washacRuttingAndPavementWear = 277;

        public const int washacEdgeRaveling = 278;

        public const int washacEdgePatching = 279;

        public const int washacWavesAndSags = 280;

        public const int washpccCracking = 290;

        public const int washpccSpalling = 291;

        public const int washpccFaulting = 292;

        public const int washpccPumping = 293;

        public const int washpccPatching = 294;

        public const int washpccRaveling = 295;

        public const int washpccRutting = 296;

        public const int washpccBlowUp = 297;

        public const int washpccEdgeRaveling = 298;

        public const int clkacRutting = 310;

        public const int clkacEdgePatch = 311;

        public const int clkacWavesAndSags = 312;

        public const int clkacAlliCrackPREV = 313;

        public const int clkacAlliCrack = 314;

        public const int clkacRaveling = 315;

        public const int clkacLongCrack = 316;

        public const int clkacTranCrack = 317;

        public const int clkacPatching = 318;

        public const int clkacEdgeRaveling = 319;

        public const int clkacDepression = 320;

        public const int clkbitAlliCrackPREV = 330;

        public const int clkbitAlliCrack = 331;

        public const int clkbitRutting = 332;

        public const int clkbitLongCrack = 333;

        public const int clkbitRaveling = 334;

        public const int clkbitTransCrack = 335;

        public const int clkbitPatching = 336;

        public const int clkbitBlockCrack = 337;

        public const int clkbitCrackSeal = 338;

        public const int clkbitEdgeRaveling = 339;

        public const int clkbitEdgePatch = 340;

        #endregion

        #region ------------------------------------- symbolic names for the PCI correction curve indices

        public const int MPACRDQ1 = 0;

        public const int MPACRDQ2 = 1;

        public const int MPACRDQ3 = 2;

        public const int MPACRDQ4 = 3;

        public const int MPACRDQ5 = 4;

        public const int MPACRDQ6 = 5;

        public const int MPACRDQ7 = 6;

        public const int FAA82PCCQ1 = 7;

        public const int FAA82PCCQ2 = 8;

        public const int FAA82PCCQ3 = 9;

        public const int FAA82PCCQ4 = 10;

        public const int FAA82PCCQ5 = 11;

        public const int FAA82PCCQ6 = 12;

        public const int FAA82PCCQ7 = 13;

        public const int FAA82PCCQ8 = 14;

        public const int FAA82ACQ1 = 15;

        public const int FAA82ACQ2 = 16;

        public const int FAA82ACQ3 = 17;

        public const int FAA82ACQ4 = 18;

        public const int FAA82ACQ5 = 19;

        public const int FAA82ACQ6 = 20;

        public const int MPPCCRDQ1 = 21;

        public const int MPPCCRDQ2 = 22;

        public const int MPPCCRDQ3 = 23;

        public const int MPPCCRDQ4 = 24;

        public const int MPPCCRDQ5 = 25;

        public const int MPPCCRDQ6 = 26;

        public const int MPPCCRDQ7 = 27;

        public const int MPPCCRDQ8 = 28;

        public const int MPPCCRDQ9 = 29;

        #endregion
    }
}
