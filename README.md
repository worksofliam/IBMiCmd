# IBMiCmd

x86 Notepad++ plugin for IBM i development.

IBMiCmd (maybe referenced as IBMiCmds) is a Notepad++ plugin for ILE development on an IBM i.

[Check out the wiki](https://github.com/WorksOfBarry/IBMiCmd/wiki) for more info, feature list, installation tips and future goals.

### Installation

1. [See the releases](https://github.com/WorksOfBarry/IBMiCmd/releases) for IBMiCmd and download the latest version available.
2. Drop the .dll into the Notepad++ plugins folder.

#### Build tips

I had an issue building with VS 2017 which was related to the location of the `lib.exe` program on Windows. The location is defined in `DllExport\NppPlugin.DllExport.targets`.

I had to find it within my VS installation directory and the diff from VS 2015 to VS 2017 is below.

```
-                   LibToolPath="$(DevEnvDir)\..\..\VC\bin"
-                   LibToolDllPath="$(DevEnvDir)"
+                   LibToolPath="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.10.25017\bin\HostX86\x86"
+                   LibToolDllPath="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.10.25017\bin\HostX86\x86"
```
