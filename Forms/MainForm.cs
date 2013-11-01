using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net.Sockets;

using System.Drawing.Imaging;
using KPP.Core.Debug;
using System.Diagnostics;

using KPP.Controls.Winforms.ImageEditorObjs;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Xml;
using BrightIdeasSoftware;
using TIS.Imaging;
using Emgu.CV.UI;
using DejaVu;
using RaspberryPiDotNet;
using IOModule;
using VisionModule.Properties;
using System.Resources;
using System.Globalization;
using KPPAutomationCore;


namespace VisionModule {


    public partial class MainForm : DockContent {
        
        //protected override CreateParams CreateParams {
        //    get {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;
        //        return cp;
        //    }
        //} 


        public override string ToString() {
            return this.Name;
        }

        private static KPPLogger log = new KPPLogger(typeof(MainForm));

        public VisionProject SelectedProject = null;
        //  Request _SelectedProject.SelectedRequest = null;


        VisionProjects _projconfig = null;
        String _appfile;
        DeserializeDockContent m_deserializeDockContent;

        //Inspection _SelectedInspection = null;




        ArrowPadControl ArrowPad = null;

        ProjectOptionsForm _ProjectOptionsForm = new ProjectOptionsForm();

        IOSettingsForm _IOSettingsForm = new IOSettingsForm();

        ConfigurationsForm _ConfigurationsForm = new ConfigurationsForm();
        ListInspForm _ListInspForm = new ListInspForm();
        ImageContainerForm _ImageContainer = new ImageContainerForm();
        LogForm _LogForm = new LogForm();
        ResultsConfiguration _ResultsConfiguration = new ResultsConfiguration();
        ViewInspections _viewinspections = new ViewInspections();
        ListROIForm _ListROIForm = new ListROIForm();





        internal static void MoveSplitter(PropertyGrid propertyGrid, int x) {
            object propertyGridView =
                typeof(PropertyGrid).InvokeMember("gridView", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null, propertyGrid, null);
            propertyGridView.GetType().InvokeMember("MoveSplitterTo", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, propertyGridView, new object[] { x });
        }





        void InputInfo_OnImageSizeChanged(Size NewImageSizeInfo) {
            try {
                //  if (_ImageContainer.__roicontainer1.BackgroundImage != null)
                // _ImageContainer.__roicontainer1.BackgroundImage = null;

                _ImageContainer.__roicontainer.NewImage(NewImageSizeInfo.Width, NewImageSizeInfo.Height);

               
                _ListInspForm.__listRoi.Enabled = true;

            } catch (Exception exp) {
                log.Error(exp);
            }
        }

        void InputInfo_OnImageFormatChanged(PixelFormat NewPixelFormatInfo) {
            if (NewPixelFormatInfo != PixelFormat.DontCare) {
               
            }
        }


        public Boolean _silentexit = false;
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;



        private void OnExit(object sender, EventArgs e) {
            Application.Exit();
        }

        private void OnShow(object sender, EventArgs e) {
            SetForm(true);
        }



