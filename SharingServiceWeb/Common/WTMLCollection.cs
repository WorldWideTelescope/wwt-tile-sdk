//-----------------------------------------------------------------------
// <copyright file="WTMLCollection.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Collection of WTML file properties.
    /// </summary>
    [DataContract(Namespace = "")]
    public class WTMLCollection
    {
        /// <summary>
        /// Gets or sets name of WTML file.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Dem is enabled.
        /// </summary>
        [DataMember]
        public bool IsDemEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether elevation model is enabled.
        /// </summary>
        [DataMember]
        public bool IsElevationModel { get; set; }

        /// <summary>
        /// Gets or sets the project type for the Pyramid.
        /// </summary>
        [DataMember]
        public string ProjectType { get; set; }

        /// <summary>
        /// Gets or sets created date for WTML.
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets levels for WTML.
        /// </summary>
        [DataMember]
        public string Levels { get; set; }

        /// <summary>
        /// Gets or sets credits 
        /// </summary>
        [DataMember]
        public string Credit { get; set; }

        /// <summary>
        /// Gets or sets credit path.
        /// </summary>
        [DataMember]
        public string CreditPath { get; set; }
    }
}