﻿using Mapsui.Geometries;
using Mapsui.Styles;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mapsui.Widgets.ScaleBar
{
    /// <summary>
    /// A ScaleBarWidget displays the ratio of a distance on the map to the corresponding distance on the ground.
    /// It uses always the center of a given Viewport to calc this ratio.
    ///
    /// Usage
    /// To show a ScaleBarWidget, add a instance of the ScaleBarWidget to Map.Widgets by
    /// 
    ///   map.Widgets.Add(new ScaleBarWidget(map.Viewport));
    ///   
    /// Customize
    /// ScaleBarMode: Determins, how much scalebars are shown. Could be Single or Both.
    /// SecondaryUnitConverter: First UnitConverter for upper scalebar. There are UnitConverters for metric, imperial and nautical units.
    /// SecondaryUnitConverter = NauticalUnitConverter.Instance });
    /// MaxWidth: Maximal width of the scalebar. Real width could be smaller.
    /// HorizontalAlignment: Where the ScaleBarWidget is shown. Could be Left, Right, Center or Position.
    /// VerticalAlignment: Where the ScaleBarWidget is shown. Could be Top, Bottom, Center or Position.
    /// PositionX: If HorizontalAlignment is Position, this value determins the distance to the left
    /// PositionY: If VerticalAlignment is Position, this value determins the distance to the top
    /// TextColor: Color for text and lines
    /// Halo: Color used around text and lines, so the scalebar is better visible
    /// TextAlignment: Alignment of scalebar text to the lines. Could be Left, Right or Center
    /// TextMargin: Space between text and lines of scalebar
    /// Font: Font which is used to draw text
    /// TickLength: Length of the ticks at scalebar
    /// </summary>
    public class ScaleBarWidget : Widget, INotifyPropertyChanged
    {
        ///
        /// Default position of the scale bar.
        ///
        private static readonly HorizontalAlignment DefaultScaleBarHorizontalAlignment = HorizontalAlignment.Left;
        private static readonly VerticalAlignment DefaultScaleBarVerticalAlignment = VerticalAlignment.Bottom;
        private static readonly Alignment DefaultScaleBarAlignment = Alignment.Left;
        private static readonly ScaleBarMode DefaultScaleBarMode = ScaleBarMode.Single;
        private static readonly Font DefaultFont = new Font { FontFamily = "Arial", Size = 10 };

        public ScaleBarWidget(Viewport viewport)
        {
            Viewport = viewport;

            HorizontalAlignment = DefaultScaleBarHorizontalAlignment;
            VerticalAlignment = DefaultScaleBarVerticalAlignment;

            maxWidth = 100;
            height = 100;
            textAlignment = DefaultScaleBarAlignment;
            scaleBarMode = DefaultScaleBarMode;

            unitConverter = MetricUnitConverter.Instance;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Viewport to use for all calculations
        /// </summary>
        public Viewport Viewport { get; } = null;

        float maxWidth;

        /// <summary>
        /// Maximum usable width for scalebar. The real used width could be less, because we 
        /// want only integers as text.
        /// </summary>
        public float MaxWidth
        {
            get
            {
                return maxWidth;
            }
            set
            {
                if (maxWidth == value)
                    return;

                maxWidth = value;
                OnPropertyChanged();
            }
        }

        float height;

        /// <summary>
        /// Real height of scalebar. Depends on number of unit converters and text size.
        /// Is calculated by renderer.
        /// </summary>
        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                if (height == value)
                    return;

                height = value;
                OnPropertyChanged();
            }
        }

        Color textColor = new Color(0, 0, 0);

        /// <summary>
        /// Foreground color of scalebar and text
        /// </summary>
        public Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                if (textColor == value)
                    return;
                textColor = value;
                OnPropertyChanged();
            }
        }

        Color haloColor = new Color(255, 255, 255);

        /// <summary>
        /// Halo color of scalebar and text, so that it is better visible
        /// </summary>
        public Color Halo
        {
            get
            {
                return haloColor;
            }
            set
            {
                if (haloColor == value)
                    return;
                haloColor = value;
                OnPropertyChanged();
            }
        }

        public float Scale { get; set; } = 1;

        /// <summary>
        /// Length of the ticks
        /// </summary>
        public float TickLength { get; set; } = 3;

        Alignment textAlignment;

        /// <summary>
        /// Alignment of text of scalebar
        /// </summary>
        public Alignment TextAlignment
        {
            get
            {
                return textAlignment;
            }
            set
            {
                if (textAlignment == value)
                    return;

                textAlignment = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Margin between end of tick and text
        /// </summary>
        public float TextMargin { get; set; } = 1;

        private Font font = DefaultFont;

        /// <summary>
        /// Font to use for drawing text
        /// </summary>
        public Font Font
        {
            get
            {
                return font ?? DefaultFont;
            }
            set
            {
                if (font == value)
                    return;

                font = value;
                OnPropertyChanged();
            }
        }

        protected IUnitConverter unitConverter;

        /// <summary>
        /// Normal unit converter for upper text. Default is MetricUnitConverter.
        /// </summary>
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
                OnPropertyChanged();
            }
        }

        protected IUnitConverter secondaryUnitConverter;
        
        /// <summary>
        /// Secondary unit converter for lower text if ScaleBarMode is Both. Default is ImperialUnitConverter.
        /// </summary>
        public IUnitConverter SecondaryUnitConverter
        {
            get { return secondaryUnitConverter; }
            set
            {
                if (secondaryUnitConverter == value)
                {
                    return;
                }

                secondaryUnitConverter = value;
                OnPropertyChanged();
            }
        }

        protected ScaleBarMode scaleBarMode;

        /// <summary>
        /// ScaleBarMode of scalebar. Could be Single to show only one or Both for showing two units.
        /// </summary>
        public ScaleBarMode ScaleBarMode
        {
            get
            {
                return scaleBarMode;
            }
            set
            {
                if (scaleBarMode == value)
                {
                    return;
                }

                scaleBarMode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Draw a rectangle around the scale bar for testing
        /// </summary>
        public bool ShowEnvelop { get; set; } = false;

        /// <summary>
        /// Calculates the length and text for both scalebars
        /// </summary>
        /// <returns>
        /// Length of upper scalebar
        /// Text of upper scalebar
        /// Length of lower scalebar
        /// Text of lower scalebar
        /// </returns>
        public (float scaleBarLength1, string scaleBarText1, float scaleBarLength2, string scaleBarText2) GetScaleBarLengthAndText()
        {
            if (Viewport == null)
                return (0, null, 0, null);

            float length1;
            string text1;

            (length1, text1) = CalculateScaleBarLengthAndValue(Viewport, MaxWidth, UnitConverter);

            float length2;
            string text2;

            if (SecondaryUnitConverter != null)
                (length2, text2) = CalculateScaleBarLengthAndValue(Viewport, MaxWidth, SecondaryUnitConverter);
            else
                (length2, text2) = (0, null);

            return (length1, text1, length2, text2);
        }

        /// <summary>
        /// Get pairs of points, which determin start and stop of the lines used to draw the scalebar
        /// </summary>
        /// <param name="scaleBarLength1">Length of upper scalebar</param>
        /// <param name="scaleBarLength2">Length of lower scalebar</param>
        /// <param name="stroke">Width of line</param>
        /// <returns>Array with pairs of Points. First is always the start point, the second is the end point.</returns>
        public Point[] GetScaleBarLinePositions(float scaleBarLength1, float scaleBarLength2, float stroke)
        {
            Point[] points = null;

            bool drawNoSecondScaleBar = ScaleBarMode == ScaleBarMode.Single || (ScaleBarMode == ScaleBarMode.Both && SecondaryUnitConverter == null);

            float maxScaleBarLength = Math.Max(scaleBarLength1, scaleBarLength2);

            var posX = CalculatePositionX(0, (int)Viewport.Width, maxWidth);
            var posY = CalculatePositionY(0, (int)Viewport.Height, height);

            float left = posX + stroke * 0.5f * Scale;
            float right = posX + maxWidth - stroke * 0.5f * Scale;
            float center1 = posX + (maxWidth - scaleBarLength1) / 2;
            float center2 = posX + (maxWidth - scaleBarLength2) / 2;
            // Top position is Y in the middle of scale bar line
            float top = posY + (drawNoSecondScaleBar ? height - stroke * 0.5f * Scale : height * 0.5f);

            switch (TextAlignment)
            {
                case Alignment.Center:
                    if (drawNoSecondScaleBar)
                    {
                        points = new Point[6];
                        points[0] = new Point(center1, top - TickLength * Scale);
                        points[1] = new Point(center1, top);
                        points[2] = new Point(center1, top);
                        points[3] = new Point(center1 + maxScaleBarLength, top);
                        points[4] = new Point(center1 + maxScaleBarLength, top);
                        points[5] = new Point(center1 + scaleBarLength1, top - TickLength * Scale);
                    }
                    else
                    {
                        points = new Point[10];
                        points[0] = new Point(Math.Min(center1, center2), top);
                        points[1] = new Point(Math.Min(center1, center2) + maxScaleBarLength, top);
                        points[2] = new Point(center1, top - TickLength * Scale);
                        points[3] = new Point(center1, top);
                        points[4] = new Point(center1 + scaleBarLength1, top - TickLength * Scale);
                        points[5] = new Point(center1 + scaleBarLength1, top);
                        points[6] = new Point(center2, top + TickLength * Scale);
                        points[7] = new Point(center2, top);
                        points[8] = new Point(center2 + scaleBarLength2, top + TickLength * Scale);
                        points[9] = new Point(center2 + scaleBarLength2, top);
                    }
                    break;
                case Alignment.Left:
                    if (drawNoSecondScaleBar)
                    {
                        points = new Point[6];
                        points[0] = new Point(left, top);
                        points[1] = new Point(left + maxScaleBarLength, top);
                        points[2] = new Point(left, top - TickLength * Scale);
                        points[3] = new Point(left, top);
                        points[4] = new Point(left + scaleBarLength1, top - TickLength * Scale);
                        points[5] = new Point(left + scaleBarLength1, top);
                    }
                    else
                    {
                        points = new Point[8];
                        points[0] = new Point(left, top);
                        points[1] = new Point(left + maxScaleBarLength, top);
                        points[2] = new Point(left, top - TickLength * Scale);
                        points[3] = new Point(left, top + TickLength * Scale);
                        points[4] = new Point(left + scaleBarLength1, top - TickLength * Scale);
                        points[5] = new Point(left + scaleBarLength1, top);
                        points[6] = new Point(left + scaleBarLength2, top + TickLength * Scale);
                        points[7] = new Point(left + scaleBarLength2, top);
                    }
                    break;
                case Alignment.Right:
                    if (drawNoSecondScaleBar)
                    {
                        points = new Point[6];
                        points[0] = new Point(right, top);
                        points[1] = new Point(right - maxScaleBarLength, top);
                        points[2] = new Point(right, top - TickLength * Scale);
                        points[3] = new Point(right, top);
                        points[4] = new Point(right - scaleBarLength1, top - TickLength * Scale);
                        points[5] = new Point(right - scaleBarLength1, top);
                    }
                    else
                    {
                        points = new Point[8];
                        points[0] = new Point(right, top);
                        points[1] = new Point(right - maxScaleBarLength, top);
                        points[2] = new Point(right, top - TickLength * Scale);
                        points[3] = new Point(right, top + TickLength * Scale);
                        points[4] = new Point(right - scaleBarLength1, top - TickLength * Scale);
                        points[5] = new Point(right - scaleBarLength1, top);
                        points[6] = new Point(right - scaleBarLength2, top + TickLength * Scale);
                        points[7] = new Point(right - scaleBarLength2, top);
                    }
                    break;
            }

            return points;
        }

        /// <summary>
        /// Calculates the top-left-position of upper and lower text
        /// </summary>
        /// <param name="textSize">Default textsize for the string "9999 m"</param>
        /// <param name="textSize1">Size of upper text of scalebar</param>
        /// <param name="textSize2">Size of lower text of scalebar</param>
        /// <param name="stroke">Width of line</param>
        /// <returns>
        /// posX1 as left position of upper scalebar text
        /// posY1 as top position of upper scalebar text
        /// posX2 as left position of lower scalebar text
        /// posY2 as top position of lower scalebar text
        /// </returns>
        public (float posX1, float posY1, float posX2, float posY2) GetScaleBarTextPositions(BoundingBox textSize, BoundingBox textSize1, BoundingBox textSize2, float stroke)
        {
            bool drawNoSecondScaleBar = ScaleBarMode == ScaleBarMode.Single || (ScaleBarMode == ScaleBarMode.Both && SecondaryUnitConverter == null);

            float posX = CalculatePositionX(0, (int)Viewport.Width, maxWidth);
            float posY = CalculatePositionY(0, (int)Viewport.Height, height);

            float left = posX + (stroke + TextMargin) * Scale;
            float right1 = posX + maxWidth - (stroke + TextMargin) * Scale - (float)textSize1.Width;
            float right2 = posX + maxWidth - (stroke + TextMargin) * Scale - (float)textSize2.Width;
            float top = posY;
            float bottom = posY + height - (float)textSize2.Height;

            switch (TextAlignment)
            {
                case Alignment.Center:
                    if (drawNoSecondScaleBar)
                    {
                        return (posX + (stroke + TextMargin) * Scale + (MaxWidth - 2.0f * (stroke + TextMargin) * Scale - (float)textSize1.Width) / 2.0f, 
                            top,
                            0, 
                            0);
                    }
                    else
                    {
                        return (posX + (stroke + TextMargin) * Scale + (MaxWidth - 2.0f * (stroke + TextMargin) * Scale - (float)textSize1.Width) / 2.0f,
                                top, 
                                posX + (stroke + TextMargin) * Scale + (MaxWidth - 2.0f * (stroke + TextMargin) * Scale - (float)textSize2.Width) / 2.0f,
                                bottom);
                    }
                case Alignment.Left:
                    if (drawNoSecondScaleBar)
                    {
                        return (left, top, 0, 0);
                    }
                    else
                    {
                        return (left, top, left, bottom);
                    }
                case Alignment.Right:
                    if (drawNoSecondScaleBar)
                    {
                        return (right1, top, 0, 0);
                    }
                    else
                    {
                        return (right1, top, right2, bottom);
                    }
                default:
                    return (0, 0, 0, 0);
            }
        }

        public override void HandleWidgetTouched(Point position)
        {
        }

        internal void OnPropertyChanged([CallerMemberName] string name = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// Calculates the required length and value of a scalebar
        ///
        /// @param viewport the Viewport to calculate for
        /// @param width of the scale bar in pixel to calculate for
        /// @param unitConverter the DistanceUnitConverter to calculate for
        /// @return scaleBarLength and scaleBarText
        private (float scaleBarLength, string scaleBarText) CalculateScaleBarLengthAndValue(IViewport viewport, float width, IUnitConverter unitConverter)
        {
            // Get current position
            var position = Projection.SphericalMercator.ToLonLat(viewport.Center.X, viewport.Center.Y);

            // Calc ground resolution in meters per pixel of viewport for this latitude
            double groundResolution = viewport.Resolution * Math.Cos(position.Y / 180.0 * Math.PI);

            // Convert in units of UnitConverter
            groundResolution = groundResolution / unitConverter.MeterRatio;

            int[] scaleBarValues = unitConverter.ScaleBarValues;

            float scaleBarLength = 0;
            int scaleBarValue = 0;

            foreach (int value in scaleBarValues)
            {
                scaleBarValue = value;
                scaleBarLength = (float)(scaleBarValue / groundResolution);
                if (scaleBarLength < (width - 10))
                {
                    break;
                }
            }

            var scaleBarText = unitConverter.GetScaleText(scaleBarValue);

            return (scaleBarLength, scaleBarText);
        }

        /**
		 * Calculates the required length and value of the scalebar using the current {@link DistanceUnitAdapter}
		 *
		 * @return a {@link ScaleBarLengthAndValue} object containing the required scaleBarLength and scaleBarValue
		 */
        private (float scaleBarLength, string scaleBarText) CalculateScaleBarLengthAndValue(IViewport viewport, float width)
        {
            return CalculateScaleBarLengthAndValue(viewport, width, UnitConverter);
        }
    }
}