using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;
using ReportExporters.Common.Rdlc.Enums;
using ReportExporters.Common.Model.DataColumns;
using System.Web.UI.WebControls;
using System.Data;
using ReportExporters.Common.Model.TableGroups;
using ReportExporters.Common.Model.Chart;
using ReportExporters.Common.Model.Style;

namespace ReportExporters.Common.Rdlc.Wrapper
{
	internal partial class RdlcWrapper
	{
		/// <summary>
		/// Indicates the ReportItem should not be (initially) shown in the output report.
		/// </summary>
		internal class RThreeDProperties : ThreeDProperties, IRdlElement
		{
			public RThreeDProperties(ThreeDProperties threeDProperties)
				: base(threeDProperties)
			{
			}

			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("ThreeDProperties");
				{
					if (Enabled.HasValue)
					{
						xmlWriter.WriteElementString("Enabled", Enabled.Value.ToString());
					}

					xmlWriter.WriteElementString("ProjectionMode", ProjectionMode.ToString());

					if (ProjectionMode == ProjectionMode3DForRendering.Perspective)
					{
						if (Perspective.HasValue)
						{
							xmlWriter.WriteElementString("Perspective", Perspective.ToString());
						}
					}

					if (Rotation.HasValue)
					{
						xmlWriter.WriteElementString("Rotation", Rotation.ToString());
					}

					if (Inclination.HasValue)
					{
						xmlWriter.WriteElementString("Inclination", Inclination.ToString());
					}

					xmlWriter.WriteElementString("Shading", Shading.ToString());

					if (WallThickness.HasValue)
					{
						xmlWriter.WriteElementString("WallThickness", WallThickness.ToString());
					}

					//if ( chart type is bar OR column)
					{
						xmlWriter.WriteElementString("DrawingStyle", DrawingStyle.ToString());
						if (Clustered.HasValue)
						{
							xmlWriter.WriteElementString("Clustered", Clustered.ToString());
						}
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}
		
		internal class RChart : RDataRegion
		{
			private RChart() : base()
			{
				this._name = String.Format("chart{0}", UniqueIndex);
			}
		}

	}
}
