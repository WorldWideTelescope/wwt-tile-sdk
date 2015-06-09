//-----------------------------------------------------------------------
// <copyright file="TestBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Microsoft.Research.Wwt.Sdk.Core.Test
{
    public abstract class TestBase
    {
        static TestBase()
        {
            TestDataPath = Environment.CurrentDirectory;
        }

        protected static string TestDataPath { get; set; }
    }
}
