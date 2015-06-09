//-----------------------------------------------------------------------
// <copyright file="MercatorTileMapTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    [TestClass]
    public class MercatorTileMapTests : TestBase
    {
        [TestMethod]
        public void MercatorMapTest()
        {
            MockClasses.MockColorMap map = new MockClasses.MockColorMap();
            map.ExpectedLongitudes = Enumerable.Range(0, Constants.TileSize * Constants.TileSize).Select(i => double.NaN).ToArray();
            map.ExpectedLatitudes = Enumerable.Range(0, Constants.TileSize * Constants.TileSize).Select(i => double.NaN).ToArray();

            // Expected value at pixel (0,0)
            map.ExpectedLongitudes[0] = -179.296875;
            map.ExpectedLatitudes[0] = 84.9901001802348;

            // Expected value at pixel (100,100)
            map.ExpectedLongitudes[100 + 100 * 256] = -38.671875;
            map.ExpectedLatitudes[100 + 100 * 256] = 36.031331776331868;

            IImageTileSerializer serializer = new MockClasses.MockTileSerializer();
            MercatorTileCreator tc = new MercatorTileCreator(map, serializer);
            Assert.AreEqual(tc.ProjectionType, ProjectionTypes.Mercator);

            tc.Create(0, 0, 0);
        }
    }
}
