//---------------------------------------------------------------------------
// <copyright file="ToastDemTileCreator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//---------------------------------------------------------------------------

using System;
using System.Linq;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Creates depth elevation model tiles for TOAST projection.
    /// </summary>
    public class ToastDemTileCreator : ITileCreator
    {
        /// <summary>
        /// Interval between vertices where elevation is sampled.
        /// </summary>
        private const int VertexInterval = 32;

        /// <summary>
        /// Upper-right and lower-left quadrants (Backslash "\" quadrants).
        /// </summary>
        private static int[] backslashVerticesX =
        { 
            0, 16, 32, 0, 16, 32, 0, 16, 32, 8, 8, 16, 8, 0, 8, 12, 12, 16, 12, 16, 12, 8, 4, 4, 12, 12, 8, 4, 0, 4, 8, 12, 12, 4, 4, 0, 4, 8, 4, 14, 14, 16,
            14, 16, 14, 12, 10, 10, 14, 14, 12, 14, 16, 14, 12, 10, 10, 14, 14, 16, 14, 12, 14, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 6, 6, 10, 10, 8, 10, 8, 10, 12,
            14, 14, 10, 10, 12, 2, 0, 2, 4, 6, 6, 2, 2, 0, 2, 4, 2, 12, 14, 14, 10, 10, 8, 10, 8, 10, 12, 10, 10, 2, 2, 0, 2, 0, 2, 4, 6, 6, 2, 2, 4, 6, 8, 6,
            4, 2, 2, 6, 6, 8, 6, 4, 6, 24, 24, 32, 24, 16, 24, 28, 28, 32, 28, 32, 28, 24, 20, 20, 28, 28, 24, 20, 16, 20, 24, 28, 28, 20, 20, 16, 20, 24, 20,
            30, 30, 32, 30, 32, 30, 28, 26, 26, 30, 30, 28, 30, 32, 30, 28, 26, 26, 30, 30, 32, 30, 28, 30, 20, 18, 18, 22, 22, 24, 22, 24, 22, 20, 22, 22, 26,
            26, 24, 26, 24, 26, 28, 30, 30, 26, 26, 28, 18, 16, 18, 20, 22, 22, 18, 18, 16, 18, 20, 18, 28, 30, 30, 26, 26, 24, 26, 24, 26, 28, 26, 26, 18, 18,
            16, 18, 16, 18, 20, 22, 22, 18, 18, 20, 22, 24, 22, 20, 18, 18, 22, 22, 24, 22, 20, 22, 8, 8, 16, 8, 0, 8, 12, 12, 16, 12, 16, 12, 8, 4, 4, 12, 12,
            8, 4, 0, 4, 8, 12, 12, 4, 4, 0, 4, 8, 4, 14, 14, 16, 14, 16, 14, 12, 10, 10, 14, 14, 12, 14, 16, 14, 12, 10, 10, 14, 14, 16, 14, 12, 14, 4, 2, 2, 6,
            6, 8, 6, 8, 6, 4, 6, 6, 10, 10, 8, 10, 8, 10, 12, 14, 14, 10, 10, 12, 2, 0, 2, 4, 6, 6, 2, 2, 0, 2, 4, 2, 12, 14, 14, 10, 10, 8, 10, 8, 10, 12, 10,
            10, 2, 2, 0, 2, 0, 2, 4, 6, 6, 2, 2, 4, 6, 8, 6, 4, 2, 2, 6, 6, 8, 6, 4, 6, 24, 24, 32, 24, 16, 24, 28, 28, 32, 28, 32, 28, 24, 20, 20, 28, 28, 24,
            20, 16, 20, 24, 28, 28, 20, 20, 16, 20, 24, 20, 30, 30, 32, 30, 32, 30, 28, 26, 26, 30, 30, 28, 30, 32, 30, 28, 26, 26, 30, 30, 32, 30, 28, 30, 20,
            18, 18, 22, 22, 24, 22, 24, 22, 20, 22, 22, 26, 26, 24, 26, 24, 26, 28, 30, 30, 26, 26, 28, 18, 16, 18, 20, 22, 22, 18, 18, 16, 18, 20, 18, 28, 30,
            30, 26, 26, 24, 26, 24, 26, 28, 26, 26, 18, 18, 16, 18, 16, 18, 20, 22, 22, 18, 18, 20, 22, 24, 22, 20, 18, 18, 22, 22, 24, 22, 20, 22
        };

        /// <summary>
        /// Upper-right and lower-left quadrants (Backslash "\" quadrants).
        /// </summary>
        private static int[] backslashVerticesY =
        {
            0, 0, 0, 16, 16, 16, 32, 32, 32, 0, 8, 8, 8, 8, 16, 8, 12, 12, 4, 4, 0, 4, 0, 4, 8, 4, 4, 12, 12, 16, 12, 16, 12, 8, 4, 4, 12, 12, 8, 12, 14, 14, 10,
            10, 8, 10, 8, 10, 12, 10, 10, 2, 2, 0, 2, 0, 2, 4, 6, 6, 2, 2, 4, 2, 0, 2, 4, 6, 6, 2, 2, 0, 2, 4, 2, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 6, 6, 14, 14, 16,
            14, 16, 14, 12, 10, 10, 14, 14, 12, 14, 16, 14, 12, 10, 10, 14, 14, 16, 14, 12, 14, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 6, 6, 10, 10, 8, 10, 8, 10, 12, 14,
            14, 10, 10, 12, 0, 8, 8, 8, 8, 16, 8, 12, 12, 4, 4, 0, 4, 0, 4, 8, 4, 4, 12, 12, 16, 12, 16, 12, 8, 4, 4, 12, 12, 8, 12, 14, 14, 10, 10, 8, 10, 8, 10,
            12, 10, 10, 2, 2, 0, 2, 0, 2, 4, 6, 6, 2, 2, 4, 2, 0, 2, 4, 6, 6, 2, 2, 0, 2, 4, 2, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 6, 6, 14, 14, 16, 14, 16, 14, 12, 10,
            10, 14, 14, 12, 14, 16, 14, 12, 10, 10, 14, 14, 16, 14, 12, 14, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 6, 6, 10, 10, 8, 10, 8, 10, 12, 14, 14, 10, 10, 12, 16, 24,
            24, 24, 24, 32, 24, 28, 28, 20, 20, 16, 20, 16, 20, 24, 20, 20, 28, 28, 32, 28, 32, 28, 24, 20, 20, 28, 28, 24, 28, 30, 30, 26, 26, 24, 26, 24, 26, 28,
            26, 26, 18, 18, 16, 18, 16, 18, 20, 22, 22, 18, 18, 20, 18, 16, 18, 20, 22, 22, 18, 18, 16, 18, 20, 18, 20, 18, 18, 22, 22, 24, 22, 24, 22, 20, 22, 22,
            30, 30, 32, 30, 32, 30, 28, 26, 26, 30, 30, 28, 30, 32, 30, 28, 26, 26, 30, 30, 32, 30, 28, 30, 20, 18, 18, 22, 22, 24, 22, 24, 22, 20, 22, 22, 26, 26,
            24, 26, 24, 26, 28, 30, 30, 26, 26, 28, 16, 24, 24, 24, 24, 32, 24, 28, 28, 20, 20, 16, 20, 16, 20, 24, 20, 20, 28, 28, 32, 28, 32, 28, 24, 20, 20, 28,
            28, 24, 28, 30, 30, 26, 26, 24, 26, 24, 26, 28, 26, 26, 18, 18, 16, 18, 16, 18, 20, 22, 22, 18, 18, 20, 18, 16, 18, 20, 22, 22, 18, 18, 16, 18, 20, 18,
            20, 18, 18, 22, 22, 24, 22, 24, 22, 20, 22, 22, 30, 30, 32, 30, 32, 30, 28, 26, 26, 30, 30, 28, 30, 32, 30, 28, 26, 26, 30, 30, 32, 30, 28, 30, 20, 18,
            18, 22, 22, 24, 22, 24, 22, 20, 22, 22, 26, 26, 24, 26, 24, 26, 28, 30, 30, 26, 26, 28
        };

        /// <summary>
        /// Upper-left and lower-right quadrants (Slash "/" quadrants).
        /// </summary>
        private static int[] slashVerticesX =
        { 
            0, 16, 32, 0, 16, 32, 0, 16, 32, 8, 0, 8, 8, 8, 16, 4, 0, 4, 8, 12, 12, 4, 4, 0, 4, 8, 4, 12, 12, 16, 12, 16, 12, 8, 4, 4, 12, 12, 8, 2, 0, 2, 4, 6, 6,
            2, 2, 0, 2, 4, 2, 12, 14, 14, 10, 10, 8, 10, 8, 10, 12, 10, 10, 2, 2, 0, 2, 0, 2, 4, 6, 6, 2, 2, 4, 6, 8, 6, 4, 2, 2, 6, 6, 8, 6, 4, 6, 14, 14, 16, 14,
            16, 14, 12, 10, 10, 14, 14, 12, 14, 16, 14, 12, 10, 10, 14, 14, 16, 14, 12, 14, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 6, 6, 10, 10, 8, 10, 8, 10, 12, 14, 14, 10,
            10, 12, 24, 16, 24, 24, 24, 32, 20, 16, 20, 24, 28, 28, 20, 20, 16, 20, 24, 20, 28, 28, 32, 28, 32, 28, 24, 20, 20, 28, 28, 24, 18, 16, 18, 20, 22, 22,
            18, 18, 16, 18, 20, 18, 28, 30, 30, 26, 26, 24, 26, 24, 26, 28, 26, 26, 18, 18, 16, 18, 16, 18, 20, 22, 22, 18, 18, 20, 22, 24, 22, 20, 18, 18, 22, 22,
            24, 22, 20, 22, 30, 30, 32, 30, 32, 30, 28, 26, 26, 30, 30, 28, 30, 32, 30, 28, 26, 26, 30, 30, 32, 30, 28, 30, 20, 18, 18, 22, 22, 24, 22, 24, 22, 20,
            22, 22, 26, 26, 24, 26, 24, 26, 28, 30, 30, 26, 26, 28, 8, 0, 8, 8, 8, 16, 4, 0, 4, 8, 12, 12, 4, 4, 0, 4, 8, 4, 12, 12, 16, 12, 16, 12, 8, 4, 4, 12, 12,
            8, 2, 0, 2, 4, 6, 6, 2, 2, 0, 2, 4, 2, 12, 14, 14, 10, 10, 8, 10, 8, 10, 12, 10, 10, 2, 2, 0, 2, 0, 2, 4, 6, 6, 2, 2, 4, 6, 8, 6, 4, 2, 2, 6, 6, 8, 6, 4,
            6, 14, 14, 16, 14, 16, 14, 12, 10, 10, 14, 14, 12, 14, 16, 14, 12, 10, 10, 14, 14, 16, 14, 12, 14, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 6, 6, 10, 10, 8, 10, 8,
            10, 12, 14, 14, 10, 10, 12, 24, 16, 24, 24, 24, 32, 20, 16, 20, 24, 28, 28, 20, 20, 16, 20, 24, 20, 28, 28, 32, 28, 32, 28, 24, 20, 20, 28, 28, 24, 18, 16,
            18, 20, 22, 22, 18, 18, 16, 18, 20, 18, 28, 30, 30, 26, 26, 24, 26, 24, 26, 28, 26, 26, 18, 18, 16, 18, 16, 18, 20, 22, 22, 18, 18, 20, 22, 24, 22, 20, 18,
            18, 22, 22, 24, 22, 20, 22, 30, 30, 32, 30, 32, 30, 28, 26, 26, 30, 30, 28, 30, 32, 30, 28, 26, 26, 30, 30, 32, 30, 28, 30, 20, 18, 18, 22, 22, 24, 22, 24,
            22, 20, 22, 22, 26, 26, 24, 26, 24, 26, 28, 30, 30, 26, 26, 28
        };

        /// <summary>
        /// Upper-left and lower-right quadrants (Slash "/" quadrants).
        /// </summary>
        private static int[] slashVerticesY =
        { 
            0, 0, 0, 16, 16, 16, 32, 32, 32, 0, 8, 8, 8, 16, 8, 8, 12, 12, 4, 4, 0, 4, 0, 4, 8, 4, 4, 12, 16, 12, 8, 4, 4, 12, 12, 16, 12, 8, 12, 12, 14, 14, 10, 10,
            8, 10, 8, 10, 12, 10, 10, 2, 2, 0, 2, 0, 2, 4, 6, 6, 2, 2, 4, 2, 0, 2, 4, 6, 6, 2, 2, 0, 2, 4, 2, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 6, 6, 14, 16, 14, 12, 10,
            10, 14, 14, 16, 14, 12, 14, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 6, 6, 14, 14, 16, 14, 16, 14, 12, 10, 10, 14, 14, 12, 10, 8, 10, 12, 14, 14, 10, 10, 8, 10, 12,
            10, 0, 8, 8, 8, 16, 8, 8, 12, 12, 4, 4, 0, 4, 0, 4, 8, 4, 4, 12, 16, 12, 8, 4, 4, 12, 12, 16, 12, 8, 12, 12, 14, 14, 10, 10, 8, 10, 8, 10, 12, 10, 10, 2,
            2, 0, 2, 0, 2, 4, 6, 6, 2, 2, 4, 2, 0, 2, 4, 6, 6, 2, 2, 0, 2, 4, 2, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 6, 6, 14, 16, 14, 12, 10, 10, 14, 14, 16, 14, 12, 14,
            4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 6, 6, 14, 14, 16, 14, 16, 14, 12, 10, 10, 14, 14, 12, 10, 8, 10, 12, 14, 14, 10, 10, 8, 10, 12, 10, 16, 24, 24, 24, 32, 24,
            24, 28, 28, 20, 20, 16, 20, 16, 20, 24, 20, 20, 28, 32, 28, 24, 20, 20, 28, 28, 32, 28, 24, 28, 28, 30, 30, 26, 26, 24, 26, 24, 26, 28, 26, 26, 18, 18, 16,
            18, 16, 18, 20, 22, 22, 18, 18, 20, 18, 16, 18, 20, 22, 22, 18, 18, 16, 18, 20, 18, 20, 18, 18, 22, 22, 24, 22, 24, 22, 20, 22, 22, 30, 32, 30, 28, 26, 26,
            30, 30, 32, 30, 28, 30, 20, 18, 18, 22, 22, 24, 22, 24, 22, 20, 22, 22, 30, 30, 32, 30, 32, 30, 28, 26, 26, 30, 30, 28, 26, 24, 26, 28, 30, 30, 26, 26, 24,
            26, 28, 26, 16, 24, 24, 24, 32, 24, 24, 28, 28, 20, 20, 16, 20, 16, 20, 24, 20, 20, 28, 32, 28, 24, 20, 20, 28, 28, 32, 28, 24, 28, 28, 30, 30, 26, 26, 24,
            26, 24, 26, 28, 26, 26, 18, 18, 16, 18, 16, 18, 20, 22, 22, 18, 18, 20, 18, 16, 18, 20, 22, 22, 18, 18, 16, 18, 20, 18, 20, 18, 18, 22, 22, 24, 22, 24, 22,
            20, 22, 22, 30, 32, 30, 28, 26, 26, 30, 30, 32, 30, 28, 30, 20, 18, 18, 22, 22, 24, 22, 24, 22, 20, 22, 22, 30, 30, 32, 30, 32, 30, 28, 26, 26, 30, 30, 28,
            26, 24, 26, 28, 30, 30, 26, 26, 24, 26, 28, 26
        };

        /// <summary>
        /// Mutex used.
        /// </summary>
        private static object token = new object();

        /// <summary>
        /// Slash quadrant mapping.
        /// </summary>
        private static int[][] slashMapping;

        /// <summary>
        /// Backslash quadrant mapping.
        /// </summary>
        private static int[][] backslashMapping;

        /// <summary>
        /// Elevation map used.
        /// </summary>
        private IElevationMap elevationMap;

        /// <summary>
        /// Tile serializer used.
        /// </summary>
        private IDemTileSerializer tileSerializer;

        /// <summary>
        /// Initializes a new instance of the ToastDemTileCreator class.
        /// </summary>
        /// <param name="map">
        /// Color map.
        /// </param>
        /// <param name="serializer">
        /// Tile serializer.
        /// </param>
        public ToastDemTileCreator(IElevationMap map, IDemTileSerializer serializer)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            this.elevationMap = map;
            this.tileSerializer = serializer;
            this.ProjectionType = ProjectionTypes.Toast;
        }

        /// <summary>
        /// Gets the ProjectionType.
        /// </summary>
        public ProjectionTypes ProjectionType { get; private set; }

        /// <summary>
        /// Creates the tile specified by level.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// X tile coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y tile coordinate.
        /// </param>
        public void Create(int level, int tileX, int tileY)
        {
            // Pick the set of vertices where elevation will be computed.
            int[] verticesX = backslashVerticesX;
            int[] verticesY = backslashVerticesY;
            if (IsSlashQuadrant(level, tileX, tileY))
            {
                verticesX = slashVerticesX;
                verticesY = slashVerticesY;
            }

            // Compute required elevations.
            short[] h = new short[verticesX.Length];
            Vector2d pt = new Vector2d();
            OctTileMap tileMap = new OctTileMap(level, tileX, tileY);
            for (int i = 0; i < verticesX.Length; i++)
            {
                pt.X = ((double)verticesX[i]) / VertexInterval;
                pt.Y = ((double)verticesY[i]) / VertexInterval;
                Vector2d v = tileMap.PointToRaDec(pt);
                h[i] = this.elevationMap.GetElevation(v.X - 180.0, v.Y);
            }

            // Save elevations.
            this.tileSerializer.Serialize(h, level, tileX, tileY);
        }

        /// <summary>
        /// Aggregates lower level DEM tiles to construct upper level tiles.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// X coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y coordinate.
        /// </param>
        public void CreateParent(int level, int tileX, int tileY)
        {
            int level1 = checked(level + 1);
            int x1 = checked(2 * tileX);
            int y1 = checked(2 * tileY);

            short[][] h = new short[4][];
            h[0] = this.tileSerializer.Deserialize(level1, x1, y1);
            h[1] = this.tileSerializer.Deserialize(level1, x1 + 1, y1);
            h[2] = this.tileSerializer.Deserialize(level1, x1, y1 + 1);
            h[3] = this.tileSerializer.Deserialize(level1, x1 + 1, y1 + 1);

            int[][] mapping = ToastDemTileCreator.GetMapping(level, tileX, tileY);
            short[] hp = new short[mapping.Length];
            for (int k = 0; k < mapping.Length; k++)
            {
                int[] m = mapping[k];
                int region = m[0];
                int position = m[1];
                if (h[region] != null)
                {
                    hp[k] = h[region][position];
                }
                else
                {
                    hp[k] = 0;
                }
            }

            this.tileSerializer.Serialize(hp, level, tileX, tileY);
        }

        /// <summary>
        /// Checks if this is is slash quadrant.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// X coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y coordinate.
        /// </param>
        /// <returns>
        /// True if this is slash quadrant, False otherwise.
        /// </returns>
        private static bool IsSlashQuadrant(int level, int tileX, int tileY)
        {
            if (level < 1)
            {
                return true;
            }

            int n = 1 << (level - 1);
            return (tileX < n) ? (tileY < n) : (tileY >= n);
        }

        /// <summary>
        /// Generates slash quadrant mapping.
        /// </summary>
        /// <returns>
        /// Mapping generated.
        /// </returns>
        private static int[][] GetSlashMapping()
        {
            if (ToastDemTileCreator.slashMapping == null)
            {
                lock (token)
                {
                    ToastDemTileCreator.slashMapping = ToastDemTileCreator.GenerateMapping(slashVerticesX, slashVerticesY);
                }
            }

            return ToastDemTileCreator.slashMapping;
        }

        /// <summary>
        /// Generates backslash quadrant mapping.
        /// </summary>
        /// <returns>
        /// Mapping generated.
        /// </returns>
        private static int[][] GetBackslashMapping()
        {
            if (ToastDemTileCreator.backslashMapping == null)
            {
                lock (token)
                {
                    ToastDemTileCreator.backslashMapping = ToastDemTileCreator.GenerateMapping(backslashVerticesX, backslashVerticesY);
                }
            }

            return ToastDemTileCreator.backslashMapping;
        }

        /// <summary>
        /// Generates mapping for the given vertices.
        /// </summary>
        /// <param name="vx">
        /// X coordinate vertices.
        /// </param>
        /// <param name="vy">
        /// Y coordinate vertices.
        /// </param>
        /// <returns>
        /// Mapping generated.
        /// </returns>
        private static int[][] GenerateMapping(int[] vx, int[] vy)
        {
            int[][] mapping = new int[vx.Length][];
            for (int n = 0; n < vx.Length; n++)
            {
                int ix = vx[n];
                int iy = vy[n];
                int region = 0;
                if (ix > 16)
                {
                    region += 1;
                    ix = ix - 16;
                }

                if (iy > 16)
                {
                    region += 2;
                    iy = iy - 16;
                }

                ix = 2 * ix;
                iy = 2 * iy;
                int pos = Enumerable.Range(0, 513).Select(i => i).Where(i => (vx[i] == ix && vy[i] == iy)).First();
                mapping[n] = new int[] { region, pos };
            }

            return mapping;
        }

        /// <summary>
        /// Computes mapping for a given level, X and Y based on the quadrant that they belong to.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// X coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y coordinate.
        /// </param>
        /// <returns>
        /// Mapping generated.
        /// </returns>
        private static int[][] GetMapping(int level, int tileX, int tileY)
        {
            if (ToastDemTileCreator.IsSlashQuadrant(level, tileX, tileY))
            {
                return ToastDemTileCreator.GetSlashMapping();
            }
            else
            {
                return ToastDemTileCreator.GetBackslashMapping();
            }
        }
    }
}