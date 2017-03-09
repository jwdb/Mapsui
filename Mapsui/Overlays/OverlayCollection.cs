using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mapsui.Overlays
{
    public class OverlayCollection : ICollection<IOverlay>
    {
        private readonly IList<IOverlay> _overlays = new List<IOverlay>();
        
        public delegate void OverlayRemovedEventHandler(IOverlay overlay);
        public delegate void OverlayAddedEventHandler(IOverlay Overlay);
        public delegate void OverlayMovedEventHandler(IOverlay Overlay);

        public event OverlayRemovedEventHandler OverlayRemoved;
        public event OverlayAddedEventHandler OverlayAdded;
        public event OverlayMovedEventHandler OverlayMoved;

        public int Count => _overlays.Count;

        public bool IsReadOnly => _overlays.IsReadOnly;

        public IEnumerator<IOverlay> GetEnumerator()
        {
            return _overlays.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _overlays.GetEnumerator();
        }

        public void Clear()
        {
            foreach (var overlay in _overlays)
            {
                OnOverlayRemoved(overlay);
            }
			_overlays.Clear();
        }

        public bool Contains(IOverlay item)
        {
            return _overlays.Contains(item);
        }

        public void CopyTo(IOverlay[] array, int arrayIndex)
        {
			_overlays.CopyTo(array, arrayIndex);
        }

        public IOverlay this[int index]
        {
            get { return _overlays[index]; }
        }

        public void Add(IOverlay overlay)
        {
            if (overlay == null) throw new ArgumentException("Overlay cannot be null");
			_overlays.Add(overlay);
            OnOverlayAdded(overlay);
        }

        public void Move(int index, IOverlay overlay)
        {
			_overlays.Remove(overlay);
			_overlays.Insert(index, overlay);
            OnOverlayMoved(overlay);
        }

        public void Insert(int index, IOverlay overlay)
        {
			_overlays.Insert(index, overlay);
            OnOverlayAdded(overlay);
        }

        public bool Remove(IOverlay overlay)
        {
            var success = _overlays.Remove(overlay);
            OnOverlayRemoved(overlay);
            return success;
        }

        private void OnOverlayRemoved(IOverlay overlay)
        {
            OverlayRemoved?.Invoke(overlay);
        }

        private void OnOverlayAdded(IOverlay overlay)
        {
            OverlayAdded?.Invoke(overlay);
        }

        private void OnOverlayMoved(IOverlay overlay)
        {
            OverlayMoved?.Invoke(overlay);
        }

        public IEnumerable<IOverlay> FindOverlay(string overlayname)
        {
            return _overlays.Where(overlay => overlay.Name.Contains(overlayname));
        }
    }
}