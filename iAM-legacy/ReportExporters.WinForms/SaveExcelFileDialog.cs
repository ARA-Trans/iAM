using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ReportExporters.WinForms
{
	public class SaveExcelFileDialog
	{
		public static bool SaveMemoryStream(MemoryStream ms, string defaultFileName, string extension)
		{
			bool wasFileSaved = false;
			try
			{
				SaveFileDialog sfDlg = new SaveFileDialog();
				try
				{
					sfDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

					switch (extension.ToLower())
					{
						case "xls" :
							sfDlg.Filter = "Microsoft Office Excel Workbook (*.xls)|*.xls";
							break;
						case "pdf" :
							sfDlg.Filter = "Adobe Portable Document Format (*.pdf)|*.pdf";
							break;
						default:
							sfDlg.Filter = string.Format("{1} files|*.{0}", extension, extension.ToUpper());
							break;
					}

					sfDlg.RestoreDirectory = true;
					sfDlg.FileName = defaultFileName;
					if (sfDlg.ShowDialog() == DialogResult.OK)
					{
						File.WriteAllBytes(sfDlg.FileName, ms.ToArray());

						wasFileSaved = true;

						if (DialogResult.Yes == MessageBox.Show(
										"Do you want to open file?",
										"Confirmation",
										MessageBoxButtons.YesNo,
										MessageBoxIcon.Question))
						{
							//open exported file in assigned application
							Process.Start(sfDlg.FileName);
						}
					}
				}
				finally
				{
					sfDlg.Dispose();
				}
			}
			catch (Exception exc)
			{
				MessageBox.Show(
						exc.Message,
						"Aborted",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
			}

			return wasFileSaved;
		}
	}
}
