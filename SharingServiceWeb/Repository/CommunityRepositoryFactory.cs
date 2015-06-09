//-----------------------------------------------------------------------
// <copyright file="CommunityRepositoryFactory.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Class decides on the instance of Community repository.
    /// </summary>
    public static class CommunityRepositoryFactory
    {
        /// <summary>
        /// Creates a community repository instance.
        /// </summary>
        /// <returns>Community repository instance.</returns>
        public static ICommunityRepository Create()
        {
            return new LocalCommunityRepository();
        }
    }
}