        void SetForm(bool show = false) {
            if (!show) {
                this.WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
            else {

                this.WindowState = FormWindowState.Maximized;
                SetForegroundWindow(Handle.ToInt32());
                ShowInTaskbar = true;
            }


        }

        [DllImport("User32.dll")]
        internal static extern Int32 SetForegroundWindow(int hWnd);

        void trayIcon_DoubleClick(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            // Application.OpenForms["Form1"].BringToFront();
        }

        void trayIcon_Click(object sender, EventArgs e) {

            SetForm(this.WindowState == FormWindowState.Minimized);

        }

        public MainForm() {

            switch (StaticObjects.Language) {
                case LanguageName.Unk:
                    break;
                case LanguageName.PT:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-PT");

                    break;
                case LanguageName.EN:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("de");

                    break;
                default:
                    break;
            }
          

            
            //   this.Hide();
            Thread splashthread = new Thread(new ThreadStart(SplashScreen.ShowSplashScreen));
            splashthread.IsBackground = true;
            splashthread.Start();

            Thread.Sleep(100);
            SplashScreen.UdpateStatusTextWithStatus(this.GetResourceText("SplashScreen_0"), TypeOfMessage.Success);
            Thread.Sleep(100);

            
            InitializeComponent();

           


            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

           
        }




        private IDockContent GetContentFromPersistString(string persistString) {

            if (persistString == typeof(ListInspForm).ToString())
                return _ListInspForm;
            //else if (persistString == typeof(InspectionOptionsForm).ToString())
            //  return _InspectionOptions;
            else if (persistString == typeof(ImageContainerForm).ToString())
                return _ImageContainer;
            else if (persistString == typeof(ListROIForm).ToString())
                return _ListROIForm;
            else if (persistString == typeof(LogForm).ToString())
                return _LogForm;
            else if (persistString == typeof(ViewInspections).ToString())
                return _viewinspections;
            else if (persistString == typeof(ResultsConfiguration).ToString())
                return _ResultsConfiguration;
            
            else {

                return null;
            }
        }





        public void LoadProjectsFromFile(String newpath) {

            String appath = AppDomain.CurrentDomain.BaseDirectory;
           

            Uri fullPath = new Uri(new Uri(appath), newpath);

            String thefilepath = fullPath.LocalPath;// +Path.GetFileName(newpath);

            log.Info("Loading projects file : " + thefilepath);


            if (File.Exists(thefilepath)) {


                if (_projconfig!=null) {
                    CloseCurrentConfiguration(true);
                }

                
                _projconfig = VisionProjects.ReadConfigurationFile(thefilepath);

                _projconfig.BackupExtention = ".bkp";
                _projconfig.BackupFilesToKeep = 5;
                _projconfig.BackupFolderName = "Backup";


                _ProjectOptionsForm._projsconf = _projconfig;
                _ProjectOptionsForm._projsfile = thefilepath;
                _ProjectOptionsForm.__listprojects.Objects = _projconfig.Projects.ToList();
                log.Info("Projects file loaded");

                if (_projconfig.Projects.Count==1) {
                    LoadProject(_projconfig.Projects[0].Name);
                }
            }
            else {
                log.Info("Projects file not found");
            }
        }


        VisionSettings _visionconfig = null;

        private void MainForm_Load(object sender, EventArgs e) {



            try {

              
                //__toolStriplevels.Items.AddRange(new String[]{"Admin","Man"});
                //__toolStriplevels.SelectedIndexChanged += new EventHandler(__toolStriplevels_SelectedIndexChanged);
                 
                DebugController.ActiveDebugController.OnDebugMessage += new OnDebugMessageHandler(ActiveDebugController_OnDebugMessage);
                _ProjectOptionsForm.TopMost = true;
               
                StaticObjects.Grids.Add(_ListInspForm.__propertyGridinsp);
                
                StaticObjects.OnRefreshPropertyGrid += new RefreshPropertyGrid(StaticObjects_OnRefreshPropertyGrid);

                ArrowPad = new ArrowPadControl();

                AppPerformanceMonitor.OnAppPerformanceMonitorTick += new OnAppPerformanceMonitorTickHandler(AppPerformanceMonitor_OnAppPerformanceMonitorTick);
                AppPerformanceMonitor.Start(12, 8);
                
                //////////////////TODO
                //if () {
                //    PhidgetsIO.SendCommand("SET_OUT", new String[] { "1", "ON" });
                //}
                

                #region project forms init



                CvInvoke.CvAllocFunc sss = new CvInvoke.CvAllocFunc(delegate { 
                    return new IntPtr();
                });
                

                SplashScreen.UdpateStatusTextWithStatus(this.GetResourceText("SplashScreen_1"), TypeOfMessage.Success);

                dockFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config\\DockPanel.config");



                if (!Directory.Exists(dockFile)) {
                    Directory.CreateDirectory(Path.GetDirectoryName(dockFile));
                }

                _appfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\Vision.config");

                if (!File.Exists(_appfile)) {
                    _visionconfig = new VisionSettings();
                    _visionconfig.WriteConfigurationFile(_appfile);
                }
                _visionconfig = VisionSettings.ReadConfigurationFile(_appfile);
                _visionconfig.BackupExtention = ".bkp";
                _visionconfig.BackupFilesToKeep = 5;
                _visionconfig.BackupFolderName = "Backup";

                _ConfigurationsForm.Appsettings = _visionconfig;

                if (_visionconfig.Servers.Count == 0) {
                    TCPServer guiserver = new TCPServer();
                    guiserver.Port = 9600;
                    guiserver.ID = "GUI";
                    _visionconfig.Servers.Add(guiserver);
                }


                foreach (TCPServer item in _visionconfig.Servers) {
                    item.Connected += new TCPServer.TcpServerEventDlgt(Server_Connected);
                    item.Disconnected += new TCPServer.TcpServerEventDlgt(Server_Disconnected);
                    item.ServerClientMessage += new TCPServer.TcpServerClientMessageEvent(Server_ClientMessage);
                    item.StartListening();
                }
                _ConfigurationsForm.__serverconflist.Objects = _visionconfig.Servers;

                //StaticObjects.AcessLevel = Acesslevel.Admin;

                //TextBox newpasstb = this.__editnewpass.Control as TextBox;
                //newpasstb.PasswordChar = '*'; 

                //TextBox __logpasstb = this.__logpass.Control as TextBox;
                //__logpasstb.KeyPress += new KeyPressEventHandler(__logpasstb_KeyPress);
                //__logpasstb.PasswordChar = '*'; 

                if (File.Exists(dockFile))
                    try {
                        __dockPanel1.LoadFromXml(dockFile, m_deserializeDockContent);
                    } catch (Exception exp) {

                        __dockPanel1.SaveAsXml(dockFile);

                    }
                else {
                    _ListROIForm.Show(__dockPanel1);
                    _ListInspForm.Show(__dockPanel1);
                    _ListROIForm.AutoScroll = true;
                    _LogForm.Show(__dockPanel1);
                    _ResultsConfiguration.Show(__dockPanel1);
                    _viewinspections.Show(__dockPanel1);
                    _ImageContainer.Show(__dockPanel1);                    

                }




                if (_ListROIForm.Visible == false) {
                    _ListROIForm.Show(__dockPanel1);
                    _ListInspForm.AutoScroll = true;
                }

                if (_ListInspForm.Visible == false) {
                    _ListInspForm.Show(__dockPanel1);
                    _ListInspForm.AutoScroll = true;
                }

                if (_ResultsConfiguration.Visible == false) {
                    _ResultsConfiguration.Show(__dockPanel1);
                    _ResultsConfiguration.AutoScroll = true;
                }

                if (_LogForm.Visible == false) {
                    _LogForm.Show(__dockPanel1);
                    _LogForm.AutoScroll = true;
                }
                
               


                if (_viewinspections.Visible == false) {
                    _viewinspections.Show(__dockPanel1);
                    _viewinspections.AutoScroll = true;
                }

                if (_ImageContainer.Visible == false) {
                    _ImageContainer.Show(__dockPanel1);
                    _ImageContainer.AutoScroll = true;

                }


                

                _ListInspForm.Show();
                #endregion

                
                

                

                _ProjectOptionsForm.FormClosed += new FormClosedEventHandler(_ProjectOptionsForm_FormClosed);
                _ProjectOptionsForm.FormClosing += new FormClosingEventHandler(_ProjectOptionsForm_FormClosing);

 
                

                SplashScreen.UdpateStatusTextWithStatus(this.GetResourceText("SplashScreen_2"), TypeOfMessage.Success);


                _ConfigurationsForm.__btSave.Click += new EventHandler(__btSave_Click);
                
                _ListROIForm.__RoiProcList.MouseClick += new MouseEventHandler(__RoiProcList_MouseClick);
                _ListROIForm.OnControlRightClick += __RoiProcList_MouseClick;
                InputSelector _inputselector = new InputSelector();

                StaticObjects.InputItemSelectorControl = _inputselector;

                ICImagingControl getcameras = new ICImagingControl();

                List<String> camnames = new List<string>();
                foreach (Device camera in getcameras.Devices) {

                    camnames.Add(camera);


                }

                getcameras.Dispose();

                foreach (String name in camnames) {
                    ICImagingControl newcam = new ICImagingControl();

                    if (newcam.LiveVideoRunning) {
                        newcam.LiveStop();
                    }
                    
                    newcam.DeviceLostExecutionMode = EventExecutionMode.MultiThreaded;
                    newcam.ImageAvailableExecutionMode = EventExecutionMode.MultiThreaded;
                    newcam.LiveDisplay = false;
                    newcam.Device = name;
                    //newcam.MemoryCurrentGrabberColorformat = ICImagingControlColorformats.ICY800;
                    newcam.ImageRingBufferSize = 1;                    
                    
                    ICSCameraInterface.ICSCameras.Add(new ICScamera(newcam));
                }

                StaticObjects.isLoading = true;
                using (UndoRedoManager.StartInvisible("Init")) {

                    StaticObjects.CaptureSources.Add(new FileCapture());
                    StaticObjects.CaptureSources.Add(new InspectionCapture());
                    StaticObjects.CaptureSources.Add(new PythonRemoteCapture());
                    StaticObjects.CaptureSources.Add(new ICSCameraCapture());
                    StaticObjects.CaptureSources.Add(new CVCameraCapture());
                    //StaticObjects.CaptureSources.Add(new RemoteCameraCapture());
                    StaticObjects.CaptureSources.Add(new DirectShowCameraCapture());                                       
                    StaticObjects.CaptureSources.Add(new uEyeCamera());                  


                    UndoRedoManager.Commit();
                }
                StaticObjects.isLoading = false;
                





                SplashScreen.UdpateStatusTextWithStatus(this.GetResourceText("SplashScreen_3"), TypeOfMessage.Success);

                //_ConfigurationsForm.
                
                //TODO Loading project
                LoadProjectsFromFile(_visionconfig.ProjectFile);






                ToolStripUndoSelector undodropDown = new ToolStripUndoSelector();
                undodropDown.Opening += new CancelEventHandler(UndoDropDown_Opening);
                undodropDown.Selector.__btok.Click += new EventHandler(__btUndook_Click);
                this.__btundo.DropDown = undodropDown;



                ToolStripUndoSelector reodropDown = new ToolStripUndoSelector();
                reodropDown.Opening += new CancelEventHandler(RedoDropDown_Opening);
                reodropDown.Selector.__btok.Click += new EventHandler(__btRedook_Click);
                this.__btredo.DropDown = reodropDown;
                                                                           

             
                _viewinspections.FormClosing += new FormClosingEventHandler(_viewinspections_FormClosing);
                
                
              
                               
                _ImageContainer.__roicontainer.MouseClick += new MouseEventHandler(__roicontainer_MouseClick);
                _ImageContainer.__addrectROI.Click += new EventHandler(__addrectROI_Click);

                ArrowPad.__btdrag.MouseMove += new MouseEventHandler(__btdrag_MouseMove);
                ArrowPad.__btdrag.MouseDown += new MouseEventHandler(__btdrag_MouseDown);

                ArrowPad.__btDOWN.MouseDown += new MouseEventHandler(__bt_MouseDown);
                ArrowPad.__btLEFT.MouseDown += new MouseEventHandler(__bt_MouseDown);
                ArrowPad.__btRIGHT.MouseDown += new MouseEventHandler(__bt_MouseDown);
                ArrowPad.__btUP.MouseDown += new MouseEventHandler(__bt_MouseDown);

                ArrowPad.__btDOWN.MouseUp += new MouseEventHandler(__btMouseUp);
                ArrowPad.__btLEFT.MouseUp += new MouseEventHandler(__btMouseUp);
                ArrowPad.__btRIGHT.MouseUp += new MouseEventHandler(__btMouseUp);
                ArrowPad.__btUP.MouseUp += new MouseEventHandler(__btMouseUp);



                this.Controls.Add(ArrowPad);
                ArrowPad.BringToFront();
                ArrowPad.Hide();
                ArrowPad.Size = new Size(210, 275);
                //new ResizeControl(ArrowPad.panel1, true);
                

                
                
                _ProjectOptionsForm.__listprojects.SelectedIndexChanged += new EventHandler(__listprojects_SelectedIndexChanged);
                _ProjectOptionsForm.__btLoadProj.Click += new EventHandler(__btLoadProj_Click);


                          

                _ImageContainer.originalToolStripMenuItem.Click += new EventHandler(originalToolStripMenuItem_Click);

                _ResultsConfiguration.__listinspResults.CellEditFinishing += new CellEditEventHandler(__listinspResults_CellEditFinishing);
                _ResultsConfiguration.__listInputs.CellEditFinishing += new CellEditEventHandler(__listInputs_CellEditFinishing);
                

                SplashScreen.UdpateStatusTextWithStatus(this.GetResourceText("SplashScreen_4"), TypeOfMessage.Success);
                //Thread.Sleep(10);
                ReflectionController.Load("dll");





                UndoRedoManager.CommandDone += new EventHandler<CommandDoneEventArgs>(UndoRedoManager_CommandDone);
                SplashScreen.UdpateStatusText(this.GetResourceText("SplashScreen_5"));
                
                //Thread.Sleep(500);


                _ListInspForm.__propertyGridinsp.PropertyValueChanged += new PropertyValueChangedEventHandler(__propertyGridinsp_PropertyValueChanged);

                this.Show();
                SplashScreen.CloseSplashScreen();
                this.Activate();





                #region Android
                //AndroidServer = new TCPServerConnection();
                //AndroidServer.Port = 9605;
                //AndroidServer.StartOnLoad = false;
                //AndroidServer.OnClientStateChanged += new TCPServerConnection.ClientStateChanged(AndroidServer_OnClientStateChanged);
                //AndroidServer.OnClientMessage += new TCPServerConnection.ClientMessage(AndroidServer_OnClientMessage);
                //AndroidServer.Start();
                #endregion


                
                _ConfigurationsForm.__contextServers.ItemClicked += new ToolStripItemClickedEventHandler(__contextServers_ItemClicked);
                
                
                _ConfigurationsForm.__contextServers.Opening += new CancelEventHandler(__contextServers_Opening);



                KPPVision.OnAcesslevelChanged += new KPPVision.AcesslevelChanged(StaticObjects_OnAcesslevelChanged);
                KPPVision.AcessLevel = AcessManagement.Acesslevel.User;

                if (SetAdmin) {
                    KPPVision.AcessLevel = AcessManagement.Acesslevel.Admin;
                }

            } catch (Exception exp) {
                log.Error(exp);
                SplashScreen.UdpateStatusText(this.GetResourceText("SplashScreen_6"));
                Thread.Sleep(500);
                this.Show();
                SplashScreen.CloseSplashScreen();
                this.Activate();

            }

        }




        void __addrectROI_Click(object sender, EventArgs e) {
            ROI newroi = SelectedProject.SelectedRequest.SelectedInspection.AddROI(new Rectangle(0, 0, 100, 100));

            for (int i = 0; i < SelectedProject.SelectedRequest.SelectedInspection.ROIList.Count; i++) {
                String roiname = "ROI" + i.ToString();
                if (!SelectedProject.SelectedRequest.SelectedInspection.ROIList.Exists(r => r.Name == roiname)) {
                    newroi.Name = roiname;
                    break;
                }
            }

            _ListInspForm.__listRoi.Objects = SelectedProject.SelectedRequest.SelectedInspection.ROIList;


            _ListInspForm.__listRoi.SelectedIndex = _ListInspForm.__listRoi.GetItemCount() - 1;
            SelectedProject.SelectedRequest.SelectedInspection.SelectedROI = newroi;
        }

        void Server_Disconnected(object sender, TCPServerEventArgs e) {
            log.Info(e.ConnectionState.Server.ID+" : Client Disconnected"); 
        }

        void Server_ClientMessage(object sender, TCPServerClientEventArgs e) {

            try {
                
                TCPServer server = sender as TCPServer;
                String[] Args = e.MessageReceived.Replace("\r\n", "").Split(new String[] { "|" }, StringSplitOptions.None);

                log.Info(server.ID +" : Command received : " + e.MessageReceived);

                for (int i = 0; i < Args.Count(); i++) {

                    String Command = Args[i];

                    switch (Command) {
                        case "PROGRAM":
                            log.Info("Loading program : " + Args[i + 1].ToString());
                            //this.BeginInvoke(new MethodInvoker(delegate {
                            if (LoadProject(Args[i + 1])) {
                                Thread.Sleep(1);
                                server.Client.Write("PROGRAM|OK\n");
                                log.Info("Program loaded: " + Args[i + 1]);

                            }
                            else {
                                Thread.Sleep(1);
                                server.Client.Write("ERROR|01");
                            }
                            // }));

                            break;
                        case "REQUEST":

                             DoRequest(sender, Args[i + 1]);

                            log.Info("Request " + Args[i + 1] + " Done");

                            break;

                        default:
                            break;
                    }
                    i++;
                    continue;
                }

            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        void Server_Connected(object sender, TCPServerEventArgs e) {
            TCPServer server = sender as TCPServer;
            log.Info(server.ID+" : Client connected");
        }


        void StaticObjects_OnAcesslevelChanged(AcessManagement.Acesslevel NewLevel) {

            Boolean state = (NewLevel == AcessManagement.Acesslevel.Admin || NewLevel == AcessManagement.Acesslevel.Man);
            _ImageContainer.__roicontainer.Enabled = state;
            _ListInspForm.__propertyGridinsp.Enabled = state;
            _ImageContainer.__roicontainer.Enabled = state;
            _ImageContainer.__roicontainer.Enabled = state;

         
            __btConfig.Enabled = state;
            
            switch (NewLevel) {
                case AcessManagement.Acesslevel.Admin:
                   
                    break;
                case AcessManagement.Acesslevel.Man:
                
                    break;
                case AcessManagement.Acesslevel.User:
                
                    break;
                default:
                    break;
            }
        }

        void AppPerformanceMonitor_OnAppPerformanceMonitorTick(OnAppPerformanceMonitorTickEventArgs e) {
            __statMEM.Text = e.Performance.WorkingMemory.ToString();
            __statusCPU.Text = e.Performance.CPU.ToString();
        }

        


        void _ProjectOptionsForm_FormClosing(object sender, FormClosingEventArgs e) {
            _ListROIForm.__propertyGridFunction.Refresh();
        }


        

        void __contextServers_Opening(object sender, CancelEventArgs e) {
            try {

                ObjectListView senderlist = (ObjectListView)((ContextMenuStrip)sender).SourceControl;
                if (senderlist != null) {

                    _ConfigurationsForm.__toolremoveServer.Enabled = senderlist.SelectedObject != null;

                }
            } catch (Exception exp) {
                log.Error(exp);
                e.Cancel = true;
            }

        }

        void __contextServers_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            try {

              
            } catch (Exception exp) {
                log.Error(exp);

            }

        }

       
        void __btSave_Click(object sender, EventArgs e) {
            try {
                _visionconfig.WriteConfigurationFile(_appfile);
                
                
            } catch (Exception exp) {

                log.Error(exp);
            }

        }

        void StaticObjects_OnRefreshPropertyGrid(PropertyGrid Grid) {
            if (Grid != null) {
                Grid.Refresh();
            }

        }

     

        void __btMouseUp(object sender, MouseEventArgs e) {
            btpressedtimer.Enabled = false;
        }

        Button bt = null;


        Stopwatch mousedowntime = new Stopwatch();
        void __bt_MouseDown(object sender, MouseEventArgs e) {
            
            bt = (Button)sender;
            if (!btpressedtimer.Enabled) {
                btpressedtimer.Enabled = true;
            }
            
        }



        int controlXpos, controlYpos;
        void __btdrag_MouseDown(object sender, MouseEventArgs e) {
            controlXpos = e.X;
            controlYpos = e.Y;
        }

        void __btdrag_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left || e.Button == System.Windows.Forms.MouseButtons.Right) {
                ArrowPad.Left = (ArrowPad.Left + e.X) - controlXpos;
                ArrowPad.Top = (ArrowPad.Top + e.Y) - controlYpos;
            }
        }



        void AndroidServer_OnClientMessage(string Command, string[] Args) {
            try {
                if (Args.Count() < 1) {
                    return;
                }

                // int param = System.Convert.ToInt32(Args[0]);
                switch (Command) {
                    case "GETPROJECT":
                        if (SelectedProject == null) {
                            //AndroidServer.Send(AndroidServer.ClientObject.workSocket, "PROJECT#Não definido");
                        }
                        else {
                            String sendstr = "PROJECT#" + SelectedProject.Name;

                            foreach (Request req in SelectedProject.RequestList) {
                                sendstr += "<REQUEST>" + req.Name;
                                foreach (Inspection insp in req.Inspections) {
                                    sendstr += "<INSP>" + insp.Name;
                                    foreach (ROI roi in insp.ROIList) {
                                        sendstr += "<ROI>" + roi.Name;
                                        foreach (ProcessingFunctionBase proc in roi.ProcessingFunctions) {
                                            sendstr += "<PROC>" + proc.FunctionName;
                                        }
                                    }
                                }
                            }

                            //AndroidServer.Send(AndroidServer.ClientObject.workSocket, sendstr);
                        }
                        break;
                    default: break;
                }
            } catch (Exception exp) {

                log.Error(exp);
            }
        }


