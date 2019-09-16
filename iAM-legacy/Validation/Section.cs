using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class Section
	{
		string name;

		public Section(string sectionName)
		{
			name = sectionName;
		}

		public string Name
		{
			get
			{
				return name;
			}
		}
	}
}
