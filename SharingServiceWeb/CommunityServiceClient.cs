//-----------------------------------------------------------------------
// <copyright file="CommunityServiceClient.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Client class which inherits ClientBase class to make calls to WCF methods without adding service reference.
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public class CommunityServiceClient : ClientBase<ICommunityService>, ICommunityService
    {
        /// <summary>
        /// Initializes a new instance of the CommunityServiceClient class. Default constructor.
        /// </summary>
        public CommunityServiceClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommunityServiceClient class.
        /// </summary>
        /// <param name="endpointConfigurationName">End point configuration name</param>
        public CommunityServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommunityServiceClient class.
        /// </summary>
        /// <param name="endpointConfigurationName">End point configuration name</param>
        /// <param name="remoteAddress">Remote address</param>
        public CommunityServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommunityServiceClient class.
        /// </summary>
        /// <param name="endpointConfigurationName">End point configuration name</param>
        /// <param name="remoteAddress">Remote address</param>
        public CommunityServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommunityServiceClient class.
        /// </summary>
        /// <param name="binding">Binding object</param>
        /// <param name="remoteAddress">Remote address</param>
        public CommunityServiceClient(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        /// <summary>
        /// Gets details for all communities.
        /// </summary>
        /// <returns>Community details</returns>
        public CommunityDetails GetAllCommunites()
        {
            return Channel.GetAllCommunites();
        }

        /// <summary>
        /// Gets the signup WTML file for the given community.
        /// </summary>
        /// <returns>Signup file stream.</returns>
        public Stream GetSignUpFile(string communityId)
        {
            return Channel.GetSignUpFile(communityId);
        }

        /// <summary>
        /// Gets the Payload WTML file for the given community.
        /// </summary>
        /// <returns>Payload file stream.</returns>
        public Stream GetPayloadFile(string communityId)
        {
            return Channel.GetPayloadFile(communityId);
        }

        /// <summary>
        /// Gets the file identified by the given id from the given communities folder.
        /// </summary>
        /// <returns>File stream.</returns>
        public Stream GetFile(string id)
        {
            return Channel.GetFile(id);
        }

        /// <summary>
        /// Gets tile image.
        /// </summary>
        /// <param name="id">ID for specified Tile.</param>
        /// <param name="level">Level for the image.</param>
        /// <param name="x">X axis of the image.</param>
        /// <param name="y">Y axis of the image.</param>
        /// <returns>Stream for the tile image for the specified level, x and y axis.</returns>
        public Stream GetTile(string id, int level, int x, int y)
        {
            return Channel.GetTile(id, level, x, y);
        }

        /// <summary>
        /// Gets Dem for the specified tile id.
        /// </summary>
        /// <param name="id">Id for the tile.</param>
        /// <param name="level">Level of the image.</param>
        /// <param name="x">X axis image.</param>
        /// <param name="y">Y axis image.</param>
        /// <returns>Dem for the specified image.</returns>
        public Stream GetDem(string id, int level, int x, int y)
        {
            return Channel.GetDem(id, level, x, y);
        }
    }
}