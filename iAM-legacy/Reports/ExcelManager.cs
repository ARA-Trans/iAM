using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

namespace Reports
{
	public class ExcelManager : IDisposable
	{
		Application excelInstance;
		Dictionary<string, Workbook> workbookIndex;

		public ExcelManager()
		{
			excelInstance = new Application();
			workbookIndex = new Dictionary<string, Workbook>();
		}

		/// <summary>
		/// Starts the excel manager, opens an existing workbook, and associates a name with it.
		/// </summary>
		/// <param name="filename">The path to the excel file you wish to open.</param>
		/// <param name="workbookName"></param>
		public ExcelManager( string filename, string workbookName )
		{
			excelInstance = new Application();
			OpenWorkbook( filename, workbookName );
			workbookIndex = new Dictionary<string, Workbook>();
		}

		/// <summary>
		/// Opens an existing workbook and associates a name with it.
		/// </summary>
		/// <param name="filename">The path to the excel file you wish to open.</param>
		/// <param name="workbookName">The name you wish to give to the workbook so that you may reference it later.</param>
		public void OpenWorkbook( string filename, string workbookName )
		{
			if( !workbookIndex.ContainsKey( workbookName ) )
			{
				Workbook wbToAdd = null;
				try
				{
					wbToAdd = excelInstance.Workbooks.Open( filename,
						Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
						Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value );

				}
				catch( Exception ex )
				{
					ArgumentException exceptionToThrow = new ArgumentException( "Error opening " + filename + ".", ex );
					throw exceptionToThrow;
				}
				workbookIndex.Add( workbookName, wbToAdd );
			}
			else
			{
				throw new ArgumentException( "Error: A workbook with that name already exists.  You will be prompted by Excel for a new save location." );
			}
		}

		public void CloseWorkbook( string workbookName )
		{
			if( workbookIndex.ContainsKey( workbookName ) )
			{
				workbookIndex[workbookName].Close(true, Missing.Value, Missing.Value);
				System.Runtime.InteropServices.Marshal.ReleaseComObject( workbookIndex[workbookName] );
				workbookIndex.Remove( workbookName );
			}
			else
			{
				throw new ArgumentException( "Workbook of the name (" + workbookName + ") could not be found." );
			}
		}

		/// <summary>
		/// Creates a new workbook using the default name.
		/// </summary>
		public void CreateNewWorkbook()
		{
			CreateNewWorkbook(ReportGlobals.defaultWorkbookName);
		}

		/// <summary>
		/// Creates a new workbook and associates a name with it.
		/// </summary>
		/// <param name="workbookName">The name you wish to give to the workbook so that you may reference it later.</param>
		public void CreateNewWorkbook( string workbookName )
		{
			if( !workbookIndex.ContainsKey( workbookName ) )
			{
				Workbook wbToAdd = excelInstance.Workbooks.Add( Missing.Value );
				workbookIndex.Add( workbookName, wbToAdd );
			}
			else
			{
				throw new ArgumentException( "Error: A workbook with the name (" + workbookName + ") already exists." );
			}
		}

		/// <summary>
		/// Saves a workbook.  Will prompt for location if this workbook is new.
		/// </summary>
		public void SaveWorkbook()
		{
			SaveWorkbook(ReportGlobals.defaultWorkbookName);
		}
		/// <summary>
		/// Saves a workbook.  Will prompt for location if this workbook is new.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		public void SaveWorkbook( string workbookName )
		{
			workbookIndex[workbookName].Save();
		}
		/// <summary>
		/// Saves a workbook at the specified filepath.
		/// </summary>
		/// <param name="fileName">The location where the workbook is to be saved.</param>
		public void SaveWorkbookAs(string fileName)
		{
			SaveWorkbookAs(ReportGlobals.defaultWorkbookName, fileName);
		}
		/// <summary>
		/// Saves a workbook at the specified filepath.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="fileName">The location where the workbook is to be saved.</param>
		public void SaveWorkbookAs( string workbookName, string fileName )
		{
			try
			{
				using( FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None) )
				{
					fs.Close();
				}
				workbookIndex[workbookName].SaveAs(fileName,
					Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, Missing.Value, Missing.Value, false, false, XlSaveAsAccessMode.xlNoChange,
					Missing.Value, false, false, false, true);
			}
			catch( Exception )
			{
				throw new ArgumentException(fileName + " is currently in use by another process.");
			}
		}