        void __roicontainer_MouseClick(object sender, MouseEventArgs e) {
            try {

                Point mousepoint = _ImageContainer.__roicontainer.GetPositionOnPic(e.Location, false);

                if (e.Button == System.Windows.Forms.MouseButtons.Right) {

                    var listshapes = _ImageContainer.__roicontainer.GetShapesAt(e.Location);
                    if (listshapes.Count()<1) {
                        return;
                    }
                    _ImageContainer.__roicontainer.SelectShapes(listshapes.ElementAt(0));


                    if (ArrowPad.Location.X == 0 && ArrowPad.Location.Y == 0) {
                        ArrowPad.Location = new Point(_ImageContainer.__roicontainer.Width / 2, _ImageContainer.__roicontainer.Height / 2);
                    }
                    // ArrowPad.Location = new Point(mousepoint.X, mousepoint.Y - 30);                         

                    ArrowPad.BringToFront();
                    ArrowPad.SelectedProcFunction = (ProcessingFunctionBase)_ListROIForm.__RoiProcList.SelectedObject;
                    ArrowPad.Show();


                }
                else {
                    //if (SelectedProject == null) {
                    //    return;
                    //}
                    //if (SelectedProject.SelectedRequest == null) {
                    //    return;
                    //}
                    //if (SelectedProject.SelectedRequest.SelectedInspection == null) {
                    //    return;
                    //}
                    //Image<Bgr, Byte> temp2 = null;
                    //Image<Bgr, Byte> temp = null;
                    //lock (SelectedProject.SelectedRequest.SelectedInspection.ImageLocker) {

                    //    temp2 = SelectedProject.SelectedRequest.SelectedInspection.ResultImageBgr;

                    //    if (temp2 == null) {
                    //        return;
                    //    }


                    //    temp = new Image<Bgr, byte>(temp2.Size);
                    //    temp2.CopyTo(temp);
                    //}
                    //if (temp==null) {
                    //    return;
                    //}
                    //if (temp.Size == Size.Empty) {
                    //    return;
                    //}

                    //temp.ROI = Rectangle.Empty;

                    //if (mousepoint.X > temp.Width || mousepoint.Y > temp.Height) {
                    //    return;
                    //}
                    //temp.Draw(new Cross2DF(new PointF(mousepoint.X, mousepoint.Y), 10, 10), new Bgr(Color.Yellow), 1);

                    //Rectangle rect = new Rectangle(mousepoint.X-1, mousepoint.Y-1, 3, 3);

                    //temp.ROI = rect;
                    ////temp.SetValue(new Bgr(Color.Blue),null);

                    //Double[] MinValues, MaxValues;
                    //Point[] MinLoc, MaxLoc;



                    //temp[0].MinMax(out MinValues, out MaxValues, out MinLoc, out MaxLoc);

                    //if (MaxValues.Count() > 0) {
                    //    __grayvalue.Text = MaxValues[0].ToString();
                    //}

                    //__x1.Text = mousepoint.X.ToString();
                    //__y1.Text = mousepoint.Y.ToString();

                    //temp.ROI = Rectangle.Empty;

                    //_ImageContainer.DoLoadBackImage(temp, false);


                    ArrowPad.Hide();
                }
            } catch (Exception exp) {

                log.Error(exp);
            }
        }


        void __RoiProcList_MouseClick(object sender, MouseEventArgs e) {
            try {

                if (_ListROIForm.__RoiProcList.SelectedObject != null) {



                    HighlightRoiProc((ProcessingFunctionBase)_ListROIForm.__RoiProcList.SelectedObject);



                }
            } catch (Exception exp) {

                log.Error(exp);
            }
        }




        public class HistoryCaption {
            private String _HistoryCaption;

            public String HistoryCaption1 {
                get { return _HistoryCaption; }

            }
            public HistoryCaption(String historyCaption) {
                _HistoryCaption = historyCaption;
            }
        }

        void UndoDropDown_Opening(object sender, CancelEventArgs e) {

            ToolStripUndoSelector c = (ToolStripUndoSelector)this.__btundo.DropDown;
            c.Selector.__history.ClearObjects();
            c.Selector.SelectionCount = 1;
            List<HistoryCaption> HistoryCaptions = new List<HistoryCaption>();
            int showcount = 0;
            foreach (String item in UndoRedoManager.UndoCommands) {
                HistoryCaptions.Add(new HistoryCaption(item));
                if (++showcount > 6) {
                    break;
                }

            }
            c.Selector.olvColumn1.Text = this.GetResourceText("Undo_History");;
            c.Selector.__history.AddObjects(HistoryCaptions);


        }

        void RedoDropDown_Opening(object sender, CancelEventArgs e) {

            ToolStripUndoSelector c = (ToolStripUndoSelector)this.__btredo.DropDown;
            c.Selector.__history.ClearObjects();
            c.Selector.SelectionCount = 1;
            List<HistoryCaption> HistoryCaptions = new List<HistoryCaption>();
            int showcount = 0;
            foreach (String item in UndoRedoManager.RedoCommands) {
                HistoryCaptions.Add(new HistoryCaption(item));
                if (++showcount > 6) {
                    break;
                }

            }
            c.Selector.olvColumn1.Text = this.GetResourceText("Redo_History");
            c.Selector.__history.AddObjects(HistoryCaptions);


        }


        void __btRedook_Click(object sender, EventArgs e) {
            ToolStripUndoSelector c = (ToolStripUndoSelector)this.__btredo.DropDown;

            for (int i = 1; i <= c.Selector.SelectionCount; i++) {
                UndoRedoManager.Redo();
            }

            __btredo.HideDropDown();
        }


        void __btUndook_Click(object sender, EventArgs e) {
            ToolStripUndoSelector c = (ToolStripUndoSelector)this.__btundo.DropDown;

            for (int i = 1; i <= c.Selector.SelectionCount; i++) {
                UndoRedoManager.Undo();
            }

            __btundo.HideDropDown();
        }

        




        Thread HighlightThread;
        ProcessingFunctionBase HighlightProc;

        void HighlightRoiProc(ProcessingFunctionBase ProcToHighlight) {
            HighlightProc = ProcToHighlight;

            if (HighlightThread != null) {
                HighlightThread.Abort();
                HighlightThread.Join();
                HighlightThread = null;
            }
            HighlightThread = new Thread(new ThreadStart(DoHighlight));
            HighlightThread.IsBackground = true;
            HighlightThread.Name = this.Name + ":Highlight Roi Proc";
            HighlightThread.Start();

        }

        private void DoHighlight() {
            BeginInvoke(new MethodInvoker(delegate {
                try {
                    if (HighlightProc != null) {
                        lock (SelectedProject.SelectedRequest.SelectedInspection.ImageLocker) {
                            Image<Bgr, Byte> temp = new Image<Bgr, byte>(SelectedProject.SelectedRequest.SelectedInspection.ResultImageBgr.ToBitmap());
                            if (temp != null) {
                                if (HighlightProc.IdentRegion.Size != Size.Empty) {

                                    temp.ROI = SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.ROIShape.ShapeEfectiveBounds;
                                    temp.SetValue(new Bgr(Color.Blue), HighlightProc.IdentRegion);
                                    temp.ROI = Rectangle.Empty;

                                    _ImageContainer.DoLoadBackImage(temp, false);
                                }
                            }
                        }
                    }
                }
                catch (Exception exp) {

                    log.Error(exp);
                }
            }));
            Thread.Sleep(500);
            lock (SelectedProject.SelectedRequest.SelectedInspection.ImageLocker) {
                BeginInvoke(new MethodInvoker(delegate {
                    try {
                        if (SelectedProject.SelectedRequest.SelectedInspection.ResultImageBgr == null) {
                            return;
                        }
                        _ImageContainer.DoLoadBackImage(SelectedProject.SelectedRequest.SelectedInspection.ResultImageBgr, false);
                    }
                    catch (Exception exp) {

                        log.Error(exp);
                    }
                })); 
            }



            return;


        }





        void __listInputs_CellEditFinishing(object sender, CellEditEventArgs e) {
            if (e.Column.Text == "Parameter") {

                ResultInput theresinput = (ResultInput)e.RowObject;

                if (theresinput != null) {
                    ResultInfo theresinfo = (ResultInfo)_ResultsConfiguration.__listinspResults.SelectedObject;
                    if (theresinfo != null) {

                        using (UndoRedoManager.Start(theresinfo + ": Result parameter changed: " + (String)e.NewValue)) {
                            theresinput.Parameter = (String)e.NewValue;
                            UndoRedoManager.Commit();
                        }
                    }
                }

            }
        }

        void __listinspResults_CellEditFinishing(object sender, CellEditEventArgs e) {
            if (e.RowObject != null && (e.Column.Text == "Result Name")) {
                ResultInfo theres = (ResultInfo)e.RowObject;
                using (UndoRedoManager.Start(SelectedProject.SelectedRequest.SelectedInspection + ": Result info name changed: " + (String)(e.NewValue))) {
                    theres.ID = (String)e.NewValue;
                    UndoRedoManager.Commit();
                }
            }
        }



        void UndoRedoManager_CommandDone(object sender, CommandDoneEventArgs e) {
            try {

                __btundo.Enabled = UndoRedoManager.CanUndo;
                __btredo.Enabled = UndoRedoManager.CanRedo;


                if (UndoRedoManager.CanUndo) {
                    __btundo.ToolTipText = UndoRedoManager.UndoCommands.First();
                    if (__btundo.ToolTipText.Contains("Circle type changed")) {
                        _ListROIForm.__propertyGridFunction.Refresh();
                    }
                }
                else {
                    __btundo.ToolTipText =this.GetResourceText("Undo_not_avaible");
                }

                if (UndoRedoManager.CanRedo) {
                    __btredo.ToolTipText = UndoRedoManager.RedoCommands.First();
                }
                else {
                    __btredo.ToolTipText = this.GetResourceText("Redo_not_avaible"); 
                }

                if (e.CommandDoneType == CommandDoneType.Redo || e.CommandDoneType == CommandDoneType.Undo) {


                    _ListROIForm.__propertyGridFunction.Refresh();


                    // TODO ?
                    _ListInspForm.__ListRequests.RefreshObjects(SelectedProject.RequestList);

                    if (SelectedProject.SelectedRequest == null) {
                        return;

                    }
                    _ListInspForm.__listinspections.RefreshObjects(SelectedProject.SelectedRequest.Inspections);


                    _ListInspForm.__propertyGridinsp.Refresh();

                    if (SelectedProject.SelectedRequest.SelectedInspection == null) {
                        return;
                    }
                    _ResultsConfiguration.__listinspResults.RefreshObjects(SelectedProject.SelectedRequest.Results);
                    _ListInspForm.__listRoi.RefreshObjects(SelectedProject.SelectedRequest.SelectedInspection.ROIList);


                    ResultInfo theresultinfo = _ResultsConfiguration.__listinspResults.SelectedObject as ResultInfo;
                    if (theresultinfo != null) {
                        _ResultsConfiguration.__listInputs.RefreshObjects(theresultinfo.Inputs);
                    }

                    if (SelectedProject.SelectedRequest.SelectedInspection.SelectedROI == null) {
                        return;
                    }


                    _ListROIForm.__RoiProcList.RefreshObjects(SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.ProcessingFunctions);

                }
            } catch (Exception exp) {

                log.Error(exp);
            }

        }

        void __btsave_Click(object sender, EventArgs e) {
            try {
                if (_projconfig != null) {
                    _projconfig.Projects.Clear();
                    _projconfig.Projects.AddRange(_ProjectOptionsForm.__listprojects.Objects.Cast < VisionProject>());
                    _projconfig.WriteConfigurationFile();
                }
            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        


        Stopwatch stopWatch = new Stopwatch();
        
       

        private void DoRequest(Object Sender, String param) {
            try {
               

                if (SelectedProject != null) {

                    Request _request = SelectedProject.RequestList.Find(Request => Request.ID == int.Parse(param));




                    if (_request != null) {
                        log.Info("Processing request: " + _request.Name);
                        if (_request.ProcessRequest(Sender,true,null)) {
                           
                            
                        } else {
                            ((TCPServer)Sender).Client.Write("ERROR|04");
                        }
                    }
                    else {
                        ((TCPServer)Sender).Client.Write("ERROR|03");
                    }

                }
                else {
                    ((TCPServer)Sender).Client.Write("ERROR|03");
                }
            } catch (Exception exp) {
                ((TCPServer)Sender).Client.Write("ERROR|02");
                log.Error(exp);
            }
        }





        void __propertyGridinsp_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            if (SelectedProject.SelectedRequest.SelectedInspection != null) {
                //_ListInspForm.__propertyGridinsp.SelectedObject = null;
                //_ListInspForm.__propertyGridinsp.SelectedObject = SelectedInspection;
                _ListInspForm.__propertyGridinsp.Refresh();

            }
            _ListInspForm.__propertyGridinsp.Refresh();

        }

        

        void _ProjectOptionsForm_FormClosed(object sender, FormClosedEventArgs e) {
            
        }



        void proc_OnUpdateResultImage(Image<Bgr, byte> imgout) {
            if (InvokeRequired) {
                BeginInvoke(new MethodInvoker(delegate { proc_OnUpdateResultImage(imgout); }));
            }
            else {
                Rectangle preroi = imgout.ROI;
                imgout.ROI = Rectangle.Empty;
                _ImageContainer.DoLoadBackImage(imgout,false);
                imgout.ROI = preroi;
            }
        }


        void _referenceContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            foreach (Request req in SelectedProject.RequestList) {
                foreach (Inspection insp in req.Inspections) {


                }
            }

        }



        
        void Cliente_OnServerImage(string Request, string Inspection, Image<Bgr, byte> NewImage) {
            if (NewImage != null) {
                if (NewImage.Data != null) {

                    if (NewImage.Size.Width > 0 && NewImage.Size.Height > 0) {
                        Request therequest = SelectedProject.RequestList.Find(ReqName => ReqName.Name == Request);

                        if (therequest != null) {
                            Inspection theInsp = therequest.Inspections.Find(insp => insp.Name == Inspection);
                            if (theInsp != null) {
                                //theInsp.OriginalImageBgr = NewImage;
                                theInsp.Execute(null,false);
                            }
                        }
                    }
                }

            }

        }





