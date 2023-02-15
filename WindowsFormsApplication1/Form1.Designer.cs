namespace WindowsFormsApplication1 {
	partial class Form1 {
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
			this.pScreen = new System.Windows.Forms.Panel();
			this.btnCreateDevice = new System.Windows.Forms.Button();
			this.labelLog = new System.Windows.Forms.Label();
			this.btnLoadCNC = new System.Windows.Forms.Button();
			this.btnTop = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.labelInfo = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// pScreen
			// 
			this.pScreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pScreen.Location = new System.Drawing.Point(12, 118);
			this.pScreen.Name = "pScreen";
			this.pScreen.Size = new System.Drawing.Size(610, 414);
			this.pScreen.TabIndex = 0;
			this.pScreen.Click += new System.EventHandler(this.pScreen_Click);
			this.pScreen.Paint += new System.Windows.Forms.PaintEventHandler(this.pScreen_Paint);
			this.pScreen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pScreen_MouseDown);
			this.pScreen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pScreen_MouseMove);
			this.pScreen.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pScreen_MouseUp);
			// 
			// btnCreateDevice
			// 
			this.btnCreateDevice.Location = new System.Drawing.Point(23, 38);
			this.btnCreateDevice.Name = "btnCreateDevice";
			this.btnCreateDevice.Size = new System.Drawing.Size(123, 30);
			this.btnCreateDevice.TabIndex = 1;
			this.btnCreateDevice.Text = "Create Device";
			this.btnCreateDevice.UseVisualStyleBackColor = true;
			this.btnCreateDevice.Click += new System.EventHandler(this.btnCreateDevice_Click);
			// 
			// labelLog
			// 
			this.labelLog.Location = new System.Drawing.Point(20, 9);
			this.labelLog.Name = "labelLog";
			this.labelLog.Size = new System.Drawing.Size(602, 13);
			this.labelLog.TabIndex = 2;
			// 
			// btnLoadCNC
			// 
			this.btnLoadCNC.Location = new System.Drawing.Point(152, 38);
			this.btnLoadCNC.Name = "btnLoadCNC";
			this.btnLoadCNC.Size = new System.Drawing.Size(123, 30);
			this.btnLoadCNC.TabIndex = 4;
			this.btnLoadCNC.Text = "Load CNC";
			this.btnLoadCNC.UseVisualStyleBackColor = true;
			this.btnLoadCNC.Click += new System.EventHandler(this.btnLoadCNC_Click);
			// 
			// btnTop
			// 
			this.btnTop.Location = new System.Drawing.Point(23, 75);
			this.btnTop.Name = "btnTop";
			this.btnTop.Size = new System.Drawing.Size(44, 37);
			this.btnTop.TabIndex = 5;
			this.btnTop.Text = "Top";
			this.btnTop.UseVisualStyleBackColor = true;
			this.btnTop.Click += new System.EventHandler(this.btnTop_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(73, 75);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(44, 37);
			this.button5.TabIndex = 5;
			this.button5.Text = "Front";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(123, 74);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(44, 37);
			this.button6.TabIndex = 5;
			this.button6.Text = "Right";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.button6_Click);
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(173, 74);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(44, 37);
			this.button7.TabIndex = 5;
			this.button7.Text = "ISO";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.button7_Click);
			// 
			// labelInfo
			// 
			this.labelInfo.AutoSize = true;
			this.labelInfo.Location = new System.Drawing.Point(239, 75);
			this.labelInfo.Name = "labelInfo";
			this.labelInfo.Size = new System.Drawing.Size(35, 13);
			this.labelInfo.TabIndex = 6;
			this.labelInfo.Text = "label1";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(636, 544);
			this.Controls.Add(this.labelInfo);
			this.Controls.Add(this.button7);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.btnTop);
			this.Controls.Add(this.btnLoadCNC);
			this.Controls.Add(this.labelLog);
			this.Controls.Add(this.btnCreateDevice);
			this.Controls.Add(this.pScreen);
			this.Name = "Form1";
			this.Text = "Show Line in window by DirectX";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Panel pScreen;
		private System.Windows.Forms.Button btnCreateDevice;
		private System.Windows.Forms.Label labelLog;
		private System.Windows.Forms.Button btnLoadCNC;
		private System.Windows.Forms.Button btnTop;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Label labelInfo;
	}
}

