using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using KPP.Controls.Winforms.ImageEditorObjs;
using KPP.Controls.Winforms;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;
using TIS.Imaging;
using System.Xml.Serialization;
using KPP.Core.Debug;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Threading;
using DejaVu;
using Emgu.CV.CvEnum;
using System.Net.Sockets;
using System.Drawing;
using IOModule;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Reflection;
using uEye.Types;
using VisionModule.Forms;
using KPPAutomationCore;
namespace VisionModule {


    #region Costum Poperty editors


   
    public class InputSourceSelector : System.Drawing.Design.UITypeEditor {
        // this is a container for strings, which can be 
        // picked-out
        ListBox Box1 = new ListBox();
        IWindowsFormsEditorService edSvc;
        // this is a string array for drop-down list
        //internal static List<CameraInfo> RemoteCameras = new List<CameraInfo>();

        internal static BaseCapture CamSource;

        

        public InputSourceSelector() {
            Box1.BorderStyle = BorderStyle.None;
            // add event handler for drop-down box when item 
            // will be selected
            Box1.Click += new EventHandler(Box1_Click);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
            Box1.Items.Clear();

            // TODO dynamic captures selection

            String[] captures = new String[] { "File", "Inspection", " ICSCamera" };
            Box1.Items.AddRange(captures);
            //Box1.Items.Add(new CameraInfo("Select File", CameraInfo.CameraTypes.File));
            Box1.Height = Box1.PreferredHeight;

            //if (context.Instance is RemoteCameraCapture) {
            //    CamSource = (BaseCapture)context.Instance;
            //    Box1.Items.Remove((StaticObjects.CaptureSources.Find(cap => cap.GetType() == typeof(RemoteCameraCapture))));
            //}
            //else {
                CamSource = (BaseCapture)((Inspection)context.Instance).CaptureSource;
           // }
            

            //if (!StaticObjects.CaptureSources.Contains(CamSource)) {
            //    StaticObjects.CaptureSources.Add(CamSource);
            //}
            


            // window.
            edSvc =
               (IWindowsFormsEditorService)provider.
               GetService(typeof
               (IWindowsFormsEditorService));
            
            if (edSvc != null) {

                

                edSvc.DropDownControl(Box1);
                if (Box1.SelectedItem == null) {                    
                    return value;
                }
                else {
                    String caminfo = (String)Box1.SelectedItem;

                    if (caminfo =="File") {


                            FileCapture newfilecap = new FileCapture("New file location",((Inspection)context.Instance).SelectedProject);

                            return newfilecap;

                      
                    }
                    else if (caminfo =="Inspection") {


                        InspectionCapture newfilecap = new InspectionCapture(((Inspection)context.Instance).SelectedProject);
                            //newfilecap.OnCaptureInspectionNameChanged += new InspectionCapture.CaptureInspectionNameChanged(newInspcap_OnCaptureInspectionNameChanged); ;

                            return newfilecap;

                      
                    } 
                    else if (caminfo =="Inspection") {
                        
                         return new ICSCameraCapture(((Inspection)context.Instance).SelectedProject);
                         
                    }
                    else if (caminfo == "CVCamera") {
                        return new CVCameraCapture(((Inspection)context.Instance).SelectedProject);                                                 
                    }
                    //else if (caminfo is PythonRemoteCapture) {
                    //    if (CamSource != null) {
                    //        return (BaseCapture)Box1.SelectedItem;
                    //    }
                    //    else {
                    //        return new PythonRemoteCapture(caminfo.SelectedProject);
                    //    }
                    //}
                    else if (caminfo=="DirectShowCamera") {
                        return new DirectShowCameraCapture(((Inspection)context.Instance).SelectedProject);                        
                    } else if (caminfo == "uEyeCamera") {

                        return new uEyeCamera(((Inspection)context.Instance).SelectedProject);
                        
                    }

                    
                    else {
                        return value;
                    }
                }

            }
            return value;
        }

      

        private void Box1_Click(object sender, EventArgs e) {

            edSvc.CloseDropDown();
        }
    }




    public class DirectShowSelector : System.Drawing.Design.UITypeEditor {
        // this is a container for strings, which can be 
        // picked-out
        ListBox Box1 = new ListBox();
        IWindowsFormsEditorService edSvc;
        // this is a string array for drop-down list
        //internal static List<CameraInfo> RemoteCameras = new List<CameraInfo>();



        public DirectShowSelector() {
            Box1.BorderStyle = BorderStyle.None;
            // add event handler for drop-down box when item 
            // will be selected
            Box1.Click += new EventHandler(Box1_Click);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) {

            if (!(context.Instance is DirectShowCameraCapture)) {
                return "";
            }

            DirectShowCameraCapture DirectShowCamSource = (DirectShowCameraCapture)(context.Instance);


            Box1.Items.Clear();


            foreach (FilterInfo item in DirectShowCameraCapture.DevicesAvaible) {
                Box1.Items.Add(item.Name);
            }


            Box1.Height = Box1.PreferredHeight;


            // window.
            edSvc =
               (IWindowsFormsEditorService)provider.
               GetService(typeof
               (IWindowsFormsEditorService));

            if (edSvc != null) {


                edSvc.DropDownControl(Box1);
                if (Box1.SelectedItem == null) {
                    return DirectShowCamSource.CameraName;
                }
                else {
                    return Box1.SelectedItem;
                }

            }
            return value;
        }



        private void Box1_Click(object sender, EventArgs e) {

            edSvc.CloseDropDown();
        }
    }


    public class ZoneSelector : System.Drawing.Design.UITypeEditor {
        // this is a container for strings, which can be 
        // picked-out
        //ListBox Box1 = new ListBox();
        IWindowsFormsEditorService edSvc;
        // this is a string array for drop-down list
        //internal static List<CameraInfo> RemoteCameras = new List<CameraInfo>();



        public ZoneSelector() {


        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) {

            
            if (!(context.Instance is InspectionCapture)) {
                return value;
            }

            InspectionCapture inspcap = (InspectionCapture)(context.Instance);

            //inspcap.InspectionSource.

            ZoneSelectorForm zoneform = new ZoneSelectorForm(inspcap.SelectedProject);

            // window.
            edSvc =
               (IWindowsFormsEditorService)provider.
               GetService(typeof
               (IWindowsFormsEditorService));

            if (edSvc != null) {
                zoneform.__image.Image = inspcap.GetImage(true).ToBitmap();
                zoneform.__numericCol.Value = inspcap.SelectedProject.SelectedRequest.ImageCol;
                zoneform.__numericLines.Value = inspcap.SelectedProject.SelectedRequest.ImageLine;
                zoneform.SelectedZone = inspcap.CaptureZone;
                
                if (edSvc.ShowDialog(zoneform) == DialogResult.OK) {
                    return zoneform.SelectedZone;
                }


            }
            return value;
        }



        private void Box1_Click(object sender, EventArgs e) {

            edSvc.CloseDropDown();
        }
    }


    public class ICSCameraSelector : System.Drawing.Design.UITypeEditor {
        // this is a container for strings, which can be 
        // picked-out
        ListBox Box1 = new ListBox();
        IWindowsFormsEditorService edSvc;
        // this is a string array for drop-down list
        //internal static List<CameraInfo> RemoteCameras = new List<CameraInfo>();



        public ICSCameraSelector() {
            Box1.BorderStyle = BorderStyle.None;
            // add event handler for drop-down box when item 
            // will be selected
            Box1.Click += new EventHandler(Box1_Click);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) {

            if (!(context.Instance is ICSCameraCapture)) {
                return "";
            }

            ICSCameraCapture ICSCamSource = (ICSCameraCapture)(context.Instance);

            
            Box1.Items.Clear();


            foreach (ICScamera item in ICSCameraInterface.ICSCameras) {
                    Box1.Items.Add(item.Name);
            }
            
            
            Box1.Height = Box1.PreferredHeight;


            // window.
            edSvc =
               (IWindowsFormsEditorService)provider.
               GetService(typeof
               (IWindowsFormsEditorService));

            if (edSvc != null) {


                edSvc.DropDownControl(Box1);
                if (Box1.SelectedItem == null) {
                    return ICSCamSource.CameraName;
                } else {
                    return Box1.SelectedItem;
                }

            }
            return value;
        }



        private void Box1_Click(object sender, EventArgs e) {

            edSvc.CloseDropDown();
        }
    }

    public class MyFileNameEditor : FileNameEditor {

        protected override void InitializeDialog(System.Windows.Forms.OpenFileDialog openFileDialog) {
            base.InitializeDialog(openFileDialog);
            openFileDialog.Filter = "Image Files | *.bmp;*.jpeg;*.jpg;*.tiff";
            openFileDialog.Title = "Select image File";
        }
    }


    public class InspNameSelector : System.Drawing.Design.UITypeEditor {

        ListBox Box1 = new ListBox();
        IWindowsFormsEditorService edSvc;

        //void Update_result_requests(String name, int id) {
        //    StaticObjects.SelectedProject.RequestList.ForEach(req => req.Inspections.ForEach(insp => insp.Results.ForEach(resinf => resinf.AddOverrideID(name, id))));
        //}


        public InspNameSelector() {
            Box1.BorderStyle = BorderStyle.None;
            // add event handler for drop-down box when item 
            // will be selected
            Box1.Click += new EventHandler(Box1_Click);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) {


            BaseCapture selectedcap = (BaseCapture)context.Instance;


            Box1.Items.Clear();

            foreach (Request req in selectedcap.SelectedProject.RequestList) {
                foreach (Inspection insp in req.Inspections) {
                    if (insp.RequestID != selectedcap.SelectedProject.SelectedRequest.ID) {
                        Box1.Items.Add(insp);
                    } else {
                        if (insp.Name != selectedcap.SelectedProject.SelectedRequest.SelectedInspection.Name) {
                            Box1.Items.Add(insp);
                        }
                    }
                    
                    
                }
            }
            
            

            Box1.Height = Box1.PreferredHeight;


            // window.
            edSvc =
               (IWindowsFormsEditorService)provider.
               GetService(typeof
               (IWindowsFormsEditorService));

            if (edSvc != null) {


                edSvc.DropDownControl(Box1);
                if (Box1.SelectedItem == null) {



                    return value;
                }
                else {
                    Inspection ins = Box1.SelectedItem as Inspection;

                    if (ins != null) {
                        ins.CaptureSource.SelectedProject.SelectedRequest.SelectedInspection.ROIList.AuxROIS = ins.ROIList.ToList();
                    }
                    

                    return (Box1.SelectedItem);
                }

            }
            return value;
        }

        private void Box1_Click(object sender, EventArgs e) {

            edSvc.CloseDropDown();
        }
    }

  
    public class InterfaceValueSelector : System.Drawing.Design.UITypeEditor {

        KPPLogger log = new KPPLogger(typeof(InterfaceValueSelector));

