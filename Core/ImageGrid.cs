//-----------------------------------------------------------------------
// <copyright file="ImageGrid.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Represents a grid for an input image.
    /// </summary>
    public class ImageGrid : IGrid
    {
        /// <summary>
        /// Width of the image.
        /// </summary>
        private int imageWidth;

        /// <summary>
        /// Height of the image.
        /// </summary>
        private int imageHeight;

        /// <summary>
        /// Stores red component value of the pixel.
        /// </summary>
        private byte[][] redPixelData;

        /// <summary>
        /// Stores blue component value of the pixel.
        /// </summary>
        private byte[][] bluePixelData;

        /// <summary>
        /// Stores green component value of the pixel.
        /// </summary>
        private byte[][] greenPixelData;

        /// <summary>
        /// Stores alpha component value of the pixel.
        /// </summary>
        private byte[][] alphaPixelData;

        /// <summary>
        /// nu variable for interpolation
        /// </summary>
        private int nu;

        /// <summary>
        /// nv variable for interpolation
        /// </summary>
        private int nv;

        /// <summary>
        /// du variable for interpolation
        /// </summary>
        private double du;

        /// <summary>
        /// dv variable for interpolation
        /// </summary>
        private double dv;

        /// <summary>
        /// If the grid is circular with right edge and left edge of the grid
        /// representing the same boundary
        /// </summary>
        private bool circular;

        /// <summary>
        /// Initializes a new instance of the ImageGrid class.
        /// </summary>
        /// <param name="path">
        /// Image path.
        /// </param>
        /// <param name="isCircular">
        /// If the grid is circular with right edge and left edge of the grid
        /// representing the same boundary
        /// </param>
        public ImageGrid(string path, bool isCircular)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            Bitmap bitmap = null;
            try
            {
                // Create a bitmap object and read the color pixels into the grid.
                bitmap = new Bitmap(path);
                this.Initialize(bitmap);
                circular = isCircular;
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch
            {
                string message = "An error occurred while reading input image file. Check the path and try again.";
                throw new InvalidOperationException(message);
            }
            finally
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the ImageGrid class.
        /// </summary>
        /// <param name="imageFiles">
        /// Array should be in the format of M X N two dimensional array where 
        ///     N = number of images in the horizontal direction
        ///     M = number of images in the vertical direction
        /// </param>
        /// <param name="isCircular">
        /// If the grid is circular with right edge and left edge of the grid
        /// representing the same boundary
        /// </param>
        public ImageGrid(string[,] imageFiles, bool isCircular)
        {
            if (imageFiles == null)
            {
                throw new ArgumentNullException("imageFiles");
            }

            // Initialize width, height and Maximum level properties.
            this.SetImageProperties(imageFiles);
            circular = isCircular;

            // Initialize pixel data array.
            this.redPixelData = new byte[this.imageHeight][];
            this.bluePixelData = new byte[this.imageHeight][];
            this.greenPixelData = new byte[this.imageHeight][];
            this.alphaPixelData = new byte[this.imageHeight][];
            for (int j = 0; j < this.imageHeight; j++)
            {
                this.redPixelData[j] = new byte[this.imageWidth];
                this.bluePixelData[j] = new byte[this.imageWidth];
                this.greenPixelData[j] = new byte[this.imageWidth];
                this.alphaPixelData[j] = new byte[this.imageWidth];
            }

            this.Initialize(imageFiles);
        }

        /// <summary>
        /// Gets the Height for the grid
        /// </summary>
        public int Height
        {
            get { return imageHeight; }
        }

        /// <summary>
        /// Gets the Width for the grid
        /// </summary>
        public int Width
        {
            get { return imageWidth; }
        }

        /// <summary>
        /// Gets the value for the specified u and v co-ordinates in the grid.
        /// </summary>
        /// <param name="u">
        /// U co-ordinate.
        /// </param>
        /// <param name="v">
        /// V co-ordinate.
        /// </param>
        /// <returns>
        /// Value for the specified u and v co-ordinates in the grid.
        /// </returns>
        public double GetValue(double u, double v)
        {
            int idash = (int)Math.Floor((u / du) - 0.5);
            int jdash = (int)Math.Floor((v / dv) - 0.5);

            if (u < 0.0 || u > 1.0 || v < 0.0 || v > 1.0 || idash < -1 || idash > nu || jdash < -1 || jdash > nv)
            {
                return double.NaN;
            }

            double u1 = du * (0.5 + idash);
            double v1 = dv * (0.5 + jdash);

            int i1 = (idash >= 0) ? idash : (circular) ? nu : 0;
            int i2 = (idash < nu) ? (idash + 1) : (circular) ? 0 : nu;
            int j1 = (jdash >= 0) ? jdash : 0;
            int j2 = (jdash < nv) ? (jdash + 1) : nv;

            int a = BilinearInterpolation(u1, v1, du, dv, alphaPixelData[j1][i1], alphaPixelData[j1][i2], alphaPixelData[j2][i1], alphaPixelData[j2][i2], u, v);
            int r = BilinearInterpolation(u1, v1, du, dv, redPixelData[j1][i1], redPixelData[j1][i2], redPixelData[j2][i1], redPixelData[j2][i2], u, v);
            int g = BilinearInterpolation(u1, v1, du, dv, greenPixelData[j1][i1], greenPixelData[j1][i2], greenPixelData[j2][i1], greenPixelData[j2][i2], u, v);
            int b = BilinearInterpolation(u1, v1, du, dv, bluePixelData[j1][i1], bluePixelData[j1][i2], bluePixelData[j2][i1], bluePixelData[j2][i2], u, v);

            return Color.FromArgb(a, r, g, b).ToArgb();
        }

        /// <summary>
        /// Gets the X index value for the specified u co-ordinate in the grid
        /// </summary>
        /// <param name="u">
        /// U co-ordinate
        /// </param>
        /// <returns>
        /// Index along X-axis
        /// </returns>
        public int GetXIndex(double u)
        {
            int idash = (int)Math.Floor((u / du) - 0.5);
            if (u < 0.0 || u > 1.0 || idash < -1 || idash > nu)
            {
                // Error condition
                return -1;
            }

            return idash;
        }

        /// <summary>
        /// Gets the Y index value for the specified v co-ordinate in the grid
        /// </summary>
        /// <param name="v">
        /// V co-ordinate
        /// </param>
        /// <returns>
        /// Index along Y axis
        /// </returns>
        public int GetYIndex(double v)
        {
            int jdash = (int)Math.Floor((v / dv) - 0.5);
            if (v < 0.0 || v > 1.0 || jdash < -1 || jdash > nv)
            {
                // Error condition
                return -1;
            }

            return jdash;
        }

        /// <summary>
        /// Gets the value contained at the specified i and j index.
        /// </summary>
        /// <param name="i">
        /// Index along X-axis.
        /// </param>
        /// <param name="j">
        /// Index along Y-axis.
        /// </param>
        /// <returns>
        /// Value contained at the specified i and j index.
        /// </returns>
        public double GetValueAt(int i, int j)
        {
            if (j < 0 || j >= this.redPixelData.GetLength(0) || i < 0 || i >= this.redPixelData[0].Length)
            {
                return double.NaN;
            }

            return Color.FromArgb(alphaPixelData[j][i], redPixelData[j][i], greenPixelData[j][i], bluePixelData[j][i]).ToArgb();
        }

        /// <summary>
        /// This function is used to get the value based on bilinear interpolation.
        /// </summary>
        /// <param name="u1">
        /// Updated u1 value.
        /// </param>
        /// <param name="v1">
        /// Updated v1 value.
        /// </param>
        /// <param name="deltaU">
        /// du Delta value.
        /// </param>
        /// <param name="deltaV">
        /// dv Delta value.
        /// </param>
        /// <param name="f11">
        /// Pixel value at (1,1).
        /// </param>
        /// <param name="f21">
        /// Pixel value at (2,1).
        /// </param>
        /// <param name="f12">
        /// Pixel value at (1,2).
        /// </param>
        /// <param name="f22">
        /// Pixel value at (2,2).
        /// </param>
        /// <param name="u">
        /// Pixel value at u.
        /// </param>
        /// <param name="v">
        /// Pixel value at v.
        /// </param>
        /// <returns>
        /// Interpolate value of (u,v) pixel.
        /// </returns>
        private static int BilinearInterpolation(double u1, double v1, double deltaU, double deltaV, int f11, int f21, int f12, int f22, double u, double v)
        {
            double us = (u - u1) / deltaU;
            double vs = (v - v1) / deltaV;
            double f1 = us * (f21 - f11) + f11;
            double f2 = us * (f22 - f12) + f12;
            return (int)(vs * (f2 - f1) + f1);
        }

        /// <summary>
        /// Initializes the data grid by reading image pixel values.
        /// </summary>
        /// <param name="bitmap">
        /// Image bitmap.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This is done in order to gain performance benefits while handling bitmaps.")]
        private void Initialize(Bitmap bitmap)
        {
            this.imageWidth = bitmap.Width;
            this.imageHeight = bitmap.Height;
            this.SetInterpolationProperties();

            this.redPixelData = new byte[this.imageHeight][];
            this.bluePixelData = new byte[this.imageHeight][];
            this.greenPixelData = new byte[this.imageHeight][];
            this.alphaPixelData = new byte[this.imageHeight][];

            // Use .Net Interop services to read color pixels.
            // Interop makes for better performance.
            Rectangle rect = new Rectangle(0, 0, this.imageWidth, this.imageHeight);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] rgb = new byte[bmpData.Stride];
            for (int j = 0; j < this.imageHeight; j++)
            {
                this.redPixelData[j] = new byte[this.imageWidth];
                this.bluePixelData[j] = new byte[this.imageWidth];
                this.greenPixelData[j] = new byte[this.imageWidth];
                this.alphaPixelData[j] = new byte[this.imageWidth];

                IntPtr p = new IntPtr(bmpData.Scan0.ToInt64() + (j * bmpData.Stride));
                Marshal.Copy(p, rgb, 0, rgb.Length);
                int pos = 0;
                for (int i = 0; i < this.imageWidth; i++)
                {
                    this.bluePixelData[j][i] = rgb[pos++];
                    this.greenPixelData[j][i] = rgb[pos++];
                    this.redPixelData[j][i] = rgb[pos++];
                    this.alphaPixelData[j][i] = rgb[pos++];
                }

                Array.Clear(rgb, 0, rgb.Length);
            }

            bitmap.UnlockBits(bmpData);
        }

        /// <summary>
        /// Initializes the data grid by reading image pixel values for multipart images.
        /// </summary>
        /// <param name="imageTiles">
        /// Array should be in the format of M X N two dimensional array where 
        ///     N = number of images in the horizontal direction
        ///     M = number of images in the vertical direction
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#", Justification = "Multidimensional array does not waste space.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This is done in order to gain performance benefits while handling bitmaps.")]
        private void Initialize(string[,] imageTiles)
        {
            this.SetInterpolationProperties();

            int totalRows = imageTiles.GetLength(0);
            int totalColumns = imageTiles.GetLength(1);
            int offsetY = this.imageHeight / totalRows;
            int offsetX = this.imageWidth / totalColumns;

            for (int row = 0; row < totalRows; row++)
            {
                for (int column = 0; column < totalColumns; column++)
                {
                    string imagePath = imageTiles[row, column];
                    if (File.Exists(imagePath))
                    {
                        System.Diagnostics.Trace.TraceInformation("{0}: Loading image {1} ...", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture), imagePath);
                        using (Bitmap bitmap = new Bitmap(imagePath))
                        {
                            long x = offsetX * column;
                            long y = offsetY * row;

                            // Use .Net Interop services to read color pixels.
                            // Interop makes for better performance.
                            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                            byte[] rgb = new byte[bmpData.Stride];
                            for (int j = 0; j < bitmap.Height; j++)
                            {
                                IntPtr p = new IntPtr(bmpData.Scan0.ToInt64() + (j * bmpData.Stride));
                                Marshal.Copy(p, rgb, 0, rgb.Length);
                                int pos = 0;
                                for (int i = 0; i < bitmap.Width; i++)
                                {
                                    this.bluePixelData[y + j][x + i] = rgb[pos++];
                                    this.greenPixelData[y + j][x + i] = rgb[pos++];
                                    this.redPixelData[y + j][x + i] = rgb[pos++];
                                    this.alphaPixelData[y + j][x + i] = rgb[pos++];
                                }

                                Array.Clear(rgb, 0, rgb.Length);
                            }

                            bitmap.UnlockBits(bmpData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This function is used to initialize the EquirectangularImage properties.
        /// </summary>
        /// <param name="imageTiles">
        /// Two dimensional array of multipart images.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#", Justification = "Multidimensional array does not waste space.")]
        private void SetImageProperties(string[,] imageTiles)
        {
            int totalRows = imageTiles.GetLength(0);
            int totalColumns = imageTiles.GetLength(1);

            // Get the width and height from the first image. 
            // Assuming 
            //  1. All images are of equal width and height.
            //  2. All images in the grid are present.
            using (Bitmap bitmap = new Bitmap(imageTiles[0, 0]))
            {
                this.imageWidth = totalColumns * bitmap.Width;
                this.imageHeight = totalRows * bitmap.Height;
            }
        }

        /// <summary>
        /// This function is used to set the properties used in the interpolation.
        /// </summary>
        private void SetInterpolationProperties()
        {
            nu = this.imageWidth - 1;
            nv = this.imageHeight - 1;
            du = 1.0 / this.imageWidth;
            dv = 1.0 / this.imageHeight;
        }
    }
}
