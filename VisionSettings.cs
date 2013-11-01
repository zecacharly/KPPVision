using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using KPP.Core.Debug;
using VisionModule;
using System.Drawing;
using System.Reflection;
using DejaVu.Collections.Generic;
using DejaVu;
using IOModule;
using System.Resources;
using System.Globalization;
using System.Threading;
using KPPAutomationCore;



namespace VisionModule {



    /// <summary>
    /// Defined types of messages: Success/Warning/Error.
    /// </summary>
    public enum TypeOfMessage {
        Success,
        Warning,
        Error,
    }


    public sealed class VisionSettings {





        #region -  Serialization attributes  -

        public static Int32 S_BackupFilesToKeep = 5;
        public static String S_BackupFolderName = "backup";
        public static String S_BackupExtention = "bkp";
        public static String S_DefaulFileExtention = "xml";

        private String _filePath = null;
        private String _defaultPath = null;

        [XmlIgnore]
        public Int32 BackupFilesToKeep { get; set; }
        [XmlIgnore]
        public String BackupFolderName { get; set; }
        [XmlIgnore]
        public String BackupExtention { get; set; }

        #endregion
        private static KPPLogger log = new KPPLogger(typeof(VisionSettings));

        [XmlAttribute]
        public String Name { get; set; }


        public List<String> ProjectDirectories { get; set; }

        public List<TCPServer> Servers { get; set; }

        public String ProjectFile { get; set; }
  

        /// <summary>
        /// 
        /// </summary>
        public VisionSettings() {
            Name = "Vision Settings";
            Servers = new List<TCPServer>();

        }

        #region Read Operations

        /// <summary>
        /// Reads the configuration.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static VisionSettings ReadConfigurationFile(string path) {
            //log.Debug(String.Format("Load Xml file://{0}", path));
            if (File.Exists(path)) {
                VisionSettings result = null;
                TextReader reader = null;

                try {
                    XmlSerializer serializer = new XmlSerializer(typeof(VisionSettings));
                    reader = new StreamReader(path);
                    VisionSettings config = serializer.Deserialize(reader) as VisionSettings;
                    config._filePath = path;

                    result = config;
                }
                catch (Exception exp) {
                    log.Error(exp);
                }
                finally {
                    if (reader != null) {
                        reader.Close();
                    }
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// Reads the configuration.
        /// </summary>
        /// <param name="childtype">The childtype.</param>
        /// <param name="xmlString">The XML string.</param>
        /// <returns></returns>
        public static VisionSettings ReadConfigurationString(string xmlString) {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(VisionSettings));
                VisionSettings config = serializer.Deserialize(new StringReader(xmlString)) as VisionSettings;

                return config;
            }
            catch (Exception exp) {
                log.Error(exp);
            }
            return null;
        }

        #endregion

        #region Write Operations

        /// <summary>
        /// Writes the configuration.
        /// </summary>
        public void WriteConfigurationFile() {
            if (_filePath != null) {
                WriteConfigurationFile(_filePath);
            }
        }

        /// <summary>
        /// Writes the configuration.
        /// </summary>
        /// <param name="path">The path.</param>
        public void WriteConfigurationFile(string path) {
            WriteConfiguration(this, path, BackupFolderName, BackupExtention, BackupFilesToKeep);
        }

        /// <summary>
        /// Writes the configuration string.
        /// </summary>
        /// <returns></returns>
        public String WriteConfigurationToString() {
            return WriteConfigurationToString(this);
        }

        /// <summary>
        /// Writes the configuration.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="path">The path.</param>
        public static void WriteConfiguration(VisionSettings config, string path) {
            WriteConfiguration(config, path, S_BackupFolderName, S_BackupExtention, S_BackupFilesToKeep);
        }

