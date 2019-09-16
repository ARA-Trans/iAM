using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Rdlc.Enums;
using System.Xml;
using ReportExporters.Common.Model.Images;
using ReportExporters.Common.Model.ControlItems;

namespace ReportExporters.Common.Rdlc.Wrapper
{


	/// <summary>
	/// Line, Rectangle, Textbox, Image, Subreport, CustomReportItem or DataRegion
	/// </summary>
	internal partial class RdlcWrapper
	{
		internal class RBaseImage : IRdlElement
		{
			private BaseImage CImage;

			internal RBaseImage(BaseImage baseImage)
			{
				this.CImage = baseImage;
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				if (CImage.Sizing.HasValue)
				{
					xmlWriter.WriteElementString("Sizing", CImage.Sizing.Value.ToString());
				}

				if ((CImage.MIMEType.HasValue) && (CImage.Source == ImageSource.Database))
				{
					xmlWriter.WriteElementString("MIMEType",
						RdlcValueConverter.GetMIMEType(CImage.MIMEType.Value));
				}

				xmlWriter.WriteElementString("Source", CImage.Source.ToString());
				//Value
				xmlWriter.WriteElementString("Value", CImage.GetXmlValue());
			}
		}

		internal class RImage : ReportControlItem
		{
			internal CIImageBox ImageBox
			{
				get
				{
					return Item as CIImageBox;
				}
			}

			static RImage()
			{
			}

			#region Properties

			private string _name;
			public string Name
			{
				get { return _name; }
				set { _name = value; }
			}

			private int _zIndex;
			public int ZIndex
			{
				get
				{
					return _zIndex;
				}
			}

			private bool _isTableOrMatrixSubItem;
			public bool IsTableOrMatrixSubItem
			{
				get { return _isTableOrMatrixSubItem; }
			}

			#endregion

