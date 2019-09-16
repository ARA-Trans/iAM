using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Data;
using DatabaseManager;

namespace RoadCare3
{
    public static class FormManager
    {
        // Multiple instance forms
        private static List<FormAttributeDocument> listformAttributeDocuments = new List<FormAttributeDocument>();
        private static List<FormAttributeView> listformAttributeViews = new List<FormAttributeView>();
        private static List<FormAssets> listFormAssets = new List<FormAssets>();
        private static List<FormSegmentation> listFormSegmentation = new List<FormSegmentation>();
        private static List<FormSegmentationResult> listFormSegmentationResult = new List<FormSegmentationResult>();
        private static List<FormRollupSegmentation> listFormRollupSegmentation = new List<FormRollupSegmentation>();
        private static List<FormSegmentedConstruction> listFormSegmentedConstruction = new List<FormSegmentedConstruction>();
        private static List<FormGISView> listFormGISView = new List<FormGISView>();
        private static List<FormAnalysis> listFormAnalysis = new List<FormAnalysis>();
        private static List<FormInvestment> listFormInvestment = new List<FormInvestment>();
        private static List<FormPerformanceEquations> listFormPerformanceEquations = new List<FormPerformanceEquations>();
        private static List<FormTreatment> listFormTreatment = new List<FormTreatment>();
        private static List<FormSimulationResults> listFormSimulationResults = new List<FormSimulationResults>();
        private static List<FormSimulationResults> listFormSimulationCommitted = new List<FormSimulationResults>();
        private static List<FormAttributeView> listFormSimulationAttributes = new List<FormAttributeView>();
        private static List<FormSectionView> listFormSectionView = new List<FormSectionView>();
        private static List<FormCalculatedField> listFormCalculatedField = new List<FormCalculatedField>();


		// Eventually, all these lists should be refactored into this base list.
		private static List<BaseForm> listBaseForms = new List<BaseForm>();

        // Single instance forms
        private static FormAssetToAttribute m_formAssetToAttribute = null;
        private static FormAssetsCalculated m_formAssetsCalculated = null;
        private static FormConstructionHistory m_formConstructionHistory = null;
        private static FormGISLayerManager m_formGISLayerManager = null;
        private static SolutionExplorer m_formSolutionExplorer = null;
        private static FormOutputWindow m_formOutputWindow = null;
		private static AttributeTab m_attributeTab = null;
		private static AssetTab m_assetTab = null;
        private static FormSimulationAttributes m_formSimulationAttributes = null;
        private static FormResultsBudget m_formResultsBudget = null;
        private static FormResultsTarget m_formResultsTarget = null;
        private static FormResultsDeficient m_formResultsDeficient = null;
        private static FormImportGeometries m_formImportGeometries = null;
        private static FormImportShapeFile m_formImportShapefile = null;
        private static FormNetworkDefinition m_formNetworkDefinitionLinear = null;
        private static FormNetworkDefinition m_formNetworkDefinitionSection = null;
		private static FormPCIDocument m_formPCIDocument = null;
		private static DockPanel m_dockPanel;

		private static FormSecurityUsers m_formSecurityUsers = null;
		private static FormSecurityUserGroups m_formSecurityUserGroups = null;
		private static FormSecurityActions m_formSecurityActions = null;
		private static FormSecurityActionGroups m_formSecurityActionGroups = null;
		private static FormSecurityPermissions m_formSecurityPermissions = null;

		// Single instanced setting classes
		private static GISSettings m_GISSettings = new GISSettings();

		public static GISSettings AllGISSettings
		{
			get { return m_GISSettings; }
		}
		
		// No comments someone fails.
        public static DockPanel GetDockPanel()
        {
            return m_dockPanel;
        }
        public static void SetDockPanel(DockPanel panel)
        {
            m_dockPanel = panel;
        }


        #region RawAttributeDocument

