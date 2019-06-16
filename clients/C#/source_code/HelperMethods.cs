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

namespace pmdbs
{
    public struct HelperMethods
    {
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

        public static string GetOS()
        {
            return Environment.OSVersion.VersionString;
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

        public enum LoadingType
        {
            DEFAULT = 0,
            LOGIN = 1
        }

        public static void LoadingHelper(object parameters)
        {
            GlobalVarPool.commandError = false;
            GlobalVarPool.loadingSpinner.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingSpinner.Start();
                GlobalVarPool.loadingSpinner.Visible = true;
            });
            GlobalVarPool.loadingLogo.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingLogo.Visible = true;
            });
            GlobalVarPool.loadingLabel.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingLabel.Visible = true;
            });
            GlobalVarPool.settingsAbort.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.settingsAbort.Visible = false;
            });
            GlobalVarPool.settingsSave.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.settingsSave.Visible = false;
            });
            GlobalVarPool.loadingPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingPanel.BringToFront();
            });
            GlobalVarPool.settingsPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.settingsPanel.BringToFront();
            });

            // PARSE PARAMETER OBJECT TO LIST
            List<object> paramsList = (List<object>)parameters;
            // GET PARAMETERS AND PARSE THEM TO CORRESPONDING DATA TYPES
            System.Windows.Forms.Panel finalPanel = (System.Windows.Forms.Panel)paramsList[0];
            System.Windows.Forms.Label output = (System.Windows.Forms.Label)paramsList[1];
            bool showBackendOutput = (bool)paramsList[2];
            Func<bool> finishCondition = (Func<bool>)paramsList[3];

            // SET GLOBAL VARIABLES
            GlobalVarPool.outputLabelIsValid = showBackendOutput;
            GlobalVarPool.outputLabel = output;

            // WAIT FOR LOADING PROCEDURE TO COMPLETE
            while (!finishCondition() && !GlobalVarPool.connectionLost && !GlobalVarPool.commandError)
            {
                Thread.Sleep(1000);
            }
            switch (GlobalVarPool.loadingType)
            {
                case LoadingType.LOGIN:
                    {
                        break;
                    }
                default:
                    {
                        if (!GlobalVarPool.connectionLost)
                        {
                            AutomatedTaskFramework.Tasks.Clear();
                            AutomatedTaskFramework.Task.Create(SearchCondition.In, "LOGGED_OUT|NOT_LOGGED_IN", NetworkAdapter.MethodProvider.Logout);
                            AutomatedTaskFramework.Task.Create(SearchCondition.Match, null, NetworkAdapter.MethodProvider.Disconnect);
                            AutomatedTaskFramework.Tasks.Execute();
                            while (GlobalVarPool.connected && !GlobalVarPool.commandError)
                            {
                                Thread.Sleep(1000);
                            }
                        }
                        break;
                    }
            }
            
            if (GlobalVarPool.connectionLost)
            {
                GlobalVarPool.previousPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    GlobalVarPool.previousPanel.BringToFront();
                });
            }
            else if (GlobalVarPool.commandError)
            {
                GlobalVarPool.previousPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    GlobalVarPool.previousPanel.BringToFront();
                });
            }
            else
            {
                // INVOKE UI AND HIDE LOADING SCREEN
                finalPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    finalPanel.BringToFront();
                });
            }
            GlobalVarPool.loadingSpinner.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingSpinner.Visible = false;
                GlobalVarPool.loadingSpinner.Stop();
            });
            GlobalVarPool.loadingLogo.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingLogo.Visible = false;
            });
            GlobalVarPool.loadingLabel.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingLabel.Visible = false;
            });
            GlobalVarPool.settingsAbort.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.settingsAbort.Visible = true;
            });
            GlobalVarPool.settingsSave.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.settingsSave.Visible = true;
            });
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
        public static async void ChangeMasterPassword(string password)
        {
            // TODO: CHECK PASSWORD STRENGTH
            InvokeOutputLabel("Creating stage 1 password hash ...");
            string stage1PasswordHash = CryptoHelper.SHA256Hash(password);
            string localAESkey = CryptoHelper.SHA256Hash(stage1PasswordHash.Substring(32, 32));
            string onlinePassword = CryptoHelper.SHA256Hash(stage1PasswordHash.Substring(0, 32));
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
            Task<string> ScryptTask = Task.Run(() => CryptoHelper.SCryptHash(stage1PasswordHash, GlobalVarPool.firstUsage));
            string stage2PasswordHash = await ScryptTask;
            InvokeOutputLabel("Setting new password ...");
            await DataBaseHelper.ModifyData("UPDATE Tbl_user SET U_password = \"" + stage2PasswordHash + "\"");
            foreach (DataRow row in encryptedUserData.Rows)
            {
                
            }
        }
    }
}
