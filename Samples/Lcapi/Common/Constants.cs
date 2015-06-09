//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2010. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// Constants class
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Text which identifies LayerApi.
        /// </summary>
        public const string LCAPIElementName = "LayerApi";

        /// <summary>
        /// Text which identifies error in processing.
        /// </summary>
        public const string LCAPIErrorText = "Error";

        /// <summary>
        /// Text which Identifies error in connection
        /// </summary>
        public const string LCAPIConnectionErrorText = "IP Not Authorized";

        /// <summary>
        /// Status attribute name
        /// </summary>
        public const string StatusAttribute = "Status";

        /// <summary>
        /// Regular expression to identify Hex Character Pattern in LCAPI response 
        /// </summary>
        public const string HexCharacterPattern = @"\p{C}+";

        /// <summary>
        /// Gets the default response xml string.
        /// </summary>
        public static string DefaultErrorResponse
        {
            get
            {
                return (new System.Xml.Linq.XElement(
                    Constants.LCAPIElementName,
                    new System.Xml.Linq.XElement(Constants.StatusAttribute, Constants.LCAPIErrorText)))
                    .ToString(System.Xml.Linq.SaveOptions.DisableFormatting);
            }
        }
    }
}
