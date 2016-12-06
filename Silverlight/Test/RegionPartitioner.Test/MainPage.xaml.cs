using System.IO;
using System.Windows;
using System.Windows.Controls;
using SEL.Collections.Generic;

namespace SEL.Silverlight.RegionPartitioner
{
    public partial class MainPage
    {
        public MainPage()
        {
            DataContext = new ViewModel();

            InitializeComponent();

            var regions = new IRegionContent[] {new Region1(), new Region2(), new Region3()};

            MainPanel.Initialize(new ObservableSortedSet<IRegionContent>(regions));
        }

        private void Persist_Click(object sender, RoutedEventArgs e)
        {
            var results = MainPanel.Persist();

            MessageBox.Show(results);
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            var d = new OpenFileDialog();

            if (d.ShowDialog() == true)
            {
                string xml;

                using (var reader = d.File.OpenText())
                {
                    xml = reader.ReadToEnd();
                }

                MainPanel.Restore(xml);
            }
        }

        private void PersistToFile_Click(object sender, RoutedEventArgs e)
        {
            var d = new SaveFileDialog();

            if (d.ShowDialog() == true)
            {
                var results = MainPanel.Persist();

                using (var writer = new StreamWriter(d.OpenFile()))
                {
                    writer.Write(results);
                }
            }
        }
    }
}
