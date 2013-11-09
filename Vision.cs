using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using TIS.Imaging;
using KPP.Core.Debug;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using KPP.Controls.Winforms.ImageEditorObjs;
using KPP.Controls.Winforms;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Globalization;
using DejaVu;
using DejaVu.Collections.Generic;
using RaspberryPiDotNet;
using IOModule;
using System.Xml;
using System.Xml.Schema;
using AForge.Video.DirectShow;
using System.Resources;
using KPPAutomationCore;






namespace VisionModule {

    public class InputReferenceSelector : System.Drawing.Design.UITypeEditor {
        // this is a container for strings, which can be 
        // picked-out
        //UserControl contextcontrol = StaticObjects.InputItemSelectorControl;
        private static KPPLogger log = new KPPLogger(typeof(InputReferenceSelector));
        internal OutputResultConfForm form = new OutputResultConfForm();

        IWindowsFormsEditorService edSvc;
        // this is a string array for drop-down list
        //internal static List<CameraInfo> RemoteCameras = new List<CameraInfo>();

       



        public InputReferenceSelector() {
            //menu.BorderStyle = BorderStyle.None;
            // add event handler for drop-down box when item 
            // will be selected
          //  textbox1.KeyDown += new KeyEventHandler(textebox1_KeyDown);

            //contextcontrol.VisibleChanged += new EventHandler(contextcontrol_VisibleChanged);
        }

      

      
        void textebox1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode== Keys.Enter) {
                edSvc.CloseDropDown();
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value) {


            ProcessingFunctionBase procbase = context.Instance as ProcessingFunctionBase;

            AttributeCollection attrs = context.PropertyDescriptor.Attributes;

            Type resultType = null;            
            for (int i = 0; i < attrs.Count; i++) {
                if (attrs[i] is AcceptType) {
                    resultType = (attrs[i] as AcceptType).acceptType;
                    break;
                }
            }

            if (resultType==null) {
                //log.Error("Result Type not set in Function: " + procbase.FunctionName+"("+procbase.FunctionType+")");
                log.Error("Result Type not set in Function: " + context.Instance.ToString());
                return value;
            }

           

            edSvc =
               (IWindowsFormsEditorService)provider.
               GetService(typeof
               (IWindowsFormsEditorService));

            if (edSvc != null) {

               // form.SelectedProject = StaticObjects.SelectedProject;
                form.acceptType = resultType;
                form.ResultRef = (value as ResultReference);
                if (edSvc.ShowDialog(form)== DialogResult.OK) {
                    value = form.ResultRef;
                }


                //if (textbox1.Text != "") {
                //    newresref.ResultReferenceID = textbox1.Text;
                //    newresref.ResultOutput = double.Parse(textbox1.Text);
                //    return newresref;
                //}

            }
            return value;
        }



    }



    [Serializable()]
    public class ReferencePoint {

        public String ReferencePointName { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public ReferencePoint(String Id) {
            OffsetX = 0;
            OffsetY = 0;
            ReferencePointName = Id;
        }


        public ReferencePoint() {
            ReferencePointName = "N/A";
        }
        public override string ToString() {
            return ReferencePointName;
        }
    }

    
    

    public class ResultInfo {



        private String _DecimalSeparator = ",";
        [XmlAttribute]
        public String DecimalSeparator {
            get { 
                return _DecimalSeparator; 
            }
            set { 
                _DecimalSeparator = value; 
            }
        }

        readonly UndoRedo<String> _ID = new UndoRedo<string>("New ID");
        public String ID {
            get {
                return _ID.Value;
            }
            set {
                if (_ID.Value != value) {
                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start("Result added:" + value)) {
                            _ID.Value = value;
                            UndoRedoManager.Commit();
                        }
                        
                    } else {
                        _ID.Value = value;
                    }
                }
            }
        }



        public class ListInputs : UndoRedoList<ResultInput> {
        }


        ListInputs _Inputs = new ListInputs();
        public ListInputs Inputs {
            get { 
                return _Inputs; 
            }
            set { 
                _Inputs = value; 
            }
        }

        public ResultInfo() {          
            
            
            
        }

        public override string ToString() {
            
            return ID;
        }
    }

    public class MyRefConverter : ExpandableObjectConverter {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }

        //public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
        //    StandardValuesCollection coll = new StandardValuesCollection(new Test[] { new TestImpl() });
        //    return coll;
        //}

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            //if (true) {
                
            //}
            //return true;
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
            ResultReference resrefinst = context.Instance as ResultReference;

            if (resrefinst==null) {
                return value;
            }
            if (resrefinst.ResultOutput==null) {
                return value;
            }
            var converter = TypeDescriptor.GetConverter(resrefinst.ResultOutput.GetType());
            if (converter.CanConvertFrom(value.GetType())) {
                object newObject = converter.ConvertFrom(value);
                value = newObject;
            }
             

            return value;
        }
    }


    [TypeConverter(typeof(ExpandableObjectConverter)), EditorAttribute(typeof(InputReferenceSelector), typeof(UITypeEditor))]
    [XmlInclude(typeof(LineSegment2D))]
    public class ResultReference {

        private Boolean _IsValid = false;
        [XmlIgnore,Browsable(false)]
        public Boolean IsValid {
            get { return _IsValid; }
            set { _IsValid = value; }
        }

        private object _ResultReferenceID = null;
        [DisplayName("Reference ID"), ReadOnly(false), TypeConverter(typeof(MyRefConverter))]
        public object ResultReferenceID {
            get {
                
                if (RequestReference != null) {
                    if (InspectionReference != null) {
                        if (ROIReference != null) {
                            if (ProcessingReference != null) {
                                if (PropertyOutput != "") {
                                    _ResultReferenceID = RequestReference.Name + "." + InspectionReference.Name + "." + ROIReference.Name + "." + ProcessingReference.FunctionName + "." + PropertyOutput;
                                } else {
                                    _ResultReferenceID = RequestReference.Name + "." + InspectionReference.Name + "." + ROIReference.Name + "." + ProcessingReference.FunctionName;
                                }

                            }
                        }
                    }
                }
                else {
                    if (IsValid==false) {
                        ResultReferenceID = _ResultReferenceID;
                    }
                }
                
                return _ResultReferenceID;
            }
            set {

                IsValid=false;
                
                _ResultReferenceID = value;
                if (SelectedProject != null) {
                    if (value is String) {
                        IsValid = true;
                        String[] References = ((string)value).Split(new String[] { "." }, StringSplitOptions.None).ToArray();
                        if (References.Count() >= 4) {
                            RequestReference = SelectedProject.RequestList.Find(n => n.Name == References[0]);
                            if (RequestReference != null) {
                                InspectionReference = RequestReference.Inspections.Find(n => n.Name == References[1]);
                                if (InspectionReference != null) {
                                    ROIReference = InspectionReference.ROIList.Find(n => n.Name == References[2]);

                                    if (ROIReference == null) {
                                        ROIReference = InspectionReference.ROIList.AuxROIS.Find(n => n.Name == References[2]);
                                    }

                                    if (ROIReference != null) {
                                        ProcessingReference = ROIReference.ProcessingFunctions.Find(n => n.FunctionName == References[3]);
                                        if (References.Count() == 5) {
                                            PropertyOutput = References[4];
                                        }
                                       
                                        UpdateValue();
                                    }
                                }

                            }
                        }
                    } else {
                        ProcessingReference = null;
                        RequestReference = null;
                        IsValid = true;
                    }
                }
                
            }
        }

        private Request RequestReference = null;
        private Inspection InspectionReference = null;
        private ROI ROIReference = null;
        private ProcessingFunctionBase ProcessingReference = null;
        String PropertyOutput = "";




        //private Boolean _IsSimpleObject = false;
        //[XmlAttribute,Browsable(false)]
        //public Boolean IsSimpleObject {
        //    get { return _IsSimpleObject; }
        //    set { _IsSimpleObject = value; }
        //}


        public void UpdateValue(Boolean SetDefault=false) {
            if (ResultReferenceID != null) {

                if (ProcessingReference != null) {

                    PropertyInfo propinfo = ProcessingReference.GetType().GetProperty(PropertyOutput);

                    if (propinfo != null) {
                        ResultOutput = propinfo.GetValue(ProcessingReference, null);
                    }

                }
                else {

                    ResultOutput = (Object)ResultReferenceID;
                }

                if (SetDefault) {

                   ResultOutput=ResultOutput.GetDefaultValue();
                }
            }
        }
        
        //else if (ProcessingReference != null) {
        //        PropertyInfo propinfo = ProcessingReference.GetType().GetProperty(PropertyOutput);

        //        if (propinfo != null) {
        //            ResultOutput = propinfo.GetValue(ProcessingReference, null);            
        //        }
           
            
            


        private Object _ResultOutput = null;       
        [TypeConverter(typeof(ExpandableObjectConverter)),DisplayName("Output"),XmlIgnore,ReadOnly(true)]
        public Object ResultOutput {
            get {                
                return _ResultOutput;
            }
            set { 
                _ResultOutput = value; 
            }
        }




        public override string ToString() {


            if (ResultReferenceID==null) {
                return "";
            }
            return ResultReferenceID.ToString(); 
        }


        //public ResultReference(String resid) {
            
        //    ResultReferenceID = resid;
        //}

        private VisionProject SelectedProject = null;
        public ResultReference(VisionProject selectedProject,Object resobj) {
            SelectedProject = selectedProject;
            if (resobj!=null) {

                ResultReferenceID = resobj;

            }
        }



        public ResultReference(VisionProject selectedProject, ProcessingFunctionBase processingfunction, String propname) {
            SelectedProject = selectedProject;
            ResultReferenceID = SelectedProject.SelectedRequest.Name + "." + SelectedProject.SelectedRequest.SelectedInspection.Name + "." + SelectedProject.SelectedRequest.SelectedInspection.SelectedROI.Name + "." + processingfunction.FunctionName + "." + propname;
        }

       
        public ResultReference() {
        }
    }


    public class ResultInput {

        private UndoRedo<ResultReference> _Input = new UndoRedo<ResultReference>();

        public ResultReference Input {
            get {
                return _Input.Value;
            }
            set {

                if (_Input.Value != value) {
                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start("Input changed to:" + value)) {
                            _Input.Value = value;
                            UndoRedoManager.Commit();
                        }
                    } else {
                        _Input.Value = value;
                    }
                }                
            }
        }

        readonly UndoRedo<String> _Parameter = new UndoRedo<string>("No Parameter");
        //[XmlAttribute]
        public String Parameter {
            get {
                return _Parameter.Value;
            }
            set {

                if (_Parameter.Value!=value) {

                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start("Parameter changed to:" + value)) {
                            _Parameter.Value = value;
                            UndoRedoManager.Commit();
                        }
                    } else {
                        _Parameter.Value = value;
                    }
                }
            }
        }


        private int _min = 0;

        public int MinValue {
            get { 
                return _min; 
            }
            set { 
                _min = value; 
            }
        }

        private int _max = 0;

        public int MaxValue {
            get {
                return _max;
            }
            set {
                _max = value;
            }
        }


        String _InputValue="";
        [XmlIgnore]
        public String InputValue {
            get {
                if (Input != null) {
                    if (Input.ResultOutput!= null) {
                        return Input.ResultOutput.ToString(); ;
                    }
                }

                return "";

            }
        }

        public ResultInput() {
        
        }

        public override string ToString() {
            return InputValue;
        }
    }



    [System.AttributeUsage(System.AttributeTargets.Event)]
    public class UseInEvents : System.Attribute {

        public Boolean Use;

        public UseInEvents(Boolean use) {
            this.Use = use;
        }
    }

    public class ResultsList : UndoRedoList<ResultInfo> {

    }

    [DefaultPropertyAttribute("Name")]
    public class Inspection {


        

        private static KPPLogger log = new KPPLogger(typeof(Inspection));

        #region Delegates
        public delegate void SelectedROIChanged(ROI ROISelected);
        
        public delegate void ROIRemoved(ROI ROIRemoved);
        
        public delegate void InspectionDone(Inspection inspection);
        public delegate void InspectionResult(Inspection inspection,String ResultString,Boolean Result);
     

        //public delegate void NoPart(Inspection inspection);

        public delegate void CaptureStopped();
        public delegate void CameraSourceChanged(BaseCapture NewCapture);

        public delegate void InspectionNameChanged(String RequestInspName, String NewInspName, Object Insp);
        public delegate void CaptureDone(Inspection sender, Image<Bgr, Byte> CapturedImage);
        public delegate void UndoDeleteInspection(String InspectionName);

        #endregion

        #region Events


        
        public event UndoDeleteInspection OnUndoDeleteInspection;

        [UseInEvents(true)]
        public event CaptureDone OnCaptureDone;
        
        public event SelectedROIChanged OnSelectedROIChanged;
        public event ROIRemoved OnROIRemoved;

        [UseInEvents(true)]
        public event InspectionDone OnInspectionDone;

        [UseInEvents(true)]
        public event InspectionResult OnInspectionResultHandler;


        
        [UseInEvents(true)]
        public event CaptureStopped OnCaptureStopped;

        public event CameraSourceChanged OnCameraSourceChanged;

        //public event ROINameChanging OnROINameChanging;
        //public event ROINameChanged OnROINameChanged;
        public event InspectionNameChanged OnInspectionNameChanged;

        

        #endregion
        [XmlIgnore,Browsable(false)]
        public VisionProject SelectedProject = null;

        private int _UseIO =-1;
        [XmlAttribute]
        public int UseIO {
            get { return _UseIO; }
            set { _UseIO = value; }
        }

        private int _ID;
        private Boolean _active = true;
        private ROI _selectedroi = null;


       


        



        //[XmlIgnore]
        //[Browsable(false)]
        //public Object  = null;

        [XmlIgnore]
        public Layer InspLayer = null;

        
        #region properties

        public event PropertyChangedEventHandler PropertyChanged;


        readonly UndoRedo<int> _InspPos = new UndoRedo<int>(1);

        [XmlAttribute]
        [DisplayName("Position"), Browsable(false)]
        public int InspPos {
            get {
                return
                    _InspPos.Value;
            }
            set {
                if (_InspPos.Value != value) {
                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start("Inspection position changed to:" + value)) {
                            _InspPos.Value = value;
                            UndoRedoManager.Commit();
                        }
                    } else {
                            _InspPos.Value = value;
                    }
                }
            }
        }

        public class ROIS : UndoRedoList<ROI> {

            public List<ROI> AuxROIS = new List<ROI>();

        }

        [Browsable(false)]
        public ROIS ROIList { 
            get; 
            set; 
        }




        readonly UndoRedo<BaseCapture> _CaptureSource = new UndoRedo<BaseCapture>();

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Aquisition Settings"), Description("Capture Source Options"), DisplayName("Capture Source"), Browsable(true)]
        [EditorAttribute(typeof(InputSourceSelector), typeof(System.Drawing.Design.UITypeEditor))]
        public BaseCapture CaptureSource {
            get {
                return
                    _CaptureSource.Value;
            }
            set {
                if (_CaptureSource.Value != value && value != null) {

                    if (_CaptureSource.Value != null) {
                        _CaptureSource.Value.Dispose();
                    }

                    if(!UndoRedoManager.IsCommandStarted){
                        using (UndoRedoManager.Start(_inspectName.Value + ": Capture Source changed to:" + value.Camtype)) {
                            _CaptureSource.Value = value;
                            UndoRedoManager.Commit();
                        }
                        
                        

                    } else{
                        _CaptureSource.Value = value;
                    }

                    this.ROIList.AuxROIS.Clear();
                    switch (value.Camtype) {
                        case BaseCapture.CameraTypes.CV:
                            break;
                        case BaseCapture.CameraTypes.ICS:
                            CaptureSource.ChangeAttributeValue<BrowsableAttribute>("CameraName", "browsable", true);
                            break;
                        case BaseCapture.CameraTypes.File:
                            CaptureSource.ChangeAttributeValue<BrowsableAttribute>("CameraName", "browsable", false);
                            break;
                        case BaseCapture.CameraTypes.Inspection:
                            CaptureSource.ChangeAttributeValue<BrowsableAttribute>("CameraName", "browsable", false);
                           
                            break;
                        case BaseCapture.CameraTypes.Undef:
                            break;
                        default:
                            break;

                    }

                    if (OnCameraSourceChanged!=null) {
                        OnCameraSourceChanged(value);
                    }

                    
                }
            }
        }


        private Double _ImageRotation = 0;

        [XmlAttribute, DisplayName("Image Rotation"), Category("Aquisition Settings"), Description("Image rotation value")]
        public Double ImageRotation {
            get { return _ImageRotation; }
            set {
                if (_ImageRotation != value) {
                    _ImageRotation = value; 
                }
            }
        }


        [XmlIgnore]
        [Browsable(false)]
        public ROI SelectedROI {
            get {
                return _selectedroi;

            }

            set {
                if ((_selectedroi != value)) {
                    _selectedroi = value;
                    //Shape theshape= InspLayer.GetSelectableShapes().Find(name => name.Name == _selectedroi.Name);
                    //if (theshape!=null) {
                        
                    //}
                    if (OnSelectedROIChanged != null) {
                        OnSelectedROIChanged(_selectedroi);
                    }

                }
            }
        }



        readonly UndoRedo<String> _inspectName = new UndoRedo<string>("New inspection");

        [XmlAttribute]
        [Category("Common settings"), Description("Inspection name"), ReadOnly(true), Browsable(true)]
        public string Name {
            get { return _inspectName.Value; }
            set {
                if (_inspectName.Value != value) {
                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start("Inspection name changed to:" + value)) {
                            _inspectName.Value = value;
                            UndoRedoManager.Commit();
                        }
                    }
                    else {
                        _inspectName.Value = value;
                    }

                    
                    OnPropertyChanged("Name");
                    if (OnInspectionNameChanged != null) {
                        OnInspectionNameChanged(RequestName, _inspectName.Value, this);
                    }

                }

            }
        }


        [XmlAttribute]
        [Browsable(false)]
        public int ID {
            get { return _ID; }
            set { _ID = value; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public int RequestID {
            get;
            set;
        }


        String _requestname;
        [Browsable(false), XmlAttribute]
        public String RequestName {
            get {
                return _requestname;
            }
            set {
                if (_requestname != value) {
                    _requestname = value;
                    if (OnInspectionNameChanged != null) {
                        OnInspectionNameChanged(_requestname, _inspectName.Value, this);
                    }
                }
            }
        }


        [XmlAttribute]
        [Category("Common settings"), Description("Enable or disable inspection"), Browsable(true)]
        public Boolean Active {
            get {
                return _active;
            }
            set {
                _active = value;
                OnPropertyChanged("Active");

            }
        }



        public void SetLayers(Boolean State) {
            if (InspLayer != null) {
                InspLayer.Visible = State;
                InspLayer.Locked = !State;
            }

            
            if (CaptureSource is InspectionCapture) {
                InspectionCapture inspcap = CaptureSource as InspectionCapture;
                do {

                    if (inspcap.InspectionSource != null && inspcap.ShowAuxROIS) {
                        inspcap.InspectionSource.InspLayer.Visible = State;
                        inspcap.InspectionSource.InspLayer.Locked = State;
                        inspcap = inspcap.InspectionSource.CaptureSource as InspectionCapture;
                    } else {
                        break;
                    }

                } while (inspcap != null);
            }
                        
        }



        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        #region Methods


        //[XmlIgnore]
        //public object _ResultImagelocker = new object();

        //[XmlIgnore]
        //public object _OriginalImagelocker = new object();

        [XmlIgnore]
        static EventWaitHandle _waitOriginalHandle = new AutoResetEvent(false);

        [NonSerialized]
        Image<Bgr, Byte> _OriginalImageBgr= new Image<Bgr,byte>(0,0);
        [XmlIgnore, Browsable(false)]
        public Image<Bgr, Byte> OriginalImageBgr {
            get {


                return _OriginalImageBgr;

            }
            set {
               
                _OriginalImageBgr = value;

            }
        }

        [NonSerialized,Browsable(false)]
        public object ImageLocker= new object();

        [NonSerialized]
        Image<Bgr, Byte> _ResultImageBgr;
        [XmlIgnore, Browsable(false)]
        public Image<Bgr, Byte> ResultImageBgr {
            get {              
                
                return _ResultImageBgr;
             
            }
            set {
                
               
                  _ResultImageBgr = value;  
                
            }
        }




        public void CaptureImage(Object Sender) {

            try {


                if (UseIO>-1) {

                    if (PhidgetsIO.SendCommand("SET_OUT", new String[] { UseIO.ToString(), "ON" })) {
                        Thread.Sleep(25);
                    } 
                }

                lock (this.ImageLocker) {

                    
                    if (CaptureSource == null) {
                        throw new Exception("Capture source not defined");
                    }


                
                    
                    Image<Bgr, Byte> captured=null;

                    if (this.CaptureSource is PythonRemoteCapture) {
                        captured = ((PythonRemoteCapture)CaptureSource).GetImage(this.InspPos, true);
                    }
                    else {
                        captured=CaptureSource.GetImage();
                    }

                    if (ImageRotation != 0 && captured!=null) {
                        captured = captured.Rotate(ImageRotation, new Bgr(Color.White));
                    }

                    OriginalImageBgr.ROI = Rectangle.Empty;
                    

                    if (OriginalImageBgr.Size==captured.Size) {
                        captured.CopyTo(OriginalImageBgr);                        
                    }
                    else {
                        OriginalImageBgr = new Image<Bgr, byte>(captured.Size);
                        captured.CopyTo(OriginalImageBgr);                        
                    }

                    if (!(CaptureSource is InspectionCapture)) {
                        captured.Dispose();
                        captured = null;

                    }
                        

                }

                if (UseIO>-1) {
                    PhidgetsIO.SendCommand("SET_OUT", new String[] { UseIO.ToString(), "OFF" }); 
                }

                if (OnCaptureDone != null) {
                    OnCaptureDone(this, OriginalImageBgr);

                }



            } catch (Exception exp) {

                log.Error(this.Name, exp);

            }



        }




        public bool RemoveROI(ROI ROIToRemove) {
            try {
                if (ROIToRemove == null)
                    return false;
                
                int i = ROIList.IndexOf(ROIToRemove);
                using (UndoRedoManager.Start("ROI Removed: " + ROIToRemove.Name)) {
                    ROIList.Remove(ROIToRemove);
                    UndoRedoManager.Commit();
                }
                if (i - 1 >= 0)
                    SelectedROI = ROIList[i - 1];
                else
                    SelectedROI = null;
                
                if (OnROIRemoved!= null) {
                    OnROIRemoved(ROIToRemove);
                }
                return true;
            } catch (Exception exp) {
                log.Error(this.Name, exp);
                return false;
            }

        }

        private Boolean _InspectionOK = false;
        [XmlIgnore]
        [ReadOnly(true)]
        public Boolean InspectionOK {
            get { return _InspectionOK; }
            //set { _InspectionOK = value; }
        }

       

        public void ProcessSelectedROI() {

            lock (this.ImageLocker) {
                if ((SelectedROI != null && OriginalImageBgr != null)) {
                    OriginalImageBgr.ROI = Rectangle.Empty;
                    if (ResultImageBgr == null) {
                        ResultImageBgr = new Image<Bgr, byte>(OriginalImageBgr.ToBitmap());
                    }
                    ResultImageBgr.ROI = Rectangle.Empty;
                    OriginalImageBgr.CopyTo(ResultImageBgr);
                    SelectedROI.ProcessRoi(OriginalImageBgr, ResultImageBgr);
                    OriginalImageBgr.ROI = Rectangle.Empty;
                    ResultImageBgr.ROI = Rectangle.Empty;
                    if (OnInspectionDone!=null) {
                        OnInspectionDone(this);
                    }
                } 
            }


        }



        

        public void Execute(Object Sender,Boolean capture) {


            if (capture) {
                CaptureImage(Sender);
            } 




            ProcessInspection(Sender);
        }

        public void Execute(Image<Bgr, Byte> inputImage) {
            OriginalImageBgr = inputImage;
            ProcessInspection(null);
        }

        

        private void ProcessInspection(Object Sender) {

            Boolean inspres = false;
            

            if (OriginalImageBgr != null) {
                Console.WriteLine("Processing ROI");
                //RoiResults.Clear();
                OriginalImageBgr.ROI = Rectangle.Empty;
                ROI[] roilist = ROIList.Where(Pos => Pos.ROIPos > 0).OrderBy(bypos => bypos.ROIPos).ToArray();
                lock (ImageLocker) {

                    if (OriginalImageBgr != null) {



                        if (ResultImageBgr==null) {
                            ResultImageBgr = new Image<Bgr, byte>(OriginalImageBgr.Size); 
                        }
                        else if (ResultImageBgr.Size!=OriginalImageBgr.Size) {
                            ResultImageBgr = new Image<Bgr, byte>(OriginalImageBgr.Size); 
                        }
                        ResultImageBgr.ROI = Rectangle.Empty;
                        OriginalImageBgr.ROI = Rectangle.Empty;

                        OriginalImageBgr.CopyTo(ResultImageBgr);
                    }



                    for (int i = 0; i < roilist.Count(); i++) {



                        Boolean roi_pass = roilist[i].ProcessRoi(OriginalImageBgr, ResultImageBgr);


                        if (roi_pass && roilist[i].NoPartCheck) {

                            inspres = true;
                            break;


                        }
                    }



                }

                ROI[] roiauxlist = ROIList.AuxROIS.Where(Pos => Pos.ROIPos > 0).OrderBy(bypos => bypos.ROIPos).ToArray();
                lock (ImageLocker) {

                    

                    for (int i = 0; i < roiauxlist.Count(); i++) {



                        Boolean roi_pass = roiauxlist[i].ProcessRoi(OriginalImageBgr, ResultImageBgr);


                        if (roi_pass && roiauxlist[i].NoPartCheck) {

                            inspres = true;
                            break;


                        }
                    }



                }
                

                if (inspres) {
                    if (Sender is TCPServer) {
                        ((TCPServer)Sender).Client.Write("NOPART");
                    }                    
                }
                ResultImageBgr.ROI = Rectangle.Empty;

                


            } else {

            }


            //this.


            if (OnInspectionDone != null && !(Sender is InspectionCapture)) {

                OnInspectionDone(this);
            }


            if (OnCaptureStopped != null) {
                OnCaptureStopped();
            }

            //if (this.CaptureSource.LogImages>0) {

            //}

        }
         

       


       


        public ROI AddROI(Rectangle ROIRectangle) {
            try {
                String name = "";
                int i = 1;
                foreach (ROI _roi in ROIList) {
                    if (_roi.Name != ("ROI" + i))
                        name=("ROI" + i);
                        break;
                    i += 1;
                }

                ShapeRectangle _newshape = new ShapeRectangle();

                _newshape.Name = name;
                _newshape.BorderColor = Color.Green;
                _newshape.ShapeEfectiveBounds = ROIRectangle;

                if (InspLayer == null)
                    InspLayer = new Layer();

                InspLayer.Name = Name;
                //InspLayer.Visible = false;

                InspLayer.AddShape(_newshape);


                ROI _newroi = new ROI(_newshape, InspLayer, ROIList.Count+1);

                //_newroi.OnROIPositionChanged += new ROI.ROIPositionChanged(_newroi_OnROIPositionChanged);

                if(!UndoRedoManager.IsCommandStarted){
                    using (UndoRedoManager.Start("ROI added: " + _newroi.Name)) {
                        ROIList.Add(_newroi);
                        UndoRedoManager.Commit();

                    }
                } else {
                    ROIList.Add(_newroi);
                }
                               

                return _newroi;

            } catch (Exception exp) {
                log.Error(this.Name, exp);
                return null;
            }
        }

        //public void _newroi_OnROIPositionChanged(ROI roi,int OldPos, int NewPos) {
        //    var newlist = ROIList.OrderBy(bypos => bypos.ROIPos);
        //    var new2 = newlist.ToList();
        //    new2.RemoveAll(n => n.Name == roi.Name);

        //    if (NewPos > OldPos) {
        //        for (int i = OldPos - 1; i < NewPos - 1; i++) {
        //            new2[i].UpdatePosition(new2[i].ROIPos -1);
        //        }
        //    } else {

        //        for (int i = NewPos-1; i < OldPos-1 ; i++) {

        //            new2[i].UpdatePosition(new2[i].ROIPos + 1);

        //        }
        //    }

        //    //StaticObjects.
        //}





        public ROI GetROI(String ROIName) {
            foreach (ROI _item in ROIList) {
                if (_item.Name == ROIName)
                    return _item;
            }
            foreach (ROI _item in ROIList.AuxROIS) {
                if (_item.Name == ROIName)
                    return _item;
            }

            return null;
        }

        #endregion

        [XmlIgnore]
        [Browsable(false)]
        public Boolean DoNext = false;

       


        public void UpdateInspection() {
            if (InspLayer == null)
                InspLayer = new Layer();

            InspLayer.Name = Name;
            InspLayer.Visible = false;

        }


        //List<String> RoiResults = new List<string>();

       

        public class DistinctItemComparer : IEqualityComparer<Inspection> {

            public bool Equals(Inspection x, Inspection y) {
                return x.Name == y.Name;
            }

            public int GetHashCode(Inspection obj) {
                return obj.Name.GetHashCode();
            }
        }





        public Inspection(string name, int id) {
            _ID = id;
            Name = name;
            ROIList = new ROIS();
            
            
            UpdateInspection();
            
        }

        


        public Inspection() {
            Name = "New Inspection";
            InspPos = 0;
            
            _inspectName.OnMemberRedo += new UndoRedo<string>.MemberRedo(_inspectName_OnMemberRedo);
            _inspectName.OnMemberUndo += new UndoRedo<string>.MemberUndo(_inspectName_OnMemberRedo);

            //_Outputs = new SourceColection<OutputSource>();
            //_Outputs.SetName("Outputs Source List");

        }

        void _inspectName_OnMemberRedo(string UndoObject) {
            if (OnInspectionNameChanged != null) {
                OnInspectionNameChanged(RequestName, UndoObject, this);

            }     
        }

       


        public void ClearEvents() {
            
            OnInspectionDone = null;
            
            OnInspectionResultHandler = null;
            OnCaptureDone = null;
            OnSelectedROIChanged = null;
            OnROIRemoved = null;
            OnInspectionDone = null;
            OnInspectionNameChanged = null;
            
            OnCaptureStopped = null;
            OnCameraSourceChanged = null;

            

        }

        public void Dispose() {

            ClearEvents();

            if (OriginalImageBgr != null) {
                OriginalImageBgr = null;
            }
            if (ResultImageBgr != null) {                
                ResultImageBgr = null;

            }
            if (CaptureSource!=null) {
                CaptureSource.Dispose(); 
            }

            if (ROIList != null) {
                foreach (ROI _roi in ROIList) {
                    _roi.Dispose();
                }

            }
            if (UseIO>-1) {
                PhidgetsIO.SendCommand("SET_OUT", new String[] { UseIO.ToString(), "OFF" }); 
            }
            SelectedROI = null;
        
        }


        public override string ToString() {
            return this.Name;
        }

    }

    public class InspectionRequest : Request {

        int _ID=-1;
        [XmlAttribute]
        [Browsable(false)]
        public override int ID {
            get {
                return _ID;
            }
            set {
                _ID = value;
            }
        }

        

        public InspectionRequest() {

        }


    }
    [XmlInclude(typeof(InspectionRequest))]
    public class Request : IDisposable {

        private static KPPLogger log = new KPPLogger(typeof(Request));

        public delegate void RequestNameChanged(String RequestName);
        public delegate void RequestStart(Request request);
        public delegate void RequestDone(Request request,String Results);
        public delegate void SelectedInspectionChanged(Inspection NewSelectedInspection);
        public delegate void InspectionRemoved(Inspection inspection);
        public delegate void NewInspectionsList(InspectionsList list);

        public event InspectionRemoved OnInspectionRemoved;
        public event NewInspectionsList OnNewInspectionsList;

        public event RequestStart OnRequestStartHandler;
        
        public event RequestDone OnRequestDoneHandler;

        public event RequestNameChanged OnRequestNameChanged;        
        public event SelectedInspectionChanged OnSelectedInspectionChanged;

        //[XmlIgnore]
        //public Object FromClient = null;

        ResultsList _results = new ResultsList();
        [Browsable(false)]
        public ResultsList Results {
            get {
                return _results;
            }
            set {
                _results = value;
            }
        }

        private Boolean _requestOK = false;

        public void RemoveInspection(Inspection inspection) {
            using (UndoRedoManager.Start("Inspection removed:" + inspection.Name)) {

                Inspections.Remove(inspection);

                UndoRedoManager.Commit();
            }
            if (OnInspectionRemoved!=null) {
                OnInspectionRemoved(inspection);
            }
            inspection.Dispose();
        }

        private String ProcessResults() {
             StringBuilder sendstr = new StringBuilder();
            try {
                _requestOK = false;



                Boolean allok = true;
                sendstr.Clear();
                foreach (ResultInfo result in Results) {
                    
                    
                    sendstr.Append("RESULTS|" + ID + "|" + result.ID + "|");


                    foreach (ResultInput item in result.Inputs) {
                        if (item.Input!=null) {
                            item.Input.UpdateValue(); 
                        }
                        String strtemp=item.InputValue.Replace(".", result.DecimalSeparator);
                        strtemp = item.InputValue.Replace(",", result.DecimalSeparator);

                        sendstr.Append(item.Parameter + "=" +strtemp + "|");

                        double val = 0;
                        double.TryParse(item.InputValue,out val);
                        if (val < item.MinValue || val > item.MaxValue) {
                            allok = false;
                        }

                    }
                    sendstr.AppendLine("");
                    Console.WriteLine(sendstr);
                    _requestOK = allok;

                    
                }




            } catch (Exception exp) {
                
                Console.WriteLine(exp);
                log.Error(this.Name, exp);

                return "Results ERROR";
            }

            return sendstr.ToString();
        }



        public void AddInspection() {

            

            try {

                // this. 
                int id = -1;
                for (int i = 0; i < Inspections.Count + 1; i++) {
                    if (!Inspections.Exists(insp => insp.ID == i)) {
                        id = i;
                        break;
                    }
                }
                if (id == -1) {
                    return;
                }
                Inspection newInsp = new Inspection("NewInspection", id);
                newInsp.InspPos = id+1;
                newInsp.RequestName = this.Name;
                if (!UndoRedoManager.IsCommandStarted) {
                    using (UndoRedoManager.Start("new Inspection added:" + Name)) {
                        Inspections.Add(newInsp);
                        UndoRedoManager.Commit();
                    }
                    
                }



            } catch (Exception exp) {
                log.Error(this.Name, exp);

            }




        }


        private int m_ImageCol = 1;
        [XmlAttribute]
        public int ImageCol {
            get { return m_ImageCol; }
            set { m_ImageCol = value; }
        }

        private int m_ImageLine = 1;
        [XmlAttribute]
        public int ImageLine {
            get { return m_ImageLine; }
            set { m_ImageLine = value; }
        }


        int _ID = -1;

        [XmlAttribute]
        [Browsable(false)]
        public virtual int ID {
            get { return _ID; }
            set { _ID = value; }
        }

        readonly UndoRedo<String> _name = new UndoRedo<string>("New Request");
        [XmlAttribute]
        public string Name {
            get { return _name.Value; }
            set {
                if (_name.Value != value) {

                    if (!UndoRedoManager.IsCommandStarted) {
                        using (UndoRedoManager.Start("Request name changed:" + _name)) {
                            _name.Value = value;
                            UndoRedoManager.Commit();
                        }
                    } else {
                        _name.Value = value;
                    }
                    
                    if (OnRequestNameChanged != null) {
                        OnRequestNameChanged(_name.Value);

                    }

                }

            }
        }

      

        public override string ToString() {
            return Name;
        }

        

        public class InspectionsList : UndoRedoList<Inspection> {


        }

        private InspectionsList _Inspections = new InspectionsList();

        public InspectionsList Inspections {
            get { return _Inspections; }
            set {
                if (_Inspections!=value) {
                    _Inspections = value;
                    if (OnNewInspectionsList!=null) {
                        OnNewInspectionsList(value);
                    }
                }
            }
        }
        

       // Thread RequestThread = null;

        public Boolean ProcessRequest(Object Sender,Boolean capture,Request CallingRequest) {

            //this.FromClient = fromClient;

            //OnRequestStart(this);
            
            try {
                foreach (ResultInfo result in Results) {

                    foreach (ResultInput item in result.Inputs) {
                        if (item.Input != null) {
                            item.Input.UpdateValue(true);
                        }

                    }
                }


                if (Sender is TCPServer) {
                    ((TCPServer)Sender).Client.Write("REQUEST OK|" + this.ID); 
                }

                var orderedinspections = from thereq in this.Inspections
                                         orderby thereq.InspPos ascending
                                         select thereq;

                for (int i = 0; i < orderedinspections.Count(); i++) {
                    Inspection _inspect = orderedinspections.ElementAt(i);
                    if (_inspect != null) {
                        //_inspect.TCPClient = FromClient;
                        _inspect.Execute(Sender,capture);
                    }
                }


            

            } catch (Exception exp) {
                log.Error(this.Name, exp);
                Console.WriteLine(exp);

                return false;
            }

            

            if (Sender is TCPServer) {
                
                ((TCPServer)Sender).Client.Write(ProcessResults());                

            }
          //  OnRequestDone(this, ProcessResults());

            return true;
           
        }


        public Inspection Add(string name, String requestname,int reqid, int _id, int Pos) {



            Inspection _newinspect;
            using (UndoRedoManager.StartInvisible("Init")) {
                _newinspect = new Inspection(name, _id);
                _newinspect.InspPos = Pos;
                _newinspect.RequestID = reqid;
                UndoRedoManager.Commit();
            }
            _newinspect.RequestName = requestname;
            if(!UndoRedoManager.IsCommandStarted){
                using (UndoRedoManager.Start("Inspection added:" + name+_id.ToString())) {
                    Inspections.Add(_newinspect);
                    UndoRedoManager.Commit();

                }
            } else {
                Inspections.Add(_newinspect);
            }
            

            return _newinspect;
        }


       

        Inspection _SelectedInspection;
        [Browsable(false),XmlIgnore]
        public Inspection SelectedInspection {
            get {
                return _SelectedInspection;
            }
            set {
                
                if (_SelectedInspection!=value) {
                    if ( _SelectedInspection!=null) {
                        if (value!=null) {
                            if (value.CaptureSource != null) {
                                //value.CaptureSource.UpdateAttributes();
                            } 
                        }

                        _SelectedInspection.SetLayers(false);  
                        
                    }
                    _SelectedInspection = value;
                    if (_SelectedInspection!=null) {
                        _SelectedInspection.SetLayers(true);                          
                    }
                    
                    if (OnSelectedInspectionChanged!=null) {
                        OnSelectedInspectionChanged(_SelectedInspection);
                    }
                    
                }
            }
        }

        public Request(String name) {
            Inspections = new InspectionsList();
            _name = new UndoRedo<string>(name);
            _name.OnMemberRedo += new UndoRedo<string>.MemberRedo(_name_OnMemberRedo);
            _name.OnMemberUndo += new UndoRedo<string>.MemberUndo(_name_OnMemberRedo);

        }


        public Request() {

           
            Inspections = new InspectionsList();
            _name = new UndoRedo<string>("New Request");
            _name.OnMemberRedo += new UndoRedo<string>.MemberRedo(_name_OnMemberRedo);
            _name.OnMemberUndo += new UndoRedo<string>.MemberUndo(_name_OnMemberRedo);

        }

        void _Removed_OnMemberUndo(string UndoObject) {
            
        }

        void _name_OnMemberRedo(string UndoObject) {
            if (OnRequestNameChanged != null) {
                OnRequestNameChanged(UndoObject);

            }  
        }

        public void Dispose() {

            foreach (Inspection insp in Inspections) {
                insp.SelectedROI = null;
                insp.Dispose();
            }
            SelectedInspection = null;
            OnRequestNameChanged = null;
            OnSelectedInspectionChanged = null;
            OnRequestDoneHandler = null;
            OnRequestStartHandler = null;

            
        }

    }
    
}




