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
                ConsoleExtension.PrintF("Requesting new cookie ...");
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
                            GlobalVarPool.passwordHash = CryptoHelper.SHA256Hash(password);
                            GlobalVarPool.localAESkey = CryptoHelper.SHA256Hash(GlobalVarPool.passwordHash.Substring(32, 32));
                            string onlinePassword = GlobalVarPool.passwordHash.Substring(0, 32);
                            GlobalVarPool.username = username;
                            ConsoleExtension.PrintF("Trying to log in as user: " + username);
                            Network.SendEncrypted("MNGLGIusername%eq!" + username + "!;password%eq!" + onlinePassword + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
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
            if (parameters.Count() > 1)
            {
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to logout of an account.\n\nThis command takes no arguments.\n");
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
                Network.SendEncrypted("MNGLGO");
            }
        }
        public static void CheckCookie(string[] parameters, bool isAutomatedQuery)
        {
            if (!GlobalVarPool.debugging && !isAutomatedQuery)
            {
                return;
            }
            if (parameters.Count() > 1)
            {
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to check weather your device cookie is valid.\n\nThis command takes no arguments.\n");
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
                ConsoleExtension.PrintF("Validating cookie...");
                Network.SendEncrypted("MNGCCKcookie%eq!" + GlobalVarPool.cookie + "!;");
            }
        }
        public static void CheckCredentials(string[] parameters)
        {

        }
        public static void NewAdminDevice(string[] parameters)
        {
            try
            {
                int parameterCount = parameters.Count();
                switch (parameterCount)
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to link a new device to the root user.\n\n-----ARGUMENTS-----\n-p <password>\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                MissingParameters(parameters[0]);
                            }
                            break;
                        }
                    default:
                        {
                            string password = string.Empty;
                            string code = string.Empty;
                            for (int i = 1; i < parameterCount; i++)
                            {
                                switch (parameters[i])
                                {
                                    case "-p":
                                        {
                                            password = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-c":
                                        {
                                            code = parameters[i + 1];
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
                            if (new string[] { code, password }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            string passwordHash = CryptoHelper.SHA256HashBase64(password);
                            ConsoleExtension.PrintF(passwordHash);
                            GlobalVarPool.username = "root";
                            ConsoleExtension.PrintF("Trying to link new device to user: root");
                            Network.SendEncrypted("MNGNADpassword%eq!" + passwordHash + "!;code%eq!" + code + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void Reboot(string[] parameters)
        {
            int parameterCount = parameters.Count();
            if (parameterCount > 1)
            {
                for (int i = 1; i < parameterCount; i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to reboot the server.\nBeware that THIS WILL TEMPORARILY END YOUR ROOT SESSION and you will have to log back in after the reboot has completed. If you want to shut the server down instead use the \"shutdown\" command.\n\nThis command takes no arguments.\n");
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
                Network.SendEncrypted("MNGRBTREBOOT");
            }
        }
        public static void Register(string[] parameters)
        {
            try
            {
                int parameterCount = parameters.Count();
                switch (parameterCount)
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to create a new user.\n\n-----ARGUMENTS-----\n-u <username>\n-p <password>\n-e <email>\n-n <nickname>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
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
                            string nickname = string.Empty;
                            string email = string.Empty;
                            for (int i = 1; i < parameterCount; i++)
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
                                    case "-e":
                                        {
                                            email = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-n":
                                        {
                                            nickname = parameters[i + 1];
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
                            if (new string[] { email, username, password }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            if (nickname.Equals(string.Empty))
                            {
                                nickname = "user";
                            }
                            string passwordHash = CryptoHelper.SHA256Hash(password).Substring(0, 32);
                            GlobalVarPool.username = username;
                            ConsoleExtension.PrintF("Trying to register new user: " + username);
                            Network.SendEncrypted("MNGREGusername%eq!" + username + "!;email%eq!" + email + "!;nickname%eq!" + nickname + "!;password%eq!" + passwordHash + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void ResendCode(string[] parameters)
        {

        }
        public static void Select(string[] parameters)
        {

        }
        public static void Shutdown(string[] parameters)
        {
            int parameterCount = parameters.Count();
            if (parameterCount > 1)
            {
                for (int i = 1; i < parameterCount; i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to shut the server down.\nBeware that THIS WILL END YOUR ROOT SESSION and you will have to rely on SSH or other remote applications to restart the server. If you just want to reboot use the \"reboot\" command.\n\nThis command takes no arguments.\n");
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
                Network.SendEncrypted("MNGSHTSHUTDOWN");
            }
        }
        public static void GetServerLog(string[] parameters)
        {

        }
        public static void Start(string[] parameters)
        {

        }
        public static void Sudo(string[] parameters)
        {
            try
            {
                int parameterCount = parameters.Count();
                switch (parameterCount)
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to log in as root user who has elevated privileges and can manage the server.\n\n-----ARGUMENTS-----\n-p <password>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                MissingParameters(parameters[0]);
                            }
                            break;
                        }
                    default:
                        {
                            string password = string.Empty;
                            for (int i = 1; i < parameterCount; i++)
                            {
                                switch (parameters[i])
                                {
                                    case "-p":
                                        {
                                            password = parameters[i + 1];
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
                            if (password.Equals(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            string passwordHash = CryptoHelper.SHA256HashBase64(password);
                            GlobalVarPool.username = "root";
                            ConsoleExtension.PrintF("Trying to log in as user: " + ConsoleColorExtension.Red.ToString() + GlobalVarPool.username);
                            Network.SendEncrypted("MNGADMpassword%eq!" + passwordHash + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void Update(string[] parameters)
        {
            try
            {
                int parameterCount = parameters.Count();
                switch (parameterCount)
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to update a row  with the provided HID in the user database.\n\n-----ARGUMENTS-----\n-hid <HID>-----OPTIONAL-----\n -hs <host>\n-u <username>\n-p <password>\n-url <url>\n-e email\n\n-----MISC-----\n--help shows this help\n-h shows this help");
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
                            string host = string.Empty;
                            string password = string.Empty;
                            string email = string.Empty;
                            //string notes = string.Empty;
                            string url = string.Empty;
                            string hid = string.Empty;

                            for (int i = 1; i < parameterCount; i++)
                            {
                                switch (parameters[i])
                                {
                                    case "-hid":
                                        {
                                            hid = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-hs":
                                        {
                                            host = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-u":
                                        {
                                            username = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-p":
                                        {
                                            password = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-url":
                                        {
                                            url = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    /*case "-n": --> multiple words are not supported as commands are split at [space]
                                        {
                                            notes = parameters[i + 1];
                                            i++;
                                            break;
                                        }*/
                                    case "-e":
                                        {
                                            email = parameters[i + 1];
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
                            if (hid.Equals(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            string query = "hid%eq!" + hid + "!;datetime%eq!" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() + "!;";
                            if (!host.Equals(string.Empty))
                            {
                                query += "host%eq!" + CryptoHelper.AESEncrypt(host, GlobalVarPool.localAESkey) + "!;";
                            }
                            if (!password.Equals(string.Empty))
                            {
                                query += "password%eq!" + CryptoHelper.AESEncrypt(password, GlobalVarPool.localAESkey) + "!;";
                            }
                            if (!username.Equals(string.Empty))
                            {
                                query += "uname%eq!" + CryptoHelper.AESEncrypt(username, GlobalVarPool.localAESkey) + "!;";
                            }
                            if (!email.Equals(string.Empty))
                            {
                                query += "email%eq!" + CryptoHelper.AESEncrypt(email, GlobalVarPool.localAESkey) + "!;";
                            }
                            /*if (!notes.Equals(string.Empty))
                            {
                                query += "notes%eq!" + CryptoHelper.AESEncrypt(notes, GlobalVarPool.localAESkey) + "!;";
                            }*/
                            if (!url.Equals(string.Empty))
                            {
                                query += "url%eq!" + CryptoHelper.AESEncrypt(url, GlobalVarPool.localAESkey) + "!;";
                            }
                            ConsoleExtension.PrintF("Trying to activate account: " + username);
                            Network.SendEncrypted("REQUPD" + query);
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void ActivateAccount(string[] parameters)
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to verify your email address and activate your account.\n\n-----ARGUMENTS-----\n-u <username>\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
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
                            string code = string.Empty;
                            for (int i = 1; i < parameters.Count(); i++)
                            {
                                switch (parameters[i])
                                {
                                    case "-u":
                                        {
                                            username = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-c":
                                        {
                                            code = parameters[i + 1];
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
                            if (new string[] { username, code }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            ConsoleExtension.PrintF("Trying to activate account: " + username);
                            Network.SendEncrypted("MNGVERusername%eq!" + username + "!;code%eq!" + code + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }

        public static void MissingParameters(string command)
        {
            ConsoleExtension.PrintF(ConsoleColorExtension.Red.ToString() + "Error: Syntax " + ConsoleColorExtension.Default.ToString() + "Type " + command.ToLower() + " --help for a summary of options.");
        }
    }
}
