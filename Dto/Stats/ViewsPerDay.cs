using System;
using System.Collections.Generic;

namespace Dto.Stats
{
    public class ViewsPerDay
    {
        public IEnumerable<DayViewSample> Samples { get; set; }
    }

    public class DayViewSample
    {
        public DateTime Date { get; set; }
        public int Views { get; set; }
    }
}