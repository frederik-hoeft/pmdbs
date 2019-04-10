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
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Start));
                        t.Start(parameters);
                        break;
                    }
                case "disconnect":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Disconnect));
                        t.Start(parameters);
                        break;
                    }
                case "exit":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Exit));
                        t.Start(parameters);
                        break;
                    }
                case "register":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Register));
                        t.Start(parameters);
                        break;
                    }
                case "insert":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Insert));
                        t.Start(parameters);
                        break;
                    }
                case "select":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Select));
                        t.Start(parameters);
                        break;
                    }
                case "update":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Update));
                        t.Start(parameters);
                        break;
                    }
                case "customencrypted":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.CustomEncrypted));
                        t.Start(parameters);
                        break;
                    }
                case "custom":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Custom));
                        t.Start(parameters);
                        break;
                    }
                case "fetchsync":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.FetchSync));
                        t.Start(parameters);
                        break;
                    }
                case "fetchall":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.FetchAll));
                        t.Start(parameters);
                        break;
                    }
                case "login":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Login));
                        t.Start(parameters);
                        break;
                    }
                case "logout":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Logout));
                        t.Start(parameters);
                        break;
                    }
                case "su":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Sudo));
                        t.Start(parameters);
                        break;
                    }
                case "shutdown":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Shutdown));
                        t.Start(parameters);
                        break;
                    }
                case "reboot":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Reboot));
                        t.Start(parameters);
                        break;
                    }
                case "start":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Start));
                        t.Start(parameters);
                        break;
                    }
                case "serverlog":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.GetServerLog));
                        t.Start(parameters);
                        break;
                    }
                case "clientlog":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.GetClientLog));
                        t.Start(parameters);
                        break;
                    }
                case "listallclients":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.ListAllClients));
                        t.Start(parameters);
                        break;
                    }
                case "error":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Error));
                        t.Start(parameters);
                        break;
                    }
                case "getcookie":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.GetCookie));
                        t.Start(parameters);
                        break;
                    }
                case "kick":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Kick));
                        t.Start(parameters);
                        break;
                    }
                case "activateaccount":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.ActivateAccount));
                        t.Start(parameters);
                        break;
                    }
                case "confirmnewdevice":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.ConfirmNewDevice));
                        t.Start(parameters);
                        break;
                    }
                case "addadmindevice":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.NewAdminDevice));
                        t.Start(parameters);
                        break;
                    }
                case "initadminpwchange":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.InitAdminPasswordChange));
                        t.Start(parameters);
                        break;
                    }
                case "commitadminpwchange":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.CommitAdminPasswordChange));
                        t.Start(parameters);
                        break;
                    }
                case "initpwchange":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.InitPasswordChange));
                        t.Start(parameters);
                        break;
                    }
                case "commitpwchange":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.CommitPasswordChange));
                        t.Start(parameters);
                        break;
                    }
                case "initdelaccount":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.InitDeleteAccount));
                        t.Start(parameters);
                        break;
                    }
                case "commitdelaccount":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.CommitDeleteAccount));
                        t.Start(parameters);
                        break;
                    }
                case "banclient":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.BanClient));
                        t.Start(parameters);
                        break;
                    }
                case "banaccount":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.BanAccount));
                        t.Start(parameters);
                        break;
                    }
                case "listallusers":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.ListAllUsers));
                        t.Start(parameters);
                        break;
                    }
                case "getaccountactivity":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.GetAccountActivity));
                        t.Start(parameters);
                        break;
                    }
                case "changeemailaddress":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.ChangeEmailAddress));
                        t.Start(parameters);
                        break;
                    }
                case "resendcode":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.ResendCode));
                        t.Start(parameters);
                        break;
                    }
                case "changename":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.ChangeName));
                        t.Start(parameters);
                        break;
                    }
                case "enabledebugging":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.EnableDebugging));
                        t.Start(parameters);
                        break;
                    }
                case "disabledebugging":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.DisableDebugging));
                        t.Start(parameters);
                        break;
                    }
                case "checkcredentials":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.CheckCredentials));
                        t.Start(parameters);
                        break;
                    }
                case "delete":
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(Commands.Delete));
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
