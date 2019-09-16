using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class SectionFacility
	{
		string name;
		List<Section> sections;

		public SectionFacility(string facilityName)
		{
			name = facilityName;
			sections = new List<Section>();
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public List<Section> Sections
		{
			get
			{
				return sections;
			}
		}
	}
}
