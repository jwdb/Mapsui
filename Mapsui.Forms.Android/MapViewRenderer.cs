using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Mapsui.Forms;
using Mapsui.Forms.Android;

// Export the rendererer, and associate it with our Mapsui Forms Control
[assembly: ExportRenderer(typeof(MapView), typeof(MapViewRenderer))]
namespace Mapsui.Forms.Android
{
	// Extend ViewRenderer and link it to our Forms Control, and to the MapsUI implementation for Android
	public class MapViewRenderer : ViewRenderer<MapView, Mapsui.UI.Android.MapControl>
	{
		// Mapsui Native Android implementation
		Mapsui.UI.Android.MapControl mapNativeControl;

		// Our Mapsui Forms Control
		MapView mapViewControl;

		protected override void OnElementChanged(ElementChangedEventArgs<MapView> e)
		{
			base.OnElementChanged(e);

			if (mapViewControl == null && e.NewElement != null)
			{
				// Get the MapsUI Forms control
				mapViewControl = e.NewElement as MapView;
			}

			if (mapNativeControl == null)
			{
				// Set Native android implementation
				mapNativeControl = new Mapsui.UI.Android.MapControl(Context, null);

				// Link our Forms Control to the Native control
				mapNativeControl.Map = mapViewControl.Map;

				// Set native app
				SetNativeControl(mapNativeControl);
			}
		}
	}
}