using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model.DataColumns;
using ReportExporters.Common.Rdlc.Enums;

namespace ReportExporters.Common.Model.TableGroups
{
	public class ReportTableGroup
	{
		private List<ReportColumn> _columnGrouping;
		public List<ReportColumn> ColumnGrouping
		{
			get { return _columnGrouping; }
			set { _columnGrouping = value; }
		}

		private Dictionary<ReportColumn, SortOrder> _columnSorting;
		public Dictionary<ReportColumn, SortOrder> ColumnSorting
		{
			get { return _columnSorting; }
			set { _columnSorting = value; }
		}

		public ReportTableGroup()
		{
			_columnGrouping = new List<ReportColumn>();
			_columnSorting = new Dictionary<ReportColumn, SortOrder>();
		}
	}

	public class ReportTableGroupList : List<ReportTableGroup>
	{
		public bool IsMatrix = false;
		
		public bool ContainsColumn(ReportColumn rColumn)
		{
			bool toRet = false;
		
			foreach(ReportTableGroup rtGroup in this)
			{
				if (rtGroup.ColumnGrouping.Contains(rColumn))
				{
					toRet = true;
					break;
				}
			}
			return toRet;
		}

	}
	
	
}
