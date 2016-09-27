using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kea.Test
{
    [TestClass]
    public class TestClass
    {
        [TestMethod]
        public void Interpolation()
        {
            var text = "the value is {{x + 1}}";
            var v = new Dictionary<string, object>();
            v["x"] = 10;

            var ret = StringInterpolation.Interpolate(text, v);
            Assert.AreEqual("the value is 11", ret);
        }

        [TestMethod]
        public void InterpolationFormat()
        {
            var text = "the value is {{x + 1 | 0.00}}$ hello";
            var v = new Dictionary<string, object>();
            v["x"] = 10;

            var ret = StringInterpolation.Interpolate(text, v);
            Assert.AreEqual("the value is 11.00$ hello", ret);
        }
    }
}
