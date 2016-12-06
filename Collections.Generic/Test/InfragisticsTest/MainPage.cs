///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Controls;
using DeathToXAML;
using Infragistics.Controls.Charts;

namespace InfragisticsTest
{
    public class MainPage : View
    {
        protected override object ViewModel { get { return _viewModel; } set { _viewModel = (TestViewModel)value; } }
        private TestViewModel _viewModel;

        public MainPage()
        {

        }

        protected override Element BuildView()
        {
            var chart = new XamDataChart();
            var yAxis = new NumericYAxis();
            chart.Axes.Add(yAxis);
            var xAxis = new CategoryDateTimeXAxis()
            {
                DateTimeMemberPath = "",
                Visibility = System.Windows.Visibility.Visible,
                Label = "{0:u}"
            };

            new PropertyBinding(CategoryDateTimeXAxis.ItemsSourceProperty, _viewModel.AxisData).Bind(xAxis);
            //new PropertyBinding(CategoryDateTimeXAxis.MinimumValueProperty, _viewModel.DataTimeRange.Value.Start).Bind(xAxis);            
            //new PropertyBinding(CategoryDateTimeXAxis.MaximumValueProperty, _viewModel.DataTimeRange.Value.End).Bind(xAxis);
            
            chart.Axes.Add(xAxis);

            var series = new LineSeries()
            {
                ValueMemberPath = "Value",
                XAxis = xAxis,
                YAxis = yAxis
            };

            new PropertyBinding(LineSeries.ItemsSourceProperty, _viewModel.SeriesData).Bind(series);

            chart.Series.Add(series);

            var grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(300.0, GridUnitType.Pixel) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(300.0, GridUnitType.Star) });

            return new Element(grid)
            {   
                new GridElement(chart, 0, 0),
                new GridElement(new ListBox() { DisplayMemberPath = "" }, 1, 0 )
                {
                    new PropertyBinding(ListBox.ItemsSourceProperty, _viewModel.AxisData)
                }
                //new GridElement(new ListBox() { DisplayMemberPath = "Timestamp" }, 1, 0 )
                //{
                //    new PropertyBinding(ListBox.ItemsSourceProperty, _viewModel.SeriesData)
                //}
            };
        }
    }
}
