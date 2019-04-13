using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pmdbs
{
    public static class IOAdapter
    {
        public static void Parse(string command)
        {
            string[] stringParameters = command.Split(' ');
            string keyword = stringParameters[0];
            object parameters = stringParameters;
            switch (keyword.ToLower())
            {
                case "connect":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Start));
                        t.Start(parameters);
                        break;
                    }
                case "disconnect":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Disconnect));
                        t.Start(parameters);
                        break;
                    }
                case "exit":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Exit));
                        t.Start(parameters);
                        break;
                    }
                case "register":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Register));
                        t.Start(parameters);
                        break;
                    }
                case "insert":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Insert));
                        t.Start(parameters);
                        break;
                    }
                case "select":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Select));
                        t.Start(parameters);
                        break;
                    }
                case "update":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Update));
                        t.Start(parameters);
                        break;
                    }
                case "customencrypted":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.CustomEncrypted));
                        t.Start(parameters);
                        break;
                    }
                case "custom":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Custom));
                        t.Start(parameters);
                        break;
                    }
                case "fetchsync":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.FetchSync));
                        t.Start(parameters);
                        break;
                    }
                case "fetchall":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.FetchAll));
                        t.Start(parameters);
                        break;
                    }
                case "login":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Login));
                        t.Start(parameters);
                        break;
                    }
                case "logout":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Logout));
                        t.Start(parameters);
                        break;
                    }
                case "su":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Sudo));
                        t.Start(parameters);
                        break;
                    }
                case "shutdown":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Shutdown));
                        t.Start(parameters);
                        break;
                    }
                case "reboot":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Reboot));
                        t.Start(parameters);
                        break;
                    }
                case "start":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Start));
                        t.Start(parameters);
                        break;
                    }
                case "serverlog":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.GetServerLog));
                        t.Start(parameters);
                        break;
                    }
                case "clientlog":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.GetClientLog));
                        t.Start(parameters);
                        break;
                    }
                case "listallclients":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.ListAllClients));
                        t.Start(parameters);
                        break;
                    }
                case "error":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Error));
                        t.Start(parameters);
                        break;
                    }
                case "getcookie":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.GetCookie));
                        t.Start(parameters);
                        break;
                    }
                case "kick":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Kick));
                        t.Start(parameters);
                        break;
                    }
                case "activateaccount":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.ActivateAccount));
                        t.Start(parameters);
                        break;
                    }
                case "confirmnewdevice":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.ConfirmNewDevice));
                        t.Start(parameters);
                        break;
                    }
                case "addadmindevice":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.NewAdminDevice));
                        t.Start(parameters);
                        break;
                    }
                case "initadminpwchange":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.InitAdminPasswordChange));
                        t.Start(parameters);
                        break;
                    }
                case "commitadminpwchange":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.CommitAdminPasswordChange));
                        t.Start(parameters);
                        break;
                    }
                case "initpwchange":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.InitPasswordChange));
                        t.Start(parameters);
                        break;
                    }
                case "commitpwchange":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.CommitPasswordChange));
                        t.Start(parameters);
                        break;
                    }
                case "initdelaccount":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.InitDeleteAccount));
                        t.Start(parameters);
                        break;
                    }
                case "commitdelaccount":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.CommitDeleteAccount));
                        t.Start(parameters);
                        break;
                    }
                case "banclient":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.BanClient));
                        t.Start(parameters);
                        break;
                    }
                case "banaccount":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.BanAccount));
                        t.Start(parameters);
                        break;
                    }
                case "listallusers":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.ListAllUsers));
                        t.Start(parameters);
                        break;
                    }
                case "getaccountactivity":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.GetAccountActivity));
                        t.Start(parameters);
                        break;
                    }
                case "changeemailaddress":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.ChangeEmailAddress));
                        t.Start(parameters);
                        break;
                    }
                case "resendcode":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.ResendCode));
                        t.Start(parameters);
                        break;
                    }
                case "changename":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.ChangeName));
                        t.Start(parameters);
                        break;
                    }
                case "enabledebugging":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.EnableDebugging));
                        t.Start(parameters);
                        break;
                    }
                case "disabledebugging":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.DisableDebugging));
                        t.Start(parameters);
                        break;
                    }
                case "checkcredentials":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.CheckCredentials));
                        t.Start(parameters);
                        break;
                    }
                case "delete":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(NetworkAdapter.CommandInterpreter.Delete));
                        t.Start(parameters);
                        break;
                    }
                default:
                    {
                        CustomException.ThrowNew.GenericException("Command not found!");
                        break;
                    }
            }
        }
    }
}
