using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseManager;
using Jace;
using Jace.Tokenizer;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Reports.MDSHA;
using Reports.MDSHA.LmDmVmtCondition;

namespace RoadCare3
{
    using JaceFunc = Func<Dictionary<string, double>, double>;
    using AttributeFunc = Func<double, Tuple<double, string>>;
    using ConditionFunc = Func<double, string>;

    /// <summary>
    ///     This is for the MDSHA "dual report" combining the two main reports
    ///     that relate lane-miles, directional miles, vehicle-miles-traveled,
    ///     and conditions.
    /// </summary>
    public partial class FormLmDmVmtConditionDualReport : Form
    {
        private const string ProfilesDataFileDirName = "Reports";

        private const string ProfilesDataFileBaseName = "lmdmvmtcond-profiles";

        private static readonly string ProfileNamePrompt =
            "Enter a name for the new profile." +
            Environment.NewLine +
            Environment.NewLine +
            "Entering a blank name will cancel the save operation.";

        private static readonly string IndexNamePrompt =
            "Enter a name for the new index, or enter a blank name to cancel.";

        private readonly string ProfilesDataFile;

        private readonly Dictionary<string, double?[]> DefaultLevelBounds;

        private readonly TextBox[] LevelBoundFields;

        private readonly TextBox[] ExpressionFields;

        private readonly List<Profile> AllProfiles;

        private Profile CurrentProfile;

        private Profile WorkingProfile;

