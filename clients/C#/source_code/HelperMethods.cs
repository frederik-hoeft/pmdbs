using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq.Expressions;
using System.Threading;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json;

namespace pmdbs
{
    public static class HelperMethods
    {
        public static async void ShowCertificateWarning(CryptoHelper.CertificateInformation cert)
        {
            System.Windows.Forms.DialogResult result = new CertificateForm(GlobalVarPool.REMOTE_ADDRESS, cert).ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                GlobalVarPool.foreignRsaKey = cert.PublicKey;
                await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "INSERT INTO Tbl_certificates (C_hash, C_accepted) VALUES (\"", cert.Checksum, "\", \"1\");" }));
                GlobalVarPool.nonce = CryptoHelper.RandomString();
                string encNonce = CryptoHelper.RSAEncrypt(GlobalVarPool.foreignRsaKey, GlobalVarPool.nonce);
                string message = "CKEformat%eq!XML!;key%eq!" + GlobalVarPool.PublicKey + "!;nonce%eq!" + encNonce + "!;";
                HelperMethods.InvokeOutputLabel("Client Key Exchange ...");
                GlobalVarPool.clientSocket.Send(Encoding.UTF8.GetBytes("\x01K" + message + "\x04"));
            }
            else
            {
                NetworkAdapter.MethodProvider.Disconnect();
                GlobalVarPool.commandErrorCode = -2;
            }
        }

        public static List<byte[]> Separate(byte[] source, byte[] separator)
        {
            var Parts = new List<byte[]>();
            var Index = 0;
            byte[] Part;
            for (var i = 0; i < source.Length; ++i)
            {
                for (int j = 0; j < separator.Length; j++)
                {
                    if (source[i].Equals(separator[j]))
                    {
                        Part = new byte[i - Index];
                        Array.Copy(source, Index, Part, 0, Part.Length);
                        Parts.Add(Part);
                        Index = i + separator.Length;
                        i += separator.Length - 1;
                    }
                }
            }
            Part = new byte[source.Length - Index];
            Array.Copy(source, Index, Part, 0, Part.Length);
            Parts.Add(Part);
            return Parts;
        }

        public static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static string GetOS()
        {
            string json = JsonConvert.SerializeObject(OSInfo.GetOS());
            return json.Replace('\"', '§').Replace('\'', '§');
        }

        public static void Prompt(string promptMain, string promptAction)
        {
            GlobalVarPool.promptAction.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.promptAction.Text = promptAction;
            });
            GlobalVarPool.promptEMail.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.promptEMail.Text = "An email containing a verification code has been sent to " + GlobalVarPool.email + ".";
            });
            GlobalVarPool.promptMain.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.promptMain.Text = promptMain;
            });
            GlobalVarPool.promptPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.promptPanel.BringToFront();
            });
            if (GlobalVarPool.promptFromBackgroundThread)
            {
                GlobalVarPool.settingsPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    GlobalVarPool.settingsPanel.BringToFront();
                });
            }
        }

        public static string ToHumanReadableFileSize(this double fileSize, int decimals)
        {
            string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            int i = 0;
            while (fileSize > 1000)
            {
                fileSize /= 1000;
                fileSize = Math.Round(fileSize, decimals, MidpointRounding.AwayFromZero);
                if (i < units.Length)
                {
                    i++;
                }
            }
            return fileSize.ToString() + units[i];
        }

        public static void InvokeOutputLabel(string text)
        {
            if (GlobalVarPool.outputLabelIsValid)
            {
                GlobalVarPool.outputLabel.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    GlobalVarPool.outputLabel.Text = text;
                });
            }
        }

        /// <summary>
        /// Loads the devices list retrieved from the server to the UI.
        /// </summary>
        /// <param name="data"></param>
        public static void LoadDevices(string data)
        {
            string deviceData = data.Replace("', '", "','").Replace("'], ['", "'],['").Replace("['", "").Replace("']", "");
            string[] devices = deviceData.Split(new string[] { "','" }, StringSplitOptions.RemoveEmptyEntries);
            GlobalVarPool.deviceList.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.deviceList.RemoveAll();
            });
            for (int i = 0; i < devices.Length; i++)
            {
                OSInfo.Device device = JsonConvert.DeserializeObject<OSInfo.Device>(devices[i]);
                Image icon;
                OSInfo.OS os = device.OS;
                string name = os.Name;
                if (os.Name.ToLower().Contains("windows"))
                {
                    
                    icon = Properties.Resources.devices_colored_windows;
                }
                else if (os.Name.ToLower().Contains("android"))
                {
                    icon = Properties.Resources.devices_colored_android;
                }
                else
                {
                    icon = Properties.Resources.devices_colored_linux;
                }
                GlobalVarPool.deviceList.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    GlobalVarPool.deviceList.Add(os.Name, icon, devices[i], i);
                });
            }
        }

        /// <summary>
        /// Loads all Settings from the database into GlobalVarPool
        /// </summary>
        public static async Task LoadSettings()
        {
            Task<List<string>> getUserSettings = DataBaseHelper.GetDataAsList("SELECT * FROM Tbl_user LIMIT 1;", (int)ColumnCount.Tbl_user);
            List<string> userSettings = await getUserSettings;
            if (userSettings.Count == 0)
            {
                return;
            }
            GlobalVarPool.username = userSettings[1];
            GlobalVarPool.name = userSettings[2];
            GlobalVarPool.scryptHash = userSettings[3];
            GlobalVarPool.wasOnline = userSettings[4].Equals("1");
            GlobalVarPool.firstUsage = userSettings[5];
            GlobalVarPool.email = userSettings[6];
            GlobalVarPool.cookie = userSettings[7];
            if (GlobalVarPool.wasOnline)
            {
                Task<List<string>> getSettings = DataBaseHelper.GetDataAsList("SELECT * FROM Tbl_settings LIMIT 1;", (int)ColumnCount.Tbl_settings);
                List<string> settings = await getSettings;
                GlobalVarPool.REMOTE_ADDRESS = settings[1];
                GlobalVarPool.REMOTE_PORT = Convert.ToInt32(settings[2]);
            }
        }
        public static async Task ChangeMasterPassword(string password, bool showLoadingScreen)
        {
            InvokeOutputLabel("Creating stage 1 password hash ...");
            string stage1PasswordHash = CryptoHelper.SHA256Hash(password);
            string localAESkey = CryptoHelper.SHA256Hash(stage1PasswordHash.Substring(32, 32));
            string onlinePassword = CryptoHelper.SHA256Hash(stage1PasswordHash.Substring(0, 32));
            GlobalVarPool.localAESkey = localAESkey;
            GlobalVarPool.onlinePassword = onlinePassword;
            DataTable encryptedUserData = GlobalVarPool.UserData.Copy();
            int columns = encryptedUserData.Columns.Count;
            int rowCounter = 0;
            int fields = (columns - 3) * encryptedUserData.Rows.Count;
            foreach (DataRow row in encryptedUserData.Rows)
            {
                for (int i = 3; i < columns; i++)
                {
                    string fieldValue = row[i].ToString();
                    if (!fieldValue.Equals("\x01"))
                    {
                        string encryptedData = CryptoHelper.AESEncrypt(fieldValue, localAESkey);
                        row.BeginEdit();
                        row.SetField(i, encryptedData);
                        row.EndEdit();
                    }
                    double Percentage = ((((double)rowCounter * ((double)columns - (double)3)) + (double)i - 3) / (double)fields) * (double)100;
                    double FinalPercentage = Math.Round(Percentage, 0, MidpointRounding.ToEven);
                    InvokeOutputLabel("Changing your password ... " + FinalPercentage.ToString() + "%");
                }
                rowCounter++;
            }
            InvokeOutputLabel("Creating stage 2 password hash ...");
            Task<string> ScryptTask = Task.Run(() => CryptoHelper.ScryptHash(stage1PasswordHash, GlobalVarPool.firstUsage));
            string stage2PasswordHash = await ScryptTask;
            InvokeOutputLabel("Setting new password ...");
            await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "UPDATE Tbl_user SET U_password = \"", stage2PasswordHash, "\"" }));
            rowCounter = 0;
            int totalRowCount = encryptedUserData.Rows.Count;
            // UPDATE DATABASE
            foreach (DataRow row in encryptedUserData.Rows)
            {
                await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "UPDATE Tbl_data SET D_host = \"", row[3].ToString(), "\", D_url = \"", row[6].ToString(), "\", D_uname = \"", row[4].ToString(), "\", D_password = \"", row[5].ToString(), "\", D_email = \"", row[7].ToString(), "\", D_notes = \"", row[8].ToString(), "\", D_icon = \"", row[9].ToString(), "\", D_hid = \"EMPTY\", D_datetime = \"", TimeConverter.TimeStamp(), "\" WHERE D_id = ", row[0].ToString(), ";" }));
                InvokeOutputLabel("Writing changes ... " + Math.Round(((float)rowCounter / (float)totalRowCount) * 100f,0,MidpointRounding.ToEven).ToString() + "%");
            }
            InvokeOutputLabel("Updating data source ...");
            // UPDATE GlobalVarPool.UserData
            foreach (DataRow row in GlobalVarPool.UserData.Rows)
            {
                row.BeginEdit();
                row.SetField(1, "EMPTY");
                row.EndEdit();
            }
        }
    }
}
