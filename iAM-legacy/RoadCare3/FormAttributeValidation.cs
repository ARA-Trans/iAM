using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DatabaseManager;

namespace RoadCare3
{
	public partial class FormAttributeValidation : Form
	{
		public FormAttributeValidation()
		{
			InitializeComponent();
		}

		private void FormAttributeValidation_Load( object sender, EventArgs e )
		{
			LoadLimits();
		}

		private void LoadLimits()
		{
			TextReader validationFileReader = LoadFile();

			List<string> numericAttributes = GetNumericAtributes();
			List<AttributeValidationData> validationLimits = new List<AttributeValidationData>();
			List<string> missingAttributes = new List<string>();

			string attributeRow;
			dataGridView1.CellValueChanged -= new DataGridViewCellEventHandler( dataGridView1_CellValueChanged );
			while( ( attributeRow = validationFileReader.ReadLine() ) != null )
			{
				string[] attributeCells = attributeRow.Split( '\t' );
				string name = attributeCells[0].Trim();
				if( numericAttributes.Contains( name ) )
				{
					double min;
					double max;

					if( !double.TryParse( attributeCells[1], out min ) )
					{
						min = double.NaN;
					}
					if( !double.TryParse( attributeCells[2], out max ) )
					{
						max = double.NaN;
					}

					AttributeValidationData toAdd = new AttributeValidationData( name, min, max );
					validationLimits.Add( toAdd );
				}
				//else
				//{
				//used to be an attribute, not any more, should probably just ignore this case
				//}
			}

			foreach( string numericAttribute in numericAttributes )
			{
				dataGridView1.Rows.Add();
				DataGridViewRow justAdded = dataGridView1.Rows[dataGridView1.Rows.Count - 1];

				AttributeValidationData attributeLimits = validationLimits.Find( delegate( AttributeValidationData a )
				{
					return a.Name.Trim() == numericAttribute.Trim();
				} );

				if( attributeLimits != null )
				{
					justAdded.Cells["ColumnAttribute"].Value = attributeLimits.Name;

					if( !double.IsNaN( attributeLimits.Min ) )
					{
						justAdded.Cells["ColumnMin"].Value = attributeLimits.Min;
					}
					if( !double.IsNaN( attributeLimits.Max ) )
					{
						justAdded.Cells["ColumnMax"].Value = attributeLimits.Max;
					}
				}
				else
				{
					justAdded.Cells["ColumnAttribute"].Value = numericAttribute;
				}
			}


			dataGridView1.CellValueChanged += new DataGridViewCellEventHandler( dataGridView1_CellValueChanged );
			validationFileReader.Close();
		}

		private List<string> GetNumericAtributes()
		{
			List<string> toReturn = new List<string>();

			string query = "SELECT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE TYPE_ = 'NUMBER' ORDER BY ATTRIBUTE_";
			DataSet numericAttributeData = DBMgr.ExecuteQuery( query );
			foreach( DataRow numericAttributeRow in numericAttributeData.Tables[0].Rows )
			{
				toReturn.Add( numericAttributeRow["ATTRIBUTE_"].ToString() );
			}

			return toReturn;
		}

		private TextReader LoadFile()
		{
			// This won't work on virtual machines.  Not sure what the workaround is.
			string validationFilePath = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) + @"\RoadCare Projects\attributevalidation.txt";
			FileInfo validationFileInfo = new FileInfo( validationFilePath );

			if( !validationFileInfo.Exists )
			{
				if(!validationFileInfo.Directory.Exists)
				{
					validationFileInfo.Directory.Create();
				}
				CreateFile();
			}

			TextReader toReturn = new StreamReader( validationFilePath );
			return toReturn;
		}

		private void CreateFile()
		{
			TextWriter attributeFileWriter = new StreamWriter( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) + @"\RoadCare Projects\attributevalidation.txt" );
			DataSet numericAttributeData = DBMgr.ExecuteQuery( "SELECT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE TYPE_ = 'NUMBER'" );
			foreach( DataRow numericAttributeRow in numericAttributeData.Tables[0].Rows )
			{
				string toWrite = numericAttributeRow[0].ToString() + "\t\t";
				attributeFileWriter.WriteLine( toWrite );
			}
			attributeFileWriter.Close();
		}

		private void buttonOK_Click( object sender, EventArgs e )
		{
			if( Valid() )
			{
				UpdateFile();
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void UpdateFile()
		{
			TextWriter attributeFileWriter = new StreamWriter( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) + @"\RoadCare Projects\attributevalidation.txt" );
			foreach( DataGridViewRow validationRow in dataGridView1.Rows )
			{
				string attributeName = validationRow.Cells[0].Value.ToString();
				string min = validationRow.Cells[1].Value != null ? validationRow.Cells[1].Value.ToString() : "";
				string max = validationRow.Cells[2].Value != null ? validationRow.Cells[2].Value.ToString() : "";
				string toWrite = attributeName + "\t" + min + "\t" + max;
				attributeFileWriter.WriteLine( toWrite );
			}
			attributeFileWriter.Close();
		}

		private bool Valid()
		{
			bool valid = true;
			foreach( DataGridViewRow rowToCheck in dataGridView1.Rows )
			{
				valid = CellValid( rowToCheck.Cells["ColumnMin"] ) && CellValid( rowToCheck.Cells["ColumnMax"] );
				if( !valid )
				{
					break;
				}
			}

			return valid;
		}

		private void buttonCancel_Click( object sender, EventArgs e )
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void dataGridView1_CellValueChanged( object sender, DataGridViewCellEventArgs e )
		{
			if( e.ColumnIndex >= 0 && e.RowIndex >= 0 )
			{
				DataGridViewCell changedCell = dataGridView1[e.ColumnIndex, e.RowIndex];

				if( CellValid( changedCell ) )
				{
					changedCell.Style.BackColor = SystemColors.Window;
				}
				else
				{
					changedCell.Style.BackColor = Color.Red;
				}
			}
		}

		private bool CellValid( DataGridViewCell toTest )
		{
			bool valid = true;
			if( toTest.Value != null )
			{
				string cellValue = toTest.Value.ToString();
				double dummy;
				valid = cellValue == "" || double.TryParse( cellValue, out dummy );
			}

			return valid;
		}
	}
}
