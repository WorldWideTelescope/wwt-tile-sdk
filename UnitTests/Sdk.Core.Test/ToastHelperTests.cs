//-----------------------------------------------------------------------
// <copyright file="ToastTileMapTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    [TestClass]
    public class ToastHelperTests : TestBase
    {
        [TestMethod]
        public void ToastTileCoordinatesTest()
        {
            try
            {
                var expectedTiles = new List<TestTile>(16);
                expectedTiles.Add(new TestTile(0, 0, 0, 0, 0));
                expectedTiles.Add(new TestTile(1, 1, 1, 0, 1));
                expectedTiles.Add(new TestTile(2, 2, 2, 1, 2));
                expectedTiles.Add(new TestTile(3, 4, 5, 3, 4));
                expectedTiles.Add(new TestTile(4, 9, 10, 6, 8));
                expectedTiles.Add(new TestTile(5, 19, 21, 12, 16));
                expectedTiles.Add(new TestTile(6, 39, 43, 24, 32));
                expectedTiles.Add(new TestTile(7, 78, 86, 49, 64));
                expectedTiles.Add(new TestTile(8, 156, 173, 99, 128));
                expectedTiles.Add(new TestTile(9, 313, 346, 198, 256));
                expectedTiles.Add(new TestTile(10, 627, 692, 398, 512));
                expectedTiles.Add(new TestTile(11, 1255, 1384, 796, 1024));
                expectedTiles.Add(new TestTile(12, 2511, 2768, 1593, 2048));
                expectedTiles.Add(new TestTile(13, 5023, 5536, 3187, 4096));
                expectedTiles.Add(new TestTile(14, 10047, 11072, 6375, 8192));
                expectedTiles.Add(new TestTile(15, 20094, 22145, 12750, 16384));

                int maxLevel = 15;
                var boundary = new Boundary(-136.40173 + 180, 58.35286, -135.91382 + 180, 58.75443);
                var tiles = ToastHelper.ComputeTileCoordinates(boundary, maxLevel);
                Assert.AreEqual(tiles.Count, maxLevel + 1);
                for (int index = 0; index <= maxLevel; index++)
                {
                    Assert.IsTrue(tiles.ContainsKey(index));
                    Assert.IsNotNull(tiles[index]);
                    Assert.IsTrue(tiles[index].Count > 0);

                    var tilesAtBase = tiles[index];
                    Assert.AreEqual(tilesAtBase.Min(item => item.X), expectedTiles[index].XMin);
                    Assert.AreEqual(tilesAtBase.Max(item => item.X), expectedTiles[index].XMax);
                    Assert.AreEqual(tilesAtBase.Min(item => item.Y), expectedTiles[index].YMin);
                    Assert.AreEqual(tilesAtBase.Max(item => item.Y), expectedTiles[index].YMax);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        internal struct TestTile
        {
            public int Level;
            public int XMin;
            public int XMax;
            public int YMin;
            public int YMax;

            public TestTile(int level, int xmin, int xmax, int ymin, int ymax)
            {
                this.Level = level;
                this.XMin = xmin;
                this.XMax = xmax;
                this.YMin = ymin;
                this.YMax = ymax;
            }
        }
    }
}
