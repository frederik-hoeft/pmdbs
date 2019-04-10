using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pmdbs
{
    public struct Commands
    {
        public static void FetchAll(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (parameters.Count() > 1)
            {
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to request all account user-saved data.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Network.SendEncrypted("REQSYNfetch_mode%eq!FETCH_ALL!;");
            }
        }
        public static void FetchSync(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (parameters.Count() > 1)
            {
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to request the headers of all user-saved data to be used for database syncing.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Network.SendEncrypted("REQSYNfetch_mode%eq!FETCH_SYNC!;");
            }
        }
        public static void Authenticate(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
        }
        public static void BanAccount(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isRoot)
            {
                return;
            }
            try
            {
                switch (parameters.Count())
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to prevent a user for X seconds from logging in.\n\n-----ARGUMENTS-----\n-u <username>\n-d <duration in seconds>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            string username = string.Empty;
                            string duration = string.Empty;
                            for (int i = 1; i < parameters.Count(); i++)
                            {
                                switch (parameters[i])
                                {
                                    case "-d":
                                        {
                                            duration = parameters[i + 1];
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
                                            return;
                                        }
                                }
                            }
                            if (new string[] { username, duration }.Contains(string.Empty))
                            {
                                return;
                            }
                            Network.SendEncrypted("MNGBNAusername%eq!" + username + "!;duration%eq!" + duration + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void BanClient(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isRoot)
            {
                return;
            }
            try
            {
                switch (parameters.Count())
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to prevent a client with the indicated IPv4 address for X seconds from connecting to the server.\n\n-----ARGUMENTS-----\n-t <target (IPv4)>\n-d <duration in seconds>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            string target = string.Empty;
                            string duration = string.Empty;
                            for (int i = 1; i < parameters.Count(); i++)
                            {
                                switch (parameters[i])
                                {
                                    case "-d":
                                        {
                                            duration = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-t":
                                        {
                                            target = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    default:
                                        {
                                            return;
                                        }
                                }
                            }
                            if (new string[] { target, duration }.Contains(string.Empty))
                            {
                                return;
                            }
                            Network.SendEncrypted("MNGBANip%eq!" + target + "!;duration%eq!" + duration + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void ChangeEmailAddress(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isUser)
            {
                return;
            }
            try
            {
                switch (parameters.Count())
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to change the email address connected to the account.\n\n-----ARGUMENTS-----\n-u <username>\n-e <new email address>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            string email = string.Empty;
                            string username = string.Empty;
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
                                    case "-e":
                                        {
                                            email = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    default:
                                        {
                                            return;
                                        }
                                }
                            }
                            if (new string[] { email, username }.Contains(string.Empty))
                            {
                                return;
                            }
                            Network.SendEncrypted("MNGCEAnew_email%eq!" + email + "!;username%eq!" + username + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void ChangeName(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (parameters.Count() > 1)
            {
                string name = string.Empty;
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to change the users display name (not the username).\n\nThis command takes no arguments.\n");
                                return;
                            }
                        case "-n":
                            {
                                name = parameters[i + 1];
                                i++;
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                if (name.Equals(string.Empty))
                {
                    return;
                }
                Network.SendEncrypted("MNGCHNnew_name%eq!" + name + "!;");
            }
            else
            {
                return;
            }
        }
        public static void GetClientLog(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isRoot)
            {
                return;
            }
            if (parameters.Count() > 1)
            {
                string username = string.Empty;
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to fetch the server log.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        case "-u":
                            {
                                username = parameters[i + 1];
                                i++;
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                if (username.Equals(string.Empty))
                {
                    return;
                }
                Network.SendEncrypted("MNGLOGmode%eq!CLIENT!;username%eq!" + username + "!;");
            }
            else
            {
                return;
            }
        }
        public static void CommitAdminPasswordChange(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isRoot)
            {
                return;
            }
            try
            {
                switch (parameters.Count())
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to provide a 2FA code as well as the new password for the root account.\n\n-----ARGUMENTS-----\n-p <new root password>\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            string password = string.Empty;
                            string code = string.Empty;
                            for (int i = 1; i < parameters.Count(); i++)
                            {
                                switch (parameters[i])
                                {
                                    case "-c":
                                        {
                                            code = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-p":
                                        {
                                            password = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    default:
                                        {
                                            return;
                                        }
                                }
                            }
                            if (new string[] { password, code }.Contains(string.Empty))
                            {
                                return;
                            }
                            string passwordHash = CryptoHelper.SHA256HashBase64(password);
                            Network.SendEncrypted("MNGAPCpassword%eq!" + passwordHash + "!;code%eq!" + code + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void CommitDeleteAccount(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isUser)
            {
                return;
            }
            if (parameters.Count() > 1)
            {
                string code = string.Empty;
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to provide the 2FA code to delete your account and all data associated with it.\n\n-----ARGUMENTS-----\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                                return;
                            }
                        case "-c":
                            {
                                code = parameters[i + 1];
                                i++;
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                if (code.Equals(string.Empty))
                {
                    return;
                }
                Network.SendEncrypted("MNGCADcode%eq!" + code + "!;");
            }
            else
            {
                return;
            }
        }
        public static void CommitPasswordChange(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            try
            {
                switch (parameters.Count())
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to provide a 2FA code as well as the new password for your account.\n\n-----ARGUMENTS-----\n-p <new password>\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            string password = string.Empty;
                            string code = string.Empty;
                            for (int i = 1; i < parameters.Count(); i++)
                            {
                                switch (parameters[i])
                                {
                                    case "-c":
                                        {
                                            code = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-p":
                                        {
                                            password = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    default:
                                        {
                                            return;
                                        }
                                }
                            }
                            if (new string[] { password, code }.Contains(string.Empty))
                            {
                                return;
                            }
                            GlobalVarPool.passwordHash = CryptoHelper.SHA256Hash(password);
                            GlobalVarPool.localAESkey = CryptoHelper.SHA256Hash(GlobalVarPool.passwordHash.Substring(32, 32));
                            string onlinePassword = GlobalVarPool.passwordHash.Substring(0, 32);
                            Network.SendEncrypted("MNGCPCpassword%eq!" + onlinePassword + "!;code%eq!" + code + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void ConfirmNewDevice(object parameterObject)
        {
            HelperMethods.InvokeOutputLabel("Confirming new device ...");
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.connected)
            {
                return;
            }
            try
            {
                switch (parameters.Count())
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to link a new device to your account.\n\n-----ARGUMENTS-----\n-u <username>\n-p <password>\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            string code = string.Empty;
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
                                    case "-c":
                                        {
                                            code = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    default:
                                        {
                                            return;
                                        }
                                }
                            }
                            if (new string[] { code, username, password }.Contains(string.Empty))
                            {
                                return;
                            }
                            GlobalVarPool.passwordHash = CryptoHelper.SHA256Hash(password);
                            GlobalVarPool.localAESkey = CryptoHelper.SHA256Hash(GlobalVarPool.passwordHash.Substring(32, 32));
                            string onlinePassword = GlobalVarPool.passwordHash.Substring(0, 32);
                            GlobalVarPool.username = username;
                            Network.SendEncrypted("MNGCNDusername%eq!" + username + "!;code%eq!" + code + "!;password%eq!" + onlinePassword + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void Disconnect(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.connected)
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
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to disconnect from the server.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Network.Send("FIN");
                GlobalVarPool.threadKilled = true;
                GlobalVarPool.clientSocket.Disconnect(true);
                GlobalVarPool.clientSocket.Close();
                GlobalVarPool.clientSocket.Dispose();
                GlobalVarPool.connected = false;
            }
        }
        public static void GetCookie(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.connected)
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
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to request a new device cookie. The device cookie is needed to identify your device and if necessary request a 2FA code. This command only needs to be executed once as the cookie is stored on your device.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
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
        public static void Custom(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
        }
        public static void CustomEncrypted(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
        }
        public static void Delete(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
        }
        public static void DisableDebugging(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;

        }
        public static void EnableDebugging(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;

        }
        public static void Error(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.debugging)
            {
                return;
            }
        }
        public static void Exit(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (parameters.Count() > 1)
            {
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to quit the program.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                if (GlobalVarPool.connected)
                {
                    GlobalVarPool.clientSocket.Disconnect(true);
                    GlobalVarPool.clientSocket.Close();
                    GlobalVarPool.clientSocket.Dispose();
                }
                //Environment.Exit(Environment.ExitCode);
            }
        }
        public static void GetAccountActivity(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (parameters.Count() > 1)
            {
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to get recent account activity.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Network.SendEncrypted("MNGGAA");
            }
        }
        /*public static void Help(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (GlobalVarPool.connected)
            {
                ConsoleExtension.PrintF(ConsoleColorExtension.Yellow.ToString() + @"------------------GENERAL INFORMATION-----------------
Command line history with arrow keys and tab completion is supported.
*any command* -h / --help displays the usage of the command

--------------------BASIC COMMANDS--------------------

<activateAccount>
<addAdminDevice>
<checkCredentials> --> WIP
<confirmNewDevice>
<disconnect>
<enableDebugging> --> WIP
<exit>
<help>
<login>
<register>
<resendCode>
<su>

");
            }
            else
            {
                ConsoleExtension.PrintF(ConsoleColorExtension.Gray.ToString() + @"------------------GENERAL INFORMATION-----------------
Command line history with arrow keys and tab completion is supported.
*any command* -h / --help displays the usage of the command

--------------------BASIC COMMANDS--------------------

<connect>
<exit>
<help>

");
            }
            
            if (GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF(ConsoleColorExtension.DarkCyan.ToString() + @"----------------USER SPECIFIC COMMANDS----------------

<changeEmailAddress>
<changeName>
<commitDelAccount>
<commitPwChange>
<delete> --> WIP
<fetchAll>
<fetchSync>
<getAccountActivity>
<initDelAccount>
<initPwChange>
<insert>
<logout>
<select> --> WIP
<update>
");
            }
            else if (GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF(ConsoleColorExtension.Red.ToString() + @"---------------------ROOT COMMANDS--------------------

<banAccount>
<banClient>
<clienLog>
<commitAdminPwChange>
<initAdminPwChange>
<kick>
<listAllClients>
<listAllUsers>
<logout>
<shutdown>
<reboot>
<serverLog>

");
            }
            if (GlobalVarPool.debugging)
            {
                ConsoleExtension.PrintF(ConsoleColorExtension.Magenta.ToString() + @"--------------------DEBUG COMMANDS--------------------

<customEncrypted> --> WIP
<custom> --> WIP
<disableDebugging> --> WIP
<error> --> WIP
<getCookie>
");
            }
        }*/
        public static void InitAdminPasswordChange(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isRoot)
            {
                return;
            }
        }
        public static void InitPasswordChange(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (parameters.Count() > 1)
            {
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to request a password change for the current user.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Network.SendEncrypted("MNGIPCmode%eq!PASSWORD_CHANGE!;");
            }
        }
        public static void InitDeleteAccount(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (parameters.Count() > 1)
            {
                for (int i = 1; i < parameters.Count(); i++)
                {
                    switch (parameters[i])
                    {
                        case "--help":
                        case "-h":
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to initialize the deletion of the current user.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Network.SendEncrypted("MNGIACmode%eq!DELETE_ACCOUNT!;");
            }
        }
        public static void Insert(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            try
            {
                int parameterCount = parameters.Count();
                switch (parameterCount)
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to insert a new row into the user data database.\n\n-----ARGUMENTS-----\n-hs <host>\n-p <password> <HID>-----OPTIONAL-----\n-u <username>\n-url <url>\n-e email\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
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

                            for (int i = 1; i < parameterCount; i++)
                            {
                                switch (parameters[i])
                                {
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
                                            return;
                                        }
                                }
                            }
                            if (new string[] { host, password }.Contains(string.Empty))
                            {
                                return;
                            }
                            string query = "local_id%eq!" + -1 + "!;host%eq!" + CryptoHelper.AESEncrypt(host, GlobalVarPool.localAESkey) + "!;password%eq!" + CryptoHelper.AESEncrypt(password, GlobalVarPool.localAESkey) + "!;datetime%eq!" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() + "!;";
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
                            Network.SendEncrypted("REQINS" + query);
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void Kick(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isRoot)
            {
                return;
            }
            try
            {
                switch (parameters.Count())
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to disconnect a specific target (client or user) from the server.\n\n-----ARGUMENTS-----\n-t <target>\n-m <ip/ipport/username>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            string target = string.Empty;
                            string mode = string.Empty;
                            for (int i = 1; i < parameters.Count(); i++)
                            {
                                switch (parameters[i])
                                {
                                    case "-m":
                                        {
                                            mode = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    case "-t":
                                        {
                                            target = parameters[i + 1];
                                            i++;
                                            break;
                                        }
                                    default:
                                        {
                                            return;
                                        }
                                }
                            }
                            if (new string[] { target, mode }.Contains(string.Empty))
                            {
                                return;
                            }
                            Network.SendEncrypted("MNGKIKtarget%eq!" + target + "!;mode%eq!" + mode + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void ListAllClients(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isRoot)
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
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to list all connected clients.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Network.SendEncrypted("MNGLICmode%eq!ALL_CONNECTED!;");
            }
        }
        public static void ListAllUsers(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isRoot)
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
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to list all users.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Network.SendEncrypted("MNGLICmode%eq!ALL_USERS!;");
            }
        }
        public static void Login(object parameterObject)
        {
            HelperMethods.InvokeOutputLabel("Logging in ...");
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.connected)
            {
                return;
            }
            try
            {
                switch (parameters.Count())
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used for authentication at the server.\n\n-----ARGUMENTS-----\n-u <username>\n-p <password>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
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
                                            return;
                                        }
                                }
                            }
                            if (new string[] { username, password }.Contains(string.Empty))
                            {
                                return;
                            }
                            
                            GlobalVarPool.username = username;
                            Network.SendEncrypted("MNGLGIusername%eq!" + username + "!;password%eq!" + GlobalVarPool.onlinePassword + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void Logout(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isUser && !GlobalVarPool.isRoot)
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
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to logout of an account.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
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
        public static void CheckCookie(object parameterObject, bool isAutomatedQuery)
        {
            string[] parameters = (string[])parameterObject;
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
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to check weather your device cookie is valid.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Network.SendEncrypted("MNGCCKcookie%eq!" + GlobalVarPool.cookie + "!;");
            }
        }
        public static void CheckCredentials(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
        }
        public static void NewAdminDevice(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.connected)
            {
                return;
            }
            try
            {
                int parameterCount = parameters.Count();
                switch (parameterCount)
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to link a new device to the root user.\n\n-----ARGUMENTS-----\n-p <password>\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
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
                                            return;
                                        }
                                }
                            }
                            if (new string[] { code, password }.Contains(string.Empty))
                            {
                                return;
                            }
                            string passwordHash = CryptoHelper.SHA256HashBase64(password);
                            GlobalVarPool.username = "root";
                            Network.SendEncrypted("MNGNADpassword%eq!" + passwordHash + "!;code%eq!" + code + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void Reboot(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isRoot)
            {
                return;
            }
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
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to reboot the server.\nBeware that THIS WILL TEMPORARILY END YOUR ROOT SESSION and you will have to log back in after the reboot has completed. If you want to shut the server down instead use the \"shutdown\" command.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
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
        public static void Register(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.connected || GlobalVarPool.isRoot || GlobalVarPool.isUser)
            {
                return;
            }
            try
            {
                int parameterCount = parameters.Count();
                switch (parameterCount)
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to create a new user.\n\n-----ARGUMENTS-----\n-u <username>\n-p <password>\n-e <email>\n-n <nickname>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
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
                                            return;
                                        }
                                }
                            }
                            if (new string[] { email, username, password }.Contains(string.Empty))
                            {
                                return;
                            }
                            if (nickname.Equals(string.Empty))
                            {
                                nickname = "user";
                            }
                            string passwordHash = CryptoHelper.SHA256Hash(password).Substring(0, 32);
                            GlobalVarPool.username = username;
                            Network.SendEncrypted("MNGREGusername%eq!" + username + "!;email%eq!" + email + "!;nickname%eq!" + nickname + "!;password%eq!" + passwordHash + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void ResendCode(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.connected)
            {
                return;
            }
            try
            {
                int parameterCount = parameters.Count();
                switch (parameterCount)
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to request a new 2FA code.\n\n-----ARGUMENTS-----\n-u <username>\n-e <email>\n-n <nickname>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            string username = string.Empty;
                            string nickname = string.Empty;
                            string email = string.Empty;
                            for (int i = 1; i < parameterCount; i++)
                            {
                                switch (parameters[i])
                                {
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
                                            return;
                                        }
                                }
                            }
                            if (new string[] { email, username, nickname }.Contains(string.Empty))
                            {
                                return;
                            }
                            Network.SendEncrypted("MNGRTCusername%eq!" + username + "!;email%eq!" + email + "!;nickname%eq!" + nickname + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void Select(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
        }
        public static void Shutdown(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isRoot)
            {
                return;
            }
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
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to shut the server down.\nBeware that THIS WILL END YOUR ROOT SESSION and you will have to rely on SSH or other remote applications to restart the server. If you just want to reboot use the \"reboot\" command.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
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
        public static void GetServerLog(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.isRoot)
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
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to fetch the server log.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Network.SendEncrypted("MNGLOGmode%eq!SERVER!;");
            }
        }
        public static void Start(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (GlobalVarPool.connected)
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
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used connect to the server.\n\nThis command takes no arguments.\n");
                                return;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Thread connectionThread = new Thread(new ThreadStart(ActiveConnection.Start))
                {
                    IsBackground = true
                };
                connectionThread.Start();
            }
        }
        public static void Sudo(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.connected)
            {
                return;
            }
            try
            {
                int parameterCount = parameters.Count();
                switch (parameterCount)
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to log in as root user who has elevated privileges and can manage the server.\n\n-----ARGUMENTS-----\n-p <password>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
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
                                            return;
                                        }
                                }
                            }
                            if (password.Equals(string.Empty))
                            {
                                return;
                            }
                            string passwordHash = CryptoHelper.SHA256HashBase64(password);
                            GlobalVarPool.username = "root";
                            Network.SendEncrypted("MNGADMpassword%eq!" + passwordHash + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void Update(object parameterObject)
        {
            string[] parameters = (string[])parameterObject;
            try
            {
                int parameterCount = parameters.Count();
                switch (parameterCount)
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to update a row  with the provided HID in the user database.\n\n-----ARGUMENTS-----\n-hid <HID>-----OPTIONAL-----\n -hs <host>\n-u <username>\n-p <password>\n-url <url>\n-e email\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
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
                                            return;
                                        }
                                }
                            }
                            if (hid.Equals(string.Empty))
                            {
                                return;
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
                            Network.SendEncrypted("REQUPD" + query);
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
        public static void ActivateAccount(object parameterObject)
        {
            HelperMethods.InvokeOutputLabel("Activating account ...");
            string[] parameters = (string[])parameterObject;
            if (!GlobalVarPool.connected)
            {
                return;
            }
            try
            {
                switch (parameters.Count())
                {
                    case 2:
                        {
                            if (new string[] { "-h", "--help" }.Contains(parameters[1].ToLower()))
                            {
                                //ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to verify your email address and activate your account.\n\n-----ARGUMENTS-----\n-u <username>\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                return;
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
                                            return;
                                        }
                                }
                            }
                            if (new string[] { username, code }.Contains(string.Empty))
                            {
                                return;
                            }
                            Network.SendEncrypted("MNGVERusername%eq!" + username + "!;code%eq!" + code + "!;");
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
    }
}
