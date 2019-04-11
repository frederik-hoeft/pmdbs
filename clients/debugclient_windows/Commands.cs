using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CE;

namespace debugclient
{
    public struct Commands
    {
        public static void FetchAll(string[] parameters)
        {
            if (!GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to request all account user-saved data.\n\nThis command takes no arguments.\n");
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
                ConsoleExtension.PrintF("Requesting all data of user: " + GlobalVarPool.currentUser + "...");
                Network.SendEncrypted("REQSYNfetch_mode%eq!FETCH_ALL!;");
            }
        }
        public static void FetchSync(string[] parameters)
        {
            if (!GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to request the headers of all user-saved data to be used for database syncing.\n\nThis command takes no arguments.\n");
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
                ConsoleExtension.PrintF("Requesting headers owned by user: " + GlobalVarPool.currentUser + "...");
                Network.SendEncrypted("REQSYNfetch_mode%eq!FETCH_SYNC!;");
            }
        }
        public static void Authenticate(string[] parameters)
        {

        }
        public static void BanAccount(string[] parameters)
        {
            if (!GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
                return;
            }
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to prevent a user for X seconds from logging in.\n\n-----ARGUMENTS-----\n-u <username>\n-d <duration in seconds>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
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
                                            MissingParameters(parameters[0]);
                                            return;
                                        }
                                }
                            }
                            if (new string[] { username, duration }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            Network.SendEncrypted("MNGBNAusername%eq!" + username + "!;duration%eq!" + duration + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void BanClient(string[] parameters)
        {
            if (!GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
                return;
            }
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to prevent a client with the indicated IPv4 address for X seconds from connecting to the server.\n\n-----ARGUMENTS-----\n-t <target (IPv4)>\n-d <duration in seconds>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                MissingParameters(parameters[0]);
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
                                            MissingParameters(parameters[0]);
                                            return;
                                        }
                                }
                            }
                            if (new string[] { target, duration }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            Network.SendEncrypted("MNGBANip%eq!" + target + "!;duration%eq!" + duration + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void ChangeEmailAddress(string[] parameters)
        {
            if (!GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
                return;
            }
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to change the email address connected to the account.\n\n-----ARGUMENTS-----\n-u <username>\n-e <new email address>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                MissingParameters(parameters[0]);
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
                                            MissingParameters(parameters[0]);
                                            return;
                                        }
                                }
                            }
                            if (new string[] { email, username }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            Network.SendEncrypted("MNGCEAnew_email%eq!" + email + "!;username%eq!" + username + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void ChangeName(string[] parameters)
        {
            if (!GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
                return;
            }
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to change the users display name (not the username).\n\nThis command takes no arguments.\n");
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
                                MissingParameters(parameters[0]);
                                break;
                            }
                    }
                }
                if (name.Equals(string.Empty))
                {
                    MissingParameters(parameters[0]);
                }
                Network.SendEncrypted("MNGCHNnew_name%eq!" + name + "!;");
            }
            else
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void GetClientLog(string[] parameters)
        {
            if (!GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to fetch the server log.\n\nThis command takes no arguments.\n");
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
                                MissingParameters(parameters[0]);
                                break;
                            }
                    }
                }
                if (username.Equals(string.Empty))
                {
                    MissingParameters(parameters[0]);
                }
                Network.SendEncrypted("MNGLOGmode%eq!CLIENT!;username%eq!" + username + "!;");
            }
            else
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void CommitAdminPasswordChange(string[] parameters)
        {
            if (!GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
                return;
            }
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to provide a 2FA code as well as the new password for the root account.\n\n-----ARGUMENTS-----\n-p <new root password>\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
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
                                            MissingParameters(parameters[0]);
                                            return;
                                        }
                                }
                            }
                            if (new string[] { password, code }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            string passwordHash = CryptoHelper.SHA256HashBase64(password);
                            Network.SendEncrypted("MNGAPCpassword%eq!" + passwordHash + "!;code%eq!" + code + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void CommitDeleteAccount(string[] parameters)
        {
            if (!GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to provide the 2FA code to delete your account and all data associated with it.\n\n-----ARGUMENTS-----\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
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
                                MissingParameters(parameters[0]);
                                break;
                            }
                    }
                }
                if (code.Equals(string.Empty))
                {
                    MissingParameters(parameters[0]);
                }
                Network.SendEncrypted("MNGCADcode%eq!" + code + "!;");
            }
            else
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void CommitPasswordChange(string[] parameters)
        {
            if (!GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: Operation not available.");
                return;
            }
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to provide a 2FA code as well as the new password for your account.\n\n-----ARGUMENTS-----\n-p <new password>\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
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
                                            MissingParameters(parameters[0]);
                                            return;
                                        }
                                }
                            }
                            if (new string[] { password, code }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
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
                MissingParameters(parameters[0]);
            }
        }
        public static void ConfirmNewDevice(string[] parameters)
        {
            if (!GlobalVarPool.connected)
            {
                ConsoleExtension.PrintF("ERROR: Operation not available.");
                return;
            }
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to link a new device to your account.\n\n-----ARGUMENTS-----\n-u <username>\n-p <password>\n-c <code>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                MissingParameters(parameters[0]);
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
                                            MissingParameters(parameters[0]);
                                            return;
                                        }
                                }
                            }
                            if (new string[] { code, username, password }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            GlobalVarPool.passwordHash = CryptoHelper.SHA256Hash(password);
                            GlobalVarPool.localAESkey = CryptoHelper.SHA256Hash(GlobalVarPool.passwordHash.Substring(32, 32));
                            string onlinePassword = GlobalVarPool.passwordHash.Substring(0, 32);
                            GlobalVarPool.username = username;
                            ConsoleExtension.PrintF("Linking new device to user account: " + username);
                            Network.SendEncrypted("MNGCNDusername%eq!" + username + "!;code%eq!" + code + "!;password%eq!" + onlinePassword + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void Disconnect(string[] parameters)
        {
            if (!GlobalVarPool.connected)
            {
                ConsoleExtension.PrintF("ERROR: Operation not available.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to disconnect from the server.\n\nThis command takes no arguments.\n");
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
                ConsoleExtension.PrintF("Disconnecting ...");
                Network.Send("FIN");
                GlobalVarPool.threadKilled = true;
                GlobalVarPool.clientSocket.Disconnect(true);
                GlobalVarPool.clientSocket.Close();
                GlobalVarPool.clientSocket.Dispose();
                GlobalVarPool.connected = false;
            }
        }
        public static void GetCookie(string[] parameters)
        {
            if (!GlobalVarPool.connected)
            {
                ConsoleExtension.PrintF("ERROR: Operation not available.");
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
                if (GlobalVarPool.connected)
                {
                    GlobalVarPool.clientSocket.Disconnect(true);
                    GlobalVarPool.clientSocket.Close();
                    GlobalVarPool.clientSocket.Dispose();
                }
                Environment.Exit(Environment.ExitCode);
            }
        }
        public static void GetAccountActivity(string[] parameters)
        {
            if (!GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to get recent account activity.\n\nThis command takes no arguments.\n");
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
                ConsoleExtension.PrintF("Querying account activity for user: " + GlobalVarPool.username);
                Network.SendEncrypted("MNGGAA");
            }
        }
        public static void Help(string[] parameters)
        {
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
        }
        public static void InitAdminPasswordChange(string[] parameters)
        {
            if (!GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
                return;
            }
        }
        public static void InitPasswordChange(string[] parameters)
        {
            if (!GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to request a password change for the current user.\n\nThis command takes no arguments.\n");
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
                Network.SendEncrypted("MNGIPCmode%eq!PASSWORD_CHANGE!;");
            }
        }
        public static void InitDeleteAccount(string[] parameters)
        {
            if (!GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to initialize the deletion of the current user.\n\nThis command takes no arguments.\n");
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
                Network.SendEncrypted("MNGIACmode%eq!DELETE_ACCOUNT!;");
            }
        }
        public static void Insert(string[] parameters)
        {
            if (!GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
                return;
            }
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to insert a new row into the user data database.\n\n-----ARGUMENTS-----\n-hs <host>\n-p <password> <HID>-----OPTIONAL-----\n-u <username>\n-url <url>\n-e email\n\n-----MISC-----\n--help shows this help\n-h shows this help");
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
                                            MissingParameters(parameters[0]);
                                            return;
                                        }
                                }
                            }
                            if (new string[] { host, password }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
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
                            ConsoleExtension.PrintF("Inserting new account ...");
                            Network.SendEncrypted("REQINS" + query);
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void Kick(string[] parameters)
        {
            if (!GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
                return;
            }
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to disconnect a specific target (client or user) from the server.\n\n-----ARGUMENTS-----\n-t <target>\n-m <ip/ipport/username>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
                            }
                            else
                            {
                                MissingParameters(parameters[0]);
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
                                            MissingParameters(parameters[0]);
                                            return;
                                        }
                                }
                            }
                            if (new string[] { target, mode }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            Network.SendEncrypted("MNGKIKtarget%eq!" + target + "!;mode%eq!" + mode + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void ListAllClients(string[] parameters)
        {

            if (!GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to list all connected clients.\n\nThis command takes no arguments.\n");
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
                Network.SendEncrypted("MNGLICmode%eq!ALL_CONNECTED!;");
            }
        }
        public static void ListAllUsers(string[] parameters)
        {
            if (!GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to list all users.\n\nThis command takes no arguments.\n");
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
                Network.SendEncrypted("MNGLICmode%eq!ALL_USERS!;");
            }
        }
        public static void Login(string[] parameters)
        {
            if (!GlobalVarPool.connected)
            {
                ConsoleExtension.PrintF("ERROR: Operation not available.");
                return;
            }
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
                            ConsoleExtension.PrintF(password);
                            ConsoleExtension.PrintF(CryptoHelper.SHA256Hash(password).Substring(0, 32));
                            ConsoleExtension.PrintF(CryptoHelper.SHA256Hash(CryptoHelper.SHA256Hash(password).Substring(0, 32)));
                            GlobalVarPool.passwordHash = CryptoHelper.SHA256Hash(password);
                            GlobalVarPool.localAESkey = CryptoHelper.SHA256Hash(GlobalVarPool.passwordHash.Substring(32, 32));
                            string onlinePassword = CryptoHelper.SHA256Hash(GlobalVarPool.passwordHash.Substring(0, 32));
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
            if (!GlobalVarPool.isUser && !GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: Operation not available.");
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
            if (!GlobalVarPool.connected)
            {
                ConsoleExtension.PrintF("ERROR: Operation not available.");
                return;
            }
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
            if (!GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
            if (!GlobalVarPool.connected || GlobalVarPool.isRoot || GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
                return;
            }
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
                            ConsoleExtension.PrintF(password);
                            ConsoleExtension.PrintF(CryptoHelper.SHA256Hash(password).Substring(0, 32));
                            ConsoleExtension.PrintF(CryptoHelper.SHA256Hash(CryptoHelper.SHA256Hash(password).Substring(0, 32)));
                            string passwordHash = CryptoHelper.SHA256Hash(CryptoHelper.SHA256Hash(password).Substring(0, 32));
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
            if (!GlobalVarPool.connected)
            {
                ConsoleExtension.PrintF("ERROR: Operation not available.");
                return;
            }
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to request a new 2FA code.\n\n-----ARGUMENTS-----\n-u <username>\n-e <email>\n-n <nickname>\n\n-----MISC-----\n--help shows this help\n-h shows this help");
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
                                            MissingParameters(parameters[0]);
                                            return;
                                        }
                                }
                            }
                            if (new string[] { email, username, nickname }.Contains(string.Empty))
                            {
                                MissingParameters(parameters[0]);
                            }
                            ConsoleExtension.PrintF("Requesting new 2FA code ...");
                            Network.SendEncrypted("MNGRTCusername%eq!" + username + "!;email%eq!" + email + "!;nickname%eq!" + nickname + "!;");
                            break;
                        }
                }
            }
            catch
            {
                MissingParameters(parameters[0]);
            }
        }
        public static void Select(string[] parameters)
        {

        }
        public static void Shutdown(string[] parameters)
        {
            if (!GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
            if (!GlobalVarPool.isRoot)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used to fetch the server log.\n\nThis command takes no arguments.\n");
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
                Network.SendEncrypted("MNGLOGmode%eq!SERVER!;");
            }
        }
        public static void Start(string[] parameters)
        {
            if (GlobalVarPool.connected)
            {
                ConsoleExtension.PrintF("ERROR: Operation not available.");
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
                                ConsoleExtension.PrintF(ConsoleColorExtension.Cyan.ToString() + parameters[0].ToLower() + ConsoleColorExtension.Default.ToString() + " is used connect to the server.\n\nThis command takes no arguments.\n");
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
                Thread connectionThread = new Thread(new ThreadStart(ActiveConnection.Start))
                {
                    IsBackground = true
                };
                connectionThread.Start();
            }
        }
        public static void Sudo(string[] parameters)
        {
            if (!GlobalVarPool.connected)
            {
                ConsoleExtension.PrintF("ERROR: Operation not available.");
                return;
            }
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
            if (!GlobalVarPool.isUser)
            {
                ConsoleExtension.PrintF("ERROR: PERM Operation not permitted.");
                return;
            }
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
            if (!GlobalVarPool.connected)
            {
                ConsoleExtension.PrintF("ERROR: Operation not available.");
                return;
            }
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
