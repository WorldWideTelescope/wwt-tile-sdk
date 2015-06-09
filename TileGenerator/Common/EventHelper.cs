//-----------------------------------------------------------------------
// <copyright file="EventHelper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Research.Wwt.TileGenerator
{
    /// <summary>
    /// Contains extension methods for raising (firing) events.
    /// </summary>
    public static class EventHelper
    {
        /// <summary>
        /// Fires the specified event handler delegate with the specified sender and event arguments.
        /// </summary>
        /// <typeparam name="TArgs">
        /// The type of the event arguments.
        /// </typeparam>
        /// <param name="handler">
        /// The event handler delegate.
        /// </param>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="args">
        /// The event arguments.
        /// </param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnFire<TArgs>(this EventHandler<TArgs> handler, object sender, TArgs args) where TArgs : EventArgs
        {
            // To make sure RACE event is not occurred while raising an event in the context of multiple threads.
            EventHandler<TArgs> handlerClone = handler;
            if (handlerClone != null)
            {
                handlerClone(sender, args);
            }
        }

        /// <summary>
        /// Fires the specified event handler delegate with the specified sender and event arguments.
        /// </summary>
        /// <param name="handler">
        /// The event handler delegate.
        /// </param>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="args">
        /// The event arguments.
        /// </param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnFire(this EventHandler handler, object sender, EventArgs args)
        {
            // To make sure RACE event is not occurred while raising an event in the context of multiple threads.
            EventHandler handlerClone = handler;
            if (handlerClone != null)
            {
                handlerClone(sender, args);
            }
        }
    }
}
