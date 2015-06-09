//-----------------------------------------------------------------------
// <copyright file="TileServiceClient.cs" company="Microsoft Corporation">
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
    public class TileServiceClient : ClientBase<ITileService>, ITileService
    {
        /// <summary>
        /// Initializes a new instance of the TileServiceClient class. Default constructor.
        /// </summary>
        public TileServiceClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TileServiceClient class.
        /// </summary>
        /// <param name="endpointConfigurationName">End point configuration name</param>
        public TileServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TileServiceClient class.
        /// </summary>
        /// <param name="endpointConfigurationName">End point configuration name</param>
        /// <param name="remoteAddress">Remote address</param>
        public TileServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TileServiceClient class.
        /// </summary>
        /// <param name="endpointConfigurationName">End point configuration name</param>
        /// <param name="remoteAddress">Remote address</param>
        public TileServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TileServiceClient class.
        /// </summary>
        /// <param name="binding">Binding object</param>
        /// <param name="remoteAddress">Remote address</param>
        public TileServiceClient(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        /// <summary>
        /// Gets details for all pyramids.
        /// </summary>
        /// <returns>pyramids details</returns>
        public PyramidDetails GetPyramidDetails()
        {
            return Channel.GetPyramidDetails();
        }

        /// <summary>
        /// Gets tile image.
        /// </summary>
        /// <param name="id">ID for the specified tile.</param>
        /// <param name="level">Level for the image.</param>
        /// <param name="x">X axis of the image.</param>
        /// <param name="y">Y axis of the image.</param>
        /// <returns>Stream for the image.</returns>
        public Stream GetTileImage(string id, int level, int x, int y)
        {
            return Channel.GetTileImage(id, level, x, y); 
        }

        /// <summary>
        /// Gets the Dem.
        /// </summary>
        /// <param name="id">ID for the specified tile.</param>
        /// <param name="level">Level for the image.</param>
        /// <param name="x">X axis of the image.</param>
        /// <param name="y">Y axis of the image.</param>
        /// <returns>Stream for the Dem.</returns>
        public Stream GetDem(string id, int level, int x, int y)
        {
            return Channel.GetDem(id, level, x, y);
        }

        /// <summary>
        /// Gets the thumbnail image.
        /// </summary>
        /// <param name="id">ID for the specified pyramid.</param>
        /// <param name="name">WTML Name of the Pyramid.</param>
        /// <returns>Stream for the image.</returns>
        public Stream GetThumbnailImage(string id, string name)
        {
            return Channel.GetThumbnailImage(id, name);
        }

        /// <summary>
        /// Get WTML file.
        /// </summary>
        /// <param name="id">ID for the specified pyramid.</param>
        /// <param name="name">Name of the WTML file.</param>
        /// <returns>Stream for WTML file.</returns>
        public Stream GetWtmlFile(string id, string name)
        {
            return Channel.GetWtmlFile(id, name);
        }
    }
}