﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
	public interface IValidator
	{
		void Validate(List<RoadCareDataObject> toValidate, string type);
	}
}
