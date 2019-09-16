using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class Direction
	{
		string name;
		double beginMilePost;
		double endMilePost;

		public Direction(string directionName, double bmp, double emp)
		{
			name = directionName;
			beginMilePost = bmp;
			endMilePost = emp;
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public double BMP
		{
			get
			{
				return beginMilePost;
			}
		}

		public double EMP
		{
			get
			{
				return endMilePost;
			}
		}
	}
}
