using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports.DDOT
{
	public class WeightedAveragePCIByClass : TwoColumnReport
	{
		public WeightedAveragePCIByClass( int netID, string usrName )
		{
			networkID = netID;
			userName = usrName;
			dataQuery = "SELECT CLASS.DATA_ AS CLASS, SUM(PCI.AREA*PCI.PCI)/SUM(PCI.AREA) FROM PCI INNER JOIN CLASS ON (PCI.FACILITY = CLASS.FACILITY AND PCI.SECTION = CLASS.SECTION) GROUP BY CLASS.DATA_";
			reportName = "WeightedAveragePCIByClass";
			reportTitle = "Weighted Average PCI By Class";
			colOneHeaderName = "Functional Class";
			colTwoHeaderName = "Average PCI";
		}
	}
}
