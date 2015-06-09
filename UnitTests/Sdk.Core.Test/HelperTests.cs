//-----------------------------------------------------------------------
// <copyright file="HelperTests.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    [TestClass]
    public class HelperTests
    {
        [TestMethod]
        public void GetMercatorTilesTest()
        {
            double xmin = -136.40173;
            double xmax = -135.91382;
            double ymin = 58.75443;
            double ymax = 58.35286;
            Dictionary<int, List<int>> xvalues = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> yvalues = new Dictionary<int, List<int>>();
            xvalues.Add(0, new List<int>() { 0, 0 });
            xvalues.Add(1, new List<int>() { 0, 0 });
            xvalues.Add(2, new List<int>() { 0, 0 });
            xvalues.Add(3, new List<int>() { 0, 0 });
            xvalues.Add(4, new List<int>() { 1, 1 });
            xvalues.Add(5, new List<int>() { 3, 3 });
            xvalues.Add(6, new List<int>() { 7, 7 });
            xvalues.Add(7, new List<int>() { 15, 15 });
            xvalues.Add(8, new List<int>() { 31, 31 });
            xvalues.Add(9, new List<int>() { 62, 62 });
            xvalues.Add(10, new List<int>() { 124, 125 });
            xvalues.Add(11, new List<int>() { 248, 250 });
            xvalues.Add(12, new List<int>() { 496, 501 });
            xvalues.Add(13, new List<int>() { 992, 1003 });
            xvalues.Add(14, new List<int>() { 1984, 2006 });
            xvalues.Add(15, new List<int>() { 3968, 4012 });
            xvalues.Add(16, new List<int>() { 7936, 8025 });

            yvalues.Add(0, new List<int>() { 0, 0 });
            yvalues.Add(1, new List<int>() { 0, 0 });
            yvalues.Add(2, new List<int>() { 1, 1 });
            yvalues.Add(3, new List<int>() { 2, 2 });
            yvalues.Add(4, new List<int>() { 4, 4 });
            yvalues.Add(5, new List<int>() { 9, 9 });
            yvalues.Add(6, new List<int>() { 19, 19 });
            yvalues.Add(7, new List<int>() { 38, 38 });
            yvalues.Add(8, new List<int>() { 76, 76 });
            yvalues.Add(9, new List<int>() { 152, 153 });
            yvalues.Add(10, new List<int>() { 304, 306 });
            yvalues.Add(11, new List<int>() { 608, 613 });
            yvalues.Add(12, new List<int>() { 1217, 1226 });
            yvalues.Add(13, new List<int>() { 2434, 2452 });
            yvalues.Add(14, new List<int>() { 4869, 4904 });
            yvalues.Add(15, new List<int>() { 9738, 9808 });
            yvalues.Add(16, new List<int>() { 19476, 19616 });

            for (int level = 0; level <= 16; level++)
            {
                int xminTile = Helper.GetMercatorXTileFromLongitude(xmin, level);
                int xmaxTile = Helper.GetMercatorXTileFromLongitude(xmax, level);
                int yminTile = Helper.GetMercatorYTileFromLatitude(ymin, level);
                int ymaxTile = Helper.GetMercatorYTileFromLatitude(ymax, level);

                Assert.AreEqual(xvalues[level].First(), xminTile);
                Assert.AreEqual(xvalues[level].Last(), xmaxTile);
                Assert.AreEqual(yvalues[level].First(), yminTile);
                Assert.AreEqual(yvalues[level].Last(), ymaxTile);
            }
        }

        [TestMethod]
        public void DegreesToRadiansTest()
        {
            double degrees = 90;
            double expectedRadians = (degrees * Math.PI / 180.0);
            Assert.AreEqual(expectedRadians, Helper.DegreesToRadians(degrees));

            degrees = 0;
            expectedRadians = (degrees * Math.PI / 180.0);
            Assert.AreEqual(expectedRadians, Helper.DegreesToRadians(degrees));

            degrees = 180;
            expectedRadians = (degrees * Math.PI / 180.0);
            Assert.AreEqual(expectedRadians, Helper.DegreesToRadians(degrees));

            degrees = -90;
            expectedRadians = (degrees * Math.PI / 180.0);
            Assert.AreEqual(expectedRadians, Helper.DegreesToRadians(degrees));

            degrees = -180;
            expectedRadians = (degrees * Math.PI / 180.0);
            Assert.AreEqual(expectedRadians, Helper.DegreesToRadians(degrees));
        }

        [TestMethod]
        public void RadiansToDegreesTest()
        {
            double radians = 1.57;
            double expectedDegrees = (radians * 180.0 / Math.PI);
            Assert.AreEqual(expectedDegrees, Helper.RadiansToDegrees(radians));

            radians = 0;
            expectedDegrees = (radians * 180.0 / Math.PI);
            Assert.AreEqual(expectedDegrees, Helper.RadiansToDegrees(radians));

            radians = 3.14;
            expectedDegrees = (radians * 180.0 / Math.PI);
            Assert.AreEqual(expectedDegrees, Helper.RadiansToDegrees(radians));

            radians = -1.57;
            expectedDegrees = (radians * 180.0 / Math.PI);
            Assert.AreEqual(expectedDegrees, Helper.RadiansToDegrees(radians));

            radians = -3.14;
            expectedDegrees = (radians * 180.0 / Math.PI);
            Assert.AreEqual(expectedDegrees, Helper.RadiansToDegrees(radians));
        }

        [TestMethod]
        public void LatLonToHVTest()
        {
            double lon = -136.40173;
            double lat = 58.75443;
            int[] hv = Helper.LatLonToHV(lon, lat);
            Assert.AreEqual(10, hv[0]);
            Assert.AreEqual(3, hv[1]);
        }

        [TestMethod]
        public void MetersPerPixelTest()
        {
            double expectedMeters = 152.8740234375;
            double meters = Helper.MetersPerPixel(10);
            Assert.AreEqual(expectedMeters, meters);
        }

        [TestMethod]
        public void AbsoluteMetersToLatitudeAtZoomTest()
        {
            double expectedLatitude = 85.0333262532395;
            int meters = 150;
            double latitude = Helper.AbsoluteMetersToLatitudeAtZoom(meters, 10);
            Assert.AreEqual(expectedLatitude, latitude);
        }

        [TestMethod]
        public void MapSizeTest()
        {
            uint expected = 256;
            int level = 0;
            uint actual = Helper.MapSize(level);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LatLongToPixelXYTest()
        {
            double latitude = 40;
            double longitude = 60;
            int levelOfDetail = 3;

            int expectedXStart = 1365;
            int expectedYStart = 775;

            int actualXStart = 0;
            int actualYStart = 0;

            Helper.LatLongToPixelXY(latitude, longitude, levelOfDetail, out actualXStart, out actualYStart);

            Assert.AreEqual(expectedXStart, actualXStart);
            Assert.AreEqual(expectedYStart, actualYStart);
        }
    }
}