        /// <summary>
        ///     Initializes the custom index settings form given a report to
        ///     work from and eventually generate.
        /// </summary>
        /// <param name="report"></param>
        public FormLmDmVmtConditionDualReport(Report report)
        {
            InitializeComponent();

            this.Report = report;

            this.tbNetwork.Text = report.Network;
            this.tbSimulation.Text = report.Simulation;

            this.fbdPickDir.Description =
                "Select the folder in which you would like the report file" +
                " to be generated.";

            // Construct absolute path to data file for profiles
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

            // Read in profiles (if file already exists)
            this.cbAvailableProfiles.DisplayMember = "ProfileName";
            this.cbAvailableProfiles.Sorted = true;
            if (File.Exists(this.ProfilesDataFile))
            {
                // Create a BACKUP copy of the profile data file before trying
                // to read it (and trying to possibly save over it later)
                File.Copy(
                    this.ProfilesDataFile,
                    this.ProfilesDataFile + ".BACKUP",
                    true);

                // Now, try to read it
                try
                {
                    var jsonText = File.ReadAllText(this.ProfilesDataFile);
                    this.AllProfiles =
                        JsonConvert.DeserializeObject<List<Profile>>(jsonText);

                    if (this.AllProfiles != null)
                    {
                        var simProfiles =
                            this.AllProfiles
                            .Where(p => p.SimulationId == report.SimulationId)
                            .ToArray();

                        this.cbAvailableProfiles.Items.AddRange(simProfiles);
                    }
                }
                catch (JsonException)
                {
                    var msg = string.Format(
                        "Report configuration profiles data is corrupted." +
                        " You may wish to remove or attempt manual recovery" +
                        " of the data file located here:\n\n{0}\n\n" +
                        "Otherwise, any profile you would save in the next" +
                        " dialog will overwrite all content in the existing" +
                        " data file.",
                        this.ProfilesDataFile);

                    Utilities.Show(
                        msg,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            // If for any reason AllProfiles was not initialized above, do it
            // now.
            if (this.AllProfiles == null)
            {
                this.AllProfiles = new List<Profile>();
            }

            // Load in this simulation's numeric source attributes and default
            // level bounds based on the simulation's performance measures
            this.DefaultLevelBounds =
                Utilities.GetDefaultAttributeBounds(report.SimulationId);

            this.chlbSourceAttributes.Items.AddRange(
                this.DefaultLevelBounds.Keys.ToArray());

            // Initialize the working profile
            this.WorkingProfile = new Profile()
            {
                NetworkId = this.Report.NetworkId,
                NetworkName = this.Report.Network,
                SimulationId = this.Report.SimulationId,
                SimulationName = this.Report.Simulation
            };
            this.WorkingProfile.AttributeIndexes.AddRange(
                this.DefaultLevelBounds.Keys.Select(
                srcAttr => new AttributeIndex() { SourceAttribute = srcAttr }));

            // Initialize some properties for the derived indexes listbox
            this.chlbDerivedIndexes.DisplayMember = "IndexName";

            // Set up lightweight groupings of the similar bound/expression
            // fields. This is so that the indices associated with the text
            // boxes can be found more easily (i.e. no hard-coding of indices;
            // nonetheless, there's some smell to this... like some of the rest
            // of this form's UI and data design, unfortunately... learning me
            // some WinForms for great good!)
            this.LevelBoundFields = new[]
            {
                this.tbLevelBound1,
                this.tbLevelBound2,
                this.tbLevelBound3,
                this.tbLevelBound4,
                this.tbLevelBound5,
            };
            this.ExpressionFields = new[]
            {
                this.tbIdxExpr1,
                this.tbIdxExpr2,
                this.tbIdxExpr3,
                this.tbIdxExpr4,
                this.tbIdxExpr5,
            };

            // Initialize selections etc.
            this.UnselectSourceAttributes();
            this.UnselectDerivedIndexes();
            this.cbAvailableProfiles.SelectedIndex = -1;

            // Set default focused control
            this.ActiveControl = this.cbAvailableProfiles;
        }

        #region Properties & Methods

        /// <summary>
        ///     Provides a way for clients of this form to retrieve the report
        ///     to be generated, if necessary (as when a builder pattern is
        ///     called for)
        /// </summary>
        /// <remarks>
        ///     Design note: This property, along with IsReportReady,
        ///     could be refactored into a base class for these report forms.
        /// </remarks>
        public Report Report { get; private set; }

        /// <summary>
        ///     Provides a mechanism by which clients of this form can tell
        ///     whether all info has been gathered/entered to successfully
        ///     generate the report.
        /// </summary>
        public bool IsReportReady { get; private set; }

        /// <summary>
        ///     Writes the given index settings profiles to the given outputfile
        ///     location.
        /// </summary>
        /// <param name="profiles"></param>
        /// <param name="outputFile"></param>
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

        /// <summary>
        ///     Checks whether the given profile name is already taken.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>
        ///     true if another profile uses the given name; false otherwise
        /// </returns>
        private bool IsNameOfExistingProfile(string name)
        {
            return this.cbAvailableProfiles.Items.Cast<Profile>().Any(
                p => p.ProfileName == name);
        }

        /// <summary>
        ///     Encapsulates common logic to be able to set a TextBox's Text
        ///     property with a double? value.
        /// </summary>
        /// <param name="i">level number 1--5; one-based, NOT zero-based!</param>
        /// <param name="value"></param>
        /// <remarks>
        ///     This is a bare inkling of what your dumble author should have
        ///     done all along: abstract the "physical"/visual/low-level
        ///     components of the UI to a "logical UI"---an insight I wish I'd
        ///     had two months ago. Live and learn... and refactor when there's
        ///     time.
        /// </remarks>
        private void SetLevelBoundText(int i, double? value)
        {
            var valueText = value == null ? string.Empty : value.ToString();

            switch (i)
            {
            case 1:
                this.tbLevelBound1.Text = valueText;
                break;
            case 2:
                this.tbLevelBound2.Text = valueText;
                break;
            case 3:
                this.tbLevelBound3.Text = valueText;
                break;
            case 4:
                this.tbLevelBound4.Text = valueText;
                break;
            case 5:
                this.tbLevelBound5.Text = valueText;
                break;
            default:
                throw new Exception("Level other than 1--5 was requested.");
            }
        }

        /// <summary>
        ///     Gets the currently selected attribute index.
        /// </summary>
        /// <returns>
        ///     the index object, or null if no attribute is selected
        /// </returns>
        private AttributeIndex GetSelectedAttributeIndex()
        {
            var sourceAttribute =
                (string)this.chlbSourceAttributes.SelectedItem;

            return this.GetAttributeIndex(sourceAttribute);
        }

        /// <summary>
        ///     Gets a specific attribute index given the source attribute name.
        /// </summary>
        /// <param name="sourceAttribute"></param>
        /// <returns>
        ///     the index object, or null if no such attribute exists
        /// </returns>
        private AttributeIndex GetAttributeIndex(string sourceAttribute)
        {
            return
                this.WorkingProfile.AttributeIndexes.FirstOrDefault(
                ai => ai.SourceAttribute == sourceAttribute);
        }

        /// <summary>
        ///     Makes sure that the index changed event is fired when clearing
        ///     the source attribute selection if there's already no selection.
        /// </summary>
        private void UnselectSourceAttributes()
        {
            this.chlbSourceAttributes.SelectedIndex = -1;
            this.chlbSourceAttributes_SelectedIndexChanged(null, null);
        }

        /// <summary>
        ///     Makes sure that the index changed event is fired when clearing
        ///     the derived index selection if there's already no selection.
        /// </summary>
        private void UnselectDerivedIndexes()
        {
            this.chlbDerivedIndexes.SelectedIndex = -1;
            this.chlbDerivedIndexes_SelectedIndexChanged(null, null);
        }

        /// <summary>
        ///     Checks that an index name is not already present among enabled
        ///     attribute indexes or derived indexes (excluding the index that
        ///     would be receiving this name)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index">index to be named</param>
        /// <returns>
        ///     true if the name is available to use; false otherwise
        /// </returns>
        private bool IsAvailableIndexName(IIndex index)
        {
            var sourceAttributes = this.DefaultLevelBounds.Keys;

            var enabledIndexes =
                Enumerable
                .Concat<IIndex>(
                    sourceAttributes
                    .Select(this.GetAttributeIndex)
                    .Where(ai => ai != null && ai.Enabled),
                    this.WorkingProfile.DerivedIndexes);

            var enabledIndexNames =
                from i in enabledIndexes
                where i != index
                select i.IndexName;

            return
                !sourceAttributes.Contains(index.IndexName) &&
                !enabledIndexNames.Contains(index.IndexName);
        }

        /// <summary>
        ///     Checks that an index name is properly formed (essentially that
        ///     it fits the Jace.NET definition of a variable name: alphanumeric
        ///     + underscore, without a leading digit)
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true if the name has a valid form; false otherwise</returns>
        private bool IsValidIndexName(string name)
        {
            List<Token> tokenized;
            return
                !string.IsNullOrWhiteSpace(name) &&
                name == name.Trim() &&
                (tokenized = new TokenReader().Read(name)).Count == 1 &&
                tokenized[0].TokenType == TokenType.Text;
        }

        #endregion

        #region UI Event Handlers

        /// <summary>
        ///     Deletes the selected "index settings" profile from the profile
        ///     storage file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            var profileItem = (Profile)this.cbAvailableProfiles.SelectedItem;

            if (profileItem == null) return;

            var warning = string.Format(
                "The stored profile named \"{0}\" will be deleted.",
                profileItem.ProfileName);

            var confirmation = Utilities.Show(
                warning,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);

            if (confirmation == DialogResult.OK)
            {
                // Capture the position of the profile to be removed
                var profileIndex = this.cbAvailableProfiles.SelectedIndex;

                // Remove the profile from the "available" list as well as the
                // "backing" list
                this.cbAvailableProfiles.Items.Remove(profileItem);
                this.AllProfiles.Remove(profileItem);

                // Reset the profile selection
                if (this.cbAvailableProfiles.Items.Count == profileIndex)
                {
                    this.cbAvailableProfiles.SelectedIndex = profileIndex - 1;
                }
                else
                {
                    this.cbAvailableProfiles.SelectedIndex = profileIndex;
                }

                // Start a thread to save the remaining profiles to disk,
                // overwriting the old save file
                var allProfiles = this.AllProfiles;
                var outputFile = this.ProfilesDataFile;
                Task.Factory.StartNew(
                    () => WriteProfiles(allProfiles, outputFile));
            }
        }

        /// <summary>
        ///     Loads a saved "index settings" profile (if one is selected).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadProfile_Click(object sender, EventArgs e)
        {
            var selectedProfile =
                (Profile)this.cbAvailableProfiles.SelectedItem;

            if (selectedProfile == null) return;

            this.CurrentProfile = selectedProfile;

            this.tbCurrentProfile.Text = this.CurrentProfile.ProfileName;
            this.chbAsNewProfile.Checked = false;

            this.WorkingProfile = new Profile(this.CurrentProfile);

            // Make sure the checked list boxes have no selected items.
            this.UnselectSourceAttributes();
            this.UnselectDerivedIndexes();

            // Load and enable applicable attribute indexes
            for (var i = 0; i < this.chlbSourceAttributes.Items.Count; ++i)
            {
                var sourceAttribute =
                    (string)this.chlbSourceAttributes.Items[i];

                var attrIdx = this.GetAttributeIndex(sourceAttribute);

                if (attrIdx == null)
                {
                    this.WorkingProfile.AttributeIndexes.Add(
                        new AttributeIndex()
                        {
                            SourceAttribute = sourceAttribute,
                        });
                }
                else if (attrIdx.Enabled)
                {
                    this.chlbSourceAttributes.SetItemChecked(i, true);
                }
            }

            // Load and enable applicable derived indexes
            var derivedIdxs = this.WorkingProfile.DerivedIndexes.ToArray();
            Array.Sort(
                derivedIdxs,
                (a, b) => string.CompareOrdinal(a.IndexName, b.IndexName));

            this.chlbDerivedIndexes.Items.Clear();
            this.chlbDerivedIndexes.Items.AddRange(derivedIdxs);
            for (var i = 0; i < derivedIdxs.Length; ++i)
            {
                if (derivedIdxs[i].Enabled)
                {
                    this.chlbDerivedIndexes.SetItemChecked(i, true);
                }
            }

            this.tbDstDir.Text = this.WorkingProfile.DestinationFolder;
            this.tbCurrentProfile.Text = this.WorkingProfile.ProfileName;
        }

        /// <summary>
        ///     Write the current profile to the settings file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            var selectedIndex = this.cbAvailableProfiles.SelectedIndex;

            if (this.chbAsNewProfile.Checked)
            {
                // Get non-null, non-blank/whitespace name from user that is not
                // the name of an existing profile
                var newProfileName =
                    Interaction.InputBox(ProfileNamePrompt).Trim();

                if (string.IsNullOrWhiteSpace(newProfileName)) return;

                while (this.IsNameOfExistingProfile(newProfileName))
                {
                    Utilities.Show(
                        "A profile with that name already exists.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    newProfileName =
                        Interaction.InputBox(ProfileNamePrompt).Trim();

                    if (string.IsNullOrWhiteSpace(newProfileName)) return;
                }

                this.WorkingProfile.ProfileName = newProfileName;
            }
            else
            {
                // Saving over the current profile, so remove it
                this.cbAvailableProfiles.Items.Remove(this.CurrentProfile);
                this.AllProfiles.Remove(this.CurrentProfile);
            }

            this.CurrentProfile = this.WorkingProfile;
            this.tbCurrentProfile.Text = this.CurrentProfile.ProfileName;
            this.chbAsNewProfile.Checked = false;

            this.cbAvailableProfiles.Items.Add(this.CurrentProfile);
            this.cbAvailableProfiles.SelectedIndex = selectedIndex;

            this.AllProfiles.Add(this.CurrentProfile);

            this.WorkingProfile = new Profile(this.CurrentProfile);

            var profiles = this.AllProfiles;
            var outputFile = this.ProfilesDataFile;
            Task.Factory.StartNew(() => WriteProfiles(profiles, outputFile));
        }

        /// <summary>
        ///     Ensures that the user cannot uncheck the "As New" checkbox
        ///     unless there is actually a current profile that they could
        ///     otherwise save over (with "As New" unchecked)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbAsNewProfile_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CurrentProfile == null)
            {
                this.chbAsNewProfile.Checked = true;
            }
        }

        #region "Attribute Indexes" tab page

        /// <summary>
        ///     This updates the interface to reflect whether the given source
        ///     attribute is checked or not. When checked, the attribute's input
        ///     must be verified/compiled and made uneditable. When unchecked,
        ///     it becomes editable again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chlbSourceAttributes_ItemCheck(
            object sender,
            ItemCheckEventArgs e)
        {
            var sourceAttribute =
                (string)this.chlbSourceAttributes.Items[e.Index];

            var attributeIndex = this.GetAttributeIndex(sourceAttribute);

            // Don't do anything extra if the item is being unchecked.
            if (e.NewValue == CheckState.Unchecked)
            {
                // But, disallow the uncheck if another enabled index depends on
                // this index/measure.
                var enabledDerivatives =
                    from derivedIndex in this.WorkingProfile.DerivedIndexes
                    where
                        derivedIndex.Enabled &&
                        derivedIndex.DependsOn(attributeIndex)
                    select derivedIndex.IndexName;

                if (enabledDerivatives.Any())
                {
                    Utilities.Show(
                        string.Format(
                        "The following derived indexes must be disabled" +
                        " before this measure index can be disabled: [{0}]",
                        string.Join(", ", enabledDerivatives)),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    e.NewValue = e.CurrentValue;
                }
                else
                {
                    this.WorkingProfile.AttributeFunctions
                        .Remove(sourceAttribute);

                    attributeIndex.Enabled = false;
                }
                return;
            }

            // So the item is not being unchecked. Now, verify a couple of
            // things, including validity of the entered name for the index
            // ---
            if (!this.IsValidIndexName(attributeIndex.IndexName))
            {
                Utilities.Show(
                    "The name of the associated index has an invalid form." +
                    " Please use letters, digits, and underscore to enter the" +
                    " name. Note that the name cannot begin with a digit.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                e.NewValue = e.CurrentValue;
                return;
            }

            // --- and uniqueness of the entered index name ---
            if (!this.IsAvailableIndexName(attributeIndex))
            {
                Utilities.Show(
                    "The name of the associated index is already in use.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                e.NewValue = e.CurrentValue;
                return;
            }

            // --- and that all bounds have had values entered.
            var boundedLevels =
                attributeIndex.Levels.Where(l => l.Bound.HasValue).ToArray();

            if (boundedLevels.Length == 0)
            {
                Utilities.Show(
                    "At least one level bound must be entered.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                e.NewValue = e.CurrentValue;
                return;
            }

            // Compute the bound ordering: -1 if strictly ascending, 1 if
            // strictly descending, 0 if neither
            var numComparisons = boundedLevels.Length - 1;
            var ordering = Enumerable.Range(0, numComparisons).Select(i =>
            {
                var firstValue = boundedLevels[i].Bound.Value;
                var secondValue = boundedLevels[i + 1].Bound.Value;
                var comparison = firstValue.CompareTo(secondValue);
                return Math.Sign(comparison);
            }).Sum() / numComparisons;

            // Now verify that the ordering is valid (either ascending or
            // descending)
            if (ordering == 0)
            {
                Utilities.Show(
                    "The level bound sequence must be either strictly" +
                    " increasing or strictly decreasing.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                e.NewValue = e.CurrentValue;
                return;
            }

            // Verify that all variable names in the expression are legal (i.e.
            // are exactly identical to the source attribute name). Note that we
            // treat all text subparts as variable names here, so usage of
            // built-in Jace.NET functions like "cos" etc. is effectively
            // disallowed --- currently. This could probably be changed, if
            // required.
            var boundedLevelNumbers =
                Enumerable.Range(1, attributeIndex.Levels.Length)
                .Where(i => attributeIndex.Levels[i - 1].Bound.HasValue);

            var levelExpressions =
                boundedLevelNumbers.Zip(
                boundedLevels.Select(l => l.Expression),
                (level, expression) => Tuple.Create(level, expression));

            var tokenReader = new TokenReader();
            foreach (var levelExpression in levelExpressions)
            {
                var level = levelExpression.Item1;
                var expression = levelExpression.Item2;

                var illegalVariables =
                    from t in tokenReader.Read(expression)
                    where t.TokenType == TokenType.Text
                    let v = expression.Substring(t.StartPosition, t.Length)
                    where v != sourceAttribute
                    select v;

                if (illegalVariables.Any())
                {
                    Utilities.Show(
                        string.Format(
                        "The index expression for level {0} contains the" +
                        " following illegal variables: [{1}]. The only legal" +
                        " variable for this index is its source measure, {2}.",
                        level,
                        string.Join(", ", illegalVariables.Distinct()),
                        sourceAttribute),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    e.NewValue = e.CurrentValue;
                    return;
                }
            }

            // Compile the expressions
            var engine = new CalculationEngine();
            var compiledExpressions =
                new Func<double, double>[attributeIndex.Levels.Length];

            foreach (var levelExpression in levelExpressions)
            {
                var level = levelExpression.Item1;
                var expression = levelExpression.Item2;

                try
                {
                    var computer = (Func<double, double>)
                        engine
                        .Formula(expression)
                        .Parameter(sourceAttribute, DataType.FloatingPoint)
                        .Result(DataType.FloatingPoint)
                        .Build();

                    compiledExpressions[level - 1] = computer;
                }
                catch (ParseException ex)
                {
                    Utilities.Show(
                        string.Format(
                        "An error occurred while parsing the index expression" +
                        " for level {0}:",
                        level) +
                        Environment.NewLine +
                        Environment.NewLine +
                        ex.Message,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    e.NewValue = e.CurrentValue;
                    return;
                }
            }

            // Attach compiled expressions to Level.Compute properties
            foreach (var level in boundedLevelNumbers)
            {
                attributeIndex.Levels[level - 1].Compute =
                    compiledExpressions[level - 1];
            }

            // Build and attach a single delegate that automates switching among
            // levels and selects the right compute expression to use and
            // condition to return
            var conditionsForBoundedLevels =
                Report.Conditions
                .ElementsAt(boundedLevelNumbers.Select(i => i - 1).ToArray());

            var boundFunctionsWithConditions =
                boundedLevelNumbers.Zip(conditionsForBoundedLevels, (l, c) => new
                {
                    Bound = attributeIndex.Levels[l - 1].Bound,
                    Compute = attributeIndex.Levels[l - 1].Compute,
                    Condition = c
                }).ToArray();

            this.WorkingProfile.AttributeFunctions[sourceAttribute] =
                ordering == -1 ?
                // Bounds are closed-ascending
                new AttributeFunc(attributeValue =>
                {
                    var bfcMatch = Array.Find(
                        boundFunctionsWithConditions,
                        bfc => attributeValue <= bfc.Bound);

                    if (bfcMatch == null)
                    {
                        bfcMatch = boundFunctionsWithConditions.Last();
                    }

                    return Tuple.Create(
                        bfcMatch.Compute(attributeValue),
                        bfcMatch.Condition);
                }) :
                // Bounds are closed-descending
                new AttributeFunc(attributeValue =>
                {
                    var bfcMatch = Array.Find(
                        boundFunctionsWithConditions,
                        bfc => attributeValue >= bfc.Bound);

                    if (bfcMatch == null)
                    {
                        bfcMatch = boundFunctionsWithConditions.Last();
                    }

                    return Tuple.Create(
                        bfcMatch.Compute(attributeValue),
                        bfcMatch.Condition);
                });

            attributeIndex.Enabled = true;
        }

        /// <summary>
        ///     Updates the Attribute Index tab page to reflect a newly selected
        ///     source attribute.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chlbSourceAttributes_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            var sourceAttribute =
                (string)this.chlbSourceAttributes.SelectedItem;

            if (sourceAttribute == null)
            {
                // Set read-only and clear all associated index fields
                this.tbIndexName.ReadOnly = true;
                this.tbIndexName.Clear();

                foreach (var field in
                    this.LevelBoundFields.Concat(this.ExpressionFields))
                {
                    field.ReadOnly = true;
                    field.Clear();
                }

                // Disable 'get defaults' button
                this.btnGetDefaultBounds.Enabled = false;
            }
            else
            {
                // Set with selected attribute's index's data
                var attributeIndex = this.GetAttributeIndex(sourceAttribute);

                if (attributeIndex == null)
                {
                    attributeIndex = new AttributeIndex()
                    {
                        SourceAttribute = sourceAttribute
                    };
                    this.WorkingProfile.AttributeIndexes.Add(attributeIndex);
                }

                this.tbIndexName.Text = attributeIndex.IndexName;
                Utilities.Zip(
                    Enumerable.Range(1, attributeIndex.Levels.Length),
                    attributeIndex.Levels.Select(level => level.Bound),
                    this.SetLevelBoundText);
                this.tbIdxExpr1.Text = attributeIndex.Levels[0].Expression;
                this.tbIdxExpr2.Text = attributeIndex.Levels[1].Expression;
                this.tbIdxExpr3.Text = attributeIndex.Levels[2].Expression;
                this.tbIdxExpr4.Text = attributeIndex.Levels[3].Expression;
                this.tbIdxExpr5.Text = attributeIndex.Levels[4].Expression;

                this.tbIndexName.ReadOnly = attributeIndex.Enabled;
                foreach (var field in this.LevelBoundFields)
                {
                    field.ReadOnly = attributeIndex.Enabled;
                }

                for (var i = 0; i < this.ExpressionFields.Length; ++i)
                {
                    this.ExpressionFields[i].ReadOnly =
                        attributeIndex.Enabled ||
                        string.IsNullOrWhiteSpace(this.LevelBoundFields[i].Text);
                }

                this.btnGetDefaultBounds.Enabled = !attributeIndex.Enabled;
            }
        }

        /// <summary>
        ///     Updates the working profile with the contents of the index name
        ///     field whenever it changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbIndexName_TextChanged(object sender, EventArgs e)
        {
            Utilities.InertTextBoxToUpper(
                this.tbIndexName,
                this.tbIndexName_TextChanged);

            var attributeIndex = this.GetSelectedAttributeIndex();

            if (attributeIndex != null)
            {
                attributeIndex.IndexName = this.tbIndexName.Text;
            }
        }

        /// <summary>
        ///     Sets a specified level bound in the working profile object with
        ///     the parsed content of the respective bound field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetProfileLevelBound(object sender, EventArgs e)
        {
            var levelIndex = Array.IndexOf(this.LevelBoundFields, sender);
            if (levelIndex == -1) return;

            var selectedAttributeIndex = this.GetSelectedAttributeIndex();

            // If no attribute is selected, it means that a profile is being
            // loaded, so ignore all these checks and sets.
            if (selectedAttributeIndex == null) return;

            // Would just use sender, but levelIndex is needed for accessing the
            // right Level struct in the selected attribute index, too, so might
            // as well draw the TextBox out without a cast
            var tbLevelBound = this.LevelBoundFields[levelIndex];

            double bound;
            if (double.TryParse(tbLevelBound.Text, out bound))
            {
                // The text is a valid double! Accept it, yay
                selectedAttributeIndex.Levels[levelIndex].Bound = bound;

                tbLevelBound.Tag = tbLevelBound.Text;
                
                this.ExpressionFields[levelIndex].ReadOnly = false;
            }
            else if (
                string.IsNullOrWhiteSpace(tbLevelBound.Text) ||
                tbLevelBound.Text == "-" ||
                tbLevelBound.Text == "." ||
                tbLevelBound.Text == "-.")
            {
                // The text is either blank or a normal double prefix, all of
                // which are okay, it just nulls the bound value
                selectedAttributeIndex.Levels[levelIndex].Bound = null;

                tbLevelBound.Tag = tbLevelBound.Text;

                this.ExpressionFields[levelIndex].ReadOnly = true;
            }
            else
            {
                // Revert the changed text (and the selection)
                var previousText = (string)tbLevelBound.Tag ?? string.Empty;
                var changeOffset =
                    tbLevelBound.Text.Length - previousText.Length;
                var previousSelectionStart =
                    Math.Max(0, tbLevelBound.SelectionStart - changeOffset);

                tbLevelBound.TextChanged -= this.SetProfileLevelBound;
                tbLevelBound.Text = previousText;
                tbLevelBound.TextChanged += this.SetProfileLevelBound;

                tbLevelBound.SelectionStart = previousSelectionStart;

                // Skip the WS trimming (next); don't need it since we're
                // restoring the text to a previous state.
                return;
            }

            // Trim any whitespace around the new text.
            var leadingWhitespaceOffset =
                tbLevelBound.Text.Length - tbLevelBound.Text.TrimStart().Length;
            var prevSelStart = tbLevelBound.SelectionStart;

            tbLevelBound.TextChanged -= this.SetProfileLevelBound;
            tbLevelBound.Text = tbLevelBound.Text.Trim();
            tbLevelBound.TextChanged += this.SetProfileLevelBound;

            tbLevelBound.SelectionStart =
                Math.Max(0, prevSelStart - leadingWhitespaceOffset);
        }

        /// <summary>
        ///     Sets a specified level expression in the working profile object
        ///     with the respective expression field contents.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetProfileLevelExpression(object sender, EventArgs e)
        {
            var levelIndex = Array.IndexOf(this.ExpressionFields, sender);
            if (levelIndex == -1) return;

            var selectedAttributeIndex = this.GetSelectedAttributeIndex();

            // Would just use sender, but levelIndex is needed for accessing the
            // right Level struct in the selected attribute index, too, so might
            // as well draw the TextBox out without a cast
            var tbExpression = this.ExpressionFields[levelIndex];

            Utilities.InertTextBoxToUpper(
                tbExpression,
                this.SetProfileLevelExpression);

            // If no attribute is selected, it means that a profile is being
            // loaded, so ignore all these checks and sets.
            if (selectedAttributeIndex == null) return;

            this.GetSelectedAttributeIndex()
                .Levels[levelIndex]
                .Expression = tbExpression.Text;
        }

        /// <summary>
        ///     Resets this attribute's level bounds to their defaults as given
        ///     by the RoadCare database at the moment this dialog was created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetDefaultBounds_Click(object sender, EventArgs e)
        {
            var sourceAttribute =
                (string)this.chlbSourceAttributes.SelectedItem;

            var defaultBounds = this.DefaultLevelBounds[sourceAttribute];

            var levelNumbers = Enumerable.Range(1, defaultBounds.Length);

            Utilities.Zip(levelNumbers, defaultBounds, this.SetLevelBoundText);
        }

        #endregion

        #region "Derived Indexes" tab page

        /// <summary>
        ///     Validates the checking/unchecking of a derived index item.
        ///     Compiles the index expression if the item is being checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chlbDerivedIndexes_ItemCheck(
            object sender,
            ItemCheckEventArgs e)
        {
            var derivedIndex =
                (DerivedIndex)this.chlbDerivedIndexes.Items[e.Index];

            // Always allow an uncheck.
            if (e.NewValue == CheckState.Unchecked)
            {
                derivedIndex.Enabled = false;
                return;
            }

            // So the item is not being unchecked. Now, verify a couple of
            // things, including validity of the entered name for the index ---
            if (!this.IsValidIndexName(derivedIndex.IndexName))
            {
                Utilities.Show(
                    "The current name of this index has an invalid form," +
                    " so this index cannot be enabled." +
                    " Please use letters, digits, and underscore to enter the" +
                    " name. Note that the name cannot begin with a digit.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                e.NewValue = e.CurrentValue;
                derivedIndex.Enabled = false;
                return;
            }

            // --- and check that the given index name is valid/available
            if (!this.IsAvailableIndexName(derivedIndex))
            {
                Utilities.Show(
                    "The current name of this index is already in use, so" +
                    " this index cannot be enabled. Please rename this index.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                e.NewValue = e.CurrentValue;
                derivedIndex.Enabled = false;
                return;
            }

            // Check for usage of any variable that is not either an enabled
            // source attribute or an enabled attribute index.
            var enabledAttributeIndexes =
                this.DefaultLevelBounds.Keys
                .Select(this.GetAttributeIndex)
                .Where(ai => ai != null && ai.Enabled);

            var enabledSourceAttributes =
                enabledAttributeIndexes.Select(ai => ai.SourceAttribute);

            var enabledAttributeIndexNames =
                enabledAttributeIndexes.Select(eai => eai.IndexName);

            var variables =
                from t in new TokenReader().Read(derivedIndex.Expression)
                where t.TokenType == TokenType.Text
                select
                    derivedIndex.Expression
                    .Substring(t.StartPosition, t.Length);

            var illegalVariables =
                variables
                .Except(enabledSourceAttributes)
                .Except(enabledAttributeIndexNames);

            if (illegalVariables.Any())
            {
                Utilities.Show(
                    string.Format(
                    "The index expression contains the following illegal" +
                    " variables: [{0}]. The only legal variables for this" +
                    " index are enabled performance measures [{1}] and their" +
                    " associated indexes [{2}].",
                    string.Join(", ", illegalVariables),
                    string.Join(", ", enabledSourceAttributes),
                    string.Join(", ", enabledAttributeIndexNames)),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                e.NewValue = e.CurrentValue;
                derivedIndex.Enabled = false;
                return;
            }

            // All variables legal, so try building it.
            JaceFunc computer;
            try
            {
                var engine = new CalculationEngine();
                computer = engine.Build(derivedIndex.Expression);
            }
            catch (ParseException ex)
            {
                Utilities.Show(
                    "An error occurred while parsing the index expression:" +
                    Environment.NewLine +
                    Environment.NewLine +
                    ex.Message,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                e.NewValue = e.CurrentValue;
                derivedIndex.Enabled = false;
                return;
            }

            // All's well; assign to the derived index.
            derivedIndex.Compute = computer;

            derivedIndex.SourceIndexes = new HashSet<string>(
                variables.Intersect(enabledAttributeIndexNames));

            derivedIndex.Enabled = true;
        }

        /// <summary>
        ///     Updates the UI to reflect a change of selected derived index.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chlbDerivedIndexes_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            var derivedIndex =
                (DerivedIndex)this.chlbDerivedIndexes.SelectedItem;

            if (derivedIndex == null)
            {
                this.tbExpression.ReadOnly = true;
                this.tbExpression.Clear();

                this.btnRenameIndex.Enabled = false;
                this.btnRemoveIndex.Enabled = false;
            }
            else
            {
                this.tbExpression.Text = derivedIndex.Expression;

                this.tbExpression.ReadOnly = derivedIndex.Enabled;

                this.btnRenameIndex.Enabled = !derivedIndex.Enabled;
                this.btnRemoveIndex.Enabled = !derivedIndex.Enabled;
            }
        }

        /// <summary>
        ///     Adds a derived index with an index name supplied by prompting
        ///     the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddIndex_Click(object sender, EventArgs e)
        {
            var newIndexName =
                Interaction.InputBox(IndexNamePrompt).Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(newIndexName)) return;

            var newDerivedIndex = new DerivedIndex()
            {
                IndexName = newIndexName
            };

            while (!this.IsAvailableIndexName(newDerivedIndex))
            {
                Utilities.Show(
                    "The name you entered is already in use. Please choose" +
                    " another.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                newIndexName =
                    Interaction.InputBox(ProfileNamePrompt).Trim().ToUpper();

                if (string.IsNullOrWhiteSpace(newIndexName)) return;

                newDerivedIndex.IndexName = newIndexName;
            }

            this.chlbDerivedIndexes.Items.Add(newDerivedIndex);
            this.WorkingProfile.DerivedIndexes.Add(newDerivedIndex);
        }

        /// <summary>
        ///     Renames the selected derived index with a name supplied by
        ///     prompting the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///     Note that a null check is not necessary since this button is
        ///     disabled when nothing is selected.
        /// </remarks>
        private void btnRenameIndex_Click(object sender, EventArgs e)
        {
            var derivedIndex =
                (DerivedIndex)this.chlbDerivedIndexes.SelectedItem;

            var oldName = derivedIndex.IndexName;

            var newIndexName =
                Interaction.InputBox(
                IndexNamePrompt,
                DefaultResponse: oldName)
                .Trim()
                .ToUpper();

            if (string.IsNullOrWhiteSpace(newIndexName)) return;

            derivedIndex.IndexName = newIndexName;

            while (!this.IsAvailableIndexName(derivedIndex))
            {
                Utilities.Show(
                    "The name you entered is already in use. Please choose" +
                    " another.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                newIndexName =
                    Interaction.InputBox(
                    IndexNamePrompt,
                    DefaultResponse: newIndexName)
                    .Trim()
                    .ToUpper();

                if (string.IsNullOrWhiteSpace(newIndexName))
                {
                    derivedIndex.IndexName = oldName;
                    return;
                }

                derivedIndex.IndexName = newIndexName;
            }

            this.chlbDerivedIndexes.Refresh();
        }

        /// <summary>
        ///     Removes the currently selected derived index.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///     Note that a null check is not necessary since this button is
        ///     disabled when nothing is selected.
        /// </remarks>
        private void btnRemoveIndex_Click(object sender, EventArgs e)
        {
            var derivedIndex =
                (DerivedIndex)this.chlbDerivedIndexes.SelectedItem;

            this.chlbDerivedIndexes.Items.Remove(derivedIndex);
            this.WorkingProfile.DerivedIndexes.Remove(derivedIndex);
        }

        /// <summary>
        ///     Updates the current index's expression when the entered
        ///     expression changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbExpression_TextChanged(object sender, EventArgs e)
        {
            Utilities.InertTextBoxToUpper(
                this.tbExpression,
                this.tbExpression_TextChanged);

            var derivedIndex =
                (DerivedIndex)this.chlbDerivedIndexes.SelectedItem;

            if (derivedIndex != null)
            {
                derivedIndex.Expression = this.tbExpression.Text;
            }
        }

        #endregion

        /// <summary>
        ///     Keeps the working profile's destination folder up-to-date with
        ///     the entered contents of the destination folder text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbDstDir_TextChanged(object sender, EventArgs e)
        {
            this.WorkingProfile.DestinationFolder = this.tbDstDir.Text;
        }

        /// <summary>
        ///     Allows the user to choose a valid directory in which to generate
        ///     the report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        ///     Passes on the working profile information (including bounds and
        ///     compiled expressions), and allows generation of the report to
        ///     proceed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFinish_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(this.tbDstDir.Text))
            {
                Utilities.Show(
                    "Destination folder does not exist.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else if (
                !this.WorkingProfile.AttributeIndexes
                .Where(ai => ai.Enabled)
                .Any())
            {
                Utilities.Show(
                    "At least one Performance Measure must be specified and" +
                    " enabled to generate this report.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                this.Report.OutputDirectory =
                    new DirectoryInfo(this.tbDstDir.Text);

                this.Report.SetCustomProfile(this.WorkingProfile);

                this.IsReportReady = true;
                this.Dispose();
            }
        }

        #endregion
    }
}
