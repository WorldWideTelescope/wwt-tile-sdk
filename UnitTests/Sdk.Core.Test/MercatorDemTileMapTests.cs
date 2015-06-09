//-----------------------------------------------------------------------
// <copyright file="MercatorDemTileMapTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    [TestClass]
    public class MercatorDemTileMapTests : TestBase
    {
        [TestMethod]
        public void MercatorDemTileMapTest()
        {
            MockClasses.MockElevationMap map = new MockClasses.MockElevationMap();

            IDemTileSerializer serializer = new MockClasses.MockDemTileSerializer();
            MercatorDemTileCreator tc = new MercatorDemTileCreator(map, serializer);
            Assert.AreEqual(tc.ProjectionType, ProjectionTypes.Mercator);
            tc.Create(0, 0, 0);
        }
    }
}