        void Cliente_OnServerMessage(string[] Commands) {
            if (InvokeRequired) {
                BeginInvoke(new MethodInvoker(delegate { Cliente_OnServerMessage(Commands); }));
            }
            else {
                try {
                    if (Commands[0].Contains("GET")) {
                        if (Commands[1].Contains("CAMERAS")) {
                            String cam_rec_list = StaticObjects.base64Decode(Commands[2]);


                            //CostumCamerasList cameras=(CostumCamerasList)(IOFunctions.DeserializeFromString(cam_rec_list, typeof(CostumCamerasList)));


                            //CamerasEditor.RemoteCameras.Clear();
                            //CamerasEditor.RemoteCameras.AddRange(cameras);

                        }
                    }
                    else if (Commands[0].Contains("IMAGE")) {
                        String reqName = Commands[1];
                        String inspName = Commands[2];

                        //Request therequest = SelectedProject.RequestList.Find(ReqName => ReqName.Name == reqName);

                        //if (therequest!=null) {
                        //    Inspection theInsp= therequest.Inspections.Find(insp => insp.Name == inspName);
                        //    if (theInsp!=null) {
                        //        //string imagestr = base64Decode(Commands[3]);

                        //        Bitmap bmp = byteArrayToImage(Encoding.Unicode.GetBytes(Commands[3]));
                        //        //Image<Bgr, byte> receivedImage = new Image<Bgr, byte>(bmp);

                        //        //XmlDocument doc = new XmlDocument();
                        //        //doc.LoadXml(imagestr);                                
                        //        //theInsp.OriginalImageBgr = (Image<Bgr, Byte>)(new XmlSerializer(typeof(Image<Bgr, Byte>))).Deserialize(new XmlNodeReader(doc));
                        //        //theInsp.OriginalImageBgr = new Image<Bgr, byte>(bmp);
                        //        //theInsp.DoProcess(false, false, true, false);
                        //    }
                        //}

                    }
                } catch (Exception exp) {

                    log.Error(exp);
                }
            }
        }

        private Bitmap byteArrayToImage(byte[] byteArrayIn) {
            if (byteArrayIn != null) {
                using (MemoryStream stream = new MemoryStream(byteArrayIn)) {
                    return (Bitmap)Bitmap.FromStream(stream);
                }
            }
            return null;
        }

        void SendProjectToRemote(TCPClientConnection client, Boolean Confirm = false) {

           

            Confirm = true;
            if (Confirm) {
                DialogResult dlgResult = MessageBox.Show("Send project to remote application?", "Confirm option", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult == System.Windows.Forms.DialogResult.Yes) {
                    Confirm = false;
                }
            }

            if (!Confirm) {

                if (SelectedProject.SelectedRequest.SelectedInspection.SelectedROI!=null) {
                    Rectangle shaperect = SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.ROIShape.ShapeEfectiveBounds;
                    
                    client.Write("SETROI|"+SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.Name+"|"+shaperect.X + "|" + shaperect.Y + "|" + shaperect.Width + "|" + shaperect.Height + "\n\r");
                    Thread.Sleep(10);
                    if (SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.ProcessingFunctions.Count>0) {
                        int thresh = SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.ProcessingFunctions[0].ImagePreProc1.Threshold;
                        client.Write("SETTHRESHOLD|" + SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.Name +"|" +thresh.ToString() + "\n\r");                        
                        double minval = SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.ProcessingFunctions[0].Mincount;
                        double maxval = SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.ProcessingFunctions[0].Maxcount;
                        client.Write("SETMINMAX|" + SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.Name + "|" + minval.ToString() + "|" + maxval.ToString() + "\n\r");
                        Thread.Sleep(10);
                    }
                }
                //String serializedProjects = IOFunctions.SerializeObject<VisionProjects>(_projconfig);                                

            }

        }

        
        
        

        void newRequest_OnRequestNameChanged(string RequestName) {

            foreach (Request req in SelectedProject.RequestList) {
                if (req != null) {
                    //req.Inspections.ForEach(ov=>ov.OverrideResults.ForEach(n=>updateoverrides(n,RequestName)));
                }
            }

            ResultInfo resultinf=_ResultsConfiguration.__listinspResults.SelectedObject as ResultInfo;

            if (resultinf!=null) {
                //_ResultsConfiguration.__listOverrides.Objects = SelectedProject.SelectedRequest.SelectedInspection.OverrideResults;
            }

            foreach (Inspection insp in SelectedProject.SelectedRequest.Inspections) {

                insp.RequestName = RequestName;

            }

            ResultInfo theresinfo = (ResultInfo)(_ResultsConfiguration.__listinspResults.SelectedObject);
            if (theresinfo == null) {
                return;
            }
            ResultInfo.ListInputs ins = theresinfo.Inputs;

            if (ins == null) {
                return;
            }

            _ResultsConfiguration.__listInputs.RefreshObjects(ins);

        }
        


        void __btRemResFuncProc_Click(object sender, EventArgs e) {
            try {
                if (SelectedProject.SelectedRequest.SelectedInspection != null) {


                }
            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        void originalToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {


                if (SelectedProject.SelectedRequest.SelectedInspection != null) {
                    SelectedProject.SelectedRequest.SelectedInspection.OriginalImageBgr.Save(_saveFileDialog1.FileName);
                }
            }
        }




        void __listprojects_SelectedIndexChanged(object sender, EventArgs e) {
            if (_ProjectOptionsForm.__listprojects.SelectedIndex < 0) {
                _ProjectOptionsForm.__btLoadProj.Enabled =_ProjectOptionsForm.__btduplicate.Enabled= false;
            }
            else {
                _ProjectOptionsForm.__btLoadProj.Enabled = _ProjectOptionsForm.__btduplicate.Enabled = true;
            }
        }






        void __btLoadProj_Click(object sender, EventArgs e) {
            _ProjectOptionsForm.Close();
            CloseCurrentConfiguration(false);
            if (_ProjectOptionsForm.__listprojects.SelectedObject == null) {
                return;
            }
            LoadProject(((VisionProject)_ProjectOptionsForm.__listprojects.SelectedObject).Name, true);

        }


        void ActiveDebugController_OnDebugMessage(object sender, DebugMessageArgs e) {
            if (InvokeRequired) {
                BeginInvoke(new MethodInvoker(delegate { ActiveDebugController_OnDebugMessage(sender, e); }));
            }
            else {

                switch (e.MessageType) {
                    case MessageType.Debug:
                        break;
                    case MessageType.Error:
                        _LogForm.__tabControlLog.SelectedTab = _LogForm.__tabexceptions;
                        _LogForm.TimeChangeColor.Enabled = true;
                        _LogForm.Show();
                        break;
                    case MessageType.Fatal:
                        break;
                    case MessageType.Information:
                        break;
                    case MessageType.Status:
                        break;
                    case MessageType.Warning:
                        break;
                    default:
                        break;
                }

            }

        }


        protected Point TranslateZoomMousePosition(Point coordinates, Image theimage) {
            // test to make sure our image is not null
            if (theimage == null) return coordinates;
            // Make sure our control width and height are not 0 and our 
            // image width and height are not 0
            if (Width == 0 || Height == 0 || theimage.Width == 0 || theimage.Height == 0) return coordinates;
            // This is the one that gets a little tricky. Essentially, need to check 
            // the aspect ratio of the image to the aspect ratio of the control
            // to determine how it is being rendered
            float imageAspect = (float)theimage.Width / theimage.Height;
            float controlAspect = (float)Width / Height;
            float newX = coordinates.X;
            float newY = coordinates.Y;
            if (imageAspect > controlAspect) {
                // This means that we are limited by width, 
                // meaning the image fills up the entire control from left to right
                float ratioWidth = (float)theimage.Width / Width;
                newX *= ratioWidth;
                float scale = (float)Width / theimage.Width;
                float displayHeight = scale * theimage.Height;
                float diffHeight = Height - displayHeight;
                diffHeight /= 2;
                newY -= diffHeight;
                newY /= scale;
            }
            else {
                // This means that we are limited by height, 
                // meaning the image fills up the entire control from top to bottom
                float ratioHeight = (float)theimage.Height / Height;
                newY *= ratioHeight;
                float scale = (float)Height / theimage.Height;
                float displayWidth = scale * theimage.Width;
                float diffWidth = Width - displayWidth;
                diffWidth /= 2;
                newX -= diffWidth;
                newX /= scale;
            }
            return new Point((int)newX, (int)newY);
        }




        void _viewinspections_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            _viewinspections.Hide();
        }

      

        public void ProcessResults() {
            ResultsToSend.Clear();
            foreach (Inspection item in SelectedProject.SelectedRequest.Inspections) {
                //TODO
                // item.ProcessResults();
            }


        }


        void __btProcessFunction_Click(object sender, EventArgs e) {
            //   ProcessResults();
        }




        private List<String> ResultsToSend = new List<string>();












        void _IOForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;

        }





        #region Inspection handling
       
        

        void _newinsp_OnCameraSourceChanged(BaseCapture NewCapture) {
            try {
                
                
            }
            catch (Exception exp) {

                log.Error(exp);
            }
        }

        void _newinsp_OnNoPart(Inspection inspection) {

        }

        void _newinsp_OnUndoDeleteInspection(string InspectionName) {
            //TODO
            //Inspection newinsp = new Inspection();
            //newinsp.Name = InspectionName;
            //AddInsp(SelectedProject.SelectedRequest.Name, newinsp);
        }

        void _newinsp_OnCaptureDone(Image<Bgr, byte> CapturedImage) {
            // send image
        }

        void _newinsp_OnRefreshPropertyGrid() {
            _ListInspForm.__propertyGridinsp.Refresh();
        }




        void _newinsp_OnCaptureStopped() {
            if (InvokeRequired) {

                BeginInvoke(new MethodInvoker(delegate { _newinsp_OnCaptureStopped(); }));
            }
            else {
             
                _ListInspForm.__ListRequests.Enabled = true;
                _ListInspForm.__listinspections.Enabled = true;
                _ListInspForm.__listRoi.Enabled = true;
            }


        }






        void UpdateResultImage(Bitmap newImage) {
            try {
                _ImageContainer.DoLoadBackImage(newImage,false);
                _ListROIForm.__propertyGridFunction.Refresh();                
                _ResultsConfiguration.__listinspResults.RefreshObject(_ResultsConfiguration.__listinspResults.SelectedObject);
                if (_ResultsConfiguration.__listinspResults.SelectedObject != null) {
                    ResultInfo res = _ResultsConfiguration.__listinspResults.SelectedObject as ResultInfo;
                    _ResultsConfiguration.__listInputs.Objects = res.Inputs;
                }

            } catch (Exception exp) {

                log.Error(exp);
            }
        }



        void _newinsp_OnInspectionDone(Inspection inspection) {

            try {


                if (SelectedProject.SelectedRequest.SelectedInspection != null) {

                    //if ((inspection.InspPos == SelectedProject.SelectedRequest.SelectedInspection.InspPos) && !(inspection.CaptureSource is InspectionCapture)) {
                    if ((inspection.InspPos == SelectedProject.SelectedRequest.SelectedInspection.InspPos)) {





                        lock (inspection.ImageLocker) {

                            if (inspection.ResultImageBgr == null) return;
                            //Bitmap. 
                            Bitmap newbitmap = inspection.ResultImageBgr.ToBitmap();

                            

                            


                            if (this.InvokeRequired) {
                                this.BeginInvoke(new MethodInvoker(delegate {
                                    //_viewinspections.UpdateImage(inspection);
                                    UpdateResultImage(newbitmap);
                                }));

                            } else {
                                //_viewinspections.UpdateImage(inspection);
                                UpdateResultImage(newbitmap);
                            }
                        }
                        //else
                        //inspection.InspLayer.Visible = false;

                    }
                }


                              


            } catch (Exception exp) {

                log.Error(exp);
            }




        }

        void _viewinspections_OnCtrDoubleClick(object sender, EventArgs e) {
            _ImageContainer.Show();
        }

