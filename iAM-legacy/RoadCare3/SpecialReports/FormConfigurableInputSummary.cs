namespace RoadCare3
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using Newtonsoft.Json;
    using Reports.MDSHA;
    using Reports.MDSHA.ConfigurableInputSummary;

    public partial class FormConfigurableInputSummary : Form
    {
        private const string ProfilesDataFileDirName = "Reports";

        private const string ProfilesDataFileBaseName = "cis-config-profiles";

        private readonly string ProfilesDataFile;

        public FormConfigurableInputSummary(Report report)
        {
            InitializeComponent();

            this.ActiveControl = this.chlbSections;

            this.Report = report;

            var availableSections = new Report.ISheet[]
            {
                new PrioritySheet(),
                new TargetSheet(),
                new DeficientSheet(),
                new InvestmentSheet(),
                new PerformanceSheet(),
                new TreatmentsSheet(),
                new FeasibilitySheet(),
                new CostsSheet(),
                new ConsequencesSheet()
            };

            this.chlbSections.Items.AddRange(availableSections);
            for (var i = 0; i < this.chlbSections.Items.Count; ++i)
            {
                this.chlbSections.SetItemChecked(i, true);
            }

            this.fbdPickDir.Description =
                "Select the folder in which you would like the report file" +
                " to be generated.";

            var programDataDir = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);

            var roadcareDataDir =
                Path.Combine(programDataDir, Global.ApplicationName);

            var reportsDataDir =
                Path.Combine(roadcareDataDir, ProfilesDataFileDirName);

            Directory.CreateDirectory(reportsDataDir);

            this.ProfilesDataFile = Path.Combine(
                reportsDataDir,
                ProfilesDataFileBaseName + ".json");

            this.lbAvailableProfiles.DisplayMember = "ProfileName";
            this.lbAvailableProfiles.Sorted = true;
            if (File.Exists(this.ProfilesDataFile))
            {
                try
                {
                    var jsonText = File.ReadAllText(this.ProfilesDataFile);
                    var profiles =
                        JsonConvert.DeserializeObject<Profile[]>(jsonText);

                    if (profiles != null)
                    {
                        this.lbAvailableProfiles.Items.AddRange(profiles);
                    }
                }
                catch (JsonReaderException)
                {
                    var msg = string.Format(
                        "Report configuration profiles data is corrupted." +
                        " You may wish to remove or attempt manual recovery" +
                        " of the data file located here:\n\n{0}\n\n" +
                        "Otherwise, any profile you would save in the next" +
                        " dialog will overwrite all content in the existing" +
                        " data file.",
                        this.ProfilesDataFile);

                    Utilities.Show(msg);
                }
            }

            this.btnSaveProfile.Enabled = false;
            this.btnLoadProfile.Enabled = false;
            this.btnRemoveProfile.Enabled = false;
        }

        public Report Report { get; private set; }

        public bool IsReportReady { get; private set; }

        private static void WriteProfiles(object profiles, string outputFile)
        {
            // Since we write to the same JSON output file every time, and
            // since this method is called from different threads (spawned to
            // handle the file-writing in case it might take a long time), we
            // lock on the Profile type to avoid file access collisions.
            lock (typeof(Profile))
            {
                var serializedProfiles =
                    JsonConvert.SerializeObject(profiles, Formatting.Indented);

                File.WriteAllText(outputFile, serializedProfiles);
            }
        }

        private void btnPickDir_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(this.tbDstDir.Text))
            {
                this.fbdPickDir.SelectedPath = this.tbDstDir.Text;
            }

            var res = this.fbdPickDir.ShowDialog();
            if (res == DialogResult.OK)
            {
                this.tbDstDir.Text = this.fbdPickDir.SelectedPath;
            }
        }

        private void btnGenRep_Click(object sender, EventArgs e)
        {
            if (this.chlbSections.CheckedItems.Count == 0)
            {
                Utilities.Show("Error: No sections are selected");
            }
            else if (!Directory.Exists(this.tbDstDir.Text))
            {
                Utilities.Show("Error: Destination folder does not exist");
            }
            else
            {
                this.Report.OutputDirectory =
                    new DirectoryInfo(this.tbDstDir.Text);

                var selectedSections =
                    this.chlbSections.CheckedItems.Cast<Report.ISheet>();

                foreach (var s in selectedSections)
                {
                    s.AddTo(this.Report);
                }

                this.IsReportReady = true;
                this.Dispose();
            }
        }

        private void tbCurrentProfile_TextChanged(object sender, EventArgs e)
        {
            this.btnSaveProfile.Enabled =
                !string.IsNullOrEmpty(this.tbCurrentProfile.Text);
        }

        private void lbAvailableProfiles_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            var isProfileSelected =
                this.lbAvailableProfiles.SelectedIndex != -1;

            this.btnLoadProfile.Enabled = isProfileSelected;
            this.btnRemoveProfile.Enabled = isProfileSelected;
        }

        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            var profiles = this.lbAvailableProfiles.Items;
            var profile = profiles.Cast<Profile>().FirstOrDefault(p =>
                p.ProfileName == this.tbCurrentProfile.Text);

            if (profile == null)
            {
                profile = new Profile();
                profile.ProfileName = this.tbCurrentProfile.Text;
                profile.Sections = new bool[this.chlbSections.Items.Count];

                profiles.Add(profile);
            }

            profile.DstDir = this.tbDstDir.Text;
            for (int i = 0; i < profile.Sections.Length; ++i)
            {
                profile.Sections[i] = this.chlbSections.GetItemChecked(i);
            }

            var outputFile = this.ProfilesDataFile;

            var bw = new BackgroundWorker();
            bw.DoWork += (sender_, e_) => WriteProfiles(profiles, outputFile);
            bw.RunWorkerAsync();
        }

        private void btnLoadProfile_Click(object sender, EventArgs e)
        {
            var profile = (Profile)this.lbAvailableProfiles.SelectedItem;
            if (profile.Sections.Length != this.chlbSections.Items.Count)
            {
                var msg = string.Format(
                    "Profile \"{0}\" is corrupted.",
                    profile.ProfileName);

                Utilities.Show(msg);
            }
            else
            {
                this.tbCurrentProfile.Text = profile.ProfileName;
                this.tbDstDir.Text = profile.DstDir;
                for (var i = 0; i < profile.Sections.Length; ++i)
                {
                    this.chlbSections.SetItemChecked(i, profile.Sections[i]);
                }
            }
        }

        private void btnRemoveProfile_Click(object sender, EventArgs e)
        {
            var profileIndex = this.lbAvailableProfiles.SelectedIndex;
            var profileItem = this.lbAvailableProfiles.SelectedItem;

            var profiles = this.lbAvailableProfiles.Items;
            var outputFile = this.ProfilesDataFile;

            profiles.Remove(profileItem);

            var bw = new BackgroundWorker();
            bw.DoWork += (sender_, e_) => WriteProfiles(profiles, outputFile);
            bw.RunWorkerAsync();

            this.ActiveControl = this.lbAvailableProfiles;

            if (profiles.Count == profileIndex)
            {
                this.lbAvailableProfiles.SelectedIndex = profileIndex - 1;
            }
            else
            {
                this.lbAvailableProfiles.SelectedIndex = profileIndex;
            }
        }

        private class Profile
        {
            public string ProfileName { get; set; }

            public bool[] Sections { get; set; }

            public string DstDir { get; set; }
        }
    }
}
