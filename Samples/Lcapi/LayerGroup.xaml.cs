//-----------------------------------------------------------------------
// <copyright file="LayerGroup.xaml.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2010. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// Interaction logic for LayerGroup.xaml.
    /// </summary>
    public partial class LayerGroup : Window
    {
        /// <summary>
        /// Initializes a new instance of the LayerGroup class.
        /// </summary>
        public LayerGroup()
        {
            this.InitializeComponent();
            this.txtGroupName.Focus();
        }

        /// <summary>
        /// Gets the layer group name.
        /// </summary>
        public string LayerGroupName { get; private set; }

        /// <summary>
        /// Stores the layer group name.
        /// </summary>
        /// <param name="sender">
        /// Create Layer Group button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnCreateLayerGroup(object sender, RoutedEventArgs e)
        {
            this.LayerGroupName = this.txtGroupName.Text.Trim();
            this.Close();
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        /// <param name="sender">
        /// Cancel button.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Enables or disables the Create Layer Group button.
        /// </summary>
        /// <param name="sender">
        /// Create Layer Group textbox.
        /// </param>
        /// <param name="e">
        /// Attributes of the event.
        /// </param>
        private void OnLayerGroupNameChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.btnCreate.IsEnabled = this.txtGroupName.Text.Trim().Length > 0;
        }
    }
}
