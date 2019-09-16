using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RoadCare3;
using RoadCareGlobalOperations;
using DataObjects;

namespace RCImageView3
{
    public partial class FormImageViewer : BaseForm
    {
        String m_sView;
        ImageObject io;
        NavigationObject m_navigationObject;
		//bool m_lockOut = false;
        public String View
        {
            set { m_sView = value; }
            get { return m_sView; }
        }


        public FormImageViewer(String sView)
        {
            InitializeComponent();
            this.View = sView;
            pictureBoxImage.SizeMode = PictureBoxSizeMode.AutoSize;
            this.TabText = sView + " Viewport";
            pictureBoxImage.Dock = DockStyle.Fill;
            //pictureBoxImage.SizeMode = PictureBoxSizeMode.StretchImage;

			pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;
            
        }

        public override void NavigationTick(NavigationObject navigationObject)
        {
            io = navigationObject.CurrentImage;
            m_navigationObject = navigationObject;
            String strPicturePath = io.GetPath(this.View);
            String strPath="";
            if (!String.IsNullOrEmpty(strPicturePath))
            {
                if (navigationObject.CurrentPath.Substring(navigationObject.CurrentPath.Length - 1,1) == "\\" && strPicturePath.Substring(0,1) == "\\")
                {
                    strPath = navigationObject.CurrentPath.Substring(0,navigationObject.CurrentPath.Length-1) + io.GetPath(this.View);
                }
                else
                {
                    strPath = navigationObject.CurrentPath + "\\" + io.GetPath(this.View);
                }
                try
                {
//                    pictureBoxImage.SizeMode = PictureBoxSizeMode.AutoSize;
                    //if (!strPath.Contains(".jpg")) strPath += ".jpg";
                    pictureBoxImage.Load(strPicturePath);

                }
                catch
                {

                }
            }
            String strLatitude = "Lat:" + io.Latitude.ToString();
            String strLatLong = strLatitude.PadRight(15) + " Long:" + io.Longitude.ToString();
            //this.TabText = this.View + " Viewport " + strPath;
            toolTip1.SetToolTip(pictureBoxImage, strPath);
            toolStripLabelLatitudeLongitude.Text = strLatLong.PadRight(30);
        }

        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            pictureBoxImage.Dock = DockStyle.None;
            pictureBoxImage.Anchor = AnchorStyles.Left|AnchorStyles.Top;
            pictureBoxImage.SizeMode = PictureBoxSizeMode.AutoSize;
            //pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            pictureBoxImage.Dock = DockStyle.Fill;
            //pictureBoxImage.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;

            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ImageViewManager.Navigation.StopNavigation();
            String strFileName = "";

            int nFind = io.GetPath(this.View).LastIndexOf("\\");
            if (nFind >= 0)
            {
                strFileName = io.GetPath(this.View).Substring(nFind+1);
            }
            else
            {
                strFileName = io.GetPath(this.View);
            }

            saveFileDialogImage.FileName = strFileName;
            if (saveFileDialogImage.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.File.Copy(m_navigationObject.CurrentPath + "\\" + io.GetPath(this.View), saveFileDialogImage.FileName);
                }
                catch (Exception exception)
                {
                    OutputWindow.WriteOutput("Error: Saving current image. " + exception.Message);

                }
            }
        }

        private void pictureBoxImage_Click(object sender, EventArgs e)
        {

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                switch (keyData)
                {
                    case Keys.Right:
                        ImageViewManager.Navigation.ButtonPlay();
                        break;
                    case Keys.Left:
                        ImageViewManager.Navigation.ButtonReverse();
                        break;
                    case Keys.Up:
                        ImageViewManager.Navigation.ButtonNext();
                        break;
                    case Keys.Down:
                        ImageViewManager.Navigation.ButtonPrevious();
                        break;
                    case Keys.Right |  Keys.Shift:
                        ImageViewManager.Navigation.ButtonFast();
                        break;
                    case Keys.Left |  Keys.Shift:
                        ImageViewManager.Navigation.ButtonSlow();
                        break;
                }



            }
            return false;
        }


    }
}
