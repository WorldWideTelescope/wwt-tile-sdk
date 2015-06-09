//-----------------------------------------------------------------------
// <copyright file="EquirectangularImageTestCases.cs" company="Microsoft Corporation">
// Copyright  Microsoft Corporation 2010. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Research.Wwt.Sdk.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.SDK.Core.TestAutomation
{
    /// <summary>
    /// Test Automation code for SDK Core test cases.
    /// </summary>
    [TestClass]
    public class EquirectangularImageTestCases
    {
        private Utility utilityObj = new Utility(@"SDKCoreTestsConfig.xml");
        private string fileTemplate = @"Pyramid\{0}\{1}\DL{0}X{1}Y{2}.dem";
        private string expectedLevel0 = @"Pyramid\0\0\DL0X0Y0.dem";
        private string expectedLevel3 = @"Pyramid\3\2\DL3X2Y1.dem";
        private string expectedLevel5 = @"Pyramid\5\4\DL5X4Y3.dem";
        private string expectedLevel7 = @"Pyramid\7\6\DL7X6Y5.dem";
        private string expectedLevel15 = @"Pyramid\15\14\DL15X14Y13.dem";
        private string imageTemplate = @"Pyramid\{0}\{1}\L{0}X{1}Y{2}.{3}";

        #region Enums

        /// <summary>
        /// Equirectangular Image class Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        public enum Params
        {
            FileName,
            BitMap,
            Stream,
            BitMapWithPams,
            StreamWithPams,
            StringException,
            BitMapException,
            StreamException,
            InvalidFileError
        }

        #endregion Enums

        /// <summary>
        /// Validate the DEM Folder structure for the level 0. 
        /// </summary>
        [TestMethod]
        public void ValidateDEMFilesForLevel0()
        {
            ValidateDEMFilesForDifferentLevel(expectedLevel0, 0, 0, 0);
        }

        /// <summary>
        /// Validate the DEM Folder structure for the level 3. 
        /// </summary>
        [TestMethod]
        public void ValidateDEMFilesForLevel3()
        {
            ValidateDEMFilesForDifferentLevel(expectedLevel3, 3, 2, 1);
        }

        /// <summary>
        /// Validate the DEM Tile Deserializer Folder structure for the level 3. 
        /// </summary>
        [TestMethod]
        public void ValidateDEMTileDeserializerForLevel3()
        {
            ValidateDeserializeDEMDifferentLevel(expectedLevel3, 3, 2, 1);
        }

        /// <summary>
        /// Validate the DEM Folder structure for the level 5. 
        /// </summary>
        [TestMethod]
        public void ValidateDEMFilesForLevel5()
        {
            ValidateDEMFilesForDifferentLevel(expectedLevel5, 5, 4, 3);
        }

        /// <summary>
        /// Validate the DEM Folder structure for the level 7 .
        /// </summary>
        [TestMethod]
        public void ValidateDEMFilesForLevel7()
        {
            ValidateDEMFilesForDifferentLevel(expectedLevel7, 7, 6, 5);
        }

        /// <summary>
        /// Validate the DEM Folder structure for the level 17.
        /// </summary>
        [TestMethod]
        public void ValidateDEMFilesForLevel15()
        {
            ValidateDEMFilesForDifferentLevel(expectedLevel15, 15, 14, 13);
        }

        /// <summary>
        /// Invalidate the DEM Serializer. 
        /// </summary>
        [TestMethod]
        public void InvalidateDEMSerializerTest()
        {
            // Get Values from XML File
            string expectedError = utilityObj.XmlUtil.GetTextValue(Constants.DemSerializerTestNode, Constants.ExpectedErrorNode);
            DemTileSerializer demObject = null;
            try
            {
                demObject = new DemTileSerializer(null);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                string message = ex.Message;
                Assert.IsNull(demObject);
                Assert.AreEqual(expectedError, message.Replace("\r", string.Empty).Replace("\n", string.Empty));
            }
        }

        /// <summary>
        /// Invalidate the ImageTileSerializer for null format. 
        /// </summary>
        [TestMethod]
        public void InvalidateImageTileSerializerForNullImageFormat()
        {
            // Get Values from XML File
            string expectedError = utilityObj.XmlUtil.GetTextValue(Constants.ImageTileSerializerNode, Constants.ExpectedErrorNode);

            ImageTileSerializer imageTileSerializer = null;
            try
            {
                imageTileSerializer = new ImageTileSerializer("Filename", null);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                string message = ex.Message;
                Assert.IsNull(imageTileSerializer);
                Assert.AreEqual(expectedError, message.Replace("\r", string.Empty).Replace("\n", string.Empty));
            }
        }

        /// <summary>
        /// Invalidate the ImageTileSerializer for null destination. 
        /// </summary>
        [TestMethod]
        public void InvalidateImageTileSerializerForNullDestination()
        {
            // Get Values from XML File
            string expectedError = utilityObj.XmlUtil.GetTextValue(Constants.DemSerializerTestNode, Constants.ExpectedErrorNode);

            ImageTileSerializer imageTileSerializer = null;
            try
            {
                imageTileSerializer = new ImageTileSerializer(null, ImageFormat.Png);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                string message = ex.Message;
                Assert.IsNull(imageTileSerializer);
                Assert.AreEqual(expectedError, message.Replace("\r", string.Empty).Replace("\n", string.Empty));
            }
        }

        /// <summary>
        /// Validate Tile ctor.
        /// </summary>
        public void ValidateTileCtor()
        {
            // Create a tile constructor
            Tile tile = new Tile(3, 4);

            // Validate tiles created
            Assert.AreEqual(3, tile.X);
            Assert.AreEqual(4, tile.Y);
        }

        /// <summary>
        /// Validate Image serilaize for the level 0. 
        /// </summary>
        [TestMethod]
        public void ValidateImageFilesForLevel0()
        {
            ValidateImageFilesForDifferentLevel(Constants.EquiRectangularWorldImageNode, imageTemplate, 0, 0, 0, ImageFormat.Png);
        }

        /// <summary>
        /// Validate Image serilaize for the level 0. 
        /// </summary>
        [TestMethod]
        public void ValidateImageDeserializeFilesForLevel0()
        {
            ValidateDeserializeImageFilesForDifferentLevel(Constants.EquiRectangularWorldImageNode, imageTemplate, 0, 0, 0, ImageFormat.Png);
        }

        /// <summary>
        /// Invalidate the DEM Serializer for NUll value. 
        /// </summary>
        [TestMethod]
        public void InvalidateMercatorSerializerTestForNull()
        {
            // Get Values from XML File
            string expectedError = utilityObj.XmlUtil.GetTextValue(Constants.MercDemSerializerTestNode, Constants.ExpectedErrorNode);
            ImageTileSerializer imageTileSerializer = new ImageTileSerializer(Path.Combine(Environment.CurrentDirectory, imageTemplate), ImageFormat.Png);

            MercatorTileCreator merc = null;
            try
            {
                merc = new MercatorTileCreator(null, imageTileSerializer);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                string message = ex.Message;
                Assert.IsNull(merc);
                Assert.AreEqual(expectedError, message.Replace("\r", string.Empty).Replace("\n", string.Empty));
            }
        }
        
        /// <summary>
        /// Invalidate the DEM Toast Serializer for NUll value. 
        /// </summary>
        [TestMethod]
        public void InvalidateToastCreatorUsingNullValue()
        {
            // Get Values from XML File
            string expectedError = utilityObj.XmlUtil.GetTextValue(Constants.MercDemSerializerTestNode, Constants.ExpectedErrorNode);
            ImageTileSerializer imageTileSerializer = new ImageTileSerializer(Path.Combine(Environment.CurrentDirectory, imageTemplate), ImageFormat.Png);

            ToastTileCreator merc = null;
            try
            {
                merc = new ToastTileCreator(null, imageTileSerializer);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                string message = ex.Message;
                Assert.IsNull(merc);
                Assert.AreEqual(expectedError, message.Replace("\r", string.Empty).Replace("\n", string.Empty));
            }
        }
        
        /// <summary>
        /// Invalidate the DEM Serializer for NUll value. 
        /// </summary>
        [TestMethod]
        public void InvalidateDEMSMercatorSerializerTestForNull()
        {
            // Get Values from XML File
            string expectedError = utilityObj.XmlUtil.GetTextValue(Constants.MercDemSerializerTestNode, Constants.ExpectedErrorNode);
            string destinationPath = Path.Combine(Environment.CurrentDirectory, fileTemplate);
            DemTileSerializer demTileSerializer = new DemTileSerializer(destinationPath);
            MercatorDemTileCreator merc = null;
            try
            {
                merc = new MercatorDemTileCreator(null, demTileSerializer);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                string message = ex.Message;
                Assert.IsNull(merc);
                Assert.AreEqual(expectedError, message.Replace("\r", string.Empty).Replace("\n", string.Empty));
            }
        }

        /// <summary>
        /// Invalidate the DEM Toast Creator Serializer for NUll value. 
        /// </summary>
        [TestMethod]
        public void InvalidateDEMToastCreatorUsingNullElevation()
        {
            // Get Values from XML File
            string expectedError = utilityObj.XmlUtil.GetTextValue(Constants.MercDemSerializerTestNode, Constants.ExpectedErrorNode);
            string destinationPath = Path.Combine(Environment.CurrentDirectory, fileTemplate);
            DemTileSerializer demTileSerializer = new DemTileSerializer(destinationPath);
            ToastDemTileCreator merc = null;
            try
            {
                merc = new ToastDemTileCreator(null, demTileSerializer);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                string message = ex.Message;
                Assert.IsNull(merc);
                Assert.AreEqual(expectedError, message.Replace("\r", string.Empty).Replace("\n", string.Empty));
            }
        }

        /// <summary>
        /// Invalidate Multitil creator.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidateMultitileCreatorForNullValue()
        {
            MultiTileCreator multiCreator = new MultiTileCreator(null, ProjectionTypes.Mercator);
            Assert.IsNull(multiCreator);
        }

        /// <summary>
        /// Validate ToastHelper class. 
        /// </summary>
        [TestMethod]
        public void ValidateToastHelper()
        {
            int maxLevel = 15;
            var boundary = new Boundary(-136.40173 + 180, 58.35286, -135.91382 + 180, 58.75443);
            var tiles = ToastHelper.ComputeTileCoordinates(boundary, maxLevel);
            Assert.AreEqual(tiles.Count, maxLevel + 1);
        }

        /// <summary>
        /// Validate WTML collection save Test.
        /// </summary>
        [TestMethod]
        public void ValidateWtmlMercatorCollectionSet()
        {
            WtmlCollection wtmlCollecn = new WtmlCollection("Temp", "tempName.jpeg", Environment.CurrentDirectory, 7, ProjectionTypes.Mercator);
            Assert.IsNotNull(wtmlCollecn);

            // Save WTMl file
            string filePath = @"ValidTemp.wtml";
            wtmlCollecn.Save(filePath);
            Assert.IsTrue(File.Exists(filePath));
        }

        /// <summary>
        /// Validate WTML cToast ollection save Test.
        /// </summary>
        [TestMethod]
        public void ValidateWtmlToastCollectionSet()
        {
            WtmlCollection wtmlCollecn = new WtmlCollection("Temp", "tempName.jpeg", Environment.CurrentDirectory, 7, ProjectionTypes.Toast);
            Assert.IsNotNull(wtmlCollecn);

            // Save WTMl file
            string filePath = @"ValidTemp.wtml";
            wtmlCollecn.Save(filePath);
            Assert.IsTrue(File.Exists(filePath));
        }

        /// <summary>
        /// Validate Get Texture for wtml files.
        /// </summary>
        [TestMethod]
        public void ValidateGetTextureForWtmlFile()
        {
            string path = WtmlCollection.GetWtmlTextureTilePath(imageTemplate, "png");
            Assert.AreEqual(@"Pyramid\{1}\{2}\L{1}X{2}Y{3}.png", path);
        }

        #region Helper Methods

        /// <summary>
        /// Validate the DEM files name for different test cases.
        /// </summary>
        /// <param name="expectedFileName">Expected file name.</param>
        /// <param name="level">Levels in the folder structure.</param>
        /// <param name="tileXValue">Tile X value.</param>
        /// <param name="tileYValue">Tile Y value.</param>
        public void ValidateDEMFilesForDifferentLevel(string expectedFileName, int level, int tileXValue, int tileYValue)
        {
            string destinationPath = Path.Combine(Environment.CurrentDirectory, fileTemplate);
            var demTileSerializer = new DemTileSerializer(destinationPath);

            // Validate destination path 
            Assert.AreEqual(demTileSerializer.DestinationPath, destinationPath);

            short[] values = new short[1089];
            Random random = new Random();
            for (int index = 0; index < 1089; index++)
            {
                values[index] = Convert.ToInt16(random.Next(100));
            }

            demTileSerializer.Serialize(values, level, tileXValue, tileYValue);

            // Validate the file structure created and the files
            string actualPath = demTileSerializer.GetFileName(level, tileXValue, tileYValue);
            Assert.AreEqual(actualPath, Path.Combine(Environment.CurrentDirectory, expectedFileName));
            Assert.IsTrue(File.Exists(actualPath));

            // Delete the file.
            File.Delete(actualPath);
        }

        /// <summary>
        /// Validate the DEM files name for different test cases.
        /// </summary>
        /// <param name="expectedFileName">Expected file name.</param>
        /// <param name="level">Levels in the folder structure.</param>
        /// <param name="tileXValue">Tile X value.</param>
        /// <param name="tileYValue">Tile Y value.</param>
        public void ValidateDeserializeDEMDifferentLevel(string expectedFileName, int level, int tileXValue, int tileYValue)
        {
            string destinationPath = Path.Combine(Environment.CurrentDirectory, fileTemplate);
            var demTileSerializer = new DemTileSerializer(destinationPath);

            short[] values = new short[1089];
            Random random = new Random();
            for (int index = 0; index < 1089; index++)
            {
                values[index] = Convert.ToInt16(random.Next(100));
            }

            demTileSerializer.Serialize(values, level, tileXValue, tileYValue);

            // Validate the file structure created and the files
            string actualPath = demTileSerializer.GetFileName(level, tileXValue, tileYValue);
            Assert.AreEqual(actualPath, Path.Combine(Environment.CurrentDirectory, expectedFileName));
            Assert.IsTrue(File.Exists(actualPath));

            var newBytes = demTileSerializer.Deserialize(level, tileXValue, tileYValue);

            // Assert that the deserialized bitmap is not null and holds original values.
            Assert.IsNotNull(newBytes);
            for (int i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(values[i], newBytes[i]);
            }

            // Delete the file.
            File.Delete(actualPath);
        }

        /// <summary>
        /// Validate the Image Serialize  for different test cases.
        /// </summary>
        /// <param name="nodeName">Differrent node name for different test cases.</param>
        /// <param name="expectedFileName">Expected file name.</param>
        /// <param name="level">Levels in the folder structure.</param>
        /// <param name="tileXValue">Tile X value.</param>
        /// <param name="tileYValue">Tile Y value.</param>
        /// <param name="format">Image format.</param>        
        public void ValidateImageFilesForDifferentLevel(string nodeName, string expectedFileName, int level, int tileXValue, int tileYValue, ImageFormat format)
        {
            // Get Values from XML File
            string filePath = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.InputFilePath);

            var imageTileSerializer = new ImageTileSerializer(Path.Combine(Environment.CurrentDirectory, expectedFileName), format);
            Assert.IsNotNull(imageTileSerializer.DestinationPath);

            using (Bitmap map = new Bitmap(filePath))
            {
                imageTileSerializer.Serialize(map, level, tileXValue, tileYValue);

                // Assert that the file exists.
                string path = Path.Combine(Environment.CurrentDirectory, imageTileSerializer.GetFileName(level, tileXValue, tileYValue));
                Assert.IsTrue(File.Exists(path));

                // Delete the file.
                File.Delete(path);
            }
        }

        /// <summary>
        /// Validate the Image DeSerialize  for different test cases.
        /// </summary>
        /// <param name="nodeName">Differrent node name for different test cases.</param>
        /// <param name="expectedFileName">Expected file name.</param>
        /// <param name="level">Levels in the folder structure.</param>
        /// <param name="tileXValue">Tile X value.</param>
        /// <param name="tileYValue">Tile Y value.</param>
        /// <param name="format">Image format.</param>        
        public void ValidateDeserializeImageFilesForDifferentLevel(string nodeName, string expectedFileName, int level, int tileXValue, int tileYValue, ImageFormat format)
        {
            // Get Values from XML File
            string filePath = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.InputFilePath);

            var imageTileSerializer = new ImageTileSerializer(expectedFileName, ImageFormat.Png);
            using (Bitmap bitmap = new Bitmap(filePath))
            {
                // Serialize the file.
                imageTileSerializer.Serialize(bitmap, level, tileXValue, tileYValue);

                string path = Path.Combine(Environment.CurrentDirectory, imageTileSerializer.GetFileName(level, tileXValue, tileYValue));
                Assert.IsTrue(File.Exists(path));

                using (Bitmap newBitmap = imageTileSerializer.Deserialize(level, tileXValue, tileYValue) as Bitmap)
                {
                    Assert.IsNotNull(newBitmap);
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            Assert.AreEqual(bitmap.GetPixel(x, y), newBitmap.GetPixel(x, y));
                        }
                    }
                }

                // Delete the file.
                File.Delete(path);
            }
        }
        #endregion Helper Methods
    }
}
