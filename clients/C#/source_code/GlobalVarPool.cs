
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    /// <summary>
    /// Contains all global variables that are needed for PMDBS.
    /// </summary>
    public struct GlobalVarPool
    {
        // CONFIG FILE
        public static string REMOTE_ADDRESS = null;
        public static int REMOTE_PORT;
        public static bool ADDRESS_IS_DNS;
        public static bool USE_PERSISTENT_RSA_KEYS = true;
        public static string CONFIG_VERSION = null;
        public static string CONFIG_BUILD = null;

        // CLIENT VERSIONING
        public static readonly string CLIENT_VERSION = "0.6-8b.19";
        public static readonly string CLIENT_BUILD = "development";

        // CRYPTO VARIABLES
        public static string XMLKey = string.Empty;
        public static string PrivateKey = string.Empty;
        public static string PublicKey = string.Empty;
        public static string aesKey = string.Empty;
        public static string nonce = string.Empty;
        public static string hmac = string.Empty;
        public static string foreignRsaKey = string.Empty;
        public static string cookie = string.Empty;
        public static string passwordHash = string.Empty;
        public static string localAESkey = string.Empty;
        public static string onlinePassword = string.Empty;

        // GLOBAL CONTROLS
        public static CustomMetroForms.AdvancedProgressSpinner loadingSpinner = null;
        public static CustomMetroForms.AdvancedImageButton settingsSave = null;
        public static CustomMetroForms.AdvancedImageButton settingsAbort = null;
        public static System.Windows.Forms.Panel loadingPanel = null;
        public static System.Windows.Forms.Label loadingLabel = null;
        public static System.Windows.Forms.PictureBox loadingLogo = null;
        public static System.Windows.Forms.TableLayoutPanel settingsPanel = null;
        public static System.Windows.Forms.Panel previousPanel = null;
        public static System.Windows.Forms.Label promptMain = null;
        public static System.Windows.Forms.Label promptEMail = null;
        public static System.Windows.Forms.Label promptAction = null;
        public static System.Windows.Forms.Panel promptPanel = null;
        public static CustomMetroForms.AdvancedImageButton SyncButton = null;

        // SETTINGS
        public static bool wasOnline = false;
        public static string firstUsage = string.Empty;

        // GLOBAL VARIABLES
        public static string name = "User";
        public static string currentUser = string.Empty;
        public static string serverName = string.Empty;
        public static string username = string.Empty;
        public static string email = string.Empty;
        public static string scryptHash = string.Empty;
        public static string promptCommand = string.Empty;
        public static List<string> selectedAccounts = new List<string>();
        /// <summary>
        /// Represents the decrypted database. Columns are D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes, D_icon.
        /// </summary>
        public static System.Data.DataTable UserData = new System.Data.DataTable();
        /// <summary>
        /// Represents the decrypted data with applied filters.
        /// Columns are D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes, D_icon.
        /// </summary>
        public static System.Data.DataTable FilteredUserData = new System.Data.DataTable();
        public static Form1 Form1 = null;
        public static HelperMethods.LoadingType loadingType = HelperMethods.LoadingType.DEFAULT;

        public static int countedPackets = 0;
        public static int expectedPacketCount = 0;

        public static bool connectionLost = false;
        public static bool commandError = false;
        public static bool countSyncPackets = false;
        public static bool connected = false;
        public static bool bootCompleted = false;
        public static bool isRoot = false;
        public static bool isUser = false;
        public static bool debugging = false;
        public static bool threadKilled = false;
        public static bool search = false;
        public static bool databaseIsInUse = false;
        public static bool promptFromBackgroundThread = false;
        public static List<int> ThreadIDs = new List<int>();
        public static Socket clientSocket;

        public static System.Windows.Forms.Label outputLabel = null;
        public static bool outputLabelIsValid = false;
    }
}
