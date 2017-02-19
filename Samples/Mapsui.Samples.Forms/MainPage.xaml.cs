using Mapsui.Forms;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Samples.Common.Maps;
using Mapsui.Styles;
using Xamarin.Forms;

namespace Mapsui.Samples.Forms
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

			mapView.Map = BingSample.CreateMap();
//			mapView.Map = InfoLayersSample.CreateMap();
//			mapView.Map = LabelsSample.CreateMap();
		}
	}
}