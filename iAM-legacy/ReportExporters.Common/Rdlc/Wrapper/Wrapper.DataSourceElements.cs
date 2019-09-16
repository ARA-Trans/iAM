using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;

namespace ReportExporters.Common.Rdlc.Wrapper
{
	internal partial class RdlcWrapper
	{


		internal class RDataSource : RdlcElement, IRdlElement
		{
			public string Name;
			public string ConnectString;
			public string DataProvider;
			public string DataSourceID = Guid.NewGuid().ToString();

			public RDataSource(CounterProvider _counterProvider) : base(_counterProvider)
			{
				//Name = "DataSource_" + ((Int32)(Counters.DataSourceCounter++)).ToString();
				Name = "DummyDataSource";
				DataProvider = "SQL";
			}

			public RDataSource(string name, string connectString, string dataProvider,
				CounterProvider _counterProvider) : base(_counterProvider)
			{
				Name = name;
				ConnectString = connectString;
				DataProvider = dataProvider;
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("DataSource");
				xmlWriter.WriteAttributeString("Name", Name);
				{
					xmlWriter.WriteStartElement("ConnectionProperties");
					xmlWriter.WriteElementString("ConnectString", ConnectString);
					xmlWriter.WriteElementString("DataProvider", DataProvider);
					xmlWriter.WriteEndElement();
					xmlWriter.WriteElementString("rd:DataSourceID", DataSourceID);
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RDataSources : Collection<RDataSource>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("DataSources");
				{
					foreach (RDataSource dataSource in this)
					{
						dataSource.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

	}
}
