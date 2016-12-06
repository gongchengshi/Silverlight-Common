// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Controls;

namespace SEL.Silverlight.RegionPartitioner
{
    /// <summary>
    /// A resizeable grid that is splitable into two regions
    /// 
    /// RegionGrid may only contain RegionGrid, Region, and/or GridSplitter
    /// </summary>
    public class RegionGrid : Grid
    {
        public enum Split
        {
            None,
            Vertically,
            Horizontally
        }

        public enum RegionID
        {
            One = 0,
            Two = 2 // Column/Row 3
        }

        internal static void SetPosition(FrameworkElement element, int row, int col)
        {
            SetRow(element, row);
            SetColumn(element, col);
        }

        internal Split SplitState { get; private set; }

        internal void SplitHorizontally()
        {
            SplitState = Split.Horizontally;

            ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            ColumnDefinitions.Add(new ColumnDefinition());

            Splitter = new GridSplitter
            {
                ShowsPreview = true,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 2
            };
            Children.Add(Splitter);

            SetPosition(Splitter, 0, 1);
        }

        internal void SplitVertically()
        {
            SplitState = Split.Vertically;

            RowDefinitions.Add(new RowDefinition());
            RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});
            RowDefinitions.Add(new RowDefinition());

            Splitter = new GridSplitter
            {
                ShowsPreview = true,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 2
            };
            Children.Add(Splitter);

            SetPosition(Splitter, 1, 0);
        }

        /// <summary>
        /// Removes any rows and columns and the splitter if they exist.
        /// </summary>
        internal void Join()
        {
            ColumnDefinitions.Clear();
            RowDefinitions.Clear();

            SplitState = Split.None;

            Children.Remove(Splitter);
            Splitter = null;
        }

        internal void SetRegion(RegionID region, FrameworkElement element)
        {
            if (SplitState == Split.Vertically)
            {
                SetPosition(element, (int)region, 0);
            }
            else
            {
                SetPosition(element, 0, (int)region);
            }

            switch (region)
            {
                case RegionID.One:
                    RegionOne = element;
                    break;
                case RegionID.Two:
                    RegionTwo = element;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("region");
            }
        }

        internal void AddAndSetRegion(RegionID region, FrameworkElement element)
        {
            Children.Add(element);
            SetRegion(region, element);
        }

        private double GetSize(RegionID region)
        {
            switch (SplitState)
            {
                case Split.None:
                    return 0;
                case Split.Vertically:
                    return RowDefinitions[(int)region].Height.Value;
                case Split.Horizontally:
                    return ColumnDefinitions[(int)region].Width.Value;
                default:
                    throw new ArgumentException();
            }
        }

        private void SetSize(RegionID region, double size)
        {
            switch (SplitState)
            {
                case Split.None:
                    break;
                case Split.Vertically:
                    RowDefinitions[(int)region].Height = new GridLength(size, GridUnitType.Star);
                    break;
                case Split.Horizontally:
                    ColumnDefinitions[(int)region].Width = new GridLength(size, GridUnitType.Star);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        protected double RegionOneSize
        {
            get { return GetSize(RegionID.One); }
            set { SetSize(RegionID.One, value); }
        }

        protected double RegionTwoSize
        {
            get { return GetSize(RegionID.Two); }
            set { SetSize(RegionID.Two, value); }
        }
       
        public FrameworkElement GetRegion(RegionID region)
        {
            switch(region)
            {
                case RegionID.One:
                    return RegionOne;
                case RegionID.Two:
                    return RegionTwo;
                default:
                    throw new ArgumentOutOfRangeException("region");
            }
        }

        public FrameworkElement RegionOne
        {
            get
            {
                // For convenience sake, RegionOne returns the first and only child if no regions have been set.
                if (_RegionOne == null && Children.Count == 1)
                {
                    return Children[0] as FrameworkElement;
                }

                return _RegionOne;
            }
            set { _RegionOne = value; }
        }
        private FrameworkElement _RegionOne;

        private GridSplitter Splitter { get; set; }       

        public FrameworkElement RegionTwo { get; set; }

        public void Clear()
        {
            Children.Clear();
            RegionOne = null;
            RegionTwo = null;
            Join();
        }
    }

    public interface IRegionContainer
    {
        void Clear();
    }

    public class RegionContainer : RegionGrid, IRegionContainer
    {}
}
