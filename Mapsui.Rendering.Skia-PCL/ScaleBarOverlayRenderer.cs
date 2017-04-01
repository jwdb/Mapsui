using Mapsui.Overlays;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mapsui.Overlays.ScaleBarOverlay;

namespace Mapsui.Rendering.Skia
{
	public class ScaleBarOverlayRenderer
	{
		private const float scaleBarMargin = 10;
		private const float strokeExternal = 4;
		private const float strokeInternal = 2;
		private const float textMargin = 1;
		private const float scaleBarTickLength = 3;

		private static SKPaint paintScaleBar;
		private static SKPaint paintScaleBarStroke;
		private static SKPaint paintScaleText;
		private static SKPaint paintScaleTextStroke;

		public static void Draw(SKCanvas canvas, IViewport viewport, ScaleBarOverlay overlay)
		{
			SKBitmap scaleBarBitmap;

			if (overlay.RedrawNeeded)
			{
				// Bitmap isn't correct anymore, so create it new

				// TODO
				// Is this correct?
				float scale = 1;

				paintScaleBar = CreateScaleBarPaint(overlay.ForegroundColor.ToSkia(), strokeInternal, SKPaintStyle.Fill, scale);
				paintScaleBarStroke = CreateScaleBarPaint(overlay.BackgroundColor.ToSkia(), strokeExternal, SKPaintStyle.Stroke, scale);
				paintScaleText = CreateTextPaint(overlay.ForegroundColor.ToSkia(), 0, SKPaintStyle.Fill, scale);
				paintScaleTextStroke = CreateTextPaint(overlay.BackgroundColor.ToSkia(), 2, SKPaintStyle.Stroke, scale);

				ScaleBarLengthAndValue lengthAndValue = overlay.CalculateScaleBarLengthAndValue(viewport, (int)(overlay.Width - 2 * (scaleBarMargin * scale)));
				ScaleBarLengthAndValue lengthAndValue2;

				if (overlay.ScaleBarMode == ScaleBarModeEnum.Both)
				{
					lengthAndValue2 = overlay.CalculateScaleBarLengthAndValue(viewport, (int)(overlay.Width - 2 * (scaleBarMargin * scale)), overlay.SecondaryUnitConverter);
				}
				else
				{
					lengthAndValue2 = new ScaleBarLengthAndValue(0, 0, null);
				}

				// Calc height of scale bar bitmap
				SKRect textSize;

				paintScaleTextStroke.MeasureText("9999 m", ref textSize);

				var scaleBarHeight = textSize.Height * 2 + 2 * (scaleBarMargin + scaleBarTickLength + strokeExternal * 0.5f + 2 * textMargin) * scale;

				if (overlay.Image != null && ((SKBitmap)overlay.Image).Height == scaleBarHeight)
				{
					scaleBarBitmap = (SKBitmap)overlay.Image;
				}
				else
				{
					scaleBarBitmap = null;

					// Create bitmap
					scaleBarBitmap = new SKBitmap(overlay.Width, (int)scaleBarHeight, SKColorType.Rgba8888, SKAlphaType.Opaque);
				}

				// Clear bitmap
				scaleBarBitmap.Erase(SKColors.Transparent);

				// Create canvas to draw onto
				SKCanvas bitmapCanvas = new SKCanvas(scaleBarBitmap);

				// Draw a rect around the scale bar for testing
				var tempPaint = new SKPaint() { StrokeWidth = 1, Color = SKColors.Blue, IsStroke = true };
				bitmapCanvas.DrawRect(new SKRect(scaleBarMargin*scale, scaleBarMargin*scale, overlay.Width-scaleBarMargin*scale, scaleBarHeight - scaleBarMargin*scale), tempPaint);

				DrawScaleBar(bitmapCanvas, overlay, scaleBarHeight, lengthAndValue.ScaleBarLength, lengthAndValue2.ScaleBarLength, paintScaleBarStroke, scale);
				DrawScaleBar(bitmapCanvas, overlay, scaleBarHeight, lengthAndValue.ScaleBarLength, lengthAndValue2.ScaleBarLength, paintScaleBar, scale);

				DrawScaleText(bitmapCanvas, overlay, scaleBarHeight, lengthAndValue.ScaleBarText, lengthAndValue2.ScaleBarText, paintScaleTextStroke, scale);
				DrawScaleText(bitmapCanvas, overlay, scaleBarHeight, lengthAndValue.ScaleBarText, lengthAndValue2.ScaleBarText, paintScaleText, scale);

				overlay.Image = scaleBarBitmap;
				overlay.RedrawNeeded = false;
			}

			scaleBarBitmap = (SKBitmap)overlay.Image;

			var left = overlay.CalculatePositionLeft(0, (int)viewport.Width, scaleBarBitmap.Width);
			var right = left + scaleBarBitmap.Width;
			var top = overlay.CalculatePositionTop(0, (int)viewport.Height, scaleBarBitmap.Height);
			var bottom = top + scaleBarBitmap.Height;

			// Image is correct, so draw it onto canvas
			var paint = new SKPaint() { BlendMode = SKBlendMode.SrcATop };

			canvas.DrawBitmap((SKBitmap)overlay.Image, new SKRect(left, top, right, bottom), paint);
		}

