using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects;

namespace RoadCareGlobalOperations
{
    public class NavigationObject
    {
        private String m_strYear;
        private String m_strImagePath;
        private bool m_bLinear;
        private String m_strRouteFacility;
        private String m_strDirection;
        private double m_dMilepost;
        private String m_strSection;
        private int m_nPrecedent;
        private List<NetworkNavigationObject> m_listNetworks = new List<NetworkNavigationObject>();
        private List<ImageObject> m_listImageObjects;
        private List<String> m_listViews;
        private ImageObject m_imageCurrent;
        private int m_nCurrentImageIndex;

        /// <summary>
        /// True if displaying linear data
        /// </summary>
        public bool IsLinear
        {
            get { return m_bLinear; }
            set { m_bLinear = value; }
        }

        /// <summary>
        /// Path where images are located
        /// </summary>
        public String ImagePath
        {
            get { return m_strImagePath; }
            set { m_strImagePath = value; }
        }
        
        /// <summary>
        /// Current Route/Facility
        /// </summary>
        public String Facility
        {
            get { return m_strRouteFacility; }
            set { m_strRouteFacility = value; }
        }
        /// <summary>
        /// Current Section
        /// </summary>
        public String Section
        {
            get { return m_strSection; }
            set { m_strSection = value; }
        }
        /// <summary>
        /// Current Direction
        /// </summary>
        public String Direction
        {
            get { return m_strDirection; }
            set { m_strDirection = value; }
        }
        /// <summary>
        /// Current Milepost
        /// </summary>
        public double Milepost
        {
            get { return m_dMilepost; }
            set { m_dMilepost = value; }
        }
        /// <summary>
        /// Current Precedent
        /// </summary>
        public int Precedent
        {
            get { return m_nPrecedent; }
            set { m_nPrecedent = value; }
        }

        public ImageObject CurrentImage
        {
            get { return m_imageCurrent; }
            set { m_imageCurrent = value; }

        }
        public String CurrentPath
        {
            get
            {
                String strImagePath = m_strImagePath;
                return strImagePath;
            }
        }

        public List<ImageObject> Images
        {
            get { return m_listImageObjects; }
        }

        public List<NetworkNavigationObject> Networks
        {
            get { return m_listNetworks; }
            set { m_listNetworks = value; }
        }
        public String Year
        {
            get { return m_strYear; }
            set { m_strYear = value; }
        }

        /// <summary>
        /// Creates NavigationObject for all included networks (USE TRY/CATCH)
        /// </summary>
        /// <param name="listNetworks"></param>
        public NavigationObject(List<NetworkObject> listNetworks)
        {
            m_listViews = GlobalDatabaseOperations.GetViews();
            foreach (NetworkObject no in listNetworks)
            {
                NetworkNavigationObject nno = new NetworkNavigationObject(no.NetworkID);
                m_listNetworks.Add(nno);
            }
        }
        /// <summary>
        /// Adds a new Network to the navigation object
        /// </summary>
        /// <param name="networkObject"></param>
        public void AddNewNetwork(NetworkObject networkObject)
        {
            NetworkNavigationObject nno = new NetworkNavigationObject(networkObject.NetworkID);
            m_listNetworks.Add(nno);
        }

        /// <summary>
        /// Removes a network from NavigationObject due to Delete Network
        /// </summary>
        /// <param name="networkObject"></param>
        public void RemoveNetwork(NetworkObject networkObject)
        {
            NetworkNavigationObject networkNavigationObject = m_listNetworks.Find(delegate(NetworkNavigationObject nno) { return nno.NetworkID == networkObject.NetworkID; });
            if (networkNavigationObject != null)
            {
                m_listNetworks.Remove(networkNavigationObject);
            }
        }

        /// <summary>
        /// Initialize a new ImageView run. Gets all images for ROUTE/DIRECTION and determines SECTIONID for each network.  
        /// </summary>
        /// <param name="strRoute">ROUTE for linear section</param>
        /// <param name="strDirection">Travel direction for linear section</param>
        /// <param name="dMilePost">Milepost to display image for</param>
        /// <param name="strYear">Year to display data for</param>
        /// <returns>Closest ImageObject for input route/Direction/Milepost.  Null is returned if ROUTE/DIRECTION is not found.</returns>
        public ImageObject UpdateNavigation(String strRoute, String strDirection, double dMilePost, String strYear)
        {
            m_strYear = strYear;
            m_bLinear = true;
            //Fill list of ImageObjects from 
            m_listImageObjects = GlobalDatabaseOperations.GetImageObjectList(strRoute, strDirection,strYear, m_listViews);
            //Find closets image.
            this.CurrentImage = GetNearestImage(dMilePost);
            //Set Navigation for each network.
            UpdateNetworkNavigation(strRoute,strDirection);

            return this.CurrentImage;
        }



