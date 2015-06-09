//-----------------------------------------------------------------------
// <copyright file="ImageColorMapTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    /// <summary>
    /// This is a test class for ImageColorMapTest and is intended
    /// to contain all ImageColorMapTest Unit Tests
    /// </summary>
    [TestClass()]
    public class ImageColorMapTest
    {
        /// <summary>
        /// A test for GetColor
        /// </summary>
        [TestMethod()]
        public void GetColorTest()
        {
            IProjectionGridMap projectionGridMap = new MockClasses.MockProjectionGridMap();
            ImageColorMap target = new ImageColorMap(projectionGridMap);
            double longitude = 200;
            double latitude = 400;
            Color expected = Color.FromArgb(0, 0, 2, 88);
            Color actual = target.GetColor(longitude, latitude);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetColor for NaN values
        /// </summary>
        [TestMethod()]
        public void GetColorNaNTest()
        {
            IProjectionGridMap projectionGridMap = new MockClasses.MockProjectionGridMap();
            ImageColorMap target = new ImageColorMap(projectionGridMap);
            Color actual = target.GetColor(double.NaN, double.NaN);
            Assert.AreEqual(Color.Transparent, actual);
        }
    }
}
