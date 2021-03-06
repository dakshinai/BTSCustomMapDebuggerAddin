using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;


namespace BTSCustomMapDebugger
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        string xslFullName = string.Empty;
        string extFullName = string.Empty;
        MainUI mUi = new MainUI();


        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect()
        {
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;

            Window window = _applicationObject.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            OutputWindow outputWindow = (OutputWindow)window.Object;

            foreach (OutputWindowPane pane in outputWindow.OutputWindowPanes)
            {
                if (pane.Name.Equals("BTS Custom Map Debugger Output"))
                {
                    _debuggerOpWindowPane = pane;
                    break;
                }
            }
            if (_debuggerOpWindowPane == null)
                _debuggerOpWindowPane = outputWindow.OutputWindowPanes.Add("BTS Custom Map Debugger Output");



            if (connectMode == ext_ConnectMode.ext_cm_UISetup)
            {
                object[] contextGUIDS = new object[] { };
                Commands2 commands = (Commands2)_applicationObject.Commands;


                //Place the command on the Project Item Context menu.
                //Find the Project Item Context command bar, which is the top-level command bar holding all the main menu items:

                Microsoft.VisualStudio.CommandBars.CommandBar solutionItemCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["Item"];

                //This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
                //  just make sure you also update the QueryStatus/Exec method to include the new command names.
                try
                {

                    //Add a command to the Commands collection:

                    Command debugMapCommand = commands.AddNamedCommand2(_addInInstance, "BTSCustomMapDebugger", "De&bug Custom Map", "Launches a VS debugger for debugging BizTalk Custom Maps", true, null, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusInvisible, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                    debugMapCommand.Bindings = "BizTalk Mapper::Ctrl+Shift+D";
                    //Add a control for the command to the Project Item Conxtext menu:

                    if ((debugMapCommand != null) && (solutionItemCommandBar != null))
                    {
                        int pIndex = 0;

                        foreach (CommandBarControl cbc in solutionItemCommandBar.Controls)
                        {
                            if (cbc.Type == MsoControlType.msoControlButton)
                            {
                                string guid;
                                int id;
                                commands.CommandInfo(cbc, out guid, out id);

                                Command command = commands.Item(guid, id);

                                if (command.Name.Equals("ProjectandSolutionContextMenus.Item.ValidateMap"))
                                {
                                    pIndex = cbc.Index;
                                    break;
                                }
                            }
                        }

                        debugMapCommand.AddControl(solutionItemCommandBar, pIndex + 1);
                    }

                }
                catch (System.ArgumentException)
                {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                }
                _addInInstance.Connected = true;
            }




        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {

            mUi.Close();

            Window window = _applicationObject.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            OutputWindow outputWindow = (OutputWindow)window.Object;

            foreach (OutputWindowPane pane in outputWindow.OutputWindowPanes)
            {
                if (pane.Name.Equals("BTS Custom Map Debugger Output"))
                {
                    _debuggerOpWindowPane = pane;
                    break;
                }
            }
            if (_debuggerOpWindowPane != null)
            {
                _debuggerOpWindowPane.Clear();
                object objService = GetService(_applicationObject, typeof(SVsOutputWindow));
                if (objService != null && outputWindow.ActivePane != _debuggerOpWindowPane)
                {
                    IVsOutputWindow output = (IVsOutputWindow)objService;
                    Guid paneGuid = new Guid(_debuggerOpWindowPane.Guid);

                    output.DeletePane(ref paneGuid);
                }
            }

        }

        private object GetService(object serviceProvider, Type type)
        {
            return GetService(serviceProvider, type.GUID);
        }

        private object GetService(object serviceProvider, Guid guid)
        {
            object objService = null;
            try
            {
                Microsoft.VisualStudio.OLE.Interop.IServiceProvider objIServiceProvider;
                IntPtr objIntPtr;
                int hr;
                Guid objSIDGuid, objIIDGuid;

                objSIDGuid = guid;
                objIIDGuid = objSIDGuid;
                objIServiceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)serviceProvider;
                hr = objIServiceProvider.QueryService(ref objSIDGuid, ref objIIDGuid, out objIntPtr);
                if (hr != 0)
                {
                    System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(hr, objIntPtr);
                }
                else
                {
                    objService = System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(objIntPtr);
                }
            }
            catch
            {
            }

            return objService;
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {

        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        /// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
        /// <param term='commandName'>The name of the command to determine state for.</param>
        /// <param term='neededText'>Text that is needed for the command.</param>
        /// <param term='status'>The state of the command in the user interface.</param>
        /// <param term='commandText'>Text requested by the neededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if (commandName == "BTSCustomMapDebugger.Connect.BTSCustomMapDebugger")
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusInvisible;

                    if (_applicationObject.Solution != null && _applicationObject.Solution.Projects != null && _applicationObject.Solution.Projects.Count > 0)
                    {
                        if (_applicationObject.SelectedItems.Count == 1 && _applicationObject.SelectedItems.Item(1).Name.ToLower().LastIndexOf(".btm") != -1)
                        {
                            string mapPath = _applicationObject.SelectedItems.Item(1).ProjectItem.get_FileNames(1);
                            string mapName = _applicationObject.SelectedItems.Item(1).ProjectItem.Name;
                            try
                            {
                                XmlDocument mapDoc = new XmlDocument();
                                mapDoc.Load(mapPath);
                                xslFullName = mapDoc.SelectSingleNode("mapsource/CustomXSLT/@XsltPath").InnerText;
                                extFullName = mapDoc.SelectSingleNode("mapsource/CustomXSLT/@ExtObjXmlPath").InnerText;

                                if (xslFullName.StartsWith("."))
                                {
                                    if (xslFullName.LastIndexOf("\\") != xslFullName.Length - 1)
                                    {
                                        string fileName = xslFullName.Substring(xslFullName.LastIndexOf("\\") + 1);
                                        string filePathSlash = mapPath.Substring(0, mapPath.Length - mapName.Length);
                                        xslFullName = filePathSlash + fileName;
                                    }
                                }
                                if (extFullName.StartsWith("."))
                                {
                                    if (extFullName.LastIndexOf("\\") != extFullName.Length - 1)
                                    {
                                        string fileName = extFullName.Substring(extFullName.LastIndexOf("\\") + 1);
                                        string filePathSlash = mapPath.Substring(0, mapPath.Length - mapName.Length);
                                        extFullName = filePathSlash + fileName;

                                    }
                                }
                            }
                            catch
                            {
                            }
                            if (xslFullName != string.Empty && extFullName != string.Empty)
                            {
                                //Also validate if the file has Cusom XSLT filled, else latch it
                                status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                            }
                        }
                    }
                    return;
                }
            }
        }


        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param term='commandName'>The name of the command to execute.</param>
        /// <param term='executeOption'>Describes how the command should be run.</param>
        /// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
        /// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
        /// <param term='handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {

            handled = false;
            if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                if (commandName == "BTSCustomMapDebugger.Connect.BTSCustomMapDebugger")
                {
                    //checkmap property page for input type as XML and input file is specified else alarm
                    //while executing if external objected is needed, alarm
                    //if o/p file path is present write to it else temp file like how MS does
                    //write results to o/p window with hyper links

                    try
                    {

                        if (xslFullName != string.Empty && extFullName != string.Empty)
                        {
                            mUi.XslFullName = xslFullName;
                            mUi.ExtFullName = extFullName;
                            mUi.ApplicationObject = _applicationObject;
                            mUi.DebuggerOpWindowPane = _debuggerOpWindowPane;
                            mUi.ShowDialog();
                        }

                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            if (_debuggerOpWindowPane != null)
                            {
                                _debuggerOpWindowPane.OutputString("Fatal Error ocurred- " + ex.InnerException.Message + "; " + ex.Message);
                                _debuggerOpWindowPane.OutputString(Environment.NewLine);
                                _debuggerOpWindowPane.Activate();
                            }
                        }
                        else
                        {
                            if (_debuggerOpWindowPane != null)
                            {
                                _debuggerOpWindowPane.OutputString("Fatal Error ocurred- " + ex.Message);
                                _debuggerOpWindowPane.OutputString(Environment.NewLine);
                            }
                        }
                    }

                    handled = true;

                    return;
                }
            }
        }

        private DTE2 _applicationObject;
        private AddIn _addInInstance;
        private OutputWindowPane _debuggerOpWindowPane = null;
    }
}