        public ImageObject UpdateNavigation(String strFacility, String strSection, String strDirection, String strYear)
        {
            m_strYear = strYear;
            m_bLinear = false;
            //Fill list of ImageObjects from 
            m_listImageObjects = GlobalDatabaseOperations.GetImageObjectList(strFacility, strDirection, strYear, m_listViews);
            this.CurrentImage = GetNearestImage(strSection);
            UpdateNetworkNavigationFacility(strFacility,strSection);
            return this.CurrentImage;
        }

        /// <summary>
        /// Retrieves first section in precedent order for a section.
        /// </summary>
        /// <param name="strSection"></param>
        /// <returns></returns>
        private ImageObject GetNearestImage(String strSection)
        {
            ImageObject imageObject = null;
            int nMinimum = int.MaxValue;
            int nIndex = 0;
            foreach (ImageObject io in m_listImageObjects)
            {
                if (io.Section == strSection)
                {
                    if (io.Precedent < nMinimum)
                    {
                        nMinimum = io.Precedent;
                        imageObject = io;
                        m_nCurrentImageIndex = nIndex;
                    }
                }
                nIndex++;
            }
            return imageObject;
        }


        /// <summary>
        /// Returns image on the list closest to input milepost.
        /// </summary>
        /// <param name="dMilepost">Milepost for which image is desired</param>
        /// <returns>Image object nearest to milepost.</returns>
        private ImageObject GetNearestImage(double dMilepost)
        {
            ImageObject imageObject = null;
            double dClosest = double.MaxValue;
            int nIndex = 0;
            foreach (ImageObject io in m_listImageObjects)
            {
                double dDifference = Math.Abs(io.Milepost - dMilepost);
                if(dDifference < dClosest)
                {
                    dClosest = dDifference;
                    imageObject = io;
                    m_nCurrentImageIndex = nIndex;
                }
                nIndex++;
            }
            return imageObject;
        }

        /// <summary>
        /// Updating Route/Direction
        /// </summary>
        /// <param name="strRoute"></param>
        /// <param name="strDirection"></param>
        private void UpdateNetworkNavigation(String strRoute, String strDirection)
        {
            foreach (NetworkNavigationObject nno in m_listNetworks)
            {

                nno.UpdateRouteDirection(strRoute, strDirection,this.CurrentImage);
            }
        }

        /// <summary>
        /// Updating Facility/Section
        /// </summary>
        /// <param name="strRoute"></param>
        /// <param name="strDirection"></param>
        private void UpdateNetworkNavigationFacility(String strFacility,String strSection)
        {
            foreach (NetworkNavigationObject nno in m_listNetworks)
            {
                nno.UpdateFacilitySection(strFacility, strSection, this.CurrentImage);
            }
        }


        /// <summary>
        /// Increment pictures (+/-) along current Route/Direction
        /// </summary>
        /// <param name="nIncrement">Skip increment (+/-)</param>
        /// <param name="bStop">Returns true if at end of route.</param>
        /// <returns></returns>
        public ImageObject IncrementUpdate(int nIncrement,out bool bStop)
        {
            bStop = false;
            m_nCurrentImageIndex += nIncrement;
            if (m_nCurrentImageIndex < 0)
            {
                m_nCurrentImageIndex = 0;
                bStop = true;
            }

            if (m_nCurrentImageIndex >= m_listImageObjects.Count)
            {
                m_nCurrentImageIndex = m_listImageObjects.Count - 1;
                bStop = true;
            }
            ImageObject imageObject = m_listImageObjects[m_nCurrentImageIndex];
            foreach (NetworkNavigationObject nno in m_listNetworks)
            {
                if (IsLinear)
                {
                    nno.UpdateLinear(this.CurrentImage);
                }
                else
                {
                    nno.UpdateSection(this.CurrentImage);
                }
            }
            this.CurrentImage = imageObject;
            return imageObject;
        }
    }



    public class NetworkNavigationObject
    {
        private SectionObject m_sectionCurrent;
        private String m_strNetworkID;
        private double m_dBeginStation;
        private double m_dEndStation;
        private int m_nBeginPrecedent;
        private int m_nEndPrecedent;
        private int m_nCurrentSectionID;
        private List<SectionObject> m_listSections;
        /// <summary>
        /// NetworkID for each Network for IMAGEVIEW
        /// </summary>
        public String NetworkID
        {
            get { return m_strNetworkID; }
            set { m_strNetworkID = value; }
        }