			public RImage(string defaultName, bool isTableOrMatrixSubItem,
				string value, ImageProperties imageProperties)
				: base(new CIImageBox(imageProperties, value))
			{
				this.Name = String.Format("image{0}", UniqueIndex);

				this._zIndex = UniqueIndex;
				this._isTableOrMatrixSubItem = isTableOrMatrixSubItem;
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Image");
				{
					xmlWriter.WriteAttributeString("Name", Name);

					if (!IsTableOrMatrixSubItem)
					{
						xmlWriter.WriteElementString("Top", Item.Top.ToString());
						xmlWriter.WriteElementString("Width", Item.Width.ToString());
					}

					if (!IsTableOrMatrixSubItem)
					{
						xmlWriter.WriteElementString("Left", Item.Left.ToString());
						xmlWriter.WriteElementString("Height", Item.Height.ToString());
					}

					if (ImageBox.Action != null)
					{
						RAction rAction = new RAction(ImageBox.Action);
						rAction.WriteTo(xmlWriter);
					}

					//Style
					//base.Write(xmlWriter);
					RStyle rStyle = new RStyle(this, Item.Style);
					rStyle.WriteTo(xmlWriter);

					xmlWriter.WriteElementString("ZIndex", ZIndex.ToString());

					RBaseImage rBaseImage = new RBaseImage(ImageBox.Image);
					rBaseImage.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RTextBox : ReportControlItem
		{
			internal CITextBox TextBox
			{
				get
				{
					return Item as CITextBox;
				}
			}

			private string _name;
			public string Name
			{
				get
				{
					return _name;
				}
			}

			private string _rdDefaultName;
			public string rdDefaultName
			{
				get
				{
					return _rdDefaultName;
				}
			}

			private int _zIndex;
			public int ZIndex
			{
				get
				{
					return _zIndex;
				}
			}

			private RTextBox()
				: base(new CITextBox())
			{
			}

			public RTextBox(string value)
				: this(String.Empty, value)
			{
			}

			public RTextBox(string defaultName, string value)
				: this()
			{
				_name = String.Format("textbox{0}", this.UniqueIndex);
				_rdDefaultName = (String.IsNullOrEmpty(defaultName)) ? Name : defaultName;
				_zIndex = UniqueIndex;
				TextBox.Value = value;
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Textbox");
				{
					xmlWriter.WriteAttributeString("Name", Name);
					xmlWriter.WriteElementString("rd:DefaultName", rdDefaultName);

					if (TextBox.Action != null)
					{
						RAction rAction = new RAction(TextBox.Action);
						rAction.WriteTo(xmlWriter);
					}

					xmlWriter.WriteElementString("ZIndex", ZIndex.ToString());

					//base.Write(xmlWriter);
					RStyle rStyle = new RStyle(this, Item.Style);
					rStyle.WriteTo(xmlWriter);

					xmlWriter.WriteElementString("CanGrow", RdlcWrapper.RdlcValueConverter.GetBoolean(TextBox.CanGrow));
					//xmlWriter.WriteElementString("HideDuplicates", RdlcWrapper.RdlcValueConverter.GetBoolean(TextBox.HideDuplicates));
					xmlWriter.WriteElementString("Value", TextBox.Value);
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RDataRegion : ReportControlItem
		{
			#region Properties

			protected string _name;
			public string Name
			{
				get
				{
					return _name;
				}
			}

			private bool _keepTogether;
			public bool KeepTogether
			{
				get
				{
					return _keepTogether;
				}
				set
				{
					_keepTogether = value;
				}
			}

			private bool _noRows;
			public bool NoRows
			{
				get
				{
					return _noRows;
				}
				set
				{
					_noRows = value;
				}
			}

			private string _dataSetName;
			public string DataSetName
			{
				get
				{
					return _dataSetName;
				}
				set
				{
					_dataSetName = value;
				}
			}

			private bool _pageBreakAtStart;
			public bool PageBreakAtStart
			{
				get
				{
					return _pageBreakAtStart;
				}
				set
				{
					_pageBreakAtStart = value;
				}
			}

			private bool _pageBreakAtEnd;
			public bool PageBreakAtEnd
			{
				get
				{
					return _pageBreakAtEnd;
				}
				set
				{
					_pageBreakAtEnd = value;
				}
			}

			#endregion

			protected RDataRegion()
				: base(new DataRegion())
			{
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				if (!String.IsNullOrEmpty(DataSetName))
					xmlWriter.WriteElementString("DataSetName", DataSetName);

				base.WriteTo(xmlWriter);
			}
		}

		internal class RSubreport : ReportControlItem
		{
			internal CISubreport Subreport
			{
				get
				{
					return Item as CISubreport;
				}
			}

			private string _name;
			public string Name
			{
				get
				{
					return _name;
				}
			}

			public RSubreport(string reportName)
				: base(new CISubreport(reportName))
			{
				_name = String.Format("subreport{0}", this.UniqueIndex);
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Subreport");
				{
					xmlWriter.WriteAttributeString("Name", Name);
					xmlWriter.WriteElementString("ReportName", Subreport.ReportName);
					base.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RRectangle : ReportControlItem
		{
			internal CIRectangle Rectangle
			{
				get
				{
					return Item as CIRectangle;
				}
			}

			private string _name;
			public string Name
			{
				get
				{
					return _name;
				}
			}

			public RReportItems ReportItems;

			private bool? pageBreakAtStart;
			public bool? PageBreakAtStart
			{
				get { return pageBreakAtStart; }
				set { pageBreakAtStart = value; }
			}

			private bool? pageBreakAtEnd;
			public bool? PageBreakAtEnd
			{
				get { return pageBreakAtEnd; }
				set { pageBreakAtEnd = value; }
			}

			public RRectangle()
				: base(new CIRectangle())
			{
				ReportItems = new RReportItems();
				_name = String.Format("rectangle{0}", this.UniqueIndex);
				//_zIndex = UniqueIndex;
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Rectangle");
				{
					xmlWriter.WriteAttributeString("Name", Name);
					
					if (PageBreakAtStart.HasValue)
					{
						xmlWriter.WriteElementString("PageBreakAtStart", RdlcWrapper.RdlcValueConverter.GetBoolean(PageBreakAtStart.Value));
					}
					if (PageBreakAtEnd.HasValue)
					{
						xmlWriter.WriteElementString("PageBreakAtEnd", RdlcWrapper.RdlcValueConverter.GetBoolean(PageBreakAtEnd.Value));
					}
					
					if (ReportItems.Count > 0)
					{
						ReportItems.WriteTo(xmlWriter);
					}
					
					base.WriteTo(xmlWriter);
				}

				xmlWriter.WriteEndElement();
			}
		}

		internal class RLine : ReportControlItem
		{
			protected RLine()
				: base(new Line())
			{
			}
		}

		internal class RList : ReportControlItem
		{
			protected RList()
				: base(new List())
			{
			}
		}

	}
}
