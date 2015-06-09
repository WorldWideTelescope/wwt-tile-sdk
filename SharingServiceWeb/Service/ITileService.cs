//-----------------------------------------------------------------------
// <copyright file="ITileService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.ServiceModel;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Interface representing the Tile Service.
    /// </summary>
    [ServiceContract(Namespace = "")]
    public interface ITileService
    {
        /// <summary>
        /// Gets details for all pyramids.
        /// </summary>
        /// <returns>pyramids details</returns>
        [OperationContract]
        PyramidDetails GetPyramidDetails();

        /// <summary>
        /// Gets tile image.
        /// </summary>
        /// <param name="id">ID for the specified tile.</param>
        /// <param name="level">Level for the image.</param>
        /// <param name="x">X axis of the image.</param>
        /// <param name="y">Y axis of the image.</param>
        /// <returns>Stream for the image.</returns>
        [OperationContract]
        Stream GetTileImage(string id, int level, int x, int y);

        /// <summary>
        /// Gets the Dem.
        /// </summary>
        /// <param name="id">ID for the specified tile.</param>
        /// <param name="level">Level for the image.</param>
        /// <param name="x">X axis of the image.</param>
        /// <param name="y">Y axis of the image.</param>
        /// <returns>Stream for the Dem.</returns>
        [OperationContract]
        Stream GetDem(string id, int level, int x, int y);

        /// <summary>
        /// Gets the thumbnail image.
        /// </summary>
        /// <param name="id">ID for the specified pyramid.</param>
        /// <param name="name">WTML Name of the Pyramid.</param>
        /// <returns>Stream for the image.</returns>
        [OperationContract]
        Stream GetThumbnailImage(string id, string name);

        /// <summary>
        /// Get WTML file.
        /// </summary>
        /// <param name="id">ID for the specified pyramid.</param>
        /// <param name="name">Name of the WTML file.</param>
        /// <returns>Stream for WTML file.</returns>
        [OperationContract]
        Stream GetWtmlFile(string id, string name);
    }
}