        /// <summary>
        /// Writes the configuration.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="path">The path.</param>
        public static void WriteConfiguration(VisionSettings config, string path, string backupFolderName, String backupExtention, Int32 backupFilesToKeep) {
            if (File.Exists(path) && backupFilesToKeep > 0) {
                //Do a file backup prior to overwrite
                try {
                    //Check if valid backup folder name
                    if (backupFolderName == null || backupFolderName.Length == 0) {
                        backupFolderName = "backup";
                    }

                    //Check Backup folder
                    String bkpFolder = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Config"), backupFolderName);
                    if (!Directory.Exists(bkpFolder)) {
                        Directory.CreateDirectory(bkpFolder);
                    }

                    //Check extention
                    String ext = backupExtention != null && backupExtention.Length > 0 ? backupExtention : Path.GetExtension(path);
                    if (!ext.StartsWith(".")) { ext = String.Format(".{0}", ext); }

                    //Delete existing backup file (This should not exist)
                    String bkpFile = Path.Combine(bkpFolder, String.Format("{0}_{1:yyyyMMddHHmmss}{2}", Path.GetFileNameWithoutExtension(path), DateTime.Now, ext));
                    if (File.Exists(bkpFile)) { File.Delete(bkpFile); }

                    //Delete excess backup files
                    String fileSearchPattern = String.Format("{0}_*{1}", Path.GetFileNameWithoutExtension(path), ext);
                    String[] bkpFilesList = Directory.GetFiles(bkpFolder, fileSearchPattern, SearchOption.TopDirectoryOnly);
                    if (bkpFilesList != null && bkpFilesList.Length > (backupFilesToKeep - 1)) {
                        bkpFilesList = bkpFilesList.OrderByDescending(f => f.ToString()).ToArray();
                        for (int i = (backupFilesToKeep - 1); i < bkpFilesList.Length; i++) {
                            File.Delete(bkpFilesList[i]);
                        }
                    }

                    //Backup current file
                    File.Copy(path, bkpFile);
                    //log.Debug(String.Format("Backup file://{0} to file://{1}", path, bkpFile));
                }
                catch (Exception exp) {
                    //log.Error(String.Format("Error copying file {0} to backup.", path), exp);
                }
            }
            try {
                XmlSerializer serializer = new XmlSerializer(config.GetType());

                //serializer.

                TextWriter textWriter = new StreamWriter(path);
                serializer.Serialize(textWriter, config);
                textWriter.Close();
                //log.Debug(String.Format("Write Xml file://{0}", path));
            }
            catch (Exception exp) {
                //log.Error("Error writing configuration. ", exp);
                Console.WriteLine(exp.ToString());
            }
        }

        /// <summary>
        /// Writes the configuration to string.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        public static String WriteConfigurationToString(VisionSettings config) {
            try {
                XmlSerializer serializer = new XmlSerializer(config.GetType());
                StringWriter stOut = new StringWriter();
                serializer.Serialize(stOut, config);
                return stOut.ToString();
            }
            catch (Exception exp) {
                //log.Error("Error writing configuration. ", exp);
            }
            return null;
        }

