using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.IO;

namespace pmdbs
{
    public partial class DeviceForm : MetroFramework.Forms.MetroForm
    {
        public DeviceForm()
        {
            InitializeComponent();
            labelDeviceId.Text = "Device ID: " + CryptoHelper.SHA256HashBase64("O32TCYHJ6asG9uFQ1EqbEQz5Ikg2Fwf0/BqWQWnnhBM35eKHU2SJSVaoOzfsk7o9Iuuzhq0LvgX0jwfTXEeiOw==");
            GetDeviceInfoTemp();
        }

        private void GetDeviceInfoTemp()
        {
            
        }

        private void labelTitle_Click(object sender, EventArgs e)
        {
            GetDeviceInfoTemp();
        }
    }
}
