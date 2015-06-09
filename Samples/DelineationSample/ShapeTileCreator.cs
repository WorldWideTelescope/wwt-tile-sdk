//-----------------------------------------------------------------------
// <copyright file="ShapeTileCreator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Research.Wwt.Sdk.Core;
using Microsoft.Research.Wwt.Sdk.Utilities;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// Creates tiles for a given level using the Reference image tiles.
    /// </summary>
    public class ShapeTileCreator : ITileCreator
    {
        /// <summary>
        /// Tile serializer object.
        /// </summary>
        private IImageTileSerializer tileSerializer;

        /// <summary>
        /// Color map to be used.
        /// </summary>
        private IColorMap colorMap;

        /// <summary>
        /// Maximum level of the image pyramid.
        /// </summary>
        private int maximumLevel;

        /// <summary>
        /// Initializes a new instance of the ShapeTileCreator class.
        /// </summary>
        /// <param name="serializer">Tile serializer to be used.</param>
        /// <param name="colorMap">Color map.</param>
        /// <param name="type">Desired projection type.</param>
        /// <param name="maximumLevel">Maximum level of the image pyramid.</param>
        public ShapeTileCreator(IImageTileSerializer serializer, IColorMap colorMap, ProjectionTypes type, int maximumLevel)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            if (colorMap == null)
            {
                throw new ArgumentNullException("colorMap");
            }

            this.tileSerializer = serializer;
            this.colorMap = colorMap;
            this.ProjectionType = type;
            this.maximumLevel = maximumLevel;
        }

        /// <summary>
        /// Gets the ProjectionType.
        /// </summary>
        public ProjectionTypes ProjectionType { get; private set; }

        /// <summary>
        /// Creates image tile specified by level.
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
            OctTileMap tileMap = new OctTileMap(level, tileX, tileY);
            bool hasData;

            List<Color> colors = this.MapToColors(tileMap, out hasData);
            if (hasData)
            {
                this.ToBitmap(level, tileX, tileY, colors);
            }
        }

        /// <summary>
        /// Aggregates lower level image tiles to construct upper level tiles.
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
            int level1 = level + 1;
            int x1 = 2 * tileX;
            int y1 = 2 * tileY;
            using (Bitmap bmp = new Bitmap(2 * Constants.TileSize, 2 * Constants.TileSize))
            using (Graphics g = Graphics.FromImage(bmp))
            using (Bitmap b00 = this.tileSerializer.Deserialize(level1, x1, y1) as Bitmap)
            using (Bitmap b10 = this.tileSerializer.Deserialize(level1, x1 + 1, y1) as Bitmap)
            using (Bitmap b01 = this.tileSerializer.Deserialize(level1, x1, y1 + 1) as Bitmap)
            using (Bitmap b11 = this.tileSerializer.Deserialize(level1, x1 + 1, y1 + 1) as Bitmap)
            {
                // Default is to display color for no data.
                g.Clear(Color.Transparent);

                // Draw each quadrant keeping track of number of quadrants actually drawn.
                short drawn = 0;
                if (b00 != null)
                {
                    g.DrawImage(b00, 0, 0);
                    drawn++;
                }

                if (b01 != null)
                {
                    g.DrawImage(b01, 0, Constants.TileSize);
                    drawn++;
                }

                if (b10 != null)
                {
                    g.DrawImage(b10, Constants.TileSize, 0);
                    drawn++;
                }

                if (b11 != null)
                {
                    g.DrawImage(b11, Constants.TileSize, Constants.TileSize);
                    drawn++;
                }

                // If we actually have data, shrink the tile and save it.
                if (drawn > 0)
                {
                    using (Bitmap b = this.ResizeBitmap(bmp, Constants.TileSize, Constants.TileSize))
                    {
                        this.tileSerializer.Serialize(b, level, tileX, tileY);
                    }
                }
            }
        }

       /// <summary>
        /// Maps image pixels to lat-long values.
       /// </summary>
       /// <param name="tileMap">Tile mapper.</param>
       /// <param name="hasData">Boolean indicates whether the coordinates maps to valid color.</param>
       /// <returns>List of color values.</returns>
        private List<Color> MapToColors(OctTileMap tileMap, out bool hasData)
        {
            List<Color> colors = new List<Color>(Constants.TileSize * Constants.TileSize);
            Vector2d point = new Vector2d();
            hasData = false;

            for (int pixelY = 0; pixelY < Constants.TileSize; pixelY++)
            {
                point.Y = ((double)pixelY + 0.5) / Constants.TileSize;
                for (int pixelX = 0; pixelX < Constants.TileSize; pixelX++)
                {
                    point.X = ((double)pixelX + 0.5) / Constants.TileSize;
                    var v = tileMap.PointToRaDec(point);
                    Color color = (this.colorMap.GetColor(v.X, v.Y));
                    if (color != Color.Transparent)
                    {
                        hasData = true;
                    }

                    colors.Add(color);
                }
            }

            return colors;
        }

        /// <summary>
        /// Creates a bitmap image from color pixels.
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
        /// <param name="values">
        /// Color pixels.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This is done in order to gain performance benefits while handling bitmaps.")]
        private void ToBitmap(int level, int tileX, int tileY, List<Color> values)
        {
            Bitmap image = null;
            try
            {
                image = new Bitmap(Constants.TileSize, Constants.TileSize, PixelFormat.Format32bppArgb);

                // Set the data row by row
                Rectangle dimension = new Rectangle(0, 0, Constants.TileSize, Constants.TileSize);
                BitmapData imageData = image.LockBits(dimension, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                Color c;
                byte[] rgb = new byte[imageData.Stride];
                int position = -1;
                for (int iy = 0; iy < Constants.TileSize; iy++)
                {
                    Array.Clear(rgb, 0, rgb.Length);
                    int pos = 0;
                    for (int ix = 0; ix < Constants.TileSize; ix++)
                    {
                        position++;
                        c = values[position];

                        rgb[pos++] = c.B;
                        rgb[pos++] = c.G;
                        rgb[pos++] = c.R;
                        rgb[pos++] = c.A;
                    }

                    IntPtr p = new IntPtr(imageData.Scan0.ToInt64() + iy * imageData.Stride);
                    System.Runtime.InteropServices.Marshal.Copy(rgb, 0, p, rgb.Length);
                }

                image.UnlockBits(imageData);
                this.tileSerializer.Serialize(image, level, tileX, tileY);
            }
            catch
            {
            }
            finally
            {
                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }
            }
        }

        /// <summary>
        /// Smoothens and resizes the given bitmap to specified size.
        /// </summary>
        /// <param name="bitmap">
        /// Bitmap image.
        /// </param>
        /// <param name="width">
        /// New width.
        /// </param>
        /// <param name="height">
        /// New height.
        /// </param>
        /// <returns>
        /// Resized bitmap.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The bitmap object is being returned to caller. Cannot dispose here.")]
        private Bitmap ResizeBitmap(Image bitmap, int width, int height)
        {
            Bitmap newBitmap = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(newBitmap))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;

                gr.DrawImage(bitmap, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
            }

            return newBitmap;
        }
    }
}
