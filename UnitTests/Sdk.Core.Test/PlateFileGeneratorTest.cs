//-----------------------------------------------------------------------
// <copyright file="PlateFileGeneratorTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    /// <summary>
    /// This is a test class for PlateFileCreatorTest and is intended.
    /// to contain all PlateFileCreatorTest Unit Tests.
    /// </summary>
    [TestClass()]
    public class PlateFileGeneratorTest : TestBase
    {
        public const string ImageFileNameTemplate = @"{3}\SamplePyramid\{0}\{1}\L{0}X{1}Y{2}.png";
        public const string TestFileName = "TestCreateFromPyramids.plate";
        public const string TestPlateFileName = "TestPlate.plate";

        private PrivateObject privateObject;

        #region PlateFileGenerator Tests

        /// <summary>
        /// A test for CreateFromPyramids.
        /// </summary>
        [TestMethod()]
        public void CreateFromPyramidsTest()
        {
            int levels = 1;
            PlateFileGenerator target = new PlateFileGenerator(Path.Combine(TestDataPath, TestFileName), levels, ImageFormat.Png);
            target.Format = ImageFormat.Png;
            target.CreateFromImageTile(new MockClasses.MockTileLocator());

            Assert.IsTrue(File.Exists(TestFileName));
        }

        /// <summary>
        /// A test for GetTile.
        /// </summary>
        [TestMethod()]
        public void GetTileTest()
        {
            int levels = 1;
            PlateFile target = new PlateFile(Path.Combine(TestDataPath, TestPlateFileName), levels);
            Bitmap actual;
            using (Bitmap expected = new Bitmap(string.Format(ImageFileNameTemplate, 0, 0, 0, TestDataPath)))
            {
                Stream tileStream = target.GetFileStream(0, 0, 0);
                actual = new Bitmap(tileStream);
                Assert.IsTrue(CompareBitmap(expected, actual));
            }

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    using (Bitmap expected = new Bitmap(string.Format(ImageFileNameTemplate, 1, x, y, TestDataPath)))
                    {
                        Stream tileStream = target.GetFileStream(1, x, y);
                        actual = new Bitmap(tileStream);
                        Assert.IsTrue(CompareBitmap(expected, actual));
                    }
                }
            }
        }

        #endregion

        #region MultiplePlateFileGenerator Tests

        /// <summary>
        /// A test for MultiplePlateFileGenerator Constructor Test.
        /// </summary>
        [TestMethod()]
        public void MultiplePlateFileGeneratorConstructorTest()
        {
            string folderPath = TestDataPath;
            int levels = 5;

            MultiplePlateFileGenerator multiplePlateFileGen = new MultiplePlateFileGenerator(folderPath, levels, ImageFormat.Png);
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
        public void CreateMultiplePlatesFromPyramidsTest()
        {
            int levels = 5;
            string testFolderName = Path.Combine(TestDataPath, "CreateMultiplePlatesFromPyramidsTest", "TestMultiplePlateFolder");

            MockClasses.MockMultipleTileLocator tileLocator = new MockClasses.MockMultipleTileLocator();
            MultiplePlateFileGenerator target = new MultiplePlateFileGenerator(testFolderName, levels, ImageFormat.Png);
            target.CreateFromImageTile(tileLocator);

            WtmlCollection collection = new WtmlCollection("test", "Test.jpeg", "testtexture", levels, ProjectionTypes.Mercator);
            collection.Save(Path.Combine(TestDataPath, "CreateMultiplePlatesFromPyramidsTest", "CreateMultiplePlatesFromPyramidsTest.wtml"));

            Assert.IsTrue(Directory.Exists(testFolderName));
            Assert.IsTrue(File.Exists(Path.Combine(testFolderName, "L0X0Y0.plate")));

            Assert.IsTrue(Directory.Exists(Path.Combine(testFolderName, "2")));
            Assert.AreEqual(16, Directory.GetFiles(Path.Combine(testFolderName, "2"), "*.plate").Length);

            ValidatePyramid(testFolderName, tileLocator, 0, 0, 0);
            ValidatePyramid(testFolderName, tileLocator, 2, 0, 0);
            ValidatePyramid(testFolderName, tileLocator, 3, 1, 1);
            ValidatePyramid(testFolderName, tileLocator, 5, 6, 6);
        }

        #endregion

        /// <summary>
        /// This function is used to compare two bitmaps.
        /// </summary>
        /// <param name="expected">The expected bitmap.</param>
        /// <param name="actual">The actual bitmap.</param>
        /// <returns>True if the bitmaps are same;Otherwise false.</returns>
        private static bool CompareBitmap(Bitmap expected, Bitmap actual)
        {
            if (expected != null && actual != null && expected.Width == actual.Width && expected.Height == actual.Height)
            {
                for (int x = 0; x < expected.Width; x++)
                {
                    for (int y = 0; y < expected.Height; y++)
                    {
                        if (expected.GetPixel(x, y) != actual.GetPixel(x, y))
                        {
                            return false;
                        }
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
        private static void ValidatePyramid(string testFolderName, MockClasses.MockMultipleTileLocator tileLocator, int level, int x, int y)
        {
            Stream tileStream = PlateFileHelper.GetTileFromMultiplePlates(level, x, y, testFolderName);
            Bitmap actual = new Bitmap(tileStream);
            Bitmap expected = tileLocator.Deserialize(level, x, y);
            Assert.IsTrue(CompareBitmap(expected, actual));
        }
    }
}
