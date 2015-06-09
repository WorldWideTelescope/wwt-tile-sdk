//---------------------------------------------------------------------
// <copyright file="TileChopper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// This class is used to tile input image.
    /// </summary>
    public class TileChopper : ITileCreator
    {
        /// <summary>
        /// Projection Type.
        /// </summary>
        private ProjectionTypes projectionType;

        /// <summary>
        /// Width of the image.
        /// </summary>
        private int imageWidth;

        /// <summary>
        /// Height of the image.
        /// </summary>
        private int imageHeight;

        /// <summary>
        /// Stores input image as byte array.
        /// </summary>
        private byte[] imageData;

        /// <summary>
        /// Input boundary
        /// </summary>
        private Boundary inputBoundary;

        /// <summary>
        /// Represents the start pixel value of X coordinate.
        /// </summary>
        private int startX;

        /// <summary>
        /// Represents the end pixel value of X coordinate.
        /// </summary>
        private int endX;

        /// <summary>
        /// Represents the start pixel value of Y coordinate.
        /// </summary>
        private int startY;

        /// <summary>
        /// Represents the end pixel value of Y coordinate.
        /// </summary>
        private int endY;

        /// <summary>
        /// Initializes a new instance of the TileChopper class
        /// </summary>
        /// <param name="fileName">
        /// Full path of the image.
        /// </param>
        /// <param name="serializer">
        /// Tile serializer.
        /// </param>
        /// <param name="projectionType">
        /// Projection type.
        /// </param>
        public TileChopper(string fileName, IImageTileSerializer serializer, ProjectionTypes projectionType)
            : this(fileName, serializer, projectionType, new Boundary(-180, -90, 180, 90))
        {
        }

        /// <summary>
        /// Initializes a new instance of the TileChopper class
        /// </summary>
        /// <param name="fileName">
        /// Full path of the image.
        /// </param>
        /// <param name="serializer">
        /// Tile serializer.
        /// </param>
        /// <param name="projectionType">
        /// Projection type.
        /// </param>
        /// <param name="inputBoundary">
        /// Input boundary.
        /// </param>
        public TileChopper(string fileName, IImageTileSerializer serializer, ProjectionTypes projectionType, Boundary inputBoundary)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            try
            {
                this.TileSerializer = serializer;
                this.projectionType = projectionType;
                this.inputBoundary = inputBoundary;

                // Create a bitmap object and read the color pixels into the grid.
                using (Bitmap inputImage = new Bitmap(fileName))
                {
                    this.Initialize(inputImage);
                }
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
        }

        /// <summary>
        /// Gets the ProjectionType.
        /// </summary>
        public ProjectionTypes ProjectionType
        {
            get
            {
                return this.projectionType;
            }
        }

        /// <summary>
        /// Gets or sets the number of levels of detail.
        /// </summary>
        public int MaximumLevelsOfDetail { get; set; }

        /// <summary>
        /// Gets the IImageTileSeralizer instance bound to this tile creator.
        /// </summary>
        public IImageTileSerializer TileSerializer { get; private set; }

        /// <summary>
        /// Creates base tiles and the pyramid.
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
            // Size of the image to extract before scaling it down to (side x side)
            int tileSize = Constants.TileSize * (int)Math.Pow(2, MaximumLevelsOfDetail - level);
            Bitmap tile = this.ExtractTile((tileX * tileSize), (tileY * tileSize), tileSize);

            if (tile != null)
            {
                if (tileSize > Constants.TileSize)
                {
                    tile = TileHelper.ResizeBitmap(tile, Constants.TileSize, Constants.TileSize);
                }

                this.TileSerializer.Serialize(tile, level, tileX, tileY);
            }
        }

        /// <summary>
        /// Fills up the pyramid starting at specified level.
        /// </summary>
        /// <param name="level">
        /// Base level of the pyramid.
        /// </param>
        /// <param name="tileX">
        /// X tile coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y tile coordinate.
        /// </param>
        public void CreateParent(int level, int tileX, int tileY)
        {
            TileHelper.CreateParent(level, tileX, tileY, this.TileSerializer);
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

            // Compute the maximum level of detail that this image supports.
            // This is a function of size of the image.
            this.MaximumLevelsOfDetail = TileHelper.CalculateMaximumLevel(this.imageHeight, this.imageWidth, this.inputBoundary);

            // Get starting pixel values for x and y
            Helper.LatLongToPixelXY(this.inputBoundary.Bottom, this.inputBoundary.Left, this.MaximumLevelsOfDetail, out startX, out startY);

            // Get ending pixel values for x and y
            Helper.LatLongToPixelXY(this.inputBoundary.Top, this.inputBoundary.Right, this.MaximumLevelsOfDetail, out endX, out endY);

            // Get image data
            this.imageData = TileHelper.BitmapToBytes(bitmap);
        }

        /// <summary>
        /// Extract a tile from a larger image.
        /// Pixel data of large image is given by source.
        /// Large image is assumed to be square of side S.
        /// Source.Length should be ((S * S) * bytesPerPixel) where bytesPerPixel = 4. Output is 32bpp RGB.
        /// The output tile is square with side "side".
        /// S does not have to be multiple of side. S should be greater than side.
        /// x and y are location of top corner of tile.
        /// </summary>
        /// <param name="coordinateX">The coordinate X.</param>
        /// <param name="coordinateY">The coordinate Y.</param>
        /// <param name="side">The side to be extracted.</param>
        /// <returns>The Extracted Tile Bitmap</returns>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This is done in order to gain performance benefits while handling bitmaps.")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This is done in order to gain performance benefits while handling bitmaps.")]
        private Bitmap ExtractTile(int coordinateX, int coordinateY, int side)
        {
            int stride = Constants.BytesPerPixel * this.imageWidth;

            Bitmap tile = new Bitmap(side, side, PixelFormat.Format32bppArgb);
            Rectangle dimension = new Rectangle(0, 0, side, side);
            BitmapData tileData = tile.LockBits(dimension, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] argb = new byte[tileData.Stride];
            bool hasData = false;
            for (int iy = 0; iy < side; iy++)
            {
                Array.Clear(argb, 0, argb.Length);
                int pos = 0;
                for (int ix = 0; ix < side; ix++)
                {
                    Color color = Color.Transparent;
                    if ((coordinateY + iy) < startY || (coordinateY + iy) >= endY || (coordinateX + ix) < startX || (coordinateX + ix) >= endX)
                    {
                        argb[pos++] = color.B;
                        argb[pos++] = color.G;
                        argb[pos++] = color.R;
                        argb[pos++] = color.A;
                    }
                    else
                    {
                        // Position will be used as index in zero based array
                        int position = ((coordinateY + iy - startY) * stride) + (Constants.BytesPerPixel * (coordinateX + ix - startX));

                        if (position + 3 <= this.imageData.Length)
                        {
                            argb[pos++] = this.imageData[position];
                            argb[pos++] = this.imageData[position + 1];
                            argb[pos++] = this.imageData[position + 2];
                            argb[pos++] = this.imageData[position + 3];
                            hasData = true;
                        }
                        else
                        {
                            argb[pos++] = color.B;
                            argb[pos++] = color.G;
                            argb[pos++] = color.R;
                            argb[pos++] = color.A;
                        }
                    }
                }

                IntPtr p = new IntPtr(tileData.Scan0.ToInt64() + (iy * side * Constants.BytesPerPixel));
                System.Runtime.InteropServices.Marshal.Copy(argb, 0, p, argb.Length);
            }

            tile.UnlockBits(tileData);
            tileData = null;
            if (!hasData)
            {
                tile = null;
            }

            return tile;
        }
    }
}
