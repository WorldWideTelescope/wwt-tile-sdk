//-----------------------------------------------------------------------
// <copyright file="MultiTileMapTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    [TestClass]
    public class MultiTileMapTests : TestBase
    {
        [TestMethod]
        public void MultiTileMapTest()
        {
            IImageTileSerializer serializer = new MockClasses.MockTileSerializer();

            MockClasses.MockColorMap map = new MockClasses.MockColorMap();
            map.ExpectedLongitudes = Enumerable.Range(0, Constants.TileSize * Constants.TileSize).Select(i => double.NaN).ToArray();
            map.ExpectedLatitudes = Enumerable.Range(0, Constants.TileSize * Constants.TileSize).Select(i => double.NaN).ToArray();

            // Expected value at pixel (0,0)
            map.ExpectedLongitudes[0] = -179.296875;
            map.ExpectedLatitudes[0] = 84.9901001802348;

            // Expected value at pixel (100,100)
            map.ExpectedLongitudes[100 + 100 * 256] = -38.671875;
            map.ExpectedLatitudes[100 + 100 * 256] = 36.031331776331868;
            MercatorTileCreator mercator = new MercatorTileCreator(map, serializer);

            // Toast map
            MockClasses.MockColorMap toastMap = new MockClasses.MockColorMap();
            toastMap.ExpectedLongitudes = Enumerable.Range(0, Constants.TileSize * Constants.TileSize).Select(i => double.NaN).ToArray();
            toastMap.ExpectedLatitudes = Enumerable.Range(0, Constants.TileSize * Constants.TileSize).Select(i => double.NaN).ToArray();

            // Expected value at pixel (0,0)
            toastMap.ExpectedLongitudes[0] = -5.44921875;
            toastMap.ExpectedLatitudes[0] = -89.309545852399211;

            // Expected value at pixel (100,100)
            toastMap.ExpectedLongitudes[100 + 100 * 256] = -45.0;
            toastMap.ExpectedLatitudes[100 + 100 * 256] = 59.83399091605358;
            ToastTileCreator toast = new ToastTileCreator(toastMap, serializer);

            MultiTileCreator multiCreator = new MultiTileCreator(new Collection<ITileCreator>() { mercator, toast }, ProjectionTypes.Mercator);
            multiCreator.Create(0, 0, 0);
            multiCreator.CreateParent(0, 0, 0);

            // Validate Mercator file
            Assert.AreEqual(ProjectionTypes.Mercator, multiCreator.ProjectionType);
        }
    }
}
