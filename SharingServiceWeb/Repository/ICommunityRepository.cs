//-----------------------------------------------------------------------
// <copyright file="ICommunityRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Collections.ObjectModel;
using System.IO;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Interface is used to implement local and azure community repositories.
    /// </summary>
    public interface ICommunityRepository
    {
        /// <summary>
        /// Gets Community location
        /// </summary>
        /// <returns>Community location</returns>
        string CommunityLocation { get; }

        /// <summary>
        /// Gets all community details for the service. 
        /// </summary>
        /// <returns>Details for all communities.</returns>
        Collection<Community> GetAllCommunites();

        /// <summary>
        /// Gets the signup details for the given community.
        /// </summary>
        /// <param name="communityId">Community for which signup xml to be fetched</param>
        /// <returns>Signup object with details.</returns>
        SignUp GetSignUpDetail(string communityId);

        /// <summary>
        /// Gets the Payload WTML file details for the given community.
        /// </summary>
        /// <param name="communityId">Community for which payload xml to be fetched</param>
        /// <returns>Payload object with details.</returns>
        Folder GetPayloadDetails(string communityId);

        /// <summary>
        /// Gets the file identified by the given id from the given communities folder.
        /// </summary>
        /// <param name="id">Id of the file</param>
        /// <returns>File stream.</returns>
        Stream GetFile(string id);

        /// <summary>
        /// Gets tile image.
        /// </summary>
        /// <param name="id">ID for specified Tile.</param>
        /// <param name="level">Level for the image.</param>
        /// <param name="x">X axis of the image.</param>
        /// <param name="y">Y axis of the image.</param>
        /// <returns>Stream for the tile image for the specified level, x and y axis.</returns>
        Stream GetTile(string id, int level, int x, int y);
        
        /// <summary>
        /// Gets Dem for the specified Tile id.
        /// </summary>
        /// <param name="id">Id for the tile.</param>
        /// <param name="level">Level of the image.</param>
        /// <param name="x">X axis image.</param>
        /// <param name="y">Y axis image.</param>
        /// <returns>Dem for the specified image.</returns>
        Stream GetDem(string id, int level, int x, int y);
    }
}