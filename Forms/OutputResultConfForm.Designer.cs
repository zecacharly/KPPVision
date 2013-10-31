namespace VisionModule {
    partial class OutputResultConfForm {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.@__comboRequest = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.@__comboInsp = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.@__comboRois = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.@__RoiProcList = new BrightIdeasSoftware.FastObjectListView();
            this.olvColumnFuncName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.@__proplist = new BrightIdeasSoftware.FastObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.@__RoiProcList)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.@__proplist)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.@__comboRequest);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(181, 49);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Request";
            // 
            // __comboRequest
            // 
            this.@__comboRequest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.@__comboRequest.FormattingEnabled = true;
            this.@__comboRequest.Location = new System.Drawing.Point(6, 19);
            this.@__comboRequest.Name = "__comboRequest";
            this.@__comboRequest.Size = new System.Drawing.Size(169, 21);
            this.@__comboRequest.TabIndex = 1;
            this.@__comboRequest.SelectedValueChanged += new System.EventHandler(this.@__comboRequest_SelectedValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.@__comboInsp);
            this.groupBox2.Location = new System.Drawing.Point(199, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(181, 49);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Inspections";
            // 
            // __comboInsp
            // 
            this.@__comboInsp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.@__comboInsp.FormattingEnabled = true;
            this.@__comboInsp.Location = new System.Drawing.Point(6, 19);
            this.@__comboInsp.Name = "__comboInsp";
            this.@__comboInsp.Size = new System.Drawing.Size(169, 21);
            this.@__comboInsp.TabIndex = 1;
            this.@__comboInsp.SelectedIndexChanged += new System.EventHandler(this.@__comboInsp_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.@__comboRois);
            this.groupBox3.Location = new System.Drawing.Point(386, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(181, 49);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ROI\'s";
            // 
            // __comboRois
            // 
            this.@__comboRois.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.@__comboRois.FormattingEnabled = true;
            this.@__comboRois.Location = new System.Drawing.Point(6, 19);
            this.@__comboRois.Name = "__comboRois";
            this.@__comboRois.Size = new System.Drawing.Size(169, 21);
            this.@__comboRois.TabIndex = 1;
            this.@__comboRois.SelectedIndexChanged += new System.EventHandler(this.@__comboRois_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.@__RoiProcList);
            this.groupBox4.Location = new System.Drawing.Point(12, 67);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(289, 145);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Functions";
            // 
            // __RoiProcList
            // 
            this.@__RoiProcList.AllColumns.Add(this.olvColumnFuncName);
            this.@__RoiProcList.AllColumns.Add(this.olvColumn2);
            this.@__RoiProcList.AllowDrop = true;
            this.@__RoiProcList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnFuncName,
            this.olvColumn2});
            this.@__RoiProcList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.@__RoiProcList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.@__RoiProcList.FullRowSelect = true;
            this.@__RoiProcList.IsSimpleDropSink = true;
            this.@__RoiProcList.Location = new System.Drawing.Point(3, 16);
            this.@__RoiProcList.MultiSelect = false;
            this.@__RoiProcList.Name = "__RoiProcList";
            this.@__RoiProcList.ShowFilterMenuOnRightClick = false;
            this.@__RoiProcList.ShowGroups = false;
            this.@__RoiProcList.ShowSortIndicators = false;
            this.@__RoiProcList.Size = new System.Drawing.Size(283, 126);
            this.@__RoiProcList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.@__RoiProcList.TabIndex = 20;
            this.@__RoiProcList.TintSortColumn = true;
            this.@__RoiProcList.UseCompatibleStateImageBehavior = false;
            this.@__RoiProcList.UseTranslucentSelection = true;
            this.@__RoiProcList.View = System.Windows.Forms.View.Details;
            this.@__RoiProcList.VirtualMode = true;
            this.@__RoiProcList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.@__RoiProcList_ItemSelectionChanged);
            // 
            // olvColumnFuncName
            // 
            this.olvColumnFuncName.AspectName = "FunctionName";
            this.olvColumnFuncName.Text = "Name";
            this.olvColumnFuncName.Width = 140;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "FunctionType";
            this.olvColumn2.IsEditable = false;
            this.olvColumn2.Text = "Function Type";
            this.olvColumn2.Width = 120;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.@__proplist);
            this.groupBox5.Location = new System.Drawing.Point(307, 67);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(289, 145);
            this.groupBox5.TabIndex = 22;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Outputs";
            // 
            // __proplist
            // 
            this.@__proplist.AllColumns.Add(this.olvColumn1);
            this.@__proplist.AllColumns.Add(this.olvColumn3);
            this.@__proplist.AllowDrop = true;
            this.@__proplist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn3});
            this.@__proplist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.@__proplist.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.@__proplist.FullRowSelect = true;
            this.@__proplist.IsSimpleDropSink = true;
            this.@__proplist.Location = new System.Drawing.Point(3, 16);
            this.@__proplist.MultiSelect = false;
            this.@__proplist.Name = "__proplist";
            this.@__proplist.ShowFilterMenuOnRightClick = false;
            this.@__proplist.ShowGroups = false;
            this.@__proplist.ShowSortIndicators = false;
            this.@__proplist.Size = new System.Drawing.Size(283, 126);
            this.@__proplist.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.@__proplist.TabIndex = 20;
            this.@__proplist.TintSortColumn = true;
            this.@__proplist.UseCompatibleStateImageBehavior = false;
            this.@__proplist.UseTranslucentSelection = true;
            this.@__proplist.View = System.Windows.Forms.View.Details;
            this.@__proplist.VirtualMode = true;
            this.@__proplist.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.@__proplist_ItemSelectionChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Name";
            this.olvColumn1.Text = "Name";
            this.olvColumn1.Width = 140;
            // 
            // olvColumn3
            // 
            this.olvColumn3.Text = "Type";
            this.olvColumn3.Width = 100;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(512, 234);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(81, 38);
            this.button1.TabIndex = 23;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(511, 291);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(82, 38);
            this.button2.TabIndex = 24;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(12, 215);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(493, 198);
            this.propertyGrid1.TabIndex = 25;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(511, 359);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(81, 38);
            this.button3.TabIndex = 26;
            this.button3.Text = "Clear result";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // OutputResultConfForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 425);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "OutputResultConfForm";
            this.Text = "Input Results Selector";
            this.Load += new System.EventHandler(this.OutputResultConfForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.@__RoiProcList)).EndInit();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.@__proplist)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox __comboRequest;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox __comboInsp;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox __comboRois;
        private System.Windows.Forms.GroupBox groupBox4;
        public BrightIdeasSoftware.FastObjectListView __RoiProcList;
        public BrightIdeasSoftware.OLVColumn olvColumnFuncName;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private System.Windows.Forms.GroupBox groupBox5;
        public BrightIdeasSoftware.FastObjectListView __proplist;
        public BrightIdeasSoftware.OLVColumn olvColumn1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button button3;
    }
}