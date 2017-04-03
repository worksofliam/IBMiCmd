# IBMiCmd

x86 Notepad++ plugin for IBM i development

IBMiCmd (maybe referenced as IBMiCmds) is a Notepad++ plugin for ILE development on an IBM i. IBMiCmd provides the ability to submit system commands remotely, as well as tools to parse an error listing (`EVFEVENT` file).

### Functionality

* Command entry
* System bindings (a group of system commands to submit on one job - primarily for compiling)
* Error listing for ILE compiles (read from the EVFEVENT file)
* Ability to convert fixed-format definitions and calculations to free-format

### Prerequisites

* For a list of source members or an IFS listing, a plugin named NppFTP is required. It is usually shipped with Notepad++, but if you don't have it: you can find it in the plugin manager within Notepad++. The connection you setup within NppFTP is seperate from the connection(s) you setup for IBMiCmd - make sure that both system connections and the same. When you have this plugin installed, you should configure a connection to the system you will develop on and also set the Transfer Mode to 'ASCII'.
* C, C++ and COBOL highlighting is available in Notepad++ by default. To add free-format RPG highlighting, you will need to manually add the syntax highlighting which can be found [in this repository](https://github.com/WorksOfBarry/Notepad-RPG).

### Installation

1. [See the releases](https://github.com/WorksOfBarry/IBMiCmd/releases) for IBMiCmd and download the latest version available.
2. Drop the .dll into the Notepad++ plugins folder.

### Images

![](http://i.imgur.com/Gk2z0OF.png)
![](https://camo.githubusercontent.com/362ccac4dd05882f4160ac6975f4b1b4854e9d4b/68747470733a2f2f6c68332e676f6f676c6575736572636f6e74656e742e636f6d2f2d3667563366784d553039632f574a78725a725237714a492f41414141414141414271492f6274537358594341535545496e4e766477346352454f6d54346f64444135446f67434c30422f683333392f323031372d30322d30392e706e67)
![](https://camo.githubusercontent.com/dd96c8b1d8b341a7374b9e6895ba452671371788/68747470733a2f2f6c68332e676f6f676c6575736572636f6e74656e742e636f6d2f2d77726541475646514142632f574a7953505659415236492f41414141414141414271552f5f5534626831545a7846634a4e4e2d45456c525172664f685252787a496b4d7177434c30422f683430392f323031372d30322d30392e706e67)
![](https://cloud.githubusercontent.com/assets/3708366/24582419/8ce6bf78-1727-11e7-963d-c40af4c125e5.png)
![](https://cloud.githubusercontent.com/assets/3708366/24633350/003c2d48-18c0-11e7-82b0-e29345a14b39.png)

#### Build tips

I had an issue building with VS 2017 which was related to the location of the `lib.exe` program on Windows. The location is defined in `DllExport\NppPlugin.DllExport.targets`.

I had to find it within my VS installation directory and the diff from VS 2015 to VS 2017 is below.

```
-                   LibToolPath="$(DevEnvDir)\..\..\VC\bin"
-                   LibToolDllPath="$(DevEnvDir)"
+                   LibToolPath="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.10.25017\bin\HostX86\x86"
+                   LibToolDllPath="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.10.25017\bin\HostX86\x86"
```