//-----------------------------------------------------------------------
// <copyright file="TileHelperTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    [TestClass]
    public class TileHelperTests : TestBase
    {
        public const string ImageFileNameTemplate = @"{3}\SamplePyramid\{0}\{1}\L{0}X{1}Y{2}.png";

        public const string TestFileName = "TestPlate.plate";

        [TestMethod]
        public void GetDefaultImageTilePathTemplateTest()
        {
            string actualTemplate = @"Pyramid\{0}\{1}\L{0}X{1}Y{2}.{3}";
            var returnedTemplate = TileHelper.GetDefaultImageTilePathTemplate(string.Empty);
            Assert.AreEqual(actualTemplate, returnedTemplate);
        }

        [TestMethod]
        public void ResizeBitmapTest()
        {
            using (Bitmap bitmap = new Bitmap(Path.Combine(TestDataPath, @"Image.png")))
            {
                using (Bitmap b = TileHelper.ResizeBitmap(bitmap, 100, 100))
                {
                    Assert.AreEqual(100, b.Height);
                    Assert.AreEqual(100, b.Width);
                }
            }
        }

        /// <summary>
        /// A test for GenerateThumbnail
        /// </summary>
        [TestMethod()]
        public void GenerateThumbnailTestValid()
        {
            MockClasses.MockTileLocator tileSerializer = new MockClasses.MockTileLocator();
            int width = 96;
            int height = 45;
            string fileName = Path.Combine(TestDataPath, "thunmbnail.jpeg");
            TileHelper.GenerateThumbnail(tileSerializer.GetFileName(0, 0, 0), width, height, fileName, ImageFormat.Jpeg);
            Assert.IsTrue(File.Exists(fileName));

            using (Image actual = Bitmap.FromFile(fileName))
            {
                Assert.AreEqual(width, actual.Width);
                Assert.AreEqual(height, actual.Height);
            }
        }

        /// <summary>
        /// A test for GenerateThumbnail
        /// </summary>
        [TestMethod()]
        public void GenerateThumbnailTestLargeInput()
        {
            int width = 96;
            int height = 45;
            string inputFileName = Path.Combine(TestDataPath, "BlueMarble.png");
            string fileName = Path.Combine(TestDataPath, "thunmbnailBlueMarble.jpeg");
            TileHelper.GenerateThumbnail(inputFileName, width, height, fileName, ImageFormat.Jpeg);
            Assert.IsTrue(File.Exists(fileName));

            using (Image actual = Bitmap.FromFile(fileName))
            {
                Assert.AreEqual(width, actual.Width);
                Assert.AreEqual(height, actual.Height);
            }
        }

        /// <summary>
        /// A test for GenerateThumbnail
        /// </summary>
        [TestMethod()]
        public void GenerateThumbnailTestSquareInput()
        {
            int width = 96;
            int height = 45;
            string inputFileName = Path.Combine(TestDataPath, "Image.png");
            string fileName = Path.Combine(TestDataPath, "thunmbnailImage.jpeg");
            TileHelper.GenerateThumbnail(inputFileName, width, height, fileName, ImageFormat.Jpeg);
            Assert.IsTrue(File.Exists(fileName));

            using (Image actual = Bitmap.FromFile(fileName))
            {
                Assert.AreEqual(width, actual.Width);
                Assert.AreEqual(height, actual.Height);
            }
        }

        /// <summary>
        /// A test for GenerateThumbnail
        /// </summary>
        [TestMethod()]
        public void GenerateThumbnailTestVerticalInput()
        {
            int width = 96;
            int height = 45;
            string inputFileName = Path.Combine(TestDataPath, "ColorMap.png");
            string fileName = Path.Combine(TestDataPath, "thunmbnailColorMap.jpeg");
            TileHelper.GenerateThumbnail(inputFileName, width, height, fileName, ImageFormat.Jpeg);
            Assert.IsTrue(File.Exists(fileName));

            using (Image actual = Bitmap.FromFile(fileName))
            {
                Assert.AreEqual(width, actual.Width);
                Assert.AreEqual(height, actual.Height);
            }
        }

        /// <summary>
        /// A test for GenerateThumbnail
        /// </summary>
        [TestMethod()]
        public void GenerateThumbnailTestInvalidFileName()
        {
            int width = 96;
            int height = 45;
            string fileName = Path.Combine(TestDataPath, "thunmbnail1.jpeg");
            TileHelper.GenerateThumbnail("InvalidFIleNAme.jpg", width, height, fileName, ImageFormat.Jpeg);
            Assert.IsTrue(!File.Exists(fileName));
        }

        /// <summary>
        /// A test for GenerateThumbnail
        /// </summary>
        [TestMethod()]
        public void GenerateThumbnailTestNoOutputFilename()
        {
            MockClasses.MockTileLocator tileSerializer = new MockClasses.MockTileLocator();
            int width = 96;
            int height = 45;
            string fileName = Path.Combine(TestDataPath, "thunmbnail2.jpeg");
            TileHelper.GenerateThumbnail(tileSerializer.GetFileName(0, 0, 0), width, height, string.Empty, ImageFormat.Jpeg);
            Assert.IsTrue(!File.Exists(fileName));
        }

        /// <summary>
        /// A test for GenerateThumbnail
        /// </summary>
        [TestMethod()]
        public void GenerateThumbnailTestNullFileName()
        {
            int width = 96;
            int height = 45;
            string fileName = Path.Combine(TestDataPath, "thunmbnail3.jpeg");
            TileHelper.GenerateThumbnail(string.Empty, width, height, fileName, ImageFormat.Jpeg);
            Assert.IsTrue(!File.Exists(fileName));
        }
    }
}
