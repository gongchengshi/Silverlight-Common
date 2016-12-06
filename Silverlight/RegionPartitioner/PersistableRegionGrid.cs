// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using SEL.Collections.Generic;

namespace SEL.Silverlight.RegionPartitioner
{
    public class PersistableRegionGrid : RegionGrid, IPersistable
    {
        protected ObservableSortedSet<IRegionContent> _regionInfo;

        /// <summary>
        /// This constructor is only here to allow default constructors on derived classes that are used from XAML.
        /// </summary>
        public PersistableRegionGrid() {}

        internal PersistableRegionGrid(ObservableSortedSet<IRegionContent> regionInfo)
        {
            _regionInfo = regionInfo;
        }

        public string Persist()
        {
            var persisted = new RegionGridPersistedAttributes
                                {
                                    SplitState = SplitState,
                                    RegionOneSize = RegionOneSize,
                                    RegionTwoSize = RegionTwoSize
                                };

            if (RegionOne is PersistableRegion)
            {
                var region = RegionOne as PersistableRegion;

                persisted.RegionOneContent = region.Persist();
                persisted.RegionOneName = region.Title.Text;
            }
            else if (RegionOne is PersistableRegionGrid)
            {
                persisted.RegionOneContent = (RegionOne as PersistableRegionGrid).Persist();
            }

            if (RegionTwo is PersistableRegion)
            {
                var region = RegionTwo as PersistableRegion;

                persisted.RegionTwoContent = region.Persist();
                persisted.RegionTwoName = region.Title.Text;
            }
            else if (RegionTwo is PersistableRegionGrid)
            {
                persisted.RegionTwoContent = (RegionTwo as PersistableRegionGrid).Persist();
            }

            return PersistanceHelpers.Serialize(persisted);
        }

        public virtual void Restore(string xml)
        {
            var persistedRegionGrid = DataContractXmlSerializer<RegionGridPersistedAttributes>.FromString(xml);

            if (persistedRegionGrid.SplitState == Split.Vertically)
            {
                SplitVertically();
            }
            else if (persistedRegionGrid.SplitState == Split.Horizontally)
            {
                SplitHorizontally();
            }

            RestoreRegion(RegionID.One, persistedRegionGrid.RegionOneName, persistedRegionGrid.RegionOneContent);

            if (persistedRegionGrid.SplitState != Split.None)
            {
                RestoreRegion(RegionID.Two, persistedRegionGrid.RegionTwoName, persistedRegionGrid.RegionTwoContent);

                RegionOneSize = persistedRegionGrid.RegionOneSize;
                RegionTwoSize = persistedRegionGrid.RegionTwoSize;
            }
        }

        private void RestoreRegion(RegionID region, string name, string xml)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var currentRegion = GetRegion(region) as PersistableRegion;

                if (currentRegion == null)
                {
                    currentRegion = new PersistableRegion(_regionInfo);
                    AddAndSetRegion(region, currentRegion);
                }

                var found = currentRegion.RegionContentInfo.FirstOrDefault(x => x.Name == name);

                if (found != null)
                {
                    currentRegion.SelectedContent.Value = found;
                    currentRegion.Restore(xml);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    var newRegionGrid = new PersistableRegionGrid(_regionInfo);
                    AddAndSetRegion(region, newRegionGrid);

                    newRegionGrid.Restore(xml);
                }
                else
                {
                    AddAndSetRegion(region, new PersistableRegion(_regionInfo));
                }
            }
        }

        public class RegionGridPersistedAttributes
        {
            public Split SplitState;
            public string RegionOneName;
            public double RegionOneSize;
            public string RegionOneContent;
            public string RegionTwoName;
            public double RegionTwoSize;
            public string RegionTwoContent;
        }

        public void Dispose()
        {
            if (RegionOne is IDisposable)
            {
                (RegionOne as IDisposable).Dispose();
            }

            if (RegionTwo is IDisposable)
            {
                (RegionTwo as IDisposable).Dispose();
            }
        }
    }

    public class PersistableRegionContainer : PersistableRegionGrid, IRegionContainer
    {
        public override void Restore(string xml)
        {
            Clear();
            base.Restore(xml);
        }

        public void Initialize(ObservableSortedSet<IRegionContent> regionInfo)
        {
            _regionInfo = regionInfo;
            AddAndSetRegion(RegionID.One, new PersistableRegion(_regionInfo));
        }
    }
}