        void _viewinspections_OnCtrSelected(object sender, EventArgs e) {
            Inspection sel_insp = (Inspection)sender;

            if (sel_insp != null) {
                Request requestobj = SelectedProject.RequestList.Where(name => name.Name == sel_insp.RequestName).First();
                int newindex = _ListInspForm.__ListRequests.IndexOf(requestobj);
                _ListInspForm.__ListRequests.SelectedIndex = newindex;
                SelectedProject.SelectedRequest = (Request)_ListInspForm.__ListRequests.SelectedObject;
                if (_ListInspForm.__ListRequests.SelectedIndex >= 0) {

                    _ListInspForm.__listinspections.SelectedIndex = _ListInspForm.__listinspections.IndexOf(sel_insp);
                }
            }
        }



        void _newinsp_OnROIRemoved(ROI ROIRemoved) {
            try {
                Boolean lockedstate = ROIRemoved.Locked;
                ROIRemoved.Locked = false;
                              
                _ImageContainer.__roicontainer.SelectShapes(ROIRemoved.Name);
                _ImageContainer.__roicontainer.RemoveSelectedShapes();


                _ListInspForm.__listRoi.Objects=SelectedProject.SelectedRequest.SelectedInspection.ROIList;

            } catch (Exception exp) {
                log.Error(exp);
            }
        }



        void newitem_Click(object sender, EventArgs e) {
            ToolStripItem theitem = sender as ToolStripItem;
            if (theitem != null) {

            }
        }

        void _newinsp_OnSelectedROIChanged(ROI ROISelected) {

            try {


                if (ROISelected != null) {
                    _ListROIForm.__grpROIProc.Enabled = true;
                    lockToolStripMenuItem.Checked = ROISelected.Locked;
                    __toolROI.Enabled = true;
                    _ListROIForm.__RoiProcList.Enabled = true;
                    if (KPPVision.AcessLevel == AcessManagement.Acesslevel.Admin) {
                        _ListROIForm.__propertyGridFunction.Enabled = true; 
                    }
                    _ListROIForm.__cbProcFunc.Enabled = true;

                    
                    _ListROIForm.__RoiProcList.Objects=ROISelected.ProcessingFunctions.ToArray();

                    if (SelectedProject.SelectedRequest.SelectedInspection.ROIList.Contains(ROISelected)) {
                        _ListInspForm.__listRoi.SelectedObject = ROISelected;
                    } else if (SelectedProject.SelectedRequest.SelectedInspection.ROIList.AuxROIS.Contains(ROISelected)) {
                        _ListInspForm.__ListAuxROIS.SelectedObject=ROISelected;
                    }
                    
                    _ListROIForm.__grpROIProc.Enabled = true;
                    ROISelected.Locked = false;
                    _ImageContainer.__roicontainer.SelectShapes(ROISelected.ROIShape);
                    ROISelected.Locked = lockToolStripMenuItem.Checked;

                    //TODO PROC FUNC
                    //_resultscontainer.__listResProc.ClearObjects();
                    //_resultscontainer.__listResProc.AddObjects(ROISelected.Results.ResultsProcessing);
                    //_resultscontainer.__listResProc.Refresh();
                    _ListROIForm.__propertyGridFunction.SelectedObject = null;

                }
                else {
                    _ListROIForm.__propertyGridFunction.SelectedObject = null;
                    _ListROIForm.__RoiProcList.Enabled = false;
                    _ListROIForm.__propertyGridFunction.Enabled = false;
                    _ListROIForm.__cbProcFunc.Enabled = false;
                    __toolROI.Enabled = false;
                    _ListROIForm.__propertyGridFunction.SelectedObject = null;
                }



            } catch (Exception exp) {
                log.Error(exp);

            }
        }




        #endregion




