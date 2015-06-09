//-----------------------------------------------------------------------
// <copyright file="Utility.cs" company="Microsoft Corporation">
// Copyright  Microsoft Corporation 2010. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SharingService.Web.Automation
{
    /// <summary>
    /// This class contains the all the common functions/variables used by all the automation test cases.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Initializes a new instance of the Utility class.
        /// </summary>
        /// <param name="filePath"> Path to the Configuration file containing the input data.</param>
        public Utility(string filePath)
        {
            XmlUtil = new XmlUtility(filePath);
        }

        public XmlUtility XmlUtil { get; set; }

        /// <summary>
        /// Gets the file content for the file path passed. 
        /// If the file doesnt exist throw the exception.
        /// </summary>
        /// <param name="filePath">File path, the content of which to be read.</param>
        /// <returns>Content of the Text file.</returns>
        internal static string GetFileContent(string filePath)
        {
            string fileContent = string.Empty;

            // Check if the File path exists, if not throw exception.
            if (File.Exists(filePath))
            {
                using (StreamReader textFile = new StreamReader(filePath))
                {
                    fileContent = textFile.ReadToEnd();
                }
            }
            else
            {
                throw new FileNotFoundException(string.Format((IFormatProvider)null, "File '{0}' not found.", filePath));
            }

            return fileContent;
        }
    }
}
