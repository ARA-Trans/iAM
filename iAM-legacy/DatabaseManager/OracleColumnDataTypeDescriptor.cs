using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseManager
{
	public enum OracleDataType
	{
		VARCHAR2,
		NVARCHAR2,
		CHAR,
		NCHAR,
		NUMBER,
		DATE,
		TIMESTAMP,
		CLOB,
		NCLOB,
		BLOB
	}

	public class OracleColumnDataTypeDescriptor : ColumnDataTypeDescriptor
	{
		//string typeName;
		//string quantityString;

		public OracleColumnDataTypeDescriptor( OracleDataType typeSpecifier )
		{
			switch( typeSpecifier )
			{
				case OracleDataType.BLOB:
					throw new NotImplementedException();
					//break;
				case OracleDataType.CHAR:
					throw new ArgumentException( "ERROR: insufficient quantity specification for this data type." );
					//break;
				case OracleDataType.CLOB:
					throw new NotImplementedException();
					//break;
				case OracleDataType.DATE:
					throw new NotImplementedException();
					//break;
				case OracleDataType.NCHAR:
					throw new ArgumentException( "ERROR: insufficient quantity specification for this data type." );
					//break;
				case OracleDataType.NCLOB:
					throw new NotImplementedException();
					//break;
				case OracleDataType.NUMBER:
					throw new ArgumentException( "ERROR: insufficient quantity specification for this data type." );
					//break;
				case OracleDataType.NVARCHAR2:
					throw new ArgumentException( "ERROR: insufficient quantity specification for this data type." );
					//break;
				case OracleDataType.TIMESTAMP:
					throw new NotImplementedException();
					//break;
				case OracleDataType.VARCHAR2:
					throw new ArgumentException( "ERROR: insufficient quantity specification for this data type." );
					//break;
			}

		}

		public OracleColumnDataTypeDescriptor( OracleDataType typeSpecifier, int quantity )
		{
			switch( typeSpecifier )
			{
				case OracleDataType.BLOB:
					throw new ArgumentException( "ERROR: quantity specificied for a data type that does not require it." );
					//break;
				case OracleDataType.CHAR:
					throw new NotImplementedException();
					//break;
				case OracleDataType.CLOB:
					throw new ArgumentException( "ERROR: quantity specificied for a data type that does not require it." );
					//break;
				case OracleDataType.DATE:
					throw new ArgumentException( "ERROR: quantity specificied for a data type that does not require it." );
					//break;
				case OracleDataType.NCHAR:
					throw new NotImplementedException();
					//break;
				case OracleDataType.NCLOB:
					throw new NotImplementedException();
					//break;
				case OracleDataType.NUMBER:
					throw new ArgumentException( "ERROR: insufficient quantity specification for this data type." );
					//break;
				case OracleDataType.NVARCHAR2:
					throw new NotImplementedException();
					//break;
				case OracleDataType.TIMESTAMP:
					throw new ArgumentException( "ERROR: quantity specificied for a data type that does not require it." );
					//break;
				case OracleDataType.VARCHAR2:
					throw new NotImplementedException();
					//break;
			}
		}

		/// <summary>
		/// For the specification of a Number column
		/// </summary>
		/// <param name="typeSpecifier"></param>
		/// <param name="precision"></param>
		/// <param name="precision"></param>
		public OracleColumnDataTypeDescriptor( OracleDataType typeSpecifier, int quantity, int precision )
		{
			switch( typeSpecifier )
			{
				case OracleDataType.BLOB:
					throw new ArgumentException( "ERROR: quantity specificied for a data type that does not require it." );
					//break;
				case OracleDataType.CHAR:
					throw new ArgumentException( "ERROR: precision specificied for a data type that does not require it." );
					//break;
				case OracleDataType.CLOB:
					throw new ArgumentException( "ERROR: quantity specificied for a data type that does not require it." );
					//break;
				case OracleDataType.DATE:
					throw new ArgumentException( "ERROR: quantity specificied for a data type that does not require it." );
					//break;
				case OracleDataType.NCHAR:
					throw new ArgumentException( "ERROR: precision specificied for a data type that does not require it." );
					//break;
				case OracleDataType.NCLOB:
					throw new ArgumentException( "ERROR: precision specificied for a data type that does not require it." );
					//break;
				case OracleDataType.NUMBER:
					throw new NotImplementedException();
					//break;
				case OracleDataType.NVARCHAR2:
					throw new ArgumentException( "ERROR: precision specificied for a data type that does not require it." );
					//break;
				case OracleDataType.TIMESTAMP:
					throw new ArgumentException( "ERROR: quantity specificied for a data type that does not require it." );
					//break;
				case OracleDataType.VARCHAR2:
					throw new ArgumentException( "ERROR: precision specificied for a data type that does not require it." );
					//break;
			}
		}

		#region ColumnDataTypeDescriptor Members

		/// <summary>
		/// The text string to be used for specification of the column name
		/// </summary>
		public string Name
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// The text string to be used for specification of the column precision
		/// </summary>
		public string Quantifier
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
