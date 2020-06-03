# PowerShell Graph Connect Module

**Note:** This module is a work in progress and is not ready for production use.

## Description

This module is to provide a module agnostic solution to create access tokens for a registered application in an Azure AD tenant for the Microsoft Graph API in PowerShell. The goal of this module is to make it easier to build modules/scripts entirely in PowerShell that use this module to authenticate to the Graph API.

## How to Build Module

### Pre-requisites

- .Net Core SDK
- Operating Systems:
 - Windows
 - macOS
 - *nix-based (That support .Net Core)

### Building

In the project directory, run the following:

```powershell
PS .\pwsh-graph-connect > .\build.ps1
```

You should receive an output like the one below:

```
Starting PowerShell module build...
Build directory already clear of project files...
Building...
Copying compiled files to the build directory...
Importing module manifest settings...
Creating module manifest...
Module is located in: '.\build\pwsh-graph-connect'
Done.
```