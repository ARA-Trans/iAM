using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Simulation;
using RoadCareDatabaseOperations;
using DatabaseManager;

namespace RoadCare3
{
	public partial class FormCompoundTreatment : BaseForm
	{
		private string _effectedTreatmentAttribute;
		private CompoundTreatment _compoundTreatment = null;
		private int _skipLoadSelection = 0;

		public FormCompoundTreatment(string effectedTreatmentAttribute)
		{
			InitializeComponent();
			_effectedTreatmentAttribute = effectedTreatmentAttribute;
		}

		public FormCompoundTreatment(CompoundTreatment selectedCompoundTreatment, string effectedTreatmentAttribute)
		{
			// Load the compound treatment
			InitializeComponent();
			_compoundTreatment = selectedCompoundTreatment;
			LoadCompoundTreatments();
			LoadCompoundTreatmentElementsGrid();

			_effectedTreatmentAttribute = effectedTreatmentAttribute;
		}

		private void LoadCompoundTreatments()
		{
			dgvCompoundTreatments.Rows.Clear();
			List<CompoundTreatment> compoundTreatments = DBOp.GetCompoundTreatments();
			foreach(CompoundTreatment toLoad in compoundTreatments)
			{
				dgvCompoundTreatments.Rows.Add(toLoad, toLoad.AffectedAttribute, toLoad.CompoundTreatmentName);
			}

			// If the user is selecting an existing compound treatment, then we need to select it in the grid, and bring up its elements.
			if(_compoundTreatment != null)
			{
				// Select the passed in compound treatment in the dataGridView
				foreach(DataGridViewRow possibleRow in dgvCompoundTreatments.Rows)
				{
					if (possibleRow.Cells["colCompoundTreatmentName"].Value != null)
					{
						if (possibleRow.Cells["colCompoundTreatmentName"].Value.ToString() == _compoundTreatment.CompoundTreatmentName)
						{
							possibleRow.Selected = true;
						}
					}
				}
			}
		}

		/// <summary>
		/// Loads the From Attribute and To Attribute drop down box at the specified row index with all attributes.
		/// </summary>
		private void LoadFromToAttributes(int rowIndex)
		{
			// Always load the attributes into the empty combo boxes
			DataGridViewComboBoxCell comboBoxAttributesFrom = comboBoxAttributesFrom = (DataGridViewComboBoxCell)dataGridViewCompoundTreatmentElements.Rows[dataGridViewCompoundTreatmentElements.Rows.Count - 1].Cells["colAttributeFrom"];
			DataGridViewComboBoxCell comboBoxAttributesTo = comboBoxAttributesTo = (DataGridViewComboBoxCell)dataGridViewCompoundTreatmentElements.Rows[dataGridViewCompoundTreatmentElements.Rows.Count - 1].Cells["colAttributeTo"]; ;
			foreach (string attribute in Global.Attributes)
			{
				comboBoxAttributesFrom.Items.Add(attribute);
				comboBoxAttributesTo.Items.Add(attribute);
			}
		}

