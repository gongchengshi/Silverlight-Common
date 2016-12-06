///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeathToXAML;
using SEL.Collections.Generic;
using System.Windows.Threading;

namespace InfragisticsTest
{
    public class DataPoint
    {
        public Single Value { get; set; }
        public DateTime Timestamp { get; set; }

        public static IComparer<DataPoint> ComparerInstance = new Comparer();

        public class Comparer : IComparer<DataPoint>
        {
            public int Compare(DataPoint x, DataPoint y)
            {
                return x.Timestamp.CompareTo(y.Timestamp);
            }
        }
    }

    public class TimeRangeViewModel
    {
        public TimeRangeViewModel(DateTime start, DateTime end)
        {
            Start = new Property<DateTime>(start);
            End = new Property<DateTime>(end);
        }

        public Property<DateTime> Start = new Property<DateTime>();
        public Property<DateTime> End = new Property<DateTime>();
    }

    public class TestViewModel
    {
        DispatcherTimer _timer = new DispatcherTimer();
        Random _rand = new Random(1001);
        TimeSpan _interval = new TimeSpan(0, 0, 1);
        TimeSpan _spacing;
        const int _numDataPointsPerUpdate = 100;

        public TestViewModel()
        {
            _spacing = new TimeSpan(_interval.Ticks / _numDataPointsPerUpdate);

            _timer.Interval =_interval;
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            var tempSeriesData = new List<DataPoint>(_numDataPointsPerUpdate);
            var tempAxisData = new List<DateTime>(_numDataPointsPerUpdate);

            for (int i = 0; i < _numDataPointsPerUpdate; ++i)
            {
                tempSeriesData.Add(new DataPoint() 
                { 
                    Timestamp = DataTimeRange.Value.End.Value, 
                    Value = Convert.ToSingle(_rand.NextDouble() * 10) 
                });
                tempAxisData.Add(DataTimeRange.Value.End.Value);
                DataTimeRange.Value.End.Value += _spacing;
            }
            
            var minTimestamp = DataTimeRange.Value.End.Value.Ticks - (_spacing.Ticks * 1000);

            SeriesData.Value.RemoveAll((s) => { return s.Timestamp.Ticks < minTimestamp; });
            AxisData.Value.RemoveAll((s) => { return s.Ticks < minTimestamp; });

            SeriesData.Value.UnionWith(tempSeriesData);
            AxisData.Value.UnionWith(tempAxisData);
        }

        public Property<ObservableSortedSet<DataPoint>> SeriesData = 
            new Property<ObservableSortedSet<DataPoint>>(
                new ObservableSortedSet<DataPoint>(DataPoint.ComparerInstance));
        public Property<ObservableSortedSet<DateTime>> AxisData = 
            new Property<ObservableSortedSet<DateTime>>(new ObservableSortedSet<DateTime>());

        public Property<TimeRangeViewModel> DataTimeRange = 
            new Property<TimeRangeViewModel>(
                new TimeRangeViewModel(new DateTime(2000, 1, 1), new DateTime(2000, 1, 1)));
    }
}
