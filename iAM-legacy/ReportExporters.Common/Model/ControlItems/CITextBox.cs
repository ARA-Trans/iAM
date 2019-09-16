using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model.ControlItems
{
	public class CITextBox : ControlItem
	{
		private bool _canGrow;
		public bool CanGrow
		{
			get
			{
				return _canGrow;
			}
			set
			{
				_canGrow = value;
			}
		}

		private string _value;
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		private Action _action;
		public Action Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}

		//private bool _hideDuplicates;
		//public bool HideDuplicates
		//{
		//  get
		//  {
		//    return _hideDuplicates;
		//  }
		//  set
		//  {
		//    _hideDuplicates = value;
		//  }
		//}
		
		public CITextBox() : base()
		{
			_canGrow = true;
		}
	}
}
