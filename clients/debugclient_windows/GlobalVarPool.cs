using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace debugclient
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
        public static string CLIENT_VERSION = "0.3-3a.19";
        public static string CLIENT_BUILD = "unstable";

        // GLOBAL VARIABLES
        public static string XMLKey = string.Empty;
        public static string PrivateKey = string.Empty;
        public static string PublicKey = string.Empty;
        public static string aesKey = string.Empty;
        public static string nonce = string.Empty;
        public static string hmac = string.Empty;
        public static string foreignRsaKey = string.Empty;
        public static string cookie = string.Empty;
        public static string currentUser = string.Empty;
        public static string passwordHash = string.Empty;
        public static string serverName = string.Empty;
        public static string username = string.Empty;
        public static string localAESkey = string.Empty;
        public static bool connected = false;
        public static bool bootCompleted = false;
        public static bool isRoot = false;
        public static bool isUser = false;
        public static bool debugging = false;
        public static bool threadKilled = false;
        public static List<int> ThreadIDs = new List<int>();
        public static Socket clientSocket;
    }
}
