using System.Collections.Generic;

namespace Dto.Stats
{
    public class GeographicViews
    {
        public IEnumerable<GeographicViewSample> Samples { get; set; }
    }

    public class GeographicViewSample
    {
        public float CentroidLat { get; set; }
        public float CentroidLong { get; set; }
        public int Views { get; set; }
    }
}