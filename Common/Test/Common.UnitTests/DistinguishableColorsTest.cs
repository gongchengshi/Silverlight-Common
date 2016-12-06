using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SEL.Common.UnitTests
{
    [TestClass]
    public class DistinguishableColorsTest
    {
        [TestMethod]
        public void UseDarkBackground()
        {
            var target = new DistinguishableColors();
            Assert.IsTrue(target.UseDarkBackground);
            var colors = target.AllColors();
        }

        [TestMethod]
        public void DontUseDarkBackground()
        {
            var target = new DistinguishableColors(false);
            var colors = target.AllColors();
        }
    }
}
