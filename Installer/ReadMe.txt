WorldWide Telescope SDK
(c) Copyright Microsoft Corporation, 2011. All rights reserved.
Website: http://www.worldwidetelescope.org

===================
System Requirements
===================

=====================
Software Requirements
=====================
• Windows 7 (Ultimate or Enterprise recommended), Windows Server 2008 R2 (Older versions of Windows are not recommended)
• WorldWide Telescope Windows Client (Version 3.0.1 or above)
• .NET Framework 4.0
• Visual Studio 2010
• Web Deploy Tool (this is required if the Sharing Service tool is to be deployed)
• IIS with default web site and default app pool configured to use .Net 4.0

=====================
Hardware Requirements
=====================
• A 64-bit dual-core processor is recommended, though a 32-bit is sufficient.
• 4 GB of physical memory

==========
Features 
==========
1. Tile Pyramid SDK, Samples and Documentation
2. Tile Generator Tool
3. LCAPI Sample
4. Tile Service Sample
5. Community Service Sample
6. Sphere Toaster Tool
7. Study Chopper Tool

============
Known Issues
============
1. The SDK currently provides samples for the tiling of image files (.png,.jpg,.jpeg,.tif,.tiff) and xyz files (comma separated containing lat, long and depth values). 
   The SDK source code is provided so it can be extended to support other input file formats. 
2. Processing large sets of data is dependent on the amount of memory available in your system. For larger datasets consider increasing the available memory on your system. 
3. When using the Sharing Service with a Community folder that is configured on a remote network location WorldWide Telescope might time-out before the whole payload file is loaded. 
   In this case reselecting the Community thumbnail in WorldWide Telescope should load the community payload file in stages - several iterations of this may be necessary for a large payload file.
4. In LCAPI Sample, when loading a file, "Data visualization Layer" dialog shown by WWT will be hidden behind the LCAPI sample window and will need to be activated manually.
5. The Tile Generator tool only supports images with longitude values increasing from left to right and latitude values decreasing from top to bottom.

================================================
Support, Feedback, Bug Reports, Feature Requests
================================================
Ask questions and provide feedback.
1. To ask technical questions or to let us know how you are using the software, you can also e-mail us at wwtefbk@microsoft.com.
