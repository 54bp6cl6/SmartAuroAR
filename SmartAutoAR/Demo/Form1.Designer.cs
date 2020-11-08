namespace Demo
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.InputSource_GB = new System.Windows.Forms.GroupBox();
			this.CameraID_Tb = new System.Windows.Forms.TextBox();
			this.SelectedInput_Lb = new System.Windows.Forms.Label();
			this.InputSelection_Bt = new System.Windows.Forms.Button();
			this.StreamSource_RB = new System.Windows.Forms.RadioButton();
			this.VideoSource_RB = new System.Windows.Forms.RadioButton();
			this.ImageSource_RB = new System.Windows.Forms.RadioButton();
			this.InputSource_OFD = new System.Windows.Forms.OpenFileDialog();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.RotaationZ_TB = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.RotaationY_TB = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.RotaationX_TB = new System.Windows.Forms.TextBox();
			this.SelectedModel_Lb = new System.Windows.Forms.Label();
			this.Resize_TB = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.ModelSelection_Bt = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.ColorHarmonizing_CB = new System.Windows.Forms.CheckBox();
			this.LightTracking_CB = new System.Windows.Forms.CheckBox();
			this.Restart_Bt = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.panel3 = new System.Windows.Forms.Panel();
			this.Exemple3_Bt = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.Exemple2_Bt = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.Exemple1_Bt = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.Model_OFD = new System.Windows.Forms.OpenFileDialog();
			this.SelectedMarker_Lb = new System.Windows.Forms.Label();
			this.MarkerSelection_Bt = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.Marker_OFD = new System.Windows.Forms.OpenFileDialog();
			this.Spin_CB = new System.Windows.Forms.CheckBox();
			this.InputSource_GB.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// InputSource_GB
			// 
			this.InputSource_GB.Controls.Add(this.CameraID_Tb);
			this.InputSource_GB.Controls.Add(this.SelectedInput_Lb);
			this.InputSource_GB.Controls.Add(this.InputSelection_Bt);
			this.InputSource_GB.Controls.Add(this.StreamSource_RB);
			this.InputSource_GB.Controls.Add(this.VideoSource_RB);
			this.InputSource_GB.Controls.Add(this.ImageSource_RB);
			this.InputSource_GB.Location = new System.Drawing.Point(12, 12);
			this.InputSource_GB.Name = "InputSource_GB";
			this.InputSource_GB.Size = new System.Drawing.Size(324, 106);
			this.InputSource_GB.TabIndex = 0;
			this.InputSource_GB.TabStop = false;
			this.InputSource_GB.Text = "資料來源";
			// 
			// CameraID_Tb
			// 
			this.CameraID_Tb.Location = new System.Drawing.Point(248, 61);
			this.CameraID_Tb.Name = "CameraID_Tb";
			this.CameraID_Tb.Size = new System.Drawing.Size(48, 23);
			this.CameraID_Tb.TabIndex = 5;
			this.CameraID_Tb.Text = "0";
			this.CameraID_Tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.CameraID_Tb.Visible = false;
			// 
			// SelectedInput_Lb
			// 
			this.SelectedInput_Lb.Location = new System.Drawing.Point(121, 19);
			this.SelectedInput_Lb.Name = "SelectedInput_Lb";
			this.SelectedInput_Lb.Size = new System.Drawing.Size(190, 39);
			this.SelectedInput_Lb.TabIndex = 3;
			this.SelectedInput_Lb.Text = "尚未選擇檔案";
			this.SelectedInput_Lb.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// InputSelection_Bt
			// 
			this.InputSelection_Bt.Location = new System.Drawing.Point(215, 61);
			this.InputSelection_Bt.Name = "InputSelection_Bt";
			this.InputSelection_Bt.Size = new System.Drawing.Size(96, 30);
			this.InputSelection_Bt.TabIndex = 2;
			this.InputSelection_Bt.Text = "選擇檔案";
			this.InputSelection_Bt.UseVisualStyleBackColor = true;
			this.InputSelection_Bt.Click += new System.EventHandler(this.FileSelection_Bt_Click);
			// 
			// StreamSource_RB
			// 
			this.StreamSource_RB.AutoSize = true;
			this.StreamSource_RB.Location = new System.Drawing.Point(13, 72);
			this.StreamSource_RB.Name = "StreamSource_RB";
			this.StreamSource_RB.Size = new System.Drawing.Size(73, 19);
			this.StreamSource_RB.TabIndex = 0;
			this.StreamSource_RB.Text = "串流輸入";
			this.StreamSource_RB.UseVisualStyleBackColor = true;
			this.StreamSource_RB.CheckedChanged += new System.EventHandler(this.InputSource_RB_CheckedChanged);
			// 
			// VideoSource_RB
			// 
			this.VideoSource_RB.AutoSize = true;
			this.VideoSource_RB.Location = new System.Drawing.Point(13, 47);
			this.VideoSource_RB.Name = "VideoSource_RB";
			this.VideoSource_RB.Size = new System.Drawing.Size(73, 19);
			this.VideoSource_RB.TabIndex = 0;
			this.VideoSource_RB.Text = "影片輸入";
			this.VideoSource_RB.UseVisualStyleBackColor = true;
			this.VideoSource_RB.CheckedChanged += new System.EventHandler(this.InputSource_RB_CheckedChanged);
			// 
			// ImageSource_RB
			// 
			this.ImageSource_RB.AutoSize = true;
			this.ImageSource_RB.Checked = true;
			this.ImageSource_RB.Location = new System.Drawing.Point(13, 22);
			this.ImageSource_RB.Name = "ImageSource_RB";
			this.ImageSource_RB.Size = new System.Drawing.Size(73, 19);
			this.ImageSource_RB.TabIndex = 0;
			this.ImageSource_RB.TabStop = true;
			this.ImageSource_RB.Text = "圖片輸入";
			this.ImageSource_RB.UseVisualStyleBackColor = true;
			this.ImageSource_RB.CheckedChanged += new System.EventHandler(this.InputSource_RB_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.RotaationZ_TB);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.RotaationY_TB);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.RotaationX_TB);
			this.groupBox1.Controls.Add(this.SelectedModel_Lb);
			this.groupBox1.Controls.Add(this.Resize_TB);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.ModelSelection_Bt);
			this.groupBox1.Location = new System.Drawing.Point(12, 200);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(324, 141);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "模型選擇";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(244, 98);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(13, 15);
			this.label6.TabIndex = 4;
			this.label6.Text = "z";
			// 
			// RotaationZ_TB
			// 
			this.RotaationZ_TB.Location = new System.Drawing.Point(263, 95);
			this.RotaationZ_TB.Name = "RotaationZ_TB";
			this.RotaationZ_TB.Size = new System.Drawing.Size(48, 23);
			this.RotaationZ_TB.TabIndex = 5;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(163, 98);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(13, 15);
			this.label5.TabIndex = 4;
			this.label5.Text = "y";
			// 
			// RotaationY_TB
			// 
			this.RotaationY_TB.Location = new System.Drawing.Point(182, 95);
			this.RotaationY_TB.Name = "RotaationY_TB";
			this.RotaationY_TB.Size = new System.Drawing.Size(48, 23);
			this.RotaationY_TB.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(84, 98);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(13, 15);
			this.label4.TabIndex = 4;
			this.label4.Text = "x";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 98);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(62, 15);
			this.label3.TabIndex = 4;
			this.label3.Text = "Rotation :";
			// 
			// RotaationX_TB
			// 
			this.RotaationX_TB.Location = new System.Drawing.Point(103, 95);
			this.RotaationX_TB.Name = "RotaationX_TB";
			this.RotaationX_TB.Size = new System.Drawing.Size(48, 23);
			this.RotaationX_TB.TabIndex = 5;
			// 
			// SelectedModel_Lb
			// 
			this.SelectedModel_Lb.AutoSize = true;
			this.SelectedModel_Lb.Location = new System.Drawing.Point(13, 28);
			this.SelectedModel_Lb.Name = "SelectedModel_Lb";
			this.SelectedModel_Lb.Size = new System.Drawing.Size(79, 15);
			this.SelectedModel_Lb.TabIndex = 4;
			this.SelectedModel_Lb.Text = "尚未選擇模型";
			// 
			// Resize_TB
			// 
			this.Resize_TB.Location = new System.Drawing.Point(72, 66);
			this.Resize_TB.Name = "Resize_TB";
			this.Resize_TB.Size = new System.Drawing.Size(80, 23);
			this.Resize_TB.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 69);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(49, 15);
			this.label2.TabIndex = 4;
			this.label2.Text = "Resize :";
			// 
			// ModelSelection_Bt
			// 
			this.ModelSelection_Bt.Location = new System.Drawing.Point(215, 20);
			this.ModelSelection_Bt.Name = "ModelSelection_Bt";
			this.ModelSelection_Bt.Size = new System.Drawing.Size(96, 30);
			this.ModelSelection_Bt.TabIndex = 2;
			this.ModelSelection_Bt.Text = "選擇模型";
			this.ModelSelection_Bt.UseVisualStyleBackColor = true;
			this.ModelSelection_Bt.Click += new System.EventHandler(this.ModelSelection_Bt_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.ColorHarmonizing_CB);
			this.groupBox2.Controls.Add(this.LightTracking_CB);
			this.groupBox2.Location = new System.Drawing.Point(12, 351);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(165, 100);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "擬真化模組";
			// 
			// ColorHarmonizing_CB
			// 
			this.ColorHarmonizing_CB.AutoSize = true;
			this.ColorHarmonizing_CB.Location = new System.Drawing.Point(32, 58);
			this.ColorHarmonizing_CB.Name = "ColorHarmonizing_CB";
			this.ColorHarmonizing_CB.Size = new System.Drawing.Size(98, 19);
			this.ColorHarmonizing_CB.TabIndex = 0;
			this.ColorHarmonizing_CB.Text = "色彩調和模組";
			this.ColorHarmonizing_CB.UseVisualStyleBackColor = true;
			// 
			// LightTracking_CB
			// 
			this.LightTracking_CB.AutoSize = true;
			this.LightTracking_CB.Location = new System.Drawing.Point(32, 33);
			this.LightTracking_CB.Name = "LightTracking_CB";
			this.LightTracking_CB.Size = new System.Drawing.Size(98, 19);
			this.LightTracking_CB.TabIndex = 0;
			this.LightTracking_CB.Text = "光源追蹤模組";
			this.LightTracking_CB.UseVisualStyleBackColor = true;
			// 
			// Restart_Bt
			// 
			this.Restart_Bt.Location = new System.Drawing.Point(211, 390);
			this.Restart_Bt.Name = "Restart_Bt";
			this.Restart_Bt.Size = new System.Drawing.Size(112, 53);
			this.Restart_Bt.TabIndex = 2;
			this.Restart_Bt.Text = "啟動";
			this.Restart_Bt.UseVisualStyleBackColor = true;
			this.Restart_Bt.Click += new System.EventHandler(this.Restart_Bt_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox3.Controls.Add(this.panel3);
			this.groupBox3.Controls.Add(this.panel2);
			this.groupBox3.Controls.Add(this.panel1);
			this.groupBox3.Location = new System.Drawing.Point(359, 12);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(445, 439);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "範例設置";
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.SystemColors.ControlLight;
			this.panel3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel3.BackgroundImage")));
			this.panel3.Controls.Add(this.Exemple3_Bt);
			this.panel3.Controls.Add(this.label8);
			this.panel3.Controls.Add(this.pictureBox3);
			this.panel3.Location = new System.Drawing.Point(17, 301);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(408, 100);
			this.panel3.TabIndex = 0;
			// 
			// Exemple3_Bt
			// 
			this.Exemple3_Bt.BackColor = System.Drawing.Color.Moccasin;
			this.Exemple3_Bt.Location = new System.Drawing.Point(326, 56);
			this.Exemple3_Bt.Name = "Exemple3_Bt";
			this.Exemple3_Bt.Size = new System.Drawing.Size(72, 34);
			this.Exemple3_Bt.TabIndex = 2;
			this.Exemple3_Bt.Text = "使用範例";
			this.Exemple3_Bt.UseVisualStyleBackColor = false;
			this.Exemple3_Bt.Click += new System.EventHandler(this.Exemple3_Bt_Click);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.BackColor = System.Drawing.Color.Transparent;
			this.label8.Font = new System.Drawing.Font("Microsoft JhengHei UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this.label8.Location = new System.Drawing.Point(102, 17);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(191, 26);
			this.label8.TabIndex = 1;
			this.label8.Text = "喔齁~貓咪好可愛~";
			// 
			// pictureBox3
			// 
			this.pictureBox3.BackColor = System.Drawing.Color.Moccasin;
			this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
			this.pictureBox3.Location = new System.Drawing.Point(17, 13);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(70, 70);
			this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox3.TabIndex = 0;
			this.pictureBox3.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
			this.panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel2.BackgroundImage")));
			this.panel2.Controls.Add(this.Exemple2_Bt);
			this.panel2.Controls.Add(this.label7);
			this.panel2.Controls.Add(this.pictureBox2);
			this.panel2.Location = new System.Drawing.Point(17, 174);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(408, 100);
			this.panel2.TabIndex = 0;
			// 
			// Exemple2_Bt
			// 
			this.Exemple2_Bt.BackColor = System.Drawing.Color.LightGray;
			this.Exemple2_Bt.Location = new System.Drawing.Point(326, 58);
			this.Exemple2_Bt.Name = "Exemple2_Bt";
			this.Exemple2_Bt.Size = new System.Drawing.Size(72, 34);
			this.Exemple2_Bt.TabIndex = 2;
			this.Exemple2_Bt.Text = "使用範例";
			this.Exemple2_Bt.UseVisualStyleBackColor = false;
			this.Exemple2_Bt.Click += new System.EventHandler(this.Exemple2_Bt_Click);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.BackColor = System.Drawing.Color.LightGray;
			this.label7.Font = new System.Drawing.Font("Microsoft JhengHei UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.label7.ForeColor = System.Drawing.Color.Black;
			this.label7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label7.Location = new System.Drawing.Point(102, 18);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(138, 26);
			this.label7.TabIndex = 1;
			this.label7.Text = "超炫炮九桃郎";
			// 
			// pictureBox2
			// 
			this.pictureBox2.BackColor = System.Drawing.Color.LightGray;
			this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
			this.pictureBox2.Location = new System.Drawing.Point(17, 14);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(70, 70);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox2.TabIndex = 0;
			this.pictureBox2.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
			this.panel1.Controls.Add(this.Exemple1_Bt);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.pictureBox1);
			this.panel1.Location = new System.Drawing.Point(17, 50);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(408, 100);
			this.panel1.TabIndex = 0;
			// 
			// Exemple1_Bt
			// 
			this.Exemple1_Bt.BackColor = System.Drawing.Color.Thistle;
			this.Exemple1_Bt.Location = new System.Drawing.Point(326, 56);
			this.Exemple1_Bt.Name = "Exemple1_Bt";
			this.Exemple1_Bt.Size = new System.Drawing.Size(72, 34);
			this.Exemple1_Bt.TabIndex = 2;
			this.Exemple1_Bt.Text = "使用範例";
			this.Exemple1_Bt.UseVisualStyleBackColor = false;
			this.Exemple1_Bt.Click += new System.EventHandler(this.FileSelection_Bt_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Microsoft JhengHei UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label1.Location = new System.Drawing.Point(102, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(264, 26);
			this.label1.TabIndex = 1;
			this.label1.Text = "老師！化學元素跳出來了！";
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.Thistle;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(17, 14);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(70, 70);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// Model_OFD
			// 
			this.Model_OFD.Filter = "OBJ檔|*.obj";
			// 
			// SelectedMarker_Lb
			// 
			this.SelectedMarker_Lb.AutoSize = true;
			this.SelectedMarker_Lb.Location = new System.Drawing.Point(13, 28);
			this.SelectedMarker_Lb.Name = "SelectedMarker_Lb";
			this.SelectedMarker_Lb.Size = new System.Drawing.Size(98, 15);
			this.SelectedMarker_Lb.TabIndex = 4;
			this.SelectedMarker_Lb.Text = "尚未選擇 Marker";
			// 
			// MarkerSelection_Bt
			// 
			this.MarkerSelection_Bt.Location = new System.Drawing.Point(215, 20);
			this.MarkerSelection_Bt.Name = "MarkerSelection_Bt";
			this.MarkerSelection_Bt.Size = new System.Drawing.Size(96, 30);
			this.MarkerSelection_Bt.TabIndex = 2;
			this.MarkerSelection_Bt.Text = "選擇Marker";
			this.MarkerSelection_Bt.UseVisualStyleBackColor = true;
			this.MarkerSelection_Bt.Click += new System.EventHandler(this.MarkerSelection_Bt_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.SelectedMarker_Lb);
			this.groupBox4.Controls.Add(this.MarkerSelection_Bt);
			this.groupBox4.Location = new System.Drawing.Point(12, 127);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(324, 63);
			this.groupBox4.TabIndex = 1;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Marker 選擇";
			// 
			// Spin_CB
			// 
			this.Spin_CB.AutoSize = true;
			this.Spin_CB.Location = new System.Drawing.Point(234, 369);
			this.Spin_CB.Name = "Spin_CB";
			this.Spin_CB.Size = new System.Drawing.Size(74, 19);
			this.Spin_CB.TabIndex = 4;
			this.Spin_CB.Text = "模型旋轉";
			this.Spin_CB.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(814, 462);
			this.Controls.Add(this.Spin_CB);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.Restart_Bt);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.InputSource_GB);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "智動擬真標記式AR引擎 Demo";
			this.InputSource_GB.ResumeLayout(false);
			this.InputSource_GB.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox InputSource_GB;
		private System.Windows.Forms.Button InputSelection_Bt;
		private System.Windows.Forms.RadioButton StreamSource_RB;
		private System.Windows.Forms.RadioButton ImageSource_RB;
		private System.Windows.Forms.RadioButton VideoSource_RB;
		private System.Windows.Forms.Label SelectedInput_Lb;
		private System.Windows.Forms.OpenFileDialog InputSource_OFD;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox RotaationZ_TB;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox RotaationY_TB;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox RotaationX_TB;
		private System.Windows.Forms.Label SelectedModel_Lb;
		private System.Windows.Forms.TextBox Resize_TB;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button ModelSelection_Bt;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox ColorHarmonizing_CB;
		private System.Windows.Forms.CheckBox LightTracking_CB;
		private System.Windows.Forms.Button Restart_Bt;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button Exemple1_Bt;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Button Exemple3_Bt;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button Exemple2_Bt;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.OpenFileDialog Model_OFD;
		private System.Windows.Forms.Label SelectedMarker_Lb;
		private System.Windows.Forms.Button MarkerSelection_Bt;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.OpenFileDialog Marker_OFD;
		private System.Windows.Forms.TextBox CameraID_Tb;
		private System.Windows.Forms.CheckBox Spin_CB;
	}
}

