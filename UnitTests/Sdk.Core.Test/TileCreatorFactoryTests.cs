//-----------------------------------------------------------------------
// <copyright file="TileCreatorFactoryTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    [TestClass]
    public class TileCreatorFactoryTests : TestBase
    {
        [TestMethod]
        public void CreateImageTileCreatorTest()
        {
            MockClasses.MockColorMap map = new MockClasses.MockColorMap();

            ITileCreator imageTileCreator = TileCreatorFactory.CreateImageTileCreator(map, ProjectionTypes.Mercator, Environment.CurrentDirectory);

            // Validate Mercator file
            Assert.AreEqual(ProjectionTypes.Mercator, imageTileCreator.ProjectionType);
        }

        [TestMethod]
        public void CreateImageTileCreatorWithSerializerTest()
        {
            MockClasses.MockColorMap map = new MockClasses.MockColorMap();
            MockClasses.MockTileSerializer mockSerializer = new MockClasses.MockTileSerializer();
            ITileCreator imageTileCreator = TileCreatorFactory.CreateImageTileCreator(map, ProjectionTypes.Toast, mockSerializer);

            // Validate Mercator file
            Assert.AreEqual(ProjectionTypes.Toast, imageTileCreator.ProjectionType);
        }

        [TestMethod]
        public void CreateDemTileCreatorTest()
        {
            MockClasses.MockElevationMap map = new MockClasses.MockElevationMap();
            ITileCreator demTileCreator = TileCreatorFactory.CreateDemTileCreator(map, ProjectionTypes.Mercator, Environment.CurrentDirectory);

            // Validate Mercator file
            Assert.AreEqual(ProjectionTypes.Mercator, demTileCreator.ProjectionType);
        }

        [TestMethod]
        public void CreateDemTileCreatorWithSerializerTest()
        {
            MockClasses.MockElevationMap map = new MockClasses.MockElevationMap();
            MockClasses.MockDemTileSerializer mockSerializer = new MockClasses.MockDemTileSerializer();
            ITileCreator demTileCreator = TileCreatorFactory.CreateDemTileCreator(map, ProjectionTypes.Toast, mockSerializer);

            // Validate Mercator file
            Assert.AreEqual(ProjectionTypes.Toast, demTileCreator.ProjectionType);
        }
    }
}
