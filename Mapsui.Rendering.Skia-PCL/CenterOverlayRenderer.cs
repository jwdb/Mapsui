using Mapsui.Overlays;
using SkiaSharp;

namespace Mapsui.Rendering.Skia
{
	public static class CenterOverlayRenderer
	{
		public static void Draw(SKCanvas canvas, CenterOverlay overlay)
		{
			// Draw cross in the mid of canvas
			float x = canvas.ClipBounds.MidX;
			float y = canvas.ClipBounds.MidY;

			var paint = new SKPaint();
			paint.StrokeCap = SKStrokeCap.Round;
			paint.Style = SKPaintStyle.Stroke;

			paint.Color = new SKColor((byte)overlay.BackgroundColor.R, 
				(byte)overlay.BackgroundColor.G, 
				(byte)overlay.BackgroundColor.B, 
				(byte)overlay.Opacity);
			paint.StrokeWidth = 5;

			canvas.DrawLine(x - 10, y, x - 40, y, paint);
			canvas.DrawLine(x + 10, y, x + 40, y, paint);
			canvas.DrawLine(x, y - 10, x, y - 40, paint);
			canvas.DrawLine(x, y + 10, x, y + 40, paint);

			paint.Color = new SKColor((byte)overlay.ForegroundColor.R, 
				(byte)overlay.ForegroundColor.G, 
				(byte)overlay.ForegroundColor.B, 
				(byte)overlay.Opacity);
			paint.StrokeWidth = 2;

			canvas.DrawLine(x - 10, y, x - 40, y, paint);
			canvas.DrawLine(x + 10, y, x + 40, y, paint);
			canvas.DrawLine(x, y - 10, x, y - 40, paint);
			canvas.DrawLine(x, y + 10, x, y + 40, paint);
		}
	}
}
