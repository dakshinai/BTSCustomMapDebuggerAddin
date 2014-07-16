BTSCustomMapDebuggerAddin
=========================

BizTalk Custom Map Debugger Addin - Visual Studio

The following steps will guide you on how to use it. 

System requirements: .NET2.0 or higher, BizTalk supported VS2005

1.	Launch your project in visual studio.
2.	Right click on the custom map (.btm). You should find an option “Debug Custom Map” if your map has custom XSL and extension object file defined, as below.
3.	Hit the shortcut “d” or click on the option
4.	In the pop that appears as below, browse to you sample XML file with which you wish to debug your map
5.	Click “Start Debugging”
6.	Your Custom XSL file from solution is automatically launched and breakpoint is hit on line 1
7.	Choose Locals from Debug menu to see values from loaded XSL variables
8.	Set more breakpoints as needed in the XSL or Custom C# or any as needed. You may want to add watches to evaluate specific expressions.
9.	You can find you output file saved to path described by output window. Choose pane “BTS Custom Map Debugger Output” in output window
10.	The mapped output will be saved to current user’s Application Data directory. Follow the URI to view the mapped XML
 
Few pointers
1.	You can launch the debugger with Ctrl+Shift+D if you are using BizTalk Mapper Editor
2.	Exceptions from XSL or environment are thrown as message alerts with Exception and Inner Exception if any
3.	Ensure the DLLs described by the extension object file and its dependencies are present in GAC