		/// <summary>
		/// Changes the background color of the specified cells on the default sheet of the default workbook.
		/// </summary>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="backColor">The color to change the background to.</param>
		public void ChangeBackgroundColor(string cellSelection, Color backColor)
		{
			ChangeBackgroundColor(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection, backColor);
		}
		/// <summary>
		/// Changes the background color of the specified cells on the specified sheet of the specified workbook.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="backColor">The color to change the background to.</param>
		public void ChangeBackgroundColor( string workbookName, int sheetIndex, string cellSelection, Color backColor )
		{
			List<Range> rangesToAlter = GetRanges( workbookName, sheetIndex, cellSelection );
			foreach( Range rangeToAlter in rangesToAlter )
			{
				rangeToAlter.Interior.Color = ColorTranslator.ToWin32( backColor );
			}
		}
		/// <summary>
		/// Changes the font of the specified cells on the default sheet of the default workbook.
		/// </summary>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="fontName">The font to change the cells to.</param>
		public void ChangeFont(string cellSelection, string fontName)
		{
			ChangeFont(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection, fontName);
		}
		/// <summary>
		/// Changes the font of the specified cells on the specified sheet of the specified workbook.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="fontName">The font to change the cells to.</param>
		public void ChangeFont( string workbookName, int sheetIndex, string cellSelection, string fontName )
		{
			List<Range> rangesToAlter = GetRanges( workbookName, sheetIndex, cellSelection );
			foreach( Range rangeToAlter in rangesToAlter )
			{
				rangeToAlter.Font.Name = fontName;
			}
		}

