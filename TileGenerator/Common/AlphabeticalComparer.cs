//-----------------------------------------------------------------------
// <copyright file="AlphabeticalComparer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Research.Wwt.TileGenerator
{
    /// <summary>
    /// Compares the string format for a object.
    /// </summary>
    public class AlphabeticalComparer<T> : IComparer<T>
    {
        /// <summary>
        /// Compares the string format for a object.
        /// </summary>
        /// <param name="x">Object to be compared x</param>
        /// <param name="y">Object to be compared y</param>
        /// <returns>If the string format is less than, equal to or greater than the other</returns>
        public int Compare(T x, T y)
        {
            return ((new CaseInsensitiveComparer(CultureInfo.InvariantCulture)).Compare(x.ToString(), y.ToString()));
        }
    }
}
