//-----------------------------------------------------------------------
// <copyright file="PlateTilePyramidTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    /// <summary>
    /// This is a test class for PlateTilePyramidTest and is intended
    /// to contain all PlateTilePyramidTest Unit Tests.
    /// </summary>
    [TestClass()]
    public class PlateTilePyramidTest : TestBase
    {
        public const string ImageFileNameTemplate = @"{3}\SamplePyramid\{0}\{1}\L{0}X{1}Y{2}.png";
        public const string TestFileName = "TestPlate.plate";
        public const string TestDEMFileName = "TestDEMPlate.plate";

        /// <summary>
        /// A test for Create.
        /// </summary>
        [TestMethod()]
        public void CreateTest()
        {
            string fileName = "TestCreateTest.plate";
            int levels = 1;
            PlateFile target = new PlateFile(Path.Combine(TestDataPath, fileName), levels);
            target.Create();
            target.UpdateHeaderAndClose();

            Assert.IsTrue(File.Exists(fileName));
        }

        /// <summary>
        /// A test for AddStream.
        /// </summary>
        [TestMethod()]
        public void AddStreamTest()
        {
            string fileName = "AddStreamTest.plate";
            int levels = 1;
            PlateFile target = new PlateFile(Path.Combine(TestDataPath, fileName), levels);
            target.Create();

            target.UpdateHeaderAndClose();

            Assert.IsTrue(File.Exists(fileName));
        }
    }
}
