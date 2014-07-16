using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EnvDTE80;
using EnvDTE;
using System.IO;
using System.Xml;
using System.Diagnostics;


namespace BTSCustomMapDebugger
{
    public partial class MainUI : Form
    {
        DTE2 _applicationObject = null;
        OutputWindowPane _debuggerOpWindowPane = null;

        string _xslFullName = string.Empty;
        string _extFullName = string.Empty;
        string _xmlFullName = string.Empty;

        public DTE2 ApplicationObject
        {
            set
            {
                _applicationObject = value;
            }
        }
        public string XslFullName
        {
            set
            {
                _xslFullName = value;
            }
        }
        public string ExtFullName
        {
            set
            {
                _extFullName = value;
            }
        }

        public OutputWindowPane DebuggerOpWindowPane
        {
            set
            {
                _debuggerOpWindowPane = value;
            }
        }



        public MainUI()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog3.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = openFileDialog3.FileName;
            }
        }

        private void engineProcess_OutputDataReceived(object opSender, DataReceivedEventArgs outLine, OutputWindowPane debuggerOpWindowPane)
        {
            if (debuggerOpWindowPane != null)
            {
                if (!String.IsNullOrEmpty(outLine.Data))
                {
                    debuggerOpWindowPane.OutputString(outLine.Data);
                    debuggerOpWindowPane.OutputString(Environment.NewLine);
                    debuggerOpWindowPane.Activate();
                }

            }
        }

        private void engineProcess_ErrorDataReceived(object opSender, DataReceivedEventArgs outLine, OutputWindowPane debuggerOpWindowPane)
        {
            if (debuggerOpWindowPane != null)
            {
                if (!String.IsNullOrEmpty(outLine.Data))
                {
                    debuggerOpWindowPane.OutputString(outLine.Data);
                    debuggerOpWindowPane.OutputString(Environment.NewLine);
                    debuggerOpWindowPane.Activate();
                }

            }
        }



        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (_debuggerOpWindowPane != null)
                {
                    _debuggerOpWindowPane.Clear();
                }

                if (textBox1.Text == string.Empty)
                {
                    MessageBox.Show("Please provide an input XML", "Invalid Input!");
                }
                else
                {
                    _xmlFullName = textBox1.Text;
                    if (_xslFullName != string.Empty && _extFullName != string.Empty && _applicationObject != null)
                    {
                        try
                        {
                            _applicationObject.Debugger.Breakpoints.Add("", _xslFullName, 1, 1, "", EnvDTE.dbgBreakpointConditionType.dbgBreakpointConditionTypeWhenTrue, "", "", 0, "", 0, EnvDTE.dbgHitCountType.dbgHitCountTypeNone);
                        }
                        catch
                        {
                        }

                        string opPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "BTS Custom Map Debugger" + "\\Output";

                        FileInfo xmlFileInfo = new FileInfo(_xmlFullName);

                        string opName = "mapped_" + xmlFileInfo.Name;

                        string args = "\"" + _xmlFullName + "\" \"" + _xslFullName + "\" \"" + _extFullName + "\" \"" + opPath + "\" \"" + opName + "\"";

                        System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
                        start.FileName = @"C:\Program Files\BTSCustomMapDebugEngine\BTSCustomMapDebugEngine.exe";
                        start.Arguments = args;
                        start.UseShellExecute = false;
                        start.CreateNoWindow = true;
                        start.RedirectStandardOutput = true;
                        start.RedirectStandardError = true;

                        System.Diagnostics.Process engineProcess = new System.Diagnostics.Process();
                        engineProcess.StartInfo = start;
                        engineProcess.OutputDataReceived += delegate(object osender, DataReceivedEventArgs de)
                        {
                            engineProcess_OutputDataReceived(osender, de, _debuggerOpWindowPane);
                        };

                        engineProcess.ErrorDataReceived += delegate(object osender, DataReceivedEventArgs de)
                        {
                            engineProcess_ErrorDataReceived(osender, de, _debuggerOpWindowPane);
                        };

                        engineProcess.Start();
                        engineProcess.BeginOutputReadLine();
                        engineProcess.BeginErrorReadLine();

                        foreach (EnvDTE.Process localProcess in _applicationObject.Debugger.LocalProcesses)
                        {

                            if (localProcess.ProcessID == engineProcess.Id)
                            {
                                localProcess.Attach();
                                break;

                            }
                        }
                    }
                    else
                    {
                        if (_debuggerOpWindowPane != null)
                        {
                            _debuggerOpWindowPane.OutputString("Error ocurred- No Custom XSL File/Custom Extension File Found");
                            _debuggerOpWindowPane.OutputString(Environment.NewLine);
                            _debuggerOpWindowPane.Activate();
                        }
                    }

                    this.Close();

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
                        _debuggerOpWindowPane.Activate();
                    }
                }

                this.Close();
            }



        }

    }
}