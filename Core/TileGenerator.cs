//---------------------------------------------------------------------
// <copyright file="TileGenerator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Encapsulates the process of creating base image tiles and filling up the pyramid.
    /// </summary>
    public class TileGenerator
    {
        /// <summary>
        /// TileCreator object used to create tiles.
        /// </summary>
        private ITileCreator tileCreator;

        /// <summary>
        /// Tile coordinates for a level.
        /// </summary>
        private IDictionary<int, List<Tile>> toastProjectionCoordinates;

        /// <summary>
        /// Stores the number of processed tiles for this the level in context.
        /// </summary>
        private long tilesProcessed;

        /// <summary>
        /// Initializes a new instance of the TileGenerator class.
        /// </summary>
        /// <param name="creator">
        /// TileCreator object to be used.
        /// </param>
        public TileGenerator(ITileCreator creator)
        {
            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }

            this.ParallelOptions = new ParallelOptions();
            this.tileCreator = creator;
        }

        /// <summary>
        /// Gets or sets Parallel execution options.
        /// </summary>
        public ParallelOptions ParallelOptions { get; set; }

        /// <summary>
        /// Gets the number of processed tiles for the level in context.
        /// </summary>
        public long TilesProcessed
        {
            get
            {
                return this.tilesProcessed;
            }
        }

        /// <summary>
        /// Gets the level number which is being currently processed.
        /// </summary>
        public int CurrentLevel { get; private set; }

        /// <summary>
        /// Generates all the tiles at the level specified and fills up the pyramid above for a given region of the world.
        /// </summary>
        /// <param name="level">
        /// Base level of zoom.
        /// </param>
        /// <param name="boundary">
        /// Bounding coordinates of the region.
        /// </param>
        /// <remarks>
        /// This API should be used only for a sparse region of the world.
        /// The overhead involved in computing the relevant tiles is negligible for a sparse region.
        /// </remarks>
        public void Generate(int level, Boundary boundary)
        {
            this.tilesProcessed = 0;
            this.BuildLevel(level, boundary);
            this.BuildParentLevels(level, boundary);
        }

        /// <summary>
        /// Generates all the tiles at the level specified and fills up the pyramid above 
        ///     within the given range of X and Y tile coordinates.
        /// </summary>
        /// <param name="level">
        /// Base level of zoom.
        /// </param>
        /// <param name="tileXStart">
        /// X tile Start.
        /// </param>
        /// <param name="tileXEnd">
        /// X tile End.
        /// </param>
        /// <param name="tileYStart">
        /// Y tile Start.
        /// </param>
        /// <param name="tileYEnd">
        /// Y tile End.
        /// </param>
        /// <param name="depth">
        /// NUmber of levels we need to generate tile.
        /// </param>
        public void Generate(int level, int tileXStart, int tileXEnd, int tileYStart, int tileYEnd, int depth)
        {
            this.tilesProcessed = 0;
            this.BuildLevel(level, tileXStart, tileXEnd, tileYStart, tileYEnd);
            this.BuildParentLevels(level, tileXStart, tileXEnd, tileYStart, tileYEnd, depth);
        }

        /// <summary>
        /// Generates all the tiles at the level specified and fills up the pyramid above. 
        /// </summary>
        /// <param name="level">
        /// Base level of zoom.
        /// </param>
        public void Generate(int level)
        {
            this.tilesProcessed = 0;
            this.BuildLevel(level);
            this.BuildParentLevels(level);
        }

        /// <summary>
        /// Generates all the tiles at the level specified for a given region of the world.
        /// </summary>
        /// <param name="level">
        /// Base level of zoom.
        /// </param>
        /// <param name="boundary">
        /// Bounding coordinates of the region.
        /// </param>
        /// <remarks>
        /// This API should be used only for a sparse region of the world.
        /// The overhead involved in computing the relevant tiles is negligible for a sparse region.
        /// </remarks>
        public void BuildLevel(int level, Boundary boundary)
        {
            if (boundary == null)
            {
                throw new ArgumentNullException("boundary");
            }

            Trace.TraceInformation("{0}: Building image at level {1}..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture), level);

            CurrentLevel = level;

            if (this.tileCreator.ProjectionType == ProjectionTypes.Toast)
            {
                this.toastProjectionCoordinates = ToastHelper.ComputeTileCoordinates(boundary, level);
                Parallel.ForEach(
                    this.toastProjectionCoordinates[level],
                    this.ParallelOptions,
                    tile =>
                    {
                        this.tileCreator.Create(level, tile.X, tile.Y);
                        Interlocked.Increment(ref this.tilesProcessed);
                        this.ParallelOptions.CancellationToken.ThrowIfCancellationRequested();
                    });
            }
            else
            {
                int tileXMin = Helper.GetMercatorXTileFromLongitude(boundary.Left, level);
                int tileXMax = Helper.GetMercatorXTileFromLongitude(boundary.Right, level);
                int tileYMin = Helper.GetMercatorYTileFromLatitude(boundary.Bottom, level);
                int tileYMax = Helper.GetMercatorYTileFromLatitude(boundary.Top, level);

                // If both xmin(ymin) and xmax(ymax) are equal,
                // the loop should run atleast for ONE time, Hence incrementing the Xmax(Ymax) by 1
                tileXMax = (tileXMin == tileXMax) ? tileXMax + 1 : tileXMax;
                tileYMax = (tileYMin == tileYMax) ? tileYMax + 1 : tileYMax;

                tileXMax = (int)Math.Min(tileXMax + 1, Math.Pow(2, level));
                tileYMax = (int)Math.Min(tileYMax + 1, Math.Pow(2, level));

                Parallel.For(
                    tileYMin,
                    tileYMax,
                    this.ParallelOptions,
                    y =>
                    {
                        for (int x = tileXMin; x < tileXMax; x++)
                        {
                            this.tileCreator.Create(level, x, y);
                            Interlocked.Increment(ref this.tilesProcessed);
                            this.ParallelOptions.CancellationToken.ThrowIfCancellationRequested();
                        }
                    });
            }
        }

        /// <summary>
        /// Generates all the tiles at the level specified within the given range of X and Y tile coordinates.
        /// </summary>
        /// <param name="level">
        /// Base level of zoom.
        /// </param>
        /// <param name="tileXStart">
        /// X tile Start.
        /// </param>
        /// <param name="tileXEnd">
        /// X tile End.
        /// </param>
        /// <param name="tileYStart">
        /// Y tile Start.
        /// </param>
        /// <param name="tileYEnd">
        /// Y tile End.
        /// </param>
        public void BuildLevel(int level, int tileXStart, int tileXEnd, int tileYStart, int tileYEnd)
        {
            Trace.TraceInformation("{0}: Building image at level {1}..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture), level);

            CurrentLevel = level;

            tileXStart = (int)Math.Max(0, tileXStart);
            tileYStart = (int)Math.Max(0, tileYStart);

            tileXEnd = (int)Math.Min(checked(tileXEnd + 1), Math.Pow(2, level));
            tileYEnd = (int)Math.Min(checked(tileYEnd + 1), Math.Pow(2, level));

            Parallel.For(
                    tileYStart,
                    tileYEnd,
                    this.ParallelOptions,
                    y =>
                    {
                        for (int x = tileXStart; x < tileXEnd; x++)
                        {
                            this.tileCreator.Create(level, x, y);
                            Interlocked.Increment(ref this.tilesProcessed);
                            this.ParallelOptions.CancellationToken.ThrowIfCancellationRequested();
                        }
                    });
        }

        /// <summary>
        /// Generates all the tiles at the level specified. 
        /// </summary>
        /// <param name="level">
        /// Base level of zoom.
        /// </param>
        public void BuildLevel(int level)
        {
            Trace.TraceInformation("{0}: Building image at level {1}..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture), level);

            CurrentLevel = level;

            int tileYMax = (int)Math.Pow(2, level);
            int tileXMax = (int)Math.Pow(2, level);
            Parallel.For(
                    0,
                    tileYMax,
                    this.ParallelOptions,
                    y =>
                    {
                        for (int x = 0; x < tileXMax; x++)
                        {
                            this.tileCreator.Create(level, x, y);
                            Interlocked.Increment(ref this.tilesProcessed);
                            this.ParallelOptions.CancellationToken.ThrowIfCancellationRequested();
                        }
                    });
        }

        /// <summary>
        /// Fills up the pyramid of tiles above the specified level. 
        /// </summary>
        /// <param name="level">
        /// Base level of zoom.
        /// </param>
        /// <param name="boundary">
        /// Bounding coordinates of the region.
        /// </param>
        public void BuildParentLevels(int level, Boundary boundary)
        {
            if (boundary == null)
            {
                throw new ArgumentNullException("boundary");
            }

            if (this.tileCreator.ProjectionType == ProjectionTypes.Toast)
            {
                this.toastProjectionCoordinates = ToastHelper.ComputeTileCoordinates(boundary, level);
                foreach (int k in Enumerable.Range(1, level).Select(i => level - i))
                {
                    Trace.TraceInformation("{0}: Filling up pyramid at level {1}..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture), k);

                    CurrentLevel = k;

                    Parallel.ForEach(
                        this.toastProjectionCoordinates[k],
                        this.ParallelOptions,
                        tile =>
                        {
                            this.tileCreator.CreateParent(k, tile.X, tile.Y);
                            Interlocked.Increment(ref this.tilesProcessed);
                            this.ParallelOptions.CancellationToken.ThrowIfCancellationRequested();
                        });
                }
            }
            else
            {
                int tileXMin, tileXMax, tileYMin, tileYMax;
                foreach (int k in Enumerable.Range(1, level).Select(i => level - i))
                {
                    CurrentLevel = k;

                    tileXMin = Helper.GetMercatorXTileFromLongitude(boundary.Left, k);
                    tileXMax = Helper.GetMercatorXTileFromLongitude(boundary.Right, k);
                    tileYMin = Helper.GetMercatorYTileFromLatitude(boundary.Bottom, k);
                    tileYMax = Helper.GetMercatorYTileFromLatitude(boundary.Top, k);

                    BuildParentLevels(k, tileXMin, tileXMax, tileYMin, tileYMax);
                }
            }
        }

        /// <summary>
        /// Fills up the pyramid of tiles above the specified level within the given range of X and Y tile coordinates.
        /// </summary>
        /// <param name="level">
        /// Base level of zoom.
        /// </param>
        /// <param name="tileXStart">
        /// X tile Start.
        /// </param>
        /// <param name="tileXEnd">
        /// X tile End.
        /// </param>
        /// <param name="tileYStart">
        /// Y tile Start.
        /// </param>
        /// <param name="tileYEnd">
        /// Y tile End.
        /// </param>
        /// <param name="depth">
        /// NUmber of levels we need to generate tile.
        /// </param>
        public void BuildParentLevels(int level, int tileXStart, int tileXEnd, int tileYStart, int tileYEnd, int depth)
        {
            tileXStart = (int)Math.Max(0, tileXStart);
            tileYStart = (int)Math.Max(0, tileYStart);

            int tileXMin, tileXMax, tileYMin, tileYMax;
            int k = checked(level - 1);
            while (k > (level - depth) && k >= 0)
            {
                CurrentLevel = k;

                int levelDiff = (int)Math.Pow(2, level - k);
                tileYMin = (int)tileYStart / levelDiff;
                tileXMin = (int)tileXStart / levelDiff;

                tileYMax = (int)tileYEnd / levelDiff;
                tileXMax = (int)tileXEnd / levelDiff;

                BuildParentLevels(k, tileXMin, tileXMax, tileYMin, tileYMax);

                k--;
            }
        }

        /// <summary>
        /// Fills up the pyramid of tiles above the specified level. 
        /// </summary>
        /// <param name="level">
        /// Base level of zoom.
        /// </param>
        public void BuildParentLevels(int level)
        {
            foreach (int k in Enumerable.Range(1, level).Select(i => level - i))
            {
                int tileYMax = (int)Math.Pow(2, k);
                int tileXMax = (int)Math.Pow(2, k);

                Trace.TraceInformation("{0}: Filling up pyramid at level {1}..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture), k);

                CurrentLevel = k;

                Parallel.For(
                0,
                tileYMax,
                this.ParallelOptions,
                y =>
                {
                    for (int x = 0; x < tileXMax; x++)
                    {
                        this.tileCreator.CreateParent(k, x, y);
                        Interlocked.Increment(ref this.tilesProcessed);
                        this.ParallelOptions.CancellationToken.ThrowIfCancellationRequested();
                    }
                });
            }
        }

        /// <summary>
        /// Fills up the pyramid of tiles for the specified level within the given range of X and Y tile coordinates..
        /// </summary>
        /// <param name="level">
        /// Base level of zoom.
        /// </param>
        /// <param name="tileXMin">
        /// X tile Start.
        /// </param>
        /// <param name="tileXMax">
        /// X tile End.
        /// </param>
        /// <param name="tileYMin">
        /// Y tile Start.
        /// </param>
        /// <param name="tileYMax">
        /// Y tile End.
        /// </param>
        private void BuildParentLevels(int level, int tileXMin, int tileXMax, int tileYMin, int tileYMax)
        {
            Trace.TraceInformation("{0}: Filling up pyramid at level {1}..", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture), level);

            // If both xmin and xmax are equal(as well as for ymin and ymax), 
            // the loop should run atleast for ONE time, Hence incrementing the Xmax by 1(as well as for ymax)
            tileXMax = (tileXMin == tileXMax) ? tileXMax + 1 : tileXMax;
            tileYMax = (tileYMin == tileYMax) ? tileYMax + 1 : tileYMax;

            tileXMax = (int)Math.Min(tileXMax + 1, Math.Pow(2, level));
            tileYMax = (int)Math.Min(tileYMax + 1, Math.Pow(2, level));

            Parallel.For(
                tileYMin,
                tileYMax,
                this.ParallelOptions,
                y =>
                {
                    for (int x = tileXMin; x < tileXMax; x++)
                    {
                        this.tileCreator.CreateParent(level, x, y);
                        Interlocked.Increment(ref this.tilesProcessed);
                        this.ParallelOptions.CancellationToken.ThrowIfCancellationRequested();
                    }
                });
        }
    }
}