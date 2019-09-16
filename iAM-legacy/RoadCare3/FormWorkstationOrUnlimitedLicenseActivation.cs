using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LogicNP.CryptoLicensing;
using System.IO;

namespace RoadCare3
{
	public partial class FormWorkstationOrUnlimitedLicenseActivation : Form
	{
		public static string licCode = "";
		private static string licensefilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoadCare Projects\\RoadCare.LogicNP.txt";

		public FormWorkstationOrUnlimitedLicenseActivation()
		{
			InitializeComponent();
		}

		private void btnActivateRoadCare_Click(object sender, EventArgs e)
		{
            // Verify license code
            CryptoLicense license = new CryptoLicense();
            license.ValidationKey = tbVKey.Text.TrimEnd('\r', '\n');
            license.LicenseCode = tbLCode.Text.TrimEnd('\r', '\n');

            // The license will be loaded from/saved to the registry
            license.StorageMode = LicenseStorageMode.ToRegistry;

            // Validate license...
            if (license.Status != LicenseStatus.Valid)
            {
                if (license.Status == LicenseStatus.SignatureInvalid)
                {
                    MessageBox.Show("Invalid license.  Please check your entries and try again.");
                }
                else
                {
                    MessageBox.Show("Invalid license: " + license.Status.ToString());
                }
            }
            else
            {
                licCode = license.LicenseCode;
                license.Save();

                // write to RoadCare.LoginNP.txt
                UpdateInfoFile();

                this.Close();
				Application.Restart();
            }
		}

		private void UpdateInfoFile()
		{
			try
			{
				File.Delete(licensefilepath);
			}
			catch
			{
				MessageBox.Show("Failed to delete RoadCare.LoginNP.txt.  Please contact your administrator.");
			}

			string validationkey = tbVKey.Text.TrimEnd('\r', '\n');
			try
			{
				using (StreamWriter file = new StreamWriter(licensefilepath, true))
				{
					if (validationkey != null && validationkey != "")
					{
						file.WriteLine("1");
						file.WriteLine(validationkey);
					}
				}
			}
			catch
			{
				MessageBox.Show("Failed to update RoadCare.LoginNP.txt.  Please contact your administrator.");
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
