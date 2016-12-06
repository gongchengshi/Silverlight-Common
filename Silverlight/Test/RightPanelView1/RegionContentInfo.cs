using System.Windows;
using Regions;
using SEL.Silverlight.RegionPartitioner;

namespace RightPanelView1
{
    [TargetRegion(Location = Location.RightPanel)]
    public class RegionContentInfo : RegionContent<RegionContentInfo>
    {
        public override string Name
        {
            get { return "RightPanelView 1"; }
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
