//-----------------------------------------------------------------------
// <copyright file="WWTRequest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2010. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// Class having implementation for WWTRequest which talks to WWT to send the commands LCAPI.
    /// </summary>
    public static class WWTRequest
    {
        /// <summary>
        /// This function is used to send request to LCAPI.
        /// </summary>
        /// <param name="command">Uri of the API</param>
        /// <param name="payload">Data to be uploaded</param>
        /// <returns>Response of the operation</returns>
        public static string Send(string command, string payload)
        {
            string response = string.Empty;
            using (WebClient client = new WebClient())
            {
                try
                {
                    response = client.UploadString(command, payload);
                    if (!string.IsNullOrEmpty(response))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(response);
                        XmlNode node = doc[Constants.LCAPIElementName];
                        string s = node.InnerText;

                        // This is valid response with error string for error happened because of the data
                        // Consuming it for the time being
                        if (s.Contains(Constants.LCAPIErrorText))
                        {
                            if (s.Contains(Constants.LCAPIConnectionErrorText))
                            {
                                Uri url = new Uri(command);
                                throw new CustomException(string.Format(System.Globalization.CultureInfo.InvariantCulture, Properties.Resources.ErrorLCAPIConnectionFailure, url.Host));
                            }
                            else
                            {
                                response = Constants.DefaultErrorResponse;
                                throw new CustomException(Properties.Resources.LCAPIErrorText);
                            }
                        }

                        response = FormatXml(response);
                    }
                }
                catch (XmlException exception)
                {
                    response = Constants.DefaultErrorResponse;
                    throw new CustomException(Properties.Resources.LCAPIErrorText, exception);
                }
                catch (WebException exception)
                {
                    response = Constants.DefaultErrorResponse;
                    throw new CustomException(Properties.Resources.WWTNotOpenErrorText, exception);
                }
            }

            return response;
        }

        /// <summary>
        /// This function is used to send request to LCAPI.
        /// </summary>
        /// <param name="command">Uri of the API</param>
        /// <returns>Response of the operation</returns>
        public static string GetLayerData(string command)
        {
            string response = string.Empty;
            using (WebClient client = new WebClient())
            {
                try
                {
                    response = client.UploadString(command, string.Empty);
                    XDocument doc = XDocument.Parse(response);
                    XElement node = doc.Element(Constants.LCAPIElementName);
                    string s = node.Value;

                    // This is valid response with error string for error happened because of the data
                    // Consuming it for the time being
                    if (s.Contains(Constants.LCAPIErrorText))
                    {
                        if (s.Contains(Constants.LCAPIConnectionErrorText))
                        {
                            Uri url = new Uri(command);
                            throw new CustomException(string.Format(System.Globalization.CultureInfo.InvariantCulture, Properties.Resources.ErrorLCAPIConnectionFailure, url.Host));
                        }
                        else
                        {
                            response = Constants.DefaultErrorResponse;
                            throw new CustomException(Properties.Resources.LCAPIErrorText);
                        }
                    }
                }
                catch (XmlException)
                {
                    // For valid response, string will not be XML. Consume this.
                }
                catch (WebException exception)
                {
                    response = Constants.DefaultErrorResponse;
                    throw new CustomException(Properties.Resources.WWTNotOpenErrorText, exception);
                }
            }
            return response;
        }

        /// <summary>
        /// Format Xml Response
        /// </summary>
        /// <param name="xml">Input Xml string</param>
        /// <returns>Formatted Xml</returns>
        private static string FormatXml(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var stringBuilder = new StringBuilder();
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
            };

            using (XmlWriter writer = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            {
                doc.Save(writer);  
            }
            
            return stringBuilder.ToString();
        }
    }
}
