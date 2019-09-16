using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;
using ReportExporters.Common.Rdlc.Enums;

namespace ReportExporters.Common.Rdlc.Wrapper
{
	/// <summary>
	/// Grouping, Sorting, Filtering
	/// </summary>
	internal partial class RdlcWrapper
	{
		internal class RSortBy : IRdlElement
		{
			private string _sortExpression;
			/// <summary>
			/// Sort the groups by expression
			/// </summary>
			public string SortExpression
			{
				get
				{
					return _sortExpression;
				}
				set
				{
					_sortExpression = value;
				}
			}

			private SortOrder _direction;
			/// <summary>
			/// direction of the sort. Ascending or Descending
			/// </summary>
			public SortOrder Direction
			{
				get
				{
					return _direction;
				}
				set
				{
					_direction = value;
				}
			}

			#region IRdlElement Members

			public void WriteTo(System.Xml.XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("SortBy");
				{
					xmlWriter.WriteElementString("SortExpression", SortExpression);
					xmlWriter.WriteElementString("Direction", Direction.ToString());
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RSorting : IRdlElement
		{
			private IList<RSortBy> _sortBy;
			public IList<RSortBy> SortBy
			{
				get
				{
					return _sortBy;
				}
				set
				{
					_sortBy = value;
				}
			}

			public RSorting()
			{
				_sortBy = new List<RSortBy>();
			}

			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				if (SortBy.Count > 0)
				{
					xmlWriter.WriteStartElement("Sorting");
					{
						foreach (RSortBy rSortBy in SortBy)
						{
							rSortBy.WriteTo(xmlWriter);
						}
					}
					xmlWriter.WriteEndElement();
				}
			}

			#endregion
		}

		internal class RFilterValues : Collection<RExpression>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("FilterValues");
				{
					foreach (RExpression filterValue in this)
					{
						xmlWriter.WriteElementString("FilterValue", filterValue.ToString());
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		//internal class RFilter : IRdlElement
		//{
		//  public RExpression FilterExpression;
		//  public Operators Operator;
		//  public RFilterValues FilterValues;
			
		//  public RFilter()
		//  {
		//  }

		//  #region IRdlElement Members

		//  public void WriteTo(XmlWriter xmlWriter)
		//  {
		//    xmlWriter.WriteStartElement("Filter");
		//    {
		//      xmlWriter.WriteElementString("FilterExpression", FilterExpression.ToString());
		//      xmlWriter.WriteElementString("Operator", Operator.ToString());
		//      FilterValues.WriteTo(xmlWriter);
		//    }
		//    xmlWriter.WriteEndElement();
		//  }

		//  #endregion
		//}

		//internal class RFilters : Collection<RFilter>, IRdlElement
		//{
		//  #region IRdlElement Members

		//  public void WriteTo(XmlWriter xmlWriter)
		//  {
		//    xmlWriter.WriteStartElement("Filters");
		//    {
		//      foreach (RFilter filter in this)
		//      {
		//        filter.WriteTo(xmlWriter);
		//      }
		//    }
		//    xmlWriter.WriteEndElement();
		//  }

		//  #endregion
		//}

		internal class RGroupExpressions : Collection<RExpression>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("GroupExpressions");
				{
					foreach (RExpression groupExpression in this)
					{
						xmlWriter.WriteElementString("GroupExpression", groupExpression.ToString());
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RGrouping : IRdlElement
		{
			private string _name;
			public string Name
			{
				get
				{
					return _name;
				}
				set
				{
					_name = value;
				}
			}

			private string _label;
			public string Label
			{
				get
				{
					return _label;
				}
				set
				{
					_label = value;
				}
			}

			private RGroupExpressions _groupExpressions;
			public RGroupExpressions GroupExpressions
			{
				get
				{
					return _groupExpressions;
				}
				set
				{
					_groupExpressions = value;
				}
			}

			//bool PageBreakAtStart;
			//bool PageBreakAtEnd;
			//string Filters;

			private RExpression _parent;
			public RExpression Parent
			{
				get
				{
					return _parent;
				}
				set
				{
					_parent = value;
				}
			}

			//string DataElementName, DataCollectionName, DataElementOutput;

			public RGrouping()
			{
				GroupExpressions = new RGroupExpressions();
			}

			public RGrouping(RMatrix matrix, RRowGroupings rowGroupings) : this()
			{
				this.Name = string.Format("{0}_RowGroup{1}", matrix.Name, rowGroupings.Count + 1);
			}

			public RGrouping(RMatrix matrix, RColumnGroupings columnGroupings)
				: this()
			{
				this.Name = string.Format("{0}_ColumnGroup{1}", matrix.Name, columnGroupings.Count + 1);
			}
			
			
			//<Grouping Name="matrix2_SubCat">

			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Grouping");
				{
					xmlWriter.WriteAttributeString("Name", Name);

					GroupExpressions.WriteTo(xmlWriter);

					if (Parent != null) xmlWriter.WriteElementString("Parent", Parent);
				}
				xmlWriter.WriteEndElement();
			}

			#endregion

			//<Grouping Name="Table1_DetailsGroup">
			//  <GroupExpressions>
			//    <GroupExpression>=Fields!EmployeeID.Value</GroupExpression>
			//  </GroupExpressions>
			//  <Parent>=Fields!ManagerID.Value</Parent>
			//</Grouping>
		}

	}
}
