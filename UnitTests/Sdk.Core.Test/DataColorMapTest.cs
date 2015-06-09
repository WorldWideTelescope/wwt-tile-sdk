//-----------------------------------------------------------------------
// <copyright file="DataColorMapTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    /// <summary>
    /// This is a test class for DataColorMapTest and is intended
    /// to contain all DataColorMapTest Unit Tests
    /// </summary>
    [TestClass()]
    public class DataColorMapTest : TestBase
    {
        /// <summary>
        /// A test for DataColorMap Constructor
        /// </summary>
        [TestMethod()]
        public void GetColorTest()
        {
            string colorMapFile = Path.Combine(TestDataPath, "ColorMap.png");
            IProjectionGridMap projectionGridMap = new MockClasses.MockProjectionGridMap();
            ColorMapOrientation orientation = ColorMapOrientation.Vertical;
            double minimumValue = double.MinValue;
            double maximumValue = double.MaxValue;
            DataColorMap target = new DataColorMap(colorMapFile, projectionGridMap, orientation, minimumValue, maximumValue);
            Color expectedColor = Color.FromArgb(255, 164, 169, 230);
            Color actual = target.GetColor(200, 400);
            Assert.AreEqual(expectedColor, actual);
        }

        [TestMethod]
        public void GetNaNColorTest()
        {
            try
            {
                string colorMapFile = Path.Combine(TestDataPath, "ColorMap.png");
                IProjectionGridMap projectionGridMap = new MockClasses.MockProjectionGridMap();
                ColorMapOrientation orientation = ColorMapOrientation.Vertical;
                double minimumValue = double.MinValue;
                double maximumValue = double.MaxValue;
                DataColorMap target = new DataColorMap(colorMapFile, projectionGridMap, orientation, minimumValue, maximumValue);
                Color color = target.GetColor(double.NaN, double.NaN);
                Assert.AreEqual(color, Color.Transparent);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
