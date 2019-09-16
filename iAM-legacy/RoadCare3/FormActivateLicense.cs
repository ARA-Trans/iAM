using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LogicNP.CryptoLicensing;
using System.Net.Sockets;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace RoadCare3
{
	public enum LicenseType { Network, Workstation, Unlimited };

	public partial class FormActivateLicense : Form
	{
		static string emailto = "cbecker@ara.com";

		public FormActivateLicense()
		{
			InitializeComponent();
		}

		private void buttonRequestLicenseCode_Click(object sender, EventArgs e)
		{
			LicenseType licenseType = LicenseType.Workstation;
			if(radioButtonUnlimited.Checked)
			{
				licenseType = LicenseType.Unlimited;
			}
			if(radioButtonNetwork.Checked)
			{
				licenseType = LicenseType.Network;
			}
			switch (licenseType)
			{
				case LicenseType.Workstation:
					string myHost = System.Net.Dns.GetHostName();
					string myHostName = System.Net.Dns.GetHostEntry(myHost).HostName;
					string myIP = GetIPAddress();
					string myUser = WindowsIdentity.GetCurrent().Name;

					string mymachinecode = GetMachineCode();

					if (tbName.Text == "" || tbEmail.Text == "" || tbOrganization.Text == "" || textBoxLastName.Text == "")
					{
						MessageBox.Show("All fields need to be filled.");
						return;
					}
					if (!IsEmail(tbEmail.Text))
					{
						MessageBox.Show("Please provide a valid email.");
						return;
					}
					// send email out using gmail smtp
					MailMessage mail = new MailMessage();
					SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

					mail.From = new MailAddress(tbEmail.Text);
					mail.To.Add(emailto);
					mail.Subject = "RoadCare License Code Request from " + tbEmail.Text;
					mail.Body = "This is for testing SMTP mail from GMAIL";

					SmtpServer.Port = 587;
					SmtpServer.Credentials = new System.Net.NetworkCredential("cbecker.ara@gmail.com", "ARA1eres");
					SmtpServer.EnableSsl = true;

					mail.Body = "Please send me a license code for RoadCare\n";
					mail.Body += "\n";
					mail.Body += "\tSubmitted By: " + tbName.Text + " " + textBoxLastName.Text + "\n";
					mail.Body += "\tEmail: " + tbEmail.Text + "\n";
					mail.Body += "\tOrgniazation: " + tbOrganization.Text + "\n";
					mail.Body += "\n";
					mail.Body += "\tHost Name: " + myHostName + "\n";
					mail.Body += "\tIP Address: " + myIP + "\n";
					mail.Body += "\tLocal User: " + myUser + "\n";
					mail.Body += "\tMachine Code: " + mymachinecode + "\n";
					mail.Body += "\n";
					mail.Body += "\tRequest Time: " + DateTime.Now + "\n";

					mail.IsBodyHtml = false; // can be MailFormat.HTML


					// create a plain text file
					// Compose a string that consists of three lines.
					string lines = "If the automated email license request was unable to be sent out through your local SMTP email server, ";
					lines += "this is the copy of email.  ";
					lines += "Simply copy and paste email body from the file and send out using any email tools you prefer. ";
					lines += "Call 217-356-4500 for further help.";
					lines += "\r\n\r\n";
					lines += "Send Email To: \r\n";
					lines += "\tcbecker@ara.com\r\n";
					lines += "\r\n";
					lines += "Subject: \r\n";
					lines += "\tRoadCare License Code Request from " + tbEmail.Text + "\r\n";
					lines += "\r\n";
					lines += "Email Body: \r\n";
					lines += "Please send me a license code for RoadCare Product.\r\n";
					lines += "\r\n";
					lines += "\tSubmitted By: " + tbName.Text + " " + textBoxLastName.Text + "\r\n";
					lines += "\tEmail: " + tbEmail.Text + "\r\n";
					lines += "\tOrgniazation: " + tbOrganization.Text + "\r\n";
					lines += "\r\n";
					lines += "\tHost Name: " + myHostName + "\r\n";
					lines += "\tIP Address: " + myIP + "\r\n";
					lines += "\tLocal User: " + myUser + "\r\n";
					lines += "\tMachine Code: " + mymachinecode + "\n";
					lines += "\r\n\r\n";
					lines += "\tRequest Sent at: " + DateTime.Now + "\r\n";

					// Write the string to a file.
					string emailFileName = "EmailLicenseRequest" + DateTime.Now.ToString("MMddyyyy") + ".txt";
					string emailTextFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoadCare Projects\\" + emailFileName;

					System.IO.StreamWriter file = new System.IO.StreamWriter(emailTextFilePath);
					file.WriteLine(lines);

					file.Close();


					try
					{
						SmtpServer.Send(mail);
						MessageBox.Show("Request has been sent successfully.  You should get a reply within 1 business day.", "RoadCare");
					}
					catch (Exception ex)
					{
						MessageBox.Show("Failed to send request: " + ex.Message.ToString(), "RoadCare");

						string mymessage = "If you failed to email this request out through your local SMTP email server, ";
						mymessage += "you can find a copy of this email at ";
						mymessage += System.Environment.NewLine;
						mymessage += emailTextFilePath;
						mymessage += System.Environment.NewLine;
						mymessage += "Simply copy and paste the email body from the file and send to us using any email tools you prefer. ";
						mymessage += System.Environment.NewLine;
						mymessage += "Call 217-356-4500 for further help.";

						MessageBox.Show(mymessage);
					}
					finally
					{
					}
					break;
				case LicenseType.Network:

					break;
				case LicenseType.Unlimited:

					break;
				default:
					throw new Exception("Invalid license type selection.");
					//break;
			}
		}

		public static bool IsEmail(string inputEmail)
		{
			string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
				+ @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
				+ @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
			Regex re = new Regex(strRegex);

			if (re.IsMatch(inputEmail))
				return (true);
			else
				return (false);
		}

		private static string GetIPAddress()
		{
			// When you are getting the IPs, you should check their AddressFamily property to determine wheter it is an IPv4 or IPv6.
			//If the AddressFamily is InterNetwork it is IPv4. 
			//if the AdressFamily is InterNetworkV6 it means that ip is in range of IPv6 protocol.

			string myip = "";

			String strHostName = Dns.GetHostName();

			IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);

			IPAddress[] addr = ipEntry.AddressList;

			for (int i = 0; i < addr.Length; i++)
			{

				if (addr[i].AddressFamily == AddressFamily.InterNetwork)
				{
					myip = addr[i].ToString();
					break;
				}

			}

			return myip;

		}

		private static string GetMachineCode()
		{
			// machine code
			CryptoLicense license = new CryptoLicense();
			return license.GetLocalMachineCodeAsString();
		}

		private void buttonEnterLicenseCode_Click(object sender, EventArgs e)
		{
			if(radioButtonNetwork.Checked)
			{
				// Launch Network based license entry form.
				FormNetworkLicenseActivation formNetworkLicenseActivation = new FormNetworkLicenseActivation();
				formNetworkLicenseActivation.ShowDialog();
				formNetworkLicenseActivation.Close();
			}
			if(radioButtonUnlimited.Checked || radioButtonWorkstation.Checked)
			{
				// Launch Workstation based license entry form.
				FormWorkstationOrUnlimitedLicenseActivation formWorkstationOrUnlimitedLicenseActivation = new FormWorkstationOrUnlimitedLicenseActivation();
				formWorkstationOrUnlimitedLicenseActivation.ShowDialog();
				formWorkstationOrUnlimitedLicenseActivation.Close();
			}
			this.Close();
		}
	}
}
