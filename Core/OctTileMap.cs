//-----------------------------------------------------------------------
// <copyright file="OctTileMap.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Represents the octahedral tile map.
    /// </summary>
    public class OctTileMap
    {
        /// <summary>
        /// Holds the master bounds for this map.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member", Justification = "Multidimensional array does not waste space.")]
        private static Vector3d[,] masterBounds = new Vector3d[3, 3];

        /// <summary>
        /// Minimum RA value.
        /// </summary>
        private double minimumRa;

        /// <summary>
        /// Maximum RA value.
        /// </summary>
        private double maximumRa;

        /// <summary>
        /// Minimum Declination value.
        /// </summary>
        private double decMin;

        /// <summary>
        /// Maximum Declination value.
        /// </summary>
        private double decMax;

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member", Justification = "Multidimensional array does not waste space.")]
        private Vector3d[,] bounds;

        /// <summary>
        /// True if this is backslash quadrant, False otherwise.
        /// </summary>
        private bool backslash;

        /// <summary>
        /// Map of RA and Declination.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member", Justification = "Multidimensional array does not waste space.")]
        private Vector2d[,] mapRaDec;

        /// <summary>
        /// Number of subdivisions.
        /// </summary>
        private int subDivisions;

        /// <summary>
        /// Size of subdivision.
        /// </summary>
        private float subDivSize;

        /// <summary>
        /// Initializes static members of the OctTileMap class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "We need to do it as part of the constructor.")]
        static OctTileMap()
        {
            OctTileMap.masterBounds[0, 0] = new Vector3d(0, -1, 0);
            OctTileMap.masterBounds[1, 0] = new Vector3d(0, 0, -1);
            OctTileMap.masterBounds[2, 0] = new Vector3d(0, -1, 0);
            OctTileMap.masterBounds[0, 1] = new Vector3d(1, 0, 0);
            OctTileMap.masterBounds[1, 1] = new Vector3d(0, 1, 0);
            OctTileMap.masterBounds[2, 1] = new Vector3d(-1, 0, 0);
            OctTileMap.masterBounds[0, 2] = new Vector3d(0, -1, 0);
            OctTileMap.masterBounds[1, 2] = new Vector3d(0, 0, 1);
            OctTileMap.masterBounds[2, 2] = new Vector3d(0, -1, 0);
        }

        /// <summary>
        /// Initializes a new instance of the OctTileMap class.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="pixelX">
        /// X coordinate.
        /// </param>
        /// <param name="pixelY">
        /// Y coordinate.
        /// </param>
        public OctTileMap(int level, int pixelX, int pixelY)
        {
            this.subDivisions = 5;
            this.subDivSize = 1.0f / (float)Math.Pow(2, 5);

            this.Level = level;
            this.Y = pixelY;
            this.X = pixelX;

            int levels = 0;
            Vector3d[,] oldBounds = null;
            this.backslash = false;
            while (levels <= level)
            {
                if (levels == 0)
                {
                    oldBounds = OctTileMap.masterBounds;
                }
                else
                {
                    Vector3d[,] newBounds = new Vector3d[3, 3];
                    int tempX = (int)(pixelX / Math.Pow(2, level - levels));
                    int tempY = (int)(pixelY / Math.Pow(2, level - levels));
                    int indexX = tempX % 2;
                    int indexY = tempY % 2;
                    if (levels == 1)
                    {
                        this.backslash = indexX == 1 ^ indexY == 1;
                    }

                    newBounds[0, 0] = oldBounds[indexX, indexY];
                    newBounds[1, 0] = Vector3d.MidPoint(oldBounds[indexX, indexY], oldBounds[indexX + 1, indexY]);
                    newBounds[2, 0] = oldBounds[indexX + 1, indexY];
                    newBounds[0, 1] = Vector3d.MidPoint(oldBounds[indexX, indexY], oldBounds[indexX, indexY + 1]);

                    if (this.backslash)
                    {
                        newBounds[1, 1] = Vector3d.MidPoint(oldBounds[indexX, indexY], oldBounds[indexX + 1, indexY + 1]);
                    }
                    else
                    {
                        newBounds[1, 1] = Vector3d.MidPoint(oldBounds[indexX + 1, indexY], oldBounds[indexX, indexY + 1]);
                    }

                    newBounds[2, 1] = Vector3d.MidPoint(oldBounds[indexX + 1, indexY], oldBounds[indexX + 1, indexY + 1]);
                    newBounds[0, 2] = oldBounds[indexX, indexY + 1];
                    newBounds[1, 2] = Vector3d.MidPoint(oldBounds[indexX, indexY + 1], oldBounds[indexX + 1, indexY + 1]);
                    newBounds[2, 2] = oldBounds[indexX + 1, indexY + 1];
                    oldBounds = newBounds;
                }

                levels++;
            }

            this.bounds = oldBounds;

            // Initialize the map.
            this.InitGrid();
        }

        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the Level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets the minimum Ra/Longitude value.
        /// </summary>
        public double RaMin
        {
            get
            {
                return this.minimumRa;
            }
        }

        /// <summary>
        /// Gets the maximum Ra/Longitude value.
        /// </summary>
        public double RaMax
        {
            get
            {
                return this.maximumRa;
            }
        }

        /// <summary>
        /// Gets the minimum Dec/Latitude value.
        /// </summary>
        public double DecMin
        {
            get
            {
                return this.decMin;
            }
        }

        /// <summary>
        /// Gets the Maximum Dec/Latitude value.
        /// </summary>
        public double DecMax
        {
            get
            {
                return this.decMax;
            }
        }

        /// <summary>
        /// Converts a given point to RA-Dec coordinate space.
        /// </summary>
        /// <param name="point">
        /// Point between 1 and 0 inclusive.
        /// </param>
        /// <returns>
        /// Point in RA-Dec coordinates.
        /// </returns>
        public Vector2d PointToRaDec(Vector2d point)
        {
            int indexX = (int)(point.X / this.subDivSize);
            int indexY = (int)(point.Y / this.subDivSize);

            if (indexX > ((int)Math.Pow(2, this.subDivisions) - 1))
            {
                indexX = ((int)Math.Pow(2, this.subDivisions) - 1);
            }

            if (indexY > ((int)Math.Pow(2, this.subDivisions) - 1))
            {
                indexY = ((int)Math.Pow(2, this.subDivisions) - 1);
            }

            double distX = (point.X - ((double)indexX * this.subDivSize)) / this.subDivSize;
            double distY = (point.Y - ((double)indexY * this.subDivSize)) / this.subDivSize;

            Vector2d interpolatedTop = Vector2d.Lerp(this.mapRaDec[indexX, indexY], this.mapRaDec[indexX + 1, indexY], distX);
            Vector2d interpolatedBottom = Vector2d.Lerp(this.mapRaDec[indexX, indexY + 1], this.mapRaDec[indexX + 1, indexY + 1], distX);
            Vector2d result = Vector2d.Lerp(interpolatedTop, interpolatedBottom, distY);
            return result;
        }

        /// <summary>
        /// Initializes the mapper.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body", Justification = "Multidimensional array does not waste space.")]
        private void InitGrid()
        {
            Collection<PositionTexture> vertexList = null;
            Collection<Triangle> triangleList = null;
            vertexList = new Collection<PositionTexture>();
            triangleList = new Collection<Triangle>();

            vertexList.Add(new PositionTexture(this.bounds[0, 0], 0, 0));
            vertexList.Add(new PositionTexture(this.bounds[1, 0], .5f, 0));
            vertexList.Add(new PositionTexture(this.bounds[2, 0], 1, 0));
            vertexList.Add(new PositionTexture(this.bounds[0, 1], 0, .5f));
            vertexList.Add(new PositionTexture(this.bounds[1, 1], .5f, .5f));
            vertexList.Add(new PositionTexture(this.bounds[2, 1], 1, .5f));
            vertexList.Add(new PositionTexture(this.bounds[0, 2], 0, 1));
            vertexList.Add(new PositionTexture(this.bounds[1, 2], .5f, 1));
            vertexList.Add(new PositionTexture(this.bounds[2, 2], 1, 1));

            if (this.Level == 0)
            {
                triangleList.Add(new Triangle(3, 7, 4));
                triangleList.Add(new Triangle(3, 6, 7));
                triangleList.Add(new Triangle(7, 5, 4));
                triangleList.Add(new Triangle(7, 8, 5));
                triangleList.Add(new Triangle(5, 1, 4));
                triangleList.Add(new Triangle(5, 2, 1));
                triangleList.Add(new Triangle(1, 3, 4));
                triangleList.Add(new Triangle(1, 0, 3));
            }
            else
            {
                if (this.backslash)
                {
                    triangleList.Add(new Triangle(4, 0, 3));
                    triangleList.Add(new Triangle(4, 1, 0));
                    triangleList.Add(new Triangle(5, 1, 4));
                    triangleList.Add(new Triangle(5, 2, 1));
                    triangleList.Add(new Triangle(3, 7, 4));
                    triangleList.Add(new Triangle(3, 6, 7));
                    triangleList.Add(new Triangle(8, 4, 7));
                    triangleList.Add(new Triangle(8, 5, 4));
                }
                else
                {
                    triangleList.Add(new Triangle(1, 0, 3));
                    triangleList.Add(new Triangle(1, 3, 4));
                    triangleList.Add(new Triangle(2, 1, 4));
                    triangleList.Add(new Triangle(2, 4, 5));
                    triangleList.Add(new Triangle(6, 4, 3));
                    triangleList.Add(new Triangle(6, 7, 4));
                    triangleList.Add(new Triangle(7, 5, 4));
                    triangleList.Add(new Triangle(8, 5, 7));
                }
            }

            int count = this.subDivisions;
            this.subDivSize = 1.0f / (float)Math.Pow(2, this.subDivisions);
            while (count-- > 1)
            {
                Collection<Triangle> newList = new Collection<Triangle>();
                foreach (Triangle tri in triangleList)
                {
                    tri.SubDivide(newList, vertexList);
                }

                triangleList = newList;
            }

            int countX = 1 + (int)Math.Pow(2, this.subDivisions);
            int countY = 1 + (int)Math.Pow(2, this.subDivisions);

            PositionTexture[,] points = new PositionTexture[countX, countY];
            this.mapRaDec = new Vector2d[countX, countY];
            foreach (PositionTexture vertex in vertexList)
            {
                int indexX = (int)((vertex.Tu / this.subDivSize) + .1);
                int indexY = (int)((vertex.Tv / this.subDivSize) + .1);

                points[indexX, indexY] = vertex;
            }

            for (int y = 0; y < countY; y++)
            {
                for (int x = 0; x < countX; x++)
                {
                    this.mapRaDec[x, y] = points[x, y].Position.ToRaDec();
                }
            }

            if (this.Level == 0)
            {
                this.minimumRa = 0;
                this.maximumRa = 360;
                this.decMin = -90;
                this.decMax = 90;
            }
            else
            {
                this.minimumRa = Math.Min(Math.Min(this.mapRaDec[0, 0].X, this.mapRaDec[0, countY - 1].X), Math.Min(this.mapRaDec[countX - 1, 0].X, this.mapRaDec[countX - 1, countY - 1].X));
                this.maximumRa = Math.Max(Math.Max(this.mapRaDec[0, 0].X, this.mapRaDec[0, countY - 1].X), Math.Max(this.mapRaDec[countX - 1, 0].X, this.mapRaDec[countX - 1, countY - 1].X));
                this.decMin = Math.Min(Math.Min(this.mapRaDec[0, 0].Y, this.mapRaDec[0, countY - 1].Y), Math.Min(this.mapRaDec[countX - 1, 0].Y, this.mapRaDec[countX - 1, countY - 1].Y));
                this.decMax = Math.Max(Math.Max(this.mapRaDec[0, 0].Y, this.mapRaDec[0, countY - 1].Y), Math.Max(this.mapRaDec[countX - 1, 0].Y, this.mapRaDec[countX - 1, countY - 1].Y));
            }
        }
    }
}
