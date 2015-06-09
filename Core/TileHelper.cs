//---------------------------------------------------------------------
// <copyright file="TileHelper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// This a helper class for creating a tile.
    /// </summary>
    public static class TileHelper
    {
        /// <summary>
        /// Gets the default output directory for generation of pyramids.
        /// </summary>
        public static string DefaultOutputDirectory
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, Constants.DefaultOutputPath, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            }
        }

        /// <summary>
        /// Gets the template used to store image tiles in the pyramid.
        /// </summary>
        /// <param name="path">
        /// Base path.
        /// </param>
        /// <returns>
        /// Full path to the image tile file.
        /// </returns>
        public static string GetDefaultImageTilePathTemplate(string path)
        {
            return Path.Combine(path, @"Pyramid\{0}\{1}\L{0}X{1}Y{2}.{3}");
        }

        /// <summary>
        /// Creates a 256x256 bitmap image for the specified tile from the given 256x256 array of color pixels.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// Tile X coordinate.
        /// </param>
        /// <param name="tileY">
        /// Tile Y coordinate.
        /// </param>
        /// <param name="values">
        /// Color pixels.
        /// </param>
        /// <param name="serializer">
        /// Serializer to invoke in order to persist the bitmap image.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This is done in order to gain performance benefits while handling bitmaps.")]
        public static void ToBitmap(int level, int tileX, int tileY, int[] values, IImageTileSerializer serializer)
        {
            TileHelper.ToBitmap(level, tileX, tileY, values, serializer, null);
        }

        /// <summary>
        /// Creates a 256x256 bitmap image for the specified tile from the given 256x256 array of color pixels.
        /// </summary>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// Tile X coordinate.
        /// </param>
        /// <param name="tileY">
        /// Tile Y coordinate.
        /// </param>
        /// <param name="values">
        /// Color pixels.
        /// </param>
        /// <param name="serializer">
        /// Serializer to invoke in order to persist the bitmap image.
        /// </param>
        /// <param name="referenceImageSerializer">
        /// Reference image tile serializer.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This is done in order to gain performance benefits while handling bitmaps.")]
        public static void ToBitmap(int level, int tileX, int tileY, int[] values, IImageTileSerializer serializer, IImageTileSerializer referenceImageSerializer)
        {
            byte[] referenceImage = null;
            using (Bitmap image = new Bitmap(Constants.TileSize, Constants.TileSize, PixelFormat.Format32bppArgb))
            {
                // Set the data row by row
                Rectangle dimension = new Rectangle(0, 0, Constants.TileSize, Constants.TileSize);
                BitmapData imageData = image.LockBits(dimension, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                byte[] rgb = new byte[imageData.Stride];
                int position = -1;

                if (referenceImageSerializer != null)
                {
                    referenceImage = TileHelper.BitmapToBytes(referenceImageSerializer.Deserialize(level, tileX, tileY));
                }

                for (int pixelY = 0; pixelY < Constants.TileSize; pixelY++)
                {
                    Array.Clear(rgb, 0, rgb.Length);
                    int pos = 0;
                    Color referenceColor = Color.White;
                    for (int pixelX = 0; pixelX < Constants.TileSize; pixelX++)
                    {
                        Color color = Color.Transparent;
                        position++;
                        if (referenceImage != null)
                        {
                            if (referenceImage != null &&
                               referenceImage[(position * 4)] == referenceColor.B &&
                               referenceImage[(position * 4) + 1] == referenceColor.G &&
                               referenceImage[(position * 4) + 2] == referenceColor.R)
                            {
                                if (values != null)
                                {
                                    color = Color.FromArgb(values[position]);
                                }
                            }
                        }
                        else
                        {
                            if (values != null)
                            {
                                color = Color.FromArgb(values[position]);
                            }
                        }

                        rgb[pos++] = color.B;
                        rgb[pos++] = color.G;
                        rgb[pos++] = color.R;
                        rgb[pos++] = color.A;
                    }

                    IntPtr p = new IntPtr(imageData.Scan0.ToInt64() + (pixelY * imageData.Stride));
                    System.Runtime.InteropServices.Marshal.Copy(rgb, 0, p, rgb.Length);
                }

                image.UnlockBits(imageData);
                if (serializer != null)
                {
                    serializer.Serialize(image, level, tileX, tileY);
                }

                imageData = null;
                rgb = null;
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
        /// <param name="tileSerializer">
        /// Serializer to invoke in order to persist the bitmap image.
        /// </param>
        public static void CreateParent(int level, int tileX, int tileY, IImageTileSerializer tileSerializer)
        {
            if (tileSerializer == null)
            {
                throw new ArgumentNullException("tileSerializer");
            }

            int level1 = checked(level + 1);
            int x1 = checked(2 * tileX);
            int y1 = checked(2 * tileY);
            using (Bitmap bmp = new Bitmap(2 * Constants.TileSize, 2 * Constants.TileSize))
            using (Graphics g = Graphics.FromImage(bmp))
            using (Bitmap b00 = tileSerializer.Deserialize(level1, x1, y1))
            using (Bitmap b10 = tileSerializer.Deserialize(level1, x1 + 1, y1))
            using (Bitmap b01 = tileSerializer.Deserialize(level1, x1, y1 + 1))
            using (Bitmap b11 = tileSerializer.Deserialize(level1, x1 + 1, y1 + 1))
            {
                // Default is to display color indicating no data.
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
                    using (Bitmap b = ResizeBitmap(bmp, Constants.TileSize, Constants.TileSize))
                    {
                        tileSerializer.Serialize(b, level, tileX, tileY);
                    }
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
        public static Bitmap ResizeBitmap(Image bitmap, int width, int height)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException("bitmap");
            }

            Bitmap newBitmap = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(newBitmap))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.CompositingQuality = CompositingQuality.HighQuality;
                gr.InterpolationMode = InterpolationMode.Bilinear;

                gr.DrawImage(bitmap, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
            }

            return newBitmap;
        }

        /// <summary>
        /// This function is used to generate thumbnail image of the bitmap.
        /// </summary>
        /// <param name="inputFile">
        /// Full path of the input file for which the thumbnail has to be created.
        /// </param>
        /// <param name="width">
        /// The width, in pixels, of the requested thumbnail image.
        /// </param>
        /// <param name="height">
        /// The height, in pixels, of the requested thumbnail image.
        /// </param>
        /// <param name="outputFilename">
        /// Filename of the thumbnail to be saved.
        /// </param>
        /// <param name="format">
        /// Format of the thumbnail image.
        /// </param>
        public static void GenerateThumbnail(string inputFile, int width, int height, string outputFilename, ImageFormat format)
        {
            if (!string.IsNullOrWhiteSpace(inputFile) && !string.IsNullOrWhiteSpace(outputFilename) && File.Exists(inputFile))
            {
                using (Bitmap bitmap = new Bitmap(inputFile))
                {
                    using (Bitmap thumbnail = MakeThumbnail(bitmap, width, height))
                    {
                        thumbnail.Save(outputFilename, format);
                    }
                }
            }
        }

        /// <summary>
        /// Compute the maximum level of detail that this image supports.
        /// </summary>
        /// <param name="imageHeight">Image height (in Pixels).</param>
        /// <param name="imageWidth">Image width (in Pixels).</param>
        /// <param name="inputBoundary">Input boundary.</param>
        /// <returns>Max level for the input image</returns>
        public static int CalculateMaximumLevel(long imageHeight, long imageWidth, Boundary inputBoundary)
        {
            if (inputBoundary == null)
            {
                throw new ArgumentNullException("inputBoundary");
            }

            // Approach:
            //   1. Based on the input coordinates, calculate how much pixel represent each latitude of the given input image.
            //   2. Then image height for the whole world is (Pixels Representing an latitude) * 180. 
            //   3. Once we have the total image height, then we can calculate the Number of levels as 
            ////      “MaxLevel = Math.Ceiling(Math.Log((image height for the whole world) / 256 ) / Math.Log(2.0))”

            double actualImageHeight = 180 * (imageHeight / (inputBoundary.Bottom - inputBoundary.Top));
            double actualImageWidth = 360 * (imageWidth / (inputBoundary.Right - inputBoundary.Left));

            // If the image height is less than 256 , max level is 0.
            int maxLevelHeight = (actualImageHeight >= 256) ? (int)Math.Ceiling(Math.Log(actualImageHeight / Constants.TileSize) / Math.Log(2.0)) : 0;
            int maxLevelWidth = (actualImageWidth >= 256) ? (int)Math.Ceiling(Math.Log(actualImageWidth / Constants.TileSize) / Math.Log(2.0)) : 0;

            return (maxLevelHeight > maxLevelWidth) ? maxLevelHeight : maxLevelWidth;
        }

        /// <summary>
        /// Converts Bitmaps to Bytes.
        /// </summary>
        /// <param name="bmp">The Bitmap to be converted.</param>
        /// <returns>The Byte array.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This is done in order to gain performance benefits while handling bitmaps.")]
        public static byte[] BitmapToBytes(Bitmap bmp)
        {
            if (bmp == null)
            {
                return null;
            }

            byte[] bmpBytes = null;
            try
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(new Point(), bmp.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

                // Number of bytes in the bitmap.
                int byteCount = bitmapData.Stride * bmp.Height;
                bmpBytes = new byte[byteCount];

                // Copy the locked bytes from memory.
                Marshal.Copy(bitmapData.Scan0, bmpBytes, 0, byteCount);
                bmp.UnlockBits(bitmapData);
            }
            finally
            {
                bmp.Dispose();
                bmp = null;
            }

            return bmpBytes;
        }

        /// <summary>
        /// This function is used to create the thumbnail of the input image.
        /// </summary>
        /// <param name="input">
        /// Input Image.
        /// </param>
        /// <param name="width">
        /// The width, in pixels, of the requested thumbnail image.
        /// </param>
        /// <param name="height">
        /// The height, in pixels, of the requested thumbnail image.
        /// </param>
        /// <returns>
        /// Thumbnail image.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore any exception which occurs.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The bitmap object is being returned to caller. Cannot dispose here.")]
        private static Bitmap MakeThumbnail(Bitmap input, int width, int height)
        {
            Bitmap image = null;
            if (input != null)
            {
                image = new Bitmap(width, height);

                Graphics graphics = Graphics.FromImage(image);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.Clear(Color.Black);

                int originalWidth = input.Width;
                int originalHeight = input.Height;
                double aspectRatioThumbnail = ((double)width) / ((double)height);
                double aspectRatioInput = ((double)originalWidth) / ((double)originalHeight);
                int revisedWidth = originalWidth;
                int revisedHeight = originalHeight;
                if (aspectRatioInput < aspectRatioThumbnail)
                {
                    revisedHeight = (int)(((double)originalWidth) / aspectRatioThumbnail);
                }
                else
                {
                    revisedHeight = (int)(originalHeight * aspectRatioInput);
                }

                int offset = 0;
                if (originalHeight > revisedHeight)
                {
                    offset = (originalHeight - revisedHeight) / 2;
                }

                Rectangle destinationRect = new Rectangle(0, 0, width, height);
                Rectangle sourceRect = new Rectangle(0, offset, revisedWidth, revisedHeight);

                try
                {
                    graphics.DrawImage(input, destinationRect, sourceRect, GraphicsUnit.Pixel);
                }
                catch
                {
                    // Ignore any exception.
                }

                graphics.Dispose();
            }

            return image;
        }
    }
}
