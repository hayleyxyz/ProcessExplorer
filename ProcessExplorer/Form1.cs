using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProcessExplorer {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            this.treeProcesses.ImageList = new ImageList() {
                ColorDepth = ColorDepth.Depth32Bit
            };

            this.Load += this.Form_Load;
            this.treeProcesses.NodeMouseClick += this.TreeProcesses_NodeMouseClick;
        }

        private void Form_Load(object sender, EventArgs e) {
            var processes = Process.GetProcesses();

            foreach(var proc in processes) {
                this.AddProcessToTree(proc);
            }
        }

        protected void AddProcessToTree(Process process) {
            var node = new TreeNode();

            node.Text = String.Format("{0} ({1})", process.ProcessName, process.Id);

            bool isWow64 = false;
            ProcessUtils.IsWow64ProcessById((uint)process.Id, ref isWow64);
            if(isWow64) {
                node.Text += " (WOW64)";
            }

            node.Tag = process;
            node.ImageKey = this.GetImageKey(process);

            try {

                foreach(ProcessModule module in process.Modules) {
                    if(module.ModuleName == process.MainModule.ModuleName && module.BaseAddress == process.MainModule.BaseAddress) {
                        continue;
                    }

                    var moduleNode = new TreeNode();

                    moduleNode.Text = module.ModuleName;
                    moduleNode.Tag = module;

                    node.Nodes.Add(moduleNode);
                }
            }
            catch(Exception exception) {
                node.Nodes.Add(exception.Message);
            }
            
            treeProcesses.Nodes.Add(node);
        }

        protected string GetImageKey(Process process) {
            try {
                if(treeProcesses.ImageList.Images.ContainsKey(process.ProcessName)) {
                    return process.ProcessName;
                }

                var icon = System.Drawing.Icon.ExtractAssociatedIcon(process.MainModule.FileName);

                treeProcesses.ImageList.Images.Add(process.ProcessName, icon);

                return process.ProcessName;
            }
            catch(Exception exception) {
                return null;
            }
        }

        private void TreeProcesses_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            var tag = e.Node.Tag;

            if(tag is Process) {
                this.SelectedProcess((Process)tag);
            }
            else if(tag is ProcessModule) {
                this.SelectedModule((ProcessModule)tag);
            }
            else {
                throw new Exception("Invalid node tag");
            }
        }

        protected void SelectedProcess(Process process) {

            foreach(ProcessThread thread in process.Threads) {

                var threadHandle = WinApi.OpenThread(WinApi.ThreadAccess.GET_CONTEXT, false, (uint)thread.Id);

                var context = new WinApi.CONTEXT64();
                context.ContextFlags = WinApi.CONTEXT_FLAGS.CONTEXT_ALL;

                var result = WinApi.GetThreadContext(threadHandle, ref context);


                var le = System.Runtime.InteropServices.Marshal.GetLastWin32Error();

                Debugger.Break();
            }
        }

        protected void SelectedModule(ProcessModule module) {
            Debugger.Break();
        }
    }
}
