using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurityManager;

namespace RoadCareSecurityOperations
{
	public class RoadCareActionGroup
	{
		private SecurityActionGroup fundamentalGroup;

		public RoadCareActionGroup( SecurityActionGroup basisGroup )
		{
			fundamentalGroup = basisGroup;
		}

		public string Name
		{
			get
			{
				return fundamentalGroup.Name;
			}
		}
	}
}
