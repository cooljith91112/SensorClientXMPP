
namespace SensorClientAppW
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBoxPassword = new System.Windows.Forms.TextBox();
            this.txtBoxPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBoxHost = new System.Windows.Forms.TextBox();
            this.txtBoxUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFindDevice = new System.Windows.Forms.Button();
            this.subscriptionDataGrid = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblConnectedStatus = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.subscriptionDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnLogin);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtBoxPassword);
            this.groupBox1.Controls.Add(this.txtBoxPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtBoxHost);
            this.groupBox1.Controls.Add(this.txtBoxUserName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(713, 126);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "XMPP Setup";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(590, 87);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(509, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Password";
            // 
            // txtBoxPassword
            // 
            this.txtBoxPassword.Location = new System.Drawing.Point(509, 41);
            this.txtBoxPassword.Name = "txtBoxPassword";
            this.txtBoxPassword.PasswordChar = '*';
            this.txtBoxPassword.Size = new System.Drawing.Size(156, 23);
            this.txtBoxPassword.TabIndex = 3;
            this.txtBoxPassword.Text = "123456";
            // 
            // txtBoxPort
            // 
            this.txtBoxPort.Location = new System.Drawing.Point(208, 41);
            this.txtBoxPort.Name = "txtBoxPort";
            this.txtBoxPort.Size = new System.Drawing.Size(100, 23);
            this.txtBoxPort.TabIndex = 3;
            this.txtBoxPort.Text = "5222";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(208, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // txtBoxHost
            // 
            this.txtBoxHost.Location = new System.Drawing.Point(7, 42);
            this.txtBoxHost.Name = "txtBoxHost";
            this.txtBoxHost.Size = new System.Drawing.Size(176, 23);
            this.txtBoxHost.TabIndex = 1;
            // 
            // txtBoxUserName
            // 
            this.txtBoxUserName.Location = new System.Drawing.Point(331, 41);
            this.txtBoxUserName.Name = "txtBoxUserName";
            this.txtBoxUserName.Size = new System.Drawing.Size(155, 23);
            this.txtBoxUserName.TabIndex = 1;
            this.txtBoxUserName.Text = "demoControlApp2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(331, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "User Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host/Broker";
            // 
            // btnFindDevice
            // 
            this.btnFindDevice.Image = global::SensorClientAppW.Properties.Resources.sicon;
            this.btnFindDevice.Location = new System.Drawing.Point(760, 45);
            this.btnFindDevice.Name = "btnFindDevice";
            this.btnFindDevice.Size = new System.Drawing.Size(123, 41);
            this.btnFindDevice.TabIndex = 1;
            this.btnFindDevice.Text = "Find Device";
            this.btnFindDevice.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFindDevice.UseVisualStyleBackColor = true;
            this.btnFindDevice.Visible = false;
            this.btnFindDevice.Click += new System.EventHandler(this.btnFindDevice_Click);
            // 
            // subscriptionDataGrid
            // 
            this.subscriptionDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.subscriptionDataGrid.Location = new System.Drawing.Point(13, 142);
            this.subscriptionDataGrid.Name = "subscriptionDataGrid";
            this.subscriptionDataGrid.RowTemplate.Height = 25;
            this.subscriptionDataGrid.Size = new System.Drawing.Size(713, 283);
            this.subscriptionDataGrid.TabIndex = 2;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(733, 22);
            this.statusStrip1.Stretch = false;
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblConnectedStatus
            // 
            this.lblConnectedStatus.AutoSize = true;
            this.lblConnectedStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblConnectedStatus.ForeColor = System.Drawing.Color.Red;
            this.lblConnectedStatus.Location = new System.Drawing.Point(13, 432);
            this.lblConnectedStatus.Name = "lblConnectedStatus";
            this.lblConnectedStatus.Size = new System.Drawing.Size(83, 13);
            this.lblConnectedStatus.TabIndex = 4;
            this.lblConnectedStatus.Text = "Not Connected";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 450);
            this.Controls.Add(this.lblConnectedStatus);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.subscriptionDataGrid);
            this.Controls.Add(this.btnFindDevice);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Sensor Control Demo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.subscriptionDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBoxPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxHost;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBoxPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBoxUserName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnFindDevice;
        private System.Windows.Forms.DataGridView subscriptionDataGrid;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Label lblConnectedStatus;
    }
}