		private static void DrawScaleBar(SKCanvas canvas, ScaleBarOverlay overlay, float height, int scaleBarLength1, int scaleBarLength2, SKPaint paint, float scale)
		{
			int maxScaleBarLength = Math.Max(scaleBarLength1, scaleBarLength2);

			double width = overlay.Width;

			float left = (float)Math.Round((scaleBarMargin + strokeExternal * 0.5f) * scale);
			float right = (float)Math.Round(width - (scaleBarMargin + strokeExternal * 0.5f) * scale);
			float center1 = (float)Math.Round(left + (width - 2.0 * (scaleBarMargin + strokeExternal * 0.5f) * scale - scaleBarLength1 * scale) / 2.0);
			float center2 = (float)Math.Round(left + (width - 2.0 * (scaleBarMargin + strokeExternal * 0.5f) * scale - scaleBarLength2 * scale) / 2.0);

			float top = (overlay.ScaleBarPosition == ScaleBarPositionEnum.BottomCenter |
				overlay.ScaleBarPosition == ScaleBarPositionEnum.BottomLeft |
				overlay.ScaleBarPosition == ScaleBarPositionEnum.BottomRight) && scaleBarLength2 == 0 
				? (float)Math.Round(height / 2.0f + ((height - scaleBarMargin * scale) / 2.0 - (scaleBarTickLength + strokeExternal) * scale)) 
				: (float)Math.Round(height / 2.0);

			switch (overlay.ScaleBarPosition)
			{
				case ScaleBarPositionEnum.TopCenter:
				case ScaleBarPositionEnum.BottomCenter:
				case ScaleBarPositionEnum.XYCenter:
					if (scaleBarLength2 == 0)
					{
						canvas.DrawLine(center1, top, center1 + maxScaleBarLength, top, paint);
						canvas.DrawLine(center1, top - scaleBarTickLength * scale, center1, top, paint);
						canvas.DrawLine(center1 + scaleBarLength1, top - scaleBarTickLength * scale, center1 + scaleBarLength1, top, paint);
					}
					else
					{
						canvas.DrawLine(Math.Min(center1, center2), top, Math.Min(center1, center2) + maxScaleBarLength, top, paint);
						canvas.DrawLine(center1, top - scaleBarTickLength * scale, center1, top, paint);
						canvas.DrawLine(center1 + scaleBarLength1, top - scaleBarTickLength * scale, center1 + scaleBarLength1, top, paint);
						canvas.DrawLine(center2, top + scaleBarTickLength * scale, center2, top, paint);
						canvas.DrawLine(center2 + scaleBarLength2, top + scaleBarTickLength * scale, center2 + scaleBarLength2, top, paint);
					}
					break;
				case ScaleBarPositionEnum.TopLeft:
				case ScaleBarPositionEnum.BottomLeft:
				case ScaleBarPositionEnum.XYLeft:
					if (scaleBarLength2 == 0)
					{
						canvas.DrawLine(left, top, left + maxScaleBarLength, top, paint);
						canvas.DrawLine(left, top - scaleBarTickLength * scale, left, top, paint);
						canvas.DrawLine(left + scaleBarLength1, top - scaleBarTickLength * scale, left + scaleBarLength1, top, paint);
					}
					else
					{
						canvas.DrawLine(left, top, left + maxScaleBarLength, top, paint);
						canvas.DrawLine(left, top - scaleBarTickLength * scale, left, top + scaleBarTickLength * scale, paint);
						canvas.DrawLine(left + scaleBarLength1, top - scaleBarTickLength * scale, left + scaleBarLength1, top, paint);
						canvas.DrawLine(left + scaleBarLength2, top + scaleBarTickLength * scale, left + scaleBarLength2, top, paint);
					}
					break;
				case ScaleBarPositionEnum.TopRight:
				case ScaleBarPositionEnum.BottomRight:
				case ScaleBarPositionEnum.XYRight:
					if (scaleBarLength2 == 0)
					{
						canvas.DrawLine(right, top, right - maxScaleBarLength, top, paint);
						canvas.DrawLine(right, top - scaleBarTickLength * scale, right, top, paint);
						canvas.DrawLine(right - scaleBarLength1, top - scaleBarTickLength * scale, right - scaleBarLength1, top, paint);
					}
					else
					{
						canvas.DrawLine(right, top, right - maxScaleBarLength, top, paint);
						canvas.DrawLine(right, top - scaleBarTickLength * scale, right, top + scaleBarTickLength * scale, paint);
						canvas.DrawLine(right - scaleBarLength1, top - scaleBarTickLength * scale, right - scaleBarLength1, top, paint);
						canvas.DrawLine(right - scaleBarLength2, top + scaleBarTickLength * scale, right - scaleBarLength2, top, paint);
					}
					break;
			}
		}

