﻿//
// Found at https://github.com/mapsforge/vtm/blob/master/vtm/src/org/oscim/scalebar/DistanceUnitAdapter.java
//

namespace Mapsui.Overlays
{
	public interface IUnitConverter
	{
		double MeterRatio { get; }

		int[] ScaleBarValues { get; }

		string GetScaleText(int mapScaleValue);
	}
}