        // this is a container for strings, which can be 
        // picked-out
        ListBox Box1 = new ListBox();
        IWindowsFormsEditorService edSvc;
        // this is a string array for drop-down list


        internal static ICSCameraCapture SelectedCamera = null;

        public InterfaceValueSelector() {
            Box1.BorderStyle = BorderStyle.None;
            // add event handler for drop-down box when item 
            // will be selected
            Box1.Click += new EventHandler(Box1_Click);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) {

            try {
                Box1.Items.Clear();

                //Box1.Height = Box1.PreferredHeight;
                SelectedCamera = (ICSCameraCapture)context.Instance;


                if (SelectedCamera==null) {
                    return value;
                }

                VCDRangeProperty selectedinterface = SelectedCamera._ICSCamera.Zoominterface;

                object returnval = -1;
                if (context.PropertyDescriptor.DisplayName == "Zoom") {
                    if (SelectedCamera._ICSCamera.Zoominterface == null ) {
                        return value;
                    }
                    selectedinterface = SelectedCamera._ICSCamera.Zoominterface;
                    Box1.Items.Add(selectedinterface.Value);
                    returnval = SelectedCamera.Zoom;
                }
                else if (context.PropertyDescriptor.DisplayName == "Exposure") {
                    if (SelectedCamera._ICSCamera.Exposureinterface == null) {
                        return value;
                    }
                    Box1.Items.Add(SelectedCamera.GetIntValue());
                    returnval = SelectedCamera.Exposure;
                }
                else if (context.PropertyDescriptor.DisplayName == "Focus") {
                    if (SelectedCamera._ICSCamera.Focusinterface == null) {
                        return value;
                    }
                    selectedinterface = SelectedCamera._ICSCamera.Focusinterface;
                    Box1.Items.Add(selectedinterface.Value);
                    returnval = SelectedCamera.Focus;
                }
                else if (context.PropertyDescriptor.DisplayName == "Iris") {
                    if (SelectedCamera._ICSCamera.Irisinterface == null) {
                        return value;
                    }
                    selectedinterface = SelectedCamera._ICSCamera.Irisinterface;
                    Box1.Items.Add(selectedinterface.Value);
                    returnval = SelectedCamera.Iris;
                }
                else if (context.PropertyDescriptor.DisplayName == "Gain") {
                    if (SelectedCamera._ICSCamera.Gaininterface== null) {
                        return value;
                    }
                    selectedinterface = SelectedCamera._ICSCamera.Gaininterface;
                    Box1.Items.Add(selectedinterface.Value);
                    returnval = SelectedCamera.Gain;
                } else if (context.PropertyDescriptor.DisplayName == "Blue Balance") {
                    if (SelectedCamera._ICSCamera.Bluebalanceinterface== null) {
                        return value;
                    }
                    selectedinterface = SelectedCamera._ICSCamera.Bluebalanceinterface;
                    Box1.Items.Add(selectedinterface.Value);
                    returnval = SelectedCamera.BlueBalance;
                }
                else if (context.PropertyDescriptor.DisplayName == "Green Balance") {
                    if (SelectedCamera._ICSCamera.Greenbalanceinterface== null) {
                        return value;
                    }
                    selectedinterface = SelectedCamera._ICSCamera.Greenbalanceinterface;
                    Box1.Items.Add(selectedinterface.Value);
                    returnval = SelectedCamera.GreenBalance;
                }
                else if (context.PropertyDescriptor.DisplayName == "Red Balance") {
                    if (SelectedCamera._ICSCamera.Redbalanceinterface == null) {
                        return value;
                    }
                    selectedinterface = SelectedCamera._ICSCamera.Redbalanceinterface;
                    Box1.Items.Add(selectedinterface.Value);
                    returnval = SelectedCamera.RedBalance;
                } else if (context.PropertyDescriptor.DisplayName == "PixelFormat") {
                    Box1.Items.AddRange(SelectedCamera._ICSCamera.Camera.VideoFormats.Select(format => format.Name).ToArray());
                    Box1.SelectedItem = SelectedCamera._ICSCamera.Camera.VideoFormatCurrent.Name;
                    returnval = SelectedCamera._ICSCamera.Camera.VideoFormatCurrent.Name;
                }


                edSvc =
                   (IWindowsFormsEditorService)provider.
                   GetService(typeof
                   (IWindowsFormsEditorService));
                if (edSvc != null) {
                    edSvc.DropDownControl(Box1);
                    if (Box1.SelectedItem == null) {
                        return returnval;
                    }

                    return Box1.SelectedItem;

                }
                return value;
            } catch (Exception exp) {
                log.Error(exp);

                return value;
                
            }
        }

        private void Box1_Click(object sender, EventArgs e) {
            //if (Box1.SelectedItem !=null) {
            //    if (SelectedInsp!=null) {
            //        SelectedInsp.CaptureSource = (CameraCapture)Box1.SelectedItem;   
            //    }
            //}
            edSvc.CloseDropDown();
        }
    }
    
    //public class uEyeCamInfo {

    //    private uEye.Types.CameraInformation _Info= new uEye.Types.CameraInformation();
    //    [XmlIgnore,Browsable(false)]
    //    public uEye.Types.CameraInformation Info {
    //        get { return _Info; }
    //        private set { 
    //            _Info = value; 
                
    //        }
    //    }

    //    [XmlIgnore,DisplayName("Model")]
    //    public String Model {
    //        get {
    //            String ret = Info.Model;
    //            if (ret!=null) {
    //                ret = ret.Replace("\0", "");
    //            }
                
    //            return ret;
    //        }

    //    }



    //    [XmlIgnore, DisplayName("Serial")]
    //    public String Serial {
    //        get {
    //            return Info.SerialNumber;
                
    //        }

    //    }

    //    public uEyeCamInfo(uEye.Types.CameraInformation info) {
    //        Info = info;
    //    }

    //    public uEyeCamInfo() {

           
    //    }

    //    public override string ToString() {
    //        if (Info.CameraID<1) {
    //            return "Camera not found";
    //        }
    //        return Model+"("+Serial+")";
           
    //    }
    //}

    public class uEyeCameraSelector : System.Drawing.Design.UITypeEditor {
        // this is a container for strings, which can be 
        // picked-out
        ListBox Box1 = new ListBox();
        IWindowsFormsEditorService edSvc;
        // this is a string array for drop-down list
        //internal static List<CameraInfo> RemoteCameras = new List<CameraInfo>();



        public uEyeCameraSelector() {
            Box1.BorderStyle = BorderStyle.None;
            // add event handler for drop-down box when item 
            // will be selected
            Box1.Click += new EventHandler(Box1_Click);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) {

            if (!(context.Instance is uEyeCamera)) {
                return "";
            }

            uEyeCamera uEyeCamSource = (uEyeCamera)(context.Instance);


            Box1.Items.Clear();


            
            uEye.Types.CameraInformation[] cameraList;
            uEye.Info.Camera.GetCameraList(out cameraList);

            foreach (uEye.Types.CameraInformation info in cameraList)
            {

                Box1.Items.Add(info.Model.Replace("\0", "") + "#" + info.SerialNumber.Replace("\0", ""));
            }                      

            Box1.Height = Box1.PreferredHeight;


            // window.
            edSvc =
               (IWindowsFormsEditorService)provider.
               GetService(typeof
               (IWindowsFormsEditorService));

            if (edSvc != null) {


                edSvc.DropDownControl(Box1);
                if (Box1.SelectedItem == null) {                   
                    return value;
                } else {
                    return Box1.SelectedItem;
                }

            }
            return value;
        }



        private void Box1_Click(object sender, EventArgs e) {

            edSvc.CloseDropDown();
        }
    }


    public class uEyeCameraConfigSelector : System.Drawing.Design.UITypeEditor {
        // this is a container for strings, which can be 
        // picked-out
        //ListBox Box1 = new ListBox();
        IWindowsFormsEditorService edSvc;
        // this is a string array for drop-down list
        //internal static List<CameraInfo> RemoteCameras = new List<CameraInfo>();



        public uEyeCameraConfigSelector() {


        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) {

            if (!(context.Instance is uEyeCamera)) {
                return value;
            }

            uEyeCamera uEyeCamSource = (uEyeCamera)(context.Instance);



            CameraControlForm camctr = new CameraControlForm(uEyeCamSource.Camera);
            
            // window.
            edSvc =
               (IWindowsFormsEditorService)provider.
               GetService(typeof
               (IWindowsFormsEditorService));

            if (edSvc != null) {


                if (edSvc.ShowDialog(camctr) == DialogResult.OK) {


                    if (context.PropertyDescriptor.DisplayName=="Exposure") {
                        double outval;
                        uEyeCamSource.Camera.Timing.Exposure.Get(out outval);
                        return outval;
                    } else if (context.PropertyDescriptor.DisplayName == "PixelClock") {
                        int outval;
                        uEyeCamSource.Camera.Timing.PixelClock.Get(out outval);
                        return outval;
                    } else if (context.PropertyDescriptor.DisplayName == "Framerate") {
                        double outval;
                        uEyeCamSource.Camera.Timing.Framerate.Get(out outval);
                        return outval;
                    }
                 
                }


            }
            return value;
        }



        private void Box1_Click(object sender, EventArgs e) {

            edSvc.CloseDropDown();
        }
    }


    #endregion


    public class uEyeCamera : BaseCapture {



        KPPLogger log = new KPPLogger(typeof(uEyeCamera));

