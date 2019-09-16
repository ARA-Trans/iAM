using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model.Style;
using System.Xml;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Globalization;
using ReportExporters.Common.Rdlc.Enums;
using ReportExporters.Common.Model;

namespace ReportExporters.Common.Rdlc.Wrapper
{
	/// <summary>
	/// Expression, Style, Action, Visibility
	/// </summary>
	internal partial class RdlcWrapper
	{
		internal class RExpression
		{
			private string Value;

			internal RExpression(string _value)
			{
				this.Value = _value;
			}

			public static implicit operator string(RExpression d)
			{
				return d.Value;
			}

			public static implicit operator RExpression(string d)
			{
				return new RExpression(d);
			}

			public override string ToString()
			{
				return this.Value;
			}
		}


		/// <summary>
		/// Indicates the ReportItem should not be (initially) shown in the output report.
		/// </summary>
		internal class RVisibility : IRdlElement
		{
			private bool? _isHidden;
			public bool? IsHidden
			{
				get
				{
					return _isHidden;
				}
				set
				{
					_isHidden = value;
				}
			}

			private string _hidden;
			public string Hidden
			{
				get
				{
					return _hidden;
				}
				set
				{
					_hidden = value;
				}
			}

			public RVisibility()
			{
				_isHidden = false;
			}

			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Visibility");
				{
					xmlWriter.WriteStartElement("Hidden");
					{
						if (!String.IsNullOrEmpty(Hidden))
						{
							xmlWriter.WriteValue(Hidden);
						}
						else
						{
							xmlWriter.WriteValue(IsHidden);
						}
					}
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		/// <summary>
		/// An action (e.g. a hyperlink) associated with the ReportItem
		/// </summary>
		internal class RAction : IRdlElement
		{
			private ReportExporters.Common.Model.Action CAction;
			public ReportExporters.Common.Model.Action Action
			{
				get
				{
					return CAction;
				}
			}

			public RAction(ReportExporters.Common.Model.Action action)
			{
				CAction = action;
			}

			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Action");
				{
					xmlWriter.WriteElementString("Hyperlink", CAction.Hyperlink);
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		/// <summary>
		/// Restore spaces & dashes for Enums values
		/// </summary>
		internal class RConverter
		{
			internal static string ToString(object o)
			{
				string strObj = o.ToString();
				string toRet = strObj.Replace("__", " ").Replace("_", "-");
				return toRet;
			}
		}
	}
}
