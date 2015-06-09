//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Core
{
    /// <summary>
    /// ProjectionTypes values.
    /// </summary>
    public enum ProjectionTypes
    {
        /// <summary>
        /// Toast projection.
        /// </summary>
        Toast,

        /// <summary>
        /// Mercator projection.
        /// </summary>
        Mercator,
    }

    /// <summary>
    /// BandPasses values.
    /// </summary>
    public enum BandPasses
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Gamma BandPass.
        /// </summary>
        Gamma,

        /// <summary>
        /// XRay BandPass.
        /// </summary>
        XRay,

        /// <summary>
        /// Ultraviolet BandPass.
        /// </summary>
        Ultraviolet,

        /// <summary>
        /// Visible BandPass.
        /// </summary>
        Visible,

        /// <summary>
        /// HydrogenAlpha BandPass.
        /// </summary>
        HydrogenAlpha,

        /// <summary>
        /// IR BandPass.
        /// </summary>
        IR,

        /// <summary>
        /// Microwave BandPass.
        /// </summary>
        Microwave,

        /// <summary>
        /// Radio BandPass.
        /// </summary>
        Radio
    }

    /// <summary>
    /// DataSetTypes values.
    /// </summary>
    public enum DataSetTypes
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Sky DataSet.
        /// </summary>
        Sky,

        /// <summary>
        /// Planet DataSet.
        /// </summary>
        Planet,

        /// <summary>
        /// Earth DataSet.
        /// </summary>
        Earth,

        /// <summary>
        /// Panorama DataSet.
        /// </summary>
        Panorama,

        /// <summary>
        /// Survey DataSet.
        /// </summary>
        Survey
    }

    /// <summary>
    /// Orientation values for the color map.
    /// </summary>
    public enum ColorMapOrientation
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Horizontally oriented.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Vertically oriented.
        /// </summary>
        Vertical
    }
}
