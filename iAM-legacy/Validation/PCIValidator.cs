using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class PCIValidator : IValidator
	{
		private bool correctErrors;
		/// <summary>
		/// Create a new PCIValidator
		/// </summary>
		/// <param name="correctErrors">Turns on the default error correction for this validator</param>
		public PCIValidator(bool fix)
		{
			correctErrors = fix;
		}

		#region IValidator Members

		public void Validate( List<Segment> segments )
		{
			List<string> validTypeValues = new List<string>();
			validTypeValues.Add("RANDOM");
			validTypeValues.Add("ADDITIONAL");
			validTypeValues.Add("IGNORE");





			List<StringPair> allowableDistressTypes = new List<StringPair>();
			allowableDistressTypes.Add(new StringPair("ac.faa", ""));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Alligator Cracking"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Bleeding"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Block Cracking"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Corrugation"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Depression"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Jet Blast Erosion"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Joint Reflection Cracking"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Longitudinal/Transverse Cracking"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Oil Spillage"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Patching/UtilityCut"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Polished Aggregate"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Raveling/Weathering"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Rutting"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Shoving"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Sippage Cracking"));
			allowableDistressTypes.Add(new StringPair("ac.faa", "Swell"));

			allowableDistressTypes.Add(new StringPair("pcc.faa", "Blow-Ups"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Corner Breaks"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Long/Trans/Diag Crack"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "D Cracking"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Small Patch"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Utility Patch"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Popouts"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Pumping"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Scaling"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Settlement"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Shattered Slabs"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Shrinkage Cracks"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Spallling Joints"));
			allowableDistressTypes.Add(new StringPair("pcc.faa", "Spalling Corner"));

			allowableDistressTypes.Add(new StringPair("ac.mpr", ""));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Alligator Cracking"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Bleeding"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Block Cracking"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Bumps and Sags"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Corrugation"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Depression"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Edge Cracking"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Joint Reflection Cracking"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Lane/Shoulder Dropoff"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Longitudinal/Transverse Cracking"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Large Parching/UtilityCut"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Polished Aggregate"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Potholes"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Railroad Crossing"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Rutting"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Shoving"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Slippage Cracking"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Swell"));
			allowableDistressTypes.Add(new StringPair("ac.mpr", "Weathering"));

			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Blow-Ups"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Corner Breaks"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Divided Slab"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "D Cracking"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Faulting"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Joint Seal Damage"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Lane/Shoulder Drop Off"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Linear Cracking"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Large Patching"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Small Patching"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Polished Aggregates"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Popouts"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Pumping"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "PunchOuts"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Railroad Crossing"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Scaling"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Shrinkage Cracks"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Corner Spalling"));
			allowableDistressTypes.Add(new StringPair("pcc.mpr", "Joint Spalling"));


			Dictionary<StringPair, string> correctionMapping = new Dictionary<StringPair, string>();

			correctionMapping[new StringPair("pcc.mpr", "Patching-Large")] = "Large Patching";
			correctionMapping[new StringPair("pcc.mpr", "Patching-Small")] = "Small Patching";
			correctionMapping[new StringPair("pcc.mpr", "Punchouts")] = "PunchOuts";
			correctionMapping[new StringPair("pcc.mpr", "Spalling-Joint")] = "Joint Spalling";
			correctionMapping[new StringPair("pcc.mpr", "Spalling-Corner")] = "Corner Spalling";
			correctionMapping[new StringPair("pcc.mpr", "Corner Break")] = "Corner Breaks";
			correctionMapping[new StringPair("pcc.mpr", "Blow-up")] = "Blow-Ups";

			correctionMapping[new StringPair("ac.mpr", "Longitudinal Cracking")] = "Longitudinal/Transverse Cracking";
			correctionMapping[new StringPair("ac.mpr", "Transverse Cracking")] = "Longitudinal/Transverse Cracking";
			correctionMapping[new StringPair("ac.mpr", "Patching-Utility Cut")] = "Large Parching/UtilityCut";
			correctionMapping[new StringPair("ac.mpr", "Raveling-Weather")] = "Weathering";

			DefinedInNetwork dinValidator = new DefinedInNetwork(correctErrors);
			DefinedInList dilValidator = new DefinedInList("TYPE", validTypeValues, correctErrors);
			GreaterThanValue areaValidator = new GreaterThanValue("Area", 0, correctErrors);
			GreaterThanValue amountValidator = new GreaterThanValue("Amount", 0, correctErrors);

			DependantDefinedInList distressTypeCorrection = new DependantDefinedInList("METHOD", "DISTRESS", allowableDistressTypes, correctionMapping, true);


			ExcludeAllIfAny onlyCorrectSegments = new ExcludeAllIfAny(true);




			//PropertyNameMapping legacyNameFixer = new PropertyNameMapping(

			dinValidator.Validate(segments);
			dilValidator.Validate(segments);
			areaValidator.Validate(segments);
			amountValidator.Validate(segments);
			distressTypeCorrection.Validate(segments);
			onlyCorrectSegments.Validate(segments);
		}

		/// <summary>
		/// Turns on or off the default error correction routines for this7 validator.
		/// </summary>
		public bool CorrectErrors
		{
			get
			{
				return correctErrors;
			}
			set
			{
				correctErrors = value;
			}
		}

		#endregion


	}
}
