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
            LOGIN = 1,
            REGISTER = 2,
            PASSWORD_CHANGE = 3
        }

        public static async void LoadingHelper(object parameters)
        {
            GlobalVarPool.commandErrorCode = -1;
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
            bool retry = true;
            while (retry)
            {
                retry = false;
                while (!finishCondition() && !GlobalVarPool.connectionLost && GlobalVarPool.commandErrorCode == -1 && GlobalVarPool.commandErrorCode != 0)
                {
                    Thread.Sleep(1000);
                }
                if (GlobalVarPool.commandErrorCode == 1)
                {
                    if (GlobalVarPool.promptCommand.Equals("CONFIRM_NEW_DEVICE"))
                    {

                    }
                    Prompt("Confirm new device", "Looks like your trying to login from a new device.");
                    retry = true;
                }
            }
            if (GlobalVarPool.commandErrorCode == -2)
            {
                AutomatedTaskFramework.Tasks.Clear();
                AutomatedTaskFramework.Task.Create(SearchCondition.Match, null, NetworkAdapter.MethodProvider.Disconnect);
                AutomatedTaskFramework.Tasks.Execute();
                while (GlobalVarPool.connected)
                {
                    Thread.Sleep(1000);
                }
            }
            else if (GlobalVarPool.connectionLost)
            {
                AutomatedTaskFramework.Tasks.Clear();
            }
            else
            {
                switch (GlobalVarPool.loadingType)
                {
                    case LoadingType.LOGIN:
                        {
                            GlobalVarPool.loadingType = LoadingType.DEFAULT;
                            await ChangeMasterPassword(GlobalVarPool.plainMasterPassword, false);
                            AutomatedTaskFramework.Tasks.Clear();
                            AutomatedTaskFramework.Task.Create(SearchCondition.Contains, "FETCH_SYNC", NetworkAdapter.MethodProvider.Sync);
                            AutomatedTaskFramework.Tasks.Execute();
                            while (GlobalVarPool.connected && GlobalVarPool.commandErrorCode == -1)
                            {
                                Thread.Sleep(1000);
                            }
                            break;
                        }
                    case LoadingType.REGISTER:
                        {
                            GlobalVarPool.loadingType = LoadingType.DEFAULT;
                            AutomatedTaskFramework.Tasks.Clear();
                            AutomatedTaskFramework.Task.Create(SearchCondition.Contains, "FETCH_SYNC", NetworkAdapter.MethodProvider.Sync);
                            AutomatedTaskFramework.Tasks.Execute();
                            while (GlobalVarPool.connected && GlobalVarPool.commandErrorCode == -1)
                            {
                                Thread.Sleep(1000);
                            }
                            break;
                        }
                    case LoadingType.PASSWORD_CHANGE:
                        {
                            GlobalVarPool.loadingType = LoadingType.DEFAULT;
                            using (Task<List<string>> GetHids = DataBaseHelper.GetDataAsList("SELECT D_hid FROM Tbl_data;", 1))
                            {
                                List<string> hids = await GetHids;
                                for (int i = 0; i < hids.Count; i++)
                                {
                                    await DataBaseHelper.ModifyData("INSERT INTO Tbl_delete (DEL_hid) VALUES (\"" + hids[i] + "\");");
                                }
                            }
                            await ChangeMasterPassword(GlobalVarPool.plainMasterPassword, false);
                            AutomatedTaskFramework.Tasks.Clear();
                            AutomatedTaskFramework.Task.Create(SearchCondition.Contains, "FETCH_SYNC", NetworkAdapter.MethodProvider.Sync);
                            AutomatedTaskFramework.Tasks.Execute();
                            while (GlobalVarPool.connected && GlobalVarPool.commandErrorCode == -1)
                            {
                                Thread.Sleep(1000);
                            }
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
                                while (GlobalVarPool.connected && GlobalVarPool.commandErrorCode == -1)
                                {
                                    Thread.Sleep(1000);
                                }
                            }
                            break;
                        }
                }
            }
            GlobalVarPool.outputLabelIsValid = false;
            if (GlobalVarPool.commandErrorCode == 0)
            {
                GlobalVarPool.commandErrorCode = -1;
            }
            else if (GlobalVarPool.connectionLost || GlobalVarPool.commandErrorCode == -2)
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
            Task<string> ScryptTask = Task.Run(() => CryptoHelper.SCryptHash(stage1PasswordHash, GlobalVarPool.firstUsage));
            string stage2PasswordHash = await ScryptTask;
            InvokeOutputLabel("Setting new password ...");
            await DataBaseHelper.ModifyData("UPDATE Tbl_user SET U_password = \"" + stage2PasswordHash + "\"");
            rowCounter = 0;
            int totalRowCount = encryptedUserData.Rows.Count;
            // UPDATE DATABASE
            foreach (DataRow row in encryptedUserData.Rows)
            {
                await DataBaseHelper.ModifyData("UPDATE Tbl_data SET D_host = \"" + row[3].ToString() + "\", D_url = \"" + row[6].ToString() + "\", D_uname = \"" + row[4].ToString() + "\", D_password = \"" + row[5].ToString() + "\", D_email = \"" + row[7].ToString() + "\", D_notes = \"" + row[8].ToString() + "\", D_icon = \"" + row[9].ToString() + "\", D_hid = \"EMPTY\", D_datetime = \"" + TimeConverter.TimeStamp() + "\" WHERE D_id = " + row[0].ToString() + ";");
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
