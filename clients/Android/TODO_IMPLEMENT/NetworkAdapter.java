package pmdbs;

import java.util.List;

public class NetworkAdapter 
{
	public static class MethodProvider
	{
        /// <summary>
        /// Activates the newly created user account on the remote server by providing a 2FA code.
        /// </summary>
        /// <param name="code">The 2FA code to use.</param>
        public static void ActivateAccount(String code)
        {
        	//HelperMethods.InvokeOutputLabel("Activating account ...");
            Network.SendEncrypted("MNGVERusername%eq!" + GlobalVarPool.username + "!;code%eq!PM-" + code + "!;");
        }
        /// <summary>
        /// Requests authorization to log into the remote server.
        /// </summary>
        public static void Authorize()
        {
            String device = HelperMethods.GetOS();
            Network.SendEncrypted("MNGATHcookie%eq!" + GlobalVarPool.cookie + "!;device%eq!" + device + "!;is_mobile%eq!True!;version%eq!" + GlobalVarPool.CLIENT_VERSION + "!;");
        }
        /// <summary>
        /// Changes the user's nickname to a new name.
        /// </summary>
        /// <param name="newName">The new name.</param>
        public static void ChangeName(String newName)
        {
        	//HelperMethods.InvokeOutputLabel("Changing name ...");
            Network.SendEncrypted("MNGCHNnew_name%eq!" + newName + "!;");
        }
        /// <summary>
        /// Requests the remote server to validate this devices cookie.
        /// </summary>
        public static void CheckCookie()
        {
        	//HelperMethods.InvokeOutputLabel("Checking cookie ...");
            Network.SendEncrypted("MNGCCKcookie%eq!" + GlobalVarPool.cookie + "!;");
        }
        /// <summary>
        /// Changes the users password on the remote server by providing a valid 2FA code.
        /// </summary>
        /// <param name="password">The new password (plaintext)</param>
        /// <param name="code">The 2FA code (PM-XXXXXX)</param>
        public static void CommitPasswordChange(String password, String code) throws Exception
        {
        	//HelperMethods.InvokeOutputLabel("Changing password ...");
            String passwordHash = CryptoHelper.SHA256(password);
            String onlinePassword = CryptoHelper.SHA256(passwordHash.substring(0, 32));
            Network.SendEncrypted("MNGCPCpassword%eq!" + onlinePassword + "!;code%eq!PM-" + code + "!;");
        }
        /// <summary>
        /// Creates a connection to the remote server.
        /// </summary>
        public static void Connect()
        {
        	//HelperMethods.InvokeOutputLabel("Connecting ...");
        	PDTPClientThreadHelper clientThreadHelper = PDTPClientThreadHelper.GetInstance();
            Thread t = new Thread(clientThreadHelper);
            t.start();
        }
        /// <summary>
        /// Links a new device to the current user by providing a 2FA code.
        /// </summary>
        /// <param name="code">2FA code</param>
        public static void ConfirmNewDevice(String code)
        {
        	//HelperMethods.InvokeOutputLabel("Adding device ...");
            Network.SendEncrypted("MNGCNDusername%eq!" + GlobalVarPool.username + "!;code%eq!PM-" + code + "!;password%eq!" + GlobalVarPool.onlinePassword + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
        }
        /// <summary>
        /// Invokes a SQL-Delete call on the remote database.
        /// </summary>
        /// <param name="hids">The HIDs to delete.</param>
        public static void Delete(List<String> hids)
        {
            //HelperMethods.InvokeOutputLabel("Deleting data ...");
            String hidsFormatted = "";
            for (int i = 0; i < hids.size(); i++)
            {
                hidsFormatted += hids.get(i) + ";";
            }
            Network.SendEncrypted("REQDEL" + hidsFormatted);
        }
        /// <summary>
        /// Terminates the connection to the remote server.
        /// </summary>
        public static void Disconnect() throws Exception
        {
            //HelperMethods.InvokeOutputLabel("Disconnecting ...");
            Network.Send("FIN");
            PDTPClient client = PDTPClient.GetInstance();
            client.setThreadKilled(true);
            client.getSocket().shutdownInput();
            client.getSocket().shutdownOutput();
            client.getSocket().close();
            client.setErrorCode(0);
        }
        /// <summary>
        /// Checks if nickname and email are up to date.
        /// </summary>
        /*public static void GetAccountDetails()
        {
            //HelperMethods.InvokeOutputLabel("Validating account details ...");
        	DataBaseHelper dbHelper = DataBaseHelper.GetInstance(context);
        	String datatime = dbHelper.execSQL("SELECT U_datetime FROM Tbl_user LIMIT 1;");
            Network.SendEncrypted("MNGGADdatetime%eq!" + datetime + "!;");
        }*/
        /// <summary>
        /// Requests a new device cookie.
        /// </summary>
        public static void GetCookie()
        {
            //HelperMethods.InvokeOutputLabel("Requested cookie.");
            Network.SendEncrypted("MNGCKI");
        }

        /// <summary>
        /// Requests a password change.
        /// </summary>
        public static void InitPasswordChange()
        {
            //HelperMethods.InvokeOutputLabel("Initiated password change.");
            Network.SendEncrypted("MNGIPCmode%eq!PASSWORD_CHANGE!;");
        }

        /// <summary>
        /// Invokes a SQL-Insert call on the remote database.
        /// </summary>
        /// <param name="account">id, host, url, username, password, email, notes, icon, hid, timestamp</param>
        public static void Insert(List<String> account)
        {
            //HelperMethods.InvokeOutputLabel("Inserting data ...");
            String id = account.get(0);
            String host = account.get(1);
            String url = account.get(2);
            String username = account.get(3);
            String password = account.get(4);
            String email = account.get(5);
            String notes = account.get(6);
            String icon = account.get(7);
            String timestamp = account.get(9);
            String query = "local_id%eq!" + id + "!;host%eq!" + host + "!;password%eq!" + password + "!;datetime%eq!" + timestamp + "!;uname%eq!" + username + "!;email%eq!" + email + "!;notes%eq!" + notes + "!;url%eq!" + url + "!;icon%eq!" + icon + "!;";
            Network.SendEncrypted("REQINS" + query);
        }
        /// <summary>
        /// Logs in as a user on the remote server.
        /// </summary>
        public static void Login()
        {
            //HelperMethods.InvokeOutputLabel("Logging in ...");
            Network.SendEncrypted("MNGLGIusername%eq!" + GlobalVarPool.username + "!;password%eq!" + GlobalVarPool.onlinePassword + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
        }
        /// <summary>
        /// Logs out from the currently active user on the remote server.
        /// </summary>
        public static void Logout()
        {
            //HelperMethods.InvokeOutputLabel("Logging out ...");
            Network.SendEncrypted("MNGLGO");
        }
        /// <summary>
        /// Creates a new user on the remote server.
        /// </summary>
        public static void Register()
        {
            //HelperMethods.InvokeOutputLabel("Registering new user ...");
            Network.SendEncrypted("MNGREGusername%eq!" + GlobalVarPool.username + "!;email%eq!" + GlobalVarPool.email + "!;nickname%eq!" + GlobalVarPool.nickname + "!;password%eq!" + GlobalVarPool.onlinePassword + "!;cookie%eq!" + GlobalVarPool.cookie + "!;");
        }
        /// <summary>
        /// Requests the remote server to resend the last 2FA email.
        /// </summary>
        public static void ResendCode()
        {
            //HelperMethods.InvokeOutputLabel("Resending code ...");
            Network.SendEncrypted("MNGRTCusername%eq!" + GlobalVarPool.username + "!;email%eq!" + GlobalVarPool.email + "!;");
        }
        /// <summary>
        /// Invokes a SQL-Select call on the remote database.
        /// </summary>
        /// <param name="hids"></param>
        public static void Select(List<String> hids)
        {
            //HelperMethods.InvokeOutputLabel("Downloading data ...");
            String hidsFormatted = "";
            for (int i = 0; i < hids.size(); i++)
            {
                hidsFormatted += hids.get(i) + ";";
            }
            Network.SendEncrypted("REQSEL" + hidsFormatted);
        }
        /// <summary>
        /// Initializes database syncing with the remote database.
        /// </summary>
        public static void Sync()
        {
            //HelperMethods.InvokeOutputLabel("Synchronizing ...");
            Network.SendEncrypted("REQSYNfetch_mode%eq!FETCH_SYNC!;");
        }

        /// <summary>
        /// Invokes a SQL-Update call on the remote database.
        /// </summary>
        /// <param name="account">id, host, url, username, password, email, notes, icon, hid, timestamp</param>
        public static void Update(List<String> account)
        {
            //HelperMethods.InvokeOutputLabel("Updating data ...");
            String host = account.get(1);
            String url = account.get(2);
            String username = account.get(3);
            String password = account.get(4);
            String email = account.get(5);
            String notes = account.get(6);
            String icon = account.get(7);
            String hid = account.get(8);
            String timestamp = account.get(9);
            String query = "hid%eq!" + hid + "!;datetime%eq!" + timestamp + "!;host%eq!" + host + "!;password%eq!" + password + "!;uname%eq!" + username + "!;email%eq!" + email + "!;notes%eq!" + notes + "!;url%eq!" + url + "!;icon%eq!" + icon + "!;";
            Network.SendEncrypted("REQUPD" + query);
        }
	}
}
