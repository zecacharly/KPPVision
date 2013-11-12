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
using System.Numerics;
using System.Drawing.Design;
using System.Reflection;



namespace VisionModule {


    [ProcessingFunction("Constant", "Math")]
    public class ProcessingFunctionConstant : ProcessingFunctionBase {

        private static KPPLogger log;



        #region Pre-Processing

        ResultReference _Value1;
        [AcceptDrop(true)]
        [Category("Pre-Processing"), Description("Constant Value"), DisplayName("Constant Value"), AcceptType(typeof(Double))]
        public ResultReference Value1 {
            get {
                return _Value1;
            }
            set {
                _Value1 = value;
            }
        }



        [TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Contour"), Category("Pre-Processing"), Browsable(false)]
        [XmlIgnore]
        public override ContourPreProc ContourPreProc1 {
            get;
            set;
        }




        [TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Image"), Category("Pre-Processing"), Browsable(false)]
        [XmlIgnore]
        public override ImagePreProc ImagePreProc1 {
            get;
            set;
        }




        #endregion



        #region Post Processing


        private ResultReference _K;
        [XmlIgnore]
        [DisplayName("Out"), Category("Post-Processing"), Description("Sum value"), ReadOnly(true)]
        //[TypeConverter(typeof(ExpandableObjectConverter))]
        [AllowDrag(true), UseInRef(true), UseInResultInput(true), AcceptType(typeof(Double))]
        public ResultReference Out {
            get { return _K; }
            set { _K = value; }
        }


        #endregion



        public override bool Process(Image<Bgr, byte> ImageIn, Image<Bgr, byte> ImageOut, Rectangle RoiRegion) {
            try {
                base.Process(ImageIn, ImageOut, RoiRegion);
                Pass = false;

                if (Value1 == null || Out == null) {
                    return Pass;
                }
                Value1.UpdateValue();
            

                if (Value1.ResultOutput is Double) {

                }
                else {
                    return Pass;
                }

                //Double value1 = (Double)Value1.ResultOutput;
                //Double value2 = (Double)Value2.ResultOutput;

                Out.ResultOutput = Math.Round((Double)Value1.ResultOutput, 2);

                //PointF point3 = (PointF)Point3.ReferenceValue;







            } catch (Exception exp) {

                log.Error(exp);
            }



            return Pass;


        }

        public ProcessingFunctionConstant() {
            log= new KPPLogger(typeof(ProcessingFunctionConstant),name:base.ModuleName);
        }

    }


     [ProcessingFunction("Sum", "Math")]
    public class ProcessingFunctionSum: ProcessingFunctionBase {

         public ProcessingFunctionSum() {

             log= new KPPLogger(typeof(ProcessingFunctionSum),name:base.ModuleName);
         }

         private static KPPLogger log;
         //= new KPPLogger(typeof(ProcessingFunctionSum));

       

        #region Pre-Processing

        ResultReference _Value1;
        [AcceptDrop(true)]
        [Category("Pre-Processing"), Description("Value 1"), DisplayName("Value 1"), AcceptType(typeof(Double))]
        public ResultReference Value1 { 
            get{
                return _Value1;
            }
            set {
                _Value1 = value;
            }
        }


        ResultReference _Value2;
        [AcceptDrop(true)]
        [Category("Pre-Processing"), Description("Value 2"), DisplayName("Value 2"), AcceptType(typeof(Double))]
        public ResultReference Value2 {
            get {
                return _Value2;
            }
            set {
                _Value2 = value;
            }
        }

       

        [TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Contour"), Category("Pre-Processing"), Browsable(false)]
        [XmlIgnore]        
        public override ContourPreProc ContourPreProc1 {
            get;
            set;
        }




        [TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Image"), Category("Pre-Processing"), Browsable(false)]
        [XmlIgnore]        
        public override ImagePreProc ImagePreProc1 {
            get;
            set;
        }




        #endregion



        #region Post Processing


        private Double _Sum;
        [XmlIgnore]
        [DisplayName("Sum"), Category("Post-Processing"), Description("Sum value"), ReadOnly(true)]
        //[TypeConverter(typeof(ExpandableObjectConverter))]
        [AllowDrag(true),UseInRef(true), UseInResultInput(true)]
        public Double Sum {
            get { return _Sum; }
            set { _Sum= value; }
        }

        
        #endregion



        public override bool Process(Image<Bgr, byte> ImageIn, Image<Bgr, byte> ImageOut, Rectangle RoiRegion) {
            try {
                base.Process(ImageIn, ImageOut, RoiRegion);
                Pass = false;
                
                if (Value1 == null || Value2== null) {
                    return Pass;
                }
                Value1.UpdateValue();
                Value2.UpdateValue();

                if (Value1.ResultOutput is Double &&  Value2.ResultOutput is Double) {
                    
                } else {
                    return Pass;
                }

                //Double value1 = (Double)Value1.ResultOutput;
                //Double value2 = (Double)Value2.ResultOutput;

                Sum = Math.Round((Double)Value1.ResultOutput + (Double)Value2.ResultOutput,2);
                
                //PointF point3 = (PointF)Point3.ReferenceValue;


                



                 
            } catch (Exception exp) {
                
                log.Error( exp);
            }



            return Pass;


        }


    }

     [ProcessingFunction("Sub", "Math")]
     public class ProcessingFunctionSub : ProcessingFunctionBase {

         public ProcessingFunctionSub() {

             log= new KPPLogger(typeof(ProcessingFunctionSub),name:base.ModuleName);
         }

         private static KPPLogger log;
         //= new KPPLogger(typeof(ProcessingFunctionSub));



         #region Pre-Processing

         ResultReference _Value1;
         [AcceptDrop(true)]
         [Category("Pre-Processing"), Description("Value 1"), DisplayName("Value 1"), AcceptType(typeof(Double))]
         public ResultReference Value1 {
             get {
                 return _Value1;
             }
             set {
                 _Value1 = value;
             }
         }


         ResultReference _Value2;
         [AcceptDrop(true)]
         [Category("Pre-Processing"), Description("Value 2"), DisplayName("Value 2"), AcceptType(typeof(Double))]
         public ResultReference Value2 {
             get {
                 return _Value2;
             }
             set {
                 _Value2 = value;
             }
         }

         //private ProcessingCondition _ProcessCondition = new ProcessingCondition();
         //[Category("Pre-Processing"), DisplayName("Condition"), Browsable(true)]
         //public override ProcessingCondition ProcessCondition {
         //    get { 
         //        return _ProcessCondition; 
         //    }
         //    set { 
         //        _ProcessCondition = value;
         //        if (_ProcessCondition.Input1==null) {
         //            _ProcessCondition.Input1 = (Double)0.0;
         //        }
         //        if (_ProcessCondition.Input2 == null) {
         //            _ProcessCondition.Input2 = (Double)0.0;
         //        }
         //    }
         //}


         [TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Contour"), Category("Pre-Processing"), Browsable(false)]
         [XmlIgnore]
         public override ContourPreProc ContourPreProc1 {
             get;
             set;
         }




         [TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Image"), Category("Pre-Processing"), Browsable(false)]
         [XmlIgnore]
         public override ImagePreProc ImagePreProc1 {
             get;
             set;
         }




         #endregion



         #region Post Processing


         private Double _Sub;
         [XmlIgnore]
         [DisplayName("Sub"), Category("Post-Processing"), Description("Sub value"), ReadOnly(true)]
         //[TypeConverter(typeof(ExpandableObjectConverter))]
         [AllowDrag(true), UseInRef(true), UseInResultInput(true)]
         public Double Sub {
             get { return _Sub; }
             set { _Sub = value; }
         }


         #endregion

         public override bool Process(Image<Bgr, byte> ImageIn, Image<Bgr, byte> ImageOut, Rectangle RoiRegion) {
             try {
                 Pass = false;
                 base.Process(ImageIn, ImageOut, RoiRegion);
                 if (Value1 == null || Value2 == null) {
                     return Pass;
                 }
                 Value1.UpdateValue();
                 Value2.UpdateValue();

                 if (Value1.ResultOutput is Double && Value2.ResultOutput is Double) {
                     
                 } else {
                     return Pass;

                 }

                 //if (ProcessCondition!="") {
                 //    "If(Value1>123)"
                 //}
                 //Boolean conditionvalid = false;

                 //try {
                 //    switch (ProcessCondition.Condition) {
                 //        case TestConditions.Equal:
                 //            break;
                 //        case TestConditions.Greater:

                 //            Double val1=0;
                 //            Double val2=0;

                 //            if (ProcessCondition.Input1 is String) {
                 //                PropertyInfo propinf = this.GetType().GetProperty((String)ProcessCondition.Input1);
                 //                if (propinf != null) {
                 //                    ResultReference resref = (ResultReference)propinf.GetValue(this, null);
                 //                    val1 = (Double)resref.ResultOutput;
                 //                    conditionvalid = true;
                 //                } else {
                 //                    var converter = TypeDescriptor.GetConverter(typeof(Double));
                 //                    if (converter.CanConvertTo(typeof(Double))) {
                 //                        val1 = (Double)converter.ConvertTo(ProcessCondition.Input1, typeof(Double));
                 //                        conditionvalid = true;
                 //                    }
                 //                }
                 //            }
                 //            if (conditionvalid) {
                 //                conditionvalid = false;
                                
                 //                if (ProcessCondition.Input2 is String) {
                 //                    PropertyInfo propinf = this.GetType().GetProperty((String)ProcessCondition.Input2);
                 //                    if (propinf != null) {
                 //                        ResultReference resref = (ResultReference)propinf.GetValue(this, null);
                 //                        val2 = (Double)resref.ResultOutput;
                 //                        conditionvalid = true;
                 //                    } else {
                 //                        var converter = TypeDescriptor.GetConverter(typeof(Double));
                 //                        if (converter.CanConvertTo(typeof(Double))) {
                 //                            val2 = (Double)converter.ConvertTo(ProcessCondition.Input2, typeof(Double));
                 //                            conditionvalid = true;
                 //                        }
                 //                    }
                 //                }
                 //            }

                 //            if (conditionvalid) {
                 //                if (val1>val2) {
                 //                    if (ProcessCondition.Process!="") {
                 //                        String[] processstrings = ProcessCondition.Process.Split(new String[] { "=" }, StringSplitOptions.None);

                 //                        if (processstrings.Count()==2) {
                 //                            PropertyInfo propinf = this.GetType().GetProperty((String)processstrings[0]);
                 //                            if (propinf != null) {
                 //                               ResultReference resref = (ResultReference)propinf.GetValue(this, null);
                                                 
                 //                                String[] setstrings = ProcessCondition.Process.Split(new String[] { "-" }, StringSplitOptions.None);
                 //                                if (setstrings.Count()==2){

	
                 //                               PropertyInfo propinf2 = this.GetType().GetProperty((String)strs2[1]);
                 //                            if (propinf != null) {
                 //                               resref.ResultOutput;
                                         
                                     
                 //                            }    }

                 //                        //if (strs.Count()>) {
                                             
                 //                        //}
                 //                    }
                 //                }
                 //            }



                 //            break;
                 //        case TestConditions.Less:
                 //            break;
                 //        default:
                 //            break;
                 //    }
                 //} catch (Exception exp) {

                 //    log.Error(exp);
                 //}

                 Sub = (Double)Value1.ResultOutput - (Double)Value2.ResultOutput;
                 Sub = Math.Round(Sub, 2);




             } catch (Exception exp) {
                 
                 log.Error(this.FunctionName,exp);
             }



             return false;


         }


     }

     [ProcessingFunction("Division", "Math")]
     public class ProcessingFunctionDivision : ProcessingFunctionBase {


         public ProcessingFunctionDivision() {

             log= new KPPLogger(typeof(ProcessingFunctionDivision),name:base.ModuleName);
         }

         private static KPPLogger log;
         //= new KPPLogger(typeof(ProcessingFunctionDivision));



         #region Pre-Processing

         ResultReference _Value1;
         [AcceptDrop(true)]
         [Category("Pre-Processing"), Description("Value 1"), DisplayName("Value 1"),AcceptType(typeof(Double))]
         public ResultReference Value1 {
             get {
                 return _Value1;
             }
             set {
                 _Value1 = value;
             }
         }


         ResultReference _Value2;
         [AcceptDrop(true)]
         [Category("Pre-Processing"), Description("Value 2"), DisplayName("Value 2"), AcceptType(typeof(Double))]
         public ResultReference Value2 {
             get {
                 return _Value2;
             }
             set {
                 _Value2 = value;
             }
         }



         [TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Contour"), Category("Pre-Processing"), Browsable(false)]
         [XmlIgnore]
         public override ContourPreProc ContourPreProc1 {
             get;
             set;
         }




         [TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Image"), Category("Pre-Processing"), Browsable(false)]
         [XmlIgnore]
         public override ImagePreProc ImagePreProc1 {
             get;
             set;
         }




         #endregion



         #region Post Processing


         private Double _Div;
         [XmlIgnore]
         [DisplayName("Div"), Category("Post-Processing"), Description("Division value"), ReadOnly(true)]
         //[TypeConverter(typeof(ExpandableObjectConverter))]
         [AllowDrag(true), UseInRef(true), UseInResultInput(true)]
         public Double Div {
             get { return _Div; }
             set { _Div = value; }
         }


         #endregion

         public override bool Process(Image<Bgr, byte> ImageIn, Image<Bgr, byte> ImageOut, Rectangle RoiRegion) {
             try {
                 Pass = false;
                 base.Process(ImageIn, ImageOut, RoiRegion);
                 if (Value1 == null || Value2 == null) {
                     return Pass;
                 }
                 Value1.UpdateValue();
                 Value2.UpdateValue();

                 if (Value1.ResultOutput is Double && Value2.ResultOutput is Double) {
                     
                 } else {
                     return Pass;
                 }


                 Div = (Double)Value1.ResultOutput / (Double)Value2.ResultOutput;
                 Div = Math.Round(Div, 2);


             } catch (Exception exp) {
                 
                 log.Error(this.FunctionName,exp);
             }



             return Pass;


         }


     }
}

