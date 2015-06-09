//-----------------------------------------------------------------------
// <copyright file="DemTileSerializerTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    [TestClass]
    public class DemTileSerializerTests : TestBase
    {
        [TestMethod]
        public void ValidDemSerializeTest()
        {
            try
            {
                string fileNameTemplate = @"Pyramid\{0}\{1}\DL{0}X{1}Y{2}.dem";
                string destination = Path.Combine(Environment.CurrentDirectory, fileNameTemplate);
                var demTileSerializer = new DemTileSerializer(destination);

                // Serialize the file.
                short[] values = new short[1089];
                Random random = new Random();
                for (int index = 0; index < 1089; index++)
                {
                    values[index] = Convert.ToInt16(random.Next(100));
                }

                int level = 0;
                int tileX = 0;
                int tileY = 0;
                demTileSerializer.Serialize(values, level, tileX, tileY);

                // Assert that the file exists.
                string path = demTileSerializer.GetFileName(level, tileX, tileY);
                Assert.AreEqual(path, Path.Combine(Environment.CurrentDirectory, @"Pyramid\0\0\DL0X0Y0.dem"));
                Assert.IsTrue(File.Exists(path));

                // Delete the file.
                File.Delete(path);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void ValidDemDeserializeTest()
        {
            try
            {
                string fileNameTemplate = @"Pyramid\{0}\{1}\DL{0}X{1}Y{2}.dem";
                string destination = Path.Combine(Environment.CurrentDirectory, fileNameTemplate);
                var demTileSerializer = new DemTileSerializer(destination);

                // Serialize the file.
                short[] values = new short[1089];
                Random random = new Random();
                for (int index = 0; index < 1089; index++)
                {
                    values[index] = Convert.ToInt16(random.Next(100));
                }

                int level = 3;
                int tileX = 2;
                int tileY = 1;
                demTileSerializer.Serialize(values, level, tileX, tileY);

                // Assert that the file exists.
                string path = Path.Combine(Environment.CurrentDirectory, demTileSerializer.GetFileName(level, tileX, tileY));
                Assert.AreEqual(path, Path.Combine(Environment.CurrentDirectory, @"Pyramid\3\2\DL3X2Y1.dem"));
                Assert.IsTrue(File.Exists(path));

                var newBytes = demTileSerializer.Deserialize(level, tileX, tileY);

                // Assert that the deserialized bitmap is not null and holds original values.
                Assert.IsNotNull(newBytes);
                for (int i = 0; i < values.Length; i++)
                {
                    Assert.AreEqual(values[i], newBytes[i]);
                }

                // Delete the file.
                File.Delete(path);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}