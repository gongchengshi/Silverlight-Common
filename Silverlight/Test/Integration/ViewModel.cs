using System.ComponentModel.Composition;
using System.Diagnostics;
using Regions;
using SEL.MEF;

namespace Integration
{
    public class ViewModel
    {
        public ViewModel()
        {
            RegionManager = new RegionManager();
        }

        public RegionManager RegionManager { get; set; }
    }
}