		private static void DrawScaleText(SKCanvas canvas, ScaleBarOverlay overlay, float height, string scaleText1, string scaleText2, SKPaint paint, float scale)
		{
			double width = overlay.Width;

			// Calc text height
			SKRect textSize;
			SKRect textSize1;
			SKRect textSize2;

			// Do this, because height of text changes sometimes (e.g. from 2 m to 1 m)
			paintScaleTextStroke.MeasureText("9999 m", ref textSize);
			paintScaleTextStroke.MeasureText(scaleText1, ref textSize1);
			paintScaleTextStroke.MeasureText(scaleText2, ref textSize2);

			var textHeight = textSize.Height;

			var left = (float)Math.Round((scaleBarMargin + strokeExternal + textMargin) * scale);
			var right1 = (float)Math.Round(width - (scaleBarMargin + strokeExternal + textMargin) * scale - paintScaleTextStroke.MeasureText(scaleText1));
			var right2 = (float)Math.Round(width - (scaleBarMargin + strokeExternal + textMargin) * scale - paintScaleTextStroke.MeasureText(scaleText2));
			var top = (float)Math.Round((scaleBarMargin + textMargin) * scale + textHeight);
			var bottom = (float)Math.Round(height - (scaleBarMargin + textMargin) * scale);

			float offset = (overlay.ScaleBarPosition == ScaleBarPositionEnum.BottomCenter |
				overlay.ScaleBarPosition == ScaleBarPositionEnum.BottomLeft |
				overlay.ScaleBarPosition == ScaleBarPositionEnum.BottomRight) 
				? (float)((height - scaleBarMargin * scale) / 2.0 - (scaleBarTickLength + strokeExternal) * scale) 
				: 0.0f;

			switch (overlay.ScaleBarPosition)
			{
				case ScaleBarPositionEnum.TopCenter:
				case ScaleBarPositionEnum.BottomCenter:
				case ScaleBarPositionEnum.XYCenter:
					if (scaleText2.Length == 0)
					{
						canvas.DrawText(scaleText1, (float)Math.Round((scaleBarMargin + strokeExternal + textMargin) * scale + (overlay.Width - 2 * (scaleBarMargin + strokeExternal + textMargin) * scale - paintScaleTextStroke.MeasureText(scaleText1)) / 2.0),
								top + offset, paint);
					}
					else
					{
						canvas.DrawText(scaleText1, (float)Math.Round((scaleBarMargin + strokeExternal + textMargin) * scale + (overlay.Width - 2 * (scaleBarMargin + strokeExternal + textMargin) * scale - paintScaleTextStroke.MeasureText(scaleText1)) / 2.0),
								top, paint);
						canvas.DrawText(scaleText2, (float)Math.Round((scaleBarMargin + strokeExternal + textMargin) * scale + (overlay.Width - 2 * (scaleBarMargin + strokeExternal + textMargin) * scale - paintScaleTextStroke.MeasureText(scaleText2)) / 2.0),
								bottom, paint);
					}
					break;
				case ScaleBarPositionEnum.TopLeft:
				case ScaleBarPositionEnum.BottomLeft:
				case ScaleBarPositionEnum.XYLeft:
					if (scaleText2.Length == 0)
					{
						canvas.DrawText(scaleText1, left, top + offset, paint);
					}
					else
					{
						canvas.DrawText(scaleText1, left, top, paint);
						canvas.DrawText(scaleText2, left, bottom, paint);
					}
					break;
				case ScaleBarPositionEnum.TopRight:
				case ScaleBarPositionEnum.BottomRight:
				case ScaleBarPositionEnum.XYRight:
					if (scaleText2.Length == 0)
					{
						canvas.DrawText(scaleText1, right1, top + offset, paint);
					}
					else
					{
						canvas.DrawText(scaleText1, right1, top, paint);
						canvas.DrawText(scaleText2, right2, bottom, paint);
					}
					break;
			}
		}

		private static SKPaint CreateScaleBarPaint(SKColor color, float strokeWidth, SKPaintStyle style, float scale)
		{
			SKPaint paint = new SKPaint();

			paint.LcdRenderText = true;
			paint.Color = color;
			paint.StrokeWidth = strokeWidth * scale;
			paint.Style = style;
			paint.StrokeCap = SKStrokeCap.Square;

			return paint;
		}

		private static SKPaint CreateTextPaint(SKColor color, float strokeWidth, SKPaintStyle style, float scale)
		{
			SKPaint paint = new SKPaint();

			paint.LcdRenderText = true;
			paint.Color = color;
			paint.StrokeWidth = strokeWidth * scale;
			paint.Style = style;
			paint.Typeface = SKTypeface.FromFamilyName(null, SKTypefaceStyle.Bold);
			paint.TextSize = 10 * scale;

			return paint;
		}
	}
}