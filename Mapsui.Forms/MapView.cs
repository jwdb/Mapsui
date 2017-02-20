using Mapsui.Forms.Extensions;
using System.Runtime.CompilerServices;
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
				if (value != nativeMap)
				{
					nativeMap = value;
					// Set defaults
					nativeMap.BackColor = BackgroundColor.ToMapsuiColor();
					RefreshGraphics();
				}
			}
		}

		/// <summary>
		/// Properties
		/// </summary>
		 
		public new Color ackgroundColor
		{
			get
			{
				return (Color)GetValue(ackgroundColorProperty);
			}
			set
			{
				SetValue(ackgroundColorProperty, value);
			}
		}

		/// <summary>
		/// Bindings
		/// </summary>
		 
		public new static readonly BindableProperty ackgroundColorProperty = BindableProperty.Create(
																propertyName: nameof(ackgroundColor),
																returnType: typeof(Color),
																declaringType: typeof(MapView),
																defaultValue: Color.White,
																defaultBindingMode: BindingMode.TwoWay,
																propertyChanged: null);

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
				RefreshGraphics();
			}
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