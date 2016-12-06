using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using SEL;
using SEL.Collections.Generic;
using SEL.Silverlight.RegionPartitioner;
using System.Linq;

namespace Integration
{
    public partial class MainPage : IPersistable
    {
        public MainPage()
        {
            var viewModel = new ViewModel();

            DataContext = viewModel;

            InitializeComponent();

            MainPanel.Children.Add(new Region(viewModel.RegionManager.MainPanelContentOptions));
        }

        public string Persist()
        {
            var info = new PersistedInfo 
                { RootLayoutColumnWidths = new List<GridWidth>(LayoutRoot.ColumnDefinitions.Count) };

            info.RootLayoutColumnWidths.AddRange(LayoutRoot.ColumnDefinitions, def => new GridWidth(def.Width));

            UserStoreSerialization<PersistedInfo>.Serialize(info, "LayoutRootColumns");

            return string.Empty;
        }

        public void Restore(string xml)
        {
            throw new NotImplementedException();
        }

        public void Restore()
        {
            try
            {
                var info = UserStoreSerialization<PersistedInfo>.Deserialize("LayoutRootColumns", true);

                if (info == null)
                {
                    return;
                }

                ForEach.Do(LayoutRoot.ColumnDefinitions, info.RootLayoutColumnWidths, (def, width) =>
                    {
                        def.Width = new GridLength(width.Value, width.GridUnitType);
                    });
            }
            catch (Exception ex)
            {
                Debugger.Break();                
            }
        }

        public class PersistedInfo
        {
            public List<GridWidth> RootLayoutColumnWidths;
        }

        public class GridWidth
        {
            public GridWidth()
            {}

            public GridWidth(GridLength width)
            {
                GridUnitType = width.GridUnitType;
                Value = width.Value;
            }

            public GridUnitType GridUnitType;
            public double Value;
        }
    }
}