        #region main form envents handling




        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            _ProjectOptionsForm.Show();

        }



       

        private Boolean LoadProject(String ProjectName, Boolean ReloadConfigurations) {
            if (ReloadConfigurations) {
                //CloseCurrentConfiguration(true);
                //_projconfig = VisionProjects.ReadConfigurationFile(_projsfile);
                //_projconfig.BackupExtention = ".bkp";
                //_projconfig.BackupFilesToKeep = 5;
                //_projconfig.BackupFolderName = "Backup";     
            }

            return LoadProject(ProjectName);
        }

        //void UpdateProcbaseReference(Object procbase) {
        //    PropertyInfo[] props = procbase.GetType().GetProperties();
        //    foreach (PropertyInfo item in props) {
        //        //if (item.PropertyType == typeof(ResultReference)) {
        //            UpdateProcReference(item.GetValue(procbase, null));
        //            Object teste = item.GetValue(procbase, null);
        //            if (teste!=null) {
        //                if (teste.GetType().Namespace!="System") {
        //                    UpdateProcbaseReference(teste);
        //                }
                        
        //            }
        //       // } 

        //    }
        //}

        private Boolean LoadProject(String ProjectName) {

            if (this.InvokeRequired) {
                Boolean ok = false;
                this.Invoke(new MethodInvoker(delegate { 
                    ok=LoadProject(ProjectName);
                }));
                return ok;
            } else {
                try {


                    int prognumber = -1;
                    VisionProject newselected = null;

                    if (int.TryParse(ProjectName, out prognumber)) {
                        newselected = _projconfig.Projects.Find(projname => projname.ProjectID == prognumber);
                    } else {
                        newselected = _projconfig.Projects.Find(projname => projname.Name == ProjectName);
                    }

                    if (newselected != null) {

                        if (SelectedProject != null) {


                            if (newselected.Name == SelectedProject.Name) {
                                return true;
                            } else {
                                CloseCurrentConfiguration(true);
                            }

                        }

                        SelectedProject = newselected;
                        StaticObjects.SelectedProject = SelectedProject;
                        _ListInspForm.SelectedProject = SelectedProject;
                        _ListROIForm.SelectedProject = SelectedProject;
                        _ImageContainer.SelectedProject = SelectedProject;

                        SelectedProject.OnSelectedRequestChanged += new VisionProject.SelectedRequestChanged(SelectedProject_OnSelectedRequestChanged);
                    



                        StaticObjects.ReferencePoints.Clear();
                        ReferencePoint dummyRefeence = new ReferencePoint();
                        dummyRefeence.ReferencePointName = "N/A";
                        StaticObjects.ReferencePoints.Add(dummyRefeence);

                        SelectedProject.OnRequestRemoved += new VisionProject.RequestRemoved(SelectedProject_OnRequestRemoved);
                        SelectedProject.RequestList.OnItemAdded += new DejaVu.Collections.Generic.UndoRedoList<Request>.ItemAdded(RequestList_OnItemAdded);
                        foreach (Request _request in SelectedProject.RequestList) {

                            _request.Inspections.OnItemAdded += new DejaVu.Collections.Generic.UndoRedoList<Inspection>.ItemAdded(Inspections_OnItemAdded);
                            _ListInspForm.__listinspections.Items.Clear();
                            log.Status("Loading inspections");

                            _request.OnSelectedInspectionChanged += new Request.SelectedInspectionChanged(_request_OnSelectedInspectionChanged);
                            _request.OnRequestNameChanged += new Request.RequestNameChanged(newRequest_OnRequestNameChanged);

                            _request.OnRequestStartHandler += new Request.RequestStart(_request_OnRequestStart);
                            //_request.OnRequestDoneHandler += new Request.RequestDone(_request_OnRequestDone);

                            _request.Inspections.OnUndoList += new DejaVu.Collections.Generic.UndoRedoList<Inspection>.UndoList(Inspections_OnUndoList);
                            _request.OnInspectionRemoved += new Request.InspectionRemoved(_request_OnInspectionRemoved);

                            int i = 0;
                            foreach (Inspection _inspect in _request.Inspections) {

                                //StaticObjects.InspectionReferences.Add(new InspectionReference(_request, _inspect));
                                _inspect.PropertyChanged += new PropertyChangedEventHandler(_inspect_PropertyChanged);
                                _inspect.ROIList.OnItemAdded += new DejaVu.Collections.Generic.UndoRedoList<ROI>.ItemAdded(ROIList_OnItemAdded);
                                i += 1;
                                _inspect.RequestID = _request.ID;
                                _inspect.UpdateInspection();
                                if (_inspect.CaptureSource != null) {
                                    _inspect.CaptureSource.UpdateSource();
                                }
                                Inspections_OnItemAdded(_inspect);



                                if (_inspect.CaptureSource is InspectionCapture) {
                                    InspectionCapture cap = _inspect.CaptureSource as InspectionCapture;

                                    List<String> SourceInfo = cap.InspectionName.Split(new Char[] { '.' }, StringSplitOptions.None).ToList();
                                    if (SourceInfo.Count == 2) {
                                        Request thereq = SelectedProject.RequestList.Find(r => r.ID == int.Parse(SourceInfo[1]));
                                        if (thereq != null) {
                                            Inspection theinsp = thereq.Inspections.Find(iname => iname.Name == SourceInfo[0]);
                                            if (theinsp != null) {
                                                cap.InspectionSource = theinsp;
                                                _inspect.CaptureSource = cap;

                                                _inspect.ROIList.AuxROIS = cap.InspectionSource.ROIList.ToList();
                                                if (cap.InspectionSource.CaptureSource != null) {

                                                    InspectionCapture inspcap = cap.InspectionSource.CaptureSource as InspectionCapture;

                                                    while (inspcap != null) {
                                                        if (inspcap.InspectionSource != null) {
                                                            _inspect.ROIList.AuxROIS.AddRange(inspcap.InspectionSource.ROIList);
                                                            inspcap = inspcap.InspectionSource.CaptureSource as InspectionCapture;
                                                        } else {
                                                            break;
                                                        }

                                                    }


                                                }
                                            }
                                        }

                                    }

                                }
                                else if (_inspect.CaptureSource is PythonRemoteCapture) {
                                    PythonRemoteCapture PythonCapture = _inspect.CaptureSource as PythonRemoteCapture;

                                    try {
                                        PythonCapture.Cliente.OnConnectionStateChanged += new TCPClientConnection.ConnectionStateChanged(Cliente_OnConnectionStateChanged);
                                        PythonCapture.Cliente.Connect();
                                        //PythonCapture.OnRemoteImage += new PythonRemoteCapture.RemoteImage(PythonCapture_OnRemoteImage);

                                    } catch (Exception exp) {
                                        log.Error(exp);

                                    }

                                }

                                if (_inspect.ROIList != null) {

                                    foreach (ROI _roi in _inspect.ROIList) {
                                       
                                       // _roi.OnROIPositionChanged += _inspect._newroi_OnROIPositionChanged;
                                        //StaticObjects.AuxiliaryInspectionROISList.Add(_request + "." + _inspect + "." + _roi);
                                        _roi.PropertyChanged += new PropertyChangedEventHandler(_roi_PropertyChanged);
                                        _roi.ProcessingFunctions.OnItemAdded += new DejaVu.Collections.Generic.UndoRedoList<ProcessingFunctionBase>.ItemAdded(ProcessingFunctions_OnItemAdded);

                                        if (_roi.referencePoint != null) {
                                            ReferencePoint newrefpoint = StaticObjects.ReferencePoints.Find(name => name.ReferencePointName == _roi.referencePoint.ReferencePointName);
                                            if (newrefpoint != null) {

                                                using (UndoRedoManager.StartInvisible("Init")) {
                                                    _roi.referencePoint = newrefpoint;
                                                    UndoRedoManager.Commit();
                                                }
                                            }
                                        }


                                        if (_inspect.CaptureSource is InspectionCapture) {
                                            if (!_inspect.ROIList.AuxROIS.Contains(_roi) || (_inspect.ROIList.AuxROIS.Contains(_roi) && (_inspect.CaptureSource as InspectionCapture).ShowAuxROIS)) {
                                                _roi.UpdateROIShape(_inspect.InspLayer);
                                            }
                                        } else {
                                            _roi.UpdateROIShape(_inspect.InspLayer);
                                        }



                                        _roi.PropertyChanged += new PropertyChangedEventHandler(_ListInspForm._SelectedROI_PropertyChanged);
                                        _roi.ProcessingFunctions.OnUndoList += new DejaVu.Collections.Generic.UndoRedoList<ProcessingFunctionBase>.UndoList(ProcessingFunctions_OnUndoList);



                                        foreach (ProcessingFunctionBase item in _roi.ProcessingFunctions) {
                                            if (item is ProcessingFunctionPrePos) {
                                                ((ProcessingFunctionPrePos)item).OnNewPrepos += new ProcessingFunctionPrePos.NewPrepos(Form1_OnNewPrepos); //+= new ProcessingFunctionPrePos.NewPrepos(Form1_OnNewPrepos);
                                                ReferencePoint newrefpoint = new ReferencePoint(((ProcessingFunctionPrePos)item).FunctionName);
                                                StaticObjects.ReferencePoints.Add(newrefpoint);

                                            }
                                            item.OnFunctionNameChanged += new ProcessingFunctionBase.FunctionNameChanged(proc_OnFunctionNameChanged);
                                            item.OnUpdateResultImage += new ProcessingFunctionBase.UpdateResultImage(proc_OnUpdateResultImage);



                                        }
                                    }


                                }

                                //_inspect.InspLayer.Visible = false;


                                log.Status("Inspection " + i + " loaded");


                            }



                        }


                        try {
                            foreach (Request req in SelectedProject.RequestList) {
                                foreach (ResultInfo resinfo in req.Results) {
                                    resinfo.Inputs.OnUndoList += new DejaVu.Collections.Generic.UndoRedoList<ResultInput>.UndoList(Inputs_OnUndoList);
                                    //foreach (ResultInput resinput in resinfo.Inputs) {
                                    //    UpdateProcReference(resinput.Input);
                                    //}
                                }
                                foreach (Inspection insp in req.Inspections) {
                                    //if (insp.AuxiliaryInspection!=null) {
                                    //    insp.AuxiliaryInspection.request = SelectedProject.RequestList.Find(n => n.Name == insp.AuxiliaryInspection.RequestName);

                                    //}
                                    //insp.AuxiliaryInspection = insp.AuxiliaryInspection;
                                    //foreach (ROI roi in insp.ROIList) {
                                    //    foreach (ProcessingFunctionBase procbase in roi.ProcessingFunctions) {
                                    //        UpdateProcbaseReference(procbase);
                                    //    }
                                    //}
                                }
                            }
                        } catch (Exception exp) {

                            log.Error(exp);
                        }

                        _ListInspForm.__tabsInsp.Enabled = true;

                        __btCloseProj.Enabled = true;
                        __btSaveproj.Enabled = true;

                        _ImageContainer.__roicontainer.SelectNone();
                        _ListInspForm.__ListRequests.Focus();
                        _ListInspForm.__ListRequests.AddObjects(SelectedProject.RequestList);

                        //_ListInspForm.__auxinsp.DataBindings.Add(new Binding("Text",SelectedProject.SelectedRequest.SelectedInspection.AuxiliaryInspectionROIS
                        log.Status("Project loaded");

                        SelectedProject.RequestList.OnUndoList += new DejaVu.Collections.Generic.UndoRedoList<Request>.UndoList(RequestList_OnUndoList);


                        //_ListInspForm.__comboListAuxROI.DataSource = StaticObjects.InspectionReferences;

                        if (SelectedProject.RequestList.Count > 0) {
                            SelectedProject.SelectedRequest = SelectedProject.RequestList.First();
                        } else {
                            SelectedProject.SelectedRequest = null;
                        }

                        this.Text = "KPP Vision" + " : " + SelectedProject.Name;



                        StaticObjects.SelectedProject = SelectedProject;


                       


                        _ListInspForm.__listinspections.Sort(_ListInspForm.olvColumnInsppos, SortOrder.Ascending);
                        _ListROIForm.__RoiProcList.Sort(_ListROIForm.olvColumnProcPOS, SortOrder.Ascending);
                        _ListInspForm.__listRoi.Sort(_ListInspForm.__olvROIPos, SortOrder.Ascending);







                        _ListInspForm.__ListRequests.SelectedObject = SelectedProject.SelectedRequest;
                        if (SelectedProject.SelectedRequest != null) {
                            if (SelectedProject.SelectedRequest.SelectedInspection != null) {
                                _ListInspForm.__listinspections.SelectedObject = SelectedProject.SelectedRequest.SelectedInspection;
                                if (SelectedProject.SelectedRequest.SelectedInspection.ROIList.Count > 0) {
                                    //SelectedProject.SelectedRequest.SelectedInspection.SelectedROI = SelectedProject.SelectedRequest.SelectedInspection.ROIList.First(); 
                                }
                            }
                        }


                    } else {
                        return false;
                    }
                    __btSaveproj.Enabled = true;
                    StaticObjects.isLoading = false;
                    _ListInspForm.__tabsInsp.Enabled = true;
                    _ListInspForm.__tabsRequest.Enabled = true;
                    _ListInspForm.__tabsInspconf.Enabled = true;
                    if (UndoRedoManager.IsCommandStarted) {
                        UndoRedoManager.Commit();
                    }
                    return true;
                } catch (Exception exp) {

                    log.Error(exp);
                    return false;
                }

            }


        }

        

        void __logpasstb_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar==13) {

            }
        }

        void PythonCapture_OnRemoteImage(Image<Bgr, byte> theimage) {
            if (this.InvokeRequired) {
                SelectedProject.SelectedRequest.SelectedInspection.OriginalImageBgr = theimage;
                SelectedProject.SelectedRequest.SelectedInspection.ResultImageBgr = theimage;
                this.BeginInvoke(new MethodInvoker(delegate { UpdateResultImage(theimage.ToBitmap()); }));
            }
        }

        void ROIList_OnItemAdded(ROI itemAdded) {
            itemAdded.ProcessingFunctions.OnUndoList += new DejaVu.Collections.Generic.UndoRedoList<ProcessingFunctionBase>.UndoList(ProcessingFunctions_OnUndoList);
        }

        void Inspections_OnItemAdded(Inspection itemAdded) {



            _ImageContainer.__roicontainer.AddLayer(itemAdded.InspLayer);

            _ListInspForm.__listinspections.Update();
            //_newinsp.OnNoPart += new Inspection.NoPart(_newinsp_OnNoPart);
            itemAdded.OnInspectionDone += new Inspection.InspectionDone(_newinsp_OnInspectionDone);
            itemAdded.OnCameraSourceChanged += new Inspection.CameraSourceChanged(_newinsp_OnCameraSourceChanged);
            itemAdded.OnCaptureStopped += new Inspection.CaptureStopped(_newinsp_OnCaptureStopped);
            //itemAdded.OnInspectionResultHandler += new Inspection.InspectionResult(_newinsp_OnInspectionResult);
            itemAdded.OnUndoDeleteInspection += new Inspection.UndoDeleteInspection(_newinsp_OnUndoDeleteInspection);
            itemAdded.OnRefreshPropertyGrid += new Inspection.RefreshPropertyGrid(_newinsp_OnRefreshPropertyGrid);
            itemAdded.OnSelectedROIChanged += new Inspection.SelectedROIChanged(_newinsp_OnSelectedROIChanged);
            //_newinsp.Results.OnUndoList += new DejaVu.Collections.Generic.UndoRedoList<ResultInfo>.UndoList(Results_OnUndoList);
            itemAdded.ROIList.OnUndoList += new DejaVu.Collections.Generic.UndoRedoList<ROI>.UndoList(ROIList_OnUndoList);



            itemAdded.OnROIRemoved += new Inspection.ROIRemoved(_newinsp_OnROIRemoved);


            //_ListInspForm.__listinspections.Objects = SelectedProject.SelectedRequest.Inspections;


            //if (_newinsp.CaptureSource is InspectionCapture) {
            //    ((InspectionCapture)_newinsp.CaptureSource).InspectionName = ((InspectionCapture)_newinsp.CaptureSource).InspectionName;
            //}

            
          
            String caption = itemAdded.RequestName + "-" + itemAdded.Name;
            _viewinspections.AddImage(null, caption, itemAdded);

            _viewinspections.OnCtrSelected += new EventHandler(_viewinspections_OnCtrSelected);
            _viewinspections.OnCtrDoubleClick += new EventHandler(_viewinspections_OnCtrDoubleClick);

            if (SelectedProject.SelectedRequest != null) {
                _ListInspForm.__listinspections.Objects = SelectedProject.SelectedRequest.Inspections;

            }
            
        }

        
        void proc_OnFunctionNameChanged(String OldValue, ref String NewName) {

            String newnameVal = NewName;
            foreach (Request req in SelectedProject.RequestList) {
                foreach (Inspection insp in req.Inspections) {
                    foreach (ROI roi in insp.ROIList) {
                        if (roi.ProcessingFunctions.Find(name => name.FunctionName == newnameVal) != null) {
                            NewName = OldValue;
                        }
                    }
                }
            }

            ReferencePoint therefpoint = StaticObjects.ReferencePoints.Find(refname => refname.ReferencePointName == OldValue);
            if (therefpoint != null) {
                therefpoint.ReferencePointName = NewName;
            }
            _ListInspForm.__listRoi.RefreshObjects(SelectedProject.SelectedRequest.SelectedInspection.ROIList);


            ResultInfo theresultinfo = _ResultsConfiguration.__listinspResults.SelectedObject as ResultInfo;
            if (theresultinfo != null) {
                _ResultsConfiguration.__listInputs.RefreshObjects(theresultinfo.Inputs);
            }

            _ListROIForm.__RoiProcList.RefreshObjects(SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.ProcessingFunctions);
        }



    


        void ProcessingFunctions_OnItemAdded(ProcessingFunctionBase itemAdded) {
            itemAdded.OnUpdateResultImage += new ProcessingFunctionBase.UpdateResultImage(proc_OnUpdateResultImage);
            itemAdded.OnFunctionNameChanged += new ProcessingFunctionBase.FunctionNameChanged(proc_OnFunctionNameChanged);
        }

        void RequestList_OnItemAdded(Request itemAdded) {
            _ListInspForm.__ListRequests.Objects=SelectedProject.RequestList;
            _ListInspForm.__ListRequests.SelectedIndex = _ListInspForm.__ListRequests.GetItemCount() - 1;
            SelectedProject.SelectedRequest = itemAdded;

            itemAdded.OnRequestNameChanged += new Request.RequestNameChanged(newRequest_OnRequestNameChanged);
            itemAdded.OnSelectedInspectionChanged += new Request.SelectedInspectionChanged(_request_OnSelectedInspectionChanged);
            itemAdded.Inspections.OnUndoList += new DejaVu.Collections.Generic.UndoRedoList<Inspection>.UndoList(Inspections_OnUndoList);
            itemAdded.Inspections.OnItemAdded += Inspections_OnItemAdded;
            itemAdded.OnRequestStartHandler += new Request.RequestStart(_request_OnRequestStart);
            //itemAdded.OnRequestDoneHandler += new Request.RequestDone(_request_OnRequestDone);


        }

        void SelectedProject_OnRequestRemoved(Request request) {
            _ListInspForm.__ListRequests.Objects = SelectedProject.RequestList;
            _ListInspForm.__ListRequests.Refresh();
            SelectedProject.SelectedRequest = (Request)_ListInspForm.__ListRequests.GetModelObject(_ListInspForm.__ListRequests.GetItemCount() - 1);
        }

        void _roi_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            _ListROIForm.__propertyGridFunction.Refresh();
            ResultInfo theresultinfo = _ResultsConfiguration.__listinspResults.SelectedObject as ResultInfo;
            if (theresultinfo != null) {
                _ResultsConfiguration.__listInputs.RefreshObjects(theresultinfo.Inputs);
            }
   
        }

        void _request_OnInspectionRemoved(Inspection inspection) {
            _viewinspections.RemoveControl(inspection);
            _ListInspForm.__listinspections.Objects = SelectedProject.SelectedRequest.Inspections.ToArray();

            Inspection _newisnp = (Inspection)_ListInspForm.__listinspections.GetModelObject(_ListInspForm.__listinspections.GetItemCount() - 1);
            _ListInspForm.__listinspections.SelectObject(_newisnp);
            SelectedProject.SelectedRequest.SelectedInspection = _newisnp;            

        }

        void _inspect_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            _ListROIForm.__propertyGridFunction.Refresh();
            ResultInfo theresultinfo = _ResultsConfiguration.__listinspResults.SelectedObject as ResultInfo;
            if (theresultinfo != null) {
                _ResultsConfiguration.__listInputs.Objects = theresultinfo.Inputs;
            }

        }

        

        //void _request_OnRequestDone(Request request, String Results) {
        //    //if (request.SendToOutput) {
        //        Send(request.FromClient, "REQUEST OK|" + request.ID.ToString());
        //        Send(request.FromClient, Results);    
        //    //}
            
        //}

        void _request_OnRequestStart(Request request) {
            //foreach (OutputSource item in request.Configurations.Outputs) {
            //    MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            //    if (method.Name.Contains(item.Event)) {
            //        item.Process("OK");

            //    }
            //}
        }

        void Cliente_OnConnectionStateChanged(TCPClientConnection client, TCPClientConnection.ConnectionState NewState) {
            if (NewState == TCPClientConnection.ConnectionState.Connected) {
                //SendProjectToRemote(client, true);
            }
        }

        void ProcessingFunctions_OnUndoList(List<ProcessingFunctionBase> UndoObject) {
            try {
                int lastselected = _ListROIForm.__RoiProcList.SelectedIndex;
                _ListROIForm.__RoiProcList.SelectedObject = null;
                _ListROIForm.__RoiProcList.ClearObjects();
                _ListROIForm.__RoiProcList.AddObjects(SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.ProcessingFunctions);

                
                _ListROIForm.__RoiProcList.SelectedIndex = lastselected;
            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        void Inputs_OnUndoList(List<ResultInput> UndoObject) {
            try {
                int lastselected = _ResultsConfiguration.__listInputs.SelectedIndex;
                _ResultsConfiguration.__listInputs.SelectedObject = null;
                _ResultsConfiguration.__listInputs.ClearObjects();
                ResultInfo theresinfo = (ResultInfo)_ResultsConfiguration.__listinspResults.SelectedObject;
                _ResultsConfiguration.__listInputs.AddObjects(theresinfo.Inputs);
                _ResultsConfiguration.__listInputs.SelectedIndex = lastselected;
            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        void Results_OnUndoList(List<ResultInfo> UndoObject) {
            try {
                int lastselected = _ResultsConfiguration.__listinspResults.SelectedIndex;
                _ResultsConfiguration.__listinspResults.SelectedObject = null;
                _ResultsConfiguration.__listinspResults.ClearObjects();

                _ResultsConfiguration.__listinspResults.AddObjects(SelectedProject.SelectedRequest.Results);
                _ResultsConfiguration.__listinspResults.SelectedIndex = lastselected;
            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        void ROIList_OnUndoList(List<ROI> UndoObject) {
            try {


                List<ROI> CurrentList = new List<ROI>();
                foreach (ROI item in _ListInspForm.__listRoi.Objects) {
                    CurrentList.Add(item);
                }


                List<ROI> Exclusive = CurrentList.Except(UndoObject).ToList();
                if (Exclusive.Count > 0) {

                    foreach (ROI item in Exclusive) {
                        _ImageContainer.__roicontainer.SelectShapes(item.Name);
                        _ImageContainer.__roicontainer.RemoveSelectedShapes();
                    }
                }
                else {
                    Exclusive = UndoObject.Except(CurrentList).ToList();
                    foreach (ROI item in Exclusive) {
                        item.UpdateROIShape(SelectedProject.SelectedRequest.SelectedInspection.InspLayer);
                    }
                }



                int lastselected = _ListInspForm.__listRoi.SelectedIndex;

                _ListInspForm.__listRoi.ClearObjects();
                _ListInspForm.__listRoi.AddObjects(UndoObject);
                _ListInspForm.__listRoi.SelectedIndex = lastselected;

            } catch (Exception exp) {

                log.Error(exp);

            }
        }

        void Inspections_OnUndoList(List<Inspection> UndoObject) {
            int lastselected = _ListInspForm.__listinspections.SelectedIndex;

            _ListInspForm.__listinspections.ClearObjects();
            _ListInspForm.__listinspections.AddObjects(UndoObject);
            _ListInspForm.__listinspections.SelectedIndex = lastselected;
        }

        void RequestList_OnUndoList(List<Request> UndoObject) {
            int lastselected = _ListInspForm.__ListRequests.SelectedIndex;

            _ListInspForm.__ListRequests.ClearObjects();
            _ListInspForm.__ListRequests.AddObjects(UndoObject);
            _ListInspForm.__ListRequests.SelectedIndex = lastselected;
        }


        void SelectedProject_OnSelectedRequestChanged(Request NewSelectedRequest) {

            if (NewSelectedRequest != null) {


                
                _ListInspForm.__listinspections.Enabled = true;
                
               
                _ListInspForm.__listRoi.ClearObjects();
                
                _ListInspForm.__listinspections.Objects=NewSelectedRequest.Inspections;
                
                _ListInspForm.__propertyGridinsp.SelectedObject = null;

                _ResultsConfiguration.SelectedRequest = NewSelectedRequest;

                Inspection selected = NewSelectedRequest.SelectedInspection;

                NewSelectedRequest.SelectedInspection = null;

                if (selected == null) {
                    if (NewSelectedRequest.Inspections.Count > 0) {
                        NewSelectedRequest.SelectedInspection = NewSelectedRequest.Inspections.First();
                    } else {
                        _request_OnSelectedInspectionChanged(null);
                    }

                }
                else {
                    NewSelectedRequest.SelectedInspection = selected;
                }

                _ListInspForm.__ListRequests.SelectedObject = NewSelectedRequest;


            }
            else {


                _ListInspForm.__ListRequests.SelectedObject = null;



            }



            
            //_ListInspForm.__ListRequests.SelectedObject = NewSelectedRequest;

        }



        //void c_MouseClick(object sender, MouseEventArgs e) {
        //    try {
        //        if (e.Button == System.Windows.Forms.MouseButtons.Right) {

        //            GridItem _parent = _ListInspForm.__propertyGridRequestConf.SelectedGridItem;
        //            if (_parent != null) {
        //                if (_parent.Value is TriggerSource) {
        //                    _ListInspForm.__propertyGridRequestConf.ContextMenuStrip=_ListInspForm.contextconfrequest;

        //                    _ListInspForm.contextconfrequest.Show();
        //                    _ListInspForm.__propertyGridRequestConf.ContextMenuStrip = null;
        //                }
        //            }
        //        }
        //    } catch (Exception exp) {

        //        log.Error(exp);
        //    }
        //}

        void _request_OnSelectedInspectionChanged(Inspection NewSelectedInspection) {
            if (InvokeRequired) {
                BeginInvoke(new MethodInvoker(delegate { _request_OnSelectedInspectionChanged(NewSelectedInspection); }));
            }
            else {

                try {

                    if ((NewSelectedInspection != null)) {

                         
                        _ListInspForm.__propertyGridinsp.SelectedObject = NewSelectedInspection;
                        _ListInspForm.__propertyGridinsp.ExpandAllGridItems();
                        _ListInspForm.__propertyGridinsp.Refresh();

                        _ListInspForm.__listRoi.Enabled = true;
                        _ListInspForm.__ListAuxROIS.Enabled = true;
                        
                        _ImageContainer._lastcaptureimages.DropDownItems.Clear();

                      
                            _ImageContainer.__refImagetool.Visible = false;
                       
                        _ImageContainer.ImagetoolStrip.Enabled = true;

                        _ListROIForm.__RoiProcList.ClearObjects();
                        _ListROIForm.__RoiProcList.Refresh();
                        
                        
                        _ListInspForm.__listRoi.Objects=NewSelectedInspection.ROIList;
                        _ListInspForm.__ListAuxROIS.Objects = NewSelectedInspection.ROIList.AuxROIS;
                      


                        __btInspect.Enabled = true;
                        _ResultsConfiguration.__btAddResult.Enabled = true;
                      
                        _ListInspForm.__listRoi.Enabled = true;
                        _ListInspForm.__listinspections.Enabled = true;

                        if (NewSelectedInspection.CaptureSource!=null) {

                            NewSelectedInspection.CaptureSource.UpdateAttributes();

                            if (NewSelectedInspection.CaptureSource is PythonRemoteCapture) {
                                PythonRemoteCapture PythonCapture = NewSelectedInspection.CaptureSource as PythonRemoteCapture;
                                if (PythonCapture.Cliente.State == TCPClientConnection.ConnectionState.Disconnected) {
                                    PythonCapture.Cliente.Connect();
                                }
                                //PythonCapture.Cliente.Write("SELECTTCPCLIENT|"+NewSelectedInspection.InspPos.ToString()+"|\n");

                                List<Object> avaiblefuncs = new List<object>();
                                foreach (Object item in ReflectionController.ProcessingFunctions.ToList<Object>()) {
                                    ProcessingFunctionDefinition procdef = item as ProcessingFunctionDefinition;

                                    if (PythonRemoteCapture.AvaibleFunctions.Contains(procdef.BaseType)) {
                                        avaiblefuncs.Add(item);
                                    }

                                }
                                _ListROIForm.__cbProcFunc.Objects = avaiblefuncs;
                            } else {
                                _ListROIForm.__cbProcFunc.Objects = ReflectionController.ProcessingFunctions.ToList<Object>();
                            }

                        }

                        
                        
                        //_ListROIForm.__cbProcFunc.
                        try {


                            lock (NewSelectedInspection.ImageLocker) {

                                _ImageContainer.DoLoadBackImage(NewSelectedInspection.ResultImageBgr, false); 
                            }


                        } catch (Exception exp) {
                            
                            _ImageContainer.DoLoadBackImage((Bitmap)null,false);

                            log.Error(exp);
                        }
                      

                        ROI SelectededROI = NewSelectedInspection.SelectedROI;
                        NewSelectedInspection.SelectedROI = null;
                        NewSelectedInspection.SelectedROI = SelectededROI;
                        if (NewSelectedInspection.SelectedROI == null) {

                            _ListROIForm.__grpROIProc.Enabled = false;

                        }
                        else {
                            _ListROIForm.__grpROIProc.Enabled = true;
                        }


                        _ListInspForm.__listinspections.SelectedObject = NewSelectedInspection;
                        _ListInspForm.__propertyGridinsp.SelectedObject = NewSelectedInspection;
                    }
                    else {
                        _ImageContainer.ImagetoolStrip.Enabled = false;
                        _ImageContainer.DoLoadBackImage((Bitmap)null, false);
                        _ListROIForm.__grpROIProc.Enabled = false;

                        __btInspect.Enabled = false;
                     

                        _ListInspForm.__propertyGridinsp.SelectedObject = null;
                        _ListInspForm.__propertyGridinsp.Refresh();
                        
                        _ListInspForm.__listRoi.ClearObjects();
                        _ListInspForm.__listRoi.Refresh();
                        _ListInspForm.__listRoi.Enabled = false;
                        
                        _ListInspForm.__ListAuxROIS.ClearObjects();
                        _ListInspForm.__ListAuxROIS.Refresh();
                        _ListInspForm.__ListAuxROIS.Enabled = false;
                        
                        _ListROIForm.__grpROIProc.Enabled = false;
                        _ListROIForm.__RoiProcList.ClearObjects();
                        _ListROIForm.__RoiProcList.Refresh();
                        
                        _ResultsConfiguration.__BtAddInput.Enabled = _ResultsConfiguration.__btAddResult.Enabled = _ResultsConfiguration.__btRemove.Enabled = _ResultsConfiguration.__BtRemoveInput.Enabled = false;

                    }
                } catch (Exception exp) {


                    log.Error(exp);
                }

            }
        }

        

        void UpdateProcReference(object ProcRef) {


            try {
                StaticObjects.isLoading = true;
                if (ProcRef!=null) {
                    if (ProcRef is ResultReference) {
                        ((ResultReference)ProcRef).ResultReferenceID = ((ResultReference)ProcRef).ResultReferenceID;    
                    }
                    
                }
                
                StaticObjects.isLoading = false;
            } catch (Exception exp) {

                log.Error(exp);
            }
        }






        void Form1_OnNewPrepos(ProcessingFunctionPrePos PrePosFunction) {
            ReferencePoint therefpoint = StaticObjects.ReferencePoints.Find(name => name.ReferencePointName == PrePosFunction.FunctionName);
            if (therefpoint != null) {
                therefpoint.OffsetX = PrePosFunction.XOffset;
                therefpoint.OffsetY = PrePosFunction.YOffset;
            }
        }

        void item_OnPrePosisionOffsetChanged() {
            if (true) {

            }
        }

        internal static void BindField(Control control, string propertyName, object dataSource, string dataMember) {

            try {
                Binding bd;

                for (int index = control.DataBindings.Count - 1; (index == 0); index--) {
                    bd = control.DataBindings[index];
                    if (bd.PropertyName == propertyName)
                        control.DataBindings.Remove(bd);
                }

                control.DataBindings.Add(propertyName, dataSource, dataMember, false, DataSourceUpdateMode.OnValidation);

            } catch (Exception exp) {

                log.Error(exp);
            }

        }



        void theproc_DOUpdateInImage(Bitmap theImage) {

            if (InvokeRequired) {
                BeginInvoke(new MethodInvoker(delegate { theproc_DOUpdateInImage(theImage); }));
                return;
            }
            _ImageContainer.DoLoadBackImage(theImage,false);
        }







        private void SaveCurrentConfiguration() {
            if (_projconfig != null) {
                //TODO: Guarda a configuração anterior
                _projconfig.WriteConfigurationFile();

            }
        }





        private void CloseCurrentConfiguration(bool overrideclose) {

            try {

                if (SelectedProject != null) {




                    this.Text = "KPP Vision";


                    _ListInspForm.__propertyGridinsp.SelectedObject = null;
                    _ListROIForm.__propertyGridFunction.SelectedObject = null;

                    _ListROIForm.__RoiProcList.ClearObjects();
                    _ListROIForm.__RoiProcList.Refresh();
                    _ImageContainer.__roicontainer.ClearLayers();
                    _ImageContainer.__roicontainer.ClearShapes();
                    //_ImageContainer.__roicontainer.NewImage(0, 0);
                    //_ImageContainer.__roicontainer.UnloadBackgroundImage();
                    _ImageContainer.DoLoadBackImage((Bitmap)null,false);

                    _ListInspForm.__listinspections.ClearObjects();
                    _ListInspForm.__listinspections.Refresh();
                    _ListInspForm.__listRoi.ClearObjects();

                    _ListInspForm.__ListRequests.ClearObjects();

                    _viewinspections.ClearControls();

                    SelectedProject.SelectedRequest = null;

                    SelectedProject.Dispose();

                    SelectedProject = null;                   
 
                   
                    _ListInspForm.__tabsInsp.Enabled = false;
                    _ListROIForm.__grpROIProc.Enabled = false;
                    __btSaveproj.Enabled = false;
                    __btCloseProj.Enabled = false;

                    _ResultsConfiguration.__listInputs.ClearObjects();
                    _ResultsConfiguration.__listinspResults.ClearObjects();
                    _ListInspForm.__tabsInsp.Enabled = false;
                    _ListInspForm.__tabsRequest.Enabled = false;
                    _ListInspForm.__tabsInspconf.Enabled = false;


                }


                __btSaveproj.Enabled = false;

            } catch (Exception exp) {

             
                log.Error(exp);
            }

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e) {
            SaveCurrentConfiguration();
        }




        #endregion

        private void CloseAllContents() {
            // we don't want to create another instance of tool window, set DockPanel to null

            _ListROIForm.DockPanel = null;
            _ListInspForm.DockPanel = null;
            _ImageContainer.DockPanel = null;
            _LogForm.DockPanel = null;



            // Close all other document windows
            CloseAllDocuments();
        }


        private void CloseAllDocuments() {
            if (__dockPanel1.DocumentStyle == DocumentStyle.SystemMdi) {
                foreach (Form form in MdiChildren)
                    form.Close();
            }
            else {
                for (int index = __dockPanel1.Contents.Count - 1; index >= 0; index--) {
                    if (__dockPanel1.Contents[index] is IDockContent) {
                        IDockContent content = (IDockContent)__dockPanel1.Contents[index];
                        content.DockHandler.Close();
                    }
                }
            }
        }

        string dockFile = "";

        public void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            try {
                System.Windows.Forms.DialogResult dialogresult;
                if (e != null) {
                    if (e.CloseReason == CloseReason.UserClosing) {


                    }
                }
                if (!_silentexit) {
                    dialogresult = MessageBox.Show(this.GetResourceText("Close_application"), this.GetResourceText("Confirm_option"), MessageBoxButtons.YesNo);
                }
                else {
                    dialogresult = System.Windows.Forms.DialogResult.Yes;
                }

                if (dialogresult == System.Windows.Forms.DialogResult.No) {
                    e.Cancel = true;
                }
                else {



                    Thread splashthread = new Thread(new ThreadStart(SplashScreen.ShowSplashScreen));
                    splashthread.IsBackground = true;
                    splashthread.Start();
                    Thread.Sleep(10);
                    SplashScreen.WindowLocation = new Point(SplashScreen.WindowLocation.X, SplashScreen.WindowLocation.Y - 100);
                    SplashScreen.UdpateStatusTextWithStatus(this.GetResourceText("Closing_application"), TypeOfMessage.Error);
                    if (SelectedProject != null) {
                        SelectedProject.Dispose();
                    }
                    //TODO Loading project
                    //if (_appconfig != null) {

                    //    _appconfig.WriteConfigurationFile(_appfile);
                    //}
                    


                    SplashScreen.UdpateStatusTextWithStatus(this.GetResourceText("Stopping_remote_connection"), TypeOfMessage.Error);






                    

                    dockFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\DockPanel.config");
                    //if (File.Exists(configFile) == false) {
                    __dockPanel1.SaveAsXml(dockFile);
                    //}
                  

                    SplashScreen.UdpateStatusTextWithStatus(this.GetResourceText("Exiting"), TypeOfMessage.Error);
                    AppPerformanceMonitor.Stop();
                }
            } catch (Exception exp) {

                log.Error(exp);
            }



        }




        private void __btCapture_Click(object sender, EventArgs e) {
            try {

                if (SelectedProject.SelectedRequest.SelectedInspection != null) {                    
                    SelectedProject.SelectedRequest.SelectedInspection.Execute(null,true);


                }
            } catch (Exception exp) {

                log.Error(exp);
            }
        }





        private void __roistoolmenu_SelectedIndexChanged(object sender, EventArgs e) {
            try {

            } catch (Exception exp) {

                log.Error(exp);
            }

        }

        private void __roistoolmenu_TextUpdate(object sender, EventArgs e) {

        }

        private void __roistoolmenu_MouseUp(object sender, MouseEventArgs e) {

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {

        }



        private void applicationConfigurationToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                //  _appconfig.Cliente = new TcpConnection();

            } catch (Exception exp) {

                log.Error(exp);
            }
        }


        private void __btCapture_cont_Click(object sender, EventArgs e) {
          

            _ListInspForm.__ListRequests.Enabled = false;
            _ListInspForm.__listinspections.Enabled = false;
            _ListInspForm.__listRoi.Enabled = false;
                       

        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                //_appconfig.WriteConfigurationFile(_appfile);
            } catch (Exception exp) {

                log.Error(exp);
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            if (SelectedProject.SelectedRequest.SelectedInspection != null) {
                
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {

            Thread.Sleep(10);
            SplashScreen.CloseSplashScreen();

        }



        private void inspectionToolStripMenuItem_Click(object sender, EventArgs e) {
            
        }

        private void __toolLocalStartLoad_Click(object sender, EventArgs e) {
            //Server.StartOnLoad = __toolLocalStartLoad.Checked;
        }

        private void __toolLocalPort_KeyUp(object sender, KeyEventArgs e) {

        }

        private void __editlocalIP_KeyDown(object sender, KeyEventArgs e) {

            //if (e.KeyCode == Keys.Enter) {
            //    Server.Address = __editIP.Text;
            //    __localserverDropDown.HideDropDown();
            //}

        }

        private void __editLocalPort_KeyDown(object sender, KeyEventArgs e) {
            //if (e.KeyCode == Keys.Enter) {
            //    try {
            //        Server.Port = Convert.ToInt32(__editLocalPort.Text);
            //        __localserverDropDown.HideDropDown();
            //    } catch (Exception exp) {
            //        MessageBox.Show("Invalid port number", "Error setting port", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //        //log.Error(exp);
            //    }
            //}
        }

        private void lockToolStripMenuItem_Click(object sender, EventArgs e) {
            try {

                SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.Locked = lockToolStripMenuItem.Checked;

            } catch (Exception exp) {
                log.Error(exp);

            }
        }



        private void __toolSaveproj_Click(object sender, EventArgs e) {
            try {
                SaveCurrentConfiguration();
                if (SelectedProject != null) {
                    if (SelectedProject.SelectedRequest != null) {
                        if (SelectedProject.SelectedRequest.SelectedInspection != null) {
                            if (SelectedProject.SelectedRequest.SelectedInspection.CaptureSource != null) {
                                if (SelectedProject.SelectedRequest.SelectedInspection.CaptureSource is PythonRemoteCapture) {
                                    PythonRemoteCapture cap = (PythonRemoteCapture)SelectedProject.SelectedRequest.SelectedInspection.CaptureSource;
                                    if (cap.Cliente.State == TCPClientConnection.ConnectionState.Connected) {
                                        SendProjectToRemote(cap.Cliente, true);
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e) {

            _ProjectOptionsForm.Show();
        }

        private void __toolExit_Click(object sender, EventArgs e) {
            CloseCurrentConfiguration(false);
            this.Close();
        }

        private void __toolCloseProj_Click(object sender, EventArgs e) {
            CloseCurrentConfiguration(false);
        }

        private void __btundo_Click(object sender, EventArgs e) {

        }

        private void __btredo_Click(object sender, EventArgs e) {

        }

        private void __toolStart_Click(object sender, EventArgs e) {

        }

        private void btpressedtimer_Tick(object sender, EventArgs e) {
            //btpressedtimer.Enabled = false;
            if (bt == null) return;
            Thread.Sleep(200);
            Rectangle shape = SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.ROIShape.ShapeEfectiveBounds;

            if (bt.Name == "__btDOWN") {
                if (ArrowPad.__checkedsize.Checked) {
                    shape.Height++;
                }
                else {
                    shape.Y++;
                }
            }
            else if (bt.Name == "__btUP") {
                if (ArrowPad.__checkedsize.Checked) {
                    shape.Height--;
                }
                else {
                    shape.Y--;
                }
            }
            else if (bt.Name == "__btLEFT") {
                if (ArrowPad.__checkedsize.Checked) {
                    shape.Width--;
                }
                else {
                    shape.X--;
                }
            }
            else if (bt.Name == "__btRIGHT") {
                if (ArrowPad.__checkedsize.Checked) {
                    shape.Width++;
                }
                else {
                    shape.X++;
                }
            }
            SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.ROIShape.ShapeEfectiveBounds = shape;
        }

        private void __toolConfig_Click(object sender, EventArgs e) {
            try {
                _ConfigurationsForm.ReloadFile = "";
                _ConfigurationsForm.ShowDialog();
                if (_ConfigurationsForm.ReloadFile!="") {
                    DialogResult dlgResult = MessageBox.Show(this.GetResourceText("Load_new_project_file") + " : " + Path.GetFileName(_ConfigurationsForm.ReloadFile) + "?", this.GetResourceText("Confirm_option"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dlgResult == System.Windows.Forms.DialogResult.Yes) {
                        LoadProjectsFromFile(_ConfigurationsForm.ReloadFile);
                    }
                }
            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        private void requestToolStripMenuItem_Click(object sender, EventArgs e) {
            
        }

        private void captureProcessToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!__btInspect.Enabled) {
                return;
            }
            try {

                SelectedProject.SelectedRequest.ProcessRequest(null, true, null);

            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        private void processToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (!__btInspect.Enabled) {
                return;
            }
            try {

                SelectedProject.SelectedRequest.ProcessRequest(null, false, null);

            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        private void captureAndProcessToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!__btInspect.Enabled) {
                return;
            }
            try {

                SelectedProject.SelectedRequest.SelectedInspection.Execute(null,true);

            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        private void processToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!__btInspect.Enabled) {
                return;
            }
            try {

                if (SelectedProject.SelectedRequest.SelectedInspection.CaptureSource is PythonRemoteCapture)
                {
                    //SelectedProject.SelectedRequest.SelectedInspection.Execute(SelectedProject.SelectedRequest.SelectedInspection.CaptureSource.GetImage(SelectedProject.SelectedRequest.SelectedInspection.InspPos, false));
                }
                else
                {
                    SelectedProject.SelectedRequest.SelectedInspection.Execute(null, false);
                }
                

            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        private void __btLogin_Click(object sender, EventArgs e) {
            

        }


       

        private void __dropLogin_Click(object sender, EventArgs e) {
            
        }

        private void __editPass_Enter(object sender, EventArgs e) {
            if (true) {
                
            }
        }

        private void __VisionMenuBar_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }




        

        public bool SetAdmin { get; set; }

        private Boolean restartapp(LanguageName lang) {
            try {

                String cap = this.GetResourceText("MessageBox_Language_caption");
                String text = this.GetResourceText("MessageBox_Language_text");
                //String text = res_man.GetString("", Thread.CurrentThread.CurrentUICulture);
                if (MessageBox.Show(text, cap, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK) {
                    StaticObjects.Language = lang;
                    //TODO 
                    //Restart Module
                    this.Close();
                }
            }
            catch (Exception exp) {

                log.Error(exp);
            }
            return false;
        }

       
    }

    internal static class SplashScreen {
        static SplashScreenForm sf = null;

        static Point _WindowLocation = new Point(0, 0);

        internal static Point WindowLocation {
            get {
                return SplashScreen._WindowLocation;
            }
            set {

                SplashScreen._WindowLocation = value;
                if (sf != null) {
                    sf.UpdateLocation(value);

                }


            }
        }

        /// <summary>
        /// Displays the splashscreen
        /// </summary>
        internal static void ShowSplashScreen() {
            if (sf == null) {
                sf = new SplashScreenForm();
                sf.Load += new EventHandler(sf_Load);
                sf.ShowSplashScreen();



            }
        }

        static void sf_Load(object sender, EventArgs e) {
            WindowLocation = sf.Location;
        }

        /// <summary>
        /// Closes the SplashScreen
        /// </summary>
        internal static void CloseSplashScreen() {
            if (sf != null) {
                sf.CloseSplashScreen();
                sf = null;
            }
        }

        /// <summary>
        /// Update text in default green color of success message
        /// </summary>
        /// <param name="Text">Message</param>
        internal static void UdpateStatusText(string Text) {
            if (sf != null)
                sf.UdpateStatusText(Text);

        }

        /// <summary>
        /// Update text with message color defined as green/yellow/red/ for success/warning/failure
        /// </summary>
        /// <param name="Text">Message</param>
        /// <param name="tom">Type of Message</param>
        internal static void UdpateStatusTextWithStatus(string Text, TypeOfMessage tom) {

            if (sf != null)
                sf.UdpateStatusTextWithStatus(Text, tom);
        }
    }


}
