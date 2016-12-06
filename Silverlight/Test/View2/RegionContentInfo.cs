using System.Windows;
using Regions;
using SEL.Silverlight.RegionPartitioner;

namespace View2
{
    [TargetRegion(Location = Location.MainPanel)]
    public class RegionContentInfo : RegionContent<RegionContentInfo>
    {
        public override string Name
        {
            get { return "View 2"; }
        }

        public override int Ordinal
        {
            get { return 2; }
        }

        protected override void InitI()
        {}

        protected override FrameworkElement CreateI()
        {
            return new MainPage();
        }
    }
}
