//-----------------------------------------------------------------------
// <copyright file="ICommunityService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.ServiceModel;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Interface representing the Community Service.
    /// </summary>
    [ServiceContract]
    public interface ICommunityService
    {
        /// <summary>
        /// Gets details for all communities.
        /// </summary>
        /// <returns>Community details</returns>
        [OperationContract]
        CommunityDetails GetAllCommunites();

        /// <summary>
        /// Gets the signup WTML file for the given community.
        /// </summary>
        /// <returns>Signup file stream.</returns>
        [OperationContract]
        Stream GetSignUpFile(string communityId);

        /// <summary>
        /// Gets the Payload WTML file for the given community.
        /// </summary>
        /// <returns>Payload file stream.</returns>
        [OperationContract]
        Stream GetPayloadFile(string communityId);

        /// <summary>
        /// Gets the file identified by the given id from the communities folder.
        /// </summary>
        /// <returns>File stream.</returns>
        [OperationContract]
        Stream GetFile(string id);

        /// <summary>
        /// Gets tile image.
        /// </summary>
        /// <param name="id">ID for specified Tile.</param>
        /// <param name="level">Level for the image.</param>
        /// <param name="x">X axis of the image.</param>
        /// <param name="y">Y axis of the image.</param>
        /// <returns>Stream for the tile image for the specified level, x and y axis.</returns>
        [OperationContract]
        Stream GetTile(string id, int level, int x, int y);

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
    }
}
