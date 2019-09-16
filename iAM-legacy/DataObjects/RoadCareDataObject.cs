using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
	public abstract class RoadCareDataObject
	{
		private List<string> m_ValidationErrors = new List<string>();

		public List<string> Errors
		{
			get
			{
				return m_ValidationErrors;
			}
		}
	}
}
