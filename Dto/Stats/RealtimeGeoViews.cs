using System;
using System.Collections.Generic;

namespace Dto.Stats
{
    public class RealtimeGeoViews
    {
        public IEnumerable<GeoViewSample> Samples { get; set; }
    }

    public class GeoViewSample
    {
        public long Id { get; set; }
        public float Lat { get; set; }
        public float Long { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
    }
}