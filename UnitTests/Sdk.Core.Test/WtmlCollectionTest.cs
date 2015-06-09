//-----------------------------------------------------------------------
// <copyright file="WtmlCollectionTest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    [TestClass]
    public class WtmlCollectionTest
    {
        [TestMethod]
        public void InitializeTest()
        {
            WtmlCollection wtmlCollection = new WtmlCollection("Sample", "tempName.jpeg", @"D:\Temp", 10, ProjectionTypes.Mercator);
            Assert.IsNotNull(wtmlCollection);
        }

        [TestMethod]
        public void MercatorSaveTest()
        {
            WtmlCollection wtmlCollection = new WtmlCollection("Sample", "tempName.jpeg", @"D:\Temp", 10, ProjectionTypes.Mercator);
            Assert.IsNotNull(wtmlCollection);

            string filePath = @"temp.wtml";
            wtmlCollection.Save(filePath);
            Assert.IsTrue(File.Exists(filePath));
            XDocument wtmlDoc = XDocument.Load(filePath);
            Assert.AreEqual("Folder", wtmlDoc.Root.Name.ToString(), true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Sample", wtmlDoc.Root.FirstAttribute.Value, true, CultureInfo.InvariantCulture);

            XElement placeNode = wtmlDoc.Root.FirstNode as XElement;
            Assert.AreEqual("Sample", placeNode.Attributes("Name").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Earth", placeNode.Attributes("DataSetType").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", placeNode.Attributes("ZoomLevel").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", placeNode.Attributes("Lat").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", placeNode.Attributes("Lng").Single().Value, true, CultureInfo.InvariantCulture);

            XElement imageSetParentNode = placeNode.FirstNode as XElement;
            XElement firstNode = imageSetParentNode.FirstNode as XElement;
            Assert.AreEqual("ImageSet", firstNode.Name.ToString(), true, CultureInfo.InvariantCulture);
            Assert.AreEqual("False", firstNode.Attributes("Generic").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Earth", firstNode.Attributes("DataSetType").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Visible", firstNode.Attributes("BandPass").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Sample", firstNode.Attributes("Name").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("BaseTileLevel").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("10", firstNode.Attributes("TileLevels").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("360", firstNode.Attributes("BaseDegreesPerTile").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual(".png", firstNode.Attributes("FileType").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("False", firstNode.Attributes("BottomsUp").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Mercator", firstNode.Attributes("Projection").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual(string.Empty, firstNode.Attributes("QuadTreeMap").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("CenterX").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("CenterY").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("OffsetX").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("OffsetY").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("Rotation").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("False", firstNode.Attributes("Sparse").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual(@"D:\Temp", firstNode.Attributes("Url").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("False", firstNode.Attributes("ElevationModel").Single().Value, true, CultureInfo.InvariantCulture);

            XElement thumbnailUrlNode = firstNode.FirstNode as XElement;
            Assert.AreEqual("ThumbnailUrl", thumbnailUrlNode.Name.ToString(), true, CultureInfo.InvariantCulture);
            Assert.AreEqual("tempName.jpeg", thumbnailUrlNode.Value, true, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void ToastSaveTest()
        {
            WtmlCollection wtmlCollection = new WtmlCollection("Sample", "tempName.jpeg", @"D:\Temp", 10, ProjectionTypes.Toast);
            Assert.IsNotNull(wtmlCollection);

            string filePath = @"Temp\temp.wtml";
            wtmlCollection.Save(filePath);
            Assert.IsTrue(File.Exists(filePath));
            XDocument wtmlDoc = XDocument.Load(filePath);
            Assert.AreEqual("Folder", wtmlDoc.Root.Name.ToString(), true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Sample", wtmlDoc.Root.FirstAttribute.Value, true, CultureInfo.InvariantCulture);

            XElement placeNode = wtmlDoc.Root.FirstNode as XElement;
            Assert.AreEqual("Sample", placeNode.Attributes("Name").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Earth", placeNode.Attributes("DataSetType").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", placeNode.Attributes("ZoomLevel").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", placeNode.Attributes("Lat").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", placeNode.Attributes("Lng").Single().Value, true, CultureInfo.InvariantCulture);

            XElement imageSetParentNode = placeNode.FirstNode as XElement;

            XElement firstNode = imageSetParentNode.FirstNode as XElement;
            Assert.AreEqual("ImageSet", firstNode.Name.ToString(), true, CultureInfo.InvariantCulture);
            Assert.AreEqual("False", firstNode.Attributes("Generic").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Earth", firstNode.Attributes("DataSetType").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Visible", firstNode.Attributes("BandPass").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Sample", firstNode.Attributes("Name").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("BaseTileLevel").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("10", firstNode.Attributes("TileLevels").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("90", firstNode.Attributes("BaseDegreesPerTile").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual(".png", firstNode.Attributes("FileType").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("False", firstNode.Attributes("BottomsUp").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("Toast", firstNode.Attributes("Projection").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual(string.Empty, firstNode.Attributes("QuadTreeMap").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("CenterX").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("CenterY").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("OffsetX").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("OffsetY").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("0", firstNode.Attributes("Rotation").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("False", firstNode.Attributes("Sparse").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual(@"D:\Temp", firstNode.Attributes("Url").Single().Value, true, CultureInfo.InvariantCulture);
            Assert.AreEqual("False", firstNode.Attributes("ElevationModel").Single().Value, true, CultureInfo.InvariantCulture);

            XElement thumbnailUrlNode = firstNode.FirstNode as XElement;
            Assert.AreEqual("ThumbnailUrl", thumbnailUrlNode.Name.ToString(), true, CultureInfo.InvariantCulture);
            Assert.AreEqual("tempName.jpeg", thumbnailUrlNode.Value, true, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void GetWtmlTextureTilePathTest()
        {
            string textureTilePath = @"Pyramid\{0}\{1}\L{0}X{1}Y{2}.{3}";
            string extension = "png";
            string path = WtmlCollection.GetWtmlTextureTilePath(textureTilePath, extension);
            Assert.AreEqual(@"Pyramid\{1}\{2}\L{1}X{2}Y{3}.png", path);
        }

        [TestMethod]
        public void GetWtmlTextureTilePathInvalidTest()
        {
            string textureTilePath = string.Empty;
            string extension = "png";
            string path = WtmlCollection.GetWtmlTextureTilePath(textureTilePath, extension);
            Assert.AreEqual(string.Empty, path);
        }

        [TestMethod]
        public void GetWtmlTextureTilePathEmptyExtensionTest()
        {
            string textureTilePath = @"Pyramid\{0}\{1}\L{0}X{1}Y{2}.{3}";
            string extension = string.Empty;
            string path = WtmlCollection.GetWtmlTextureTilePath(textureTilePath, extension);
            Assert.AreEqual(@"Pyramid\{1}\{2}\L{1}X{2}Y{3}.png", path);
        }
    }
}
