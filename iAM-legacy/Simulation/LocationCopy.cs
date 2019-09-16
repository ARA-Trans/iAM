using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
	public class LocationCopy
	{
		string _route;
		string _section;
		string _direction;
		bool _linear;
		double _start;
		double _end;
		double _area;
		#region Accessors

		public double Area
		{
			get
			{
				return _area;
			}
		}

		public string Route
		{
			get
			{
				return _route;
			}
		}

		public string Section
		{
			get
			{
				return _section;
			}
		}

		public string Direction
		{
			get
			{
				return _direction;
			}
		}

		public bool Linear
		{
			get
			{
				return _linear;
			}
		}

		public double Start
		{
			get
			{
				return _start;
			}
		}

		public double End
		{
			get
			{
				return _end;
			}
		}
		#endregion

		#region Constructors

		public LocationCopy( string route, string direction, double start, double end, double area )
		{
			_linear = true;
			_section = "";

			_route = route;
			_direction = direction;
			_start = start;
			_end = end;
			_area = area;
		}

		public LocationCopy( string route, string section, double area )
		{
			_linear = false;
			_start = 0.0;
			_end = 0.0;
			_direction = "";

			_route = route;
			_section = section;
			_area = area;
		}


		/// <summary>
		/// Hybrid case
		/// </summary>
		/// <param name="route"></param>
		/// <param name="section"></param>
		/// <param name="direction"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public LocationCopy( string route, string section, string direction, double start, double end, double area )
		{
			_linear = true;
			_route = route;
			_direction = direction;
			_section = section;
			_start = start;
			_end = end;
			_area = area;
		}

		#endregion

		public override bool Equals( object obj )
		{
			//return base.Equals( obj );
			bool equal = true;
			if( obj is LocationCopy )
			{
				LocationCopy other = ( LocationCopy )obj;
				equal = ( _route == other._route ) &&
					( _section == other._section ) &&
					( _direction == other._direction ) &&
					( _linear == other._linear ) &&
					( _start == other._start ) &&
					( _end == other._end );
			}
			else
			{
				equal = base.Equals( obj );
			}

			return equal;
		}

		public override int GetHashCode()
		{
			int hash;

			hash = _route.GetHashCode() & _section.GetHashCode() & _direction.GetHashCode() & _linear.GetHashCode() & _start.GetHashCode() & _end.GetHashCode();

			return hash;
		}

	}
}
