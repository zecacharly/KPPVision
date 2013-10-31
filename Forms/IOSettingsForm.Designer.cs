namespace VisionModule {
    partial class IOSettingsForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IOSettingsForm));
            this.@__context = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.@__tooladdRequest = new System.Windows.Forms.ToolStripMenuItem();
            this.@__toolremoveRequest = new System.Windows.Forms.ToolStripMenuItem();
            this.@__ListIOEvents = new BrightIdeasSoftware.ObjectListView();
            this.@__olvname = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.@__olvIOName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.@__olvCondition = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.@__olvConditionVar = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.@__olvEventName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.@__context.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.@__ListIOEvents)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // __context
            // 
            resources.ApplyResources(this.@__context, "__context");
            this.@__context.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.@__tooladdRequest,
            this.@__toolremoveRequest});
            this.@__context.Name = "__contextAddInsp";
            // 
            // __tooladdRequest
            // 
            resources.ApplyResources(this.@__tooladdRequest, "__tooladdRequest");
            this.@__tooladdRequest.Image = global::VisionModule.Properties.Resources.Actions_list_add_icon;
            this.@__tooladdRequest.Name = "__tooladdRequest";
            this.@__tooladdRequest.Click += new System.EventHandler(this.@__tooladdRequest_Click);
            // 
            // __toolremoveRequest
            // 
            resources.ApplyResources(this.@__toolremoveRequest, "__toolremoveRequest");
            this.@__toolremoveRequest.Image = global::VisionModule.Properties.Resources.Actions_remove_icon;
            this.@__toolremoveRequest.Name = "__toolremoveRequest";
            this.@__toolremoveRequest.Click += new System.EventHandler(this.@__toolremoveRequest_Click);
            // 
            // __ListIOEvents
            // 
            resources.ApplyResources(this.@__ListIOEvents, "__ListIOEvents");
            this.@__ListIOEvents.AllColumns.Add(this.@__olvname);
            this.@__ListIOEvents.AllColumns.Add(this.@__olvIOName);
            this.@__ListIOEvents.AllColumns.Add(this.@__olvCondition);
            this.@__ListIOEvents.AllColumns.Add(this.@__olvConditionVar);
            this.@__ListIOEvents.AllColumns.Add(this.@__olvEventName);
            this.@__ListIOEvents.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.@__ListIOEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.@__olvname,
            this.@__olvIOName,
            this.@__olvCondition,
            this.@__olvConditionVar,
            this.@__olvEventName});
            this.@__ListIOEvents.FullRowSelect = true;
            this.@__ListIOEvents.GridLines = true;
            this.@__ListIOEvents.HideSelection = false;
            this.@__ListIOEvents.IsSimpleDropSink = true;
            this.@__ListIOEvents.LabelEdit = true;
            this.@__ListIOEvents.MultiSelect = false;
            this.@__ListIOEvents.Name = "__ListIOEvents";
            this.@__ListIOEvents.OverlayText.Text = resources.GetString("resource.Text");
            this.@__ListIOEvents.ShowCommandMenuOnRightClick = true;
            this.@__ListIOEvents.ShowGroups = false;
            this.@__ListIOEvents.ShowItemToolTips = true;
            this.@__ListIOEvents.ShowSortIndicators = false;
            this.@__ListIOEvents.SortGroupItemsByPrimaryColumn = false;
            this.@__ListIOEvents.UseCompatibleStateImageBehavior = false;
            this.@__ListIOEvents.UseTranslucentSelection = true;
            this.@__ListIOEvents.View = System.Windows.Forms.View.Details;
            this.@__ListIOEvents.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.@__ListIOEvents_CellEditFinishing);
            this.@__ListIOEvents.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.@__ListRequests_CellEditStarting);
            // 
            // __olvname
            // 
            this.@__olvname.AspectName = "Name";
            resources.ApplyResources(this.@__olvname, "__olvname");
            // 
            // __olvIOName
            // 
            this.@__olvIOName.AspectName = "IOName";
            resources.ApplyResources(this.@__olvIOName, "__olvIOName");
            // 
            // __olvCondition
            // 
            this.@__olvCondition.AspectName = "Condition";
            resources.ApplyResources(this.@__olvCondition, "__olvCondition");
            // 
            // __olvConditionVar
            // 
            this.@__olvConditionVar.AspectName = "ConditionVar";
            resources.ApplyResources(this.@__olvConditionVar, "__olvConditionVar");
            // 
            // __olvEventName
            // 
            this.@__olvEventName.AspectName = "EventName";
            resources.ApplyResources(this.@__olvEventName, "__olvEventName");
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.@__ListIOEvents);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // IOSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.@__context;
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "IOSettingsForm";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IOSettingsForm_FormClosing);
            this.Load += new System.EventHandler(this.IOSettingsForm_Load);
            this.@__context.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.@__ListIOEvents)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ContextMenuStrip __context;
        public System.Windows.Forms.ToolStripMenuItem __tooladdRequest;
        public System.Windows.Forms.ToolStripMenuItem __toolremoveRequest;
        public BrightIdeasSoftware.ObjectListView __ListIOEvents;
        public BrightIdeasSoftware.OLVColumn __olvname;
        public BrightIdeasSoftware.OLVColumn __olvIOName;
        private BrightIdeasSoftware.OLVColumn __olvCondition;
        private BrightIdeasSoftware.OLVColumn __olvConditionVar;
        public BrightIdeasSoftware.OLVColumn __olvEventName;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}