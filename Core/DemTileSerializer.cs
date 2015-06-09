//-----------------------------------------------------------------------
// <copyright file="DemTileSerializer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Defines a mechanism for serializing a DEM tile to the local file system.
    /// </summary>
    public class DemTileSerializer : IDemTileSerializer
    {
        /// <summary>
        /// Full file path where the DEM tile will be saved.
        /// </summary>
        private string destinationPath;

        /// <summary>
        /// Initializes a new instance of the DemTileSerializer class.
        /// </summary>
        /// <param name="destination">
        /// Destination file path.
        /// </param>
        public DemTileSerializer(string destination)
        {
            if (string.IsNullOrEmpty(destination))
            {
                throw new ArgumentNullException("destination");
            }

            this.destinationPath = destination;
        }

        /// <summary>
        /// Gets the DEM tile file path.
        /// </summary>
        public string DestinationPath
        {
            get
            {
                return this.destinationPath;
            }
        }

        /// <summary>
        /// Serializes the binary array to file system.
        /// </summary>
        /// <param name="tile">
        /// Values to be serialized .
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
        public void Serialize(short[] tile, int level, int tileX, int tileY)
        {
            if (tile != null)
            {
                string filename = this.GetFileName(level, tileX, tileY);
                Directory.GetParent(filename).Create();
                FileStream fileStream = null;
                try
                {
                    fileStream = File.Create(filename);
                    using (BinaryWriter bw = new BinaryWriter(fileStream))
                    {
                        fileStream = null;
                        foreach (short value in tile)
                        {
                            bw.Write(value);
                        }
                    }
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes an array of short values from file system.
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
        /// Deserialized array.
        /// </returns>
        public short[] Deserialize(int level, int tileX, int tileY)
        {
            string filename = this.GetFileName(level, tileX, tileY);
            short[] values = null;
            if (File.Exists(filename))
            {
                FileInfo fileInfo = new FileInfo(filename);
                if (fileInfo.Length == 1026)
                {
                    values = new short[513];
                }
                else if (fileInfo.Length == 2178)
                {
                    values = new short[1089];
                }
                else
                {
                    throw new NotImplementedException();
                }

                FileStream fileStream = null;
                try
                {
                    fileStream = File.OpenRead(filename);
                    using (BinaryReader br = new BinaryReader(fileStream))
                    {
                        fileStream = null;
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = br.ReadInt16();
                        }
                    }
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Dispose();
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// Gets or sets the file name for a given level, X and Y tile coordinates.
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
        /// Filename of the DEM tile at specified level, X and Y tile coordinates.
        /// </returns>
        public string GetFileName(int level, int tileX, int tileY)
        {
            return string.Format(CultureInfo.CurrentCulture, this.destinationPath, level, tileX, tileY);
        }
    }
}
