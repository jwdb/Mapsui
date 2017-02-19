using Mapsui.Samples.Common.Maps;
using System;
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

		void OnButtonClicked(object sender, EventArgs e)
		{
			mapView.BackgroundColor = Xamarin.Forms.Color.Red; 
		}
	}
}