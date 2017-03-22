using Mapsui.Styles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Mapsui.Geometries;

namespace Mapsui.Overlays
{
	///
	/// A ScaleBarOverlay displays the ratio of a distance on the map to the corresponding distance on the ground.
	///
	public class ScaleBarOverlay : BaseOverlay
	{
		public enum ScaleBarPositionEnum { BottomCenter, BottomLeft, BottomRight, TopCenter, TopLeft, TopRight, XYRight, XYCenter, XYLeft }
		public enum ScaleBarModeEnum { Single, Both };

		///
		/// Internal class used by calculateScaleBarLengthAndValue
		///
		public class ScaleBarLengthAndValue
		{
			public int ScaleBarLength;
			public int ScaleBarValue;
			public string ScaleBarText;

			public ScaleBarLengthAndValue(int scaleBarLength, int scaleBarValue, string scaleBarText)
			{
				ScaleBarLength = scaleBarLength;
				ScaleBarValue = scaleBarValue;
				ScaleBarText = scaleBarText ?? string.Empty;
			}
		}

		///
		/// Default position of the scale bar.
		///
		private static readonly ScaleBarPositionEnum DefaultScaleBarPosition = ScaleBarPositionEnum.BottomLeft;
		private static readonly ScaleBarModeEnum DefaultScaleBarMode = ScaleBarModeEnum.Single;

		//static readonly double LatitudeRedrawThreshold = 0.2;

		protected IUnitConverter _unitConverter;
		protected IUnitConverter _secondaryUnitConverter;
		protected bool _redrawNeeded;
		protected ScaleBarPositionEnum _scaleBarPosition;
		protected ScaleBarModeEnum _scaleBarMode;
		int _marginHorizontal;
		int _marginVertical;
		Color _foregroundColor = new Color(0, 0, 0);
		Color _backgroundColor = new Color(255, 255, 255);
		object _image;
		int _width;
		int _height;
		double _lastResolution = double.MaxValue;

		public ScaleBarOverlay()
		{
			_scaleBarPosition = DefaultScaleBarPosition;
			_scaleBarMode = DefaultScaleBarMode;

			Enabled = true;
			Exclusive = false;

			Name = "ScaleBarOverlay";

			_unitConverter = MetricUnitConverter.Instance;
			_redrawNeeded = true;
		}

		public object Image
		{
			get
			{
				return _image;
			}
			set
			{
				if (_image == value)
					return;

				_image = value;
				OnPropertyChanged();
			}
		}

		public int Width
		{
			get
			{
				return _width;
			}
			set
			{
				if (_width == value)
					return;

				_width = value;
				_redrawNeeded = true;
				OnPropertyChanged();
			}
		}

		public int Height
		{
			get
			{
				return _height;
			}
			set
			{
				if (_height == value)
					return;

				_height = value;
				_redrawNeeded = true;
				OnPropertyChanged();
			}
		}

		public Color ForegroundColor
		{
			get
			{
				return _foregroundColor;
			}
			set
			{
				if (_foregroundColor == value)
					return;
				_foregroundColor = value;
				OnPropertyChanged();
			}
		}

		public Color BackgroundColor
		{
			get
			{
				return _backgroundColor;
			}
			set
			{
				if (_backgroundColor == value)
					return;
				_backgroundColor = value;
				OnPropertyChanged();
			}
		}

		public IUnitConverter UnitConverter
		{
			get { return _unitConverter; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("UnitConverter must not be null");
				}
				if (_unitConverter == value)
				{
					return;
				}

				_unitConverter = value;
				_redrawNeeded = true;
			}
		}

		public IUnitConverter SecondaryUnitConverter
		{
			get { return _secondaryUnitConverter; }
			set
			{
				if (_secondaryUnitConverter == value)
				{
					return;
				}

				_secondaryUnitConverter = value;
				_redrawNeeded = true;
			}
		}

		public int MarginHorizontal
		{
			get
			{
				return _marginHorizontal;
			}
			set
			{
				if (_marginHorizontal == value)
				{
					return;
				}

				_marginHorizontal = value;
				_redrawNeeded = true;
			}
		}

		public int MarginVertical
		{
			get
			{
				return _marginVertical;
			}
			set
			{
				if (_marginVertical == value)
				{
					return;
				}

				_marginVertical = value;
				_redrawNeeded = true;
			}
		}

		public ScaleBarPositionEnum ScaleBarPosition
		{
			get
			{
				return _scaleBarPosition;
			}
			set
			{
				if (_scaleBarPosition == value)
				{
					return;
				}

				_scaleBarPosition = value;
				_redrawNeeded = true;
			}
		}

		public int PosX { get; set; }
		public int PosY { get; set; }

		public ScaleBarModeEnum ScaleBarMode
		{
			get
			{
				return _scaleBarMode;
			}
			set
			{
				if (_scaleBarMode == value)
				{
					return;
				}

				_scaleBarMode = value;
				_redrawNeeded = true;
			}
		}

