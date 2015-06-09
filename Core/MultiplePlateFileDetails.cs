//-----------------------------------------------------------------------
// <copyright file="MultiplePlateFileDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// This class is used to calculate and store the details of the multiple plate files.
    /// </summary>
    public class MultiplePlateFileDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplePlateFileDetails"/> class.
        /// </summary>
        /// <param name="maxLevel">
        /// Number of levels.
        /// </param>
        public MultiplePlateFileDetails(int maxLevel)
        {
            int totalLevels = checked(maxLevel + 1);
            this.LevelsPerPlate = (int)(Math.Floor((double)(totalLevels) / 2)) + 1;
            this.TotalOverlappedLevels = (this.LevelsPerPlate * 2) - totalLevels;
            this.MaxOverlappedLevel = (int)(Math.Floor((double)(totalLevels) / 2));
            this.MinOverlappedLevel = totalLevels - this.LevelsPerPlate;
        }

        /// <summary>
        /// Gets the highest overlapped level.
        /// </summary>
        public int MaxOverlappedLevel { get; private set; }

        /// <summary>
        /// Gets the minimum overlapped level.
        /// </summary>
        public int MinOverlappedLevel { get; private set; }

        /// <summary>
        /// Gets the total number of overlapped level.
        /// </summary>
        public int TotalOverlappedLevels { get; private set; }

        /// <summary>
        /// Gets the number of levels per plate.
        /// </summary>
        public int LevelsPerPlate { get; private set; }
    }
}
