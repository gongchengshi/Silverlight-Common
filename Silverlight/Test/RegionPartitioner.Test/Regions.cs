// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System.Windows;
using System.Windows.Controls;

namespace SEL.Silverlight.RegionPartitioner
{
    public class RegionViewModel
    {
        public string Name { get; set; }
    }

    public class PersistableUserControl : TextBox, IPersistable
    {
        public RegionViewModel ViewModel
        {
            get { return _ViewModel; }
            set
            {
                _ViewModel = value;
                Text = _ViewModel.Name;
            }
        }

        private RegionViewModel _ViewModel;

        public PersistableUserControl()
        {
            IsReadOnly = true;
        }

        public string Persist()
        {
            return PersistanceHelpers.Serialize(ViewModel);
        }

        public void Restore(string xml)
        {
            ViewModel = PersistanceHelpers.Deserialize<RegionViewModel>(xml);
        }
    }

    public class Region1 : RegionContent<Region1>
    {
        public override string Name
        {
            get { return "Region 1"; }
        }

        public override int Ordinal
        {
            get { return 1; }
        }

        protected override void InitI()
        { }

        protected override FrameworkElement CreateI()
        {
            return new PersistableUserControl { ViewModel = new RegionViewModel { Name = Name } };
        }
    }
    public class Region2 : RegionContent<Region2>
    {
        public override string Name
        {
            get { return "Region 2"; }
        }

        public override int Ordinal
        {
            get { return 2; }
        }

        protected override void InitI()
        { }

        protected override FrameworkElement CreateI()
        {
            return new PersistableUserControl { ViewModel = new RegionViewModel { Name = Name } };
        }
    }

    public class Region3 : RegionContent<Region3>
    {
        public override string Name
        {
            get { return "Region 3"; }
        }

        public override int Ordinal
        {
            get { return 1; }
        }

        protected override void InitI()
        { }

        protected override FrameworkElement CreateI()
        {
            return new PersistableUserControl { ViewModel = new RegionViewModel { Name = Name } };
        }
    }

    public class Region4 : RegionContent<Region4>
    {
        public override string Name
        {
            get { return "Region 4"; }
        }

        public override int Ordinal
        {
            get { return 4; }
        }

        protected override void InitI()
        { }

        protected override FrameworkElement CreateI()
        {
            return new PersistableUserControl { ViewModel = new RegionViewModel { Name = Name } };
        }
    }
}