        readonly UndoRedo<int> _PixelClock = new UndoRedo<int>(-1);
        [XmlAttribute, ReadOnly(false)]
        [EditorAttribute(typeof(uEyeCameraConfigSelector), typeof(UITypeEditor))]
        public int PixelClock {
            get {
                
                return _PixelClock.Value;
            }
            set {
                if (_PixelClock.Value != value) {

                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start(this.CameraName + " Pixel Clock changed to:" + value)) {

                            if (Camera != null) {
                                uEye.Types.Range<int> range;
                                Camera.Timing.PixelClock.GetRange(out range);
                                if (value > range.Minimum && value < range.Maximum) {
                                    Camera.Timing.PixelClock.Set(value);
                                    _PixelClock.Value = value;
                                }

                            }

                            UndoRedoManager.Commit();
                        }
                    } else {

                        _PixelClock.Value = value;
                    }

                }
            }
        }

        readonly UndoRedo<double> _Framerate = new UndoRedo<double>(-1);
        [XmlAttribute, ReadOnly(false)]
        [EditorAttribute(typeof(uEyeCameraConfigSelector), typeof(UITypeEditor))]
        public double Framerate {
            get {
                return Math.Round(_Framerate.Value,3);
            }
            set {
                if (_Framerate.Value != value) {

                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start(this.CameraName + " Framerate changed to:" + value)) {

                            if (Camera != null) {
                                uEye.Types.Range<double> range;
                                Camera.Timing.Framerate.GetFrameRateRange(out range);
                                if (value > range.Minimum && value < range.Maximum) {
                                    Camera.Timing.Framerate.Set(value);
                                    _Framerate.Value = value;
                                }

                            }

                            UndoRedoManager.Commit();
                        }
                    } else {

                        _Framerate.Value = value;
                    }

                }
            }
        }

        readonly UndoRedo<Rectangle> _AOI = new UndoRedo<Rectangle>(Rectangle.Empty);
        [ReadOnly(false)]
        //[EditorAttribute(typeof(uEyeCameraConfigSelector), typeof(UITypeEditor))]
        public Rectangle AOI {
            get {
                return _AOI.Value;
            }
            set {
                if (_AOI.Value != value) {

                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start(this.CameraName + " AOI changed to:" + value)) {
                            
                            if (Camera != null && value!=null) {
                                uEye.Types.Range<Int32> rangex, rangey, rangew, rangeh;                                
                                
                                Camera.Size.AOI.GetPosRange(out rangex,out rangey);
                                Camera.Size.AOI.GetSizeRange(out rangew, out rangeh);

                                value.X = value.X.NextEven();
                                value.Y = value.Y.NextEven();
                                value.Width = value.Width.RoundUp(4);
                                value.Height = value.Height.NextEven();


                                if (true ||  value.X > rangex.Minimum && value.X < rangex.Maximum 
                                    && value.Y > rangey.Minimum && value.Y < rangey.Maximum
                                    && value.Width > rangew.Minimum && value.Width < rangew.Maximum
                                    && value.Height > rangeh.Minimum && value.Height < rangeh.Maximum) {
                                    uEye.Defines.Status statusRet = uEye.Defines.Status.NO_SUCCESS;
                                    if (value.Width<=4 || value.Height<=2) {
                                        value = new Rectangle(0, 0, _FormatSize.Width, _FormatSize.Height);
                                    }
                                    statusRet = Camera.Size.AOI.Set(value);
                                    if (statusRet != uEye.Defines.Status.SUCCESS) {

                                        //Camera = null;
                                        throw new Exception("Error setting AOI");

                                    }
                                    _AOI.Value = value;
                                }
                                
                            }

                            UndoRedoManager.Commit();
                        }
                    } else {

                        _AOI.Value = value;
                    }

                } 
            }
        }

        readonly UndoRedo<double> _exposure = new UndoRedo<double>(-1);
        [XmlAttribute, ReadOnly(false)]
        [EditorAttribute(typeof(uEyeCameraConfigSelector), typeof(UITypeEditor))]
        public double Exposure {
            get {
                return Math.Round(_exposure.Value, 2);
            }
            set {
                if (_exposure.Value != value) {

                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start(this.CameraName + " Exposure changed to:" + value)) {

                            if (Camera != null) {
                                uEye.Types.Range<Double> range;
                                Camera.Timing.Exposure.GetRange(out range);
                                if (value > range.Minimum && value < range.Maximum) {
                                    Camera.Timing.Exposure.Set(value);
                                    _exposure.Value = value;
                                }

                            }

                            UndoRedoManager.Commit();
                        }
                    } else {

                        _exposure.Value = value;
                    }

                }
            }
        }

        private Size<int> _FormatSize = new Size<int>(0, 0);

        private Boolean SetCamera(String name) {
            try {

                if (name.Contains("#") == false) {
                    return false;
                }
                String SerialNumber = name.Split(new String[] { "#" }, StringSplitOptions.None)[1];

                uEye.Types.CameraInformation[] cameraList;
                uEye.Info.Camera.GetCameraList(out cameraList);
                uEye.Types.CameraInformation SelectedCameraInfo = cameraList.ToList().Find(byserial => byserial.SerialNumber.Replace("\0", "") == SerialNumber);


                uEye.Camera _SelectedCamera = new uEye.Camera();

                if (SelectedCameraInfo.CameraID <= 0) {
                    return false;
                }

                Int32 id = Convert.ToInt32(SelectedCameraInfo.CameraID);
                uEye.Defines.Status statusRet = uEye.Defines.Status.NO_SUCCESS;
                statusRet = _SelectedCamera.Init(id);



                if (statusRet != uEye.Defines.Status.SUCCESS) {

                    throw new Exception("Error initializing camera");


                }
                Camera = _SelectedCamera;
                statusRet = Camera.Memory.Allocate();

                if (statusRet != uEye.Defines.Status.SUCCESS) {

                    throw new Exception("Error allocating memory");

                }
                Camera.Acquisition.Stop();

                Camera.AutoFeatures.Software.SetEnableAutoWhitebalanceOnce(false);
                Camera.AutoFeatures.Software.SetEnableAutoWhiteBalance(false);
                Camera.AutoFeatures.Software.SetEnableAutoShutter(false);
                Camera.AutoFeatures.Software.SetEnableAutoGain(false);
                Camera.AutoFeatures.Software.SetEnableAutoFramerate(false);
                Camera.AutoFeatures.Software.SetEnableAutoBrightnessOnce(false);

                Camera.Timing.PixelClock.Set(PixelClock);
                Camera.Timing.Framerate.Set(Framerate);
                Camera.Timing.Exposure.Set(Exposure);

                Camera.EventFrame += new EventHandler(Camera_EventFrame);

                ImageFormatInfo[] FormatInfoList;
                Camera.Size.ImageFormat.GetList(out FormatInfoList);

                //int maxsize=0;
                foreach (ImageFormatInfo item in FormatInfoList) {
                    if (item.Size.Width > _FormatSize.Width) {
                        _FormatSize.Width = item.Size.Width;
                        _FormatSize.Height = item.Size.Height;
                    }
                }
                if (!AOI.IsEmpty) {
                    statusRet = Camera.Size.AOI.Set(AOI);
                    if (statusRet != uEye.Defines.Status.SUCCESS) {

                        //Camera = null;
                        throw new Exception("Error setting AOI");

                    }
                }

            }
            catch (DllNotFoundException exp) {

                log.Error(exp);
            }

            return true;

        }

        void Camera_EventFrame(object sender, EventArgs e) {
            waitimage.Set();
        }



        String _CameraName = "No camera selected";
        [EditorAttribute(typeof(uEyeCameraSelector), typeof(UITypeEditor))]
        [XmlAttribute, DisplayName("Camera Name"), ReadOnly(false),Browsable(true)]        
        public override String CameraName {
            get {

                return _CameraName;
            }

            set {

                if (_CameraName!=value) {
                    _CameraName = value;

                    SetCamera(value);
                }
            }
            
        }

        

     

        uEye.Camera _Camera = null;
        [XmlIgnore]
        [Browsable(false)]
        public uEye.Camera Camera {
            get {
                return _Camera;
            }
            set {
                if (_Camera != value) {
                    _Camera = value;
                }
            }
        }


        ManualResetEvent waitimage = new ManualResetEvent(true);
        private object lockobject = new object();

        public override Image<Bgr, Byte> GetImage() {

            Image<Bgr, Byte> newimage = null;

            uEye.Defines.Status statusRet = uEye.Defines.Status.NO_SUCCESS;
            waitimage.Reset();

            if (Camera.IsOpened==false) {
                SetCamera(_CameraName);
            }
            Camera.Timing.PixelClock.Set(PixelClock);
            Camera.Timing.Framerate.Set(Framerate);
            Camera.Timing.Exposure.Set(Exposure);

            statusRet = Camera.Acquisition.Freeze();
            //Camera.Acquisition.Freeze(uEye.Defines.DeviceParameter.Wait);
            
            if (statusRet != uEye.Defines.Status.SUCCESS) {

                //Camera = null;
                throw new Exception("Error setting snapshot");

            }
            waitimage.WaitOne();
            uEye.Defines.DisplayMode mode;
            Camera.Display.Mode.Get(out mode);

            // only display in dib mode
            if (mode == uEye.Defines.DisplayMode.DiB) {
                Int32 s32MemID;
                Camera.Memory.GetActive(out s32MemID);
                Camera.Memory.Lock(s32MemID);

                

                Bitmap bitmap = null;
                Camera.Memory.ToBitmap(s32MemID, out bitmap);
                 //Camera.Memory.
                if (bitmap != null) {

                    //DoDrawing(ref graphics, s32MemID);
                    newimage = new Image<Bgr, byte>(bitmap);
                    
                    bitmap.Dispose();
                }

                Camera.Memory.Unlock(s32MemID);

            }



            
            return newimage;
            //Camera.Acquisition.;

            
           
            

        }

        //private int framecounter = 0;
        

        public override string ToString() {
            return "uEye Capture";
        }

        public uEyeCamera(VisionProject selectedproject)
            : base(selectedproject) {

            this.CameraName = "uEye camera input";
            this.Camtype = CameraTypes.uEye;
        }

        public uEyeCamera() {

            this.CameraName = "uEye camera input";
            this.Camtype = CameraTypes.uEye;
        }

        public override void Dispose() {
            if (Camera != null) {                
                //Camera.Stop();
                Camera.Exit();

                Console.WriteLine("Camera Stopped");
            }
        }

    }


    public class ICScamera {
        KPPLogger log = new KPPLogger(typeof(ICScamera));
        String _name;
       

        

        

         VCDRangeProperty _zoominterface;
         public VCDRangeProperty Zoominterface {
            get {
                return _zoominterface;
            }
             private set {
                 if (_zoominterface!=value) {
                     _zoominterface = value;
                     if (_zoominterface==null) {
                        
                     }
                 }
             }
            
        }

         VCDRangeProperty _irisinterface;
         public VCDRangeProperty Irisinterface {
             get {
                 return _irisinterface;
             }
             private set {
                 if (_irisinterface!=value) {
                     _irisinterface = value;
                     if (_irisinterface==null) {
                       
                     }
                 }
             }
         }


         VCDRangeProperty _focusinterface;
         public VCDRangeProperty Focusinterface {
             get {
                 return _focusinterface;
             }
             private set {
                 if (_focusinterface!=value) {
                     _focusinterface = value;
                     if (_focusinterface==null) {
                         

                     }
                 }
             }
         }


         VCDRangeProperty _gaininterface;
         public VCDRangeProperty Gaininterface {
             get {
                 return _gaininterface;
             }

         }



         VCDRangeProperty _redbalanceinterface;
         public VCDRangeProperty Redbalanceinterface {
             get {
                 return _redbalanceinterface;
             }

         }

         VCDRangeProperty _bluebalanceinterface;
         public VCDRangeProperty Bluebalanceinterface {
             get {
                 return _bluebalanceinterface;
             }

         }

         VCDRangeProperty _greenbalanceinterface;
         public VCDRangeProperty Greenbalanceinterface {
             get {
                 return _greenbalanceinterface;
             }

         }


         VCDAbsoluteValueProperty _exposureinterface;
         public VCDAbsoluteValueProperty Exposureinterface {
             get {
                 return _exposureinterface;
             }

         }

         ICImagingControl _Camera;
         public ICImagingControl Camera {
            get { return _Camera; }
            private set {
                if (_Camera != value) {
                    try
                    {

                        if (value != null)
                        {
                            value.LiveStop();
                            value.ImageRingBufferSize = 1;
                            value.ImageAvailableExecutionMode = EventExecutionMode.MultiThreaded;
                            value.LiveStart();
                            Thread.Sleep(1000);
                            value.LiveStop();

                        }
                    }
                    catch (ICException exp)
                    {

                        log.Warn(exp.Message);
                    }

                    _Camera = value;
                }

            }
        }
        public String Name {
            get { return _name; }

        }


    

        public ICScamera(ICImagingControl icsCamera) {
            try {
                _name = icsCamera.Device;
                Camera = icsCamera;
                
                
                
                VCDSwitchProperty _whitebalance = (VCDSwitchProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_Auto, VCDGUIDs.VCDInterface_Switch);
                if (_whitebalance != null) {
                    if (_whitebalance.Switch) {
                        _whitebalance.Switch = false;
                    }
                }


                VCDSwitchProperty _gainautointerface = (VCDSwitchProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_Gain, VCDGUIDs.VCDElement_Auto, VCDGUIDs.VCDInterface_Switch);
                if (_gainautointerface != null) {
                    if (_gainautointerface.Switch) {
                        _gainautointerface.Switch = false;
                    }
                }

                VCDSwitchProperty _irisautointerface = (VCDSwitchProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_Iris, VCDGUIDs.VCDElement_Auto, VCDGUIDs.VCDInterface_Switch);
                if (_irisautointerface != null) {
                    if (_irisautointerface.Switch) {
                        _irisautointerface.Switch = false;
                    }
                }



                Zoominterface= (VCDRangeProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_Zoom, VCDGUIDs.VCDElement_Value, VCDGUIDs.VCDInterface_Range);                
                Irisinterface= (VCDRangeProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_Iris, VCDGUIDs.VCDElement_Value, VCDGUIDs.VCDInterface_Range);
                Focusinterface = (VCDRangeProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_Focus, VCDGUIDs.VCDElement_Value, VCDGUIDs.VCDInterface_Range);


                _gaininterface = (VCDRangeProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_Gain, VCDGUIDs.VCDElement_Value, VCDGUIDs.VCDInterface_Range);                
                _exposureinterface = (VCDAbsoluteValueProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_Exposure, VCDGUIDs.VCDElement_Value, VCDGUIDs.VCDInterface_AbsoluteValue);
                _redbalanceinterface= (VCDRangeProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_WhiteBalanceRed, VCDGUIDs.VCDInterface_Range);
                _bluebalanceinterface = (VCDRangeProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_WhiteBalanceBlue, VCDGUIDs.VCDInterface_Range);
                _greenbalanceinterface= (VCDRangeProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_WhiteBalance, VCDGUIDs.VCDElement_WhiteBalanceGreen, VCDGUIDs.VCDInterface_Range);

                VCDSwitchProperty exposureautointerface = (VCDSwitchProperty)icsCamera.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_Exposure, VCDGUIDs.VCDElement_Auto, VCDGUIDs.VCDInterface_Switch);
                if (exposureautointerface != null) {
                    if (exposureautointerface.Switch) {
                        exposureautointerface.Switch = false;
                    }
                }


            } catch (Exception exp) {

                log.Error(exp);
            }
        }



        void Camera_ImageAvailable(object sender, ICImagingControl.ImageAvailableEventArgs e) {
            
        }

        void _Camera_ImageAvailable(object sender, ICImagingControl.ImageAvailableEventArgs e) {
            //e.ImageBuffer.Bitmap
        }

    }

    internal static class ICSCameraInterface {
        
        internal static List<ICScamera> ICSCameras = new List<ICScamera>();
        
    }


    public class ICSCameraCapture : BaseCapture {

        KPPLogger log = new KPPLogger(typeof(ICSCameraCapture));

        void SetCamera(String value) {
            try {
                ICScamera camera = ICSCameraInterface.ICSCameras.Find(name => name.Name== value);
                if (camera != null) {
                    _ICSCamera = camera;
                    
                    _CameraName.Value = camera.Name;
                    
                    if (camera.Zoominterface != null) {
                        this.ChangeAttributeValue<BrowsableAttribute>("Zoom", "browsable", true);
                        _maxzoom = camera.Zoominterface.RangeMax;
                        _minzoom = camera.Zoominterface.RangeMin;
                    }
                    else {
                        this.ChangeAttributeValue<BrowsableAttribute>("Zoom", "browsable", false);
                    }

                    if (camera.Irisinterface!= null) {
                        this.ChangeAttributeValue<BrowsableAttribute>("Iris", "browsable", true);
                        _maxiris= camera.Irisinterface.RangeMax;
                        _miniris = camera.Irisinterface.RangeMin;
                    }
                    else {
                        this.ChangeAttributeValue<BrowsableAttribute>("Iris", "browsable", false);
                    }

                    if (camera.Gaininterface != null) {
                        _maxgain = camera.Gaininterface.RangeMax;
                        _mingain = camera.Gaininterface.RangeMin;
                    }



                    if (camera.Focusinterface!=null) {
                        this.ChangeAttributeValue<BrowsableAttribute>("Focus", "browsable", true);
                        _maxfocus = camera.Focusinterface.RangeMax;
                        _minfocus = camera.Focusinterface.RangeMin;
                    }
                    else {
                        this.ChangeAttributeValue<BrowsableAttribute>("Focus", "browsable", false);
                    }

                    if (camera.Greenbalanceinterface != null) {
                        _MinGreenBalance = camera.Greenbalanceinterface.RangeMin;
                        _MaxGreenBalance = camera.Greenbalanceinterface.RangeMax;
                        this.ChangeAttributeValue<BrowsableAttribute>("GreenBalance", "browsable", true);                        
                    } else {
                        this.ChangeAttributeValue<BrowsableAttribute>("GreenBalance", "browsable", false);
                    }

                    if (camera.Redbalanceinterface!= null) {
                        _MinRedBalance = camera.Redbalanceinterface.RangeMin;
                        _MaxRedBalance = camera.Redbalanceinterface.RangeMax;
                        this.ChangeAttributeValue<BrowsableAttribute>("RedBalance", "browsable", true);
                    } else {
                        this.ChangeAttributeValue<BrowsableAttribute>("RedBalance", "browsable", false);
                    }

                    if (camera.Bluebalanceinterface!= null) {
                        _MinBlueBalance = camera.Bluebalanceinterface.RangeMin;
                        _MaxBlueBalance = camera.Bluebalanceinterface.RangeMax;
                        this.ChangeAttributeValue<BrowsableAttribute>("BlueBalance", "browsable", true);
                    } else {
                        this.ChangeAttributeValue<BrowsableAttribute>("BlueBalance", "browsable", false);
                    }

                    
                    _maxexposure = 100;
                    _minexposure = 1;
                    base.UpdateAttributes();


                }
            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        

        public int GetIntValue() {
            double rmin = 0;
            double rmax = 0;
            double absval = 0;
            double rangelen = 0;
            double p = 0;

            // Get the property data from the interface
            rmin = _ICSCamera.Exposureinterface.RangeMin;
            rmax = _ICSCamera.Exposureinterface.RangeMax;
            absval = _ICSCamera.Exposureinterface.Value;

            // Do calculation depending of the dimension function of the property
            if (_ICSCamera.Exposureinterface.DimFunction == TIS.Imaging.AbsDimFunction.eAbsDimFunc_Log) {

                rangelen = System.Math.Log(rmax) - System.Math.Log(rmin);
                p = 100 / rangelen * (System.Math.Log(absval) - System.Math.Log(rmin));
            }
            else // AbsValItf.DimFunction = AbsDimFunction.eAbsDimFunc_Linear
            {
                rangelen = rmax - rmin;
                p = 100 / rangelen * (absval - rmin);
            }

            // Round to integer
            return (int)System.Math.Round(p, 0);
        }

        private double GetAbsVal(int Val) {

            double rmin = 0;
            double rmax = 0;
            double rangelen = 0;
            double value = 0;

            // Get the property data from the interface
            rmin = _ICSCamera.Exposureinterface.RangeMin;
            rmax = _ICSCamera.Exposureinterface.RangeMax;

            // Do calculation depending of the dimension function of the property
            if (_ICSCamera.Exposureinterface.DimFunction == TIS.Imaging.AbsDimFunction.eAbsDimFunc_Log) {

                rangelen = System.Math.Log(rmax) - System.Math.Log(rmin);
                value = System.Math.Exp(System.Math.Log(rmin) + rangelen / 100 * Val);

            }
            else // AbsValItf.DimFunction = AbsDimFunction.eAbsDimFunc_Linear
            {

                rangelen = rmax - rmin;
                value = rmin + rangelen / 100 * Val;

            }

            // Correct the value if it is out of bounds
            if (value > rmax) {
                value = rmax;
            }
            if (value < rmin) {
                value = rmin;
            }

            return value;
        }


        public override void UpdateAttributes() {

            if (_ICSCamera==null) {
                return;
            }
            
            if (_ICSCamera.Zoominterface != null) {
                this.ChangeAttributeValue<BrowsableAttribute>("Zoom", "browsable", true);                
            } else {
                this.ChangeAttributeValue<BrowsableAttribute>("Zoom", "browsable", false);
            }

            if (_ICSCamera.Irisinterface != null) {
                this.ChangeAttributeValue<BrowsableAttribute>("Iris", "browsable", true);                
            } else {
                this.ChangeAttributeValue<BrowsableAttribute>("Iris", "browsable", false);
            }

            if (_ICSCamera.Focusinterface != null) {
                this.ChangeAttributeValue<BrowsableAttribute>("Focus", "browsable", true);                
            } else {
                this.ChangeAttributeValue<BrowsableAttribute>("Focus", "browsable", false);
            }

            if (_ICSCamera.Greenbalanceinterface != null) {
                this.ChangeAttributeValue<BrowsableAttribute>("GreenBalance", "browsable", true);
            } else {
                this.ChangeAttributeValue<BrowsableAttribute>("GreenBalance", "browsable", false);
            }

            if (_ICSCamera.Redbalanceinterface != null) {
                this.ChangeAttributeValue<BrowsableAttribute>("RedBalance", "browsable", true);
            } else {
                this.ChangeAttributeValue<BrowsableAttribute>("RedBalance", "browsable", false);
            }

            if (_ICSCamera.Bluebalanceinterface != null) {
                this.ChangeAttributeValue<BrowsableAttribute>("BlueBalance", "browsable", true);
            } else {
                this.ChangeAttributeValue<BrowsableAttribute>("BlueBalance", "browsable", false);
            }

            base.UpdateAttributes();
        }

        readonly UndoRedo<String> _CameraName = new UndoRedo<string>("Capture from IC Imaging Camera");


        [XmlAttribute, DisplayName("Camera Name"), ReadOnly(false), Browsable(true)]
        [EditorAttribute(typeof(ICSCameraSelector), typeof(UITypeEditor))]
        public override String CameraName {
            get {
                return _CameraName.Value;
            }
            set {

                if (_CameraName.Value!=value) {

                    if (!UndoRedoManager.IsCommandStarted) {

                        using (UndoRedoManager.Start(this.CameraName + " Camera changed to: "+value)) {
                            SetCamera(value);
                            UndoRedoManager.Commit();
                        }
                    } else {
                        SetCamera(value);
                    }
                }
            }
        }

        [XmlIgnore]
        internal ICScamera _ICSCamera;

       


        int _minzoom = 0;
        int _maxzoom = 15;

        int _miniris = 0;
        int _maxiris = 4000;

        int _mingain = 0;
        int _maxgain = 4000;

        int _MinRedBalance = 0;
        int _MaxRedBalance = 4000;

        int _MinGreenBalance = 0;
        int _MaxGreenBalance = 4000;

        int _MinBlueBalance = 0;
        int _MaxBlueBalance = 4000;

        int _minfocus = 0;
        int _maxfocus = 350;
        double _minexposure = 1;
        double _maxexposure = 4000;

        readonly UndoRedo<int> _iris = new UndoRedo<int>();
        [XmlAttribute]
        [EditorAttribute(typeof(InterfaceValueSelector), typeof(System.Drawing.Design.UITypeEditor)),Browsable(true)]
        public int Iris {
            get {

                return _iris.Value;
            }
            set {
                if (_iris.Value != value) {

                    if (value >= _miniris && value <= _maxiris) {

                        
                            if (!UndoRedoManager.IsCommandStarted) {
                                using (UndoRedoManager.Start(this.CameraName + " Iris changed to:" + value)) {
                                    _iris.Value = value;
                                    UndoRedoManager.Commit();
                                }


                            } else {

                                _iris.Value = value;

                            }
                        
                    }
                }
            }
        }

        readonly UndoRedo<int> _gain = new UndoRedo<int>();
        [XmlAttribute]
        [EditorAttribute(typeof(InterfaceValueSelector), typeof(System.Drawing.Design.UITypeEditor))]
        public int Gain {
            get {

                return _gain.Value;
            }
            set {
                if (_gain.Value != value) {

                    if (value >= _mingain && value <= _maxgain) {


                        if (!UndoRedoManager.IsCommandStarted) {
                            using (UndoRedoManager.Start(this.CameraName + " Gain changed to:" + value)) {
                                _gain.Value = value;
                                UndoRedoManager.Commit();
                            }


                        } else {

                            _gain.Value = value;

                        }

                    }
                }
            }
        }

        readonly UndoRedo<int> _RedBalance = new UndoRedo<int>();
        [XmlAttribute, Browsable(true), DisplayName("Red Balance")]
        [EditorAttribute(typeof(InterfaceValueSelector), typeof(System.Drawing.Design.UITypeEditor))]
        public int RedBalance {
            get {

                return _RedBalance.Value;
            }
            set {
                if (_RedBalance.Value != value) {

                    if (value >= _MinRedBalance && value <= _MaxRedBalance) {


                        if (UndoRedoManager.IsCommandStarted) {
                            using (UndoRedoManager.Start(this.CameraName + " Red White balance changed to:" + value)) {
                                _RedBalance.Value = value;
                                UndoRedoManager.Commit();
                            }


                        } else {

                            _RedBalance.Value = value;

                        }

                    }
                }
            }
        }

        readonly UndoRedo<int> _BlueBalance = new UndoRedo<int>();
        [XmlAttribute, Browsable(true),DisplayName("Blue Balance")]
        [EditorAttribute(typeof(InterfaceValueSelector), typeof(System.Drawing.Design.UITypeEditor))]
        public int BlueBalance {
            get {

                return _BlueBalance.Value;
            }
            set {
                if (_BlueBalance.Value != value) {

                    if (value >= _MinBlueBalance && value <= _MaxBlueBalance) {


                        if (!UndoRedoManager.IsCommandStarted) {
                            using (UndoRedoManager.Start(this.CameraName + " Blue White balance changed to:" + value)) {
                                _BlueBalance.Value = value;
                                UndoRedoManager.Commit();
                            }
                        } else {
                            _BlueBalance.Value = value;

                        }

                    }
                }
            }
        }

        readonly UndoRedo<int> _GreenBalance = new UndoRedo<int>();
        [XmlAttribute, Browsable(true), DisplayName("Green Balance")]
        [EditorAttribute(typeof(InterfaceValueSelector), typeof(System.Drawing.Design.UITypeEditor))]
        public int GreenBalance {
            get {

                return _GreenBalance.Value;
            }
            set {
                if (_GreenBalance.Value != value) {

                    if (value >= _MinGreenBalance && value <= _MaxGreenBalance) {


                        if (!UndoRedoManager.IsCommandStarted) {
                            using (UndoRedoManager.Start(this.CameraName + " Green White balance changed to:" + value)) {
                                _GreenBalance.Value = value;
                                UndoRedoManager.Commit();
                            }
                        } else {
                            _GreenBalance.Value = value;

                        }

                    }
                }
            }
        }

        readonly UndoRedo<string> _PixelFormat = new UndoRedo<string>();
        [XmlAttribute]
        [EditorAttribute(typeof(InterfaceValueSelector), typeof(System.Drawing.Design.UITypeEditor)), Browsable(true)]
        public string PixelFormat {
            get {
                return _PixelFormat.Value;
            }
            set {
                if (_PixelFormat.Value != value) {


                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start(this.CameraName + " PixelFormat changed to:" + value)) {
                            _PixelFormat.Value = value;
                            UndoRedoManager.Commit();
                        }


                    } else {

                        _PixelFormat.Value = value;

                    }

                    if (_ICSCamera!=null) {
                        if (_ICSCamera.Camera != null) {
                            if (this._ICSCamera.Camera.DeviceValid) {
                                if (_ICSCamera.Camera.VideoFormats.ToList().Find(byname => byname.Name == value) != null) {
                                    _ICSCamera.Camera.VideoFormat = value;
                                }

                            }
                        } 
                    }
                }
            }
        }
        

        readonly UndoRedo<int> _zoom = new UndoRedo<int>();
        [XmlAttribute]
        [EditorAttribute(typeof(InterfaceValueSelector), typeof(System.Drawing.Design.UITypeEditor)), Browsable(true)]
        public int Zoom {
            get {                
                return _zoom.Value;
            }
            set {
                if (_zoom.Value != value) {

                    if (value >= _minzoom && value <= _maxzoom) {

                        if (value > _minfocus && value < _maxfocus) {
                            if (!UndoRedoManager.IsCommandStarted) {
                                using (UndoRedoManager.Start(this.CameraName + " Zoom changed to:" + value)) {
                                    _zoom.Value = value;
                                    UndoRedoManager.Commit();
                                }


                            } else {

                                _zoom.Value = value;

                            }
                        }
                    }
                }
            }
        }

        readonly UndoRedo<int> _focus = new UndoRedo<int>();
        [XmlAttribute]
        [EditorAttribute(typeof(InterfaceValueSelector), typeof(System.Drawing.Design.UITypeEditor)), Browsable(true)]
        public int Focus {
            get {

                return _focus.Value;
            }
            set {
                if (_focus.Value != value) {

                    if (value > _minfocus && value < _maxfocus) {
                        if (!UndoRedoManager.IsCommandStarted) {
                            using (UndoRedoManager.Start(this.CameraName + " Focus changed to:" + value)) {
                                _focus.Value = value;
                                UndoRedoManager.Commit();
                            }

                        } else {

                            _focus.Value = value;
                        }
                    }
                }
            }
        }

        readonly UndoRedo<int> _exposure = new UndoRedo<int>();

        [XmlAttribute, Browsable(true)]
        [EditorAttribute(typeof(InterfaceValueSelector), typeof(System.Drawing.Design.UITypeEditor))]
        public int Exposure {
            get {
                return _exposure.Value;
            }
            set {
                if (_exposure.Value != value) {

                    if (value > _minexposure && value < _maxexposure) {

                        if (!UndoRedoManager.IsCommandStarted) {
                            using (UndoRedoManager.Start(this.CameraName + " Exposure changed to:" + value)) {
                                _exposure.Value = value;
                                UndoRedoManager.Commit();
                            }


                        } else {

                            _exposure.Value = value;
                        }

                    }
                }
            }
        }



        public override Image<Bgr, Byte> GetImage() {


            try {

                if (_ICSCamera == null) {
                    return null;
                }


                Boolean updatesettings = false;

                if (!_ICSCamera.Camera.IsLivePrepared) {
                    _ICSCamera.Camera.LiveStart();
                }

                if (_ICSCamera.Zoominterface != null) {
                    if (_ICSCamera.Zoominterface.Value != Zoom) {
                        int zoomdif = Math.Abs(_ICSCamera.Zoominterface.Value - Zoom);
                        _ICSCamera.Zoominterface.Value = Zoom;
                        _ICSCamera.Camera.MemorySnapImageSequence(1,5000);
                        int sleeptime = (int)(0.40 * zoomdif);
                        Thread.Sleep(sleeptime * 1000);
                    }
                }


                if (_ICSCamera.Exposureinterface != null) {

                    if (Math.Round(_ICSCamera.Exposureinterface.Value, 4) != Math.Round(GetAbsVal(Exposure), 4)) {
                        _ICSCamera.Exposureinterface.Value = Math.Round(GetAbsVal(Exposure), 4);
                        updatesettings = true;
                    }


                }

                if (_ICSCamera.Focusinterface != null) {

                    if (_ICSCamera.Focusinterface.Value != Focus) {
                        _ICSCamera.Focusinterface.Value = Focus;
                        updatesettings = true;
                    }
                }

                if (_ICSCamera.Irisinterface != null) {

                    if (_ICSCamera.Irisinterface.Value != Iris) {
                        _ICSCamera.Irisinterface.Value = Iris;
                        updatesettings = true;
                    }
                }

                if (_ICSCamera.Gaininterface != null) {

                    if (_ICSCamera.Gaininterface.Value != Gain) {
                        _ICSCamera.Gaininterface.Value = Gain;
                        updatesettings = true;
                    }
                }

                if (_ICSCamera.Redbalanceinterface != null) {

                    if (_ICSCamera.Redbalanceinterface.Value != RedBalance) {
                        _ICSCamera.Redbalanceinterface.Value = RedBalance;
                        updatesettings = true;
                    }
                }

                if (_ICSCamera.Greenbalanceinterface != null) {

                    if (_ICSCamera.Greenbalanceinterface.Value != GreenBalance) {
                        _ICSCamera.Greenbalanceinterface.Value = GreenBalance;
                        updatesettings = true;
                    }
                }

                if (_ICSCamera.Bluebalanceinterface != null) {

                    if (_ICSCamera.Bluebalanceinterface.Value != BlueBalance) {
                        _ICSCamera.Bluebalanceinterface.Value = BlueBalance;
                        updatesettings = true;
                    }
                }


                if (updatesettings) {
                    _ICSCamera.Camera.MemorySnapImageSequence(2,5000);
                }


                int ct1 = 0;

                while (true) {
                    log.Info("Capturing image from : " +_ICSCamera.Camera.Device);

                    try {
                        _ICSCamera.Camera.MemorySnapImageSequence(2, 5000);
                        break;
                    } catch (Exception exp) {

                        _ICSCamera.Camera.LiveStop();                        

                        ct1++;
                        if (ct1 > 2) {
                            log.Error(exp);
                            return null;
                        }

                    }
                }
                log.Info("Capturing Done");

                _ICSCamera.Camera.ImageActiveBuffer.Lock();
                Image<Bgr, Byte> outimage = null;
                if (_ICSCamera.Camera.ImageActiveBuffer.BitsPerPixel == 8) {
                    outimage = new Image<Bgr, byte>(_ICSCamera.Camera.ImageActiveBuffer.Bitmap);
                }
                else {
                    using (Image<Bgr, Int32> temp = new Image<Bgr, Int32>(_ICSCamera.Camera.ImageActiveBuffer.Bitmap)) {
                        if (_ICSCamera.Camera.ImageActiveBuffer.BitsPerPixel == 24) {
                            outimage = new Image<Bgr, byte>(temp.Size);
                            outimage.ConvertFrom<Bgr, Int32>(temp);
                            //CvInvoke.cvCvtColor(temp, outimage, Emgu.CV.CvEnum.COLOR_CONVERSION.cvbgr24);
                        }
                        else {
                            CvInvoke.cvCvtColor(temp, outimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_GRAY2BGR);
                        }
                    }

                }
                switch (UseChannel) {
                    case Channel.Bgr:
                        break;
                    case Channel.Red:
                        CvInvoke.cvCvtColor(outimage[2], outimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_GRAY2BGR);
                        break;
                    case Channel.Green:
                        CvInvoke.cvCvtColor(outimage[1], outimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_GRAY2BGR);
                        break;
                    case Channel.Blue:
                        CvInvoke.cvCvtColor(outimage[0], outimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_GRAY2BGR);
                        break;
                    case Channel.Mono:
                        //CvInvoke.cvCvtColor(outimage, outimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_BGR2GRAY);                        
                        Image<Gray, Byte> grayimage = new Image<Gray, byte>(outimage.Size);
                        grayimage.ConvertFrom<Bgr, Byte>(outimage);
                        CvInvoke.cvCvtColor(grayimage, outimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_GRAY2BGR);
                        break;
                    default:
                        break;
                }

                _ICSCamera.Camera.ImageActiveBuffer.Unlock();

                return outimage;

            } catch (Exception exp) {

                log.Error(exp);
                return null;
            }
           
        }

        public override string ToString() {

            if (this.CameraName == "Capture from IC Imaging Camera") {
                return this.CameraName;
            }
                    
            
            return "ICS:" + this.CameraName;
        }

        public ICSCameraCapture(VisionProject selectedProject) 
            :base(selectedProject) {
            _CameraName.OnMemberRedo += new UndoRedo<string>.MemberRedo(_CameraName_OnMemberRedo);
            _CameraName.OnMemberUndo += new UndoRedo<string>.MemberUndo(_CameraName_OnMemberRedo);
            Camtype = CameraTypes.ICS;
        }

        public ICSCameraCapture() {


           

            _CameraName.OnMemberRedo += new UndoRedo<string>.MemberRedo(_CameraName_OnMemberRedo);
            _CameraName.OnMemberUndo += new UndoRedo<string>.MemberUndo(_CameraName_OnMemberRedo);
            Camtype = CameraTypes.ICS;

        }

        void _CameraName_OnMemberRedo(string UndoObject) {
            SetCamera(UndoObject);
        }

        public ICSCameraCapture(String DeviceName, VisionProject selectedproject)
            : base(selectedproject) {


            CameraName = DeviceName;

        }

        public override void Dispose() {
            base.Dispose();
        }

    }



    public class FileCapture : BaseCapture {
        KPPLogger log = new KPPLogger(typeof(FileCapture));


        String _CameraName = "";
        [XmlAttribute, DisplayName("Camera Name"), ReadOnly(false)]
        public override String CameraName {
            get {
                return _CameraName;
            }
            set {

                _CameraName = value;
            }
        }


        readonly UndoRedo<String> _FileLoc = new UndoRedo<string>("");
        [XmlAttribute]
        [Category("Aquisition Settings"), Description("Select file location"), DisplayName("File Name"), Browsable(true)]
        [EditorAttribute(typeof(MyFileNameEditor), typeof(UITypeEditor))]
        public String FileLoc {
            get {
                return _FileLoc.Value;
            }
            set {
                if (value != _FileLoc.Value) {


                    if (!UndoRedoManager.IsCommandStarted) {

                        using (UndoRedoManager.Start(this.CameraName + " File location changed to: " + value)) {
                            _FileLoc.Value = value;
                            UndoRedoManager.Commit();
                        }
                    }
                    else {
                        _FileLoc.Value = value;
                    }



                }
            }
        }


        public override Image<Bgr, Byte> GetImage() {


                return new Image<Bgr, byte>(FileLoc);
           
        }

        public override string ToString() {
            return FileLoc;
        }

        public FileCapture(String name,VisionProject selectedproject)
            :base(selectedproject) {
            CameraName = name;
            FileLoc = "Select file location";
            this.Camtype = CameraTypes.File;
            //SpecialFunctions.ChageAttributeValue<ReadOnlyAttribute>(this, "CameraName", "isReadOnly", false);


        }

        public FileCapture() {
            CameraName = "Add file location";
            FileLoc = "Select location";
            this.Camtype = CameraTypes.File;
            //SpecialFunctions.ChageAttributeValue<ReadOnlyAttribute>(this, "CameraName", "isReadOnly", false);
            

        }

        public override void Dispose() {
            base.Dispose();
        }



    }

    public class RequestSourceInspections {



        private Image<Bgr, Byte> _InspectionImage;
        [XmlIgnore]
        public Image<Bgr, Byte> InspectionImage {
            get {

                return _InspectionImage;
            }

        }

        private String _CallingRequest;

        public String CallingRequest {
            get { return _CallingRequest; }
            set { _CallingRequest = value; }
        }

        private Inspection _RequestSourceInspection;

        public Inspection RequestSourceInspection {
            get { return _RequestSourceInspection; }
            set {
                _RequestSourceInspection = value;
                if (_RequestSourceInspection != null) {

                } else {

                }
            }
        }

      

        public RequestSourceInspections(String Callingrequest, Inspection inspection) {
            CallingRequest = Callingrequest;
            RequestSourceInspection = inspection;
        }

        public RequestSourceInspections() {
        }
    }


    public class ZoneInfo {


        private String m_ZoneName = "Not set";
        [XmlAttribute,DisplayName("Zone Name")]
        public String ZoneName {
            get { return m_ZoneName; }
            set { m_ZoneName = value; }
        }

        private Rectangle m_Zone = Rectangle.Empty;

        public Rectangle Zone {
            get { return m_Zone; }
            set { m_Zone = value; }
        }

        public override string ToString() {
            return ZoneName + " - " + Zone.ToString();
        }

        public ZoneInfo(String name, Rectangle zone) {
            ZoneName = name;
            Zone = zone;
        }
        public ZoneInfo() {
        }
    }

    [Serializable]
    public class InspectionCapture : BaseCapture {
        [NonSerialized]
        KPPLogger log = new KPPLogger(typeof(InspectionCapture));

        public delegate void CaptureInspectionNameChanged(InspectionCapture Sender, String newName);



        String _CameraName = "";
        [XmlAttribute, DisplayName("Camera Name"), ReadOnly(false)]
        public override String CameraName {
            get {
                return _CameraName;
            }
            set {
                _CameraName = value;
            }
        }

        readonly UndoRedo<Inspection> _InspectionSource = new UndoRedo<Inspection>();
        [EditorAttribute(typeof(InspNameSelector), typeof(System.Drawing.Design.UITypeEditor))]
        [XmlIgnore]
        public Inspection InspectionSource {
            get {
                return _InspectionSource.Value;
            }
            set {
                if (_InspectionSource.Value != value) {

                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start(this.CameraName + " Inspection source changed to:" + value)) {
                            _InspectionSource.Value = value;
                            UndoRedoManager.Commit();
                        }


                    } else {

                        _InspectionSource.Value = value;

                    }

                    if (InspectionSource != null) {
                        _CameraName = InspectionSource.Name+"."+InspectionSource.RequestID;
                        InspectionName = InspectionSource.Name + "." + InspectionSource.RequestID;
                    }
                }
            }
        }

        private Boolean _ShowAuxROIS = false;
        [XmlAttribute,DisplayName("Show Aux ROIs")]
        public Boolean ShowAuxROIS {
            get { return _ShowAuxROIS; }
            set { _ShowAuxROIS = value; }
        }


        String _InspectionName = "";
        [XmlAttribute]
        [Browsable(false)]
        public String InspectionName {
            get {

                return _InspectionName;
            }
            set {
                if (_InspectionName != value) {
                    _InspectionName = value;

                }

            }
        }


        private ZoneInfo m_CaptureZone = new ZoneInfo();
        [EditorAttribute(typeof(ZoneSelector), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ZoneInfo CaptureZone {
            get { return m_CaptureZone; }
            set {
                if (m_CaptureZone != value) {
                    m_CaptureZone = value;
                }
            }
        }


        public override Image<Bgr, Byte> GetImage(Boolean FullImage) {

            if (InspectionSource == null) {
                return null;
            }
            InspectionSource.Execute(this, false);

            if (InspectionSource.OriginalImageBgr == null) {
                return new Image<Bgr, byte>(new Size(800, 600));
            }


            InspectionSource.OriginalImageBgr.ROI = Rectangle.Empty;

            return InspectionSource.OriginalImageBgr;

        }



        public override Image<Bgr, Byte> GetImage() {


            if (InspectionSource == null) {
                return null;
            }
            InspectionSource.Execute(this, false);
            if ( InspectionSource.OriginalImageBgr==null) {
                return new Image<Bgr,byte>(new Size(800,600));
            }
            if (CaptureZone.Zone!=Rectangle.Empty) {                
                InspectionSource.OriginalImageBgr.ROI = CaptureZone.Zone;
                return InspectionSource.OriginalImageBgr;
            } else {
                return InspectionSource.OriginalImageBgr;
            }
            

        }

        public override string ToString() {
            if (InspectionSource != null) {

                return InspectionSource.RequestName+" - "+InspectionSource.Name;
            } else {
                return "No Inspection Source";
            }

        }

        public InspectionCapture(String name,VisionProject selectedproject)
            :base(selectedproject) {            
            CameraName = name;

            this.Camtype = CameraTypes.Inspection;
            //SpecialFunctions.ChageAttributeValue<ReadOnlyAttribute>(this, "CameraName", "isReadOnly", false);


        }

        public InspectionCapture(VisionProject selectedproject)
            : base(selectedproject) {

            CameraName = "Select Inspection";
            
            this.Camtype = CameraTypes.Inspection;
            //SpecialFunctions.ChageAttributeValue<ReadOnlyAttribute>(this, "CameraName", "isReadOnly", false);


        }


        public InspectionCapture() {
            CameraName = "Select Inspection";
            this.Camtype = CameraTypes.Inspection;
            //SpecialFunctions.ChageAttributeValue<ReadOnlyAttribute>(this, "CameraName", "isReadOnly", false);


        }

        public override void Dispose() {
            base.Dispose();
        }


    }

    public class DirectShowCameraCapture : BaseCapture {
        KPPLogger log = new KPPLogger(typeof(DirectShowCameraCapture));

        private static FilterInfoCollection _DevicesAvaible = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        [XmlIgnore]
        internal static FilterInfoCollection DevicesAvaible {
            get { return DirectShowCameraCapture._DevicesAvaible; }
            set { DirectShowCameraCapture._DevicesAvaible = value; }
        }

        VideoCaptureDevice _Camera =null;
        [XmlIgnore,Browsable(false)]
        public VideoCaptureDevice Camera {
            get { return _Camera; }
            set {

                if (_Camera != value) {
                    if (_Camera!=null) {
                        _Camera.Stop();
                        _Camera = null;                        
                    }
                    if (value!=null) {
                        _Camera = value;
                        _Camera.VideoResolution = _Camera.VideoCapabilities[_Camera.VideoCapabilities.Count() - 1];
                        _Camera.NewFrame += new NewFrameEventHandler(_Camera_NewFrame);
                        _Camera.Start();
                    }
                }
            }
        }

        int FrameCount = 0;
      
        void _Camera_NewFrame(object sender, NewFrameEventArgs eventArgs) {
            try {
                lock (lockobject) {
                    if (FrameImage != null) {
                        FrameImage.Dispose();
                    }
                    FrameImage = new Image<Bgr, byte>(eventArgs.Frame);
                }
            } catch (Exception exp) {

                FrameImage = null;
            }
        }

        

        void SetCamera(String value) {
            try {
                FilterInfo camerainfo = DirectShowCameraCapture.DevicesAvaible.Cast<FilterInfo>().ToList().Find(name => name.Name == value);
                if (camerainfo != null) {
                    Camera = new VideoCaptureDevice(camerainfo.MonikerString);
                    

                }
            } catch (Exception exp) {

            }
        }

        private UndoRedo<String> _CameraName = new UndoRedo<string>("");
        [XmlAttribute, DisplayName("Camera Name"), ReadOnly(false), Browsable(true)]
        [EditorAttribute(typeof(DirectShowSelector), typeof(UITypeEditor))]
        public override String CameraName {
            get {
                return _CameraName.Value;
            }
            set {

                if (_CameraName.Value != value) {

                    if (!UndoRedoManager.IsCommandStarted) {

                        using (UndoRedoManager.Start(this.CameraName + " Camera changed to: " + value)) {
                            SetCamera(value);
                            _CameraName.Value = value;
                            UndoRedoManager.Commit();

                        }
                    }
                    else {
                        SetCamera(value);
                        _CameraName.Value = value;
                    }
                }
            }
        }


        readonly UndoRedo<double> _exposure= new UndoRedo<double>();
        [XmlAttribute]
        public double Exposure {
            get {

                return _exposure.Value;
            }
            set {
                if (_exposure.Value != value) {

                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start(this.CameraName + " Exposure changed to:" + value)) {
                            _exposure.Value = value;
                            UndoRedoManager.Commit();
                        }
                    }

                } else {

                    _exposure.Value = value;
                }
            }
        }



        private Image<Bgr, Byte> FrameImage = null;

        private object lockobject = new object();        
 //       private Boolean capture = false;
        public override Image<Bgr, Byte> GetImage() {


           
            Image<Bgr, Byte> newimage = null;

            for (int i = 0; i < 5; i++) {
                Thread.Sleep(1); 
            }
            lock (lockobject) {
                if (FrameImage != null) {
                    newimage = new Image<Bgr, byte>(FrameImage.Size);
                    switch (UseChannel) {
                        case Channel.Bgr:
                            break;
                        case Channel.Red:
                            CvInvoke.cvCvtColor(FrameImage[2], newimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_GRAY2BGR);
                            break;
                        case Channel.Green:
                            CvInvoke.cvCvtColor(FrameImage[1], newimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_GRAY2BGR);
                            break;
                        case Channel.Blue:
                            CvInvoke.cvCvtColor(FrameImage[0], newimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_GRAY2BGR);
                            break;
                        case Channel.Mono:
                            //CvInvoke.cvCvtColor(outimage, outimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_BGR2GRAY);                        
                            Image<Gray, Byte> grayimage = new Image<Gray, byte>(FrameImage.Size);
                            grayimage.ConvertFrom<Bgr, Byte>(FrameImage);
                            CvInvoke.cvCvtColor(grayimage, newimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_GRAY2BGR);
                            break;
                        default:
                            break;
                    }
                    //FrameImage.CopyTo(newimage);
                } 
            }
                Console.WriteLine("Capture Done");
                return newimage;
            
        }

        private int framecounter=0;
        void Camera_ImageGrabbed(object sender, EventArgs e) {
           
        }

        public override string ToString() {
            return "Direct Show Capture";
        }

        static int Globalid = -1;

        int id;
        public DirectShowCameraCapture(VisionProject selectedproject)
            : base(selectedproject) {
           // CamIndex = -1;
            this.id = ++DirectShowCameraCapture.Globalid;
            this.CameraName = "Direct Show input";
            this.Camtype = CameraTypes.DirectShow;
        }

        public DirectShowCameraCapture(){
            // CamIndex = -1;
            this.id = ++DirectShowCameraCapture.Globalid;
            this.CameraName = "Direct Show input";
            this.Camtype = CameraTypes.DirectShow;
        }


        public override void Dispose() {
            if (_Camera != null) {
                _Camera.Stop();
                _Camera = null;
               
            }
        }

    }

    public class CVCameraCapture : BaseCapture {
        KPPLogger log = new KPPLogger(typeof(CVCameraCapture));

        decimal _CamIndex = -1;
        [XmlAttribute("Camera Index")]
        public decimal CamIndex {
            get {
                return _CamIndex;
            }
            set {
                if (_CamIndex != value) {
                    try {
                        if (Camera != null) {
                            Camera.Dispose();
                            Camera = null;
                        }
                        _CamIndex = value;
                    } catch (Exception exp) {
                        _CamIndex = -1;
                        log.Error(exp);
                    }


                }
            }
        }

        readonly UndoRedo<double> _exposure= new UndoRedo<double>();
        [XmlAttribute]
        public double Exposure {
            get {

                return _exposure.Value;
            }
            set {
                if (_exposure.Value != value) {

                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start(this.CameraName + " Exposure changed to:" + value)) {
                            _exposure.Value = value;
                            UndoRedoManager.Commit();
                        }
                    }

                } else {

                    _exposure.Value = value;
                }
            }
        }


        Capture _Camera = null;
        [XmlIgnore]
        [Browsable(false)]
        public Capture Camera {
            get {
                return _Camera;
            }
            set {
                if (_Camera != value) {
                    _Camera = value;
                }
            }
        }



        private object lockobject = new object();
        private Image<Bgr, Byte> frameImage = null;
        private Boolean capture = false;
        public override Image<Bgr, Byte> GetImage() {

         
                if (Camera == null) {

                    
                    
                    Camera = new Capture(0);
                    Camera.ImageGrabbed += new Capture.GrabEventHandler(Camera_ImageGrabbed);
                    Camera.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 320);
                    Camera.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 240);

                    //Camera.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 640);
                    //Camera.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 480);
                    //Camera.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FPS, 1); 
                    //Camera.Start();
                    Camera.Grab();
                    

                }

                Image<Bgr, Byte> newimage = null;
                //frameImage = null;
                //Console.WriteLine("Capture Start");
                //capture = true;
                //do {

                //    lock (lockobject) {
                //        if (frameImage!=null) {
                //            newimage = frameImage;
                //            break;
                //        }
                //    }
                //    CvInvoke.cvWaitKey(1);
    
                //} while (true);

                //capture = false;
                
                //Image<Bgr, Byte> newimage = Camera.QueryFrame().Convert<Bgr,Byte>();

                for (int i = 0; i < 5; i++) {
                    Camera.Grab();
                    CvInvoke.cvWaitKey(30);
                    //Camera.RetrieveBgrFrame();
                    //CvInvoke.cvWaitKey(5);
                }

                newimage = Camera.RetrieveBgrFrame();

            //    string _appfile = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "temp.bmp");
             //   newimage.Save(_appfile);
                Console.WriteLine("Capture Done");
                return newimage;
            
        }

        private int framecounter=0;
        void Camera_ImageGrabbed(object sender, EventArgs e) {
          
        }

        public override string ToString() {
            return "OpenCV Capture";
        }

        public CVCameraCapture(VisionProject selectedproject)
            : base(selectedproject) {
        }

        public CVCameraCapture() {
            CamIndex = -1;
            this.CameraName = "Opencv input";
            this.Camtype = CameraTypes.CV;
        }

        public override void Dispose() {
            if (Camera!=null) {
                Camera.Stop();
                Camera = null;

                Console.WriteLine("Camera Stopped");
            }
        }

    }

    //public class RemoteCameraCapture : BaseCapture {
        
        
    //    KPPLogger log = new KPPLogger(typeof(RemoteCameraCapture));


    //    public delegate void RemoteImage(Image<Bgr,Byte> theimage);

    //    public event RemoteImage OnRemoteImage;


    //    BaseCapture _RemoteCamera = new BaseCapture();
    //    [TypeConverter(typeof(ExpandableObjectConverter))]
    //    [DisplayName("Remote Camera")]
    //    [EditorAttribute(typeof(InputSourceSelector), typeof(System.Drawing.Design.UITypeEditor))]
    //    public BaseCapture RemoteCamera {
    //        get { return _RemoteCamera; }
    //        set { _RemoteCamera = value; }
    //    }

    //    private TCPClientConnection _Cliente = new TCPClientConnection();
    //    [TypeConverter(typeof(ExpandableObjectConverter))]
    //    public TCPClientConnection Cliente {
    //        get { return _Cliente; }
    //        set {
    //            if (_Cliente != value ) {
    //                _Cliente = value;
    //                if (_Cliente != null) {
    //                    _Cliente.OnServerMessage += new TCPClientConnection.ServerMessage(_Cliente_OnServerMessage);

    //                }
    //            }
                
    //        }
    //    }

    //    object locker = new object();

    //    Image<Bgr,Byte> remoteimage = null;
    //    void _Cliente_OnServerMessage(string[] Commands) {
    //        if (Commands.Count() > 0) {
    //            if (Commands[0] == "IMAGE") {

    //                String image_serialized = StaticObjects.base64Decode(Commands[1]);

    //                lock (locker) {
                        
    //                    remoteimage = (Image<Bgr, Byte>)IOFunctions.DeserializeFromString(image_serialized, typeof(Image<Bgr, Byte>));
    //                    if (OnRemoteImage != null) {
    //                        OnRemoteImage(remoteimage);
    //                    }
    //                }


    //            }
    //            else {
    //                //String sendstr = "";
    //                //foreach (String item in Commands) {
    //                //    sendstr += item + "|";
    //                //}
    //                //throw new Exception("Unknown remote command : "+sendstr);
    //                //log.Debug();
    //            }
    //        }
    //    }

    //    Stopwatch waitimage = new Stopwatch();
    //    public override void GetImage(ref Image<Bgr, Byte> CapturedImage) {

      

    //            if (StaticObjects.isRemote) {
    //                RemoteCamera.GetImage(ref CapturedImage);


    //            }
    //            else {
    //                if (Cliente.State!= TCPClientConnection.ConnectionState.Connected) {
    //                    Cliente.Connect();
    //                    Thread.Sleep(1000);
    //                }
    //                if (Cliente.State!= TCPClientConnection.ConnectionState.Connected) {
    //                    throw new Exception("Error connecting to remote capture...");
    //                }

    //                if (remoteimage!=null) {
    //                    remoteimage.Dispose();
    //                    remoteimage = null;
    //                }
    //                this.Cliente.Write("REQUEST|1|0\n\r");
    //                waitimage.Reset();
    //                waitimage.Start();
                  
    //                Boolean doexit = false;
    //                do {
    //                    Thread.Sleep(1);
    //                    if (waitimage.ElapsedMilliseconds == 10000) {
    //                        throw new Exception("Receiving image time out");

    //                    }
    //                    lock (locker) {
    //                        if (remoteimage!=null) {
    //                            doexit = true;
    //                        }
    //                    }
                        

    //                } while (!doexit);
                  

    //                return remoteimage;
    //            }
               
           
    //    }

    //    public override string ToString() {
    //        return "Remote Capture";
    //    }

    //    public RemoteCameraCapture() {
    //        this.CameraName = "Remote Camera";
    //        this.Camtype = CameraTypes.Remote;
    //    }

    //    public override void Dispose() {
    //        if (Cliente!=null) {
    //            Cliente.Disconnect();
    //        }

    //        if (RemoteCamera!=null) {
    //            RemoteCamera.Dispose();
    //        }
    //        base.Dispose();
    //    }
    //}



    public class PythonRemoteCapture : BaseCapture {

        static List<Type> _AvaibleFunctions = new List<Type>() {typeof(ProcessingFunctionPixelanalysis) };

        internal static List<Type> AvaibleFunctions {
            get { return PythonRemoteCapture._AvaibleFunctions; }
            //set { PythonRemoteCapture._AvaibleFunctions = value; }
        }

        KPPLogger log = new KPPLogger(typeof(PythonRemoteCapture));

        
        public delegate void RemoteImage(Image<Bgr, Byte> theimage);
        
        public event RemoteImage OnRemoteImage;


        //BaseCapture _RemoteCamera = new BaseCapture();
        //[TypeConverter(typeof(ExpandableObjectConverter))]
        //[DisplayName("Remote Camera")]
        //[EditorAttribute(typeof(InputSourceSelector), typeof(System.Drawing.Design.UITypeEditor))]
        //public BaseCapture RemoteCamera {
        //    get { return _RemoteCamera; }
        //    set { _RemoteCamera = value; }
        //}


        private TCPClientConnection _Cliente = new TCPClientConnection();
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TCPClientConnection Cliente {
            get { return _Cliente; }
            set {
                if (_Cliente != value) {
                    _Cliente = value;
                    if (_Cliente != null) {
                        _Cliente.OnServerMessage += new TCPClientConnection.ServerMessage(_Cliente_OnServerMessage);

                    }
                }

            }
        }

        object locker = new object();

        Image<Bgr, Byte> remoteimage = null;
        void _Cliente_OnServerMessage(string[] Commands) {
            if (Commands.Count() > 0) {
                if (Commands[0] == "IMAGE") {

                    //String image_serialized = StaticObjects.base64Decode();
                    byte[] data = Convert.FromBase64String(Commands[1]);
                    TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                    Bitmap bitmap1 = (Bitmap)tc.ConvertFrom(data);

                    lock (locker) {

                        remoteimage = new Image<Bgr, byte>(bitmap1);
                        if (OnRemoteImage != null) {
                            OnRemoteImage(remoteimage);
                        }
                    }


                }
                else {
                    //String sendstr = "";
                    //foreach (String item in Commands) {
                    //    sendstr += item + "|";
                    //}
                    //throw new Exception("Unknown remote command : "+sendstr);
                    //log.Debug();
                }
            }
        }

        Stopwatch waitimage = new Stopwatch();

        int _cam=0;
        Boolean _capture = false;
        public override Image<Bgr, Byte> GetImage(int cam, Boolean capture) {
            _cam = cam;
            _capture = capture;
            return GetImage();
        }

        public override Image<Bgr, Byte> GetImage() {



            //if (StaticObjects.isRemote) {
            //    return RemoteCamera.GetImage();


            //}
            //else {
                if (Cliente.State != TCPClientConnection.ConnectionState.Connected) {
                    Cliente.Connect();
                    Thread.Sleep(1000);
                }
                if (Cliente.State != TCPClientConnection.ConnectionState.Connected) {
                    throw new Exception("Error connecting to remote capture...");
                }

                if (remoteimage != null) {
                    remoteimage.Dispose();
                    remoteimage = null;
                }
                if (_cam>0) {
                    if (_capture)
                    {
                        this.Cliente.Write("CAPTUREIMAGE|" + _cam.ToString() + "\n");
                    }
                    else
                    {
                        this.Cliente.Write("GETIMAGE|" + _cam.ToString() + "\n");
                    }

                }
                else {
                    this.Cliente.Write("CAPTUREIMAGES\n");
                }
                
                waitimage.Reset();
                waitimage.Start();

                Boolean doexit = false;
                do {
                    Thread.Sleep(1);
                    if (waitimage.ElapsedMilliseconds >= 10000) {
                        throw new Exception("Receiving image time out");

                    }
                    lock (locker) {
                        if (remoteimage != null) {
                            doexit = true;
                        }

                    }


                } while (!doexit);



                return null;

        }

        public override string ToString() {
            return "Python Remote Capture";
        }

        public PythonRemoteCapture(VisionProject selectedproject)
            : base(selectedproject) {
            this.CameraName = "Python Remote Camera";
            this.Camtype = CameraTypes.Remote;
        }

        public PythonRemoteCapture() {
            this.CameraName = "Python Remote Camera";
            this.Camtype = CameraTypes.Remote;
        }

        public override void Dispose() {
            if (Cliente != null) {
                Cliente.Disconnect();
            }

            
            base.Dispose();
        }
    }



    //[XmlInclude(typeof(RemoteCameraCapture))]
    [XmlInclude(typeof(DirectShowCameraCapture))]
    [XmlInclude(typeof(CVCameraCapture))]
    [XmlInclude(typeof(ICSCameraCapture))]
    [XmlInclude(typeof(FileCapture))]
    [XmlInclude(typeof(InspectionCapture))]
    [XmlInclude(typeof(uEyeCamera))]
    [XmlInclude(typeof(PythonRemoteCapture))]
    [Serializable]
    public class BaseCapture {




        public enum CameraTypes { Remote, DirectShow, CV, ICS, File, Inspection, Request, uEye,Undef }
        [NonSerialized]
        KPPLogger log = new KPPLogger(typeof(BaseCapture));

        CameraTypes _camtype = CameraTypes.Undef;
        [XmlAttribute, ReadOnly(true)]
        public CameraTypes Camtype {
            get { return _camtype; }
            set {
                if (_camtype != value) {
                    _camtype = value;



                }
            }
        }

        //private int _LogImages = 0;
        //[XmlAttribute,DisplayName("Log Images"),Description("Number of images to log")]
        //public int LogImages {
        //    get { return _LogImages; }
        //    set { _LogImages = value; }
        //}

        //internal static List<BaseCapture> CaptureSources = new List<BaseCapture>();


        
        public virtual void UpdateSource() {

        }

        private Channel _UseChannel = Channel.Bgr;
        [XmlAttribute,DisplayName("Output channel")]
        public virtual Channel UseChannel {
            get { return _UseChannel; }
            set { _UseChannel = value; }
        }



        String _CameraName = "";
        [XmlAttribute, DisplayName("Camera Name"), ReadOnly(false), Browsable(false)]
        public virtual String CameraName {
            get {
                return _CameraName;
            }
            set {
                if (_CameraName != value) {

                    String oldname = _CameraName;

                    _CameraName = value;

                   
                     
                    

                }
            }
        }

        internal delegate void AcquisitionAttributesChanged();
        internal event AcquisitionAttributesChanged OnAcquisitionAttributesChanged;

        public virtual void UpdateAttributes() {
            if (OnAcquisitionAttributesChanged!=null) {
                OnAcquisitionAttributesChanged();
            }
        }

        public virtual Image<Bgr, Byte> GetImage(Boolean FullImage) {
            return null;
        }


        public virtual Image<Bgr, Byte> GetImage(int cam, Boolean Capture) {
            return null;
        }


        public virtual Image<Bgr,Byte> GetImage() {

            return null;
        }

        private VisionProject m_SelectedProject = null;
        [XmlIgnore]
        public virtual VisionProject SelectedProject {
            get { return m_SelectedProject; }
            set { m_SelectedProject = value; }
        }


        public BaseCapture(String name,VisionProject selectedProject) {
            try {
                SelectedProject = selectedProject;
                CameraName = name;

            } catch (Exception exp) {

                log.Error(exp);
            }


        }

        public BaseCapture(VisionProject selectedProject) {
            try {
                SelectedProject = selectedProject;
                

            } catch (Exception exp) {

                log.Error(exp);
            }


        }


        public BaseCapture() {
            try {


            } catch (Exception exp) {

                log.Error(exp);
            }


        }

        public override string ToString() {
            return CameraName;
        }

        public virtual void Dispose() {
            OnAcquisitionAttributesChanged = null;
        }
    }

}
