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

        public static MethodInfo CompileFunction(string config_file)
        {
            string code = @"
        using System;
            
        namespace pmdbs
        {                
            public class BinaryFunction
            {                
                public static bool Condition()
                {
                    return place_holder;
                }
            }
        }
    ";
            string[] finalCode = new string[] { code.Replace("place_holder", config_file) };
            
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters
            {
                // True - memory generation, false - external file generation
                GenerateInMemory = true
            };
            parameters.ReferencedAssemblies.Add(Assembly.GetEntryAssembly().Location);
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, finalCode);
            Type binaryFunction = results.CompiledAssembly.GetType("pmdbs.BinaryFunction");
            return binaryFunction.GetMethod("Condition");
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

        public static void LoadingHelper(object parameters)
        {
            GlobalVarPool.commandError = false;
            GlobalVarPool.loadingSpinner.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
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
            string finishCondition = (string)paramsList[3];

            // SET GLOBAL VARIABLES
            GlobalVarPool.outputLabelIsValid = showBackendOutput;
            GlobalVarPool.outputLabel = output;

            // COMPILE CONDITION AT RUNTIME BECAUSE IT'S IMPOSSIBLE TO PASS A LAMBDA EXPRESSION AS PARAMETER WHEN USING ParameterizedThreadStart()
            // STUPID, OVERCOMPLICATED, OFFICIALLY CONSIDERED "UGLY" BUT IT WORKS ... SOMEHOW *sigh*
            MethodInfo condition = CompileFunction(finishCondition);

            // WAIT FOR LOADING PROCEDURE TO COMPLETE
            while (!(bool)condition.Invoke(null,null) && GlobalVarPool.connected && !GlobalVarPool.commandError)
            {
                Thread.Sleep(1000);
            }
            if (!GlobalVarPool.connected)
            {
                CustomException.ThrowNew.NetworkException("Connection lost!");
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
        /// SYNCHRONIZE LOCAL DATABASE WITH REMOTE DATABASE ON THE SERVER
        /// </summary>
        /// <param name="parameter">new string[] { remoteHeaderString, deletedItemString }</param>
        public static async void Sync(object parameter)
        {
            string[] parameters = (string[])parameter;
            string remoteHeaderString = parameters[0];
            string deletedItemString = parameters[1];
            // HEADERS FORMAT:
            // headers%eq![('HID','1555096481'),('HID','1555097171')]!
            // DELETED FORMAT:
            // deleted%eq![('HID'),('HID')]!
            if (!deletedItemString.Equals("deleted%eq![]!"))
            {
                string cleanedDeletedItemString = deletedItemString.Replace("deleted%eq![('", "").Replace("')]!", "");
                string[] deletedItems = cleanedDeletedItemString.Split(new string[] { "'),('" }, StringSplitOptions.RemoveEmptyEntries);
                // DELETE ALL LOCAL ACCOUNTS THAT HAVE BEEN DELETED ON THE SERVER
                for (int i = 0; i < deletedItems.Length; i++)
                {
                    Task Delete = DataBaseHelper.ModifyData("DELETE FROM Tbl_data WHERE D_hid = \"" + deletedItems[i] + "\";");
                    await Task.WhenAny(Delete);
                }
            }
            // GET LOCAL HEADERS
            Task<List<List<string>>> localHeaderTask = DataBaseHelper.GetDataAs2DList("SELECT D_hid, D_datetime, D_id FROM Tbl_data;", 3);

            List<List<string>> localHeaders = await localHeaderTask;
            List<string> accountsToGet = new List<string>();
            List<string> accountsToUpdate = new List<string>();
            List<string> accountsToDelete = new List<string>();
            if (!remoteHeaderString.Equals("headers%eq![]!"))
            {
                string cleanedRemoteHeaderString = remoteHeaderString.Replace("headers%eq![('", "").Replace("')]!", "");
                string[] splittedRemoteHeader = cleanedRemoteHeaderString.Split(new string[] { "'),('" }, StringSplitOptions.RemoveEmptyEntries);
                List<List<string>> remoteHeaders = new List<List<string>>();
                // APPEND REMOTE HEADERS TO LIST
                for (int i = 0; i < splittedRemoteHeader.Length; i++)
                {
                    remoteHeaders.Add(splittedRemoteHeader[i].Split(new string[] { "','" }, StringSplitOptions.RemoveEmptyEntries).ToList());
                }

                List<List<string>> tempRemoteHeaders = remoteHeaders;
                
                // ITERATE OVER REMOTE HEADERS
                for (int i = 0; i < tempRemoteHeaders.Count; i++)
                {
                    // GET REMOTE HID AND TIMESTAMP
                    string remoteHid = tempRemoteHeaders[i][0];
                    int remoteTimestamp = Convert.ToInt32(tempRemoteHeaders[i][1]);
                    List<List<string>> tempLocalHeaders = localHeaders;
                    //ITERATE OVER LOCAL HEADERS
                    for (int j = 0; j < tempLocalHeaders.Count; j++)
                    {
                        // GET LOCAL HID AND TIMESTAMPS
                        string localHid = tempLocalHeaders[j][0];
                        int localTimestamp = Convert.ToInt32(tempLocalHeaders[j][1]);
                        // FIND MATCHING REMOTE AND LOCAL HIDS
                        if (remoteHid.Equals(localHid))
                        {
                            // GET ALL HIDS WHERE THE REMOTE HID IS NEWER
                            if (remoteTimestamp > localTimestamp)
                            {
                                accountsToGet.Add(remoteHid);
                            }
                            // GET ALL HIDS WHERE THE LOCAL HID IS NEWER --> IGNORE IF THEY'RE THE SAME AGE
                            else if (remoteTimestamp != localTimestamp)
                            {
                                // UPDATE SERVER (ADD LOCAL ID TO accountsToUpdate LIST)
                                accountsToUpdate.Add(localHeaders[j][2]);
                            }
                            // REMOVE THEM FROM THE LISTS
                            localHeaders.Remove(tempLocalHeaders[j]);
                            remoteHeaders.Remove(tempRemoteHeaders[i]);
                        }
                    }
                }
                // DOWNLOAD ALL DATA THAT IS NOT PRESENT ON THE CLIENT YET
                for (int i = 0; i < remoteHeaders.Count; i++)
                {
                    string hid = remoteHeaders[i][0];
                    Task<List<string>> TaskCheckExists = DataBaseHelper.GetDataAsList("SELECT EXISTS (SELECT 1 FROM Tbl_delete WHERE DEL_hid = \"" + hid + "\" LIMIT 1);", 1);
                    List<string> TaskCheckExistsResult = await TaskCheckExists;
                    bool isDeleted = Convert.ToBoolean(Convert.ToInt32(TaskCheckExistsResult[0]));
                    if (isDeleted)
                    {
                        accountsToDelete.Add(hid);
                    }
                    else
                    {
                        accountsToGet.Add(hid);
                    }
                }
            }
            GlobalVarPool.countSyncPackets = true;
            GlobalVarPool.expectedPacketCount = accountsToUpdate.Count + accountsToGet.Count + localHeaders.Count + (accountsToDelete.Count > 0 ? 1 : 0);
            // DELETE ON SERVER
            if (accountsToDelete.Count > 0)
            {
                NetworkAdapter.MethodProvider.Delete(accountsToDelete);
            }
            // UPLOAD ALL DATA THAT IS NOT PRESENT ON THE SERVER YET
            for (int i = 0; i < localHeaders.Count; i++)
            {
                string id = localHeaders[i][2];
                Task<List<string>> GetAccount = DataBaseHelper.GetDataAsList("SELECT * FROM Tbl_data WHERE D_id = \"" + id + "\" LIMIT 1;", (int)ColumnCount.Tbl_data);
                List<string> account = await GetAccount;
                // UPLOAD DATA
                NetworkAdapter.MethodProvider.Insert(account);
            }
            // UPDATE DATA ON THE SERVER
            for (int i = 0; i < accountsToUpdate.Count; i++)
            {
                Task<List<string>> GetAccount = DataBaseHelper.GetDataAsList("SELECT * FROM Tbl_data WHERE D_id = \"" + accountsToUpdate[i] + "\" LIMIT 1;", (int)ColumnCount.Tbl_data);
                List<string> account = await GetAccount;
                NetworkAdapter.MethodProvider.Update(account);
            }
            // DOWNLOAD FROM SERVER
            if (accountsToGet.Count > 0)
            {
                NetworkAdapter.MethodProvider.Select(accountsToGet);
            }
            if (GlobalVarPool.expectedPacketCount == 0)
            {
                GlobalVarPool.Form1.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    CustomException.ThrowNew.GenericException("Done. Nothing to do.");
                    GlobalVarPool.SyncButton.Enabled = true;
                });
            }
        }

        public static async void FinishSync()
        {
            for (int i = 0; i < GlobalVarPool.selectedAccounts.Count; i++)
            {
                string[] account = new string[] { "host", "url", "uname", "password", "email", "notes", "icon", "hid", "datetime" };
                string[] values = new string[] { null, null, null, null, null, null, null, null, null };
                string[] accountParts = GlobalVarPool.selectedAccounts[i].Split(';');
                try
                {
                    for (int j = 0; j < accountParts.Length; j++)
                    {
                        if (accountParts[j].Equals("mode%eq!SELECT!"))
                        {
                            continue;
                        }
                        for (int k = 0; k < account.Length; k++)
                        {
                            if (accountParts[j].Contains(account[k]))
                            {
                                values.SetValue(accountParts[j].Split('!')[1], k);
                                break;
                            }
                        }
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    CustomException.ThrowNew.IndexOutOfRangeException(e.ToString());
                }
                if (values.Contains(null))
                {
                    CustomException.ThrowNew.GenericException("NULL value in sync values!");
                    continue;
                }
                Task<List<string>> CheckHidExistsTask = DataBaseHelper.GetDataAsList("SELECT EXISTS(SELECT 1 FROM Tbl_data WHERE D_hid = \"" + values[7] + "\");",1);
                List<string> hidExists = await CheckHidExistsTask;
                if (Convert.ToBoolean(Convert.ToInt32(hidExists[0])))
                {
                    string query = "UPDATE Tbl_data SET ";
                    bool isFirstValue = true;
                    try
                    {
                        for (int j = 0; j < account.Length; j++)
                        {
                            if (isFirstValue)
                            {
                                query += "D_" + account[j] + " = \"" + values[j] + "\"";
                                isFirstValue = false;
                            }
                            else
                            {
                                query += ", D_" + account[j] + " = \"" + values[j] + "\"";
                            }
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        CustomException.ThrowNew.IndexOutOfRangeException(e.ToString());
                    }
                    query += " WHERE D_hid = \"" + values[7] + "\";";
                    Task Update = DataBaseHelper.ModifyData(query);
                    await Task.WhenAny(Update);
                }
                else
                {
                    string query = "INSERT INTO Tbl_data (";
                    bool isFirst = true;
                    try
                    {
                        for (int j = 0; j < account.Length; j++)
                        {
                            if (isFirst)
                            {
                                query += "D_" + account[j];
                                isFirst = false;
                            }
                            else
                            {
                                query += ", D_" + account[j];
                            }
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        CustomException.ThrowNew.IndexOutOfRangeException(e.ToString());
                    }
                    query += ") VALUES (";
                    isFirst = true;
                    try
                    {
                        for (int j = 0; j < values.Length; j++)
                        {
                            if (isFirst)
                            {
                                query += "\"" + values[j] + "\"";
                                isFirst = false;
                            }
                            else
                            {
                                query += ", \"" + values[j] + "\"";
                            }
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        CustomException.ThrowNew.IndexOutOfRangeException(e.ToString());
                    }
                    query += ");";
                    Task Insert = DataBaseHelper.ModifyData(query);
                    await Task.WhenAny(Insert);
                }
            }
            GlobalVarPool.selectedAccounts.Clear();
            // TODO: INVOKE UI
            Task<DataTable> GetData = DataBaseHelper.GetDataAsDataTable("SELECT D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes, D_icon FROM Tbl_data;", (int)ColumnCount.Tbl_data);
            GlobalVarPool.UserData = await GetData;
            int Columns = GlobalVarPool.UserData.Columns.Count;
            int RowCounter = 0;
            int Fields = (Columns - 3) * GlobalVarPool.UserData.Rows.Count;
            foreach (DataRow Row in GlobalVarPool.UserData.Rows)
            {
                for (int i = 3; i < Columns; i++)
                {
                    string FieldValue = Row[i].ToString();
                    if (!FieldValue.Equals("\x01"))
                    {
                        string decryptedData = CryptoHelper.AESDecrypt(FieldValue, GlobalVarPool.localAESkey);
                        Row.BeginEdit();
                        Row.SetField(i, decryptedData);
                        Row.EndEdit();
                    }
                    double Percentage = ((((double)RowCounter * ((double)Columns - (double)3)) + (double)i - 3) / (double)Fields) * (double)100;
                    double FinalPercentage = Math.Round(Percentage, 0, MidpointRounding.ToEven);
                }
                RowCounter++;
            }
            GlobalVarPool.Form1.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.SyncButton.Enabled = true;
                Form1.InvokeReload();
            });
        }

        public static async void SetHid(object parameter)
        {
            string[] parameters = (string[])parameter;
            string localID = string.Empty;
            string hid = string.Empty;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Contains("local_id"))
                {
                    localID = parameters[i].Split('!')[1];
                }
                else if (parameters[i].Contains("hashed_id"))
                {
                    hid = parameters[i].Split('!')[1];
                }
            }
            if (new string[] { localID, hid }.Contains(string.Empty))
            {
                CustomException.ThrowNew.GenericException("Missing parameter in SetHid().");
                return;
            }
            Task Update = DataBaseHelper.ModifyData("UPDATE Tbl_data SET D_hid = \"" + hid + "\" WHERE D_id = " + localID + ";");
            await Task.WhenAny(Update);
        }
    }
}