		private void LoadCompoundTreatmentElementsGrid()
		{
			dataGridViewCompoundTreatmentElements.Rows.Clear();
			int addedRowIndex = 0;
			if (_compoundTreatment != null)
			{
				DataGridViewComboBoxCell comboBoxAttributesFrom = null;
				DataGridViewComboBoxCell comboBoxAttributesTo = null;
				foreach (CompoundTreatmentElement element in _compoundTreatment.CompoundTreatmentElements)
				{
					addedRowIndex = dataGridViewCompoundTreatmentElements.Rows.Add();
					comboBoxAttributesFrom = (DataGridViewComboBoxCell)dataGridViewCompoundTreatmentElements.Rows[addedRowIndex].Cells["colAttributeFrom"];
					comboBoxAttributesTo = (DataGridViewComboBoxCell)dataGridViewCompoundTreatmentElements.Rows[addedRowIndex].Cells["colAttributeTo"];
					foreach (string attribute in Global.Attributes)
					{
						comboBoxAttributesFrom.Items.Add(attribute);
						comboBoxAttributesTo.Items.Add(attribute);
					}

					dataGridViewCompoundTreatmentElements.Rows[addedRowIndex].Cells["colCompoundTreatmentElement"].Value = element;
					dataGridViewCompoundTreatmentElements.Rows[addedRowIndex].Cells["colAttributeFrom"].Value = element.AttributeFrom;
					dataGridViewCompoundTreatmentElements.Rows[addedRowIndex].Cells["colAttributeTo"].Value = element.AttributeTo;
					dataGridViewCompoundTreatmentElements.Rows[addedRowIndex].Cells["colExtent"].Value = element.ExtentEquation.m_expression;
					dataGridViewCompoundTreatmentElements.Rows[addedRowIndex].Cells["colQuantity"].Value = element.Quantity.m_expression;
					dataGridViewCompoundTreatmentElements.Rows[addedRowIndex].Cells["colCriteria"].Value = element.CriteriaEquation.Criteria;
					dataGridViewCompoundTreatmentElements.Rows[addedRowIndex].Cells["colCost"].Value = element.CostEquation.m_expression;
				}
			}
			LoadFromToAttributes(addedRowIndex);
		}

		private void dataGridViewCompoundTreatmentElements_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			// Extent column or Quantity column
			if (e.ColumnIndex != -1)
			{
					// Select only one element at a time for editing
				if (dataGridViewCompoundTreatmentElements.SelectedCells.Count == 1)
				{
					// If the selected cell contains data
					if (e.RowIndex != -1)
					{
						CompoundTreatmentElement selectedCompoundElement = (CompoundTreatmentElement)dataGridViewCompoundTreatmentElements["colCompoundTreatmentElement", e.RowIndex].Value;
						if (selectedCompoundElement != null)
						{
							if (dataGridViewCompoundTreatmentElements.Columns["colExtent"].Index == e.ColumnIndex)
							{
								// Extent column selected
								FormEditEquation formEditCompoundElementEquation = new FormEditEquation(selectedCompoundElement.ExtentEquation.m_expression, _effectedTreatmentAttribute);
								if (formEditCompoundElementEquation.ShowDialog() == DialogResult.OK)
								{
									selectedCompoundElement.ExtentEquation.m_expression = formEditCompoundElementEquation.Equation;
									dataGridViewCompoundTreatmentElements[e.ColumnIndex, e.RowIndex].Value = formEditCompoundElementEquation.Equation;
								}
							}
							if (dataGridViewCompoundTreatmentElements.Columns["colQuantity"].Index == e.ColumnIndex)
							{
								// Quantity column selected
								FormEditEquation formEditCompoundElementEquation = new FormEditEquation(selectedCompoundElement.Quantity.m_expression, _effectedTreatmentAttribute);
								if (formEditCompoundElementEquation.ShowDialog() == DialogResult.OK)
								{
									selectedCompoundElement.Quantity.m_expression = formEditCompoundElementEquation.Equation;
									dataGridViewCompoundTreatmentElements[e.ColumnIndex, e.RowIndex].Value = formEditCompoundElementEquation.Equation;
								}
							}
							if (dataGridViewCompoundTreatmentElements.Columns["colCriteria"].Index == e.ColumnIndex)
							{
								// Criteria column selected
								FormAdvancedSearch formEditCriteria = new FormAdvancedSearch(selectedCompoundElement.CriteriaEquation.Criteria);
								if (formEditCriteria.ShowDialog() == DialogResult.OK)
								{
									selectedCompoundElement.CriteriaEquation.Criteria = formEditCriteria.GetWhereClause();
									dataGridViewCompoundTreatmentElements[e.ColumnIndex, e.RowIndex].Value = formEditCriteria.GetWhereClause();
								}
							}
							if (dataGridViewCompoundTreatmentElements.Columns["colCost"].Index == e.ColumnIndex)
							{
								// Cost column selected
								FormEditEquation formEditCompoundElementExtent = new FormEditEquation(selectedCompoundElement.CostEquation.m_expression, _effectedTreatmentAttribute);
								if (formEditCompoundElementExtent.ShowDialog() == DialogResult.OK)
								{
									selectedCompoundElement.CostEquation.m_expression = formEditCompoundElementExtent.Equation;
									dataGridViewCompoundTreatmentElements[e.ColumnIndex, e.RowIndex].Value = formEditCompoundElementExtent.Equation;
								}
							}
						}
					}
					else
					{
						if (dataGridViewCompoundTreatmentElements.Columns["colExtent"].Index == e.ColumnIndex)
						{
							// Extent column selected
							FormEditEquation formEditCompoundElementEquation = new FormEditEquation("", _effectedTreatmentAttribute);
							if (formEditCompoundElementEquation.ShowDialog() == DialogResult.OK)
							{
								dataGridViewCompoundTreatmentElements[e.ColumnIndex, e.RowIndex].Value = formEditCompoundElementEquation.Equation;
							}
						}
						if (dataGridViewCompoundTreatmentElements.Columns["colQuantity"].Index == e.ColumnIndex)
						{
							// Quantity column selected
							FormEditEquation formEditCompoundElementEquation = new FormEditEquation("", _effectedTreatmentAttribute);
							if (formEditCompoundElementEquation.ShowDialog() == DialogResult.OK)
							{
								dataGridViewCompoundTreatmentElements[e.ColumnIndex, e.RowIndex].Value = formEditCompoundElementEquation.Equation;
							}
						}
						if (dataGridViewCompoundTreatmentElements.Columns["colCriteria"].Index == e.ColumnIndex)
						{
							// Criteria column selected
							FormAdvancedSearch formEditCriteria = new FormAdvancedSearch("");
							if (formEditCriteria.ShowDialog() == DialogResult.OK)
							{
								dataGridViewCompoundTreatmentElements[e.ColumnIndex, e.RowIndex].Value = formEditCriteria.GetWhereClause();
							}
						}
						if (dataGridViewCompoundTreatmentElements.Columns["colCost"].Index == e.ColumnIndex)
						{
							// Cost column selected
							FormEditEquation formEditCompoundElementExtent = new FormEditEquation("", _effectedTreatmentAttribute);
							if (formEditCompoundElementExtent.ShowDialog() == DialogResult.OK)
							{
								dataGridViewCompoundTreatmentElements[e.ColumnIndex, e.RowIndex].Value = formEditCompoundElementExtent.Equation;
							}
						}
					}
				}
			}
		}

