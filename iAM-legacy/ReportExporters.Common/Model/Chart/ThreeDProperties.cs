using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model.Chart
{
	/// <summary>
	/// Properties for 3D layout.
	/// </summary>
	public class ThreeDProperties
	{
		#region Enums
		
		public enum ProjectionMode3DForRendering
		{
			/// <summary>
			/// Default mode
			/// </summary>
			Perspective,
			Orthographic
		}

		public enum ShadingMode
		{
			/// <summary>
			/// Default value
			/// </summary>
			None,
			Simple,
			Real
		}

		public enum DrawingStyleMode
		{
			Cylinder,
			/// <summary>
			/// Default value
			/// </summary>
			Cube
		}
		
		#endregion

		#region Properties

		private bool? _enabled;
		/// <summary>
		/// If Enabled then Chart should be displayed in 3D, elsewere - 2D
		/// </summary>
		public bool? Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		private ProjectionMode3DForRendering _projectionMode;
		/// <summary>
		/// Projection mode used for the 3D rendering
		/// </summary>
		public ProjectionMode3DForRendering ProjectionMode
		{
			get
			{
				return _projectionMode;
			}
			set
			{
				_projectionMode = value;
			}
		}

		private int? _perspective;
		/// <summary>
		/// Percent of perspective. Applies only for Perspective projection.
		/// </summary>
		public int? Perspective
		{
			get
			{
				return _perspective;
			}
			set
			{
				_perspective = value;
			}
		}

		private int? _rotation;
		/// <summary>
		/// Rotation angle
		/// </summary>
		public int? Rotation
		{
			get
			{
				return _rotation;
			}
			set
			{
				_rotation = value;
			}
		}

		private int? _inclination;
		/// <summary>
		/// Inclination angle
		/// </summary>
		public int? Inclination
		{
			get
			{
				return _inclination;
			}
			set
			{
				_inclination = value;
			}
		}

		private ShadingMode _shading;
		/// <summary>
		/// Projection mode used for the 3D rendering
		/// </summary>
		public ShadingMode Shading
		{
			get
			{
				return _shading;
			}
			set
			{
				_shading = value;
			}
		}

		private int? _wallThickness;
		/// <summary>
		/// Percent thickness of outer walls
		/// </summary>
		public int? WallThickness
		{
			get
			{
				return _wallThickness;
			}
			set
			{
				_wallThickness = value;
			}
		}

		private DrawingStyleMode _drawingStyle;
		/// <summary>
		/// Determines shape of chart data displayed.
		/// Applies only to bar and column.
		/// </summary>
		public DrawingStyleMode DrawingStyle
		{
			get
			{
				return _drawingStyle;
			}
			set
			{
				_drawingStyle = value;
			}
		}

		private bool? _clustered;
		/// <summary>
		/// IDetermines if data series are clustered(displayed along distinct rows).
		/// Applies only to bar and column.
		/// </summary>
		public bool? Clustered
		{
			get
			{
				return _clustered;
			}
			set
			{
				_clustered = value;
			}
		}
		
		#endregion

		public ThreeDProperties(ThreeDProperties threeDProperties)
		{
		}
		
		public ThreeDProperties()
		{
			_enabled = false;
			_projectionMode = ProjectionMode3DForRendering.Perspective;
			_shading = ShadingMode.None;
		}
	}
}
