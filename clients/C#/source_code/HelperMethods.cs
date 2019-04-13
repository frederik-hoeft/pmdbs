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
        /// <param name="remoteHeaderString">REMOTE HEADERS</param>
        /// <param name="deletedItemString">REMOTE DELETED HEADERS</param>
        public static async void Sync(string remoteHeaderString, string deletedItemString)
        {
            // HEADERS FORMAT:
            // headers%eq![('HID','1555096481'),('HID','1555097171')]!
            // DELETED FORMAT:
            // deleted%eq![('HID'),('HID')]!
            string cleanedDeletedItemString = deletedItemString.Replace("deleted%eq![('", "").Replace("')]!", "");
            string[] deletedItems = cleanedDeletedItemString.Split(new string[] { "'),('" }, StringSplitOptions.RemoveEmptyEntries);
            // DELETE ALL LOCAL ACCOUNTS THAT HAVE BEEN DELETED ON THE SERVER
            for (int i = 0; i < deletedItems.Length; i++)
            {
                Task Delete = DataBaseHelper.ModifyData("DELETE FROM Tbl_data WHERE D_hid = \"" + deletedItems[i] + "\";");
                await Task.WhenAny(Delete);
            }
            string cleanedRemoteHeaderString = remoteHeaderString.Replace("headers%eq![('", "").Replace("')]!", "");
            string[] splittedRemoteHeader = cleanedRemoteHeaderString.Split(new string[] { "'),('" }, StringSplitOptions.RemoveEmptyEntries);
            List<List<string>> remoteHeaders = new List<List<string>>();
            // APPEND REMOTE HEADERS TO LIST
            for (int i = 0; i < splittedRemoteHeader.Length; i++)
            {
                remoteHeaders.Add(splittedRemoteHeader[i].Split(new string[] { "','" },StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            // GET LOCAL HEADERS
            Task<List<List<string>>> localHeaderTask = DataBaseHelper.GetDataAs2DList("SELECT D_hid, D_datetime, D_id FROM Tbl_data;", 3);

            List<List<string>> localHeaders = await localHeaderTask;
            List<List<string>> tempRemoteHeaders = remoteHeaders;
            List<string> accountsToGet = new List<string>();
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
                    string localHid = tempLocalHeaders[j][0];
                    int localTimestamp = Convert.ToInt32(tempLocalHeaders[j][1]);
                    if (remoteHid.Equals(localHid))
                    {
                        if (remoteTimestamp > localTimestamp)
                        {
                            accountsToGet.Add(remoteHid);
                        }
                        else if(remoteTimestamp != localTimestamp)
                        {
                            // UPDATE SERVER
                            string id = localHeaders[j][2];
                            Task<List<string>> GetAccount = DataBaseHelper.GetDataAsList("SELECT * FROM Tbl_data WHERE D_id = \"" + id + "\" LIMIT 1;", (int)ColumnCount.Tbl_data);
                            List<string> account = await GetAccount;
                            NetworkAdapter.MethodProvider.Update(account);
                        }
                        localHeaders.Remove(tempLocalHeaders[j]);
                        remoteHeaders.Remove(tempRemoteHeaders[i]);
                    }
                }
            }
            // DOWNLOAD ALL DATA THAT IS NOT PRESENT ON THE CLIENT YET
            List<string> accountsToDelete = new List<string>();
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
            // DELETE ON SERVER
            NetworkAdapter.MethodProvider.Delete(accountsToDelete);
            // UPLOAD ALL DATA THAT IS NOT PRESENT ON THE SERVER YET
            for (int i = 0; i < localHeaders.Count; i++)
            {
                string id = localHeaders[i][2];
                Task<List<string>> GetAccount = DataBaseHelper.GetDataAsList("SELECT * FROM Tbl_data WHERE D_id = \"" + id + "\" LIMIT 1;", (int)ColumnCount.Tbl_data);
                List<string> account = await GetAccount;
                // UPLOAD DATA
                NetworkAdapter.MethodProvider.Insert(account);
            }
            // DOWNLOAD FROM SERVER
            NetworkAdapter.MethodProvider.Select(accountsToGet);
        }
    }
}
