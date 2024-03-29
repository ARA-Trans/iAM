﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Services.SummaryReport.Charts.PostedCountByBPN;
using BridgeCare.Services.SummaryReport.PoorDeckAreaByBPN;
using OfficeOpenXml;

namespace BridgeCare.Services.SummaryReport.Charts
{
    public class BridgeWorkSummaryCharts
    {
        private readonly PoorBridgeDeckAreaByBPN poorBridgeDeckAreaByBPN;
        private readonly PostedBridgeCountByBPN postedCountByBPN;
        private readonly PostedBridgeDeckAreaByBPN postedBridgeDeckAreaByBPN;
        private readonly ClosedBridgeCountByBPN closedBridgeCountByBPN;
        private readonly ClosedBridgeDeckAreaByBPN closedBridgeDeckAreaByBPN;
        private readonly CombinedPostedClosedByBPN combinedPostedClosedByBPN;
        private readonly CashNeededByBPN cashNeededByBPN;

        public BridgeWorkSummaryCharts(PoorBridgeDeckAreaByBPN poorBridgeDeckAreaByBPN, PostedBridgeCountByBPN postedCountByBPN,
            PostedBridgeDeckAreaByBPN postedBridgeDeckAreaByBPN, ClosedBridgeCountByBPN closedBridgeCountByBPN,
            ClosedBridgeDeckAreaByBPN closedBridgeDeckAreaByBPN, CombinedPostedClosedByBPN combinedPostedClosedByBPN, CashNeededByBPN cashNeededByBPN)
        {
            this.poorBridgeDeckAreaByBPN = poorBridgeDeckAreaByBPN ?? throw new ArgumentNullException(nameof(poorBridgeDeckAreaByBPN));
            this.postedCountByBPN = postedCountByBPN ?? throw new ArgumentNullException(nameof(postedCountByBPN));
            this.postedBridgeDeckAreaByBPN = postedBridgeDeckAreaByBPN ?? throw new ArgumentNullException(nameof(postedBridgeDeckAreaByBPN));
            this.closedBridgeCountByBPN = closedBridgeCountByBPN ?? throw new ArgumentNullException(nameof(closedBridgeCountByBPN));
            this.closedBridgeDeckAreaByBPN = closedBridgeDeckAreaByBPN ?? throw new ArgumentNullException(nameof(closedBridgeDeckAreaByBPN));
            this.combinedPostedClosedByBPN = combinedPostedClosedByBPN ?? throw new ArgumentNullException(nameof(combinedPostedClosedByBPN));
            this.cashNeededByBPN = cashNeededByBPN ?? throw new ArgumentNullException(nameof(cashNeededByBPN));
        }
        internal void FillPoorDeckAreaByBPN(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorDeckAreaByBPNSectionYearsRow, int simulationYearsCount)
        {
            poorBridgeDeckAreaByBPN.Fill(worksheet, bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNSectionYearsRow, simulationYearsCount);
        }

        internal void FillPostedBridgeCountByBPN(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalBridgePostedCountByBPNYearsRow, int simulationYearsCount)
        {
            postedCountByBPN.Fill(worksheet, bridgeWorkSummaryWorkSheet, totalBridgePostedCountByBPNYearsRow, simulationYearsCount);
        }

        internal void FillPostedBridgeDeckAreaByBPN(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPostedBridgeDeckAreaByBPNYearsRow, int simulationYearsCount)
        {
            postedBridgeDeckAreaByBPN.Fill(worksheet, bridgeWorkSummaryWorkSheet, totalPostedBridgeDeckAreaByBPNYearsRow, simulationYearsCount);
        }

        internal void FillClosedBridgeCountByBPN(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalClosedBridgeCountByBPNYearsRow, int simulationYearsCount)
        {
            closedBridgeCountByBPN.Fill(worksheet, bridgeWorkSummaryWorkSheet, totalClosedBridgeCountByBPNYearsRow, simulationYearsCount);
        }

        internal void FillClosedBridgeDeckAreaByBPN(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalClosedBridgeDeckAreaByBPNYearsRow, int simulationYearsCount)
        {
            closedBridgeDeckAreaByBPN.Fill(worksheet, bridgeWorkSummaryWorkSheet, totalClosedBridgeDeckAreaByBPNYearsRow, simulationYearsCount);
        }

        internal void FillCombinedPostedAndClosedByBPN(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPostedAndClosedByBPNYearsRow, int simulationYearsCount)
        {
            combinedPostedClosedByBPN.Fill(worksheet, bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, simulationYearsCount);
        }

        internal void FillCashNeededDeckAreaByBPN(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalCashNeededByBPNYearsRow, int simulationYearsCount)
        {
            cashNeededByBPN.Fill(worksheet, bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, simulationYearsCount);
        }
    }
}
