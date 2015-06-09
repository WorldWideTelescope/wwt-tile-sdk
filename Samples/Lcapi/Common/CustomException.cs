//-----------------------------------------------------------------------
// <copyright file="CustomException.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2010. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Microsoft.Research.Wwt.Sdk.Samples
{
    /// <summary>
    /// This class is responsible for custom exception.
    /// </summary>
    [Serializable]
    public class CustomException : Exception
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CustomException class.
        /// </summary>
        public CustomException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CustomException class.
        /// </summary>
        /// <param name="message">
        /// String Message
        /// </param>
        public CustomException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CustomException class. 
        /// </summary>
        /// <param name="message">
        /// Message that describes the Error
        /// </param>
        /// <param name="innerException">
        /// The Exception is the cause of Current Exception
        /// </param>
        public CustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the CustomException class. 
        /// </summary>
        /// <param name="info">
        /// Holds the serialized object data about the Exception being thrown
        /// </param>
        /// <param name="context">
        /// Instance of System.Runtime.Serialization.StreamingContext
        /// </param>
        protected CustomException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }
}
