using Mapsui.Styles;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mapsui.Overlays
{
	public class CenterOverlay : IOverlay
	{
		bool _enabled = true;
		bool _exclusive = false;
		string _name;
		double _opacity = 255;
		object _tag;
		Color _foregroundColor = new Color(0, 0, 0);
		Color _backgroundColor = new Color(255, 255, 255);

		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				if (_enabled == value)
					return;
				_enabled = value;
				OnPropertyChanged();
			}
		}

		public bool Exclusive
		{
			get
			{
				return _exclusive;
			}
			set
			{
				if (_exclusive == value)
					return;
				_exclusive = value;
				OnPropertyChanged();
			}
		}

		public int Id { get; }

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (_name == value)
					return;
				_name = value;
				OnPropertyChanged();
			}
		}

		public double Opacity
		{
			get
			{
				return _opacity;
			}
			set
			{
				if (_opacity == value)
					return;
				_opacity = value;
				OnPropertyChanged();
			}
		}

		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				if (_tag == value)
					return;
				_tag = value;
				OnPropertyChanged();
			}
		}

		public Color ForegroundColor
		{
			get
			{
				return _foregroundColor;
			}
			set
			{
				if (_foregroundColor == value)
					return;
				_foregroundColor = value;
				OnPropertyChanged();
			}
		}

		public Color BackgroundColor
		{
			get
			{
				return _backgroundColor;
			}
			set
			{
				if (_backgroundColor == value)
					return;
				_backgroundColor = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
