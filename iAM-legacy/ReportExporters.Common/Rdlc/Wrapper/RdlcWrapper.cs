using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;
using System.IO;
using System.Data;
using ReportExporters.Common.Model.Style;
using System.Web.UI.WebControls;
using ReportExporters.Common.Rdlc.Enums;
using System.Drawing;
using System.Globalization;
using ReportExporters.Common.Rdlc;
using ReportExporters.Common.Model.DataColumns;
using ReportExporters.Common.Model;
using System.Collections.ObjectModel;
using ReportExporters.Common.Model.Images;

using RSize = ReportExporters.Common.Model.Size;
using ReportExporters.Common.Model.ControlItems;

namespace ReportExporters.Common.Rdlc.Wrapper
{
	internal interface IRdlElement
	{
		void WriteTo(XmlWriter xmlWriter);
	}

	internal class CounterProvider
	{
		internal int DataSourceCounter = 0;
	}

	internal class RdlcElement
	{
		internal CounterProvider Counters;

		public RdlcElement(CounterProvider _counterProvider)
		{
			Counters = _counterProvider;
		}
	}

	internal partial class RdlcWrapper
	{
		internal CounterProvider Counters;

		private RReport report;
		public RReport Report
		{
			get
			{
				return report;
			}
			set
			{
				report = value;
			}
		}

		public RdlcWrapper()
		{
			Counters = new CounterProvider();
			Report = new RReport(this);
		}

		internal partial class RReport : IRdlElement
		{
			#region Report Properties

			private RRect margin;
			public RRect Margin
			{
				get
				{
					return margin;
				}
				set
				{
					margin = value;
				}
			}

			private RSize width = new Unit(6.5, UnitType.Inch);
			public RSize Width
			{
				get
				{
					return width;
				}
				set
				{
					width = value;
				}
			}

			private RSize interactiveHeight = new Unit(11, UnitType.Inch);
			public RSize InteractiveHeight
			{
				get
				{
					return interactiveHeight;
				}
				set
				{
					interactiveHeight = value;
				}
			}

			public RSize interactiveWidth = new Unit(6.5, UnitType.Inch);
			public RSize InteractiveWidth
			{
				get
				{
					return interactiveWidth;
				}
				set
				{
					interactiveWidth = value;
				}
			}

			private REmbeddedImages embeddedImages;
			public REmbeddedImages EmbeddedImages
			{
				get
				{
					return embeddedImages;
				}
				set
				{
					embeddedImages = value;
				}
			}

			private string language = Thread.CurrentThread.CurrentCulture.Name;
			public string Language
			{
				get
				{
					return language;
				}
				set
				{
					language = value;
				}
			}

			#endregion

			private string rdReportID = Guid.NewGuid().ToString();

			#region Designer Properties

			public bool DrawGrid = true;
			public bool SnapToGrid = true;

			#endregion

			#region General Properties

			private RDataSources dataSources;
			public RDataSources DataSources
			{
				get
				{
					return dataSources;
				}
				set
				{
					dataSources = value;
				}
			}

			private RDataSets dataSets;
			public RDataSets DataSets
			{
				get
				{
					return dataSets;
				}
				set
				{
					dataSets = value;
				}
			}

			private RBody body;
			public RBody Body
			{
				get
				{
					return body;
				}
				set
				{
					body = value;
				}
			}

			#endregion

