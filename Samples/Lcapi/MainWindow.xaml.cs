//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2010. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Microsoft.Win32;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Current latitude.
        /// </summary>
        private double latitude = 0.0;

        /// <summary>
        /// Current longitude.
        /// </summary>
        private double longitude = 0.0;

        /// <summary>
        /// Current azimuth.
        /// </summary>
        private double azimuth = 0.0;

        /// <summary>
        /// Current angle of view.
        /// </summary>
        private double lookAngle = 0.0;

        /// <summary>
        /// Current zoom level.
        /// </summary>
        private double zoomLevel = 16.0;

        /// <summary>
        /// IP address of the machine where WWT is installed.
        /// </summary>
        private string machineIp;

        /// <summary>
        /// Layer id for the created layer
        /// </summary>
        private string layerId;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // Get a string representing the loopback interface.
            this.machineIp = this.GetIp().ToString();
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.OnLoaded);

            // Looks for any network address change in the local machine.
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(OnNetworkAddressChanged);
        }

        /// <summary>
        /// Network address changed event handler which will be resetting the machineIp
        /// for the changed IP address so that the call to WWT LCAPI will go through smoothly.
        /// </summary>
        /// <param name="sender">Null, since there is no sender</param>
        /// <param name="e">Empty event arguments</param>
        private void OnNetworkAddressChanged(object sender, EventArgs e)
        {
            // Reset target machine with the changed IP address.
            this.machineIp = this.GetIp().ToString();
        }

        /// <summary>
        /// Initializes the application with default values.
        /// </summary>
        /// <param name="sender">
        /// Application Window.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.RefreshLayers(false, false);
            }
            catch (CustomException ex)
            {
                ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Instantly flies to New York.
        /// </summary>
        /// <param name="sender">
        /// Fly To button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnFlyToInstant(object sender, RoutedEventArgs e)
        {
            try
            {
                // Change mode to Earth - to ensure that we are on earth.
                if (this.cmbModes.SelectedIndex != 0)
                {
                    this.cmbModes.SelectionChanged -= new SelectionChangedEventHandler(OnModeSelected);
                    this.cmbModes.SelectedIndex = 0;
                    this.cmbModes.SelectionChanged += new SelectionChangedEventHandler(OnModeSelected);
                }

                this.SetMode("Earth");
                this.SetNewYorkCoordinates();
                this.SetPerspective(true);
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Traverses the path to New York.
        /// </summary>
        /// <param name="sender">
        /// Fly To button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnFlyToTraverse(object sender, RoutedEventArgs e)
        {
            try
            {
                // Change mode to Earth - to ensure that we are on earth.
                if (this.cmbModes.SelectedIndex != 0)
                {
                    this.cmbModes.SelectionChanged -= new SelectionChangedEventHandler(OnModeSelected);
                    this.cmbModes.SelectedIndex = 0;
                    this.cmbModes.SelectionChanged += new SelectionChangedEventHandler(OnModeSelected);
                }

                this.SetMode("Earth");
                this.SetNewYorkCoordinates();
                this.SetPerspective(false);
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Retrieve all the properties for a specified layer.
        /// This includes list  Spreadsheet, 3d Model, Shape file and ImageSet.
        /// </summary>
        /// <param name="sender">
        /// Get Properties button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnGetLayerProperties(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=getprops&id={1}", this.machineIp, layerId);
                string payload = string.Empty;
                string response = WWTRequest.Send(url, string.Empty);
                this.PrintSendAndReceive(url, response);
                this.PrintSendAndReceiveHeader("Get Properties");
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Retrieve a value of a single layer property.
        /// The property can include all the layer properties which can be set through LCAPI.
        /// </summary>
        /// <param name="sender">
        /// Get Opacity button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnGetOpacity(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=getprop&id={1}&propname=Opacity", this.machineIp, layerId);
                string payload = string.Empty;
                string response = WWTRequest.Send(url, payload);
                this.PrintSendAndReceive(url, response);
                this.PrintSendAndReceiveHeader("Get Opacity");
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Sets value for a single layer property.
        /// In this case PlotType value is set for the created layer in WWT.
        /// </summary>
        /// <param name="sender">
        /// Set PlotType button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnSetPushPin(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=setprop&id={1}&propname=PlotType&propvalue=PushPin", this.machineIp, layerId);
                string payload = string.Empty;
                string response = WWTRequest.Send(url, payload);
                this.PrintSendAndReceive(url, response);
                this.PrintSendAndReceiveHeader("Use Push Pin");
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Sets multiple properties of the layer.
        /// Multiple properties of a layer can be set at a time.
        /// </summary>
        /// <param name="sender">
        /// Set Properties button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnSetMultiProperties(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=setprops&id={1}", this.machineIp, layerId);
                string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Layer Opacity=\".5\" ColorValue=\"ARGBColor:255:255:0:0\"> </Layer></LayerApi>";
                string response = WWTRequest.Send(url, payload);
                this.PrintSendAndReceive(url + "\r\n" + payload, response);
                this.PrintSendAndReceiveHeader("Set Properties");
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Highlights the selected layer in the layer manager in WWT.
        /// The layer gets activated when a new layer is created.
        /// </summary>
        /// <param name="sender">
        /// Activate Layer button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnActivateLayer(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=activate&id={1}", this.machineIp, layerId);
                string payload = string.Empty;
                string response = WWTRequest.Send(url, payload);
                this.PrintSendAndReceive(url, response);
                this.PrintSendAndReceiveHeader("Activate");
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Refreshes the layer list with all the layers created in WWT.
        /// </summary>
        /// <param name="sender">
        /// Refresh Layer button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            try
            {
                this.RefreshLayers(true, false);
            }
            catch (CustomException ex)
            {
                ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a layer permanently from WWT.
        /// </summary>
        /// <param name="sender">
        /// Delete Layer button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=delete&id={1}", this.machineIp, layerId);
                string payload = string.Empty;
                string response = WWTRequest.Send(url, payload);
                this.PrintSendAndReceive(url, response);
                this.PrintSendAndReceiveHeader("Delete");

                this.RefreshLayers(false, false);
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Gets the GUID of the layer selected from the tree view of all 
        /// the layers and reference frames.
        /// </summary>
        /// <param name="sender">
        /// Layer Tree View.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnLayerSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedNode = this.layersTree.SelectedItem as TreeViewItem;
            if (selectedNode != null)
            {
                if (selectedNode.Tag != null)
                {
                    layerId = selectedNode.Tag.ToString();
                    SetCommonOperationsButtons(false);
                    this.SetLayerButtons(true);
                }
                else
                {
                    layerId = string.Empty;
                    this.btnCreateGroup.IsEnabled = (selectedNode.Name == "ReferenceFrame" || selectedNode.Name == "LayerGroup");
                    this.btnCreateLayer.IsEnabled = (selectedNode.Name == "ReferenceFrame" || selectedNode.Name == "LayerGroup");
                    this.btnLoadFiles.IsEnabled = (selectedNode.Name == "ReferenceFrame" || selectedNode.Name == "LayerGroup");
                    this.SetLayerButtons(false);
                }
            }
        }

        /// <summary>
        /// Changes the current look at mode to Earth, Planet, Sky, Panorama, SolarSystem..
        /// </summary>
        /// <param name="sender">
        /// Mode combo box.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnModeSelected(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem item = this.cmbModes.SelectedItem as ComboBoxItem;
                this.SetMode(item.Content.ToString());
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new layer in WWT through header data and updates the properties
        /// of layer created and pass data to be attached to the layer which is created.
        /// </summary>
        /// <param name="sender">
        /// Create Layer button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnCreateNewLayer(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem selectedNode = this.layersTree.SelectedItem as TreeViewItem;
                if (selectedNode != null)
                {
                    // Call New to create the layer.
                    string url = string.Format(
                        "http://{0}:5050/layerApi.aspx?cmd=new&datetime={1}&name={2}&frame={3}&color={4}",
                        this.machineIp,
                        "1/9/2009 3:44:38 AM",
                        "New Layer",
                        selectedNode.Header.ToString(),
                        "FFFF0000");
                    string payload = "TIME\tLAT\tLON\tDEPTH\tMAG\tColor";
                    string response = WWTRequest.Send(url, payload);
                    this.PrintSendAndReceive(url, response);
                    this.PrintSendAndReceiveHeader("Step 1 : Create new layer");

                    if (!string.IsNullOrEmpty(response))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(response);
                        XmlNode node = doc["LayerApi"];
                        layerId = node.InnerText;

                        url = string.Format("http://{0}:5050/layerApi.aspx?cmd=setprops&id={1}", this.machineIp, layerId);

                        payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Layer Name=\"New Layer\" LatColumn=\"1\" LngColumn=\"2\" GeometryColumn=\"-1\"" +
                        " ColorMapColumn=\"5\" AltColumn=\"3\" StartDateColumn=\"0\" EndDateColumn=\"-1\" SizeColumn=\"4\" NameColumn=\"0\" Decay=\"16\" ScaleFactor=\"1\" Opacity=\"1\" StartTime=\"1/1/0001 12:00:00 AM\" EndTime=\"12/31/9999 11:59:59 PM\" FadeSpan=\"00:00:00\" ColorValue=\"ARGBColor:255:255:0:0\" AltType=\"Depth\" MarkerScale=\"World\"" +
                        " AltUnit=\"Kilometers\" RaUnits=\"Hours\" PointScaleType=\"Power\" FadeType=\"None\" PlotType=\"Gaussian\" MarkerIndex=\"0\" ShowFarSide=\"true\" TimeSeries=\"false\" /></LayerApi>";
                        response = WWTRequest.Send(url, payload);

                        this.PrintSendAndReceive(url + "\r\n" + payload, response);
                        this.PrintSendAndReceiveHeader("Step 2 : Update layer properties");

                        // Call Update to update the layer with data.
                        url = string.Format("http://{0}:5050/layerApi.aspx?cmd=update&id={1}", this.machineIp, layerId);
                        string lineBuffer = "1/9/2009 3:44\t0\t0\t10\t7\tRed\r\n1/9/2009 6:28\t0\t10\t243\t9\tBlue\r\n1/9/2009 6:54\t-5\t5\t35\t8\t90%Green";
                        response = WWTRequest.Send(url, lineBuffer);

                        this.PrintSendAndReceive(url + "\r\n" + lineBuffer, response);
                        this.PrintSendAndReceiveHeader("Step 3 : Send layer data");

                        url = string.Format("http://{0}:5050/layerApi.aspx?cmd=showlayermanager", this.machineIp);
                        response = WWTRequest.Send(url, string.Empty);
                        this.PrintSendAndReceive(url, response);
                        this.PrintSendAndReceiveHeader("Step 4 : Show Layer Manager Pane in WWT");
                    }

                    this.RefreshLayers(false, true);
                    this.tabControl.SelectedIndex = 0;
                }
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Clears the output window.
        /// </summary>
        /// <param name="sender">
        /// Clear button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnClearOutput(object sender, RoutedEventArgs e)
        {
            this.txtOutput.Text = string.Empty;
        }

        /// <summary>
        /// Clears the output window.
        /// </summary>
        /// <param name="sender">
        /// Clear button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnClearOutputResponse(object sender, RoutedEventArgs e)
        {
            this.txtOutputResponse.Text = string.Empty;
        }

        /// <summary>
        /// Creates a layer group.
        /// </summary>
        /// <param name="sender">
        /// Create Group button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnCreateGroup(object sender, RoutedEventArgs e)
        {
            try
            {
                var layerGroup = new LayerGroup();
                layerGroup.Owner = this;
                layerGroup.ShowDialog();
                if (!string.IsNullOrEmpty(layerGroup.LayerGroupName))
                {
                    TreeViewItem selectedNode = this.layersTree.SelectedItem as TreeViewItem;
                    if (selectedNode != null)
                    {
                        string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=group&frame={1}&name={2}", this.machineIp, selectedNode.Header.ToString(), layerGroup.LayerGroupName);
                        string payload = string.Empty;
                        string response = WWTRequest.Send(url, payload);
                        this.PrintSendAndReceive(url, response);
                        this.PrintSendAndReceiveHeader("Create Group");

                        url = string.Format("http://{0}:5050/layerApi.aspx?cmd=showlayermanager", this.machineIp);
                        response = WWTRequest.Send(url, string.Empty);
                        this.PrintSendAndReceive(url, response);
                        this.PrintSendAndReceiveHeader("Show Layer Manager Pane in WWT");

                        this.RefreshLayers(false, false);
                    }
                }
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// The load specifies a data file which can be loaded to WWT
        /// with some optional parameters, to apply to a new layer.
        /// </summary>
        /// <param name="sender">
        /// Load button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnLoadFiles(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();

                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".csv";

                dlg.Filter = "Text documents (.txt)|*.txt|CSV Files(.csv)|*.csv|Excel(.xlsx)|*.xlsx";

                Nullable<bool> result = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    string filename = dlg.FileName;
                    LoadFile(filename);
                }
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// The load specifies a data file which can be loaded to WWT
        /// with some optional parameters, to apply to a new layer.
        /// </summary>
        /// <param name="filePath">Full file path.</param>
        private void LoadFile(string filePath)
        {
            TreeViewItem selectedNode = this.layersTree.SelectedItem as TreeViewItem;
            if (selectedNode != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=load&frame={1}&filename={2}&name={3}", this.machineIp, selectedNode.Header.ToString(), filePath, fileName);
                string payload = string.Empty;
                string response = WWTRequest.Send(url, payload);
                this.PrintSendAndReceive(url, response);
                this.PrintSendAndReceiveHeader("Load File");
                this.RefreshLayers(false, false);
            }
        }

        /// <summary>
        /// Gets the current version for the 
        /// WWT running on the system.
        /// </summary>
        /// <param name="sender">
        /// Get Version button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnGetVersion(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=version", this.machineIp);
                string payload = string.Empty;
                string response = WWTRequest.Send(url, payload);
                this.PrintSendAndReceive(url, response);
                this.PrintSendAndReceiveHeader("Get Version");
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Gets the IP address of the local machine.
        /// </summary>
        /// <returns>
        /// IP address of the local machine.
        /// </returns>
        private IPAddress GetIp()
        {
            IPAddress ip = IPAddress.Loopback;
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = address;
                    break;
                }
            }

            return ip;
        }

        /// <summary>
        /// Sets properties to New York with the specified latitude, longitude and zoom level.
        /// </summary>
        private void SetNewYorkCoordinates()
        {
            this.latitude = 40.70;
            this.longitude = -74;
            this.zoomLevel = 3.0;
            this.azimuth = Math.PI / 2.0;
            this.lookAngle = -Math.PI / 4.0;
        }

        /// <summary>
        /// Changes the view to one of Earth, Planet, Sky, Panorama, SolarSystem in WWT.
        /// </summary>
        /// <param name="mode">
        /// Mode to be set.
        /// </param>
        private void SetMode(string mode)
        {
            string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=mode&lookat=" + mode, this.machineIp);
            string payload = string.Empty;
            string response = WWTRequest.Send(url, payload);
            this.PrintSendAndReceive(url, response);
            this.PrintSendAndReceiveHeader("Change mode to");
        }

        /// <summary>
        /// Fetches the structure of the layers and layer group names (in an XML document format)
        /// that are currently in the layer manager in WWT.
        /// </summary>
        /// <param name="displayResponse">show the response in response output</param>
        /// <param name="isLayerCreated">Checks if the call has come from Create layer</param>
        private void RefreshLayers(bool displayResponse, bool isLayerCreated)
        {
            try
            {
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=layerlist", this.machineIp);
                string payload = string.Empty;
                string response = WWTRequest.Send(url, payload);
                if (displayResponse)
                {
                    this.PrintSendAndReceive(url, response);
                    this.PrintSendAndReceiveHeader("Refresh Layer List");
                }

                this.layersTree.Items.Clear();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);

                XmlNode root = doc["LayerApi"];
                XmlNode layersNode = root["LayerList"];
                foreach (XmlElement element in layersNode.ChildNodes)
                {
                    TreeViewItem node = new TreeViewItem();
                    node.Header = element.Attributes["Name"].Value;
                    node.Name = element.Name;
                    if (element.Attributes["ID"] != null)
                    {
                        node.Tag = element.Attributes["ID"].Value;
                    }

                    this.layersTree.Items.Add(node);
                    node.IsExpanded = true;
                    foreach (XmlElement child in element.ChildNodes)
                    {
                        // If the layer is not of spread sheet layer type, then ignore them.
                        if (child.Attributes["Type"] != null && !child.Attributes["Type"].Value.Equals("SpreadSheetLayer", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        TreeViewItem childNode = new TreeViewItem();
                        childNode.Header = child.Attributes["Name"].Value;
                        childNode.Name = child.Name;
                        if (child.Attributes["ID"] != null)
                        {
                            childNode.Tag = child.Attributes["ID"].Value;
                        }

                        node.Items.Add(childNode);

                        // Select Earth.
                        if (childNode.Header.ToString().Equals("Earth", StringComparison.OrdinalIgnoreCase) && !isLayerCreated)
                        {
                            childNode.IsSelected = true;
                        }

                        foreach (XmlElement grandChild in child.ChildNodes)
                        {
                            // If the layer is not of spread sheet layer type, then ignore them.
                            if (grandChild.Attributes["Type"] != null && !grandChild.Attributes["Type"].Value.Equals("SpreadSheetLayer", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            TreeViewItem grandChildNode = new TreeViewItem();
                            grandChildNode.Header = grandChild.Attributes["Name"].Value;
                            grandChildNode.Name = grandChild.Name;
                            if (grandChild.Attributes["ID"] != null)
                            {
                                grandChildNode.Tag = grandChild.Attributes["ID"].Value;
                                if (isLayerCreated)
                                {
                                    if (grandChild.Attributes["ID"].Value == layerId)
                                    {
                                        grandChildNode.IsSelected = true;
                                        layersTree.Focus();
                                        grandChildNode.Focus();
                                        childNode.IsExpanded = true;
                                    }
                                }
                            }

                            childNode.Items.Add(grandChildNode);
                            foreach (XmlElement greatGrandChild in grandChild.ChildNodes)
                            {
                                // If the layer is not of spread sheet layer type, then ignore them.
                                if (greatGrandChild.Attributes["Type"] != null && !greatGrandChild.Attributes["Type"].Value.Equals("SpreadSheetLayer", StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }

                                TreeViewItem greatGrandChildNode = new TreeViewItem();
                                greatGrandChildNode.Header = greatGrandChild.Attributes["Name"].Value;
                                greatGrandChildNode.Name = greatGrandChild.Name;
                                if (greatGrandChild.Attributes["ID"] != null)
                                {
                                    greatGrandChildNode.Tag = greatGrandChild.Attributes["ID"].Value;
                                    if (isLayerCreated)
                                    {
                                        if (greatGrandChild.Attributes["ID"].Value == layerId)
                                        {
                                            greatGrandChildNode.IsSelected = true;
                                            layersTree.Focus();
                                            greatGrandChildNode.Focus();
                                            childNode.IsExpanded = true;
                                            grandChildNode.IsExpanded = true;
                                        }
                                    }
                                }

                                grandChildNode.Items.Add(greatGrandChildNode);
                            }
                        }
                    }
                }

                if (this.cmbModes.SelectedIndex <= 0)
                {
                    this.cmbModes.SelectedIndex = 0;
                }
            }
            catch (CustomException ex)
            {
                // Refresh has failed
                this.SetLayerButtons(false);
                this.SetCommonOperationsButtons(false);

                // WorldWide Telescope might not be open.
                throw ex;
            }
        }

        /// <summary>
        /// Sets the vantage point in the Client.
        /// </summary>
        /// <param name="instantType">
        /// Specifies whether the traversal is instantaneous or gradual.
        /// </param>
        private void SetPerspective(bool instantType)
        {
            string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=state&instant={1}&flyto={2},{3},{4},{5},{6}", this.machineIp, instantType.ToString(), this.latitude, this.longitude, this.zoomLevel, this.azimuth, this.lookAngle);
            string payload = string.Empty;
            string response = WWTRequest.Send(url, payload);
            this.PrintSendAndReceive(url, response);
            if (instantType)
            {
                this.PrintSendAndReceiveHeader("Jump to New York");
            }
            else
            {
                this.PrintSendAndReceiveHeader("Traverse to New York");
            }
        }

        /// <summary>
        /// Gets the view state from WWT.
        /// </summary>
        /// <param name="sender">Get state button on the UI.</param>
        /// <param name="e">Attributes of the event</param>
        private void OnGetState(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=state", this.machineIp);
                string payload = string.Empty;
                string response = WWTRequest.Send(url, payload);
                this.PrintSendAndReceive(url, response);
                this.PrintSendAndReceiveHeader("Get State");
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Gets the layer data from WWT for the specified layer
        /// It only fetches the data for the layer not the layer properties.
        /// </summary>
        /// <param name="sender">Get Layer Data button on the UI.</param>
        /// <param name="e">Attributes of the event</param>
        private void OnGetLayerData(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=get&id={1}", this.machineIp, layerId);
                string payload = string.Empty;
                string response = WWTRequest.GetLayerData(url);
                this.PrintSendAndReceive(url, response);
                this.PrintSendAndReceiveHeader("Get Layer Data");
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Writes the output from LCAPI.
        /// </summary>
        /// <param name="sendText">
        /// Text sent.
        /// </param>
        /// <param name="receiveText">
        /// Text received.
        /// </param>
        private void PrintSendAndReceive(string sendText, string receiveText)
        {
            string currentText = this.txtOutput.Text;
            this.txtOutput.Text = "Sending: " + sendText + Environment.NewLine;
            this.txtOutput.AppendText("----------------------------------------------------------------------------------------------------");
            this.txtOutput.AppendText(Environment.NewLine);
            this.txtOutput.AppendText(currentText);
            this.txtOutput.ScrollToHome();

            currentText = this.txtOutputResponse.Text;
            this.txtOutputResponse.Text = ("Received: " + Environment.NewLine + receiveText.Replace("\0", string.Empty) + Environment.NewLine);
            this.txtOutputResponse.AppendText("----------------------------------------------------------------------------------------------------");
            this.txtOutputResponse.AppendText(Environment.NewLine);
            this.txtOutputResponse.AppendText(currentText);
            this.txtOutputResponse.ScrollToHome();
        }

        /// <summary>
        /// Writes the header text for the send and receive command
        /// </summary>
        /// <param name="headerText">Header Text</param>
        private void PrintSendAndReceiveHeader(string headerText)
        {
            string currentText = this.txtOutput.Text;
            this.txtOutput.Text = headerText + Environment.NewLine;
            this.txtOutput.AppendText(currentText);
            this.txtOutput.ScrollToHome();

            currentText = this.txtOutputResponse.Text;
            this.txtOutputResponse.Text = headerText + Environment.NewLine;
            this.txtOutputResponse.AppendText(currentText);
            this.txtOutputResponse.ScrollToHome();
        }

        /// <summary>
        /// Enables or Disables Layer operations.
        /// </summary>
        /// <param name="enabled">
        /// True if layer operations should be enabled, False otherwise.
        /// </param>
        private void SetLayerButtons(bool enabled)
        {
            this.btnDelete.IsEnabled = enabled;
            this.btnOpacity.IsEnabled = enabled;
            this.btnActivate.IsEnabled = enabled;
            this.btnProperties.IsEnabled = enabled;
            this.btnSetPushpin.IsEnabled = enabled;
            this.btnSetMultiProps.IsEnabled = enabled;
            this.btnGetLayerData.IsEnabled = enabled;
        }

        /// <summary>
        /// Enables or Disables Common operations.
        /// </summary>
        /// <param name="enabled">True if common operations should be enabled, False otherwise.</param>
        private void SetCommonOperationsButtons(bool enabled)
        {
            this.btnCreateGroup.IsEnabled = enabled;
            this.btnCreateLayer.IsEnabled = enabled;
            this.btnLoadFiles.IsEnabled = enabled;
        }

        /// <summary>
        /// Shows error message to the user.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        private void ReportError(string errorMessage)
        {
            MessageBox.Show(errorMessage, "LCAPI Sample", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Move view for left, right and in and out
        /// </summary>
        /// <param name="moveString">tag for view.</param>
        private void MoveView(string moveString, string content)
        {
            try
            {
                string url = string.Format("http://{0}:5050/layerApi.aspx?cmd=move&move={1}", this.machineIp, moveString);
                string payload = string.Empty;
                string response = WWTRequest.Send(url, payload);
                this.PrintSendAndReceive(url, response);
                this.PrintSendAndReceiveHeader(content);
            }
            catch (CustomException ex)
            {
                // WorldWide Telescope might not be open.
                this.ReportError(ex.Message);
            }
        }

        /// <summary>
        /// Event is fired movements of WWT to various directions
        /// and zoom
        /// </summary>
        /// <param name="sender">Zoom and direction button</param>
        /// <param name="e">Routed event argument</param>
        private void OnZoom(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            MoveView(b.Tag.ToString(), b.Content.ToString());
        }

        /// <summary>
        /// Event is fired when help is clicked, It shows LCAPI help file , with all the required
        /// details on WWT.
        /// </summary>
        /// <param name="sender">Help button</param>
        /// <param name="e">Routed event</param>
        private void OnHelp(object sender, RoutedEventArgs e)
        {
            // Launch the help URL.
            try
            {
                Process.Start(Properties.Resources.LCAPIFileUrl);
            }
            catch (Win32Exception ex)
            {
                this.ReportError(ex.Message);
            }
        }
    }
}