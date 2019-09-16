using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
	public class CommitCopy
	{
		TreatmentCopy _treatment;
		List<AttributeChange> _nonStandardConsequences;
		int _yearSame;
		int _yearAny;
		double _originalCost;
		double _newCost;

		string _budget;

		#region Accessors
		public double NewCost
		{
			get
			{
				return _newCost;
			}
			set
			{
				_newCost = value;
			}
		}

		public TreatmentCopy Treatment
		{
			get
			{
				return _treatment;
			}
		}

		public List<AttributeChange> CommitConsequences
		{
			get
			{
				return _nonStandardConsequences;
			}
		}

		public int YearSame
		{
			get
			{
				return _yearSame;
			}
		}

		public int YearAny
		{
			get
			{
				return _yearAny;
			}
		}

		public string Budget
		{
			get
			{
				return _budget;
			}
		}

		public double OriginalCost
		{
			get
			{
				return _originalCost;
			}
		}

		#endregion

		public CommitCopy( TreatmentCopy treatment, int yearSame, int yearAny, string budget, List<AttributeChange> nonStandardConsequences, double originalCost )
		{
			_treatment = treatment;
			_nonStandardConsequences = nonStandardConsequences;
			_yearSame = yearSame;
			_yearAny = yearAny;
			_budget = budget;
			_originalCost = originalCost;
			_newCost = -1.0;
		}

		
	}
}
