using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurityManager;

namespace RoadCareSecurityOperations
{
	public class RoadCareUserGroup
	{
		private SecurityUserGroup fundamentalGroup;

		public RoadCareUserGroup( SecurityUserGroup basisGroup )
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
