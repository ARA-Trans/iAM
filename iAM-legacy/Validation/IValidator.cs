using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public interface IValidator
	{
		bool CorrectErrors
		{
			get;
			set;
		}

		void Validate(List<Segment> segments);
	}
}