		/// <summary>
		/// Changes the font size of the specified cells on the default sheet of the default workbook.
		/// </summary>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="fontSize">The font size to change the cells to.</param>
		public void ChangeFontSize(string cellSelection, int fontSize)
		{
			ChangeFontSize(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection, fontSize);
		}
		/// <summary>
		/// Changes the font size of the specified cells on the specified sheet of the specified workbook.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="fontSize">The font size to change the cells to.</param>
		public void ChangeFontSize( string workbookName, int sheetIndex, string cellSelection, int fontSize )
		{
			List<Range> rangesToAlter = GetRanges( workbookName, sheetIndex, cellSelection );
			foreach( Range rangeToAlter in rangesToAlter )
			{
				rangeToAlter.Font.Size = fontSize;
			}
		}
		/// <summary>
		/// Changes the font color of the specified cells on the default workbook.
		/// </summary>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="foreColor">The color to change the font to.</param>
		public void ChangeForegroundColor(string cellSelection, Color foreColor)
		{
			ChangeForegroundColor(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection, foreColor);
		}
		/// <summary>
		/// Changes the font color of the specified cells on the specified workbook.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="foreColor">The color to change the font to.</param>
		public void ChangeForegroundColor( string workbookName, int sheetIndex, string cellSelection, Color foreColor )
		{
			List<Range> rangesToAlter = GetRanges( workbookName, sheetIndex, cellSelection );
			foreach( Range rangeToAlter in rangesToAlter )
			{
				rangeToAlter.Font.Color = ColorTranslator.ToWin32( foreColor );
			}
		}
		/// <summary>
		/// Sets the horizontal alignment for the specified cells on the default sheet of the default workbook.
		/// </summary>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="alignment">The horizontal justication enumeration.</param>
		public void ChangeHorizontalAlignment(string cellSelection, Microsoft.Office.Interop.Excel.XlHAlign alignment)
		{
			ChangeHorizontalAlignment(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection, alignment);
		}
		/// <summary>
		/// Sets the horizontal alignment for the specified cells on the specified sheet of the specified workbook.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="alignment">The horizontal justication enumeration.</param>
		public void ChangeHorizontalAlignment( string workbookName, int sheetIndex, string cellSelection, Microsoft.Office.Interop.Excel.XlHAlign alignment )
		{
			List<Range> rangesToAlter = GetRanges( workbookName, sheetIndex, cellSelection );
			foreach( Range rangeToAlter in rangesToAlter )
			{
				rangeToAlter.HorizontalAlignment = alignment;
			}
		}
		/// <summary>
		/// Sets the vertical alignment for the specified cells on the default sheet of the default workbook.
		/// </summary>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="alignment">The vertical justication enumeration.</param>
		public void ChangeVerticalAlignment(string cellSelection, Microsoft.Office.Interop.Excel.XlVAlign alignment)
		{
			ChangeVerticalAlignment(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection, alignment);
		}
		/// <summary>
		/// Sets the vertical alignment for the specified cells on the specified sheet of the specified workbook.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="alignment">The vertical justication enumeration.</param>
		public void ChangeVerticalAlignment( string workbookName, int sheetIndex, string cellSelection, Microsoft.Office.Interop.Excel.XlVAlign alignment )
		{
			List<Range> rangesToAlter = GetRanges( workbookName, sheetIndex, cellSelection );
			foreach( Range rangeToAlter in rangesToAlter )
			{
				rangeToAlter.VerticalAlignment = alignment;
			}
		}
		/// <summary>
		/// Toggles a border of the specified cells and applies the settings if it is toggled on.
		/// </summary>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="borderToToggle">Indicates the border that will be toggled.</param>
		/// <param name="weightToSet">Indicates the weight the border will be set to if it is being toggled on.</param>
		/// <param name="styleToSet">Indicates the line style the border will be set to if it is being toggled on.</param>
		public void SetBorders(string cellSelection, XlBordersIndex bordersToToggle, Microsoft.Office.Interop.Excel.XlBorderWeight weightToSet, XlLineStyle styleToSet)
		{
			SetBorders(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection, bordersToToggle, weightToSet, styleToSet);
		}
		/// <summary>
		/// Toggles a border of the specified cells and applies the settings if it is toggled on.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="borderToToggle">Indicates the border that will be toggled.</param>
		/// <param name="weightToSet">Indicates the weight the border will be set to if it is being toggled on.</param>
		/// <param name="styleToSet">Indicates the line style the border will be set to if it is being toggled on.</param>
		public void SetBorders( string workbookName, int sheetIndex, string cellSelection, XlBordersIndex borderToToggle, Microsoft.Office.Interop.Excel.XlBorderWeight weightToSet, XlLineStyle styleToSet )
		{
			List<Range> rangesToAlter = GetRanges( workbookName, sheetIndex, cellSelection );
			foreach( Range rangeToAlter in rangesToAlter )
			{
				if( (XlLineStyle)rangeToAlter.Borders[borderToToggle].LineStyle != XlLineStyle.xlLineStyleNone )
				{
					rangeToAlter.Borders[borderToToggle].LineStyle = XlLineStyle.xlLineStyleNone;
				}
				else
				{
					rangeToAlter.Borders[borderToToggle].LineStyle = styleToSet;
					rangeToAlter.Borders[borderToToggle].Weight = weightToSet;
				}
			}
		}
		/// <summary>
		/// Merges the designated cells.
		/// </summary>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="splitByRow">Specifies whether the rows of the selection should be independently merged.</param>
		public void MergeCells(string cellSelection, bool splitByRow)
		{
			MergeCells(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection, splitByRow);
		}
		/// <summary>
		/// Merges the designated cells.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="cellSelection">Indicates the cells to be changed.  Works just like Excel.  Example: "A:A,1:1,A2:B3,F4,F5"</param>
		/// <param name="splitByRow">Specifies whether the rows of the selection should be independently merged.</param>
		public void MergeCells( string workbookName, int sheetIndex, string cellSelection, bool splitByRow )
		{
			List<Range> rangesToAlter = GetRanges( workbookName, sheetIndex, cellSelection );
			foreach( Range rangeToAlter in rangesToAlter )
			{
				rangeToAlter.Merge( splitByRow );
			}
		}
		/// <summary>
		/// Inserts an image into the default sheet of the default workbook.
		/// </summary>
		/// <param name="imagePath">The path to the image file to be inserted.</param>
		public void InsertImage(string imagePath)
		{
			InsertImage(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, imagePath);
		}
		/// <summary>
		/// Inserts an image into the specifed sheet of the specified workbook.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="imagePath">The path to the image file to be inserted.</param>
		public void InsertImage( string workbookName, int sheetIndex, string imagePath )
		{
			InsertImage( workbookName, sheetIndex, "A1", imagePath );
		}
		/// <summary>
		/// Inserts an image into upper left hand corner (plus offset) of the default sheet of the default workbook.
		/// </summary>
		/// <param name="imageLocation">The cell into which the picture will be pasted.</param>
		/// <param name="imagePath">The path to the image file to be inserted.</param>
		public void InsertImage(string imageLocation, string imagePath)
		{
			InsertImage(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, imageLocation, imagePath);
		}
		/// <summary>
		/// Inserts an image into upper left hand corner (plus offset) of the specifed sheet of the specified workbook.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="imageLocation">The cell into which the picture will be pasted.</param>
		/// <param name="imagePath">The path to the image file to be inserted.</param>
		public void InsertImage( string workbookName, int sheetIndex, string imageLocation, string imagePath )
		{
			Worksheet operationalSheet = ( Worksheet )( workbookIndex[workbookName].Sheets[sheetIndex] );
			Range operationalRange = operationalSheet.get_Range( imageLocation, Missing.Value );

			Image pictureToPaste = Image.FromFile( imagePath, true );
			System.Windows.Forms.Clipboard.SetDataObject( pictureToPaste, false );
			operationalSheet.Paste( operationalRange, pictureToPaste );
		}
		/// <summary>
		/// Inserts a column in front of the specified column on the default sheet of the default workbook.
		/// </summary>
		/// <param name="beforeColumn">The column behind which the new column will be placed. Example: "C"</param>
		public void InsertColumn(string beforeColumn)
		{
			InsertColumn(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, beforeColumn);
		}
		/// <summary>
		/// Inserts a column in front of the specified column on the specified sheet of the specified workbook.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="beforeColumn">The column behind which the new column will be placed. Example: "C"</param>
		public void InsertColumn( string workbookName, int sheetIndex, string beforeColumn )
		{
			Worksheet operationalSheet = ( Worksheet )( workbookIndex[workbookName].Sheets[sheetIndex] );
			Range columnToInsertInFrontOf = ( ( Range )operationalSheet.Cells[0, beforeColumn] ).EntireColumn;
			columnToInsertInFrontOf.Insert(Missing.Value, Missing.Value);
		}
		/// <summary>
		/// Inserts a row in front of the specified row on the default sheet of the default workbook.
		/// </summary>
		/// <param name="beforeRow">The row above which the new row  will be placed.  </param>
		public void InsertRow(int beforeRow)
		{
			InsertRow(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, beforeRow);
		}
		/// <summary>
		/// Inserts a row in front of the specified row on the specified sheet of the specified workbook.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="beforeRow">The row above which the new row  will be placed.  </param>
		public void InsertRow( string workbookName, int sheetIndex, int beforeRow )
		{
			Worksheet operationalSheet = ( Worksheet )( workbookIndex[workbookName].Sheets[sheetIndex] );
			Range columnToInsertInFrontOf = ( ( Range )operationalSheet.Cells[beforeRow, "A"] ).EntireRow;
			columnToInsertInFrontOf.Insert( Missing.Value, Missing.Value );
		}
		/// <summary>
		/// Inserts cells at the specified locations.
		/// </summary>
		/// <param name="cellSelection">Indicates the cells to insert on.</param>
		/// <param name="shiftDown">Specifies whether to shift cells down (true) or right (false).</param>
		public void InsertCell(string cellSelection, bool shiftDown)
		{
			InsertCell(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection, shiftDown);
		}
		/// <summary>
		/// Inserts cells at the specified locations.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="cellSelection">Indicates the cells to insert on.</param>
		/// <param name="shiftDown">Specifies whether to shift cells down (true) or right (false).</param>
		public void InsertCell( string workbookName, int sheetIndex, string cellSelection, bool shiftDown )
		{
			List<Range> listofCells = GetRanges( workbookName, sheetIndex, cellSelection );

			foreach( Range rangeToInsertOn in listofCells )
			{
				if( shiftDown )
				{
					rangeToInsertOn.Insert( XlInsertShiftDirection.xlShiftDown, Missing.Value );
				}
				else
				{
					rangeToInsertOn.Insert( XlInsertShiftDirection.xlShiftToRight, Missing.Value );
				}
			}
		}
		/// <summary>
		/// Sets the values of blocks of cells.
		/// </summary>
		/// <param name="cellSelection">Indicates the cells to paste the data into.</param>
		/// <param name="values">A two dimensional array of values to be pasted.  The left dimension is the rows  The right dimension is the column.</param>
		public void SetValues(string cellSelection, object[,] values)
		{
			SetValues(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection, values);
		}
		/// <summary>
		/// Sets the values of blocks of cells.
		/// </summary>
		/// <param name="workbookName">The stored index of the workbook.</param>
		/// <param name="sheetIndex">The sheet number you wish to alter (note: excel sheets begin with number 1, not 0).</param>
		/// <param name="cellSelection">Indicates the cells to paste the data into.</param>
		/// <param name="values">A two dimensional array of values to be pasted.  The left dimension is the rows  The right dimension is the column.</param>
		public void SetValues( string workbookName, int sheetIndex, string cellSelection, object[,] values )
		{
			//"So the long winded moral of the story (which is mainly an excuse to talk about a little of the
			//inner workings that are going on in COM interop when you write this code) is that when you
			//want to set a range of values to an array, you must declare that array as a 2 dimensional array
			//where the left-most dimension is the number of rows you are going to set and the right-most
			//dimension is the number of columns you are going to set.  Even if you are just setting one
			//column, you can’t create a 1 dimensional array and have it work."

			List<Range> rangesToAlter = GetRanges( workbookName, sheetIndex, cellSelection );
			foreach( Range rangeToAlter in rangesToAlter )
			{
				rangeToAlter.Value2 = values;
			}

		}
		/// <summary>
		/// Auto sizes the columns in the parameter string 
		/// </summary>
		/// <param name="columnsToAutoSize">Should be of the form "col1:col2"</param>
		/// <param name="workbookName">The name of the workbook</param>
		/// <param name="sheetIndex">The sheet index</param>
		public void AutoSizeColumns(string columnsToAutoSize, string workbookName, int sheetIndex)
		{
			Worksheet operationalSheet = (Worksheet)(workbookIndex[workbookName].Sheets[sheetIndex]);
			Range autoSizeColumns = ((List<Range>)GetRanges(columnsToAutoSize))[0];
			autoSizeColumns.AutoFit();
		}
		/// <summary>
		/// Auto sizes the columns in the parameter string
		/// </summary>
		/// <param name="columnsToAutoSize">Should be of the form "col1:col2"</param>
		public void AutoSizeColumns(string columnsToAutoSize)
		{
			AutoSizeColumns(columnsToAutoSize, ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex);
		}
		/// <summary>
		/// Changes the back color of EVERY OTHER column from a set of given columns, starting with the first highlighted.
		/// </summary>
		/// <param name="startingColumn">The first column to highlight</param>
		/// <param name="totalPossibleColumns">The total number of columns in the highlighting range</param>
		/// <param name="startRowNumber">The start row to highlight</param>
		/// <param name="endRowNumber">The end row to highlight</param>
		/// <param name="highlightColor">The color of the highlighting</param>
		public void HighlightRanges(char startingColumn, int totalPossibleColumns, string startRowNumber, string endRowNumber, Color highlightColor)
		{
			string columnsToHighlight = "";
			int asciiNum = Convert.ToInt32(startingColumn);
			for (int i = 0; i < totalPossibleColumns; i++)
			{
				if (i % 2 == 0)
				{
					char columnLetter = Convert.ToChar(asciiNum + i);
					columnsToHighlight += columnLetter + startRowNumber + ":" + columnLetter + endRowNumber + ",";
				}
			}
			columnsToHighlight = columnsToHighlight.Substring(0, columnsToHighlight.Length - 1);
			ReportGlobals.XLMgr.ChangeBackgroundColor(columnsToHighlight, highlightColor);
		}
		/// <summary>
		/// Gives titles to the x and y axes of a given chart
		/// </summary>
		/// <param name="xAxisTitle">x axis title</param>
		/// <param name="yAxisTitle">y axis title</param>
		/// <param name="axesToTitle">Chart to label with x and y axes.</param>
		public void SetXYAxisTitles(string xAxisTitle, string yAxisTitle, Chart axesToTitle)
		{
			Axis xAxis = (Axis)axesToTitle.Axes(Microsoft.Office.Interop.Excel.XlAxisType.xlCategory,
					XlAxisGroup.xlPrimary);
			xAxis.HasTitle = true;
			xAxis.AxisTitle.Text = xAxisTitle;

			Axis yAxis = (Axis)axesToTitle.Axes(Microsoft.Office.Interop.Excel.XlAxisType.xlValue, Microsoft.Office.Interop.Excel.XlAxisGroup.xlPrimary);
			yAxis.HasTitle = true;
			yAxis.AxisTitle.Orientation = Microsoft.Office.Interop.Excel.XlOrientation.xlHorizontal;
			yAxis.AxisTitle.Text = yAxisTitle;
		}
		/// <summary>
		/// Gets the chart in this workbook, on this sheet, with the given name.
		/// </summary>
		/// <param name="workbookName">this workbook name</param>
		/// <param name="sheetIndex">this sheet index</param>
		/// <param name="chartName">Name of the chart to get.</param>
		/// <returns>Chart to manipulate.</returns>
		public Chart GetChart(string workbookName, int sheetIndex, string chartName)
		{
			Worksheet operationalSheet = (Worksheet)(workbookIndex[workbookName].Sheets[sheetIndex]);
			ChartObject chartObjs = (ChartObject)operationalSheet.ChartObjects(chartName);
			return chartObjs.Chart;
		}
		public Chart GetChart(string chartName)
		{
			return (GetChart(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, chartName));
		}
		/// <summary>
		/// Set the series title for a given chart
		/// </summary>
		/// <param name="seriesTitle">Series title</param>
		/// <param name="chartToTitle">The chart object to which the series title belongs</param>
		public void SetSeriesTitle(string seriesTitle, string chartName, string seriesName)
		{
			Chart toTitle = GetChart(chartName);
			toTitle.HasLegend = false;
			//Axis series = (Axis)toTitle.Axes(XlAxisType.xlSeriesAxis, XlAxisGroup.xlPrimary;
			//series.HasTitle = true;
			//series.AxisTitle.Text = "TEST";
		}
		/// <summary>
		/// Creates a 2-DIMENSIONAL vertical bar chart using the specified data range.
		/// </summary>
		/// <param name="dataRange">The data to use in the vertical bar chart must represent the x and y axes</param>
		/// <param name="startCol">The letter of the column the left side of the chart should start on.</param>
		/// <param name="top">The topmost pixel of the chart</param>
		/// <param name="width">The total width of the chart in pixels</param>
		/// <param name="height">The total height of the chart in pixels</param>
		/// <param name="rowCol">The XL row/col datatype to use?</param>
		/// <param name="uniqueName">Unique chart name for this worksheet</param>
		public string CreateVerticalBarChart(string dataRange, char startCol, double top, double width, double height, Microsoft.Office.Interop.Excel.XlRowCol rowCol)
		{
			Range vertBarChartData = ((Range)(ReportGlobals.XLMgr.GetRanges(dataRange))[0]);
			// Get the active sheet
			Worksheet operationalSheet = (Worksheet)(workbookIndex[ReportGlobals.defaultWorkbookName].Sheets[ReportGlobals.defaultSheetIndex]);
			ChartObjects chartObjs = (ChartObjects)operationalSheet.ChartObjects(Type.Missing);

			// Get the number of pixels from the left we are going to count to get to this column
			int asciiColumn = Convert.ToInt32(startCol);
			int asciiA = Convert.ToInt32('A');
			int totalPixels = 0;
			asciiColumn = 0 + asciiColumn;
			for (int i = asciiA; i < asciiColumn; i++)
			{
				totalPixels += asciiColumn;
			}
			ChartObject chartObj = chartObjs.Add((double)totalPixels, top, width, height);
			Chart xlChart = chartObj.Chart;
			
			xlChart.SetSourceData(vertBarChartData, rowCol);
			xlChart.ChartType = Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered;
			return chartObj.Name;
		}
		public void AdjustRowHeight(string startRow, string endRow, int newRowHeight)
		{
			AdjustRowHeight(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, startRow, endRow, newRowHeight);
		}
		public void AdjustRowHeight(string workbookName, int sheetIndex, string startRow, string endRow, int newRowHeight)
		{
			Worksheet operationalWorksheet = (Worksheet)workbookIndex[workbookName].Sheets[sheetIndex];
			((Range)operationalWorksheet.Rows[startRow + ":" + endRow, Missing.Value]).RowHeight = newRowHeight;
		}
		public void AdjustColumnWidth(string startColumn, string endColumn, int newColumnWidth)
		{
			AdjustColumnWidth(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, startColumn, endColumn, newColumnWidth);
		}
		public void AdjustColumnWidth(string workbookName, int sheetIndex, string startColumn, string endColumn, int newColumnWidth)
		{
			Worksheet operationalWorksheet = (Worksheet)workbookIndex[workbookName].Sheets[sheetIndex];
			((Range)operationalWorksheet.Columns[startColumn + ":" + endColumn, Missing.Value]).ColumnWidth = newColumnWidth;
		}
		private List<Range> GetRanges(string cellSelection)
		{
			return GetRanges(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection);
		}
		private List<Range> GetRanges( string workbookName, int sheetIndex, string cellSelection )
		{
			Worksheet operationalSheet = ( Worksheet )( workbookIndex[workbookName].Sheets[sheetIndex] );
			List<Range> applicableRanges = new List<Range>();
			string[] independantSections = cellSelection.ToUpper().Split( ',' );
			foreach( string section in independantSections )
			{
				applicableRanges.Add( ParseRange( operationalSheet, section ));
			}
			return applicableRanges;
		}
		/// <summary>
		/// Creates a header with some standard formatting in the cells given in cellSelection
		/// </summary>
		/// <param name="title">The name to give the sub header. Note the title will only fill the first row.</param>
		/// <param name="cellSelection">The cells to fill with the header</param>
		/// <returns></returns>
		public string CreateSubHeader(string title, string cellSelection)
		{
			//This is completely the wrong place for this function.

			string lastRow = "";
			object[,] headerData = new object[2, 1];
			headerData[0, 0] = title;
			ReportGlobals.XLMgr.SetValues(cellSelection, headerData);
			ReportGlobals.XLMgr.MergeCells(cellSelection, false);
			ReportGlobals.XLMgr.ChangeBackgroundColor(cellSelection, Color.FromArgb(0, 0, 211));
			ReportGlobals.XLMgr.ChangeHorizontalAlignment(cellSelection, XlHAlign.xlHAlignCenter);
			ReportGlobals.XLMgr.ChangeVerticalAlignment(cellSelection, XlVAlign.xlVAlignCenter);
			ReportGlobals.XLMgr.ChangeFontSize(cellSelection, 15);
			ReportGlobals.XLMgr.SetBorders(cellSelection, XlBordersIndex.xlEdgeBottom, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
			return lastRow;
		}
		public void SetNumberFormat( string cellSelection, string formatString )
		{
			SetNumberFormat(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, cellSelection, formatString);
		}

	
		public void SetNumberFormat( string workbookName, int sheetIndex, string cellSelection, string formatString )
		{
			List<Range> rangesToAlter = GetRanges( workbookName, sheetIndex, cellSelection );
			foreach( Range rangeToAlter in rangesToAlter )
			{
				rangeToAlter.NumberFormat = formatString;
			}
		}

		private Range ParseRange( Worksheet operationalSheet, string section )
		{
			Regex columnParser = new Regex( @"[A-Z]+" );
			Regex rowParser = new Regex( @"[0-9]+" );

			Range toAdd = null;
			if( section.Contains( ':' ) ) //we are dealing with a range
			{
				string[] bounds = section.Split( ':' );
				string start = bounds[0];
				string end = bounds[1];
				if( columnParser.IsMatch( start ))
				{
					if( rowParser.IsMatch( start ) )
					{
						//since the designation has both a row and a column, we are dealing with a vanilla range
						toAdd = operationalSheet.get_Range( start, end );
					}
					else
					{
						start = start + "1";
						end = end + "1";
						toAdd = operationalSheet.get_Range( start, end ).EntireColumn;
					}
				}
				else
				{
					//if the designation doesn't begin with a letter, then we can only be selecting whole rows
					start = "A" + start;
					end = "A" + end;
					toAdd = operationalSheet.get_Range( start, end ).EntireRow;
				}
			}
			else
			{

				//if the designation doesn't contain a colon, we're dealing with a single cell
				toAdd = operationalSheet.get_Range(section, section);
			}

			return toAdd;
		}

		#region IDisposable Members

		public void Dispose()
		{
			excelInstance.Workbooks.Close();
			System.Runtime.InteropServices.Marshal.ReleaseComObject( excelInstance.Workbooks );
			excelInstance.Quit();
			System.Runtime.InteropServices.Marshal.ReleaseComObject( excelInstance );
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		#endregion
		public void FormatAllCellsAsText()
		{
			FormatAllCellsAsText(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex);
		}
		public void FormatAllCellsAsText(string workbookName, int sheetIndex)
		{
			Worksheet operationalWorksheet = (Worksheet)workbookIndex[workbookName].Sheets[sheetIndex];
			operationalWorksheet.Cells.NumberFormat = "@";
		}
		
		public void CreatePieChart(string chartData, string chartLocation, string chartTitle)
		{
			CreatePieChart(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, chartData, chartLocation, chartTitle);
		}

		public void CreatePieChart2(string chartData, string chartLocation, string chartTitle)
		{
			CreatePieChart2(ReportGlobals.defaultWorkbookName, ReportGlobals.defaultSheetIndex, chartData, chartLocation, chartTitle);
		}

		private void CreatePieChart2(string workbook, int sheetIndex, string chartData, string chartLocation, string chartTitle)
		{

			Worksheet operationalWorksheet = (Worksheet)workbookIndex[workbook].Sheets[sheetIndex];
			//Range chartWidthAndHeight = ParseRange(operationalWorksheet, chartLocation);
			//double width = double.Parse(chartWidthAndHeight.Cells.ColumnWidth.ToString());
			//double height = double.Parse(chartWidthAndHeight.Cells.RowHeight.ToString());

			// Column A width, plus half again column A
			//Range rngA = ParseRange(operationalWorksheet, "A");
			//double aPlusHalfA = double.Parse(rngA.Cells.ColumnWidth.ToString());

			//// The top of the chart will start below the bottom of the data?
			////Range rngDataBottom = GetHeaderPlusSubHeaderHeight();

			ChartObjects chartObjs = (ChartObjects)operationalWorksheet.ChartObjects(Type.Missing);
			ChartObject chartObj = chartObjs.Add(50, 150, 300, 300);
			Chart xlChart = chartObj.Chart;

			xlChart.Location(XlChartLocation.xlLocationAutomatic, operationalWorksheet.Name);

			//Range sourceData = ParseRange(operationalWorksheet, chartData);
			//Range sourceData = operationalWorksheet.get_Range("B5", "D5");
			Range sourceData2 = operationalWorksheet.get_Range("B16", "D16");
			//Range sourceData3 = operationalWorksheet.Application.Union(sourceData, sourceData2, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
			
			xlChart.ChartType = XlChartType.xl3DPie;
			xlChart.SetSourceData(sourceData2, XlRowCol.xlRows);
			xlChart.HasTitle = true;
			xlChart.ChartTitle.Text = chartTitle;

			xlChart.HasLegend = true;
			xlChart.Legend.Position = XlLegendPosition.xlLegendPositionBottom;
			xlChart.Legend.Font.Size = 14;
			Series oSeries = (Series)xlChart.SeriesCollection(1);
		}
		public void CreatePieChart(string workbook, int sheetIndex, string chartData, string chartLocation, string chartTitle)
		{
			Worksheet operationalWorksheet = (Worksheet)workbookIndex[workbook].Sheets[sheetIndex];
			//Range chartWidthAndHeight = ParseRange(operationalWorksheet, chartLocation);
			//double width = double.Parse(chartWidthAndHeight.Cells.ColumnWidth.ToString());
			//double height = double.Parse(chartWidthAndHeight.Cells.RowHeight.ToString());

			// Column A width, plus half again column A
			//Range rngA = ParseRange(operationalWorksheet, "A");
			//double aPlusHalfA = double.Parse(rngA.Cells.ColumnWidth.ToString());

			//// The top of the chart will start below the bottom of the data?
			////Range rngDataBottom = GetHeaderPlusSubHeaderHeight();

			ChartObjects chartObjs = (ChartObjects)operationalWorksheet.ChartObjects(Type.Missing);
			ChartObject chartObj = chartObjs.Add(50, 150, 300, 300); 
			Chart xlChart = chartObj.Chart;

			xlChart.Location(XlChartLocation.xlLocationAutomatic, operationalWorksheet.Name);
			
			Range sourceData = ParseRange(operationalWorksheet, chartData);
			xlChart.ChartType = XlChartType.xl3DPie;
			xlChart.SetSourceData(sourceData, XlRowCol.xlRows);
			xlChart.HasTitle = true;
			xlChart.ChartTitle.Text = chartTitle;

			xlChart.HasLegend = true;
			xlChart.Legend.Position = XlLegendPosition.xlLegendPositionBottom;
			xlChart.Legend.Font.Size = 14;
            Series oSeries = (Series)xlChart.SeriesCollection(1);
            
		}

		
    }

}