        #endregion
    }
    

    public class VisionProject : ICloneable {

        private static KPPLogger log = new KPPLogger(typeof(VisionProject));

        public delegate void SelectedRequestChanged(Request NewSelectedRequest);

        public delegate void RequestRemoved(Request request);


        public event SelectedRequestChanged OnSelectedRequestChanged;
        public event RequestRemoved OnRequestRemoved;

        


        [XmlAttribute]
        public String Name { get; set; }
        [XmlAttribute("ID")]
        public int ProjectID { get; set; }


        private bool _loadonstart = false;
        [XmlAttribute]
        public bool Loadonstart {
            get { return _loadonstart; }
            set { _loadonstart = value; }
        }

        public object Clone() {
            return this.MemberwiseClone();
        }

        public class Requests : UndoRedoList<Request> {

            
        }

       

        public void RemoveRequest(Request therequest) {
            try {

                if (therequest != null) {
                    using (UndoRedoManager.Start("Request remove:" + therequest.Name)) {
                        RequestList.Remove(therequest);
                        UndoRedoManager.Commit();
                    }

                    if (OnRequestRemoved!=null) {
                        OnRequestRemoved(therequest);
                    }

                    therequest.Dispose();


                }
            } catch (Exception exp) {

                log.Error(exp);

            }
        }


        public Requests RequestList { get; set; }

        private Request _SelectedRequest;
        [XmlIgnore]
        public Request SelectedRequest {
            get {
                return _SelectedRequest;
            }
            set {
                if (_SelectedRequest != value) {
                    if (_SelectedRequest!=null) {
                        if (_SelectedRequest.SelectedInspection != null) {
                            _SelectedRequest.SelectedInspection.SetLayers(false);
                        }

                    }
                    _SelectedRequest = value;
                    if (OnSelectedRequestChanged != null) {
                        OnSelectedRequestChanged(value);
                    }
                }

            }
        }


        internal static List<Inspection> ListInspections = new List<Inspection>();

        public void Dispose() {
            try {
                OnSelectedRequestChanged = null;
                foreach (Request req in RequestList) {
                    req.Dispose();
                }
               
                SelectedRequest = null;
               

            } catch (Exception exp) {

                log.Error(exp);
            }
        }

        public VisionProject() {

            Name = "default project name";

            RequestList = new Requests();

            ProjectID = -1;

        }

    }


    public sealed class VisionProjects {

        #region -  Serialization attributes  -

        internal static Int32 S_BackupFilesToKeep = 5;
        internal static String S_BackupFolderName = "backup";
        internal static String S_BackupExtention = "bkp";
        internal static String S_DefaulFileExtention = "xml";

        private String _filePath = null;
        private String _defaultPath = null;

        [XmlIgnore]
        public Int32 BackupFilesToKeep { get; set; }
        [XmlIgnore]
        public String BackupFolderName { get; set; }
        [XmlIgnore]
        public String BackupExtention { get; set; }

        #endregion
        private static KPPLogger log = new KPPLogger(typeof(VisionProjects));

        [XmlAttribute]
        public String Name { get; set; }



       
        
        public List<VisionProject> Projects { get; set; }
    


        /// <summary>
        /// 
        /// </summary>
        public VisionProjects() {
            Name = "Project Settings";
                       
            Projects = new List<VisionProject>();
        
          
        }


        //    StaticObjects.ListInspections.Add(item);
    
        #region Read Operations

        /// <summary>
        /// Reads the configuration.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        internal static VisionProjects ReadConfigurationFile(string path) {
            //log.Debug(String.Format("Load Xml file://{0}", path));
            if (File.Exists(path)) {
                VisionProjects result = null;
                TextReader reader = null;

                try {
                    XmlSerializer serializer = new XmlSerializer(typeof(VisionProjects));
                    reader = new StreamReader(path);
                    StaticObjects.isLoading = true;
                    UndoRedoManager.StartInvisible("Init");

                    VisionProjects config = serializer.Deserialize(reader) as VisionProjects;
                    StaticObjects.isLoading = false;
                    config._filePath = path;

                    result = config;
                    UndoRedoManager.Commit();
                } catch (Exception exp) {
                    log.Error(exp);
                }
                finally {
                    if (reader != null) {
                        reader.Close();
                    }
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// Reads the configuration.
        /// </summary>
        /// <param name="childtype">The childtype.</param>
        /// <param name="xmlString">The XML string.</param>
        /// <returns></returns>
        internal static VisionProjects ReadConfigurationString(string xmlString) {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(VisionProjects));
                VisionProjects config = serializer.Deserialize(new StringReader(xmlString)) as VisionProjects;

                return config;
            }
            catch (Exception exp) {
                log.Error(exp);
            }
            return null;
        }

        #endregion

        #region Write Operations

        /// <summary>
        /// Writes the configuration.
        /// </summary>
        public void WriteConfigurationFile() {
            if (_filePath != null) {
            
                WriteConfigurationFile(_filePath);
      
            }
        }

        /// <summary>
        /// Writes the configuration.
        /// </summary>
        /// <param name="path">The path.</param>
        public void WriteConfigurationFile(string path) {

            WriteConfiguration(this, path, BackupFolderName, BackupExtention, BackupFilesToKeep);
       
        }

        /// <summary>
        /// Writes the configuration string.
        /// </summary>
        /// <returns></returns>
        public String WriteConfigurationToString() {
            
            return WriteConfigurationToString(this);
        }

        /// <summary>
        /// Writes the configuration.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="path">The path.</param>
        internal static void WriteConfiguration(VisionProjects config, string path) {
            WriteConfiguration(config, path, S_BackupFolderName, S_BackupExtention, S_BackupFilesToKeep);
        }

        /// <summary>
        /// Writes the configuration.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="path">The path.</param>
        internal static void WriteConfiguration(VisionProjects config, string path, string backupFolderName, String backupExtention, Int32 backupFilesToKeep) {
            if (File.Exists(path) && backupFilesToKeep > 0) {
                //Do a file backup prior to overwrite
                try {
                    //Check if valid backup folder name
                    if (backupFolderName == null || backupFolderName.Length == 0) {
                        backupFolderName = "backup";
                    }

                    //Check Backup folder
                    String bkpFolder = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Config"), backupFolderName);
                    if (!Directory.Exists(bkpFolder)) {
                        Directory.CreateDirectory(bkpFolder);
                    }

                    //Check extention
                    String ext = backupExtention != null && backupExtention.Length > 0 ? backupExtention : Path.GetExtension(path);
                    if (!ext.StartsWith(".")) { ext = String.Format(".{0}", ext); }

                    //Delete existing backup file (This should not exist)
                    String bkpFile = Path.Combine(bkpFolder, String.Format("{0}_{1:yyyyMMddHHmmss}{2}", Path.GetFileNameWithoutExtension(path), DateTime.Now, ext));
                    if (File.Exists(bkpFile)) { File.Delete(bkpFile); }

                    //Delete excess backup files
                    String fileSearchPattern = String.Format("{0}_*{1}", Path.GetFileNameWithoutExtension(path), ext);
                    String[] bkpFilesList = Directory.GetFiles(bkpFolder, fileSearchPattern, SearchOption.TopDirectoryOnly);
                    if (bkpFilesList != null && bkpFilesList.Length > (backupFilesToKeep - 1)) {
                        bkpFilesList = bkpFilesList.OrderByDescending(f => f.ToString()).ToArray();
                        for (int i = (backupFilesToKeep - 1); i < bkpFilesList.Length; i++) {
                            File.Delete(bkpFilesList[i]);
                        }
                    }

                    //Backup current file
                    File.Copy(path, bkpFile);
                    //log.Debug(String.Format("Backup file://{0} to file://{1}", path, bkpFile));
                }
                catch (Exception exp) {
                    //log.Error(String.Format("Error copying file {0} to backup.", path), exp);
                }
            }
            try {
                StaticObjects.isLoading = true;
                XmlSerializer serializer = new XmlSerializer(config.GetType());                                
                TextWriter textWriter = new StreamWriter(path);                
                serializer.Serialize(textWriter, config);
                textWriter.Close();
                StaticObjects.isLoading = false;
                //log.Debug(String.Format("Write Xml file://{0}", path));
            }
            catch (Exception exp) {
                log.Error("Error writing configuration. ", exp);
                StaticObjects.isLoading = false;
                Console.WriteLine(exp.ToString());
            }
        }

        /// <summary>
        /// Writes the configuration to string.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        internal static String WriteConfigurationToString(VisionProjects config) {
            try {
                XmlSerializer serializer = new XmlSerializer(config.GetType());
                StringWriter stOut = new StringWriter();
                StaticObjects.isLoading = true;
                serializer.Serialize(stOut, config);
                StaticObjects.isLoading = false;
                return stOut.ToString();
            }
            catch (Exception exp) {
                //log.Error("Error writing configuration. ", exp);
                StaticObjects.isLoading = false;
            }
            return null;
        }

        #endregion
    }


    



    internal sealed class ProcessingFunctionDefinition {
        public String FunctionType { get; set; }
        [Browsable(false), XmlAttribute]
        public String FunctionGroup { get; set; }
        public Type BaseType { get; set; }

        
        public ProcessingFunctionDefinition(String _FunctionType,String _FunctionGroup, Type t) {
            FunctionType = _FunctionType;
            FunctionGroup = _FunctionGroup;
            BaseType = t;
        }

        public override string ToString() {
            return FunctionType;
        }
    }

    internal static class ReflectionController {

        internal static List<ProcessingFunctionDefinition> ProcessingFunctions = new List<ProcessingFunctionDefinition>();
        //internal static List<ResultsProcessingDefinition> ResultsProcessingFunctions = new List<ResultsProcessingDefinition>();

        internal static void LoadFolder(String path) {
            if (path != null && path.Length > 0) {
                String dpath;
                if (Directory.Exists(dpath = Path.GetDirectoryName(path))) {
                    DirectoryInfo di = new DirectoryInfo(dpath);
                    List<FileInfo> files = di.GetFiles("*.dll").ToList();
                    if (files.Count > 0) {
                        foreach (FileInfo fi in files) {
                            Load(fi.FullName);
                        }
                    }
                }
            }
        }

        internal static void Load(String path) {
            if (path != null && path.Length > 0) {
                if (File.Exists(path)) {
                    try {
                        Assembly ass = Assembly.LoadFrom(path);

                        List<Type> types = ass.GetTypes().ToList();
                        if (types.Count > 0) {
                            List<object> atts;
                            foreach (Type t in types) {
                                atts = t.GetCustomAttributes(typeof(ProcessingFunctionAttribute), true).ToList();
                                try {
                                    if (atts.Count > 0) {
                                        foreach (ProcessingFunctionAttribute pf in atts) {
                                            if (ProcessingFunctions.Where(p => p.BaseType.FullName.Equals(t.FullName)).Count() == 0) {
                                                Console.WriteLine("Found new Processing Function " + pf.FunctionType);
                                                ProcessingFunctions.Add(new ProcessingFunctionDefinition(pf.FunctionType,pf.FunctionGroup, t));
                                            }
                                            else {
                                                //O tipo ja está na lista
                                            }
                                        }
                                    }
                                } catch (Exception exp) {
                                    Console.WriteLine("Error loading Type Attributes");
                                    Console.WriteLine(exp.ToString());
                                }

                                //atts = t.GetCustomAttributes(typeof(ResultsProcessingAttribute), true).ToList();
                                //try {
                                //    if (atts.Count > 0) {
                                //        foreach (ResultsProcessingAttribute pf in atts) {
                                //            if (ResultsProcessingFunctions.Where(r => r.BaseType.FullName.Equals(t.FullName)).Count() == 0) {
                                //                Console.WriteLine("Found new result processing function " + pf.Name);
                                //                ResultsProcessingFunctions.Add(new ResultsProcessingDefinition(pf.Name, t));
                                //            }
                                //            else {
                                //                //O tipo ja está na lista
                                //            }
                                //        }
                                //    }
                                //} catch (Exception exp) {
                                //    Console.WriteLine("Error loading Type Attributes");
                                //    Console.WriteLine(exp.ToString());
                                //}

                            }
                        }
                    } catch (Exception exp) {
                        Console.WriteLine("Unable to load file " + path);
                        Console.WriteLine(exp.ToString());
                    }
                }
            }
        }

        internal static void UnloadAll() {
            ProcessingFunctions.Clear();

        }
    }

    public class KPPVision {

       


        private MainForm RunningForm = null;

        public KPPVision() {
            DebugController.ActiveDebugController = new DebugController(Path.Combine(Application.StartupPath, "app.log"));

            RunningForm = new MainForm();
            RunningForm.Show();
            
        }

        public KPPVision(String LoadProjectName)
            : this() {
        }

        public KPPVision(int LoadProjectID)
            : this() {

        }
    }

}
