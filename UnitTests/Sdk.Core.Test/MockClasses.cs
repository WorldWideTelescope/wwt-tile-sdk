//-----------------------------------------------------------------------
// <copyright file="MockClasses.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    public static class MockClasses
    {
        internal class MockColorMap : IColorMap
        {
            public double[] ExpectedLongitudes { get; set; }

            public double[] ExpectedLatitudes { get; set; }

            public int Count { get; set; }

            public Color GetColor(double longitude, double latitude)
            {
                double lng = ExpectedLongitudes[Count];
                if (!double.IsNaN(lng))
                {
                    Assert.IsTrue(Math.Abs(longitude - lng) < 0.0000001);
                }

                double lat = ExpectedLatitudes[Count];
                if (!double.IsNaN(lat))
                {
                    Assert.IsTrue(Math.Abs(latitude - lat) < 0.0000001);
                }

                Count++;
                return Color.Transparent;
            }
        }

        internal class MockTileSerializer : IImageTileSerializer
        {
            public void Serialize(Bitmap tile, int level, int tileX, int tileY)
            {
                // No-op. Intentional.
            }

            public Bitmap Deserialize(int level, int tileX, int tileY)
            {
                return null;
            }
        }

        internal class MockTileLocator : TestBase, IImageTileSerializer
        {
            public const string ImageFileNameTemplate = @"{3}\SamplePyramid\{0}\{1}\L{0}X{1}Y{2}.png";

            public void Serialize(Bitmap tile, int level, int tileX, int tileY)
            {
                // No-op. Intentional.
            }

            public Bitmap Deserialize(int level, int tileX, int tileY)
            {
                string path = string.Format(CultureInfo.InvariantCulture, ImageFileNameTemplate, level, tileX, tileY, TestDataPath);
                return new Bitmap(path);
            }

            public string GetFileName(int level, int tileX, int tileY)
            {
                return string.Format(CultureInfo.InvariantCulture, ImageFileNameTemplate, level, tileX, tileY, TestDataPath);
            }
        }

        internal class MockMultipleTileLocator : TestBase, IImageTileSerializer
        {
            public const string ImageFileNameTemplate = @"{3}\SamplePyramid\{0}\{1}\L{0}X{1}Y{2}.png";

            public void Serialize(Bitmap tile, int level, int tileX, int tileY)
            {
                // No-op. Intentional.
            }

            public Bitmap Deserialize(int level, int tileX, int tileY)
            {
                string path = string.Format(ImageFileNameTemplate, 0, 0, 0, TestDataPath);
                return new Bitmap(path);
            }
        }

        internal class MockDemTileLocator : TestBase, IDemTileSerializer
        {
            public void Serialize(short[] tile, int level, int tileX, int tileY)
            {
                // No-op. Intentional.
            }

            public short[] Deserialize(int level, int tileX, int tileY)
            {
                if (level == 0)
                {
                    return new short[] { 0, 1, 2, 3, 4, 5 };
                }
                else
                {
                    if (tileX == 0 && tileY == 0)
                    {
                        return new short[] { 6, 7, 8, 9, 10 };
                    }
                    else if (tileX == 1 && tileY == 0)
                    {
                        return new short[] { 10, 11, 12, 13, 14, 15 };
                    }
                    else if (tileX == 0 && tileY == 1)
                    {
                        return new short[] { 20, 21, 22, 23, 24, 25 };
                    }
                    else
                    {
                        return new short[] { 30, 31, 32, 33, 34, 35 };
                    }
                }
            }
        }

        internal class MockMultipleDemTileLocator : TestBase, IDemTileSerializer
        {
            public void Serialize(short[] tile, int level, int tileX, int tileY)
            {
                // No-op. Intentional.
            }

            public short[] Deserialize(int level, int tileX, int tileY)
            {
                return new short[] { 0, 1, 2, 3, 4, 5 };
            }
        }

        internal class MockTileCreator : TestBase, IImageTileSerializer
        {
            public const string ImageFileNameTemplate = @"{3}\Test_L{0}X{1}Y{2}.png";

            public void Serialize(Bitmap tile, int level, int tileX, int tileY)
            {
                if (tile == null)
                {
                    throw new ArgumentNullException("tile");
                }

                string path = string.Format(CultureInfo.InvariantCulture, ImageFileNameTemplate, level, tileX, tileY, TestDataPath);
                tile.Save(path);
            }

            public Bitmap Deserialize(int level, int tileX, int tileY)
            {
                // No Op intentional
                return null;
            }

            public string GetFileName(int level, int tileX, int tileY)
            {
                return string.Format(CultureInfo.InvariantCulture, ImageFileNameTemplate, level, tileX, tileY, TestDataPath);
            }
        }

        internal class MockDemTileSerializer : IDemTileSerializer
        {
            public void Serialize(short[] tile, int level, int tileX, int tileY)
            {
                // No-op. Intentional.
            }

            public short[] Deserialize(int level, int tileX, int tileY)
            {
                return null;
            }
        }

        internal class MockElevationMap : IElevationMap
        {
            public short GetElevation(double longitude, double latitude)
            {
                return 1;
            }
        }

        internal class MockTileCreatorForTileGenerator : ITileCreator
        {
            public MockTileCreatorForTileGenerator()
            {
                TileCreated = new Dictionary<int, int>();
            }

            public Dictionary<int, int> TileCreated { get; set; }

            public ProjectionTypes ProjectionType
            {
                get
                {
                    return ProjectionTypes.Toast;
                }
            }

            public void Create(int level, int tileX, int tileY)
            {
                IncrementTileCreated(level);
            }

            public void CreateParent(int level, int tileX, int tileY)
            {
                IncrementTileCreated(level);
            }

            private void IncrementTileCreated(int level)
            {
                lock (this)
                {
                    if (!TileCreated.ContainsKey(level))
                    {
                        TileCreated.Add(level, 0);
                    }

                    TileCreated[level] += 1;
                }
            }
        }

        internal class MockTileCreatorForMercatorTileGenerator : ITileCreator
        {
            public MockTileCreatorForMercatorTileGenerator()
            {
                TileCreated = new Dictionary<int, int>();
            }

            public Dictionary<int, int> TileCreated { get; set; }

            public ProjectionTypes ProjectionType
            {
                get
                {
                    return ProjectionTypes.Mercator;
                }
            }

            public void Create(int level, int tileX, int tileY)
            {
                IncrementTileCreated(level);
            }

            public void CreateParent(int level, int tileX, int tileY)
            {
                IncrementTileCreated(level);
            }

            private void IncrementTileCreated(int level)
            {
                lock (this)
                {
                    if (!TileCreated.ContainsKey(level))
                    {
                        TileCreated.Add(level, 0);
                    }

                    TileCreated[level] += 1;
                }
            }
        }

        internal class MockGrid : IGrid
        {
            public int Height
            {
                get { return 10; }
            }

            public int Width
            {
                get { return 10; }
            }

            public double GetValue(double u, double v)
            {
                return u + v;
            }

            public double GetValueAt(int i, int j)
            {
                return i + j;
            }

            public int GetXIndex(double u)
            {
                return (int)u;
            }

            public int GetYIndex(double v)
            {
                return (int)v;
            }
        }

        internal class MockProjectionGridMap : IProjectionGridMap
        {
            private Boundary inputBoundary;
            private IGrid inputGrid;
            public MockProjectionGridMap()
            {
                inputBoundary = new Boundary(-180, -90, 180, 90);
                inputGrid = new MockGrid();
            }

            public Boundary InputBoundary
            {
                get { return inputBoundary; }
            }

            public IGrid InputGrid
            {
                get { return inputGrid; }
            }

            public double GetValue(double longitude, double latitude)
            {
                return longitude + latitude;
            }

            public int GetXIndex(double longitude)
            {
                return (int)longitude;
            }

            public int GetYIndex(double latitude)
            {
                return (int)latitude;
            }

            public bool IsInRange(double longitude, double latitude)
            {
                return true;
            }
        }
    }
}