        public static bool IsRawAttributeFormOpen(String strRawAttribute)
        {
            FormAttributeDocument formAttributeDocument = listformAttributeDocuments.Find(delegate(FormAttributeDocument form) { return form.Tag.ToString() == strRawAttribute; });
            if (formAttributeDocument != null)
            {
                formAttributeDocument.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveRawAttributeForm(FormAttributeDocument form)
        {
            listformAttributeDocuments.Remove(form);
        }

        public static void AddRawAttributeForm(FormAttributeDocument form)
        {
            listformAttributeDocuments.Add(form);
        }

        #endregion



        #region FormCalculatedField

        public static bool IsFormCalculatedFieldOpen(String strAttribute)
        {
            FormCalculatedField formCalculatedField = listFormCalculatedField.Find(delegate(FormCalculatedField form) { return form.Tag.ToString() == strAttribute; });
            if (formCalculatedField != null)
            {
                formCalculatedField.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormCalculatedField(FormCalculatedField form)
        {
            listFormCalculatedField.Remove(form);
        }

        public static void AddFormCalculatedField(FormCalculatedField form)
        {
            listFormCalculatedField.Add(form);
        }

        #endregion







        #region AttributeViewForm
        public static bool IsAttributeViewOpen(String strNetwork)
        {
            FormAttributeView formAttributeView = listformAttributeViews.Find(
				delegate(FormAttributeView form)
				{
					return form.Tag.ToString() == "Attribute-" + strNetwork;
				});
            if (formAttributeView != null)
            {
                formAttributeView.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static FormAttributeView GetCurrentAttributeView(String strNetwork)
        {
            return listformAttributeViews.Find(delegate(FormAttributeView form) { return form.Tag.ToString() == "Attribute-" + strNetwork; });
        }

        public static void RemoveAttributeViewForm(FormAttributeView form)
        {
            listformAttributeViews.Remove(form);
        }

        public static void AddAttributeViewForm(FormAttributeView form)
        {
            listformAttributeViews.Add(form);
        }
        #endregion

        #region AssetsForm
        public static bool IsAssetsFormOpen(String strAsset)
        {
            FormAssets formAssets = listFormAssets.Find(delegate(FormAssets form) { return form.Tag.ToString() == strAsset; });
            if (formAssets != null)
            {
                formAssets.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

		public static FormAssets GetAssetForm(string assetName)
		{
			FormAssets formAsset = listFormAssets.Find(delegate(FormAssets form) { return form.Tag.ToString() == assetName; });
			return formAsset;
		}

        public static void RemoveAssetsForm(FormAssets form)
        {
            listFormAssets.Remove(form);
        }

        public static void AddAssetsForm(FormAssets form)
        {
            listFormAssets.Add(form);
        }

        #endregion

        #region FormSegmentation
        public static bool IsFormSegmentationOpen(String strNetworkName)
        {
            FormSegmentation formTemp = listFormSegmentation.Find(delegate(FormSegmentation form) { return form.Tag.ToString() == strNetworkName; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormSegmentation(FormSegmentation form)
        {
            listFormSegmentation.Remove(form);
        }

        public static void AddFormSegmentation(FormSegmentation form)
        {
            listFormSegmentation.Add(form);
        }


        #endregion

        #region FormSegmentationResult
        public static bool IsFormSegmentationResultOpen(String strNetworkName)
        {
            FormSegmentationResult formTemp = listFormSegmentationResult.Find(delegate(FormSegmentationResult form) { return form.Tag.ToString() == strNetworkName; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormSegmentationResult(FormSegmentationResult form)
        {
            listFormSegmentationResult.Remove(form);
        }

        public static void AddFormSegmentationResult(FormSegmentationResult form)
        {
            listFormSegmentationResult.Add(form);
        }
        #endregion

        #region FormRollupSegmentation
        public static bool IsFormRollupSegmentationOpen(String strNetworkName)
        {
            FormRollupSegmentation formTemp = listFormRollupSegmentation.Find(delegate(FormRollupSegmentation form) { return form.Tag.ToString() == strNetworkName; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormRollupSegmentation(FormRollupSegmentation form)
        {
            listFormRollupSegmentation.Remove(form);
        }

        public static void AddFormRollupSegmentation(FormRollupSegmentation form)
        {
            listFormRollupSegmentation.Add(form);
        }
        #endregion

        #region FormSegmentedConstruction
        public static bool IsFormSegmentedConstructionOpen(String strNetworkName)
        {
            FormSegmentedConstruction formTemp = listFormSegmentedConstruction.Find(delegate(FormSegmentedConstruction form) { return form.Tag.ToString() == strNetworkName; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormSegmentedConstruction(FormSegmentedConstruction form)
        {
            listFormSegmentedConstruction.Remove(form);
        }

        public static void AddFormSegmentedConstruction(FormSegmentedConstruction form)
        {
            listFormSegmentedConstruction.Add(form);
        }
        #endregion

        #region FormGISView
        public static bool IsFormGISViewOpen(String strNetworkName, out FormGISView formTemp)
        {
            formTemp = listFormGISView.Find(delegate(FormGISView form) { return form.Tag.ToString() == strNetworkName; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormGISView(FormGISView form)
        {
            listFormGISView.Remove(form);
        }

        public static void AddFormGISView(FormGISView form)
        {
            listFormGISView.Add(form);
        }

        public static List<FormGISView> ListGISForms
        {
            get
            {
                return listFormGISView;
            }
        }

        #endregion

        #region FormAnalysis
        public static bool IsFormAnalysisOpen(String strSimID, out FormAnalysis formTemp)
        {
            formTemp = listFormAnalysis.Find(delegate(FormAnalysis form) { return form.Tag.ToString() == strSimID; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormAnalysis(FormAnalysis form)
        {
            listFormAnalysis.Remove(form);
        }

        public static void AddFormAnalysis(FormAnalysis form)
        {
            listFormAnalysis.Add(form);
        }

        public static List<FormAnalysis> ListFormAnalysis
        {
            get
            {
                return listFormAnalysis;
            }
        }
        #endregion

		#region FormInvestment
		public static bool IsFormInvestmentOpen(String strSimID, out FormInvestment formTemp)
        {
            formTemp = listFormInvestment.Find(delegate(FormInvestment form) { return form.Tag.ToString() == strSimID; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormInvestment(FormInvestment form)
        {
            listFormInvestment.Remove(form);
        }

        public static void AddFormInvestment(FormInvestment form)
        {
            listFormInvestment.Add(form);
        }

        public static List<FormInvestment> ListFormInvestment
        {
            get
            {
                return listFormInvestment;
            }
        }

        #endregion 

        #region FormPerformanceEquations
        public static bool IsFormPerformanceEquationsOpen(String strSimID, out FormPerformanceEquations formTemp)
        {
            formTemp = listFormPerformanceEquations.Find(delegate(FormPerformanceEquations form) { return form.Tag.ToString() == strSimID; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormPerformanceEquations(FormPerformanceEquations form)
        {
            listFormPerformanceEquations.Remove(form);
        }

        public static void AddFormPerformanceEquations(FormPerformanceEquations form)
        {
            listFormPerformanceEquations.Add(form);
        }

        public static List<FormPerformanceEquations> ListFormPerformanceEquations
        {
            get
            {
                return listFormPerformanceEquations;
            }
        }

        #endregion 

        #region FormTreatment
        public static bool IsFormTreatmentOpen(String strSimID, out FormTreatment formTemp)
        {
            formTemp = listFormTreatment.Find(delegate(FormTreatment form) { return form.Tag.ToString() == strSimID; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormTreatment(FormTreatment form)
        {
            listFormTreatment.Remove(form);
        }

        public static void AddFormTreatment(FormTreatment form)
        {
            listFormTreatment.Add(form);
        }

        public static List<FormTreatment> ListFormTreatment
        {
            get
            {
                return listFormTreatment;
            }
        }

        #endregion 

        #region FormSimulationResults
        public static bool IsFormSimulationResultsOpen(String strSimID, out FormSimulationResults formTemp)
        {
            formTemp = listFormSimulationResults.Find(delegate(FormSimulationResults form) { return form.Tag.ToString() == strSimID; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormSimulationResults(FormSimulationResults form)
        {
            listFormSimulationResults.Remove(form);
            if (listFormSimulationCommitted.Count == 0 && listFormSimulationResults.Count == 0)
            {
                RemoveSimulationAttributeWindow();
                RemoveResultsBudgetWindow();
            }

            if (listFormSimulationResults.Count == 0)
            {
                RemoveResultsTargetWindow();
                RemoveResultsDeficientWindow();

            }



        }

        public static void AddFormSimulationResults(FormSimulationResults form)
        {
            listFormSimulationResults.Add(form);
        }

        public static List<FormSimulationResults> ListFormSimulationResults
        {
            get
            {
                return listFormSimulationResults;
            }
        }

        #endregion 

        #region FormSimulationCommitted
        public static bool IsFormSimulationCommittedOpen(String strSimID, out FormSimulationResults formTemp)
        {
            formTemp = listFormSimulationCommitted.Find(delegate(FormSimulationResults form) { return form.Tag.ToString() == strSimID; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormSimulationCommitted(FormSimulationResults form)
        {
            listFormSimulationCommitted.Remove(form);
            if (listFormSimulationCommitted.Count == 0 && listFormSimulationResults.Count == 0)
            {
                RemoveSimulationAttributeWindow();
            }
        }

        public static void AddFormSimulationCommitted(FormSimulationResults form)
        {
            listFormSimulationCommitted.Add(form);
        }

        public static List<FormSimulationResults> ListFormSimulationCommitted
        {
            get
            {
                return listFormSimulationCommitted;
            }
        }

        #endregion 

        #region FormSimulationAttribute

        public static bool IsSimulationAttributeOpen(out FormSimulationAttributes form)
        {
            if (m_formSimulationAttributes != null)
            {
                m_formSimulationAttributes.Focus();
                form = m_formSimulationAttributes;
                return true;
            }
            else
            {
                form = null;
                return false;
            }
        }

        public static void RemoveSimulationAttributeWindow()
        {
            if (m_formSimulationAttributes != null)
            {
                m_formSimulationAttributes.Dispose();
                m_formSimulationAttributes = null;
            }
        }

        public static void AddSimulationAttributeWindow(FormSimulationAttributes form)
        {
            m_formSimulationAttributes = form;
        }

        public static FormSimulationAttributes GetSimulationAttributeWindow()
        {
            return m_formSimulationAttributes;
        }
        #endregion

        #region FormConstructionHistory
        public static bool IsFormConstructionHistoryOpen()
        {
            if (m_formConstructionHistory != null)
            {
                m_formConstructionHistory.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveFormConstructionHistory(FormConstructionHistory form)
        {
            m_formConstructionHistory.Dispose();
            m_formConstructionHistory = null;
        }

        public static void AddFormConstructionHistory(FormConstructionHistory form)
        {
            m_formConstructionHistory = form;
        }
        #endregion

        #region FormCalculatedAssets
        public static bool IsFormCalculatedAssetsOpen()
        {
            if (m_formAssetsCalculated != null)
            {
                m_formAssetsCalculated.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static FormAssetsCalculated GetFormCalculatedAssets()
        {
            return m_formAssetsCalculated;
        }


        public static void RemoveFormCalculatedAssets(FormAssetsCalculated form)
        {
            m_formAssetsCalculated.Dispose();
            m_formAssetsCalculated = null;
        }

        public static void AddFormCalculatedAssets(FormAssetsCalculated form)
        {
            m_formAssetsCalculated = form;
        }
        #endregion

        #region FormAssetToAttribute
        public static bool IsFormAssetToAttributeOpen()
        {
            if (m_formAssetToAttribute != null)
            {
                m_formAssetToAttribute.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static FormAssetToAttribute GetFormAssetToAttribute()
        {
            return m_formAssetToAttribute;
        }


        public static void RemoveAssetToAttribute(FormAssetToAttribute form)
        {
            m_formAssetToAttribute.Dispose();
            m_formAssetToAttribute = null;
        }

        public static void AddFormAssetToAttribute(FormAssetToAttribute form)
        {
            m_formAssetToAttribute = form;
        }
        #endregion


        #region SimulationAttribute
        public static bool IsSimulationAttributeOpen(String strSimulationID)
        {
            FormAttributeView formAttributeView = listFormSimulationAttributes.Find(delegate(FormAttributeView form) { return form.Tag.ToString() == strSimulationID; });
            if (formAttributeView != null)
            {
                formAttributeView.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }
        #region FormSectionView

        public static FormAttributeView GetSimulationAttribute(String strSimulationID)
        {
            return listFormSimulationAttributes.Find(delegate(FormAttributeView form) { return form.Tag.ToString() == strSimulationID; });
        }

        public static void RemoveSimulationAttributeForm(FormAttributeView form)
        {
            listFormSimulationAttributes.Remove(form);
        }
        public static bool IsFormSectionViewOpen(String strNetworkID, out FormSectionView formTemp)
        {
            formTemp = listFormSectionView.Find(delegate(FormSectionView form) { return form.NetworkID == strNetworkID; });
            if (formTemp != null)
            {
                formTemp.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void AddSimulationAttributeForm(FormAttributeView form)
        {
            listFormSimulationAttributes.Add(form);
        }
        #endregion
        public static void RemoveFormSectionView(FormSectionView form)
        {
            listFormSectionView.Remove(form);
        }

        public static void AddFormSectionView(FormSectionView form)
        {
            listFormSectionView.Add(form);
        }

        public static List<FormSectionView> ListSectionView
        {
            get
            {
				return listFormSectionView;
            }
        }



        #endregion

		#region BaseForm
		public static void AddBaseForm(BaseForm form)
		{
			listBaseForms.Add(form);
		}

		public static void RemoveBaseForm(BaseForm form)
		{
			listBaseForms.Remove(form);
		}

		public static bool IsBaseFormOpen(String check)
		{
			return (listBaseForms.Find(delegate(BaseForm form) { return form.Tag.ToString() == check; }) != null);
		}

		#endregion







		#region FormNetworkDefinitionLinear
		public static bool IsFormNetworkDefinitionLinearOpen(out FormNetworkDefinition formNetworkDefinitionLinear)
        {
            if (m_formNetworkDefinitionLinear != null)
            {
                formNetworkDefinitionLinear = m_formNetworkDefinitionLinear;
                m_formNetworkDefinitionLinear.Focus();
                return true;
            }
            else
            {
                formNetworkDefinitionLinear = null;
                return false;
            }
        }

        public static void RemoveFormNetworkDefinitionLinear(FormNetworkDefinition form)
        {
            m_formNetworkDefinitionLinear.Dispose();
            m_formNetworkDefinitionLinear = null;
        }

        public static void AddFormNetworkDefinitionLinear(FormNetworkDefinition form)
        {
            m_formNetworkDefinitionLinear = form;
        }

        public static FormNetworkDefinition formNetworkDefinitionLinear
        {
            get
            {
                return m_formNetworkDefinitionLinear;
            }
        }

        #endregion

        #region FormNetworkDefinitionSection
        public static bool IsFormNetworkDefinitionSectionOpen(out FormNetworkDefinition formNetworkDefinitionSection)
        {
            if (m_formNetworkDefinitionSection != null)
            {
                formNetworkDefinitionSection = m_formNetworkDefinitionSection;
                m_formNetworkDefinitionSection.Focus();
                return true;
            }
            else
            {
                formNetworkDefinitionSection = null;
                return false;
            }
        }
		public static bool IsFormPCIDocumentOpen(out FormPCIDocument formPCIDocument)
		{
			if (m_formPCIDocument != null)
			{
				formPCIDocument = m_formPCIDocument;
				m_formPCIDocument.Focus();
				return true;
			}
			else
			{
				formPCIDocument = null;
				return false;
			}
		}

        public static void RemoveFormNetworkDefinitionSection(FormNetworkDefinition form)
        {
            m_formNetworkDefinitionSection.Dispose();
            m_formNetworkDefinitionSection = null;
        }

        public static void AddFormNetworkDefinitionSection(FormNetworkDefinition form)
        {
            m_formNetworkDefinitionSection = form;
        }

        public static FormNetworkDefinition formNetworkDefinitionSection
        {
            get
            {
                return m_formNetworkDefinitionSection;
            }
        }

		public static void RemovePCIDocument(FormPCIDocument form)
		{
			m_formPCIDocument.Dispose();
			m_formPCIDocument = null;
		}

		public static void AddPCIDocument(FormPCIDocument form)
		{
			m_formPCIDocument = form;
		}

		public static FormPCIDocument formPCIDocument
		{
			get
			{
				return m_formPCIDocument;
			}
		}

        #endregion

        #region SolutionExplorer
        public static bool IsSolutionExplorerOpen()
        {
            if (m_formSolutionExplorer != null)
            {
				//dsmelser 2008.10.17
				m_formSolutionExplorer.Show( m_dockPanel, DockState.DockLeft );
                m_formSolutionExplorer.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static SolutionExplorer GetSolutionExplorer()
        {
            return m_formSolutionExplorer;
        }

        public static void RemoveSolutionExplorer(SolutionExplorer form)
        {
            m_formSolutionExplorer.Dispose();
            m_formSolutionExplorer = null;
        }

        public static void AddSolutionExplorer(SolutionExplorer form)
        {
            m_formSolutionExplorer = form;
        }

        public static void AddSolutionExplorerNetworkViewers(String strNetwork, String strNetworkID)
        {
            m_formSolutionExplorer.AddViewersAfterRollup(strNetwork, strNetworkID);
        }

        #endregion

        #region OutputWindow
        public static bool IsOutputWindowOpen()
        {
            if (m_formOutputWindow != null)
            {
				m_formOutputWindow.Show( m_dockPanel, DockState.DockBottom );
                m_formOutputWindow.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveOutputWindow(FormOutputWindow form)
        {
            m_formOutputWindow.Dispose();
            m_formOutputWindow = null;
        }

        public static void AddOutputWindow(FormOutputWindow form)
        {
            m_formOutputWindow = form;
        }

        public static FormOutputWindow GetOutputWindow()
        {
            return m_formOutputWindow;
        }

        #endregion

        #region FormGISLayerManager
        public static bool IsFormGISLayerManagerOpen(out FormGISLayerManager formGISLayerManager)
        {
            if (m_formGISLayerManager != null)
            {
                formGISLayerManager = m_formGISLayerManager;
                m_formGISLayerManager.Focus();
                return true;
            }
            else
            {
                formGISLayerManager = null;
                return false;
            }
        }

        public static void RemoveFormGISLayerManager(FormGISLayerManager form)
        {
            m_formGISLayerManager.Dispose();
            m_formGISLayerManager = null;
        }

        public static void AddFormGISLayerManager(FormGISLayerManager form)
        {
            m_formGISLayerManager = form;
        }

        public static FormGISLayerManager formGISLayerManager
        {
            get
            {
                return m_formGISLayerManager;
            }
        }
        #endregion

		#region AttributeTab
		public static bool IsAttributeTabOpen(out AttributeTab attributeTab)
		{
			if (m_attributeTab != null)
			{
				attributeTab = m_attributeTab;
				m_attributeTab.Show();
				return true;
			}
			else
			{
				attributeTab = null;
				return false;
			}
		}

		public static void RemoveAttributeTab(AttributeTab form)
		{
			m_attributeTab.Dispose();
			m_attributeTab = null;
		}

		public static void AddAttributeTab(AttributeTab form)
		{
			m_attributeTab = form;
		}

		public static AttributeTab attributeTab
		{
			get
			{
				return m_attributeTab;
			}
		}
		#endregion

		#region AssetTab
		public static bool IsAssetTabOpen(out AssetTab assetTab)
		{
			if (m_assetTab != null)
			{
				assetTab = m_assetTab;
				m_assetTab.Show();
				return true;
			}
			else
			{
				assetTab = null;
				return false;
			}
		}

		public static void RemoveAssetTab(AssetTab form)
		{
			m_assetTab.Dispose();
			m_assetTab = null;
		}

		public static void AddAssetTab(AssetTab form)
		{
			m_assetTab = form;
		}

		public static AssetTab assetTab
		{
			get
			{
				return m_assetTab;
			}
		}

		#endregion

		#region FormResultsBudget

		public static bool IsResultsBudgetOpen(out FormResultsBudget form)
        {
            if (m_formResultsBudget != null)
            {
                m_formResultsBudget.Focus();
                form = m_formResultsBudget;
                return true;
            }
            else
            {
                form = null;
                return false;
            }
        }

        public static void RemoveResultsBudgetWindow()
        {
            if (m_formResultsBudget != null)
            {
                m_formResultsBudget.Dispose();
                m_formResultsBudget = null;
            }
        }

        public static void AddResultsBudgetWindow(FormResultsBudget form)
        {
            m_formResultsBudget = form;
        }

        public static FormResultsBudget GetResultsBudgetWindow()
        {
            return m_formResultsBudget;
        }
        #endregion

        #region FormResultsTarget

        public static bool IsResultsTargetOpen(out FormResultsTarget form)
        {
            if (m_formResultsTarget != null)
            {
                m_formResultsTarget.Focus();
                form = m_formResultsTarget;
                return true;
            }
            else
            {
                form = null;
                return false;
            }
        }

        public static void RemoveResultsTargetWindow()
        {
            if (m_formResultsTarget != null)
            {
                m_formResultsTarget.Dispose();
                m_formResultsTarget = null;
            }
        }

        public static void AddResultsTargetWindow(FormResultsTarget form)
        {
            m_formResultsTarget = form;
        }

        public static FormResultsTarget GetResultsTargetWindow()
        {
            return m_formResultsTarget;
        }
        #endregion

        #region FormResultsDeficient

        public static bool IsResultsDeficientOpen(out FormResultsDeficient form)
        {
            if (m_formResultsDeficient != null)
            {
                m_formResultsDeficient.Focus();
                form = m_formResultsDeficient;
                return true;
            }
            else
            {
                form = null;
                return false;
            }
        }

        public static void RemoveResultsDeficientWindow()
        {
            if (m_formResultsDeficient != null)
            {
                m_formResultsDeficient.Dispose();
                m_formResultsDeficient = null;
            }
        }

        public static void AddResultsDeficientWindow(FormResultsDeficient form)
        {
            m_formResultsDeficient = form;
        }

        public static FormResultsDeficient GetResultsDeficientWindow()
        {
            return m_formResultsDeficient;
        }
        #endregion

        #region FormImportGeometries
        public static bool IsFormImportGeometriesOpen(out FormImportGeometries formImportGeometries)
        {
            if (m_formImportGeometries != null)
            {
                formImportGeometries = m_formImportGeometries;
                m_formImportGeometries.Focus();
                return true;
            }
            else
            {
                formImportGeometries = null;
                return false;
            }
        }

        public static void RemoveFormImportGeometries(FormImportGeometries form)
        {
            m_formImportGeometries.Dispose();
            m_formImportGeometries = null;
        }

        public static void AddFormImportGeometries(FormImportGeometries form)
        {
            m_formImportGeometries = form;
        }

        public static FormImportGeometries formImportGeometries
        {
            get
            {
                return m_formImportGeometries;
            }
        }
        #endregion

        #region FormImportShapefile
        public static bool IsFormImportShapefileOpen(out FormImportShapeFile formImportShapefile)
        {
            if (m_formImportShapefile != null)
            {
                formImportShapefile = m_formImportShapefile;
                m_formImportShapefile.Focus();
                return true;
            }
            else
            {
                formImportShapefile = null;
                return false;
            }
        }

        public static void RemoveFormImportShapeFile(FormImportShapeFile form)
        {
            m_formImportShapefile.Dispose();
            m_formImportShapefile = null;
        }

        public static void AddFormImportShapeFile(FormImportShapeFile form)
        {
            m_formImportShapefile = form;
        }

        public static FormImportShapeFile formImportShapefile
        {
            get
            {
                return m_formImportShapefile;
            }
        }
        #endregion




        #region CloseAllTabs
        public static void CloseSegmentationTabs()
        {
            while (listFormSegmentation.Count > 0)
            {
                listFormSegmentation[listFormSegmentation.Count - 1].Close();
            }
            while (listFormSegmentationResult.Count > 0)
            {
                listFormSegmentationResult[listFormSegmentationResult.Count - 1].Close();
            }
            while (listFormRollupSegmentation.Count > 0)
            {
                listFormRollupSegmentation[listFormRollupSegmentation.Count - 1].Close();
            }
        }

        public static void CloseSimulationTabs()
        {
            while (listFormAnalysis.Count > 0)
            {
                listFormAnalysis[listFormAnalysis.Count - 1].Close();
            }
            while (listFormInvestment.Count > 0)
            {
                listFormInvestment[listFormInvestment.Count - 1].Close();
            }
            while (listFormTreatment.Count > 0)
            {
                listFormTreatment[listFormTreatment.Count - 1].Close();
            }
            while (listFormPerformanceEquations.Count > 0)
            {
                listFormPerformanceEquations[listFormPerformanceEquations.Count - 1].Close();
            }
            while (listFormSimulationCommitted.Count > 0)
            {
                listFormSimulationCommitted[listFormSimulationCommitted.Count - 1].Close();
            }
            while (listFormSimulationResults.Count > 0)
            {
                listFormSimulationResults[listFormSimulationResults.Count - 1].Close();
            }
        }

        public static void CloseNetworkTabs()
        {
            while (listformAttributeViews.Count > 0)
            {
                listformAttributeViews[listformAttributeViews.Count - 1].Close();
            }
            while (listFormGISView.Count > 0)
            {
                listFormGISView[listFormGISView.Count - 1].Close();
            }
            while (listFormSegmentedConstruction.Count > 0)
            {
                listFormSegmentedConstruction[listFormSegmentedConstruction.Count - 1].Close();
            }
        }

        public static void CloseAttributeTab(String strRawAttribute)
        {
            FormAttributeDocument formAttributeDocument = listformAttributeDocuments.Find(delegate(FormAttributeDocument form) { return form.Tag.ToString() == strRawAttribute; });
            if (formAttributeDocument != null)
            {
                formAttributeDocument.Close();
            }
        }
        #endregion

        public static void CloseMultipleSimulations(String strNetworkID)
        {
            String strQuery = "SELECT SIMULATIONID FROM SIMULATIONS WHERE NETWORKID = '" + strNetworkID + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strQuery);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CloseSingleSimulation(dr[0].ToString());
                }
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Could not get simulation ID from SIMULATIONS table. " + exc.Message);
            }
        }
        public static void CloseSingleSimulation(String strSimulationID)
        {
            int i = 0;
            while (i < listFormAnalysis.Count)
            {
                if (listFormAnalysis[i].Tag.ToString() == strSimulationID)
                {
                    listFormAnalysis[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < listFormInvestment.Count)
            {
                if (listFormInvestment[i].Tag.ToString() == strSimulationID)
                {
                    listFormInvestment[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < listFormPerformanceEquations.Count)
            {
                if (listFormPerformanceEquations[i].Tag.ToString() == strSimulationID)
                {
                    listFormPerformanceEquations[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < listFormTreatment.Count)
            {
                if (listFormTreatment[i].Tag.ToString() == strSimulationID)
                {
                    listFormTreatment[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < listFormSimulationResults.Count)
            {
                if (listFormSimulationResults[i].Tag.ToString() == strSimulationID)
                {
                    listFormSimulationResults[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < listFormSimulationCommitted.Count)
            {
                if (listFormSimulationCommitted[i].Tag.ToString() == strSimulationID)
                {
                    listFormSimulationCommitted[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < listFormSimulationAttributes.Count)
            {
                if (listFormSimulationAttributes[i].Tag.ToString() == strSimulationID)
                {
                    listFormSimulationAttributes[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
        }
        public static void CloseNetworkViewers(String strNetworkID)
        {
            CloseMultipleSimulations(strNetworkID);
            int i = 0;
            while (i < listformAttributeViews.Count)
            {
                if (listformAttributeViews[i].NetworkID == strNetworkID)
                {
                    listformAttributeViews[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < listFormSegmentation.Count)
            {
                if (listFormSegmentation[i].NetworkID == strNetworkID)
                {
                    listFormSegmentation[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < listFormSegmentationResult.Count)
            {
                if (listFormSegmentationResult[i].NetworkID == strNetworkID)
                {
                    listFormSegmentationResult[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < listFormSectionView.Count)
            {
                if (listFormSectionView[i].NetworkID == strNetworkID)
                {
                    listFormSectionView[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < listFormGISView.Count)
            {
                if (listFormGISView[i].NetworkID == strNetworkID)
                {
                    listFormGISView[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < listFormSegmentedConstruction.Count)
            {
                if (listFormSegmentedConstruction[i].NetworkID == strNetworkID)
                {
                    listFormSegmentedConstruction[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
        }
        public static void CloseFormSegmentationCriteria(String strNetworkID)
        {
            int i = 0;
            while (i < listFormSegmentationResult.Count)
            {
                if (listFormSegmentationResult[i].NetworkID == strNetworkID)
                {
                    listFormSegmentationResult[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
        }
        public static void CloseFormSegmentedConstruction(String strNetworkID)
        {
            int i = 0;
            while (i < listFormSegmentedConstruction.Count)
            {
                if (listFormSegmentedConstruction[i].NetworkID == strNetworkID)
                {
                    listFormSegmentedConstruction[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
        }
        public static void CloseFormRollup(String strNetworkID)
        {
            int i = 0;
            while (i < listFormRollupSegmentation.Count)
            {
                if (listFormRollupSegmentation[i].NetworkID == strNetworkID)
                {
                    listFormRollupSegmentation[i].Close();
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
		}


		#region FormSecurityUsers
		public static bool IsFormSecurityUsersOpen( out FormSecurityUsers formSecurityUsers )
		{
			formSecurityUsers = m_formSecurityUsers;
			return m_formSecurityUsers != null;
		}
		public static void AddFormSecurityUsers( FormSecurityUsers formSecurityUsers )
		{
			m_formSecurityUsers = formSecurityUsers;
		}

		public static void RemoveFormSecurityUsers()
		{
			m_formSecurityUsers = null;
		}
		#endregion

		#region FormSecurityUserGroups
		public static bool IsFormSecurityUserGroupsOpen( out FormSecurityUserGroups formSecurityUserGroups )
		{
			formSecurityUserGroups = m_formSecurityUserGroups;
			return m_formSecurityUserGroups != null; 
		}
		public static void AddFormSecurityUserGroups( FormSecurityUserGroups formSecurityUserGroups )
		{
			m_formSecurityUserGroups = formSecurityUserGroups;
		}

		public static void RemoveFormSecurityUserGroups()
		{
			m_formSecurityUserGroups = null;
		}
		#endregion

		#region FormSecurityAction
		public static bool IsFormSecurityActionsOpen( out FormSecurityActions formSecurityActions )
		{
			formSecurityActions = m_formSecurityActions;
			return m_formSecurityActions != null;
		}

		public static void AddFormSecurityActions( FormSecurityActions formSecurityActions )
		{
			m_formSecurityActions = formSecurityActions;
		}

		public static void RemoveFormSecurityActions()
		{
			m_formSecurityActions = null;
		}
		#endregion

		#region FormSecurityActionGroups
		public static bool IsFormSecurityActionGroupsOpen( out FormSecurityActionGroups formSecurityActionGroups )
		{
			formSecurityActionGroups = m_formSecurityActionGroups;
			return m_formSecurityActionGroups != null;
		}

		public static void AddFormSecurityActionGroups( FormSecurityActionGroups formSecurityActionGroups )
		{
			m_formSecurityActionGroups = formSecurityActionGroups;
		}

		public static void RemoveFormSecurityActionGroups()
		{
			m_formSecurityActionGroups = null;
		}
		#endregion

		#region FormSecurityPermissions
		public static bool IsFormSecurityPermissionsOpen( out FormSecurityPermissions formSecurityPermissions )
		{
			formSecurityPermissions = m_formSecurityPermissions;
			return m_formSecurityPermissions != null;
		}
		public static void AddFormSecurityPermissions( FormSecurityPermissions formSecurityPermissions )
		{
			m_formSecurityPermissions = formSecurityPermissions;
		}
		public static void RemoveFormSecurityPermissions()
		{
			m_formSecurityPermissions = null;
		}
		#endregion
	}
}
