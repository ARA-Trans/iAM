using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class LinearFacility
	{
		string name;

		List<Direction> directions;

		public LinearFacility(string facilityName)
		{
			name = facilityName;
			directions = new List<Direction>();
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public List<Direction> Directions
		{
			get
			{
				return directions;
			}
		}
	}
}
