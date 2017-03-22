using Android.App;
using Android.Content.PM;
using Android.OS;
using Mapsui.Overlays;
using Mapsui.Samples.Common.Maps;
using Mapsui.UI.Android;

namespace Mapsui.Samples.Android
{
    [Activity(Label = "Mapsui.Samples.Android", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Nosensor)]
    public class Activity1 : Activity
    {
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);
			var mapControl = FindViewById<MapControl>(Resource.Id.mapcontrol);
			mapControl.Map = InfoLayersSample.CreateMap();
			mapControl.Map.Overlays.Add(new CenterOverlay() { ForegroundColor = new Styles.Color(255, 0, 255) });
			mapControl.Map.Overlays.Add(new ScaleBarOverlay() { Width = 100, ForegroundColor = new Styles.Color(255, 0, 255), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Single, SecondaryUnitConverter = ImperialUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.BottomRight });
			mapControl.Map.Overlays.Add(new ScaleBarOverlay() { Width = 200, ForegroundColor = new Styles.Color(255, 0, 255), BackgroundColor = new Styles.Color(255, 255, 0), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Both, SecondaryUnitConverter = NauticalUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.BottomCenter });
			mapControl.Map.Overlays.Add(new ScaleBarOverlay() { Width = 100, ForegroundColor = new Styles.Color(255, 0, 255), BackgroundColor = new Styles.Color(255, 255, 0), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Single, SecondaryUnitConverter = ImperialUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.BottomLeft });
			mapControl.Map.Overlays.Add(new ScaleBarOverlay() { Width = 100, ForegroundColor = new Styles.Color(255, 0, 255), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Both, SecondaryUnitConverter = ImperialUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.TopRight });
			mapControl.Map.Overlays.Add(new ScaleBarOverlay() { Width = 200, ForegroundColor = new Styles.Color(255, 0, 255), BackgroundColor = new Styles.Color(255, 255, 0), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Single, SecondaryUnitConverter = ImperialUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.TopCenter });
			mapControl.Map.Overlays.Add(new ScaleBarOverlay() { Width = 100, ForegroundColor = new Styles.Color(255, 0, 255), BackgroundColor = new Styles.Color(255, 255, 0), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Both, SecondaryUnitConverter = NauticalUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.TopLeft });
			mapControl.Map.Overlays.Add(new ScaleBarOverlay() { Width = 100, PosX = 250, PosY = 400, ForegroundColor = new Styles.Color(0, 0, 0), BackgroundColor = new Styles.Color(255, 255, 255), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Both, SecondaryUnitConverter = NauticalUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.XYCenter });
		}
	}
}