			public RReport(RdlcWrapper rdlcWrapper)
			{
				Margin = new RRect("", "Margin", new Rect(new Unit(1.0, UnitType.Inch)));

				DataSources = new RDataSources();
				DataSets = new RDataSets();
				Body = new RBody();
				EmbeddedImages = new REmbeddedImages();

				DataSources.Add(new RDataSource(rdlcWrapper.Counters));
			}

			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Report");
				{
					// Namespaces
					xmlWriter.WriteAttributeString("xmlns", "", null, "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition");
					xmlWriter.WriteAttributeString("xmlns", "rd", null, "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

					DataSources.WriteTo(xmlWriter);

					Margin.WriteTo(xmlWriter);

					xmlWriter.WriteElementString("rd:ReportID", rdReportID);

					if (EmbeddedImages.Count > 0)
					{
						EmbeddedImages.WriteTo(xmlWriter);
					}

					//design properties
					xmlWriter.WriteElementString("rd:DrawGrid", RdlcWrapper.RdlcValueConverter.GetBoolean(DrawGrid));
					xmlWriter.WriteElementString("rd:SnapToGrid", RdlcWrapper.RdlcValueConverter.GetBoolean(SnapToGrid));

					Body.WriteTo(xmlWriter);

					xmlWriter.WriteElementString("Width", Width.ToString());
					xmlWriter.WriteElementString("InteractiveHeight", InteractiveHeight.ToString());
					xmlWriter.WriteElementString("InteractiveWidth", InteractiveWidth.ToString());

					xmlWriter.WriteElementString("Language", Language);

					DataSets.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		#region SubClasses

		internal class ReportControlItem : IRdlElement
		{
			private static int ReportControlItemsCount = 0;

			private ControlItem CItem;
			/// <summary>
			/// Report Control Item
			/// </summary>
			internal ControlItem Item
			{
				get
				{
					return CItem;
				}
			}

			private int _uniqueIndex;
			protected int UniqueIndex
			{
				get
				{
					return _uniqueIndex;
				}
			}

			protected ReportControlItem(ControlItem controlItem)
			{
				CItem = controlItem;
				_uniqueIndex = ++ReportControlItemsCount;
			}

			public virtual void WriteTo(XmlWriter xmlWriter)
			{
				if (CItem.Width.HasValue)
				{
					xmlWriter.WriteElementString("Width", CItem.Width.ToString());
				}
				if (CItem.Top.HasValue)
				{
					xmlWriter.WriteElementString("Top", CItem.Top.ToString());
				}
				if (CItem.Left.HasValue)
				{
					xmlWriter.WriteElementString("Left", CItem.Left.ToString());
				}
				if (CItem.Height.HasValue)
				{
					xmlWriter.WriteElementString("Height", CItem.Height.ToString());
				}

				if (CItem.Style != null)
				{
					RStyle rStyle = new RStyle(this, CItem.Style);
					rStyle.WriteTo(xmlWriter);
				}
			}
		}

		internal class REmbeddedImage : IRdlElement
		{
			EmbeddedImage EmbImage;

			public REmbeddedImage(EmbeddedImage embeddedImage)
			{
				EmbImage = embeddedImage;
			}

			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("EmbeddedImage");
				{
					xmlWriter.WriteAttributeString("Name", EmbImage.Name);
					xmlWriter.WriteElementString("MIMEType", RdlcValueConverter.GetMIMEType(EmbImage.MIMEType.Value));
					xmlWriter.WriteElementString("ImageData", Convert.ToBase64String(EmbImage.ImageData));
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RReportItems : Collection<ReportControlItem>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("ReportItems");
				{
					foreach (ReportControlItem reportItem in this)
					{
						reportItem.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class REmbeddedImages : Collection<REmbeddedImage>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("EmbeddedImages");
				{
					foreach (REmbeddedImage embeddedImage in this)
					{
						embeddedImage.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class PageHeader : ReportItemCointainer
		{
			private bool printOnFirstPage;
			public bool PrintOnFirstPage
			{
				get { return printOnFirstPage; }
				set { printOnFirstPage = value; }
			}

			private bool printOnLastPage;
			/// <summary>
			/// Not used in singlepage reports.
			/// </summary>
			public bool PrintOnLastPage
			{
				get { return printOnLastPage; }
				set { printOnLastPage = value; }
			}
		}

		internal class PageFooter : ReportItemCointainer
		{
			private bool printOnFirstPage;
			/// <summary>
			/// Not used in singlepage reports.
			/// </summary>
			public bool PrintOnFirstPage
			{
				get { return printOnFirstPage; }
				set { printOnFirstPage = value; }
			}

			private bool printOnLastPage;
			public bool PrintOnLastPage
			{
				get { return printOnLastPage; }
				set { printOnLastPage = value; }
			}
		}

		internal class ReportItemCointainer : IRdlElement
		{
			private RReportItems reportItems;
			public RReportItems ReportItems
			{
				get
				{
					return reportItems;
				}
				set
				{
					reportItems = value;
				}
			}

			private RSize? height;
			public RSize? Height
			{
				get { return height; }
				set { height = value; }
			}

			public ReportItemCointainer()
			{
				this.ReportItems = new RReportItems();
			}

			/// <summary>
			/// Write ReportItems, Height
			/// </summary>
			/// <param name="xmlWriter"></param>
			public virtual void WriteTo(XmlWriter xmlWriter)
			{
				if (ReportItems.Count > 0)
				{
					ReportItems.WriteTo(xmlWriter);
				}
				
				if (Height.HasValue)
				{
					xmlWriter.WriteElementString("Height", Height.ToString());
				}
			}
		}

		internal class RBody : ReportItemCointainer
		{
			/// <summary>
			/// Number of columns for the report
			/// </summary>
			//public int Columns;

			/// <summary>
			/// Spacing between each column in multi-column output. Default: 0.5 in
			/// </summary>
			//public DSize ColumnSpacing;

			public RBody()
				: base()
			{
				Height = new Unit(2, UnitType.Inch);
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Body");
				{
					base.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}
		}

		#endregion

	}
}
