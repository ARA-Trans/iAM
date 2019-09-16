using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RoadCare3;
using RCImageView3.Properties;
using RoadCareGlobalOperations;
using DataObjects;

namespace RCImageView3
{
    public partial class FormNavigation : ToolWindow
    {
        int m_nFrameSkip;
        double m_dFrameDelay;
        List<String> m_listTypes;
        bool m_bUpdateComboBox = false;
        NavigationObject m_NavigationObject;
        bool m_bPlay = true; //Play on timer forward if true
        EventSubscriber m_eventSubscriber;

        public bool UpdateNavigation
        {
            set { m_bUpdateComboBox = value; }
            get { return m_bUpdateComboBox; }
        }

        public int FrameSkip
        {
            set { m_nFrameSkip = value; }
            get { return m_nFrameSkip; }
        }

        public double FrameDelay
        {
            set { m_dFrameDelay = value; }
            get { return m_dFrameDelay; }
        }

        public List<String> ReferenceTypes
        {
            set { m_listTypes = value; }
            get { return m_listTypes; }
        }

        public NavigationObject Navigation
        {
            set { m_NavigationObject = value; }
            get { return m_NavigationObject; }
        }


        public FormNavigation()
        {
            InitializeComponent();
            ImageViewManager.Navigation = this;
            this.TabText = "Navigation";
            this.FrameSkip = Settings.Default.FRAMESKIP;
            
            this.FrameDelay = Settings.Default.FRAMERATE;
            LoadReferenceType();
            LoadFacility();
            this.UpdateNavigation = false;
            toolStripLabelSkip.Text = "Frame skip:" + this.FrameSkip.ToString();
            toolStripTextBoxFrameDelay.Text = this.FrameDelay.ToString();
            if (toolStripComboBoxType.Text == "Linear")
            {
                LoadDirection();
                toolStripLabelStationOrSection.Text = "Station";
                LoadInitialStation();
                LoadYearLinear();
            }
            else if (toolStripComboBoxType.Text == "Section")
            {
                toolStripLabelDirection.Visible = true;
                toolStripComboBoxDirection.Visible = true;
                toolStripLabelStationOrSection.Text = "Section";
                toolStripComboBoxSection.Visible = true;
                LoadDirection();
                LoadSection();
                LoadYearSection();
            }
            this.UpdateNavigation = true;

            //this.Navigation = new NavigationObject(ImageViewManager.Networks);

        }

        public void AddFormEvent(BaseForm form)
        {
            form.m_event = new EventPublisher();
            m_eventSubscriber = new EventSubscriber(form);
        }



        private void LoadReferenceType()
        {
            try
            {
                this.ReferenceTypes = GlobalDatabaseOperations.GetReferenceTypes();
                if (ReferenceTypes.Count == 0)
                {
                    OutputWindow.WriteOutput("Warning: IMAGELOCATION table is empty.");
                }

                foreach (String sReference in this.ReferenceTypes)
                {
                    toolStripComboBoxType.Items.Add(sReference);
                }
                String sReferenceDefault = Settings.Default.REFERENCETYPE;
                if (String.IsNullOrEmpty(sReferenceDefault) && this.ReferenceTypes.Count > 0)
                {
                    toolStripComboBoxType.SelectedIndex = 0;
                }
                else if (this.ReferenceTypes.Contains(sReferenceDefault))
                {
                    toolStripComboBoxType.Text = sReferenceDefault;
                }
                else if (this.ReferenceTypes.Count > 0)
                {
                    toolStripComboBoxType.SelectedIndex = 0;
                }

            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Error retrieving images from IMAGELOCATION table." + except.Message);
            }
        }


        private void LoadFacility()
        {
            List<String> listFacility = null;
            toolStripComboBoxFacility.Items.Clear();

            try
            {
                listFacility = GlobalDatabaseOperations.GetNavigationFacility(toolStripComboBoxType.Text);
                foreach (String sFacility in listFacility)
                {
                    toolStripComboBoxFacility.Items.Add(sFacility);
                }

                String sFacilityDefault = Settings.Default.FACILITY;
                if (String.IsNullOrEmpty(sFacilityDefault) && listFacility.Count > 0)
                {
                    toolStripComboBoxFacility.SelectedIndex = 0;
                }
                else if (listFacility.Contains(sFacilityDefault))
                {
                    toolStripComboBoxFacility.Text = sFacilityDefault;
                }
                else if (listFacility.Count > 0)
                {
                    toolStripComboBoxFacility.SelectedIndex = 0;
                }

            
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Retrieving Facility from IMAGELOCATION table. " + except.Message);
            }
        }


