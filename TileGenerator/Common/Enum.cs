//-----------------------------------------------------------------------
// <copyright file="Enum.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.TileGenerator
{
    /// <summary>
    /// Coordinate position values.
    /// </summary>
    public enum Position
    {
        /// <summary>
        /// Top left position
        /// </summary>
        TopLeft,

        /// <summary>
        /// Bottom right position
        /// </summary>
        BottomRight
    }

    /// <summary>
    /// Tags for progress bar status.
    /// </summary>
    public enum ProgressBarStatus
    {
        /// <summary>
        /// Status to represent processing state.
        /// </summary>
        Processing,

        /// <summary>
        /// Status to represent error state.
        /// </summary>
        Error,

        /// <summary>
        /// Status to represent success state.
        /// </summary>
        Success
    }

    /// <summary>
    /// Steps in pyramids generation.
    /// </summary>
    public enum PyramidGenerationSteps
    {
        /// <summary>
        /// Loading image.
        /// </summary>
        LoadingImage,

        /// <summary>
        /// Generating pyramids.
        /// </summary>
        PyramidGeneration,

        /// <summary>
        /// Generating plate file.
        /// </summary>
        PlateFileGeneration,

        /// <summary>
        /// Generating thumbnail.
        /// </summary>
        ThumbnailGeneration,

        /// <summary>
        /// Generating WTML file.
        /// </summary>
        WTMLGeneration,
    }

    /// <summary>
    /// Status of pyramid generation.
    /// </summary>
    public enum PyramidGenerationStatus
    {
        /// <summary>
        /// Steps has started.
        /// </summary>
        Started,

        /// <summary>
        /// Step has completed.
        /// </summary>
        Completed,
    }

    /// <summary>
    /// ProjectionTypes values.
    /// </summary>
    public enum InputProjections
    {
        /// <summary>
        /// EquiRectangular projection
        /// </summary>
        EquiRectangular,

        /// <summary>
        /// Mercator projection.
        /// </summary>
        Mercator,
    }
}