        /// <summary>
        /// Current Section
        /// </summary>
        public SectionObject CurrentSection
        {
            get { return m_sectionCurrent; }
            set { m_sectionCurrent = value; }
        }


        /// <summary>
        /// Begin Station for current displayed SECTION
        /// </summary>
        public double BeginStation
        {
            get { return m_dBeginStation; }
            set { m_dBeginStation = value; }
        }
        /// <summary>
        /// End Station for current displayed SECTION
        /// </summary>
        public double EndStation
        {
            get { return m_dEndStation; }
            set { m_dEndStation = value; }
        }
        /// <summary>
        /// Begin Precedent for current displayed SECTION
        /// </summary>
        public int BeginPrecedent
        {
            get { return m_nBeginPrecedent; }
            set { m_nBeginPrecedent = value; }
        }
        /// <summary>
        /// End Precedent for current displayed SECTION
        /// </summary>
        public int EndPrecedent
        {
            get { return m_nEndPrecedent; }
            set { m_nEndPrecedent = value; }
        }
        /// <summary>
        /// SectionID for current displayed SECTION
        /// </summary>
        public int SectionID
        {
            get { return m_nCurrentSectionID; }
            set { m_nCurrentSectionID = value; }
        }
        public NetworkNavigationObject(String strNetworkID)
        {
            this.NetworkID = strNetworkID;
        }

        /// <summary>
        /// Find current section for new Route/Direction
        /// </summary>
        /// <param name="strRoute"></param>
        /// <param name="strDirection"></param>
        /// <param name="imageObject"></param>
        public void UpdateRouteDirection(String strRoute, String strDirection,ImageObject imageObject)
        {
			bool below = true;
			bool above = true;

			m_listSections = GlobalDatabaseOperations.GetSections( strRoute, strDirection, this.NetworkID );

			if (m_listSections.Count != 0)
			{
				SectionObject minBeginSection = m_listSections[0];
				SectionObject maxEndSection = m_listSections[0];


				//Search this list to find which section
				foreach (SectionObject so in m_listSections)
				{
					if (imageObject.Milepost >= so.BeginStation && imageObject.Milepost <= so.EndStation)
					{
						this.CurrentSection = so;
					}
					else if (imageObject.Milepost < so.BeginStation)
					{
						above = false;
					}
					else if (imageObject.Milepost > so.EndStation)
					{
						below = false;
					}

					if (so.BeginStation < minBeginSection.BeginStation)
					{
						minBeginSection = so;
					}
					if (so.EndStation > maxEndSection.EndStation)
					{
						maxEndSection = so;
					}
				}

				if (this.CurrentSection == null)	//didn't find a match
				{
					if (above)
					{
						this.CurrentSection = maxEndSection;
					}

					if (below)
					{
						this.CurrentSection = minBeginSection;
					}
				}
			}
        }

        /// <summary>
        /// Find current section for new Route/Direction
        /// </summary>
        /// <param name="strRoute"></param>
        /// <param name="strDirection"></param>
        /// <param name="imageObject"></param>
        public void UpdateFacilitySection(String strFacility, String strSection, ImageObject imageObject)
        {
            m_listSections = GlobalDatabaseOperations.GetSections(strFacility, this.NetworkID);

            if (imageObject != null)
            {
                //Search this list to find which section
                foreach (SectionObject so in m_listSections)
                {
                    if (imageObject.Section == so.Section)
                    {
                        this.CurrentSection = so;
                    }
                }
            }
            else
            {
                if (m_listSections.Count > 0)
                {
                    this.CurrentSection = m_listSections[0];
                }
            }
        }        
        
        
        /// <summary>
        /// Find current section for new Route/Direction
        /// </summary>
        /// <param name="imageObject"></param>
        public bool UpdateLinear(ImageObject imageObject)
        {
            if (this.CurrentSection == null || imageObject.Milepost < this.CurrentSection.BeginStation || imageObject.Milepost > this.CurrentSection.EndStation)
            {
                //Search this list to find which section
                foreach (SectionObject so in m_listSections)
                {
                    if (imageObject.Milepost >= so.BeginStation && imageObject.Milepost < so.EndStation)
                    {
                        this.CurrentSection = so;
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Find current section for new Facility/Section
        /// </summary>
        /// <param name="imageObject"></param>
        public bool UpdateSection(ImageObject imageObject)
        {
            if (this.CurrentSection == null || imageObject.Section != this.CurrentSection.Section)
            {
                //Search this list to find which section
                foreach (SectionObject so in m_listSections)
                {
                    if (imageObject.Section == so.Section)
                    {
                        this.CurrentSection = so;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
