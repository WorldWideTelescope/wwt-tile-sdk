//-----------------------------------------------------------------------
// <copyright file="TileChopperTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    /// <summary>
    /// This is a test class for TileChopperTest and is intended
    /// to contain all TileChopperTest Unit Tests
    /// </summary>
    [TestClass()]
    public class TileChopperTest : TestBase
    {
        private static TileChopper chopper;
        private static MockClasses.MockTileCreator serializer;

        /// <summary>
        /// Use ClassInitialize to run code before running the first test in the class
        /// </summary>
        /// <param name="testContext">Test Context.</param>
        [ClassInitialize()]
        public static void TileChopperTestsInitialize(TestContext testContext)
        {
            string fileName = Path.Combine(TestDataPath, "Mercator.jpg");
            ProjectionTypes projectionType = ProjectionTypes.Mercator;
            serializer = new MockClasses.MockTileCreator();
            chopper = new TileChopper(fileName, serializer, projectionType);
        }

        /// <summary>
        /// A test for TileChopper Constructor
        /// </summary>
        [TestMethod()]
        public void TileChopperConstructorTest()
        {
            Assert.AreEqual(2, chopper.MaximumLevelsOfDetail);
            Assert.AreEqual(ProjectionTypes.Mercator, chopper.ProjectionType);
        }

        /// <summary>
        /// A test for TileChopper Constructor
        /// </summary>
        [TestMethod()]
        public void TileChopperCreateTest()
        {
            int level = 0;
            int tileX = 0;
            int tileY = 0;

            chopper.Create(level, tileX, tileY);

            Assert.IsTrue(File.Exists(serializer.GetFileName(level, tileX, tileX)));
        }
    }
}
