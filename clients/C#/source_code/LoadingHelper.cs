using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pmdbs
{
    /// <summary>
    /// Provides backend control for the loading screen.
    /// </summary>
    public static class LoadingHelper
    {
        /// <summary>
        /// Provides different loading presets,
        /// </summary>
        public enum LoadingType
        {
            /// <summary>
            /// The default preset.
            /// </summary>
            DEFAULT = 0,
            /// <summary>
            /// Preset for Login from settings.
            /// </summary>
            LOGIN = 1,
            /// <summary>
            /// Preset for Register from settings.
            /// </summary>
            REGISTER = 2,
            /// <summary>
            /// Preset for PasswordChange from settings.
            /// </summary>
            PASSWORD_CHANGE = 3
        }

        /// <summary>
        /// Controls the loading screen and checks periodically if the finish conditions have been met.
        /// </summary>
        /// <param name="parameters">finalPanel, outputLabel, showBackendOutput, finishCondition</param>
        public static async void Load(object parameters)
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
                    if (GlobalVarPool.promptCommand.Equals("VERIFY_PASSWORD_CHANGE"))
                    {
                        HelperMethods.Prompt("Verify password change", "Looks like your trying to change your password.");
                        retry = true;
                    }
                }
            }
            if (GlobalVarPool.commandErrorCode == -2)
            {
                AutomatedTaskFramework.Tasks.Clear();
                AutomatedTaskFramework.Task.Create(TaskType.FireAndForget, NetworkAdapter.MethodProvider.Disconnect);
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
                            await HelperMethods.ChangeMasterPassword(GlobalVarPool.plainMasterPassword, false);
                            AutomatedTaskFramework.Tasks.Clear();
                            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "AD_OUTDATED|AD_UPTODATE", NetworkAdapter.MethodProvider.GetAccountDetails);
                            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "FETCH_SYNC", NetworkAdapter.MethodProvider.Sync);
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
                            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "FETCH_SYNC", NetworkAdapter.MethodProvider.Sync);
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
                                    await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "INSERT INTO Tbl_delete (DEL_hid) VALUES (\"", hids[i], "\");" }));
                                }
                            }
                            await HelperMethods.ChangeMasterPassword(GlobalVarPool.plainMasterPassword, false);
                            AutomatedTaskFramework.Tasks.Clear();
                            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "FETCH_SYNC", NetworkAdapter.MethodProvider.Sync);
                            AutomatedTaskFramework.Tasks.Execute();
                            while (GlobalVarPool.connected && new int[] { -1, 0 }.Contains(GlobalVarPool.commandErrorCode))
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
                                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "LOGGED_OUT|NOT_LOGGED_IN", NetworkAdapter.MethodProvider.Logout);
                                AutomatedTaskFramework.Task.Create(TaskType.FireAndForget, NetworkAdapter.MethodProvider.Disconnect);
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
            if (GlobalVarPool.connectionLost || GlobalVarPool.commandErrorCode == -2)
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
        }
    }
}
