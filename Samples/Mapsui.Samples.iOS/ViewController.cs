using System;
using Mapsui.UI.iOS;
using UIKit;
using CoreGraphics;
using Mapsui.Overlays;

namespace Mapsui.Samples.iOS
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View = CreateMap(View.Bounds);
        }


        private static MapControl CreateMap(CGRect bounds)
        {
			var mapControl = new MapControl(bounds)
            {
                Map = Common.Maps.InfoLayersSample.CreateMap()
			};

			mapControl.Map.Overlays.Add(new CenterOverlay() { ForegroundColor = new Styles.Color(255, 0, 255) });

			return mapControl;
        }
    }
}