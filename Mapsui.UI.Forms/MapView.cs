﻿using Mapsui.Layers;
using Mapsui.UI.Forms.Extensions;
using Mapsui.UI.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Mapsui.UI.Forms
{
    public class MapView : ContentView, INotifyPropertyChanged, IEnumerable<Pin>
    {
        private const string MyLocationLayerName = "MyLocation";
        private const string PinLayerName = "Pins";
        private const string DrawableLayerName = "Drawables";

        internal MapControl _mapControl;

        private MyLocationLayer _mapMyLocationLayer;
        private Layer _mapPinLayer;
        private Layer _mapDrawableLayer;
        private StackLayout _mapButtons;
        private Image _mapZoomInButton;
        private Image _mapZoomOutButton;
        private Image _mapSpacingButton1;
        private Image _mapMyLocationButton;
        private Image _mapSpacingButton2;
        private Image _mapNorthingButton;

        readonly ObservableCollection<Pin> _pins = new ObservableCollection<Pin>();
        readonly ObservableCollection<Drawable> _drawable = new ObservableCollection<Drawable>();
        readonly ObservableCollection<Callout> _callouts = new ObservableCollection<Callout>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Mapsui.UI.Forms.MapView"/> class.
        /// </summary>
        public MapView()
        {
            MyLocationEnabled = false;
            MyLocationFollow = false;

            IsClippedToBounds = true;

            _mapControl = new MapControl();
            _mapMyLocationLayer = new MyLocationLayer(this) { Enabled = true };
            _mapPinLayer = new Layer(PinLayerName);
            _mapDrawableLayer = new Layer(DrawableLayerName);

            // Add some events to _mapControl
            _mapControl.SingleTap += HandlerTap;
            _mapControl.DoubleTap += HandlerTap;
            _mapControl.LongTap += HandlerLongTap;
            _mapControl.Hovered += HandlerHover;
            _mapControl.TouchMove += (s, e) =>
            {
                Device.BeginInvokeOnMainThread(() => MyLocationFollow = false);
            };

            AbsoluteLayout.SetLayoutBounds(_mapControl, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(_mapControl, AbsoluteLayoutFlags.All);

            _mapZoomInButton = new Image { BackgroundColor = Color.Green, WidthRequest = 40, HeightRequest = 40 };
            _mapZoomInButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command((object obj) => Device.BeginInvokeOnMainThread(() => _mapControl.Map.NavigateTo(_mapControl.Map.Viewport.Resolution /= 2)))
            });

            _mapZoomOutButton = new Image { BackgroundColor = Color.LightGreen, WidthRequest = 40, HeightRequest = 40 };
            _mapZoomOutButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command((object obj) => Device.BeginInvokeOnMainThread(() => _mapControl.Map.NavigateTo(_mapControl.Map.Viewport.Resolution *= 2)))
            });

            _mapSpacingButton1 = new Image { BackgroundColor = Color.Transparent, WidthRequest = 40, HeightRequest = 8 };

            _mapMyLocationButton = new Image { BackgroundColor = Color.Red, WidthRequest = 40, HeightRequest = 40 };
            _mapMyLocationButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command((object obj) => Device.BeginInvokeOnMainThread(() => MyLocationFollow = true))
            });

            _mapSpacingButton2 = new Image { BackgroundColor = Color.Transparent, WidthRequest = 40, HeightRequest = 8 };

            _mapNorthingButton = new Image { BackgroundColor = Color.Cyan, WidthRequest = 40, HeightRequest = 40 };
            _mapNorthingButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command((object obj) => Device.BeginInvokeOnMainThread(() => _mapControl.Map.Viewport.Rotation = 0))
            });

            _mapButtons = new StackLayout { BackgroundColor = Color.Transparent, Opacity = 0.8, Spacing = 0, IsVisible = true };

            _mapButtons.Children.Add(_mapZoomInButton);
            _mapButtons.Children.Add(_mapZoomOutButton);
            _mapButtons.Children.Add(_mapSpacingButton1);
            _mapButtons.Children.Add(_mapMyLocationButton);
            _mapButtons.Children.Add(_mapSpacingButton2);
            _mapButtons.Children.Add(_mapNorthingButton);

            AbsoluteLayout.SetLayoutBounds(_mapButtons, new Rectangle(0.95, 0.03, 40, 176));
            AbsoluteLayout.SetLayoutFlags(_mapButtons, AbsoluteLayoutFlags.PositionProportional);

            Content = new AbsoluteLayout
            {
                Children = {
                    _mapControl,
                    _mapButtons,
                }
            };

            _pins.CollectionChanged += HandlerPinsOnCollectionChanged;
            _drawable.CollectionChanged += HandlerDrawablesOnCollectionChanged;

            _mapPinLayer.DataSource = new ObservableCollectionProvider<Pin>(_pins);
            _mapPinLayer.Style = null;  // We don't want a global style for this layer

            _mapDrawableLayer.DataSource = new ObservableCollectionProvider<Drawable>(_drawable);
            _mapDrawableLayer.Style = null;  // We don't want a global style for this layer
        }

        /// <summary>
        /// Events
        /// </summary>

        ///<summary>
        /// Occurs when a pin clicked
        /// </summary>
        public event EventHandler<PinClickedEventArgs> PinClicked;

        /// <summary>
        /// Occurs when selected pin changed
        /// </summary>
        public event EventHandler<SelectedPinChangedEventArgs> SelectedPinChanged;

        /// <summary>
        /// Occurs when map clicked
        /// </summary>
        public event EventHandler<MapClickedEventArgs> MapClicked;

        /// <summary>
        /// Occurs when map long clicked
        /// </summary>
        public event EventHandler<MapLongClickedEventArgs> MapLongClicked;

        /// <summary>
        /// Bindings
        /// </summary>

        public static readonly BindableProperty SelectedPinProperty = BindableProperty.Create(nameof(SelectedPin), typeof(Pin), typeof(MapView), default(Pin), defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty MyLocationEnabledProperty = BindableProperty.Create(nameof(MyLocationEnabled), typeof(bool), typeof(MapView), false, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty MyLocationFollowProperty = BindableProperty.Create(nameof(MyLocationFollow), typeof(bool), typeof(MapView), false, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty UnSnapRotationDegreesProperty = BindableProperty.Create(nameof(UnSnapRotationDegreesProperty), typeof(double), typeof(MapView), default(double));
        public static readonly BindableProperty ReSnapRotationDegreesProperty = BindableProperty.Create(nameof(ReSnapRotationDegreesProperty), typeof(double), typeof(MapView), default(double));
        public static readonly BindableProperty RotationLockProperty = BindableProperty.Create(nameof(RotationLockProperty), typeof(bool), typeof(MapView), default(bool));
        public static readonly BindableProperty ZoomLockProperty = BindableProperty.Create(nameof(ZoomLockProperty), typeof(bool), typeof(MapView), default(bool));
        public static readonly BindableProperty PanLockProperty = BindableProperty.Create(nameof(PanLockProperty), typeof(bool), typeof(MapView), default(bool));

        ///<summary>
        /// Properties
        ///</summary>

        ///<summary>
        /// Native Mapsui Map object
        ///</summary>
        public Map Map
        {
            get
            {
                return _mapControl.Map;
            }
            set
            {
                if (_mapControl.Map.Equals(value))
                    return;

                if (_mapControl.Map != null)
                {
                    _mapControl.Map.Viewport.ViewportChanged -= HandlerViewportChanged;
                    _mapControl.Map.Info -= HandlerInfo;
                    _mapControl.Map.InfoLayers.Remove(_mapPinLayer);
                    _mapControl.Map.InfoLayers.Remove(_mapDrawableLayer);
                    _mapControl.Map.Layers.Remove(_mapPinLayer);
                    _mapControl.Map.Layers.Remove(_mapDrawableLayer);
                    _mapControl.Map.Layers.Remove(_mapMyLocationLayer);
                }

                _mapControl.Map = value;

                if (_mapControl.Map != null)
                {
                    // Get updates of Viewport
                    _mapControl.Map.Viewport.ViewportChanged += HandlerViewportChanged;
                    _mapControl.Map.Info += HandlerInfo;
                    // Add layer for MyLocation
                    _mapControl.Map.Layers.Add(_mapMyLocationLayer);
                    // Draw drawables first
                    _mapControl.Map.Layers.Add(_mapDrawableLayer);
                    _mapControl.Map.InfoLayers.Add(_mapDrawableLayer);
                    // Draw pins on top of drawables
                    _mapControl.Map.Layers.Add(_mapPinLayer);
                    _mapControl.Map.InfoLayers.Add(_mapPinLayer);
                }

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// MyLocation layer
        /// </summary>
        public MyLocationLayer MyLocationLayer
        {
            get { return _mapMyLocationLayer; }
        }

        /// <summary>
        /// Should my location be visible on map
        /// </summary>
        /// <remarks>
        /// Needs a BeginInvokeOnMainThread to change MyLocationLayer.Enabled
        /// </remarks>
        public bool MyLocationEnabled
        {
            get { return (bool)GetValue(MyLocationEnabledProperty); }
            set { Device.BeginInvokeOnMainThread(() => SetValue(MyLocationEnabledProperty, value)); }
        }

        /// <summary>
        /// Should center of map follow my location
        /// </summary>
        public bool MyLocationFollow
        {
            get { return (bool)GetValue(MyLocationFollowProperty); }
            set { SetValue(MyLocationFollowProperty, value); }
        }

        /// <summary>
        /// Pins on map
        /// </summary>
        public IList<Pin> Pins
        {
            get { return _pins; }
        }

        /// <summary>
        /// Selected pin
        /// </summary>
        public Pin SelectedPin
        {
            get { return (Pin)GetValue(SelectedPinProperty); }
            set { SetValue(SelectedPinProperty, value); }
        }

        /// <summary>
        /// List of drawables like polyline and polygon
        /// </summary>
        public IList<Drawable> Drawables
        {
            get { return _drawable; }
        }

        /// <summary>
        /// Number of degrees, before the rotation starts
        /// </summary>
        public double UnSnapRotationDegrees
        {
            get { return (double)GetValue(UnSnapRotationDegreesProperty); }
            set { SetValue(UnSnapRotationDegreesProperty, value); }
        }

        /// <summary>
        /// Number of degrees, when map shows to north
        /// </summary>
        public double ReSnapRotationDegrees
        {
            get { return (double)GetValue(ReSnapRotationDegreesProperty); }
            set { SetValue(ReSnapRotationDegreesProperty, value); }
        }

        /// <summary>
        /// Enable rotation with pinch gesture
        /// </summary>
        public bool RotationLock
        {
            get { return (bool)GetValue(RotationLockProperty); }
            set { SetValue(RotationLockProperty, value); }
        }

        /// <summary>
        /// Enable zooming
        /// </summary>
        public bool ZoomLock
        {
            get { return (bool)GetValue(ZoomLockProperty); }
            set { SetValue(ZoomLockProperty, value); }
        }

        /// <summary>
        /// Enable paning
        /// </summary>
        public bool PanLock
        {
            get { return (bool)GetValue(PanLockProperty); }
            set { SetValue(PanLockProperty, value); }
        }

        /// <summary>
        /// Refresh screen
        /// </summary>
        public void Refresh()
        {
            _mapControl.InvalidateSurface();
        }

        private Callout callout;

        /// <summary>
        /// Creates a callout at the given position
        /// </summary>
        /// <returns>The callout</returns>
        /// <param name="position">Position of callout</param>
        public Callout CreateCallout(Position position)
        {
            if (position == null)
                return null;

            Device.BeginInvokeOnMainThread(() => {
                callout = new Callout(_mapControl)
                {
                    Anchor = position,
                };
            });

            while (callout == null) ;

            var result = callout;
            callout = null;

            return result;
        }

        /// <summary>
        /// Shows given callout
        /// </summary>
        /// <param name="callout">Callout to show</param>
        public void ShowCallout(Callout callout)
        {
            if (callout == null)
                return;

            // Set absolute layout constrains
            AbsoluteLayout.SetLayoutFlags(callout, AbsoluteLayoutFlags.None);

            // Add it to MapView
            if (!((AbsoluteLayout)Content).Children.Contains(callout))
                Device.BeginInvokeOnMainThread(() => ((AbsoluteLayout)Content).Children.Add(callout));

            // Add it to list of active Callouts
            _callouts.Add(callout);

            // When Callout is closed by close button
            callout.CalloutClosed += (s, e) => HideCallout((Callout)s);

            // Inform Callout
            callout.Show();
        }

        /// <summary>
        /// Hides given callout
        /// </summary>
        /// <param name="callout">Callout to hide</param>
        public void HideCallout(Callout callout)
        {
            if (callout == null)
                return;

            // Inform Callout
            callout.Hide();

            // Remove it from list of active Callouts
            _callouts.Remove(callout);

            // Remove it from MapView
            if (((AbsoluteLayout)Content).Children.Contains(callout))
                Device.BeginInvokeOnMainThread(() => ((AbsoluteLayout)Content).Children.Remove(callout));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Pin> GetEnumerator()
        {
            return _pins.GetEnumerator();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName.Equals(nameof(MyLocationEnabled)))
            {
                _mapMyLocationLayer.Enabled = MyLocationEnabled;
                Refresh();
            }

            if (propertyName.Equals(nameof(MyLocationFollow)))
            {
                _mapMyLocationButton.IsEnabled = !MyLocationFollow;

                if (MyLocationFollow)
                {
                    _mapControl.Map.NavigateTo(_mapMyLocationLayer.MyLocation.ToMapsui());    
                }

                Refresh();
            }

            if (propertyName.Equals(nameof(UnSnapRotationDegreesProperty)))
                _mapControl.UnSnapRotationDegrees = UnSnapRotationDegrees;

            if (propertyName.Equals(nameof(ReSnapRotationDegreesProperty)))
                _mapControl.ReSnapRotationDegrees = ReSnapRotationDegrees;
            
            if (propertyName.Equals(nameof(RotationLockProperty)))
                _mapControl.RotationLock = RotationLock;

            if (propertyName.Equals(nameof(ZoomLockProperty)))
                _mapControl.ZoomLock = ZoomLock;

            if (propertyName.Equals(nameof(PanLockProperty)))
                _mapControl.PanLock = PanLock;
        }

        /// <summary>
        /// Handlers
        /// </summary>

        /// <summary>
        /// Viewport of map has changed
        /// </summary>
        /// <param name="sender">Viewport of this event</param>
        /// <param name="e">Event arguments containing what changed</param>
        private void HandlerViewportChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Viewport.Rotation)))
            {
                _mapMyLocationLayer.UpdateMyDirection(_mapMyLocationLayer.Direction, Map.Viewport.Rotation);

                // Check all callout positions
                var list = _callouts.ToList();

                // First check all Callouts, that belong to a pin
                foreach (var pin in _pins)
                {
                    if (pin.Callout != null)
                    {
                        pin.UpdateCalloutPosition();
                        list.Remove(pin.Callout);
                    }
                }

                // Now check the rest, Callouts not belonging to a pin
                foreach (var callout in list)
                    callout.UpdateScreenPosition();

            }
            if (e.PropertyName.Equals(nameof(Viewport.Center)))
            {
            }
        }

        private void HandlerHover(object sender, HoveredEventArgs e)
        {
        }

        private void HandlerPinsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Cast<Pin>().Any(pin => pin.Label == null))
                throw new ArgumentException("Pin must have a Label to be added to a map");

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    // Remove old pins from layer
                    var pin = item as Pin;

                    HideCallout(pin.Callout);
                    pin.Callout = null;

                    pin.PropertyChanged -= HandlerPinPropertyChanged;

                    if (SelectedPin.Equals(pin))
                        SelectedPin = null;
                }
            }

            foreach (var item in e.NewItems)
            {
                // Add new pins to layer
                var pin = item as Pin;

                pin.PropertyChanged += HandlerPinPropertyChanged;
            }
        }

        private void HandlerDrawablesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // TODO: Do we need any information about this?
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    // Remove old drawables from layer
                    var drawable = item as INotifyPropertyChanged;

                    drawable.PropertyChanged -= HandlerDrawablePropertyChanged;
                }
            }

            foreach (var item in e.NewItems)
            {
                // Add new drawables to layer
                var drawable = item as INotifyPropertyChanged;

                drawable.PropertyChanged += HandlerDrawablePropertyChanged;
            }
        }

        private void HandlerInfo(object sender, MapInfoEventArgs e)
        {
            // Click on pin?
            Pin clickedPin = null;
            
            foreach(var pin in _pins)
            {
                if (pin.IsVisible && pin.Feature.Equals(e.MapInfo.Feature))
                {
                    clickedPin = pin;
                    break;
                }
            }

            if (clickedPin != null)
            {
                SelectedPin = clickedPin;

                SelectedPinChanged?.Invoke(this, new SelectedPinChangedEventArgs(SelectedPin));

                var pinArgs = new PinClickedEventArgs(clickedPin, Map.Viewport.ScreenToWorld(e.MapInfo.ScreenPosition).ToForms(), e.NumTaps);

                PinClicked?.Invoke(this, pinArgs);

                if (pinArgs.Handled)
                {
                    e.Handled = true;
                    return;
                }
            }

            // Check for clicked drawables
            var drawables = GetDrawablesAt(Map.Viewport.ScreenToWorld(e.MapInfo.ScreenPosition), _mapDrawableLayer);

            var drawableArgs = new DrawableClickedEventArgs(Map.Viewport.ScreenToWorld(e.MapInfo.ScreenPosition).ToForms(), new Xamarin.Forms.Point(e.MapInfo.ScreenPosition.X, e.MapInfo.ScreenPosition.Y), e.NumTaps);

            // Now check each drawable until one handles the event
            foreach (var drawable in drawables)
            {
                drawable.HandleClicked(drawableArgs);

                if (drawableArgs.Handled)
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void HandlerLongTap(object sender, TappedEventArgs e)
        {
            var args = new MapLongClickedEventArgs(Map.Viewport.ScreenToWorld(e.ScreenPosition).ToForms());

            MapLongClicked?.Invoke(this, args);

            if (args.Handled)
            {
                e.Handled = true;
                return;
            }
        }

        private void HandlerTap(object sender, TappedEventArgs e)
        {
            // Close all closable Callouts
            var list = _callouts.ToList();

            // First check all Callouts, that belong to a pin
            foreach (var pin in _pins)
            {
                if (pin.Callout != null)
                {
                    if (pin.Callout.IsClosableByClick)
                        pin.IsCalloutVisible = false;
                    list.Remove(pin.Callout);
                }
            }

            // Now check the rest, Callouts not belonging to a pin
            foreach (var callout in list)
                if (callout.IsClosableByClick)
                    HideCallout(callout);

            // Check, if we hit a widget or drawable
            // Is there a widget at this position
            // Is there a drawable at this position
            if (Map != null)
                e.Handled = Map.InvokeInfo(e.ScreenPosition * _mapControl.SkiaScale, e.ScreenPosition * _mapControl.SkiaScale, _mapControl.SymbolCache, null, e.NumOfTaps);

            if (e.Handled)
                return;

            var args = new MapClickedEventArgs(Map.Viewport.ScreenToWorld(e.ScreenPosition).ToForms(), e.NumOfTaps);

            MapClicked?.Invoke(this, args);

            if (args.Handled)
            {
                e.Handled = true;
                return;
            }

            // Event isn't handled up to now.
            // Than look, what we could do.
        }

        private void HandlerPinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Map.RefreshData(false);
        }

        private void HandlerDrawablePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Map.RefreshData(false);
        }

        /// <summary>
        /// Get all drawables of layer that contain given point
        /// </summary>
        /// <param name="point">Point to search for in world coordinates</param>
        /// <param name="layer">Layer to search for drawables</param>
        /// <returns>List with all drawables at point, which are clickable</returns>
        private IList<Drawable> GetDrawablesAt(Geometries.Point point, Layer layer)
        {
            List<Drawable> drawables = new List<Drawable>();

            if (layer.Enabled == false) return drawables;
            if (layer.MinVisible > Map.Viewport.Resolution) return drawables;
            if (layer.MaxVisible < Map.Viewport.Resolution) return drawables;

            var allFeatures = layer.GetFeaturesInView(layer.Envelope, Map.Viewport.Resolution);

            // Now check all features, if they are clicked and clickable
            foreach (var feature in allFeatures)
            {
                if (feature.Geometry.Contains(point))
                {
                    var drawable = _drawable.Where(f => f.Feature == feature).First();
                    // Take only the clickable object
                    if (drawable.IsClickable)
                        drawables.Add(drawable);
                }
            }

            // If there more than one drawables found, than reverse, because the top most should be the first
            if (drawables.Count > 1)
                drawables.Reverse();

            return drawables;
        }
    }
}