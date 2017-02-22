using Mapsui.Forms.Extensions;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System;
using System.ComponentModel;

namespace Mapsui.Forms
{
	/// <summary>
	/// This is the Mapsui Forms Control that will be used within the Forms PCL project
	/// </summary>
	public class MapView : View
	{
		/// <summary>
		/// Privates
		/// </summary>
		internal Map nativeMap;

		public MapView()
		{
			Map = new Map();
		}

		/// <summary>
		/// Events
		/// </summary>

		public Map Map
		{
			get
			{
				return nativeMap;
			}
			set
			{
				if (value == nativeMap)
					return;

				nativeMap = value;
				// Replace viewport with NotifyViewport, so that we get events
				var oldViewport = nativeMap.Viewport as Viewport;
				if (oldViewport != null)
				{
					var newViewport = new NotifyingViewport(oldViewport);
					newViewport.PropertyChanged += ViewportPropertyChanged;
					nativeMap.Viewport = newViewport;
				}
				// Get values
				//Center = nativeMap.Viewport.Center;
				// Set values
				nativeMap.BackColor = BackgroundColor.ToMapsuiColor();
			}
		}

		/// <summary>
		/// Properties
		/// </summary>

		public Mapsui.Geometries.Point Center
		{
			get { return (Mapsui.Geometries.Point)GetValue(CenterProperty); }
			set { SetValue(CenterProperty, value); }
		}

		/// <summary>
		/// Bindings
		/// </summary>
		 
		public static readonly BindableProperty CenterProperty = BindableProperty.Create(
										propertyName: nameof(Center),
										returnType: typeof(Mapsui.Geometries.Point),
										declaringType: typeof(MapView),
										defaultValue: default(Mapsui.Geometries.Point),
										defaultBindingMode: BindingMode.TwoWay,
										propertyChanged: null);

		/// <summary>
		/// Get updates from Map
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Property {0} changed", e.PropertyName);
		}

		/// <summary>
		/// Check if something important for Map changed
		/// </summary>
		/// <param name="propertyName">Name of property which changed</param>
		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (propertyName.Equals(nameof(BackgroundColor)))
			{
				nativeMap.BackColor = BackgroundColor.ToMapsuiColor();
			}

			if (propertyName.Equals(nameof(Center)))
			{
				nativeMap.Viewport.Center = Center;
			}
		}

		private void ViewportPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var viewport = sender as IViewport;

			if (viewport == null)
				return;

			if (e.PropertyName.Equals(nameof(NotifyingViewport.Center)))
				System.Diagnostics.Debug.WriteLine("Center {0}", nativeMap.Viewport.Center.ToString());

			if (e.PropertyName.Equals(nameof(NotifyingViewport.Resolution)))
				System.Diagnostics.Debug.WriteLine("Resolution {0}", nativeMap.Viewport.Resolution.ToString());
		}

		/// <summary>
		/// Refresh the graphics of the map
		/// </summary>
		public void RefreshGraphics()
		{
			MessagingCenter.Send<MapView>(this, "Refresh");
		}
	}
}