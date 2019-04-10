using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    public struct GlobalVarPool
    {
        // CONFIG FILE
        public static string REMOTE_ADDRESS = null;
        public static int REMOTE_PORT;
        public static bool ADDRESS_IS_DNS;
        public static bool USE_PERSISTENT_RSA_KEYS;
        public static string CONFIG_VERSION = null;
        public static string CONFIG_BUILD = null;

        // CLIENT VERSIONING
        public static string CLIENT_VERSION = "0.4-1a.19";
        public static string CLIENT_BUILD = "unstable";

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

        // GLOBAL CONTROLS
        public static AdvancedProgressSpinner loadingSpinner = null;
        public static AdvancedImageButton settingsSave = null;
        public static AdvancedImageButton settingsAbort = null;
        public static System.Windows.Forms.Panel loadingPanel = null;
        public static System.Windows.Forms.Label loadingLabel = null;
        public static System.Windows.Forms.PictureBox loadingLogo = null;
        public static System.Windows.Forms.TableLayoutPanel settingsPanel = null;
        public static System.Windows.Forms.Panel previousPanel = null;
        public static System.Windows.Forms.Label promptMain = null;
        public static System.Windows.Forms.Label promptEMail = null;
        public static System.Windows.Forms.Label promptAction = null;
        public static System.Windows.Forms.Panel promptPanel = null;

        // GLOBAL VARIABLES
        public static string name = "User";
        public static string currentUser = string.Empty;
        public static string serverName = string.Empty;
        public static string username = string.Empty;
        public static string email = string.Empty;
        public static string promptCommand = string.Empty;
        public static string automatedTaskCondition = string.Empty;
        public static List<string> automatedTask = new List<string>();
        public static SearchCondition searchCondition = SearchCondition.Match;
        
        public static bool connected = false;
        public static bool bootCompleted = false;
        public static bool isRoot = false;
        public static bool isUser = false;
        public static bool wasOnline = false;
        public static bool debugging = false;
        public static bool threadKilled = false;
        public static bool search = false;
        public static List<int> ThreadIDs = new List<int>();
        public static Socket clientSocket;

        public static System.Windows.Forms.Label outputLabel = null;
        public static bool outputLabelIsValid = false;
    }
    public enum SearchCondition
    {
        Match = 1,
        Contains = 2,
        In = 3
    }
}
