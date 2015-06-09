//-----------------------------------------------------------------------
// <copyright file="CommonHelperMethods.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2010. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Configuration;
using System.Diagnostics;
using System.Windows.Automation;
using System.Net;
using System.Xml;
using System.Net.Sockets;
using System.Windows;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
namespace SharingService.Web.Automation.Util
{
    /// <summary>
    /// Common Methods Helper class
    /// </summary>
    public static class CommonHelperMethods
    {
        /// <summary>
        /// This method opens the application from the specified path in the config file.
        /// </summary>
        /// <returns>bool(Start Application Success - TRUE or Failure - FALSE)</returns>
        public static bool StartApplication()
        {
            ProcessStartInfo startInfo = null;
            startInfo = new ProcessStartInfo(ConfigurationManager.AppSettings["IEExploreerExePath"]);

            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.UseShellExecute = false;
            try
            {
                Process.Start(startInfo);
                return true;
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// This method returns an element searched by its name from the tree.
        /// </summary>
        /// <param name="root">Automation Element of the control</param>
        /// <param name="name">Name of the string</param>
        /// <param name="recursive">bool(Recursive - TRUE Or FALSE)</param>
        /// <returns>Automation Element</returns>
        public static AutomationElement GetElement(AutomationElement root, string name, bool recursive)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            else
            {
                PropertyCondition condName = new PropertyCondition(AutomationElement.NameProperty, name);
                return root.FindFirst(recursive ? TreeScope.Descendants : TreeScope.Children, condName);
            }
        }

        /// <summary>
        /// This method returns an element searched by its control type from the tree.
        /// </summary>
        /// <param name="root">Root Element of the control</param>
        /// <param name="controlType">Control Type of the element</param>
        /// <returns>Automation Element of teh control</returns>
        public static AutomationElement GetElement(AutomationElement root, ControlType controlType)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            if (controlType == null)
            {
                throw new ArgumentNullException("controlType");
            }
            else
            {
                PropertyCondition condType = new PropertyCondition(AutomationElement.ControlTypeProperty, controlType);
                return root.FindFirst(TreeScope.Descendants, condType);
            }
        }

        /// <summary>
        /// This method returns an element searched by its Automation ID from the tree.
        /// </summary>
        /// <param name="root">Root Element of the control</param>
        /// <param name="id">Automation Id of the control</param>
        /// <param name="recursive">Returns true if success else false</param>
        /// <returns>Automation Element of the control</returns>
        public static AutomationElement GetElementById(AutomationElement root, string id, bool recursive)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            else
            {
                PropertyCondition condName = new PropertyCondition(AutomationElement.AutomationIdProperty, id);
                return root.FindFirst(recursive ? TreeScope.Descendants : TreeScope.Children, condName);
            }
        }
    }
}