        private void LoadDirection()
        {
            List<String> listDirection = null;
            toolStripComboBoxDirection.Items.Clear();
            try
            {
                listDirection = GlobalDatabaseOperations.GetNavigationDirection(toolStripComboBoxFacility.Text);
                foreach (String sDirection in listDirection)
                {
                    toolStripComboBoxDirection.Items.Add(sDirection);                
                }
                if (listDirection.Count > 0)
                {
                    toolStripComboBoxDirection.SelectedIndex = 0;
                }
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Retrieving Direction from IMAGELOCATION table. " + except.Message);
            }
        }

        private void LoadInitialStation()
        {
            try
            {
                String sStation = GlobalDatabaseOperations.GetNavigationStation(toolStripComboBoxFacility.Text,toolStripComboBoxDirection.Text);
                toolStripTextBoxStation.Text = sStation;

            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Retrieving Station from ImageLocation table. " + except.Message);
            }
        }



        private void LoadYearLinear()
        {
            String strYear = toolStripComboBoxYear.Text;
            try
            {
                toolStripComboBoxYear.Items.Clear();
                List<String> listYears = GlobalDatabaseOperations.GetNavigationYear(toolStripComboBoxFacility.Text, toolStripComboBoxDirection.Text);
                foreach (String sYear in listYears)
                {
                    toolStripComboBoxYear.Items.Add(sYear);
                }
                if (listYears.Contains(strYear))
                {
                    toolStripComboBoxYear.Text = strYear;
                }
                else if (listYears.Count > 0)
                {
                    toolStripComboBoxYear.SelectedIndex = 0;
                }
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Filling Year List for Linear Facility. " + except.Message);


            }
        }


        private void LoadSection()
        {
            toolStripComboBoxSection.Items.Clear();
            try
            {
                
                List<String> listSections;
                if (toolStripComboBoxDirection.Text != "")
                {
                    listSections = GlobalDatabaseOperations.GetNavigationSection(toolStripComboBoxFacility.Text, toolStripComboBoxDirection.Text);
                }
                else
                {
                    listSections = GlobalDatabaseOperations.GetNavigationSection(toolStripComboBoxFacility.Text);
                }
                foreach (String sSection in listSections)
                {
                    toolStripComboBoxSection.Items.Add(sSection);
                }
                if (listSections.Count > 0)
                {
                    toolStripComboBoxSection.SelectedIndex = 0;
                }
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Filling Section list for Section Reference Facility. " + except.Message);

            }

        }


        private void LoadYearSection()
        {
            String strYear = toolStripComboBoxYear.Text;
            try
            {
                toolStripComboBoxYear.Items.Clear();
                List<String> listYears = GlobalDatabaseOperations.GetNavigationYearSRS(toolStripComboBoxFacility.Text);
                foreach (String sYear in listYears)
                {
                    toolStripComboBoxYear.Items.Add(sYear);
                }
                if (listYears.Contains(strYear))
                {
                    toolStripComboBoxYear.Text = strYear;
                }
                else if (listYears.Count > 0)
                {
                    toolStripComboBoxYear.SelectedIndex = 0;
                }
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Filling Year List for Linear Facility. " + except.Message);
            }
        }

        private void toolStripComboBoxDirection_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LinearSectionChange();
        }

        private void LinearSectionChange()
        {
            if (!UpdateNavigation) return;
            StopNavigation();
            String strType = toolStripComboBoxType.Text;
            this.UpdateNavigation = false;
            if (strType == "Linear")
            {
                LoadFacility();
                LoadDirection();
                toolStripLabelStationOrSection.Text = "Station";
                LoadInitialStation();
                LoadYearLinear();
                toolStripLabelDirection.Visible = true;
                toolStripComboBoxDirection.Visible = true;
                toolStripComboBoxSection.Visible = false;
                toolStripTextBoxStation.Visible = true;
            }
            else if (strType == "Section")
            {
                LoadFacility();
                toolStripLabelDirection.Visible = true;
                toolStripComboBoxDirection.Visible = true;
                toolStripLabelStationOrSection.Text = "Section";
                toolStripComboBoxSection.Visible = true;
                toolStripTextBoxStation.Visible = false;
                LoadSection();
                LoadYearSection();
                LoadDirection();
            }
            this.UpdateNavigation = true;
        }


