using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports.DDOT
{
	public class WeightedAveragePCIByWard : TwoColumnReport
	{
		public WeightedAveragePCIByWard( int netID, string usrName )
		{
			networkID = netID;
			userName = usrName;
			dataQuery = "SELECT WARD.DATA_ AS WARD, SUM(PCI.AREA*PCI.PCI)/SUM(PCI.AREA) FROM PCI INNER JOIN WARD ON (PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION) GROUP BY WARD.DATA_";
			reportName = "WeightedAveragePCIByWard";
			reportTitle = "Weighted Average PCI By Ward";
			colOneHeaderName = "Ward";
			colTwoHeaderName = "Average PCI";
		}
	}
}
