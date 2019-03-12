using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE;

namespace debugclient
{
    public struct IO
    {
        public static void Start()
        {
            ConsoleExtension.PrintF("USER INPUT NOW SUPPORTED");
            ConsoleExtension.PrintF("type \"help\" to display a list of commands");

            GlobalVarPool.serverName = Network.GetHost(GlobalVarPool.ip);
            GlobalVarPool.currentUser = "<" + GlobalVarPool.serverName + ">";
            try
            {
                while (true)
                {
                    string[] parameters = ConsoleExtension.Input(GlobalVarPool.currentUser + " ", ConsoleColorExtension.Red, 1).Split(' ');
                    string command = parameters[0];
                    switch (command.ToLower())
                    {
                        case "exit":
                            {
                                Commands.Exit(parameters);
                                break;
                            }
                        case "register":
                            {
                                Commands.Register(parameters);
                                break;
                            }
                        case "insert":
                            {
                                Commands.Insert(parameters);
                                break;
                            }
                        case "select":
                            {
                                Commands.Select(parameters);
                                break;
                            }
                        case "update":
                            {
                                Commands.Update(parameters);
                                break;
                            }
                        case "customencrypted":
                            {
                                Commands.CustomEncrypted(parameters);
                                break;
                            }
                        case "custom":
                            {
                                Commands.Custom(parameters);
                                break;
                            }
                        case "sync":
                            {
                                Commands.FetchSync(parameters);
                                break;
                            }
                        case "alldata":
                            {
                                Commands.FetchAll(parameters);
                                break;
                            }
                        case "login":
                            {
                                Commands.Login(parameters);
                                break;
                            }
                        case "logout":
                            {
                                Commands.Logout(parameters);
                                break;
                            }
                        case "su":
                            {
                                Commands.Sudo(parameters);
                                break;
                            }
                        case "shutdown":
                            {
                                Commands.Shutdown(parameters);
                                break;
                            }
                        case "reboot":
                            {
                                Commands.Reboot(parameters);
                                break;
                            }
                        case "start":
                            {
                                Commands.Start(parameters);
                                break;
                            }
                        case "serverlog":
                            {
                                Commands.GetServerLog(parameters);
                                break;
                            }
                        case "clientlog":
                            {
                                Commands.GetClientLog(parameters);
                                break;
                            }
                        case "listallclients":
                            {
                                Commands.ListAllClients(parameters);
                                break;
                            }
                        case "error":
                            {
                                if (GlobalVarPool.debugging)
                                {
                                    Commands.Error(parameters);
                                }
                                else
                                {
                                    ConsoleExtension.PrintF("Enable Debug Mode to use this command");
                                }
                                break;
                            }
                        case "getcookie":
                            {
                                Commands.GetCookie(parameters);
                                break;
                            }
                        case "kick":
                            {
                                Commands.Kick(parameters);
                                break;
                            }
                        case "activateaccount":
                            {
                                Commands.ActivateAccount(parameters);
                                break;
                            }
                        case "confirmnewdevice":
                            {
                                Commands.ConfirmNewDevice(parameters);
                                break;
                            }
                        case "addadmindevice":
                            {
                                Commands.NewAdminDevice(parameters);
                                break;
                            }
                        case "initadminpwchange":
                            {
                                Commands.InitAdminPasswordChange(parameters);
                                break;
                            }
                        case "commitadminpwchange":
                            {
                                Commands.CommitAdminPasswordChange(parameters);
                                break;
                            }
                        case "initpwchange":
                            {
                                Commands.InitPasswordChange(parameters);
                                break;
                            }
                        case "commitpwchange":
                            {
                                Commands.CommitPasswordChange(parameters);
                                break;
                            }
                        case "initdelaccount":
                            {
                                Commands.InitDeleteAccount(parameters);
                                break;
                            }
                        case "commitdelaccount":
                            {
                                Commands.CommitDeleteAccount(parameters);
                                break;
                            }
                        case "banclient":
                            {
                                Commands.BanClient(parameters);
                                break;
                            }
                        case "banaccount":
                            {
                                Commands.BanAccount(parameters);
                                break;
                            }
                        case "listallusers":
                            {
                                Commands.ListAllUsers(parameters);
                                break;
                            }
                        case "help":
                            {
                                Commands.Help(parameters);
                                break;
                            }
                        case "getaccountactivity":
                            {
                                Commands.GetAccountActivity(parameters);
                                break;
                            }
                        case "changeemailaddress":
                            {
                                Commands.ChangeEmailAddress(parameters);
                                break;
                            }
                        case "resendcode":
                            {
                                Commands.ResendCode(parameters);
                                break;
                            }
                        case "changename":
                            {
                                Commands.ChangeName(parameters);
                                break;
                            }
                        case "enabledebugging":
                            {
                                Commands.EnableDebugging(parameters);
                                break;
                            }
                        case "disabledebugging":
                            {
                                Commands.DisableDebugging(parameters);
                                break;
                            }
                        case "checkcredentials":
                            {
                                Commands.CheckCredentials(parameters);
                                break;
                            }
                        case "delete":
                            {
                                Commands.Delete(parameters);
                                break;
                            }
                        default:
                            {
                                ConsoleExtension.PrintF("unknown command: \"" + command + "\"");
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleExtension.PrintF(e.Message);
            }
        }
    }
}
