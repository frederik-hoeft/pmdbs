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
using Newtonsoft.Json;

namespace pmdbs
{
    public partial class DeviceForm : MetroFramework.Forms.MetroForm
    {
        private readonly string cookie = string.Empty;
        public DeviceForm(string jsonData)
        {
            InitializeComponent();
            WinAPI.PreventFlickering(this);
            OSInfo.Device device = JsonConvert.DeserializeObject<OSInfo.Device>(jsonData);
            cookie = device.DeviceId;
            LoadData(device);
        }

        private void LoadData(OSInfo.Device device)
        {
            labelDeviceId.Text = "Device ID: " + CryptoHelper.SHA256HashBase64(device.DeviceId);
            OSInfo.OS os = device.OS;
            if (os.Name.ToLower().Contains("windows"))
            {

                pictureBoxLogo.Image = Properties.Resources.devices_colored_windows;
            }
            else if (os.Name.ToLower().Contains("android"))
            {
                pictureBoxLogo.Image = Properties.Resources.devices_colored_android;
            }
            else
            {
                pictureBoxLogo.Image = Properties.Resources.devices_colored_linux;
            }
            labelTitle.Text = GlobalVarPool.cookie.Equals(device.DeviceId) ? os.Name + " (this device)" : os.Name;
            labelArchitecture.Text = os.Architecture;
            labelDeviceName.Text = os.DeviceName;
            labelEdition.Text = os.Edition;
            labelIp.Text = device.IP;
            if (device.LastSeen.Equals("online"))
            {
                lunaSmallCardIsOnline.Header = "Online";
                lunaSmallCardIsOnline.Image = Properties.Resources.confirmed2;
                labelLastSeen.Text = "Just now";
            }
            else
            {
                lunaSmallCardIsOnline.Header = "Offline";
                lunaSmallCardIsOnline.Image = Properties.Resources.breach;
                labelLastSeen.Text = TimeConverter.UnixTimeStampToDateTime(Convert.ToDouble(device.LastSeen)).ToString();
            }
            
            labelProcessor.Text = os.Processor;
            labelServicePack.Text = string.IsNullOrEmpty(os.ServicePack) ? "-" : os.ServicePack;
            labelUsername.Text = os.UserName;
            labelVersion.Text = os.Version;
            labelMemory.Text = Convert.ToDouble(os.PhysicalMemory).ToHumanReadableFileSize(1);
        }

        private void LunaAnimatedButtonLogout_Click(object sender, EventArgs e)
        {
            lunaAnimatedButtonLogout.Enabled = false;
            AutomatedTaskFramework.Tasks.Clear();
            if (!GlobalVarPool.connected)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DEVICE_AUTHORIZED", NetworkAdapter.MethodProvider.Connect);
            }
            if (!GlobalVarPool.isUser)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login);
            }
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "UNLINK_SUCCESSFUL", () => NetworkAdapter.MethodProvider.RemoveDevice(cookie));
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DTADEVdata%eq", NetworkAdapter.MethodProvider.GetDevices);
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "LOGGED_OUT|NOT_LOGGED_IN", NetworkAdapter.MethodProvider.Logout);
            AutomatedTaskFramework.Task.Create(TaskType.Interactive,NetworkAdapter.MethodProvider.Disconnect,new Func<bool>(delegate 
            {
                return !PDTPClient.ThreadRunning;
            }));
            AutomatedTaskFramework.Tasks.Execute();
            GlobalVarPool.deviceList.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.deviceList.RemoveAll();
            });
            this.DialogResult = DialogResult.Cancel;
        }

        private void WindowButtonClose_OnClickEvent(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void WindowButtonMinimize_OnClickEvent(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void DeviceForm_Shown(object sender, EventArgs e)
        {
            this.Focus();
        }
    }
}
