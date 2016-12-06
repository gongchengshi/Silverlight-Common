// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Controls;
using SEL.Collections.Generic;

namespace SEL.Silverlight.RegionPartitioner
{
    /// <summary>
    /// Allows persisting a Region to a file using the IPersistable interface.
    /// </summary>
    public partial class PersistableRegion : IPersistable
    {
        public PersistableRegion(ObservableSortedSet<IRegionContent> regionContentInfo)
        {
            DataContext = this;

            RegionContentInfo = regionContentInfo;

            InitializeComponent();

            SelectedContent.Changed +=
                () =>
                    {
                        if (SelectedContent.Value == null)
                        {
                            Title.Text = string.Empty;
                            ClearContent();
                            ContentOptions.Visibility = Visibility.Visible;
                            return;
                        }

                        Title.Text = SelectedContent.Value.Name;
                        SetContent(SelectedContent.Value.Create());
                        ContentOptions.Visibility = Visibility.Collapsed;
                    };
        }

        public ObservableSortedSet<IRegionContent> RegionContentInfo { get; private set; }

        private PersistableRegionGrid ParentGrid
        {
            get { return Parent as PersistableRegionGrid; }
        }

        private void NestIfNeeded()
        {
            if ((ParentGrid is IRegionContainer) && (ParentGrid.Children.Count <= 1))
            {
                return;
            }

            var previousParent = ParentGrid;
            previousParent.Children.Remove(this);

            var newParent = new PersistableRegionGrid(RegionContentInfo);
            newParent.Children.Add(this);
                
            if (this == previousParent.RegionOne)
            {
                previousParent.AddAndSetRegion(RegionGrid.RegionID.One, newParent);
            }
            else if (this == previousParent.RegionTwo)
            {
                previousParent.AddAndSetRegion(RegionGrid.RegionID.Two, newParent);
            }
        }

        private void Unnest()
        {
            var myParent = ParentGrid;

            var sibling = (this == myParent.RegionOne) ? myParent.RegionTwo : myParent.RegionOne;

            myParent.Clear();

            if (sibling == null)
            {
                myParent.Children.Add(new PersistableRegion(RegionContentInfo));
                return;
            }

            if (myParent is IRegionContainer) // This is the top most grid
            {                
                myParent.Children.Add(sibling);
            }
            else if (myParent.Parent is PersistableRegionGrid)
            {
                var parentsParent = myParent.Parent as PersistableRegionGrid;
                var parentsRegion = (parentsParent.RegionOne == myParent) ? RegionGrid.RegionID.One : RegionGrid.RegionID.Two;

                parentsParent.Children.Remove(myParent);

                parentsParent.AddAndSetRegion(parentsRegion, sibling);
            }
        }

        private void SplitVerticallyClick(object sender, RoutedEventArgs e)
        {
            Split(true);
        }

        private void SplitHorizontallyClick(object sender, RoutedEventArgs e)
        {
            Split(false);
        }

        private void Split(bool vertically)
        {
            NestIfNeeded();

            if (vertically)
            {
                ParentGrid.SplitVertically();
            }
            else
            {
                ParentGrid.SplitHorizontally();
            }

            ParentGrid.SetRegion(RegionGrid.RegionID.One, this);
            ParentGrid.AddAndSetRegion(RegionGrid.RegionID.Two, new PersistableRegion(RegionContentInfo));
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            DisposeContent();
            Unnest();
        }

        public Property<IRegionContent> SelectedContent { get { return _SelectedContent; } }
        private readonly Property<IRegionContent> _SelectedContent = new Property<IRegionContent>();

        internal void Reset()
        {
            Title.Text = string.Empty;
            LayoutRoot.Children.Remove(RegionContent);
            ContentOptions.Visibility = Visibility.Visible;
        }

        private void SetContent(FrameworkElement content)
        {
            LayoutRoot.Children.Add(content);
            Grid.SetRow(content, 1);
            Grid.SetColumn(content, 0);
            RegionContent = content;
        }

        private void DisposeContent()
        {
            var found = RegionContent as IDisposable;
            if (found != null)
            {
                found.Dispose();
            }
            RegionContent = null;
        }

        private void ClearContent()
        {
            LayoutRoot.Children.Remove(RegionContent);
            DisposeContent();
        }

        public FrameworkElement RegionContent { get; private set; }

        public string Persist()
        {
            return (RegionContent is IPersistable)
                       ? (RegionContent as IPersistable).Persist()
                       : string.Empty;
        }

        public void Restore(string xml)
        {
            (RegionContent as IPersistable).Restore(xml);
        }

        public void Dispose()
        {
            DisposeContent();
        }
    }
}
