using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
	interface ILRSObject
	{        /// <summary>
		/// LRS Route name
		/// </summary>
		String Route
		{
			set;
			get;
		}
		/// <summary>
		/// LRS Begin Station
		/// </summary>
		double BeginStation
		{
			set;
			get;
		}

		/// <summary>
		/// LRS End Station
		/// </summary>
		double EndStation
		{
			set;
			get;
		}

		/// <summary>
		/// LRS Direction
		/// </summary>
		String Direction
		{
			set;
			get;
		}

		/// <summary>
		/// Date of LRS Object
		/// </summary>
		DateTime Date
		{
			set;
			get;
		}
	}
}
