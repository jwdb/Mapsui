using Mapsui.Forms.Extensions;
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
		/// <summary>
		/// Privates
		/// </summary>
		internal Map nativeMap;

		public MapView()
		{
			Map = new Map();
		}

		public Map Map
		{
			get
			{
				return nativeMap;
			}
			set
			{
				if (value != nativeMap)
				{
					nativeMap = value;
					// Set defaults
					nativeMap.BackColor = BackgroundColor.ToMapsuiColor();
					// TODO
					// Something changed, so update map
				}
			}
		}

		/// <summary>
		/// Properties
		/// </summary>
		 
		public new Color BackgroundColor
		{
			get
			{
				return (Color)GetValue(BackgroundColorProperty);
			}
			set
			{
				if (value != null && nativeMap.BackColor != value.ToMapsuiColor())
				{
					nativeMap.BackColor = value.ToMapsuiColor();
					SetValue(BackgroundColorProperty, value);
				}
			}
		}

		/// <summary>
		/// Bindings
		/// </summary>
		 
		public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
																propertyName: nameof(BackgroundColor),
																returnType: typeof(Color),
																declaringType: typeof(MapView),
																defaultValue: Color.White,
																defaultBindingMode: BindingMode.TwoWay,
																propertyChanged: null);
	}
}