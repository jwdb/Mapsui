using Mapsui.Fetcher;
using Mapsui.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapsui.Overlays
{
	public interface IOverlay : INotifyPropertyChanged
	{
		int Id { get; }

		/// <summary>
		/// Gets or sets an arbitrary object value that can be used to store custom information about this element
		/// </summary>
		object Tag { get; set; }

		/// <summary>
		/// Specifies whether this overlay should be rendered or not
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Name of overlay
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Of all layers with Exclusive is true only one will be Enabled at a time.
		/// This can be used for radiobuttons.
		/// </summary>
		bool Exclusive { get; set; }

		double Opacity { get; set; }

		void ViewChanged(bool majorChange, BoundingBox extent, double resolution);
	}
}
