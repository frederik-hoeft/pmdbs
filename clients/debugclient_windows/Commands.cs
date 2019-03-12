using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE;

namespace debugclient
{
    public struct Commands
    {
        public static void FetchAll(string[] parameters)
        {

        }
        public static void FetchSync(string[] parameters)
        {

        }
        public static void Authenticate(string[] parameters)
        {

        }
        public static void BanAccount(string[] parameters)
        {

        }
        public static void BanClient(string[] parameters)
        {

        }
        public static void ChangeEmailAddress(string[] parameters)
        {

        }
        public static void ChangeName(string[] parameters)
        {

        }
        public static void GetClientLog(string[] parameters)
        {

        }
        public static void CommitAdminPasswordChange(string[] parameters)
        {

        }
        public static void CommitDeleteAccount(string[] parameters)
        {

        }
        public static void CommitPasswordChange(string[] parameters)
        {

        }
        public static void ConfirmNewDevice(string[] parameters)
        {

        }
        public static void GetCookie(string[] parameters)
        {
            if (parameters.Count() > 1)
            {
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to request a new device cookie. The device cookie is needed to identify your device and if necessary request a 2FA code. This command only needs to be executed once as the cookie is stored on your device.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                MissingParameters(parameters[0]);
                                break;
                            }
                    }
                }
            }
            else
            {
                Network.SendEncrypted("MNGCKI");
            }
        }
        public static void Custom(string[] parameters)
        {

        }
        public static void CustomEncrypted(string[] parameters)
        {

        }
        public static void Delete(string[] parameters)
        {

        }
        public static void DisableDebugging(string[] parameters)
        {

        }
        public static void EnableDebugging(string[] parameters)
        {

        }
        public static void Error(string[] parameters)
        {
            if (!GlobalVarPool.debugging)
            {
                return;
            }
        }
        public static void Exit(string[] parameters)
        {
            if (parameters.Count() > 1)
            {
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to quit the program.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                MissingParameters(parameters[0]);
                                break;
                            }
                    }
                }
            }
            else
            {
                GlobalVarPool.clientSocket.Disconnect(true);
                GlobalVarPool.clientSocket.Close();
                GlobalVarPool.clientSocket.Dispose();
                Environment.Exit(Environment.ExitCode);
            }
        }
        public static void GetAccountActivity(string[] parameters)
        {

        }
        public static void Help(string[] parameters)
        {

        }
        public static void InitAdminPasswordChange(string[] parameters)
        {

        }
        public static void InitPasswordChange(string[] parameters)
        {

        }
        public static void InitDeleteAccount(string[] parameters)
        {

        }
        public static void Insert(string[] parameters)
        {

        }
        public static void Kick(string[] parameters)
        {

        }
        public static void ListAllClients(string[] parameters)
        {

        }
        public static void ListAllUsers(string[] parameters)
        {

        }
        public static void Login(string[] parameters)
        {
            try
            {
                switch (parameters.Count())
                {
                    case 1:
                        {
                            MissingParameters(parameters[0]);
                            break;
                        }
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used for authentication at the server.\n\n-----ARGUMENTS-----\n-u <username>\n-p <password>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                MissingParameters(parameters[0]);
                            }
                            break;
                        }
                    default:
                        {
                            string username = string.Empty;
                            string password = string.Empty;
                            for (int i = 1; i < parameters.Count(); i++)
                            {
                                switch (parameters[i])
                                {
                                    case "-p":
                                        {
                                            password = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-u":
                                        {
                                            username = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    default:
                                        {
                                            MissingParameters(parameters[0]);
                                            return;
                                        }
                                }
                            }
                            if (new string[] { username, password }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            string passwordHash = CryptoHelper.SHA256Hash(password).Substring(0,32);
                            /*GlobalVarPool.username = username;
                            string previousUser = GlobalVarPool.currentUser;
                            GlobalVarPool.currentUser = "<" + username + "@" + GlobalVarPool.serverName + ">";
                            ConsoleExtension.formatPrompt = ConsoleExtension.formatPrompt.Replace(previousUser, GlobalVarPool.currentUser);*/
                            ConsoleExtension.PrintF("Trying to log in as user: " + username);
                            Network.SendEncrypted("MNGLGIusername%eq!" + username + "!;password%eq!" + passwordHash + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
            
        }
        public static void Logout(string[] parameters)
        {

        }
        public static void CheckCredentials(string[] parameters)
        {

        }
        public static void NewAdminDevice(string[] parameters)
        {

        }
        public static void Reboot(string[] parameters)
        {

        }
        public static void Register(string[] parameters)
        {

        }
        public static void ResendCode(string[] parameters)
        {

        }
        public static void Select(string[] parameters)
        {

        }
        public static void Shutdown(string[] parameters)
        {

        }
        public static void GetServerLog(string[] parameters)
        {

        }
        public static void Start(string[] parameters)
        {

        }
        public static void Sudo(string[] parameters)
        {

        }
        public static void Update(string[] parameters)
        {

        }
        public static void ActivateAccount(string[] parameters)
        {

        }

        public static void MissingParameters(string command)
        {
            ConsoleExtension.PrintF(ConsoleColorExtension.Red.ToString() + "Error: Syntax " + ConsoleColorExtension.Default.ToString() + "Type " + command.ToLower() + " --help for a summary of options.");
        }
    }
}
