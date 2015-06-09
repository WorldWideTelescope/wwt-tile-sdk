//-----------------------------------------------------------------------
// <copyright file="Pyramid.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Pyramid data for the service. 
    /// </summary>
    [DataContract(Namespace = "")]
    public class Pyramid
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets thumbnail URL.
        /// </summary>
        [DataMember]
        public string ThumbNailPath { get; set; }

        /// <summary>
        /// Gets or sets Tile pyramid URL.
        /// </summary>
        [DataMember]
        public string TilePyramidPath { get; set; }

        /// <summary>
        /// Gets or sets WTML URL. 
        /// </summary>
        [DataMember]
        public string WtmlPath { get; set; }

        /// <summary>
        /// Gets or sets WTML details.
        /// </summary>
        [DataMember]
        public WTMLCollection WtmlDetails { get; set; }
    }
}