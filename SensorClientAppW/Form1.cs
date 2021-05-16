using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SensorClientAppW
{
    public partial class Form1 : Form
    {
        TheClientV theClient = null;
        public List<WeatherData> weatherData = new List<WeatherData>(); 
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Hello");
            this.subscriptionDataGrid.DataSource = weatherData;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string Host = this.txtBoxHost.Text;
                int Port = Int32.Parse(this.txtBoxPort.Text);
                string UserName = this.txtBoxUserName.Text;
                string Password = this.txtBoxPassword.Text;
                this.theClient = new TheClientV(Host, Port, UserName, Password, this);
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Something Went Wrong");
            }
        }

        public void disableFormGroup()
        {
            this.groupBox1.Enabled = false;
        }

        public void setConnectionStatus(bool status)
        {
            this.lblConnectedStatus.Text = status ? "Connected" : "Connection Failed";
            this.lblConnectedStatus.ForeColor = status ? Color.YellowGreen: Color.DarkRed;
        }

        private void btnFindDevice_Click(object sender, EventArgs e)
        {
            //this.theClient.searchDevices();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.theClient!=null)
            {
                this.theClient.Shutdown();
            }
        }

        public void UpdateDataGrid(WeatherData data)
        {
            this.subscriptionDataGrid.DataSource = null;
            this.weatherData.Add(data);
            this.subscriptionDataGrid.DataSource = weatherData;
            this.subscriptionDataGrid.Update();
            this.subscriptionDataGrid.Refresh();


        }
    }
}
