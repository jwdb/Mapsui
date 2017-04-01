using System.Linq;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.Overlays;

namespace Mapsui.Samples.Common.Maps
{
    public static class OverlaysSample
    {
        public static Map CreateMap()
        {
            var map = new Map();

            map.Layers.Add(OpenStreetMap.CreateTileLayer());

			map.Overlays.Add(new CenterOverlay() { ForegroundColor = new Styles.Color(255, 0, 255) });
			map.Overlays.Add(new ScaleBarOverlay() { Width = 100, ForegroundColor = new Styles.Color(255, 0, 255), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Single, SecondaryUnitConverter = ImperialUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.BottomRight });
			map.Overlays.Add(new ScaleBarOverlay() { Width = 200, ForegroundColor = new Styles.Color(255, 0, 255), BackgroundColor = new Styles.Color(255, 255, 0), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Both, SecondaryUnitConverter = NauticalUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.BottomCenter });
			map.Overlays.Add(new ScaleBarOverlay() { Width = 100, ForegroundColor = new Styles.Color(255, 0, 255), BackgroundColor = new Styles.Color(255, 255, 0), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Single, SecondaryUnitConverter = ImperialUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.BottomLeft });
			map.Overlays.Add(new ScaleBarOverlay() { Width = 100, ForegroundColor = new Styles.Color(255, 0, 255), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Both, SecondaryUnitConverter = ImperialUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.TopRight });
			map.Overlays.Add(new ScaleBarOverlay() { Width = 200, ForegroundColor = new Styles.Color(255, 0, 255), BackgroundColor = new Styles.Color(255, 255, 0), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Single, SecondaryUnitConverter = ImperialUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.TopCenter });
			map.Overlays.Add(new ScaleBarOverlay() { Width = 100, ForegroundColor = new Styles.Color(255, 0, 255), BackgroundColor = new Styles.Color(255, 255, 0), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Both, SecondaryUnitConverter = NauticalUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.TopLeft });
			map.Overlays.Add(new ScaleBarOverlay() { Width = 100, PosX = 250, PosY = 400, ForegroundColor = new Styles.Color(0, 0, 0), BackgroundColor = new Styles.Color(255, 255, 255), ScaleBarMode = ScaleBarOverlay.ScaleBarModeEnum.Both, SecondaryUnitConverter = NauticalUnitConverter.Instance, ScaleBarPosition = ScaleBarOverlay.ScaleBarPositionEnum.XYCenter });

            return map;
        }
    }
}