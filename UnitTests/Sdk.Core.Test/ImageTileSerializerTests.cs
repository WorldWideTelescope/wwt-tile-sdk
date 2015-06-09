//-----------------------------------------------------------------------
// <copyright file="ImageTileSerializerTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    [TestClass]
    public class ImageTileSerializerTests : TestBase
    {
        [TestMethod]
        public void ValidImageSerializeTest()
        {
            try
            {
                string imageFileNameTemplate = @"Pyramid\{0}\{1}\L{0}X{1}Y{2}.{3}";
                var imageTileSerializer = new ImageTileSerializer(Path.Combine(Environment.CurrentDirectory, imageFileNameTemplate), ImageFormat.Png);
                using (Bitmap bitmap = new Bitmap(Path.Combine(TestDataPath, @"Image.png")))
                {
                    // Serialize the file.
                    imageTileSerializer.Serialize(bitmap, 0, 0, 0);

                    // Assert that the file exists.
                    string path = Path.Combine(Environment.CurrentDirectory, imageTileSerializer.GetFileName(0, 0, 0));
                    Assert.IsTrue(File.Exists(path));

                    // Delete the file.
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void ValidImageDeserializeTest()
        {
            try
            {
                string imageFileNameTemplate = @"Pyramid\{0}\{1}\L{0}X{1}Y{2}.{3}";
                var imageTileSerializer = new ImageTileSerializer(Path.Combine(Environment.CurrentDirectory, imageFileNameTemplate), ImageFormat.Png);
                using (Bitmap bitmap = new Bitmap(Path.Combine(TestDataPath, @"Image.png")))
                {
                    // Serialize the file.
                    imageTileSerializer.Serialize(bitmap, 0, 0, 0);

                    // Assert that the file exists.
                    string path = Path.Combine(Environment.CurrentDirectory, imageTileSerializer.GetFileName(0, 0, 0));
                    Assert.IsTrue(File.Exists(path));

                    using (Bitmap newBitmap = imageTileSerializer.Deserialize(0, 0, 0) as Bitmap)
                    {
                        // Assert that the deserialized bitmap is not null.
                        Assert.IsNotNull(newBitmap);
                        for (int i = 0; i < bitmap.Width; i++)
                        {
                            for (int j = 0; j < bitmap.Height; j++)
                            {
                                Assert.AreEqual(bitmap.GetPixel(i, j), newBitmap.GetPixel(i, j));
                            }
                        }
                    }

                    // Delete the file.
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
