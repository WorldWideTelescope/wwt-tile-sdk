//-----------------------------------------------------------------------
// <copyright file="ImageTileSerializer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Defines a mechanism for serializing an image tile to a Bitmap on the local file system.
    /// </summary>
    public class ImageTileSerializer : IImageTileSerializer
    {
        /// <summary>
        /// File path where the image tile will be saved.
        /// </summary>
        private string destinationPath;

        /// <summary>
        /// File format of the image.
        /// </summary>
        private ImageFormat imageFormat;

        /// <summary>
        /// Initializes a new instance of the ImageTileSerializer class.
        /// </summary>
        /// <param name="destination">
        /// Destination file path.
        /// </param>
        /// <param name="format">
        /// Destination file format.
        /// </param>
        public ImageTileSerializer(string destination, ImageFormat format)
        {
            if (string.IsNullOrEmpty(destination))
            {
                throw new ArgumentNullException("destination");
            }

            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            this.destinationPath = destination;
            this.imageFormat = format;
        }

        /// <summary>
        /// Gets the image tile file path.
        /// </summary>
        public string DestinationPath
        {
            get
            {
                return this.destinationPath;
            }
        }

        /// <summary>
        /// Gets the file name for a given level, X and Y tile coordinates.
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
        /// <returns>
        /// Filename of the image tile at specified level, X and Y tile coordinates.
        /// </returns>
        public string GetFileName(int level, int tileX, int tileY)
        {
            return string.Format(CultureInfo.InvariantCulture, this.destinationPath, level, tileX, tileY, this.imageFormat.ToString());
        }

        /// <summary>
        /// Serializes the bitmap to the file system.
        /// </summary>
        /// <param name="tile">
        /// Bitmap to be saved.
        /// </param>
        /// <param name="level">
        /// Zoom level.
        /// </param>
        /// <param name="tileX">
        /// X tile coordinate.
        /// </param>
        /// <param name="tileY">
        /// Y tile coordinate.
        /// </param>
        public void Serialize(Bitmap tile, int level, int tileX, int tileY)
        {
            if (tile != null)
            {
                string filename = this.GetFileName(level, tileX, tileY);
                Directory.GetParent(filename).Create();
                tile.Save(filename, this.imageFormat);
            }
        }

        /// <summary>
        /// Deserializes a bitmap object from the file system.
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
        /// <returns>
        /// Deserialized bitmap image.
        /// </returns>
        /// <remarks>
        /// The caller is responsible for disposing of the Bitmap instance being returned.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The bitmap object is being returned to the caller who is responsible for disposing of it.")]
        public Bitmap Deserialize(int level, int tileX, int tileY)
        {
            string filename = this.GetFileName(level, tileX, tileY);
            if (File.Exists(filename))
            {
                return new Bitmap(filename, false);
            }

            return null;
        }
    }
}
