using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Xml.Serialization;
using System.ComponentModel;
using KPP.Core.Debug;
using KPP.Controls.Winforms.ImageEditorObjs;
using Emgu.CV.VideoSurveillance;
using Emgu.CV.Cvb;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using KPPAutomationCore;
using AForge.Imaging;
using AForge.Math.Geometry;
using System.Drawing.Imaging;
using AForge;



namespace VisionModule {



    [TypeConverter(typeof(Customconverter<BlobInfo>))]
    public class BlobInfo {


        private String _ID = "No ID";
        [DisplayName("Blob ID")]
        [UseInRef(true), AllowDrag(true), UseInResultInput(true)]
        public String ID {
            get {
                return _ID.ToString();
            }
        }

        private PointF _Center = new PointF(0, 0);
        [DisplayName("Blob Center"), UseInRef(true), AllowDrag(true), UseInResultInput(true)]
        public PointF Center {
            get {
                return _Center;
            }
        }

        private Double _Area = 0;
        [UseInRef(true), AllowDrag(true), UseInResultInput(true)]
        public Double Area {
            get { return _Area; }

        }

        private Double _Angle = -1;
        [UseInRef(true), AllowDrag(true), UseInResultInput(true)]
        public Double Angle {
            get { return _Angle; }
        }

        private Rectangle _BoundingBox = Rectangle.Empty;
        [DisplayName("Bounding Box")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Rectangle BoundingBox {
            get {
                return _BoundingBox;
            }

        }


        private Double _PixelAverage;
        [XmlIgnore]
        [UseInResultInput(true), DisplayName("Pixel Average"), Category("Post-Processing"), Description("Pixel Average inside contour"), ReadOnly(true)]
        public Double PixelAverage {
            get { return _PixelAverage; }

        }


        private Double _PixelMinVal;
        [XmlIgnore]
        [UseInResultInput(true), DisplayName("Pixel Min Value"), Category("Post-Processing"), Description("Pixel Min Value inside contour"), ReadOnly(true)]
        public Double PixelMinVal {
            get { return _PixelMinVal; }
        }


        private Double _PixelMaxVal;
        [XmlIgnore]
        [UseInResultInput(true), DisplayName("Pixel Max Value"), Category("Post-Processing"), Description("Pixel Max Value inside contour"), ReadOnly(true),]
        public Double PixelMaxVal {
            get { return _PixelMaxVal; }

        }


        private Double _PixelCount;
        [XmlIgnore]
        [UseInResultInput(true), DisplayName("Pixel Count"), Category("Post-Processing"), Description("Pixel Count above average inside contour"), ReadOnly(true)]
        public Double PixelCount {
            get { return _PixelCount; }

        }



        private double _PixelDeviation;
        [XmlIgnore]
        [UseInResultInput(true), DisplayName("Pixel Deviation"), Category("Post-Processing"), Description("Pixel Standard Deviation inside contour"), ReadOnly(true)]
        public double PixelDeviation {
            get { return _PixelDeviation; }
            set { _PixelDeviation = value; }
        }

        private CvBlob _Blob;
        [XmlIgnore, Browsable(false)]
        public CvBlob Blob {
            get { return _Blob; }

        }

        public BlobInfo(CvBlob blob, Rectangle RoiRegion, ProcessingFunctionPixelanalysis pixelanalisys) {

            _Blob = blob;
        
            _Area = blob.Area;
            _ID = blob.Label.ToString();
            //Translate

            _Center = new PointF(blob.Centroid.X + RoiRegion.X, blob.Centroid.Y + RoiRegion.Y);
            _BoundingBox = blob.BoundingBox;


            using (MemStorage storage = new MemStorage()) {

                Contour<System.Drawing.Point> blobcontour = blob.GetContour(storage);
                MCvBox2D box = blobcontour.GetMinAreaRect();
                _Angle = 0 - Math.Round(box.angle, 3);
            }

            _PixelDeviation = pixelanalisys.PixelDeviation;
            _PixelCount = pixelanalisys.PixelCount;
            _PixelMaxVal = pixelanalisys.PixelMinVal;
            _PixelMinVal = pixelanalisys.PixelMaxVal;
            _PixelAverage = pixelanalisys.PixelAverage;

        }

        public override string ToString() {
            return "Blob";
        }
        public BlobInfo() {

        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BlobConstraint {



        private ShapeType _Shape = ShapeType.Unknown;
        [XmlAttribute]
        [Category("Pre-Processing"), Description("Filter shapes"), DisplayName("Shape")]
        public ShapeType Shape {
            get { return _Shape; }
            set { _Shape = value; }
        }

     

        [XmlAttribute]
        [Category("Pre-Processing"), Description("Acceptable shape distortion (0-100%)"), DisplayName("Distortion")]
        public float ShapeDistortion { get; set; }


        [XmlAttribute]
        [Category("Pre-Processing"), Description("Min blob area"), DisplayName("Min Blob Area")]
        public int MinBlobArea { get; set; }

        [XmlAttribute]
        [Category("Pre-Processing"), Description("Max blob area"), DisplayName("Max Blob Area")]
        public int MaxBlobArea { get; set; }


        [XmlAttribute]
        [Category("Pre-Processing"), Description("Min blob width"), DisplayName("Min blob width")]
        public int MinBlobWidth { get; set; }

        [XmlAttribute]
        [Category("Pre-Processing"), Description("Max blob width"), DisplayName("Max blob width")]
        public int MaxBlobWidth { get; set; }

        [XmlAttribute]
        [Category("Pre-Processing"), Description("Min blob heigth"), DisplayName("Min blob heigth")]
        public int MinBlobHeigth { get; set; }

        [XmlAttribute]
        [Category("Pre-Processing"), Description("Max blob heigth"), DisplayName("Max blob heigth")]
        public int MaxBlobHeigth { get; set; }

        ProcessingFunctionPixelanalysis _PixelAnalisys = new ProcessingFunctionPixelanalysis();
        [Category("Pre-Processing"), DisplayName("Pixel Analisys")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ProcessingFunctionPixelanalysis PixelAnalisys {
            get { return _PixelAnalisys; }
            set { _PixelAnalisys = value; }
        }




        public BlobConstraint() {

        }
    }

    [ProcessingFunction("Blob Analysis", "Area")]
    public class BlobAnalysis : ProcessingFunctionBase {

        private static KPPLogger log = new KPPLogger(typeof(BlobAnalysis));

        public BlobAnalysis() {


        }


        //= new KPPLogger(typeof(ProcessingFunctionBoundingRectangle));


        #region Pre-Processing

        [XmlIgnore, Browsable(false)]
        public virtual ContourPreProc ContourPreProc1 { get; set; }

        private BlobConstraint _BlobSettings = new BlobConstraint();

        [Category("Pre-Processing"), DisplayName("Blob Settings")]
        public BlobConstraint BlobSettings {
            get {
                return _BlobSettings;
            }

            set {
                if (value != _BlobSettings) {
                    _BlobSettings = value;
                }
            }
        }



        #endregion



        #region Post Processing





        [XmlIgnore]
        [UseInResultInput(true), AllowDrag(true), Category("Post-Processing"), Description("Number of blobs found"), DisplayName("Number Blobs"), ReadOnly(true), Browsable(true)]
        public Double NumBlobs { get; set; }


        private CustomCollection<BlobInfo> _BlobsList = new CustomCollection<BlobInfo>();
        [Category("Post-Processing"), TypeConverter(typeof(ExpandableObjectConverter)), XmlIgnore, ReadOnly(true), DisplayName("Blobs List")]
        public CustomCollection<BlobInfo> BlobsList {
            get { return _BlobsList; }
            set { _BlobsList = value; }
        }



        #endregion

        public override void UpdateRegionToHighligth(object Region) {
            if (Region is BlobInfo) {
                BlobInfo blobinf = Region as BlobInfo;
                base.IdentRegion = new Image<Gray, byte>(base.BaseRoi.Size);
                base.IdentRegion.Draw(blobinf.BoundingBox, new Gray(255), 2);
            }
        }

        public override Boolean Process(Image<Bgr, byte> ImageIn, Image<Bgr, byte> ImageOut, Rectangle RoiRegion) {
            try {
                base.Process(ImageIn, ImageOut, RoiRegion);
                Pass = false;
                //ReferenceBlob = new BlobInfo();


                NumBlobs = 0;
                BlobsList.Clear();

                Image<Bgr, byte> roiImage = new Image<Bgr, byte>(RoiRegion.Size);
                ImageIn.ROI = RoiRegion;
                ImageOut.ROI = RoiRegion;
                ImageIn.CopyTo(roiImage);



                try {

                    Image<Gray, Byte> grayimage = new Image<Gray, Byte>(roiImage.Size);
                    Image<Gray, Byte> originalgrayimage = new Image<Gray, Byte>(roiImage.Size);



                    switch (ImagePreProc1.UseChannel) {
                        case Channel.Bgr:
                            break;
                        case Channel.Red:
                            grayimage = roiImage[0];
                            break;
                        case Channel.Green:
                            grayimage = roiImage[1];
                            break;
                        case Channel.Blue:
                            grayimage = roiImage[2];
                            break;
                        case Channel.Mono:
                            CvInvoke.cvCvtColor(roiImage, grayimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_BGR2GRAY);
                            break;
                        default:
                            CvInvoke.cvCvtColor(roiImage, grayimage, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_BGR2GRAY);
                            break;
                    }



                    grayimage.CopyTo(originalgrayimage);


                    //Grayimage.ROI = Rectangle.Empty;


                    //Grayimage.CopyTo(ThresholdImage);
                    grayimage._Erode(ImagePreProc1.Erode);
                    grayimage._Dilate(ImagePreProc1.Dilate);

                    if (base.ImagePreProc1.UseChannel != Channel.Bgr) {
                        switch (base.ImagePreProc1.ThresholdType) {
                            case TypeOfThreshold.Normal:
                                grayimage._ThresholdBinary(new Gray(base.ImagePreProc1.Threshold), new Gray(255));

                                break;
                            case TypeOfThreshold.Inverted:
                                grayimage._ThresholdBinaryInv(new Gray(base.ImagePreProc1.Threshold), new Gray(255));

                                break;
                            case TypeOfThreshold.Adaptive:

                                CvInvoke.cvCanny(grayimage, grayimage, base.ImagePreProc1.Threshold, base.ImagePreProc1.ThresholdLink, base.ImagePreProc1.ApertureSize);

                                break;
                            default:
                                break;
                        }

                        try {

                            #region AFORGE
                            //    Bitmap image= grayimage.ToBitmap();
                            //  BlobCounter blobCounter = new BlobCounter();
                            //// process input image
                            //blobCounter.ProcessImage(image);
                            //// get information about detected objects
                            //Blob[] blobs2 = blobCounter.GetObjectsInformation();
                            //SimpleShapeChecker checkshape = new SimpleShapeChecker();
                            //foreach (Blob item in blobs2) {
                            //    List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(item);
                            //    if (edgePoints.Count<10) {
                            //        continue;
                            //    }
                            //    checkshape.RelativeDistortionLimit = 0.1f;
                            //    checkshape.MinAcceptableDistortion= 0.7f;
                            //    ShapeType shape=checkshape.CheckShapeType(edgePoints);
                                
                            //    if (shape== ShapeType.Circle) {
                            //        ImageOut.Draw(item.Rectangle, new Bgr(Color.Green),2);
                            //    }
                            //}

                            
                            
                            #endregion





                            CvBlobDetector blobdetector = new CvBlobDetector();
                            CvBlobs blobs = new CvBlobs();


                            blobdetector.Detect(grayimage, blobs);

                            if (BlobSettings.MinBlobArea != 0 || BlobSettings.MaxBlobArea != 0) {
                                blobs.FilterByArea(BlobSettings.MinBlobArea, BlobSettings.MaxBlobArea);
                            }

                            

                            List<CvBlob> blobarray = blobs.Values.ToList();

                            //List<CvBlob> newbloblist = new List<CvBlob>();

                            foreach (CvBlob blob in blobarray) {
                                if (blob.BoundingBox.Left != grayimage.ROI.Left && blob.BoundingBox.Top != grayimage.ROI.Top && blob.BoundingBox.Right != grayimage.ROI.Right && blob.BoundingBox.Bottom != grayimage.ROI.Bottom) {
                                    if (blob.BoundingBox.Width > BlobSettings.MinBlobWidth && blob.BoundingBox.Width < BlobSettings.MaxBlobWidth &&
                                        blob.BoundingBox.Height > BlobSettings.MinBlobHeigth && blob.BoundingBox.Height < BlobSettings.MaxBlobHeigth) {
                                        //newbloblist.Add(blob);
                                        Image<Gray, Byte> BlobImage = new Image<Gray, byte>(blob.BoundingBox.Size);
                                        originalgrayimage.ROI = blob.BoundingBox;
                                        originalgrayimage.CopyTo(BlobImage);
                                        Boolean goodblob = false;

                                        Image<Gray, Byte> mask = new Image<Gray, byte>(grayimage.Size);
                                        using (MemStorage mem = new MemStorage()) {

                                            Contour<System.Drawing.Point> ctr = blob.GetContour(mem);

                                            mask.Draw(ctr, new Gray(255), -1);
                                            //mask.Erode(2);
                                            mask.ROI = blob.BoundingBox;
                                            Image<Gray, Byte> Finalmask = new Image<Gray, byte>(blob.BoundingBox.Size);
                                            mask.CopyTo(Finalmask);

                                            BlobSettings.PixelAnalisys.MaskImage = Finalmask;




                                            switch (BlobSettings.Shape) {
                                                case ShapeType.Circle:
                                                    List<IntPoint> edgePoints = new List<IntPoint>();
                                                    foreach (System.Drawing.Point item in ctr) {
                                                        edgePoints.Add(new IntPoint(item.X, item.Y));
                                                    }

                                                    if (edgePoints.Count < 10) {
                                                        continue;
                                                    }
                                                    SimpleShapeChecker checkshape = new SimpleShapeChecker();
                                                    checkshape.RelativeDistortionLimit = BlobSettings.ShapeDistortion/100;
                                                    

                                                    ShapeType shape = checkshape.CheckShapeType(edgePoints);

                                                    if (shape == ShapeType.Circle) {
                                                        ImageOut.Draw(blob.BoundingBox, new Bgr(Color.Green), 2);

                                                        Double CircleArea = checkshape.Radius * checkshape.Radius * Math.PI;
                                                        Double ContourArea = ctr.Area;

                                                        Double dist = checkshape.MeanDistance;
                                                        

                                                        goodblob = true;
                                                    }


                                                    break;
                                                case ShapeType.Quadrilateral:
                                                    break;
                                                case ShapeType.Triangle:
                                                    break;
                                                default:
                                                    goodblob = true;
                                                    break;
                                            }                                            



                                            if (goodblob) {
                                                BlobSettings.PixelAnalisys.Process(new Image<Bgr, byte>(BlobImage.ToBitmap()), new Image<Bgr, byte>(BlobImage.ToBitmap()), Rectangle.Empty);
                                                BlobsList.Add(new BlobInfo(blob, RoiRegion, BlobSettings.PixelAnalisys));
                                            }
                                        }
                                    }



                                }


                            }

                            var ordered = from v in BlobsList.ToList()
                                          orderby v.Area descending
                                          select v;

                            var orderedList = ordered.ToList();



                            BlobsList.Clear();

                            for (int i = 0; i < orderedList.Count; i++) {
                                BlobsList.Add(orderedList[i]);
                            }

                            //if (BlobsList.Count>0) {
                            //    ReferenceBlob = BlobsList[0];
                            //}

                            if (ResultsInROI == OutputResultType.orResults) {

                                foreach (BlobInfo blobinf in BlobsList) {
                                    ImageOut.Draw(blobinf.BoundingBox, new Bgr(Color.Green), 1);
                                    ImageOut.Draw(new Cross2DF(blobinf.Center, 3, 3), new Bgr(Color.Green), 1);
                                }
                            }
                            else if (ResultsInROI == OutputResultType.orContours) {
                                foreach (BlobInfo blobinf in BlobsList) {
                                    using (MemStorage contourstorage = new MemStorage()) {
                                        Contour<System.Drawing.Point> contour = blobinf.Blob.GetContour(contourstorage);
                                        ImageOut.Draw(contour, new Bgr(Color.Yellow), 1);
                                        ImageOut.Draw(new Cross2DF(blobinf.Blob.Centroid, 3, 3), new Bgr(Color.Yellow), 1);
                                    }
                                }
                            }
                            else if (ResultsInROI == OutputResultType.orPreProcessing) {
                                ImageOut.ROI = RoiRegion;
                                CvInvoke.cvCvtColor(grayimage, ImageOut, Emgu.CV.CvEnum.COLOR_CONVERSION.CV_GRAY2BGR);
                                ImageOut.ROI = Rectangle.Empty;
                            }



                            NumBlobs = BlobsList.Count;
                        }
                        catch (DllNotFoundException exp) {
                            log.Error(exp);
                            return false;
                        }


                        grayimage.Dispose();
                        originalgrayimage.Dispose();

                    }
                }
                catch (Exception exp) {

                    log.Error(this.FunctionName, exp);
                }




                roiImage.Dispose();


            }
            catch (Exception exp) {
                Console.WriteLine(exp);
                log.Error(exp);
            }




            return Pass;


        }


    }

}
