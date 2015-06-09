//-----------------------------------------------------------------------
// <copyright file="ImageGridTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    /// <summary>
    /// This is a test class for ImageGridTest and is intended
    /// to contain all ImageGridTest Unit Tests
    /// </summary>
    [TestClass()]
    public class ImageGridTest : TestBase
    {
        /// <summary>
        /// A test for ImageGrid Constructor
        /// </summary>
        [TestMethod()]
        public void ImageGridConstructorTest()
        {
            string path = Path.Combine(TestDataPath, "BlueMarble.png");
            bool isCircular = false;
            ImageGrid target = new ImageGrid(path, isCircular);

            Assert.AreEqual(5400, target.Width);
            Assert.AreEqual(2700, target.Height);
            Assert.AreEqual(-16644328.0, target.GetValue(0.5, 0.5));
            Assert.AreEqual(-16180676.0, target.GetValueAt(2400, 2000));
            Assert.AreEqual(2591, target.GetXIndex(0.48));
            Assert.AreEqual(971, target.GetYIndex(0.36));
        }
    }
}
