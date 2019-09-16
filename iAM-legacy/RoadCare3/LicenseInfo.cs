using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LogicNP.CryptoLicensing;

namespace RoadCare3
{
	class LicenseInfo
	{
		private static string licensefilepath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "RoadCare\\RoadCare.LogicNP.txt";

		public static string ValidationKey1 = "";
		public static string LicenseURL = "";
		public static string licCode = "";
		//public static int licType;

		public static CryptoLicense currentlicense;

		public static int GetLicenseType(string path)
		{
			string licenseType = "";
			string line;

			if (File.Exists(path))
			{
				StreamReader file = new StreamReader(path);
				while ((line = file.ReadLine()) != null)
				{
					licenseType = line;
					break;
				}

				file.Close();
			}
			if (licenseType == "1" || licenseType == "2")
			{
				return Convert.ToInt32(licenseType);
			}
			else
			{
				return -1;
			}

		}


		public static string GetLicenseValidationKey(string path)
		{
			string licenseCode = "";
			string line;
			int lineno = 0;

			if (File.Exists(path))
			{
				StreamReader file = new StreamReader(path);
				while ((line = file.ReadLine()) != null)
				{
					if (lineno == 1)
					{
						licenseCode = line;
						break;
					}
					lineno++;
				}

				file.Close();
			}

			return licenseCode;

		}

		public static string GetLicenseURL(string path)
		{
			string licURL = "";
			string line;

			if (File.Exists(path))
			{
				StreamReader file = new StreamReader(path);
				while ((line = file.ReadLine()) != null)
				{
					if (line.Contains("http"))
					{
						licURL = line;
						break;
					}
				}

				file.Close();
			}

			return licURL;

		}
	}
}
