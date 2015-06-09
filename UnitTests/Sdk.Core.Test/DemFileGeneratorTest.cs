//-----------------------------------------------------------------------
// <copyright file="PlateFileGeneratorTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    /// <summary>
    /// This is a test class for PlateFileCreatorTest and is intended.
    /// to contain all PlateFileCreatorTest Unit Tests.
    /// </summary>
    [TestClass()]
    public class DemFileGeneratorTest : TestBase
    {
        public const string ImageFileNameTemplate = @"{3}\SamplePyramid\{0}\{1}\L{0}X{1}Y{2}.png";
        public const string TestDEMFileName = "TestCreateFromDEMPyramids.plate";
        public const string TestDEMPlateFileName = "TestDEMPlate.plate";

        private PrivateObject privateObject;

        #region DemFileGenerator Tests

        /// <summary>
        /// A test for CreateFromDEMPyramidsTest.
        /// </summary>
        [TestMethod()]
        public void CreateFromDemPyramidsTest()
        {
            int levels = 1;
            DemPlateFileGenerator target = new DemPlateFileGenerator(Path.Combine(TestDataPath, TestDEMFileName), levels);
            target.CreateFromDemTile(new MockClasses.MockDemTileLocator());

            Assert.IsTrue(File.Exists(TestDEMFileName));
        }

        /// <summary>
        /// A test for GetTile.
        /// </summary>
        [TestMethod()]
        public void GetDemTileTest()
        {
            int levels = 1;
            PlateFile target = new PlateFile(Path.Combine(TestDataPath, TestDEMPlateFileName), levels);
            MockClasses.MockDemTileLocator locator = new MockClasses.MockDemTileLocator();

            short[] expected = locator.Deserialize(0, 0, 0);
            Stream tileStream = target.GetFileStream(0, 0, 0);
            short[] actual = GetDemTileData(tileStream);

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    tileStream = target.GetFileStream(1, x, y);
                    actual = GetDemTileData(tileStream);
                    expected = locator.Deserialize(1, x, y);
                    Assert.IsTrue(CompareShortArray(expected, actual));
                }
            }
        }

        #endregion

        #region MultipleDemPlateFileGenerator Tests

        /// <summary>
        /// A test for MultiplePlateFileGenerator Constructor Test.
        /// </summary>
        [TestMethod()]
        public void MultipleDemPlateFileGeneratorConstructorTest()
        {
            string folderPath = TestDataPath;
            int levels = 5;

            MultipleDemPlateFileGenerator multiplePlateFileGen = new MultipleDemPlateFileGenerator(folderPath, levels);
            privateObject = new PrivateObject(multiplePlateFileGen);
            MultiplePlateFileDetails field = (MultiplePlateFileDetails)privateObject.GetField("plateFileDetails");

            Assert.AreEqual(4, field.LevelsPerPlate);
            Assert.AreEqual(3, field.MaxOverlappedLevel);
            Assert.AreEqual(2, field.MinOverlappedLevel);
            Assert.AreEqual(2, field.TotalOverlappedLevels);
        }

        /// <summary>
        /// A test for CreateFromPyramids.
        /// </summary>
        [TestMethod()]
        public void CreateMultipleDemPlatesFromPyramidsTest()
        {
            int levels = 5;
            string testFolderName = Path.Combine(TestDataPath, "CreateMultipleDemPlatesFromPyramidsTest", "TestDemMultiplePlateFolder");
            MockClasses.MockMultipleDemTileLocator tileLocator = new MockClasses.MockMultipleDemTileLocator();

            MultipleDemPlateFileGenerator target = new MultipleDemPlateFileGenerator(testFolderName, levels);
            target.CreateFromDemTile(tileLocator);

            WtmlCollection collection = new WtmlCollection("test", "Test.jpeg", "testtexture", levels, ProjectionTypes.Mercator);
            collection.Save(Path.Combine(TestDataPath, "CreateMultipleDemPlatesFromPyramidsTest", "CreateMultipleDemPlatesFromPyramidsTest.wtml"));

            Assert.IsTrue(Directory.Exists(testFolderName));
            Assert.IsTrue(File.Exists(Path.Combine(testFolderName, "DL0X0Y0.plate")));

            Assert.IsTrue(Directory.Exists(Path.Combine(testFolderName, "2")));
            Assert.AreEqual(16, Directory.GetFiles(Path.Combine(testFolderName, "2"), "*.plate").Length);

            ValidatePyramid(testFolderName, tileLocator, 0, 0, 0);
            ValidatePyramid(testFolderName, tileLocator, 2, 0, 0);
            ValidatePyramid(testFolderName, tileLocator, 3, 1, 1);
            ValidatePyramid(testFolderName, tileLocator, 5, 6, 6);
        }

        #endregion

        /// <summary>
        /// This function is used to compare two short[] which contains data.
        /// </summary>
        /// <param name="expected">The expected short[] data.</param>
        /// <param name="actual">The actual short[] data.</param>
        /// <returns>True if the two arrays are same;Otherwise false.</returns>
        private static bool CompareShortArray(short[] expected, short[] actual)
        {
            if (expected != null && actual != null && expected.Length == actual.Length)
            {
                for (int x = 0; x < expected.Length; x++)
                {
                    if (expected[x] != actual[x])
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This function is used to validate the bitmap at specified level, x and y.
        /// </summary>
        /// <param name="testFolderName">Plate file folder.</param>
        /// <param name="tileLocator">Tile Locator.</param>
        /// <param name="level">Level of tile.</param>
        /// <param name="x">X coordinate of the tile.</param>
        /// <param name="y">Y coordinate of the tile.</param>
        private void ValidatePyramid(string testFolderName, MockClasses.MockMultipleDemTileLocator tileLocator, int level, int x, int y)
        {
            Stream tileStream = PlateFileHelper.GetDEMTileFromMultiplePlates(level, x, y, testFolderName);
            short[] expected = tileLocator.Deserialize(level, x, y);
            short[] actual = GetDemTileData(tileStream);
            Assert.IsTrue(CompareShortArray(expected, actual));
        }

        /// <summary>
        /// This function is used to get the DEM tile.
        /// </summary>
        /// <param name="tileStream">
        /// Tile data as stream
        /// </param>
        /// <returns>
        /// DEM tile value as short Array.
        /// </returns>
        private short[] GetDemTileData(Stream tileStream)
        {
            List<short> value = new List<short>();

            using (BinaryReader br = new BinaryReader(tileStream))
            {
                while (tileStream.Length > tileStream.Position)
                {
                    value.Add(br.ReadInt16());
                }
            }

            return value.ToArray();
        }
    }
}
