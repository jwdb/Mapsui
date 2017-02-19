using Mapsui;
using Mapsui.Projection;
using System;
using Xamarin.Forms;

namespace Mapsui.Forms
{
	/// <summary>
	/// This is the Mapsui Forms Control that will be used within the Forms PCL project
	/// </summary>
	public class MapView : View
	{
		public Map Map { get; set; }

		public MapView()
		{
			Map = new Map();
		}
	}
}