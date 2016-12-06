using System.Windows;
using Regions;
using SEL.Silverlight.RegionPartitioner;

namespace View1
{
    [TargetRegion(Location = Location.MainPanel)]
    public class RegionContentInfo : RegionContent<RegionContentInfo>
    {
        public override string Name
        {
            get { return "View 1"; }
        }

        public override int Ordinal
        {
            get { return 1; }
        }

        protected override void InitI()
        { }

        protected override FrameworkElement CreateI()
        {
            return new MainPage();
        }
    }
}
