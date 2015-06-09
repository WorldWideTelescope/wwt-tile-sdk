//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;

namespace Microsoft.Research.Wwt.TileGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            InitializeViewmodel();
        }

        /// <summary>
        /// Start over clicked on the tile generator.
        /// </summary>
        /// <param name="sender">Tile generator</param>
        /// <param name="e">Routed event</param>
        private void OnStartOverClickedEvent(object sender, System.EventArgs e)
        {
            InitializeViewmodel();
        }

        /// <summary>
        /// Event is fired on click of close of main window.
        /// </summary>
        /// <param name="sender">Main window</param>
        /// <param name="e">Routed event</param>
        private void OnClose(object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event is fired on minimize click of the main window
        /// </summary>
        /// <param name="sender">Main window</param>
        /// <param name="e">Routed event</param>
        private void OnMinimizeClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Initialize view model.
        /// </summary>
        private void InitializeViewmodel()
        {
            TileGeneratorViewModel viewModel = new TileGeneratorViewModel();
            viewModel.Close += new System.EventHandler(OnClose);
            viewModel.StartOverClickedEvent += new System.EventHandler(OnStartOverClickedEvent);
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Event is fired when the window is moved through mouse down event.
        /// </summary>
        /// <param name="sender">Main window</param>
        /// <param name="e">Routed event</param>
        private void OnDragMoveWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}