//-----------------------------------------------------------------------
// <copyright file="ToastTileMapTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    [TestClass]
    public class ToastTileMapTests : TestBase
    {
        [TestMethod]
        public void ToastMapTest()
        {
            MockColorMap map = new MockColorMap();
            map.ExpectedLongitudes = Enumerable.Range(0, Constants.TileSize * Constants.TileSize).Select(i => double.NaN).ToArray();
            map.ExpectedLatitudes = Enumerable.Range(0, Constants.TileSize * Constants.TileSize).Select(i => double.NaN).ToArray();

            // Expected value at pixel (0,0)
            map.ExpectedLongitudes[0] = -5.44921875;
            map.ExpectedLatitudes[0] = -89.309545852399211;

            // Expected value at pixel (100,100)
            map.ExpectedLongitudes[100 + 100 * 256] = -45.0;
            map.ExpectedLatitudes[100 + 100 * 256] = 59.83399091605358;

            IImageTileSerializer serializer = new MockTileSerializer();

            ToastTileCreator tc = new ToastTileCreator(map, serializer);
            Assert.AreEqual(tc.ProjectionType, ProjectionTypes.Toast);

            tc.Create(0, 0, 0);
        }

        internal class MockColorMap : IColorMap
        {
            public double[] ExpectedLongitudes { get; set; }

            public double[] ExpectedLatitudes { get; set; }

            public int Count { get; set; }

            public Color GetColor(double longitude, double latitude)
            {
                double lng = ExpectedLongitudes[Count];
                if (!double.IsNaN(lng))
                {
                    Assert.IsTrue(Math.Abs(longitude - lng) < 0.0000001);
                }
                double lat = ExpectedLatitudes[Count];
                if (!double.IsNaN(lat))
                {
                    Assert.IsTrue(Math.Abs(latitude - lat) < 0.0000001);
                }
                Count++;
                return Color.Transparent;
            }
        }

        internal class MockTileSerializer : IImageTileSerializer
        {
            public void Serialize(Bitmap tile, int level, int tileX, int tileY)
            {
                // No-op. Intentional.
            }

            public Bitmap Deserialize(int level, int tileX, int tileY)
            {
                throw new NotImplementedException();
            }
        }
    }
}
