using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BrightIdeasSoftware;
using KPP.Core.Debug;
using System.Reflection;
using IOModule;
using System.Threading;
using System.Globalization;
using KPPAutomationCore;

namespace VisionModule {
    internal partial class IOSettingsForm : Form {

        private static KPPLogger log = new KPPLogger(typeof(IOSettingsForm));

        public IOSettingsForm() {
            switch (LanguageSettings.Language) {
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

            InitializeComponent();
        }

        public object SelectedObject = null;
        private void IOSettingsForm_Load(object sender, EventArgs e) {

        }

        private void IOSettingsForm_FormClosing(object sender, FormClosingEventArgs e) {
            this.Hide();
        }

        private void __ListRequests_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e) {
            // Ignore edit events for other columns 
            try {

                if (e.Column==__olvname || e.Column==__olvCondition) {
                    return;
                }

                ComboBox cb = new ComboBox();
                cb.Bounds = e.CellBounds;
                cb.Font = ((ObjectListView)sender).Font;
                cb.DropDownStyle = ComboBoxStyle.DropDown;

                if (e.Column == __olvIOName) {

                                      
                }
                else if (e.Column == __olvConditionVar) {
                    PropertyInfo[] props = SelectedObject.GetType().GetProperties();

                    foreach (PropertyInfo item in props) {
                        cb.Items.Add(item.Name);
                    }

                }
                
                
               
                e.Control = cb;

            } catch (Exception exp) {

                e.Cancel = true;
                log.Error(exp);
            }
        }

        private void __ListIOEvents_CellEditFinishing(object sender, CellEditEventArgs e) {

            try {

                if (e.Column == __olvIOName) {


                    if (e.Control is ComboBox) {

                        e.Cancel = false;
                        object selecteobject = ((ComboBox)e.Control).SelectedItem;
                        
                    }
                    e.Cancel = false;


                }
                else if (e.Column == __olvEventName || e.Column == __olvConditionVar) {

                    if (e.Control is ComboBox) {


                        e.NewValue = ((ComboBox)e.Control).SelectedItem;
                       
                    }
                    e.Cancel = false;

                }
                
            } catch (Exception exp) {

                e.Cancel = true;
                log.Error(exp);
            }
        }

        private void __tooladdRequest_Click(object sender, EventArgs e) {
            if (SelectedObject is Request) {
                Request req = (Request)SelectedObject;
             
            }
            if (SelectedObject is Inspection) {
                Inspection insp = (Inspection)SelectedObject;
             
            }
        }

        private void __toolremoveRequest_Click(object sender, EventArgs e) {
            if (SelectedObject is Request) {
                Request req = (Request)SelectedObject;
             
            }
            if (SelectedObject is Inspection) {
                Inspection insp = (Inspection)SelectedObject;
             
            }
        }
    }
}
