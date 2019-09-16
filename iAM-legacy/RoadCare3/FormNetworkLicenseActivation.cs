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
	public partial class FormNetworkLicenseActivation : Form
	{
		public static string licCode = "";
		private static string licensefilepath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\RoadCare\\RoadCare.LogicNP.txt";

		public FormNetworkLicenseActivation()
		{
			InitializeComponent();
		}

		private void btnActivate_Click(object sender, EventArgs e)
		{
			if (tbVKey.Text == "" || tbLCode.Text == "" || tbLURL.Text == "")
			{
				MessageBox.Show("All fields need to be filled.");
				return;
			}

			// Verify license code
			CryptoLicense license = new CryptoLicense();
			license.ValidationKey = tbVKey.Text.TrimEnd('\r', '\n');
			license.LicenseCode = tbLCode.Text.TrimEnd('\r', '\n');
			license.LicenseServiceURL = tbLURL.Text.TrimEnd('\r', '\n');

			// The license will be loaded from/saved to the registry
			license.StorageMode = LicenseStorageMode.ToRegistry;
			license.UseHashedMachineCode = false;

			// Validate license...
			if (license.Status != LicenseStatus.Valid)
			{
				if (license.Status == LicenseStatus.SignatureInvalid)
				{
					MessageBox.Show("Invalid license.  Please check your entries and try again.");
				}
				else if (license.Status == LicenseStatus.ActivationFailed)
				{
					if (license.GetCurrentActivationCount() == license.MaxActivations)
					{
						MessageBox.Show("License Server has reached maximum usage. Please contact your administrator.");
					}
					else
					{
						MessageBox.Show("License activation failed.  Please contact your administrator.");
					}
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

				DumpFloatingLicense(license);

				this.Close();
				Application.Restart();
			}
		}

		private bool DumpFloatingLicense(CryptoLicense license)
		{
			license.Dispose();
			license = null;
			return true;
		}

		private void UpdateInfoFile()
		{
			string validationkey = tbVKey.Text.TrimEnd('\r', '\n');
			string licenseURL = tbLURL.Text.TrimEnd('\r', '\n');

			try
			{
				File.Delete(licensefilepath);
			}
			catch
			{
				MessageBox.Show("Failed to delete RoadCare.LoginNP.txt.  Please contact your administrator.");
			}

			try
			{
				using (StreamWriter file = new StreamWriter(licensefilepath, true))
				{
					if (validationkey != null && validationkey != "")
					{
						file.WriteLine("2");
						file.WriteLine(validationkey);
						file.WriteLine(licenseURL);
					}
				}
			}
			catch (Exception /*ex*/)
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