        private void toolStripComboBoxFacility_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!UpdateNavigation) return;
            StopNavigation();
            String strType = toolStripComboBoxType.Text;
            this.UpdateNavigation = false;
            if (strType == "Linear")
            {
                LoadDirection();
                toolStripLabelStationOrSection.Text = "Station";
                LoadInitialStation();
                LoadYearLinear();
                toolStripComboBoxDirection.Visible = true;
                toolStripComboBoxSection.Visible = false;
                toolStripTextBoxStation.Visible = true;
            }
            else if (strType == "Section")
            {
                toolStripLabelDirection.Visible = true;
                toolStripComboBoxDirection.Visible = true;
                toolStripLabelStationOrSection.Text = "Section";
                toolStripComboBoxSection.Visible = true;
                toolStripTextBoxStation.Visible = false;
                LoadSection();
                LoadYearSection();
                LoadDirection();
            }
            this.UpdateNavigation = true;

        }

        private void toolStripComboBoxDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            String strPreviousSection = toolStripComboBoxSection.Text;

            if (!UpdateNavigation) return;
            StopNavigation();
            this.UpdateNavigation = false;
            if (toolStripComboBoxSection.Visible)
            {
                LoadSection();
                if (toolStripComboBoxSection.Items.Contains(strPreviousSection))
                {
                    toolStripComboBoxSection.Text = strPreviousSection;
                }

            }
            String strDirection = toolStripComboBoxDirection.Text;

            LoadInitialStation();
            LoadYearLinear();
            this.UpdateNavigation = true;
        }


        private void toolStripTextBoxStation_TextChanged(object sender, EventArgs e)
        {
            if (!UpdateNavigation) return;
            StopNavigation();
        }

        private void toolStripComboBoxYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!UpdateNavigation) return;
            StopNavigation();
            m_NavigationObject.Images.Clear();
            InitializeNavigation();


        }

        private void toolStripComboBoxSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!UpdateNavigation) return;
            StopNavigation();
            this.UpdateNavigation = false;
            LoadYearSection();
            this.UpdateNavigation = true;

        }

        public void StopNavigation()
        {
            toolStripButtonPlay.Image = Resources.play_icon;
            toolStripButtonPlay.Tag = "Play";
            toolStripButtonReverse.Image = Resources.reverse_icon;
            toolStripButtonReverse.Tag = "Reverse";
            toolStripLabelSecond.Text = "seconds";
            toolStripTextBoxFrameDelay.Visible = true;
            timerNavigation.Stop();
        }

        private void FormNavigation_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.FRAMERATE = this.FrameDelay;
            Settings.Default.FRAMESKIP = this.FrameSkip;
            Settings.Default.REFERENCETYPE = toolStripComboBoxType.Text;
            Settings.Default.FACILITY = toolStripComboBoxFacility.Text;
            Settings.Default.Save();
        }

        private void toolStripButtonFast_Click(object sender, EventArgs e)
        {
            this.FrameSkip *= 2;
            if (this.FrameSkip > 64) this.FrameSkip = 64;
            toolStripLabelSkip.Text = "Frame skip:" + this.FrameSkip.ToString();
            Settings.Default.FRAMESKIP = this.FrameSkip;
            Settings.Default.Save();
        }

        public void ButtonFast()
        {
            this.FrameSkip *= 2;
            if (this.FrameSkip > 64) this.FrameSkip = 64;
            toolStripLabelSkip.Text = "Frame skip:" + this.FrameSkip.ToString();
            Settings.Default.FRAMESKIP = this.FrameSkip;
            Settings.Default.Save();

        }

        private void toolStripButtonSlow_Click(object sender, EventArgs e)
        {
            this.FrameSkip /= 2;
            if (this.FrameSkip < 1) this.FrameSkip = 1;
            toolStripLabelSkip.Text = "Frame skip:" + this.FrameSkip.ToString();
            Settings.Default.FRAMESKIP = this.FrameSkip;
            Settings.Default.Save();
        }

        public void ButtonSlow()
        {
            this.FrameSkip /= 2;
            if (this.FrameSkip < 1) this.FrameSkip = 1;
            toolStripLabelSkip.Text = "Frame skip:" + this.FrameSkip.ToString();
            Settings.Default.FRAMESKIP = this.FrameSkip;
            Settings.Default.Save();
        }

        private void toolStripButtonPlay_Click(object sender, EventArgs e)
        {
            if (toolStripButtonPlay.Tag.ToString() == "Play")
            {
                toolStripButtonPlay.Tag = "Stop";
                toolStripButtonPlay.Image = Resources.stop_icon;
                toolStripButtonReverse.Image = Resources.reverse_icon;
                toolStripButtonReverse.Tag = "Reverse";
                m_bPlay = true;
                StartNavigation();
            }
            else
            {
                toolStripButtonPlay.Tag = "Play";
                toolStripButtonPlay.Image = Resources.play_icon;
                StopNavigation();
            }
        }

        public void ButtonPlay()
        {
            if (toolStripButtonPlay.Tag.ToString() == "Play")
            {
                toolStripButtonPlay.Tag = "Stop";
                toolStripButtonPlay.Image = Resources.stop_icon;
                toolStripButtonReverse.Image = Resources.reverse_icon;
                toolStripButtonReverse.Tag = "Reverse";
                m_bPlay = true;
                StartNavigation();
            }
            else
            {
                toolStripButtonPlay.Tag = "Play";
                toolStripButtonPlay.Image = Resources.play_icon;
                StopNavigation();
            }
        }



        private void OnPlay()
        {
            if (toolStripButtonPlay.Tag.ToString() == "Play")
            {
                toolStripButtonPlay.Tag = "Stop";
                toolStripButtonPlay.Image = Resources.stop_icon;
                toolStripButtonReverse.Image = Resources.reverse_icon;
                toolStripButtonReverse.Tag = "Reverse";
            }
            else
            {
                toolStripButtonPlay.Tag = "Play";
                toolStripButtonPlay.Image = Resources.play_icon;
            }
        }

        private void StartNavigation()
        {
            if (InitializeNavigation())
            {
                timerNavigation.Start();
            }
        }

        private bool InitializeNavigation()
        {
            String strType = toolStripComboBoxType.Text;
            if (strType == "Linear")
            {
                UpdateLinearNavigation();
            }
            else
            {
                return UpdateSectionNavigation();
            }
            return true;

        }


        private void UpdateLinearNavigation()
        {
            String strRoute = toolStripComboBoxFacility.Text;
            String strDirection = toolStripComboBoxDirection.Text;
            String strStation = toolStripTextBoxStation.Text;
            double dStation = double.Parse(strStation);
            String strYear = toolStripComboBoxYear.Text;
            ImageObject io = m_NavigationObject.UpdateNavigation(strRoute, strDirection, dStation, strYear);

            toolStripTextBoxFrameDelay.Visible = false;
            toolStripLabelSecond.Text = toolStripTextBoxFrameDelay.Text + " seconds";

            double dTime = 0.25;

            try
            {
                dTime = double.Parse(toolStripTextBoxFrameDelay.Text);
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Parsing Frame Delay.  A number greater than zero must be entered." + except.Message);
            }
            if (dTime <= 0)
            {
                dTime = 0.1;
                toolStripTextBoxFrameDelay.Text = "0.1";
            }



            toolStripTextBoxFrameDelay.Visible = false;
            toolStripLabelSecond.Text = toolStripTextBoxFrameDelay.Text + " seconds";

            timerNavigation.Interval = (int)(dTime * 1000);

        }

        private bool UpdateSectionNavigation()
        {
            String strRoute = toolStripComboBoxFacility.Text;
            String strSection = toolStripComboBoxSection.Text;
            String strDirection = toolStripComboBoxDirection.Text;
            String strYear = toolStripComboBoxYear.Text;
            m_NavigationObject.Section = strSection;
            ImageObject io = m_NavigationObject.UpdateNavigation(strRoute, strSection, strDirection, strYear);
            if (io == null)
            {
                FormMissingImages formMissing = new FormMissingImages(strRoute, strSection, strDirection,strYear);
                if (formMissing.ShowDialog() == DialogResult.OK)
                {
                    strYear = formMissing.Year;
                    if (String.IsNullOrEmpty(strYear))
                    {
                        return false;
                    }
                    strDirection = formMissing.Direction;
                    UpdateNavigation = false;
                    toolStripComboBoxDirection.Text = strDirection;
                    UpdateNavigation = true;
                    
                    toolStripComboBoxYear.Text = strYear;
                    io = m_NavigationObject.UpdateNavigation(strRoute, strSection, strDirection, strYear);
                }
                else
                {
                    StopNavigation();
                    return false;
                }

            }
            toolStripTextBoxFrameDelay.Visible = false;
            toolStripLabelSecond.Text = toolStripTextBoxFrameDelay.Text + " seconds";

            double dTime = 0.25;

            try
            {
                dTime = double.Parse(toolStripTextBoxFrameDelay.Text);
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Parsing Frame Delay.  A number greater than zero must be entered." + except.Message);
            }
            if (dTime <= 0)
            {
                dTime = 0.1;
                toolStripTextBoxFrameDelay.Text = "0.1";
            }

            toolStripTextBoxFrameDelay.Visible = false;
            toolStripLabelSecond.Text = toolStripTextBoxFrameDelay.Text + " seconds";

            timerNavigation.Interval = (int)(dTime * 1000);
            return true;
        }


        private void toolStripButtonReverse_Click(object sender, EventArgs e)
        {
            if (toolStripButtonReverse.Tag.ToString() == "Reverse")
            {
                toolStripButtonReverse.Tag = "Stop";
                toolStripButtonReverse.Image = Resources.stop_icon;
                toolStripButtonPlay.Image = Resources.play_icon;
                toolStripButtonPlay.Tag = "Play";
                m_bPlay = false;
                StartNavigation();

            }
            else
            {
                toolStripButtonReverse.Tag = "Reverse";
                toolStripButtonReverse.Image = Resources.reverse_icon;
                StopNavigation();
            }
        }


        public void ButtonReverse()
        {
            if (toolStripButtonReverse.Tag.ToString() == "Reverse")
            {
                toolStripButtonReverse.Tag = "Stop";
                toolStripButtonReverse.Image = Resources.stop_icon;
                toolStripButtonPlay.Image = Resources.play_icon;
                toolStripButtonPlay.Tag = "Play";
                m_bPlay = false;
                StartNavigation();

            }
            else
            {
                toolStripButtonReverse.Tag = "Reverse";
                toolStripButtonReverse.Image = Resources.reverse_icon;
                StopNavigation();
            }
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            ButtonNext();

        }

        public void ButtonNext()
        {
            toolStripButtonPlay.Image = Resources.play_icon;
            toolStripButtonPlay.Tag = "Play";
            toolStripButtonReverse.Image = Resources.reverse_icon;
            toolStripButtonReverse.Tag = "Reverse";
            StopNavigation();
            if (m_NavigationObject.Images == null)
            {
                InitializeNavigation();
            }
            if (toolStripComboBoxSection.Visible)
            {
                if (m_NavigationObject.Section != toolStripComboBoxSection.Text)
                {
                    if (this.UpdateNavigation)
                    {
                        InitializeNavigation();
                    }
                }
            }

            IncrementNavigation();

        }

        private void toolStripButtonPrevious_Click(object sender, EventArgs e)
        {
            toolStripButtonPlay.Image = Resources.play_icon;
            toolStripButtonPlay.Tag = "Play";
            toolStripButtonReverse.Image = Resources.reverse_icon;
            toolStripButtonReverse.Tag = "Reverse";
            StopNavigation();
            if (m_NavigationObject.Images == null)
            {
                InitializeNavigation();
            }
            DecrementNavigation();

        }

        public void ButtonPrevious()
        {

            toolStripButtonPlay.Image = Resources.play_icon;
            toolStripButtonPlay.Tag = "Play";
            toolStripButtonReverse.Image = Resources.reverse_icon;
            toolStripButtonReverse.Tag = "Reverse";
            StopNavigation();
            if (m_NavigationObject.Images == null)
            {
                InitializeNavigation();
            }
            DecrementNavigation();
        }


        private void toolStripTextBoxFrameDelay_TextChanged(object sender, EventArgs e)
        {
            if (!this.UpdateNavigation) return;
            StopNavigation();
            double dFrameDelay = this.FrameDelay;
            double.TryParse(toolStripTextBoxFrameDelay.Text, out dFrameDelay);
            this.FrameDelay = dFrameDelay;
            Settings.Default.FRAMERATE = this.FrameDelay;
            Settings.Default.Save();

        }

        private void timerNavigation_Tick(object sender, EventArgs e)
        {
            bool bStop = false;
            int nIncrement = m_nFrameSkip;
            if (!m_bPlay) nIncrement *= -1;
            ImageObject io = m_NavigationObject.IncrementUpdate(nIncrement, out bStop);
            this.UpdateNavigation = false;
            toolStripTextBoxStation.Text = io.Milepost.ToString();
            this.UpdateNavigation = false;
            toolStripComboBoxSection.Text = io.Section;
            this.UpdateNavigation = true;
            ImageViewManager.UpdateNavigateTick(ImageViewManager.TreeView.Nodes, m_NavigationObject);
        
        }

        private void IncrementNavigation()
        {
            bool bStop = false;
            ImageObject io = m_NavigationObject.IncrementUpdate(1, out bStop);
            if (!bStop)
            {
                this.UpdateNavigation = false;
                toolStripTextBoxStation.Text = io.Milepost.ToString();
                toolStripComboBoxSection.Text = io.Section;
                ImageViewManager.UpdateNavigateTick(ImageViewManager.TreeView.Nodes, m_NavigationObject); 
                if (toolStripTextBoxFrameDelay.Visible)
                {
                    toolStripTextBoxFrameDelay.Visible = false;
                    toolStripLabelSecond.Text = toolStripTextBoxFrameDelay.Text + " seconds";
                }
                this.UpdateNavigation = true;

            }
        }

        private void DecrementNavigation()
        {
            bool bStop = false;
            ImageObject io = m_NavigationObject.IncrementUpdate(-1, out bStop);
            if (!bStop)
            {
                this.UpdateNavigation = false;
                toolStripTextBoxStation.Text = io.Milepost.ToString();
                toolStripComboBoxSection.Text = io.Section;
                ImageViewManager.UpdateNavigateTick(ImageViewManager.TreeView.Nodes, m_NavigationObject);
                if (toolStripTextBoxFrameDelay.Visible)
                {
                    toolStripTextBoxFrameDelay.Visible = false;
                    toolStripLabelSecond.Text = toolStripTextBoxFrameDelay.Text + " seconds";
                }
                this.UpdateNavigation = true;

            }
        }


        public void OnNavigationEvent(NavigationEvent navigationEvent)
        {
            StopNavigation();
            if (navigationEvent.IsLinear && toolStripComboBoxType.Text == "Section")
            {
                toolStripComboBoxType.Text = "Linear";
                LinearSectionChange();
            }
            else if (!navigationEvent.IsLinear && toolStripComboBoxType.Text == "Linear")
            {
                toolStripComboBoxType.Text = "Section";
                LinearSectionChange();

            }

            this.UpdateNavigation = false;
 
            if (navigationEvent.IsLinear)
            {
                toolStripComboBoxFacility.Text = navigationEvent.Facility;
                toolStripComboBoxDirection.Text = navigationEvent.Direction;
                toolStripTextBoxStation.Text = navigationEvent.Station.ToString();

            }
            else
            {
                toolStripComboBoxFacility.Text = navigationEvent.Facility;
                toolStripComboBoxSection.Text = navigationEvent.Section;

            }

            LoadSection();
            toolStripComboBoxSection.Text = navigationEvent.Section;
            if (!String.IsNullOrEmpty(navigationEvent.Year))
            {
                if (toolStripComboBoxYear.Items.Contains(navigationEvent.Year))
                {
                    toolStripComboBoxYear.Text = navigationEvent.Year;
                }
            }

            
            this.UpdateNavigation = true;
            OnPlay();
            m_bPlay = true;
            StartNavigation();
        }

        private void FormNavigation_Load(object sender, EventArgs e)
        {

        }

        private void toolStripComboBoxYear_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBoxFacility_Click(object sender, EventArgs e)
        {

        }



    }
}