		/**
		 * Determines if a redraw is necessary or not
		 *
		 * @return true if redraw is necessary, false otherwise
		 */
		public bool RedrawNeeded
		{
			get
			{
				return _redrawNeeded;
			}
			//		if (this.redrawNeeded || this.prevMapPosition == null)
			//		{
			//			return true;
			//		}

			//		this.map.getMapPosition(this.currentMapPosition);
			//		if (this.currentMapPosition.getScale() != this.prevMapPosition.getScale())
			//		{
			//			return true;
			//		}

			//		double latitudeDiff = Math.abs(this.currentMapPosition.getLatitude() - this.prevMapPosition.getLatitude());
			//		return latitudeDiff > LatitudeRedrawThreshold;
			set
			{
				if (_redrawNeeded == value)
				{
					return;
				}

				_redrawNeeded = value;
			}
		}

		public int CalculatePositionLeft(int left, int right, int width)
		{
			switch (_scaleBarPosition)
			{
				case ScaleBarPositionEnum.BottomLeft:
				case ScaleBarPositionEnum.TopLeft:
					return _marginHorizontal;

				case ScaleBarPositionEnum.BottomCenter:
				case ScaleBarPositionEnum.TopCenter:
					return (right - left - width) / 2;

				case ScaleBarPositionEnum.BottomRight:
				case ScaleBarPositionEnum.TopRight:
					return right - left - width - _marginHorizontal;
				case ScaleBarPositionEnum.XYCenter:
				case ScaleBarPositionEnum.XYLeft:
				case ScaleBarPositionEnum.XYRight:
					return PosX;
			}

			throw new ArgumentException("Unknown horizontal position: " + _scaleBarPosition);
		}

		public int CalculatePositionTop(int top, int bottom, int height)
		{
			switch (_scaleBarPosition)
			{
				case ScaleBarPositionEnum.TopCenter:
				case ScaleBarPositionEnum.TopLeft:
				case ScaleBarPositionEnum.TopRight:
					return _marginVertical;

				case ScaleBarPositionEnum.BottomCenter:
				case ScaleBarPositionEnum.BottomLeft:
				case ScaleBarPositionEnum.BottomRight:
					return bottom - top - height - _marginVertical;
				case ScaleBarPositionEnum.XYCenter:
				case ScaleBarPositionEnum.XYLeft:
				case ScaleBarPositionEnum.XYRight:
					return PosY;
			}

			throw new ArgumentException("Unknown vertical position: " + _scaleBarPosition);
		}

		/// Calculates the required length and value of the scalebar
		///
		/// @param viewport the Viewport to calculate for
		/// @param width of the scale bar in pixel to calculate for
		/// @param unitConverter the DistanceUnitConverter to calculate for
		/// @return a {@link ScaleBarLengthAndValue} object containing the required scaleBarLength and scaleBarValue
		public ScaleBarLengthAndValue CalculateScaleBarLengthAndValue(IViewport viewport, int width, IUnitConverter unitConverter)
		{
			// Get current position
			var position = Projection.SphericalMercator.ToLonLat(viewport.Center.X, viewport.Center.Y);

			// Calc ground resolution in meters per pixel of viewport for this latitude
			double groundResolution = viewport.Resolution * Math.Cos(position.Y / 180.0 * Math.PI);

			// Convert in units of UnitConverter
			groundResolution = groundResolution / unitConverter.MeterRatio;

			int[] scaleBarValues = unitConverter.ScaleBarValues;

			int scaleBarLength = 0;
			int mapScaleValue = 0;

			foreach (int scaleBarValue in scaleBarValues)
			{
				mapScaleValue = scaleBarValue;
				scaleBarLength = (int)(mapScaleValue / groundResolution);
				if (scaleBarLength < (width - 10))
				{
					break;
				}
			}

			var mapScaleText = unitConverter.GetScaleText(mapScaleValue);

			return new ScaleBarLengthAndValue(scaleBarLength, mapScaleValue, mapScaleText);
		}

		/**
		 * Calculates the required length and value of the scalebar using the current {@link DistanceUnitAdapter}
		 *
		 * @return a {@link ScaleBarLengthAndValue} object containing the required scaleBarLength and scaleBarValue
		 */
		public ScaleBarLengthAndValue CalculateScaleBarLengthAndValue(IViewport viewport, int width)
		{
			return CalculateScaleBarLengthAndValue(viewport, width, UnitConverter);
		}

		public override void ViewChanged(bool majorChange, BoundingBox extent, double resolution)
		{
			// If resolution changes, than we need a redraw
			if (_lastResolution != resolution)
			{
				_lastResolution = resolution;
				_redrawNeeded = true;
			}

			// TODO
			// If Center changes for mor than 0.2 degrees, we need a redraw
		}
	}
}