		private void dataGridViewCompoundTreatmentElements_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			// YESSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
		}

		private void dgvCompoundTreatments_SelectionChanged(object sender, EventArgs e)
		{
			// For some reason this event fires twice when we load hte form, so I just put in a counter to stop unecessary database transactions from occuring.
			if (_skipLoadSelection < 2)
			{
				_skipLoadSelection++;
			}
			else
			{
				SaveCurrentCompoundTreatmentChanges();
			}
			if(dgvCompoundTreatments.SelectedRows.Count == 1)
			{
				// Load up the elements, if they select more than one Compound Treatment at a time, call them a loser and select the first.
				if (dgvCompoundTreatments.SelectedRows[0].Cells["colCompoundTreatments"].Value != null)
				{
					_compoundTreatment = (CompoundTreatment)dgvCompoundTreatments.SelectedRows[0].Cells["colCompoundTreatments"].Value;
					LoadCompoundTreatmentElementsGrid();
				}
				else
				{
					_compoundTreatment = null;
					dataGridViewCompoundTreatmentElements.Rows.Clear();
				}
			}
		}

		private void FormCompoundTreatment_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveCurrentCompoundTreatmentChanges();
		}

		/// <summary>
		/// Saves any changes made to the currently selected compound treatment, including its associated elements to the database.
		/// </summary>
		private void SaveCurrentCompoundTreatmentChanges()
		{
			// Navigation off of a blank row will produce a null value for _compoundTreatment.  No changes are necessary in the database in this case.
			if (_compoundTreatment != null)
			{
				// Statements for database changes.
				List<string> changeStatements = new List<string>();

				// Delete from the compound treatment table our current compound treatment.  This should cascade and remove its elements from teh element table as well.
				string delete = "DELETE FROM COMPOUND_TREATMENTS WHERE COMPOUND_TREATMENT_ID = '" + _compoundTreatment.CompoundTreatmentID + "'";
				changeStatements.Add(delete);

				// Now insert the current compound treatment
				string insert = "INSERT INTO COMPOUND_TREATMENTS (COMPOUND_TREATMENT_NAME, AFFECTED_ATTRIBUTE, COMPOUND_TREATMENT_ID) VALUES ('" +
					_compoundTreatment.CompoundTreatmentName +
					"', '" + _compoundTreatment.AffectedAttribute + 
					"', '" + _compoundTreatment.CompoundTreatmentID + "')";
				changeStatements.Add(insert);

				foreach (DataGridViewRow elementRow in dataGridViewCompoundTreatmentElements.Rows)
				{
					CompoundTreatmentElement toInsert = (CompoundTreatmentElement)elementRow.Cells["colCompoundTreatmentElement"].Value;
					if (toInsert != null)
					{
						// Insert the data.
						insert = "INSERT INTO COMPOUND_TREATMENT_ELEMENTS (ATTRIBUTE_FROM, ATTRIBUTE_TO, EXTENT_, QUANTITY_, CRITERIA_, COST_, COMPOUND_TREATMENT_ID) VALUES ('" +
							toInsert.AttributeFrom +
							"', '" + toInsert.AttributeTo +
							"', '" + toInsert.ExtentEquation.m_expression +
							"', '" + toInsert.Quantity.m_expression +
							"', '" + toInsert.CriteriaEquation.Criteria +
							"', '" + toInsert.CostEquation.m_expression +
							"', '" + toInsert.CompoundTreatmentID + "')";
						changeStatements.Add(insert);
					}
				}
				try
				{
					DBMgr.ExecuteBatchNonQuery(changeStatements);
				}
				catch (Exception exc)
				{
					SimulationMessaging.AddMessage(new SimulationMessage("Error updating database with new compound treatment values.  Transaction aborted. " + exc.Message));
				}
			}
		}

		private void dataGridViewCompoundTreatmentElements_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			LoadFromToAttributes(e.RowIndex);
		}

		private void dataGridViewCompoundTreatmentElements_SelectionChanged(object sender, EventArgs e)
		{
			
		}

		private void dataGridViewCompoundTreatmentElements_UserAddedRow(object sender, DataGridViewRowEventArgs e)
		{
			DataGridViewRow selectedRow = dataGridViewCompoundTreatmentElements.SelectedRows[0];
			if (dataGridViewCompoundTreatmentElements.SelectedRows[0].IsNewRow)
			{
				if (selectedRow.Cells["colAttributeFrom"].Value != null &&
					selectedRow.Cells["colAttributeTo"].Value != null &&
					selectedRow.Cells["colExtent"].Value != null &&
					selectedRow.Cells["colQuantity"].Value != null)
				{
					string attributeFrom = selectedRow.Cells["colAttributeFrom"].Value.ToString();
					string attributeTo = selectedRow.Cells["colAttributeTo"].Value.ToString();
					string extent = selectedRow.Cells["colExtent"].Value.ToString();
					string quantity = selectedRow.Cells["colQuantity"].Value.ToString();
					string cost = "";
					string criteria = "";
					if (!String.IsNullOrEmpty(selectedRow.Cells["colCost"].Value.ToString()))
					{
						cost = selectedRow.Cells["colCost"].Value.ToString();
					}
					if (!String.IsNullOrEmpty(selectedRow.Cells["colCriteria"].Value.ToString()))
					{
						criteria = selectedRow.Cells["colCriteria"].Value.ToString();
					}
					// Make a new compound treatment element and store it in the row.
					CompoundTreatmentElement createdElement = new CompoundTreatmentElement(_compoundTreatment.CompoundTreatmentID, _compoundTreatment.CompoundTreatmentName, attributeFrom, attributeTo, cost, extent, quantity, criteria);

				}
			}
		}
	}
}
