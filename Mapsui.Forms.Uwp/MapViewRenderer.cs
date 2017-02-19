using Xamarin.Forms.Platform.UWP;
using Xamarin.Forms;
using Mapsui.Forms;
using Mapsui.Forms.Uwp;

// Export the rendererer, and associate it with out MapsUI Forms Control
[assembly: ExportRenderer(typeof(MapView), typeof(MapViewRenderer))]
namespace Mapsui.Forms.Uwp
{
	// Extend ViewRenderer and link it to our Forms Control, and to the MapsUI implementation for Android
	public class MapViewRenderer : ViewRenderer<MapView, Mapsui.UI.Uwp.MapControl>
	{
		// MapsUI Native Android implementation
		Mapsui.UI.Uwp.MapControl mapControl;

		// Our MapsUI Forms Control
		MapView mapViewControl;

		protected override void OnElementChanged(ElementChangedEventArgs<MapView> e)
		{
			base.OnElementChanged(e);

			if (mapViewControl == null && e.NewElement != null)
			{
				// Get the MapsUI Forms control
				mapViewControl = e.NewElement as MapView;
			}

			if (mapControl == null)
			{
				// Set Native iOS implementation
				mapControl = new Mapsui.UI.Uwp.MapControl();

				// Link our Forms Control to the Native control
				mapControl.Map = mapViewControl.Map;

				// Set native app
				SetNativeControl(mapControl);
			}
		}
	}
}