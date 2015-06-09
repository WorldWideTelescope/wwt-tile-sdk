//-----------------------------------------------------------------------
// <copyright file="PlateFile.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// Represents a node.
    /// </summary>
    internal struct NodeInfo
    {
        /// <summary>
        /// Gets or sets the start index of node.
        /// </summary>
        public uint Start { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public uint Size { get; set; }
    }

    /// <summary>
    /// Contains utility methods for PlateFile and node information.
    /// </summary>
    public class PlateFile
    {
        #region Constants
        /// <summary>
        /// 7E69AD43 in hex.
        /// magic number is ceil(0.9876 * 2^31) = 0111 1110 0110 1001 1010 1101 0100 0011 in binary
        /// this identifies that this plate file has useful header information.
        /// </summary>
        private const uint DotPlateFileTypeNumber = 2120854851;
        #endregion Constants

        #region Member Variables

        /// <summary>
        /// File Stream object. 
        /// </summary>
        private FileStream fileStream;

        /// <summary>
        /// Offset at current position.
        /// </summary>
        private uint currentOffset;

        /// <summary>
        /// LevelInfo objects.
        /// </summary>
        private LevelInfo[] levelMap;

        #endregion Member Variables

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PlateFile"/> class.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="levels">
        /// Index of the bottom-most level relative to the top.
        ///   For Example:  If a plate file has 6 levels (0,1,2,3,4,5) then “levels=5”
        /// </param>
        public PlateFile(string fileName, int levels)
        {
            this.FileName = fileName;
            this.Levels = levels;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the level for pyramid.
        /// </summary>
        public int Levels { get; private set; }

        /// <summary>
        /// Gets the size of the header.
        /// </summary>
        private uint HeaderSize
        {
            get
            {
                return GetFileIndexOffset(this.Levels, 0, 0);
            }
        }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Handles the initialization of the plate file.
        /// </summary>
        public void Create()
        {
            this.levelMap = new LevelInfo[this.Levels + 1];

            for (int i = 0; i <= this.Levels; i++)
            {
                this.levelMap[i] = new LevelInfo(i);
            }

            try
            {
                this.fileStream = File.Open(this.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                this.WriteHeaders();
                this.currentOffset = this.HeaderSize;
                this.fileStream.Seek(this.currentOffset, SeekOrigin.Begin);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Updates the header information and closes file stream.
        /// </summary>
        public void UpdateHeaderAndClose()
        {
            if (this.fileStream != null)
            {
                this.WriteHeaders();
                this.fileStream.Close();
                this.fileStream = null;
            }
        }

        /// <summary>
        /// This function is used to write the data final file stream form the input stream.
        /// Finally after writing data input stream is closed.
        /// </summary>
        /// <param name="inputStream">
        /// Input stream containing information.
        /// </param>
        /// <param name="level">
        /// Level for PTP.
        /// </param>
        /// <param name="xIndex">
        /// X index for tile.
        /// </param>
        /// <param name="yIndex">
        /// Y index for tile.
        /// </param>
        public void AddStream(Stream inputStream, int level, int xIndex, int yIndex)
        {
            if (inputStream != null)
            {
                long start = this.fileStream.Seek(0, SeekOrigin.End);
                byte[] buf = null;

                int len = (int)inputStream.Length;
                buf = new byte[inputStream.Length];

                this.levelMap[level].FileMap[xIndex, yIndex].Start = (uint)start;
                this.levelMap[level].FileMap[xIndex, yIndex].Size = (uint)len;
                inputStream.Seek(0, SeekOrigin.Begin);

                int numBytes;
                while ((numBytes = inputStream.Read(buf, 0, len)) > 0)
                {
                    this.fileStream.Write(buf, 0, numBytes);
                }

                inputStream.Close();
            }
        }

        /// <summary>
        /// Overloaded method to get the required file stream from level information.
        /// </summary>
        /// <param name="level">
        /// The level for PTP.
        /// </param>
        /// <param name="xIndex">
        /// The index X for tile.
        /// </param>
        /// <param name="yIndex">
        /// The index Y for tile.
        /// </param>
        /// <returns>
        /// Stream having required information.
        /// </returns>
        public Stream GetFileStream(int level, int xIndex, int yIndex)
        {
            Stream ms = null;
            if (this.fileStream != null)
            {
                ms = GetTileStream(this.fileStream, level, xIndex, yIndex);
            }
            else if (this.FileName.Length > 0 && File.Exists(this.FileName) && this.Levels >= level)
            {
                ms = GetTileStream(level, xIndex, yIndex);
            }

            return (ms.Length == 0) ? null : ms;
        }

        /// <summary>
        /// This function is used to get the tile.
        /// </summary>
        /// <param name="level">
        /// Level of tile.
        /// </param>
        /// <param name="xIndex">
        /// The index X for tile.
        /// </param>
        /// <param name="yIndex">
        /// The index Y for tile.
        /// </param>
        /// <returns>
        /// Image of the tile.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore any exception which occurs.")]
        public Bitmap GetBitmap(int level, int xIndex, int yIndex)
        {
            try
            {
                Stream tileStream = this.GetFileStream(level, xIndex, yIndex);
                return (tileStream != null) ? new Bitmap(tileStream) : null;
            }
            catch
            {
                // Ignore any exception and return null.
            }

            return null;
        }

        /// <summary>
        /// This function is used to get the DEM tile.
        /// </summary>
        /// <param name="level">
        /// Level of tile.
        /// </param>
        /// <param name="xIndex">
        /// The index X for tile.
        /// </param>
        /// <param name="yIndex">
        /// The index Y for tile.
        /// </param>
        /// <returns>
        /// DEM tile value as short Array.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore any exception which occurs.")]
        public short[] GetDemTile(int level, int xIndex, int yIndex)
        {
            Stream tileStream = this.GetFileStream(level, xIndex, yIndex);
            List<short> value = new List<short>();

            try
            {
                using (BinaryReader br = new BinaryReader(tileStream))
                {
                    while (tileStream.Length > tileStream.Position)
                    {
                        value.Add(br.ReadInt16());
                    }
                }
            }
            catch
            {
                // ignore any exception
            }

            return value.ToArray();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the file offset index.
        /// </summary>
        /// <param name="level">
        /// The level of PTP.
        /// </param>
        /// <param name="xIndex">
        /// The index X.
        /// </param>
        /// <param name="yIndex">
        /// The index Y.
        /// </param>
        /// <returns>
        /// File offset index.
        /// </returns>
        private static uint GetFileIndexOffset(int level, int xIndex, int yIndex)
        {
            uint offset = 8;
            for (uint i = 0; i < level; i++)
            {
                offset += (uint)(Math.Pow(2, i * 2) * 8);
            }

            offset += (uint)((yIndex * Math.Pow(2, level)) + xIndex) * 8;

            return offset;
        }

        /// <summary>
        /// Gets the node information.
        /// </summary>
        /// <param name="fs">
        /// The file stream containing information.
        /// </param>
        /// <param name="offset">
        /// The offset to seek from.
        /// </param>
        /// <param name="length">
        /// The length/level of node.
        /// </param>
        /// <returns>
        /// Start index of node.
        /// </returns>
        private static uint GetNodeInfo(FileStream fs, uint offset, out uint length)
        {
            byte[] buf = new byte[8];
            fs.Seek(offset, SeekOrigin.Begin);
            fs.Read(buf, 0, 8);

            length = (uint)(buf[4] + (buf[5] << 8) + (buf[6] << 16) + (buf[7] << 24));
            return (uint)(buf[0] + (buf[1] << 8) + (buf[2] << 16) + (buf[3] << 24));
        }

        /// <summary>
        /// Sets the node information.
        /// </summary>
        /// <param name="offset">
        /// The offset value to seek from.
        /// </param>
        /// <param name="start">
        /// The start index.
        /// </param>
        /// <param name="length">
        /// The length of node.
        /// </param>
        private void SetNodeInfo(uint offset, uint start, uint length)
        {
            byte[] buf = new byte[8];
            buf[0] = (byte)(start % 256);
            buf[1] = (byte)((start >> 8) % 256);
            buf[2] = (byte)((start >> 16) % 256);
            buf[3] = (byte)((start >> 24) % 256);
            buf[4] = (byte)(length % 256);
            buf[5] = (byte)((length >> 8) % 256);
            buf[6] = (byte)((length >> 16) % 256);
            buf[7] = (byte)((length >> 24) % 256);

            this.fileStream.Seek(offset, SeekOrigin.Begin);
            this.fileStream.Write(buf, 0, 8);
        }

        /// <summary>
        /// Gets the required subset stream from filestream and level information.
        /// </summary>
        /// <param name="stream">
        /// The stream containing information.
        /// </param>
        /// <param name="level">
        /// The level of PTP.
        /// </param>
        /// <param name="xIndex">
        /// The index X for tile.
        /// </param>
        /// <param name="yIndex">
        /// The index Y for tile.
        /// </param>
        /// <returns>
        /// Stream having required information.
        /// </returns>
        private MemoryStream GetTileStream(FileStream stream, int level, int xIndex, int yIndex)
        {
            MemoryStream ms = null;

            uint length = this.levelMap[level].FileMap[xIndex, yIndex].Size;
            uint start = this.levelMap[level].FileMap[xIndex, yIndex].Start;

            byte[] buffer = new byte[length];
            stream.Seek(start, SeekOrigin.Begin);
            stream.Read(buffer, 0, (int)length);
            ms = new MemoryStream(buffer);

            return ms;
        }

        /// <summary>
        /// Opens the specified file and returns the stream containing it.
        /// </summary>
        /// <param name="level">
        /// The level for PTP.
        /// </param>
        /// <param name="xIndex">
        /// The index X for tile.
        /// </param>
        /// <param name="yIndex">
        /// The index Y for tile.
        /// </param>
        /// <returns>
        /// Stream containing the specified file.
        /// </returns>
        private Stream GetTileStream(int level, int xIndex, int yIndex)
        {
            uint offset = GetFileIndexOffset(level, xIndex, yIndex);
            uint length;
            uint start;

            MemoryStream ms = null;
            using (FileStream f = File.Open(this.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                f.Seek(offset, SeekOrigin.Begin);
                start = GetNodeInfo(f, offset, out length);

                byte[] buffer = new byte[length];
                f.Seek(start, SeekOrigin.Begin);
                f.Read(buffer, 0, (int)length);
                ms = new MemoryStream(buffer);
            }

            return ms;
        }

        /// <summary>
        /// Writes the header information.
        /// </summary>
        private void WriteHeaders()
        {
            uint l = (uint)this.Levels;
            byte[] buffer = new byte[8];
            buffer[0] = (byte)(DotPlateFileTypeNumber % 256);
            buffer[1] = (byte)((DotPlateFileTypeNumber >> 8) % 256);
            buffer[2] = (byte)((DotPlateFileTypeNumber >> 16) % 256);
            buffer[3] = (byte)((DotPlateFileTypeNumber >> 24) % 256);
            buffer[4] = (byte)l;
            buffer[5] = (byte)(l >> 8);
            buffer[6] = (byte)(l >> 16);
            buffer[7] = (byte)(l >> 24);

            this.fileStream.Write(buffer, 0, 8);

            uint currentSeek = 8;
            foreach (LevelInfo li in this.levelMap)
            {
                int count = (int)Math.Pow(2, li.Level);
                for (int y = 0; y < count; y++)
                {
                    for (int x = 0; x < count; x++)
                    {
                        SetNodeInfo(currentSeek, li.FileMap[x, y].Start, li.FileMap[x, y].Size);
                        currentSeek += 8;
                    }
                }
            }
        }

        #endregion Private Methods

        /// <summary>
        /// Represents a level in plate pyramid.
        /// </summary>
        internal class LevelInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LevelInfo"/> class.
            /// </summary>
            /// <param name="level">
            /// The level for pyramid.
            /// </param>
            public LevelInfo(int level)
            {
                this.Level = level;
                this.FileMap = new NodeInfo[(int)Math.Pow(2, level), (int)Math.Pow(2, level)];
            }

            /// <summary>
            /// Gets or sets the level.
            /// </summary>
            public int Level { get; set; }

            /// <summary>
            /// Gets or sets the file mapping information.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member", Justification = "Multidimensional array does not waste space.")]
            public NodeInfo[,] FileMap { get; set; }
        }
    }
}
