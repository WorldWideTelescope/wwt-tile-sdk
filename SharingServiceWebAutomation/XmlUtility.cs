//-----------------------------------------------------------------------
// <copyright file="XmlUtility.cs" company="Microsoft Corporation">
// Copyright  Microsoft Corporation 2010. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SharingService.Web.Automation
{
     /// <summary>
    /// This class contains the all the xml related functions.
    /// </summary>
    public class XmlUtility
    {
        private XmlDocument xmlDoc;

        /// <summary>
        /// Initializes a new instance of the XmlUtility class.
        /// </summary>
        /// <param name="xmlFilePath">Path of the XML file containing the config values.</param>
        public XmlUtility(string xmlFilePath)
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
        }

        /// <summary>
        /// Returns the Text value for the nodes specified from the configuration file.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, from which the text value to be read.</param>
        /// <returns>Text with in the node.</returns>
        internal string GetTextValue(string parentNode, string nodeName)
        {
            XmlNode actualNode = GetNode(parentNode, nodeName);
            return actualNode.InnerText;
        }

        /// <summary>
        /// Returns the contents of the file for the Path specified.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, from which the file path to be read.</param>
        /// <returns>Contents of file for the path specified.</returns>
        internal string GetFileTextValue(string parentNode, string nodeName)
        {
            XmlNode actualNode = GetNode(parentNode, nodeName);
            string textValue = Utility.GetFileContent(actualNode.InnerText);
            return textValue;
        }

        /// <summary>
        /// Returns the node from the configuration file.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, which needs to be returned.</param>
        /// <returns>Xml node which matches the parameter passed.</returns>
        internal XmlNode GetNode(string parentNode, string nodeName)
        {
            XmlNode lst = null;

            lst = xmlDoc.ChildNodes[1];
            string xmlXPath = string.Concat("/AutomationTest/", parentNode, "/", nodeName);

            XmlNode childNode = lst.SelectSingleNode(xmlXPath);

            if (null == childNode)
            {
                throw new XmlException(string.Format((IFormatProvider)null, "Could not find the Xpath '{0}'", xmlXPath));
            }
            return childNode;
        }
    }
}
