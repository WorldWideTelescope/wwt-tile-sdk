@echo off

@echo.
@echo This will set up Sharing Service virtual directory under the Default Web Site.
@echo Make sure you have configured IIS and Default Web Site.
@echo Also, you need to have .Net Framework 4.0 and Web Deploy Tool (http://www.iis.net/download/webdeploy) installed.
@echo.

set /p Choice=Do you want to setup Sharing Service? [y/n]?
if /i not "%Choice%" == "y" (
goto :EXIT
)

SET Path=%Path%;%windir%\Microsoft.NET\Framework\v4.0.30319

@echo ON

MSBuild.exe /p:Configuration=Release "..\Tile Pyramid SDK\Source Code\Core\Sdk.Core.csproj"
MSBuild.exe /T:Package /p:Configuration=Release "Source Code\SharingServiceWeb\SharingService.Web.csproj"

COPY /Y SharingService.Web.deploy.cmd "Source Code\SharingServiceWeb\Bin\Package\SharingService.Web.deploy.cmd"

"Source Code\SharingServiceWeb\bin\Package\SharingService.Web.deploy.cmd" /Y

:EXIT
Pause