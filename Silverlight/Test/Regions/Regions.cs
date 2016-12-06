using System;
using System.ComponentModel.Composition;
using SEL.Silverlight.RegionPartitioner;

namespace Regions
{
    public enum Location
    {
        MainPanel,
        RightPanel
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TargetRegionAttribute : ExportAttribute
    {
        public TargetRegionAttribute()
            : base(typeof(IRegionContent))
        {}

        public Location Location { get; set; }
    }

    public interface IRegionContentMetaData
    {
        Location Location { get; }
    }
}
