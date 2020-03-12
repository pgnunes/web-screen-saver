web-screen-saver
==============
Windows screensaver that iterates through a list of URLs. Based on [CefSharp](https://github.com/cefsharp/CefSharp) ([Chromium Embedded Framework](https://bitbucket.org/chromiumembedded/cef/src/master/)).

Can be used to display dashboards from different web sites (like Grafana) and/or as a secure way to display information (i.e. kiosk TV mode) 

Dependencies
------------

 * [Microsoft Visual C++ for Visual Studio 2015, 2017 and 2019 - x86](https://aka.ms/vs/16/release/vc_redist.x86.exe)
* .Net Version 4.5.2+ (shipped with Windows 10)

Installation
----------------------
Download WebScreenSaver-x.x.x.zip from [releases](https://github.com/pgnunes/web-screen-saver/releases) and extract it (i.e. user home). Right click on ```WebScreenSaver.scr``` and select 'install'. 

*NOTE:* Keep the folder and its contents as the screensaver requires all existent files to run.

Configuration
----------------------
A default config file will be created under ```%UserProfile%\\webscreensaver.txt```. File location can be overridden by the screensaver settings.

Configuration file contains number of seconds and webpage. One entry per line. i.e.:
```
5  https://github.com/pgnunes/web-screen-saver
10 https://duckduckgo.com/
```