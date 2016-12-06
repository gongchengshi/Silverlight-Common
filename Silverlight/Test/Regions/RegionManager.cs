using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Collections.Specialized;
using System.Diagnostics;
using SEL.MEF;
using SEL.Silverlight.RegionPartitioner;
using SEL.Collections.Generic;

namespace Regions
{
    public class RegionManager : IPartImportsSatisfiedNotification
    {
        public ObservableSortedSet<IRegionContent> MainPanelContentOptions { get { return _mainPanelContentOptions; } }
        private readonly ObservableSortedSet<IRegionContent> _mainPanelContentOptions = new ObservableSortedSet<IRegionContent>();

        [ImportMany(AllowRecomposition = true)]
        public ObservableCollection<Lazy<IRegionContent, IRegionContentMetaData>> RegionContentInfo { get; set; }

        public ICatalogService CatalogService = new CatalogService();

        public RegionManager()
        {
            RegionContentInfo = new ObservableCollection<Lazy<IRegionContent, IRegionContentMetaData>>();
            RegionContentInfo.CollectionChanged += RegionContentInfoCollectionChanged;

            MainPanelContentOptions.CollectionChanged += MainPanelContentOptions_CollectionChanged;

            CatalogService.AddRemoteXap("Modules\\View1.xap");
            CatalogService.AddRemoteXap("Modules\\View2.xap");

            CompositionInitializer.SatisfyImports(this);
        }

        void MainPanelContentOptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int i = 9;
        }

        void RegionContentInfoCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
            {
                return;
            }

            foreach (var item in e.NewItems)
            {
                var controlInfo = item as Lazy<IRegionContent, IRegionContentMetaData>;

                if (controlInfo != null)
                {
                    switch (controlInfo.Metadata.Location)
                    {
                        case Location.MainPanel:
                            MainPanelContentOptions.Add(controlInfo.Value);
                            break;
                    }
                }
            }
        }

        public void OnImportsSatisfied()
        {
            Debug.WriteLine("RegionManager Imports Satisfied");
        }
    }
}
