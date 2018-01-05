using System;
using System.Collections.Generic;

namespace Dto.Stats
{
    public class ViewsOverTime
    {
        public IEnumerable<DayViewSample> ViewsPerDay { get; set; }
        public int TotalPreviousPeriod { get; set; }
        // Future... Views per month, or per 30 days for rolled up stats
    }

    public class DayViewSample
    {
        public DateTime Date { get; set; }
        public int Views { get; set; }
    }
}