//-----------------------------------------------------------------------
// <copyright file="PropertyChangeBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.Wwt.TileGenerator
{
    using System.ComponentModel;

    /// <summary>
    /// This is the base class to be used by all viewModels that want the 
    /// property change notifications to be raised for the binding system.
    /// </summary>
    public abstract class PropertyChangeBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        /// This event is handled by the binding system to refresh the data when the value is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This is the method that should be called by derived classes to raise the property change event notification 
        /// so that the binding systems refreshes the data on the UI.
        /// </summary>
        /// <param name="propertyName">propertyName that is changed / updated</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChangedEventArgs arguments = new PropertyChangedEventArgs(propertyName);
                handler(this, arguments);
            }
        }

        #endregion
    }
}
