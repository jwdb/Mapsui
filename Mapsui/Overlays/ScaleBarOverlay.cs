using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapsui.Overlays
{
	///
	/// A ScaleBarOverlay displays the ratio of a distance on the map to the corresponding distance on the ground.
	///
	public class ScaleBarOverlay : IOverlay
	{
		public enum ScaleBarPosition { BottomCenter, BottomLeft, BottomRight, TopCenter, TopLeft, TopRight }

		///
		/// Internal class used by calculateScaleBarLengthAndValue
		///
		protected class ScaleBarLengthAndValue
		{
			public int ScaleBarLength;
			public int ScaleBarValue;

			public ScaleBarLengthAndValue(int scaleBarLength, int scaleBarValue)
			{
				ScaleBarLength = scaleBarLength;
				ScaleBarValue = scaleBarValue;
			}
		}

		///
		/// Default position of the scale bar.
		///
		private static readonly ScaleBarPosition DefaultPosition = ScaleBarPosition.BottomLeft;

		private static readonly double LatitudeRedrawThreshold = 0.2;

		private readonly MapPosition currentMapPosition = new MapPosition();
		private MapPosition prevMapPosition;
		protected IUnitConverter unitConverter;
		protected readonly Map map;
		private int marginHorizontal;
		private int marginVertical;
		protected bool redrawNeeded;
		protected ScaleBarPosition position;

		public ScaleBarOverlay(Map map, int width, int height)
		{
			this.map = map;

			position = DefaultPosition;

			unitConverter = MetricUnitConverter.Instance;
			redrawNeeded = true;
		}

		public IUnitConverter UnitConverter
		{
			get { return unitConverter; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("UnitConverter must not be null");
				}
				if (unitConverter == value)
				{
					return;
				}

				unitConverter = value;
				redrawNeeded = true;
			}
		}

		public int MarginHorizontal
		{
			get
			{
				return marginHorizontal;
			}
			set
			{
				if (marginHorizontal == value)
				{
					return;
				}

				marginHorizontal = value;
				redrawNeeded = true;
			}
		}
	
		public int MarginVertical
		{
			get
			{
				return marginVertical;
			}
			set
			{
				if (marginVertical == value)
				{
					return;
				}

				marginVertical = value;
				redrawNeeded = true;
			}
		}

		public ScaleBarPosition Position
		{
			get
			{
				return position;
			}
			set
			{
				if (position == value)
				{
					return;
				}

				position = value;
				redrawNeeded = true;
			}
		}

		private int CalculatePositionLeft(int left, int right, int width)
		{
			switch (position)
			{
				case ScaleBarPosition.BottomLeft:
				case ScaleBarPosition.TopLeft:
					return marginHorizontal;

				case ScaleBarPosition.BottomCenter:
				case ScaleBarPosition.TopCenter:
					return (right - left - width) / 2;

				case ScaleBarPosition.BottomRight:
				case ScaleBarPosition.TopRight:
					return right - left - width - marginHorizontal;
			}

			throw new ArgumentException("Unknown horizontal position: " + position);
		}

		private int CalculatePositionTop(int top, int bottom, int height)
		{
			switch (position)
			{
				case ScaleBarPosition.TopCenter:
				case ScaleBarPosition.TopLeft:
				case ScaleBarPosition.TopRight:
					return marginVertical;

				case ScaleBarPosition.BottomCenter:
				case ScaleBarPosition.BottomLeft:
				case ScaleBarPosition.BottomRight:
					return bottom - top - height - marginVertical;
			}

			throw new ArgumentException("Unknown vertical position: " + position);
		}

		/**
		 * Calculates the required length and value of the scalebar
		 *
		 * @param unitAdapter the DistanceUnitAdapter to calculate for
		 * @return a {@link ScaleBarLengthAndValue} object containing the required scaleBarLength and scaleBarValue
		 */
		protected ScaleBarLengthAndValue calculateScaleBarLengthAndValue(DistanceUnitAdapter unitAdapter)
			{
				this.prevMapPosition = this.map.getMapPosition();
				double groundResolution = MercatorProjection.groundResolution(this.prevMapPosition);

				groundResolution = groundResolution / unitAdapter.getMeterRatio();
				int[] scaleBarValues = unitAdapter.getScaleBarValues();

				int scaleBarLength = 0;
				int mapScaleValue = 0;

				for (int scaleBarValue : scaleBarValues)
				{
					mapScaleValue = scaleBarValue;
					scaleBarLength = (int)(mapScaleValue / groundResolution);
					if (scaleBarLength < (this.mapScaleBitmap.getWidth() - 10))
					{
						break;
					}
				}

				return new ScaleBarLengthAndValue(scaleBarLength, mapScaleValue);
			}

			/**
			 * Calculates the required length and value of the scalebar using the current {@link DistanceUnitAdapter}
			 *
			 * @return a {@link ScaleBarLengthAndValue} object containing the required scaleBarLength and scaleBarValue
			 */
			protected ScaleBarLengthAndValue calculateScaleBarLengthAndValue()
			{
				return calculateScaleBarLengthAndValue(this.DistanceUnitAdapter);
			}

			/**
			 * @param canvas The canvas to use to draw the MapScaleBar
			 */
			public void draw(Canvas canvas)
			{
				if (!this.visible)
				{
					return;
				}

				if (this.map.getHeight() == 0)
				{
					return;
				}

				if (this.isRedrawNecessary())
				{
					redraw(this.mapScaleCanvas);
					this.redrawNeeded = false;
				}

				int positionLeft = calculatePositionLeft(0, this.map.getWidth(), this.mapScaleBitmap.getWidth());
				int positionTop = calculatePositionTop(0, this.map.getHeight(), this.mapScaleBitmap.getHeight());

				canvas.drawBitmap(this.mapScaleBitmap, positionLeft, positionTop);
			}

			/**
			 * The scalebar is redrawn now.
			 */
			public void drawScaleBar()
			{
				draw(mapScaleCanvas);
			}

			/**
			 * The scalebar will be redrawn on the next draw()
			 */
			public void redrawScaleBar()
			{
				this.redrawNeeded = true;
			}

			/**
			 * Determines if a redraw is necessary or not
			 *
			 * @return true if redraw is necessary, false otherwise
			 */
			protected boolean isRedrawNecessary()
			{
				if (this.redrawNeeded || this.prevMapPosition == null)
				{
					return true;
				}

				this.map.getMapPosition(this.currentMapPosition);
				if (this.currentMapPosition.getScale() != this.prevMapPosition.getScale())
				{
					return true;
				}

				double latitudeDiff = Math.abs(this.currentMapPosition.getLatitude() - this.prevMapPosition.getLatitude());
				return latitudeDiff > LatitudeRedrawThreshold;
			}

			/**
			 * Redraw the map scale bar.
			 * Make sure you always apply scale factor to all coordinates and dimensions.
			 *
			 * @param canvas The canvas to draw on
			 */
			protected abstract void redraw(Canvas canvas);
		}
	}