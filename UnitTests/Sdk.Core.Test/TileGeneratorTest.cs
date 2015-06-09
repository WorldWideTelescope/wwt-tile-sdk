//-----------------------------------------------------------------------
// <copyright file="TileGeneratorTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    /// <summary>
    /// This is a test class for TileGeneratorTest and is intended
    /// to contain all TileGeneratorTest Unit Tests
    /// </summary>
    [TestClass()]
    public class TileGeneratorTest
    {
        /// <summary>
        /// A test for Generate pyramids with specific X and Y tile values.
        /// </summary>
        [TestMethod()]
        public void GeneratePyramidsDepthTest()
        {
            MockClasses.MockTileCreatorForTileGenerator creator = new MockClasses.MockTileCreatorForTileGenerator();
            TileGenerator target = new TileGenerator(creator);
            int level = 5;
            int tileXStart = 5;
            int tileXEnd = 28;
            int tileYStart = 10;
            int tileYEnd = 28;
            int depth = 3;

            target.Generate(level, tileXStart, tileXEnd, tileYStart, tileYEnd, depth);

            Assert.AreEqual(3, creator.TileCreated.Count);
            Assert.AreEqual(456, creator.TileCreated[5]);
            Assert.AreEqual(130, creator.TileCreated[4]);
            Assert.AreEqual(42, creator.TileCreated[3]);
        }

        /// <summary>
        /// A test for Generate pyramid.
        /// </summary>
        [TestMethod()]
        public void GeneratePyramidsTest()
        {
            MockClasses.MockTileCreatorForTileGenerator creator = new MockClasses.MockTileCreatorForTileGenerator();
            TileGenerator target = new TileGenerator(creator);
            int level = 5;
            target.Generate(level);

            Assert.AreEqual(6, creator.TileCreated.Count);
            Assert.AreEqual(1024, creator.TileCreated[5]);
            Assert.AreEqual(256, creator.TileCreated[4]);
            Assert.AreEqual(64, creator.TileCreated[3]);
            Assert.AreEqual(16, creator.TileCreated[2]);
            Assert.AreEqual(4, creator.TileCreated[1]);
            Assert.AreEqual(1, creator.TileCreated[0]);
        }

        /// <summary>
        /// A test for Generate pyramids with specific boundary.
        /// </summary>
        [TestMethod()]
        public void GeneratePyramidsWithBoundaryTest()
        {
            MockClasses.MockTileCreatorForTileGenerator creator = new MockClasses.MockTileCreatorForTileGenerator();
            TileGenerator target = new TileGenerator(creator);
            int level = 5;
            var imageBoundary = new Boundary(-180, -90, 180, 90);

            target.Generate(level, imageBoundary);

            Assert.AreEqual(6, creator.TileCreated.Count);
            Assert.AreEqual(574, creator.TileCreated[5]);
            Assert.AreEqual(158, creator.TileCreated[4]);
            Assert.AreEqual(46, creator.TileCreated[3]);
            Assert.AreEqual(14, creator.TileCreated[2]);
            Assert.AreEqual(4, creator.TileCreated[1]);
            Assert.AreEqual(1, creator.TileCreated[0]);
        }

        /// <summary>
        /// A test for Generate pyramids with specific boundary.
        /// </summary>
        [TestMethod()]
        public void GeneratePyramidsWithBoundaryMercatorTest()
        {
            MockClasses.MockTileCreatorForMercatorTileGenerator creator = new MockClasses.MockTileCreatorForMercatorTileGenerator();
            TileGenerator target = new TileGenerator(creator);
            int level = 5;
            var imageBoundary = new Boundary(-180, -90, 180, 90);

            target.Generate(level, imageBoundary);

            Assert.AreEqual(6, creator.TileCreated.Count);
            Assert.AreEqual(1024, creator.TileCreated[5]);
            Assert.AreEqual(256, creator.TileCreated[4]);
            Assert.AreEqual(64, creator.TileCreated[3]);
            Assert.AreEqual(16, creator.TileCreated[2]);
            Assert.AreEqual(4, creator.TileCreated[1]);
            Assert.AreEqual(1, creator.TileCreated[0]);
        }
    }
}
