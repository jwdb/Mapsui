using Mapsui.Geometries;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mapsui.Overlays
{
	public abstract class BaseOverlay : IOverlay
	{
		private static int _instanceCounter;
		private bool _enabled;
		private bool _exclusive;
		private string _name;
		private double _maxVisible;
		private double _minVisible;
		private double _opacity;
		private object _tag;

		protected BaseOverlay()
		{
			Name = "Overlay";
			Enabled = true;
			MinVisible = 0;
			MaxVisible = double.MaxValue;
			Opacity = 1;
			Id = _instanceCounter++;
		}

		protected BaseOverlay(string name)
            : this()
        {
			Name = name;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public int Id { get; }

		/// <summary>
		/// Gets or sets an arbitrary object value that can be used to store custom information about this element
		/// </summary>
		public object Tag
		{
			get { return _tag; }
			set
			{
				_tag = value;
				OnPropertyChanged(nameof(Tag));
			}
		}

		/// <summary>
		/// Minimum visibility zoom, including this value
		/// </summary>
		public double MinVisible
		{
			get { return _minVisible; }
			set
			{
				_minVisible = value;
				OnPropertyChanged(nameof(MinVisible));
			}
		}

		/// <summary>
		/// Maximum visibility zoom, excluding this value
		/// </summary>
		public double MaxVisible
		{
			get { return _maxVisible; }
			set
			{
				_maxVisible = value;
				OnPropertyChanged(nameof(MaxVisible));
			}
		}

		/// <summary>
		/// Specified whether the layer is rendered or not
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled == value) return;
				_enabled = value;
				OnPropertyChanged(nameof(Enabled));
			}
		}

		/// <summary>
		/// Gets or sets the name of the layer
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public bool Exclusive
		{
			get { return _exclusive; }
			set
			{
				_exclusive = value;
				OnPropertyChanged(nameof(Exclusive));
			}
		}

		public double Opacity
		{
			get { return _opacity; }
			set
			{
				_opacity = value;
				OnPropertyChanged(nameof(Opacity));
			}
		}

		public abstract void ViewChanged(bool majorChange, BoundingBox extent, double resolution);

		public override string ToString()
		{
			return Name;
		}

		internal void OnPropertyChanged([CallerMemberName] string name = "")
		{
			var handler = PropertyChanged;
			handler?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}