//-----------------------------------------------------------------------
// <copyright file="ToastHelper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Helper class which compute toast tile coordinates for the given boundary and level.
    /// </summary>
    public static class ToastHelper
    {
        /// <summary>
        /// Compute tile coordinates for toast projection.
        /// </summary>
        /// <param name="preferredRegion">Region of interest for which the tile coordinates has to be computed.</param>
        /// <param name="maxLevel">Maximum level.</param>
        /// <returns>Tile coordinates for each level.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "As per design")]
        public static Dictionary<int, List<Tile>> ComputeTileCoordinates(Boundary preferredRegion, int maxLevel)
        {
            var tiles = new Dictionary<int, List<Tile>>();
            tiles.Add(0, new List<Tile> { new Tile(0, 0) });

            for (int level = 1; level <= maxLevel; level++)
            {
                tiles = ComputeTiles(tiles, preferredRegion, level);
            }

            return tiles;
        }

        /// <summary>
        /// Compute tile coordinates for a specified level. Computation is done by using tiles from the previous level.
        /// </summary>
        /// <param name="tiles">List of tiles already computed for previous levels.</param>
        /// <param name="preferredRegion">Region of interest.</param>
        /// <param name="level">Current level for which the tile coorinates has to be computed.</param>
        private static Dictionary<int, List<Tile>> ComputeTiles(Dictionary<int, List<Tile>> tiles, Boundary preferredRegion, int level)
        {
            if (level == 0)
            {
                return tiles;
            }

            List<Tile> points = tiles[level - 1];
            tiles.Add(level, new List<Tile>());

            int xmin, xmax, ymin, ymax;

            foreach (Tile point in points)
            {
                xmin = (int)point.X * 2;
                xmax = xmin + 1;
                ymin = (int)point.Y * 2;
                ymax = ymin + 1;

                for (int y = ymin; y <= ymax; y++)
                {
                    for (int x = xmin; x <= xmax; x++)
                    {
                        if (ToastHelper.IsDesiredTile(preferredRegion, level, x, y))
                        {
                            tiles[level].Add(new Tile(x, y));
                        }
                    }
                }
            }

            return tiles;
        }

        /// <summary>
        /// Check whether a given x,y tile coodinate belongs/overlap with the prefered regions' coordinates.
        /// </summary>
        /// <param name="preferredRegion">Region of preference.</param>
        /// <param name="level">Level at which the tile coordinate has to be examined.</param>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <returns>Boolean indicating the tile coordinates belongs/overlaps with the region of interest.</returns>
        private static bool IsDesiredTile(Boundary preferredRegion, int level, int tileX, int tileY)
        {
            OctTileMap octMap = new OctTileMap(level, tileX, tileY);
            Boundary tileBound = new Boundary(octMap.RaMin, octMap.DecMin, octMap.RaMax, octMap.DecMax);
            return IsBoundOrOverlap(tileBound, preferredRegion);
        }

        /// <summary>
        /// Checks if two bounding coordinates overlaps each other.
        /// </summary>
        /// <param name="region1">First region to be compared.</param>
        /// <param name="region2">Second region to be compared.</param>
        /// <returns>True/false indicating the overlap.</returns>
        private static bool IsBoundOrOverlap(Boundary region1, Boundary region2)
        {
            // Complete containment check, either region1 might engulf region2 or vice versa.
            // Check if region1 bounds point region2 OR region2 bounds region1
            if (region1.Left <= region2.Left && region1.Right >= region2.Right && region1.Top <= region2.Top && region1.Bottom >= region2.Bottom)
            {
                // Region1 bounds/surrounds region2
                return true;
            }
            else if (region2.Left <= region1.Left && region2.Right >= region1.Right && region2.Top <= region1.Top && region2.Bottom >= region1.Bottom)
            {
                // Region2 bounds/surrounds region1
                return true;
            }

            // Overlap check.
            if (((region2.Left >= region1.Left && region2.Left <= region1.Right) || (region2.Right >= region1.Left && region2.Right <= region1.Right)) &&
                ((region2.Top >= region1.Top && region2.Top <= region1.Bottom) || (region2.Bottom >= region1.Top && region2.Bottom <= region1.Bottom)))
            {
                // Now region2 coordinates overlaps(some coordinates of region2 lies inside of region1) with region1
                return true;
            }

            if (((region1.Left >= region2.Left && region1.Left <= region2.Right) || (region1.Right >= region2.Left && region1.Right <= region2.Right)) &&
               ((region1.Top >= region2.Top && region1.Top <= region2.Bottom) || (region1.Bottom >= region2.Top && region1.Bottom <= region2.Bottom)))
            {
                // Now region1 coordinates overlaps(some region1 coordinates lies inside of region2) with region2
                return true;
            }

            return false;
        }
    }
}
