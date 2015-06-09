//-----------------------------------------------------------------------
// <copyright file="SharingServiceTestCases.cs" company="Microsoft Corporation">
// Copyright  Microsoft Corporation 2010. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Automation;
using System.Diagnostics;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharingService.Web.Automation.Util;
namespace SharingService.Web.Automation
{
    /// <summary>
    /// Test Automation code for Sharing Service test cases.
    /// </summary>
    [TestClass]
    public class SharingServiceTestCases
    {
        private static Utility utilityObj = new Utility(@"SharingServiceConfig.xml");

        /// <summary>
        /// Element which is the parent of the control on which action takes place
        /// </summary>
        private static AutomationElement parentElement;

        /// <summary>
        /// Control on which action takes place
        /// </summary>
        private static AutomationElement childElement;

        #region Helper Methods
        /// <summary>
        /// This method gets the ParentElement from the Root 
        /// </summary>
        /// <param name="parentName">Automation name of the current window</param>
        /// <param name="parentId">AutomationId of the current window</param>
        /// <returns>returns true if success else false</returns>
        public static bool GetParentElement(string parentName, string parentId)
        {
            try
            {
                if (parentId == "null")
                {
                    parentElement = CommonHelperMethods.GetElement(AutomationElement.RootElement, parentName, true);
                }
                else if (parentName == "null")
                {
                    parentElement = CommonHelperMethods.GetElementById(AutomationElement.RootElement, parentId, true);
                }
                return true;
            }
            catch (ElementNotAvailableException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// This method gets the list of ChildElements of the control type in the Window.
        /// </summary>
        /// <param name="type">Control type</param>       
        public static bool GetChildElement(ControlType type)
        {
            try
            {
                CommonHelperMethods.GetElement(AutomationElement.RootElement, type);
                return true;
            }
            catch (ElementNotAvailableException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// This method gets the ChildElement in the Window 
        /// </summary>
        /// <param name="childName">Automation name of the control</param>
        /// <param name="childId">AutomationId of the control</param>
        public static bool GetChildElement(string childName, string childId)
        {
            try
            {
                if (childId == "null")
                {
                    childElement = CommonHelperMethods.GetElement(AutomationElement.RootElement, childName, true);
                }
                else
                {
                    childElement = CommonHelperMethods.GetElementById(AutomationElement.RootElement, childId, true);
                }

                return true;
            }
            catch (ElementNotAvailableException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Copy Web config file To SharingServiceVirtualDirectory
        /// </summary>
        /// <returns></returns>
        public static void CopyWebConfigFile(string filePath)
        {
            // Get web Config File
            string currentDir = Environment.CurrentDirectory;
            string webConfigFile = string.Concat(currentDir, filePath);
            string virtualDir = ConfigurationManager.AppSettings["WebServicePhycalPath"];
            FileInfo webConfigFileInfo = new FileInfo(webConfigFile);
            webConfigFileInfo.CopyTo(string.Concat(virtualDir, "\\Web.config"), true);
        }

        /// <summary>
        /// General method to validate WTML files
        /// </summary>
        /// <param name="nodeName">Xml NodeName</param>
        /// <param name="isDemFile">Check For Dem File</param>
        /// <param name="isPlatFile">Check Foe Plat File</param>
        public static void ValidateWTMLTagAndDownloadFiles(string nodeName, bool isDemFile, bool isPlatFile)
        {
            // Get Values from XML File
            string filePath = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.InputFilePath);
            string outputFile = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.OutputWTMLFile1Node);
            string tagsNode = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.TagsNode);
            string filesCount = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.GridItemsNode);
            string demCheck = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.DemCheckNode);
            string checkTileImage = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.CheckTileNode);
            string thumbNailUrlCheck = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.ThumbanailImageValue);
            string thumbNailWindowName = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.ThumbnailImageWindowname);
            string sourceLocationText = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.SourceLocationNode);

            // Copy Web config file in Web virtual directory.
            CopyWebConfigFile(filePath);

            // Start IEExplorer
            CommonHelperMethods.StartApplication();
            System.Threading.Thread.Sleep(5000);
            if (GetParentElement(Constants.DefaultPageName, "null"))
            {
                PatternList.ParentElement = parentElement;
            }
            if (GetChildElement(Constants.AddressBoxName, "null"))
            {
                PatternList.ChildElement = childElement;
            }

            AutomationElement addressBox = childElement;

            // Enter Url in Web browser address box..
            PatternList.DoEnterText(ConfigurationManager.AppSettings["AspxPage"]);
            System.Threading.Thread.Sleep(3000);
            PatternList.DoSendKeys("{ENTER}");
            System.Threading.Thread.Sleep(16000);

            if (GetParentElement(Constants.SharingServiceWindowName, "null"))
            {
                PatternList.ParentElement = parentElement;
            }

            ////  Download WTML file on Local machine and validate
            if (GetChildElement("WWT - Tile Service Sample", "null"))
            {
                PatternList.ChildElement = childElement;
            }

            System.Threading.Thread.Sleep(2000);

            // Do tabs to find the dowload button.
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{TAB}");
            PatternList.DoSendKeys("{ENTER}");

            ////  PatternList.DoClick();
            System.Threading.Thread.Sleep(4000);

            if (GetChildElement(Constants.SaveButtonName, "null"))
            {
                PatternList.ChildElement = childElement;
            }

            string currentDir = Directory.GetCurrentDirectory();
            string updatedOutputWTMLPath = Path.Combine(currentDir + outputFile);

            // Get download button
            AutomationElementCollection downlodWTMLButton =
             childElement.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.SplitButton));

            PatternList.ChildElement = downlodWTMLButton[0];

            PatternList.DoClick();
            System.Threading.Thread.Sleep(2000);
            PatternList.DoSendKeys("{DOWN}");
            PatternList.DoSendKeys("{ENTER}");

            System.Threading.Thread.Sleep(4000);
            PatternList.DoSendKeys(updatedOutputWTMLPath);
            System.Threading.Thread.Sleep(2000);
            PatternList.DoSendKeys("{ENTER}");

            // Validate whether the WTML file is downloaded on local machine.
            Assert.IsTrue(File.Exists(updatedOutputWTMLPath));

            // Validate Urls of WTML file.
            ValidateWTMLUrls(updatedOutputWTMLPath, nodeName, isDemFile);

            // Close download PopUp.
            if (GetChildElement(Constants.CloseButtonName, "null"))
            {
                PatternList.ChildElement = childElement;
            }

            PatternList.DoClick();

            System.Threading.Thread.Sleep(7000);

            PatternList.ChildElement = addressBox;
            if (isDemFile)
            {
                // Enter Url in Web browser.
                PatternList.DoEnterText(checkTileImage);
                System.Threading.Thread.Sleep(3000);
                PatternList.DoSendKeys("{ENTER}");
                System.Threading.Thread.Sleep(3000);
                PatternList.DoEnterText(thumbNailUrlCheck);
                System.Threading.Thread.Sleep(3000);
                PatternList.DoSendKeys("{ENTER}");
                System.Threading.Thread.Sleep(10000);
                PatternList.ChildElement = addressBox;
                PatternList.DoEnterText(demCheck);
                System.Threading.Thread.Sleep(3000);
                PatternList.DoSendKeys("{ENTER}");

                // Close IE Brrowser.
                if (GetChildElement(Constants.CloseButtonName, "null"))
                {
                    PatternList.ChildElement = childElement;
                }

                PatternList.DoClick();
            }
            else if (isPlatFile)
            {
                // Enter Url in Web browser.
                PatternList.DoEnterText(checkTileImage);
                System.Threading.Thread.Sleep(3000);
                PatternList.DoSendKeys("{ENTER}");
                System.Threading.Thread.Sleep(8000);

                if (GetChildElement(thumbNailWindowName, "null"))
                {
                    PatternList.ChildElement = childElement;
                }               
            }

            System.Threading.Thread.Sleep(6000);

            // Close IE Brrowser.
            if (GetChildElement(Constants.CloseButtonName, "null"))
            {
                PatternList.ChildElement = childElement;
            }

            PatternList.DoClick();
        }

        /// <summary>
        /// Copy Web config file SharingServiceVirtualDirectory
        /// </summary>
        /// <returns></returns>
        public static void ValidateWTMLUrls(string filePath, string nodeName, bool isDEM)
        {
            // Get Values from xml
            string urlValue = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.UrlValue);
            string elevation = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.ElevationModelValue);
            string thumbnailImage = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.ThumbanailImageValue);
            string demUrl = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.DemUrlValue);

            // Get web Config File
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            XmlNode viewStateNode = doc.SelectSingleNode("//ImageSet");

            // Validate Url Values
            Assert.AreEqual(urlValue, viewStateNode.Attributes["Url"].Value);
            Assert.AreEqual(elevation, viewStateNode.Attributes["ElevationModel"].Value);
            if (isDEM)
            {
                Assert.AreEqual(demUrl, viewStateNode.Attributes["DemUrl"].Value);
            }
        }

        /// <summary>
        /// Validate Invalid WTML files.
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        public static void InvalidateWTMLFiles(string nodeName, string errorMessage)
        {
            // Get Values from XML File
            string filePath = utilityObj.XmlUtil.GetTextValue(nodeName, Constants.InputFilePath);

            // Copy Web config file in Web virtual directory.
            CopyWebConfigFile(filePath);

            // Start IEExplorer
            CommonHelperMethods.StartApplication();
            System.Threading.Thread.Sleep(5000);

            if (GetParentElement(Constants.DefaultPageName, "null"))
            {
                PatternList.ParentElement = parentElement;
            }
            if (GetChildElement(Constants.AddressBoxName, "null"))
            {
                PatternList.ChildElement = childElement;
            }

            AutomationElement addressBox = childElement;

            // Enter Url in Web browser address box..
            PatternList.DoEnterText(ConfigurationManager.AppSettings["AspxPage"]);
            System.Threading.Thread.Sleep(3000);
            PatternList.DoSendKeys("{ENTER}");
            System.Threading.Thread.Sleep(10000);

            // Get Parent Window name
            if (GetChildElement(Constants.SharingServiceWindowName, "null"))
            {
                PatternList.ParentElement = parentElement;
            }

            // Validate the error message
            if (GetChildElement(errorMessage, "null"))
            {
                PatternList.ChildElement = childElement;
            }
            System.Threading.Thread.Sleep(4000);

            Assert.AreEqual(errorMessage, childElement.Current.Name);

            // Close IE Brrowser.
            if (GetChildElement(Constants.CloseButtonName, "null"))
            {
                PatternList.ChildElement = childElement;
            }

            PatternList.DoClick();
        }

        #endregion Helper Methods

        /// <summary>
        /// Initialize() is called once during test execution before
        /// test methods in this test class are executed.
        /// </summary>
        [TestInitialize()]
        public void Initialize()
        {
            Process[] processs = Process.GetProcessesByName("WebDev.WebServer40");
            if (processs.Length >= 1)
            {
                foreach (Process webserver in processs)
                {
                    webserver.Kill();
                }
            }

            // settings
            string portNumber = ConfigurationManager.AppSettings["PortNumber"];
            string localHostUrl = string.Format("http://localhost:{0}", portNumber);
            string physicalPath = ConfigurationManager.AppSettings["WebServicePhycalPath"];

            // create a new process to start the ASP.NET Development ServerProcess
            Process process = new Process();

            // configure the web server
            process.StartInfo.FileName = ConfigurationManager.AppSettings["WebServerExePath"];
            process.StartInfo.Arguments = string.Format("/port:{0} /path:\"{1}\"", portNumber, physicalPath);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            // start the web server
            process.Start();
        }

        /// <summary>
        /// Cleanup() is called once during test execution after
        /// test methods in this class have executed unless
        /// this test class' Initialize() method throws an exception.
        /// </summary>
        [TestCleanup()]
        public void Cleanup()
        {
            // Get all the process running 
            Process[] processs = Process.GetProcessesByName("WebDev.WebServer40");

            if (processs.Length >= 1)
            {
                foreach (Process webserver in processs)
                {
                    webserver.Kill();
                }
            }
        }

        #region SharingService TestCases

        /// <summary>
        /// Validate Local Path WTML File
        /// </summary>
        [TestMethod]
        public void DownloadAndSaveWTMLFiles()
        {
            ValidateWTMLTagAndDownloadFiles(Constants.MercatorFileValidationNode, false, false);
        }

        /// <summary>
        /// Validate Local Path WTML File
        /// </summary>
        [TestMethod]
        public void ValidateGlacierBayToastWTMLFiles()
        {
            ValidateWTMLTagAndDownloadFiles(Constants.GlacierBayNode, true, false);
        }

        /// <summary>
        /// Validate Local Path WTML File
        /// </summary>
        [TestMethod]
        public void ValidateGlacierBayMercatorWTMLFiles()
        {
            ValidateWTMLTagAndDownloadFiles(Constants.GlacierBayMercatorNode, false, false);
        }

        /// <summary>
        /// Validate Plat file File
        /// </summary>
        [TestMethod]
        public void ValidatePlatFileWTMLFiles()
        {
            ValidateWTMLTagAndDownloadFiles(Constants.PlateFileValidationNode, false, true);
        }

        /// <summary>
        /// Validate Plat file File without pyramid folder
        /// </summary>
        [TestMethod]
        public void ValidatePlatFileWithoutPyramidFolder()
        {
            ValidateWTMLTagAndDownloadFiles(Constants.PlateFileValidationWithoutPyramidFolderNode, false, true);
        }

        /// <summary>
        /// InValidate UnAthorized pyramid directory.
        /// </summary>
        [TestMethod]
        public void InvalidateUnathorizedAccessException()
        {
            InvalidateWTMLFiles(Constants.UnauthorizedAccessDirNode, Constants.ErrorMessage);
        }

        /// <summary>
        /// InValidate WTML files with invalid tags.
        /// </summary>
        [TestMethod]
        public void InvalidateWTMLFilesWithInvalidTags()
        {
            InvalidateWTMLFiles(Constants.InvalidWTMLFilesNode, Constants.NoPyramidsErrorMessage);
        }

        /// <summary>
        /// Validate MultiplePlate DEM file File
        /// </summary>
        [TestMethod]
        public void ValidateMultiplePlateDEMFile()
        {
            ValidateWTMLTagAndDownloadFiles(Constants.MultiplePlateDEMFileNode, true, true);
        }

        /// <summary>
        /// Validate MultiplePlate File
        /// </summary>
        [TestMethod]
        public void ValidateMultiplePlateFile()
        {
            ValidateWTMLTagAndDownloadFiles(Constants.MultiplePlateFileValidationNode, false, true);
        }
        #endregion SharingService TestCases
    }
}

