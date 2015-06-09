//-----------------------------------------------------------------------
// <copyright file="EquirectangularGridMapTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    /// <summary>
    /// This is a test class for EquirectangularGridMapTest and is intended
    /// to contain all EquirectangularGridMapTest Unit Tests
    /// </summary>
    [TestClass()]
    public class EquirectangularGridMapTest
    {
        /// <summary>
        /// A test for GetValue
        /// </summary>
        [TestMethod()]
        public void GetValueTest()
        {
            IGrid inputGrid = new MockClasses.MockGrid();
            Boundary boundary = new Boundary(-180, -90, 180, 90);
            EquirectangularGridMap target = new EquirectangularGridMap(inputGrid, boundary);
            Assert.AreEqual(1.2777777777777777, target.GetValue(-80, -90));
            Assert.AreEqual(0, target.GetXIndex(-80));
            Assert.AreEqual(1, target.GetYIndex(-90));
            Assert.AreEqual(double.NaN, target.GetValue(-190, -90));
        }
    }
}
