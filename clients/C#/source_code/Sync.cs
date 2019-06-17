using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    class Sync
    {
        /// <summary>
        /// Determines differences between the local database and the server and initializes the syncing process.
        /// </summary>
        /// <param name="parameter">new string[] { remoteHeaderString, deletedItemString }</param>
        public static async void Initialize(object parameter)
        {
            bool refresh = false;
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
                Task<List<string>> getHids = DataBaseHelper.GetDataAsList("Select D_hid from Tbl_data;", (int)ColumnCount.SingleColumn);
                List<string> hids = await getHids;
                // DELETE ALL LOCAL ACCOUNTS THAT HAVE BEEN DELETED ON THE SERVER
                for (int i = 0; i < deletedItems.Length; i++)
                {
                    if (hids.Contains(deletedItems[i]))
                    {
                        await DataBaseHelper.ModifyData("DELETE FROM Tbl_data WHERE D_hid = \"" + deletedItems[i] + "\";");
                        refresh = true;
                    }
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

                // DEEP COPY ONLY OUTER LIST (ISN'T IMPLEMENTED BY DEFAULT *sigh*)
                List<List<string>> tempRemoteHeaders = remoteHeaders.ConvertAll(stringList => stringList);

                // ITERATE OVER REMOTE HEADERS
                for (int i = 0; i < tempRemoteHeaders.Count; i++)
                {
                    // GET REMOTE HID AND TIMESTAMP
                    string remoteHid = tempRemoteHeaders[i][0];
                    int remoteTimestamp = Convert.ToInt32(tempRemoteHeaders[i][1]);

                    // DEEP COPY (ISN'T IMPLEMENTED BY DEFAULT *sigh*)
                    List<List<string>> tempLocalHeaders = localHeaders.ConvertAll(stringList => stringList);

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
            GlobalVarPool.countedPackets = 0;
            GlobalVarPool.expectedPacketCount = accountsToUpdate.Count + accountsToGet.Count + localHeaders.Count + (accountsToDelete.Count > 0 ? 1 : 0);
            if (GlobalVarPool.expectedPacketCount > 0)
            {
                GlobalVarPool.countSyncPackets = true;
            }
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
                GlobalVarPool.countSyncPackets = false;
                AutomatedTaskFramework.Tasks.Clear();
                AutomatedTaskFramework.Task.Create(SearchCondition.In, "LOGGED_OUT|NOT_LOGGED_IN", NetworkAdapter.MethodProvider.Logout);
                AutomatedTaskFramework.Task.Create(SearchCondition.Match, null, NetworkAdapter.MethodProvider.Disconnect);
                AutomatedTaskFramework.Tasks.Execute();
                if (refresh)
                {
                    ReloadData();
                }
                else
                {
                    GlobalVarPool.Form1.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        Form1.InvokeSyncAnimationStop();
                        // CustomException.ThrowNew.GenericException("Done. Nothing to do.");
                        GlobalVarPool.SyncButton.Enabled = true;
                    });
                }
            }
        }

        public static async void Finish()
        {
            for (int i = 0; i < GlobalVarPool.selectedAccounts.Count; i++)
            {
                string[] account = new string[] { "host%eq", "url%eq", "uname%eq", "password%eq", "email%eq", "notes%eq", "icon%eq", "hid%eq", "datetime%eq" };
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
                for (int j = 0; j < account.Length; j++)
                {
                    account.SetValue(account[j].Replace("%eq", ""), j);
                }
                Task<List<string>> CheckHidExistsTask = DataBaseHelper.GetDataAsList("SELECT EXISTS(SELECT 1 FROM Tbl_data WHERE D_hid = \"" + values[7] + "\");", 1);
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
                    await DataBaseHelper.ModifyData(query);
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
                    await DataBaseHelper.ModifyData(query);
                }
            }
            GlobalVarPool.selectedAccounts.Clear();
            ReloadData();
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
            await DataBaseHelper.ModifyData("UPDATE Tbl_data SET D_hid = \"" + hid + "\" WHERE D_id = " + localID + ";");
        }

        private static async void ReloadData()
        {
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
                    // TODO: DISPLAY PROGRESS
                    double Percentage = ((((double)RowCounter * ((double)Columns - (double)3)) + (double)i - 3) / (double)Fields) * (double)100;
                    double FinalPercentage = Math.Round(Percentage, 0, MidpointRounding.ToEven);
                }
                RowCounter++;
            }
            GlobalVarPool.Form1.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                Form1.InvokeSyncAnimationStop();
                Form1.InvokeReload();
                GlobalVarPool.SyncButton.Enabled = true;
            });
        }
    }
}
