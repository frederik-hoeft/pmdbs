using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace pmdbs
{
    public struct ActiveConnection
    {
        public static void Start()
        {
            if (string.IsNullOrEmpty(GlobalVarPool.PrivateKey) || string.IsNullOrEmpty(GlobalVarPool.PublicKey))
            {
                if (GlobalVarPool.USE_PERSISTENT_RSA_KEYS)
                {
                    DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
                    bool retry = true;
                    // ConsoleExtension.PrintF(HelperMethods.Check() + "Looking for RSA key pair in " + d.FullName + "\\keys ...");
                    if (!Directory.Exists(d.FullName + "\\keys"))
                    {
                        // ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Directory \"keys\" does not exist in " + d.FullName + ".");
                        // ConsoleExtension.PrintF(HelperMethods.Check() + "Creating directory \"keys\" ...");
                        try
                        {
                            Directory.CreateDirectory(d.FullName + "\\keys");
                        }
                        catch (Exception e)
                        {
                            // ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Could not create directory: " + e.ToString());
                            return;
                        }
                        // ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Directory \"keys\" created successfully.");
                    }
                    while (retry)
                    {
                        d = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\keys");
                        FileInfo[] files = d.GetFiles().Where(file => (new[] { "client.privatekey", "client.publickey" }).Contains(file.Name)).ToArray();
                        if (files.Length < 2)
                        {
                            bool selected = false;
                            while (!selected)
                            {
                                // ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "No key file found!");
                                // ConsoleExtension.PrintF(HelperMethods.CheckManual() + "What to do next?");
                                // ConsoleExtension.PrintF(HelperMethods.Check() + "[R] = Retry");
                                // ConsoleExtension.PrintF(HelperMethods.Check() + "[G] = Generate");
                                string choice = "G";
                                if (choice.ToUpper().Equals("G"))
                                {
                                    HelperMethods.InvokeOutputLabel("Generating RSA Keys ...");
                                    // ConsoleExtension.PrintF(HelperMethods.Check() + "Generating 4096 bit RSA key pair...");
                                    string[] RSAKeyPair = CryptoHelper.RSAKeyPairGenerator();
                                    GlobalVarPool.PublicKey = RSAKeyPair[0];
                                    GlobalVarPool.PrivateKey = RSAKeyPair[1];
                                    // ConsoleExtension.PrintF(HelperMethods.Check() + "Exporting RSA private key ...");
                                    File.WriteAllText(d.FullName + "\\client.privatekey", GlobalVarPool.PrivateKey);
                                    // ConsoleExtension.PrintF(HelperMethods.CheckOk() + "RSA private key exported successfully.");
                                    // ConsoleExtension.PrintF(HelperMethods.Check() + "Exporting RSA public key ...");
                                    File.WriteAllText(d.FullName + "\\client.publickey", GlobalVarPool.PublicKey);
                                    // ConsoleExtension.PrintF(HelperMethods.CheckOk() + "RSA public key exported successfully.");
                                    // ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Generated RSA key pair.");
                                    retry = false;
                                    selected = true;
                                }
                                else if (choice.ToUpper().Equals("R"))
                                {
                                    selected = true;
                                }
                                else
                                {
                                    // ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Invalid option! Please try again.");
                                }
                            }
                        }
                        else
                        {
                            HelperMethods.InvokeOutputLabel("Reading RSA keys ...");
                            // ConsoleExtension.PrintF(HelperMethods.Check() + "Reading RSA keys ...");
                            // KINDA LAZY BUT IT WORKS
                            GlobalVarPool.PrivateKey = File.ReadAllText(files.Where(file => file.Name.Equals("client.privatekey")).ToArray()[0].FullName);
                            GlobalVarPool.PublicKey = File.ReadAllText(files.Where(file => file.Name.Equals("client.publickey")).ToArray()[0].FullName);
                            // ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Successfully set up RSA keys.");
                            retry = false;
                        }
                    }
                }
                else
                {
                    HelperMethods.InvokeOutputLabel("Generating RSA keys ...");
                   //  ConsoleExtension.PrintF(HelperMethods.Check() + "Generating 4096 bit RSA key pair...");
                    string[] RSAKeyPair = CryptoHelper.RSAKeyPairGenerator();
                    GlobalVarPool.PublicKey = RSAKeyPair[0];
                    GlobalVarPool.PrivateKey = RSAKeyPair[1];
                    // ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Generated RSA key pair.");
                    // ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Successfully set up RSA keys.");
                }
            }
            if (string.IsNullOrEmpty(GlobalVarPool.cookie))
            {
                // ConsoleExtension.PrintF(HelperMethods.Check() + "Looking for cookie ...");
                DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
                try
                {
                    GlobalVarPool.cookie = File.ReadAllText(d.GetFiles().Where(file => file.Name.Equals("cookie.txt")).First().FullName);
                    // ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Found cookie.");
                }
                catch
                {
                    // ConsoleExtension.PrintF(HelperMethods.CheckWarning() + "Cookie not found.");
                }
            }
            GlobalVarPool.threadKilled = false;
            string ip = GlobalVarPool.REMOTE_ADDRESS;
            int port = GlobalVarPool.REMOTE_PORT;
            HelperMethods.InvokeOutputLabel("Connecting to " + ip + ":" + port + " ...");
            bool isDisconnected = false;
            bool isSocketError = false;
            bool isTcpFin = false;
            IPAddress ipAddress;
            try
            {
                if (GlobalVarPool.ADDRESS_IS_DNS)
                {
                    IPHostEntry entry = Dns.GetHostEntry(ip);
                    ipAddress = entry.AddressList[0];
                }
                else
                {
                    ipAddress = IPAddress.Parse(ip);
                }
            }
            catch (Exception e)
            {
                CustomException.ThrowNew.NetworkException("Unable to resolve + " + ip + " Error message: " + e.ToString());
                return;
            }
            IPEndPoint server = new IPEndPoint(ipAddress, port);
            GlobalVarPool.clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            GlobalVarPool.clientSocket.Connect(server);
            ip = ipAddress.ToString();
            string address = ip + ":" + port;
            HelperMethods.InvokeOutputLabel("Successfully connected to " + ip + ":" + port + "!");
            GlobalVarPool.bootCompleted = true;
            GlobalVarPool.connected = true;
            GlobalVarPool.clientSocket.Send(Encoding.UTF8.GetBytes("\x01UINIXML\x04"));
            HelperMethods.InvokeOutputLabel("Sending Client Hello ...");
            //LINE 1225 IN DEBUGCLIENT.PY BELOW;
            // AWAIT PACKETS FROM SERVER
            try
            {
                GlobalVarPool.ThreadIDs.Add(Thread.CurrentThread.ManagedThreadId);
                // INITIALIZE BUFFER FOR HUGE PACKETS (>32 KB)
                List<byte> buffer = new List<byte>();
                // INITIALIZE 32 KB RECEIVE BUFFER FOR INCOMING DATA
                byte[] data = new byte[32768];
                // RUN UNTIL THREAD IS TERMINATED
                while (true)
                {
                    bool receiving = true;
                    // INITIALIZE LIST TO STORE ALL PACKETS FOUND IN RECEIVE BUFFER
                    List<byte[]> dataPackets = new List<byte[]>();
                    // RECEIVE AND DUMP TO BUFFER UNTIL EOT FLAG (USED TO TERMINATE PACKETS IN CUSTOM PROTOCOL --> HEX VALUE 0x04) IS FOUND
                    while (receiving)
                    {
                        // LOAD DATA TO 32768 BYTE BUFFER
                        int connectionDropped = GlobalVarPool.clientSocket.Receive(data);
                        // CHECK IF RECEIVED 0 BYTE MESSAGE (TCP RST)
                        if (connectionDropped == 0)
                        {
                            // BREAK ENDLESS LOOP AND JUMP TO CATCH
                            GlobalVarPool.threadKilled = true;
                            throw new SocketException { Source = "RST" };
                        }
                        // ----HANDLE CASES OF MORE THAN ONE PACKET IN RECEIVE BUFFER
                        // CHECK IF PACKET CONTAINS EOT FLAG AND IF THE BUFFER FOR BIG PACKETS IS EMPTY
                        if (data.Contains<byte>(0x04) && buffer.Count == 0)
                        {
                            // SPLIT PACKETS ON EOT FLAG (MIGHT BE MORE THAN ONE PACKET)
                            List<byte[]> rawDataPackets = HelperMethods.Separate(data, new byte[] { 0x04 });
                            // GRAB THE LAST PACKET
                            byte[] lastDataPacket = rawDataPackets[rawDataPackets.Count - 1];
                            // MOVE ALL BUT THE LAST PACKET INTO THE 2D PACKET ARRAY LIST
                            List<byte[]> tempRawDataPackets = new List<byte[]>(rawDataPackets);
                            tempRawDataPackets.Remove(tempRawDataPackets.Last());
                            dataPackets = new List<byte[]>(tempRawDataPackets);
                            // IN CASE THE LAST PACKET CONTAINS DATA TOO --> MOVE IT IN BUFFER FOR NEXT "RECEIVING ROUND"
                            if (lastDataPacket.Length != 0 && lastDataPacket.Any(b => b != 0))
                            {
                                buffer.AddRange(new List<byte>(lastDataPacket));
                            }
                            // STOP RECEIVING AND BREAK THE LOOP
                            receiving = false;
                        }
                        // CHECK IF PACKET CONTAINS EOT FLAG AND THE BUFFER IS NOT EMPTY
                        else if (data.Contains<byte>(0x04) && buffer.Count != 0)
                        {
                            // SPLIT PACKETS ON EOT FLAG (MIGHT BE MORE THAN ONE PACKET)
                            List<byte[]> rawDataPackets = HelperMethods.Separate(data, new byte[] { 0x04 });
                            // APPEND CONTENT OF BUFFER TO THE FIRST PACKET
                            List<byte> firstPacket = new List<byte>();
                            firstPacket.AddRange(buffer);
                            firstPacket.AddRange(new List<byte>(rawDataPackets[0]));
                            rawDataPackets[0] = firstPacket.ToArray();
                            // RESET THE BUFFER
                            buffer = new List<byte>();
                            // GRAB THE LAST PACKET
                            byte[] lastDataPacket = rawDataPackets[rawDataPackets.Count - 1];
                            // MOVE ALL BUT THE LAST PACKET INTO THE 2D PACKET ARRAY LIST
                            List<byte[]> tempRawDataPackets = new List<byte[]>(rawDataPackets);
                            tempRawDataPackets.Remove(tempRawDataPackets.Last());
                            dataPackets = new List<byte[]>(tempRawDataPackets);
                            // IN CASE THE LAST PACKET CONTAINS DATA TOO --> MOVE IT IN BUFFER FOR NEXT "RECEIVING ROUND"
                            if (lastDataPacket.Length != 0 && lastDataPacket.Any(b => b != 0))
                            {
                                buffer.AddRange(new List<byte>(lastDataPacket));
                            }
                            // STOP RECEIVING AND BREAK THE LOOP
                            receiving = false;
                        }
                        // THE BUFFER DOES NOT CONTAIN ANY EOT FLAG
                        else
                        {
                            // DAMN THAT'S A HUGE PACKET. APPEND THE WHOLE THING TO THE BUFFER AND REPEAT UNTIL EOT FLAG IS FOUND
                            buffer.AddRange(new List<byte>(data));
                        }
                        // RESET THE DATA BUFFER
                        data = new byte[32768];
                    }
                    // ITERATE OVER EVERY SINGE PACKET THAT HAS BEEN RECEIVED
                    for (int i = 0; i < dataPackets.Count; i++)
                    {
                        // LOAD BYTES INTO BYTE LIST --> EASIER TO HANDLE THAN BYTE ARRAYS
                        List<byte> dataPacket = new List<byte>(dataPackets[i]);
                        // CHECK IF PACKETS HAVE A VALID ENTRYPOINT / START OF HEADING
                        if (dataPacket[0] != 0x01)
                        {
                            // WELL... APPARENTLY THERE'S NO ENTRY POINT --> IGNORE AND CONTINUE WITH NEXT ONE
                            if (!dataPacket.Contains(0x01))
                            {
                                CustomException.ThrowNew.NetworkException("Received invalid packet from server.", "[ERRNO 02] ISOH");
                                continue;
                            }
                            // CHECK AND HOPE THAT THERE'S AT LEAST ONE VALID ENTRY POINT
                            else if (dataPacket.Where(currentByte => currentByte.Equals(0x01)).Count() == 1)
                            {
                                dataPacket.RemoveRange(0, dataPacket.IndexOf(0x01));
                            }
                            // THIS PACKET IS OFICIALLY BROKEN (CONTAINING SEVERAL ENTRY POINTS). HOPE THAT IT WASN'T TOO IMPORTANT AND CONTINUE WITH THE NEXT ONE
                            else
                            {
                                CustomException.ThrowNew.NetworkException("Received invalid packet from server.", "[ERRNO 02] ISOH");
                                continue;
                            }
                        }
                        // REMOVE ENTRY POINT MARKER BYTE (0x01)
                        dataPacket.RemoveAt(0);
                        // CONVERT THAT THING TO UTF-8 STRING
                        string dataString = Encoding.UTF8.GetString(dataPacket.ToArray());
                        // GRAB THE PACKET SPECIFIER --> INDICATES ENCRYPTION STATE
                        char packetSpecifier = dataString[0];
                        // FROM HERE ON USE MAIN PROTOCOL DEFINED HERE: https://github.com/Th3-Fr3d/pmdbs/blob/development/doc/CustomProtocolFinal.pdf
                        // ALL THAT'S HAPPENING FROM HERE ON DOWN IS PACKET PARSING AND MATCHING METHODS TO SAID PACKETS. SHOULD BE SOMEWHERE BETWEEN SELF-EXPLANATORY AND BLACK MAGIC
                        switch (packetSpecifier)
                        {
                            case 'U':
                                {
                                    string packetID = dataString.Substring(1, 3);
                                    switch (packetID)
                                    {
                                        case "FIN":
                                            {
                                                isDisconnected = true;
                                                try
                                                {
                                                    GlobalVarPool.clientSocket.Disconnect(true);
                                                    GlobalVarPool.clientSocket.Close();
                                                    GlobalVarPool.clientSocket.Dispose();
                                                    isTcpFin = true;
                                                }
                                                catch
                                                {

                                                }
                                                if (GlobalVarPool.debugging)
                                                {
                                                    Console.WriteLine("Disconnected.");
                                                    Console.WriteLine("REASON: " + dataString.Substring(4).Split(new string[] { "%eq" }, StringSplitOptions.None)[1].Replace("!", "").Replace(";", ""));
                                                }
                                                
                                                return;
                                            }
                                        case "KEY":
                                            {
                                                HelperMethods.InvokeOutputLabel("Received Server Hello ...");
                                                string packetSID = dataString.Substring(4, 3);
                                                if (packetSID.Equals("XML"))
                                                {
                                                    GlobalVarPool.foreignRsaKey = dataString.Substring(7).Split('!')[1];
                                                    GlobalVarPool.nonce = CryptoHelper.RandomString();
                                                    string encNonce = CryptoHelper.RSAEncrypt(GlobalVarPool.foreignRsaKey, GlobalVarPool.nonce);
                                                    string message = "CKEkey%eq!" + GlobalVarPool.PublicKey + "!;nonce%eq!" + encNonce + "!;";
                                                    HelperMethods.InvokeOutputLabel("Client Key Exchange ...");
                                                    GlobalVarPool.clientSocket.Send(Encoding.UTF8.GetBytes("\x01K" + message + "\x04"));
                                                }
                                                else
                                                {
                                                    CustomException.ThrowNew.CryptographicException("RSA Key Format not supported.");
                                                }
                                                break;
                                            }
                                        default:
                                            {
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 'K':
                                {
                                    string decrypted = CryptoHelper.RSADecrypt(GlobalVarPool.PrivateKey, dataString.Substring(1));
                                    string packetID = decrypted.Substring(0, 3);
                                    if (packetID.Equals("SKE"))
                                    {
                                        HelperMethods.InvokeOutputLabel("Symmetric Keys Exchange ...");
                                        string key = string.Empty;
                                        string providedNonce = string.Empty;
                                        foreach (string returnValue in decrypted.Substring(3).Split(';'))
                                        {
                                            if (returnValue.Contains("key"))
                                            {
                                                key = returnValue.Split('!')[1];
                                            }
                                            else if (returnValue.Contains("nonce"))
                                            {
                                                providedNonce = returnValue.Split('!')[1].Replace("\0", "");
                                            }
                                        }
                                        if (GlobalVarPool.debugging)
                                        {
                                            Console.WriteLine("Provided nonce: " + providedNonce);
                                            Console.WriteLine("Real nonce: " + GlobalVarPool.nonce);
                                        }
                                        if (!providedNonce.Equals(GlobalVarPool.nonce))
                                        {
                                            return;
                                        }
                                        GlobalVarPool.aesKey = key;
                                        string shaIn = GlobalVarPool.aesKey + GlobalVarPool.nonce;
                                        GlobalVarPool.hmac = CryptoHelper.SHA256Hash(GlobalVarPool.aesKey + GlobalVarPool.nonce);
                                        if (GlobalVarPool.debugging)
                                        {
                                            Console.WriteLine("AES KEY: " + GlobalVarPool.aesKey);
                                            Console.WriteLine("NONCE: " + GlobalVarPool.nonce);
                                            Console.WriteLine("HMAC KEY: " + GlobalVarPool.hmac);
                                        }
                                        GlobalVarPool.nonce = CryptoHelper.RandomString();
                                        HelperMethods.InvokeOutputLabel("Acknowledging ...");
                                        Network.SendEncrypted("KEXACKnonce%eq!" + GlobalVarPool.nonce + "!;");
                                    }
                                    break;
                                }
                            case 'E':
                                {
                                    Thread.Sleep(100); // <-- APPARENTLY THERE'S SOME NASTY BUG SOMEWHERE. DONT KNOW DONT CARE. USING "Thread.Sleep(100);" SEEMS TO FIX IT SOMEHOW. JUST MICROSOFT THINGS *sigh*
                                    if (!CryptoHelper.VerifyHMAC(GlobalVarPool.hmac, dataString.Substring(1)))
                                    {
                                        CustomException.ThrowNew.NetworkException("Received an invalid HMAC checksum.", "[ERRNO 31] IMAC");
                                    }
                                    else if (GlobalVarPool.debugging)
                                    {
                                        Console.WriteLine("HMAC OK!");
                                    }
                                    string decryptedData = CryptoHelper.AESDecrypt(dataString.Substring(1, dataString.Length - 45), GlobalVarPool.aesKey);
                                    if (GlobalVarPool.debugging)
                                    {
                                        Console.WriteLine("SERVER: " + decryptedData);
                                    }
                                    if (GlobalVarPool.search)
                                    {
                                        if (GlobalVarPool.searchCondition == SearchCondition.Match)
                                        {
                                            if (decryptedData.Equals(GlobalVarPool.automatedTaskCondition))
                                            {
                                                IOAdapter.Parse(GlobalVarPool.automatedTask);
                                                GlobalVarPool.search = false;
                                            }
                                        }
                                        else if (GlobalVarPool.searchCondition == SearchCondition.In)
                                        {
                                            if (GlobalVarPool.automatedTaskCondition.Split('|').Where(taskCondition => decryptedData.Contains(taskCondition)).Count() != 0)
                                            {
                                                IOAdapter.Parse(GlobalVarPool.automatedTask);
                                                GlobalVarPool.search = false;
                                            }
                                        }
                                        else
                                        {
                                            if (decryptedData.Contains(GlobalVarPool.automatedTaskCondition))
                                            {
                                                IOAdapter.Parse(GlobalVarPool.automatedTask);
                                                GlobalVarPool.search = false;
                                            }
                                        }
                                    }
                                    string packetID = decryptedData.Substring(0, 3);
                                    string packetSID = decryptedData.Substring(3, 3);
                                    switch (packetID)
                                    {
                                        case "KEX":
                                            {
                                                if (!packetSID.Equals("ACK") || !decryptedData.Substring(6).Split('!')[1].Equals(GlobalVarPool.nonce))
                                                {
                                                    return;
                                                }
                                                else
                                                {
                                                    HelperMethods.InvokeOutputLabel("Encrypted connection established!");
                                                    if (GlobalVarPool.cookie.Equals(string.Empty))
                                                    {
                                                        Commands.GetCookie(new string[] { "getcookie" });
                                                    }
                                                    else
                                                    {
                                                        Commands.CheckCookie(new string[] { "checkcookie" }, true);
                                                    }
                                                    break;
                                                }
                                            }
                                        case "DTA":
                                            {
                                                switch (packetSID)
                                                {
                                                    case "CKI":
                                                        {
                                                            GlobalVarPool.cookie = decryptedData.Substring(6).Split('!')[1];
                                                            File.WriteAllText("cookie.txt",GlobalVarPool.cookie);
                                                            break;
                                                        }
                                                    case "LOG":
                                                        {
                                                            try
                                                            {
                                                                string[] parts = decryptedData.Substring(6).Split('§');
                                                                string mode = string.Empty;
                                                                string message = string.Empty;
                                                                foreach (string part in parts)
                                                                {
                                                                    if (part.Contains("mode"))
                                                                    {
                                                                        mode = part.Split('$')[1];
                                                                    }
                                                                    else if (part.Contains("msg"))
                                                                    {
                                                                        message = part.Split('$')[1];
                                                                    }
                                                                    else if (part.Length != 0)
                                                                    {
                                                                        CustomException.ThrowNew.FormatException("Unexpected number of arguments in log dump.");
                                                                    }
                                                                }
                                                                if (new string[] { mode, message }.Contains(string.Empty))
                                                                {
                                                                    CustomException.ThrowNew.FormatException("Too few arguments in log dump.");
                                                                }
                                                                else
                                                                {
                                                                    // TODO: ACTUALLY DISPLAY THESE LOGS
                                                                    if (mode.Equals("USER_REQUEST"))
                                                                    {
                                                                        //ConsoleExtension.PrintF("Your account activity:");
                                                                        //ConsoleExtension.PrintF(message);
                                                                    }
                                                                    else
                                                                    {

                                                                    }
                                                                    //ConsoleExtension.PrintF("Received " + mode + " log.");
                                                                    //ConsoleExtension.PrintF(message);
                                                                }
                                                            }
                                                            catch (IndexOutOfRangeException e)
                                                            {
                                                                CustomException.ThrowNew.FormatException(e.ToString());
                                                            }
                                                            break;
                                                        }
                                                }
                                                break;
                                            }
                                        case "INF":
                                            {
                                                switch (packetSID)
                                                {
                                                    case "ERR":
                                                        {
                                                            string errno = string.Empty;
                                                            string errID = string.Empty;
                                                            string message = string.Empty;
                                                            try
                                                            {
                                                                errno = decryptedData.Split(';').Where(element => element.ToLower().Contains("errno")).ToArray()[0].Split('!')[1];
                                                                errID = decryptedData.Split(';').Where(element => element.ToLower().Contains("code")).ToArray()[0].Split('!')[1];
                                                                message = decryptedData.Split(';').Where(element => element.ToLower().Contains("message")).ToArray()[0].Split('!')[1];
                                                            }
                                                            catch
                                                            {
                                                                errno = "-1";
                                                                errID = "UNKN";
                                                                message = "UNKNOWN ERROR";
                                                            }
                                                            CustomException.ThrowNew.NetworkException(message, "[ERRNO " + errno + "] " + errID);
                                                            break;
                                                        }
                                                        //HANDLING RETURN VALUES BELOW... IT'S 03:30 AM WHAT AM I DOING WITH MY LIFE???
                                                    case "RET":
                                                        {
                                                            switch (decryptedData.Split('!')[1])
                                                            {
                                                                case "LOGIN_SUCCESSFUL":
                                                                    {
                                                                        
                                                                        string previousUser = GlobalVarPool.currentUser;
                                                                        GlobalVarPool.currentUser = "<" + GlobalVarPool.username + "@" + GlobalVarPool.serverName + ">";
                                                                        GlobalVarPool.isUser = true;
                                                                        HelperMethods.InvokeOutputLabel("Successfully logged in as " + GlobalVarPool.username + "!");
                                                                        CustomException.ThrowNew.NotImplementedException("Wow it actually worked \\(^_^)/");
                                                                        break;
                                                                    }
                                                                case "SEND_VERIFICATION_ACTIVATE_ACCOUNT":
                                                                    {
                                                                        GlobalVarPool.promptCommand = "activateaccount -u " + GlobalVarPool.username;
                                                                        break;
                                                                    }
                                                                case "LOGGED_OUT":
                                                                    {
                                                                        
                                                                        string previousUser = GlobalVarPool.currentUser;
                                                                        GlobalVarPool.currentUser = "<" + GlobalVarPool.serverName + ">";
                                                                        if (GlobalVarPool.isRoot)
                                                                        {
                                                                            GlobalVarPool.isRoot = false;
                                                                        }
                                                                        else if (GlobalVarPool.isUser)
                                                                        {
                                                                            GlobalVarPool.isUser = false;
                                                                        }
                                                                        break;
                                                                    }
                                                                case "ALREADY_ADMIN":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "SEND_VERIFICATION_ADMIN_NEW_DEVICE":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "SUCCESSFUL_ADMIN_LOGIN":
                                                                    {
                                                                        string previousUser = GlobalVarPool.currentUser;
                                                                        GlobalVarPool.currentUser = "<root@" + GlobalVarPool.serverName + ">";
                                                                        GlobalVarPool.isRoot = true;
                                                                        break;
                                                                    }
                                                                case "COOKIE_DOES_EXIST":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "COOKIE_DOES_NOT_EXIST":
                                                                    {
                                                                        Commands.GetCookie(new string[] { "getcookie" });
                                                                        break;
                                                                    }
                                                                case "ACCOUNT_VERIFIED":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "SEND_VERIFICATION_NEW_DEVICE":
                                                                    {
                                                                        GlobalVarPool.promptCommand = "activateaccount -u " + GlobalVarPool.username;
                                                                        HelperMethods.Prompt("Confirm new device", "Looks like your trying to login from a new device.");
                                                                        break;
                                                                    }
                                                                case "NOT_LOGGED_IN":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "CODE_RESENT":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "CLIENT_NOT_FOUND":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "CLIENT_KICKED":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "SEND_VERIFICATION_CHANGE_PASSWORD":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "SEND_VERIFICATION_DELETE_ACCOUNT":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "ACCOUNT_DELETED_SUCCESSFULLY":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "PASSWORD_CHANGED":
                                                                    {
                                                                        break;
                                                                    }
                                                                case "":
                                                                    {
                                                                        break;
                                                                    }
                                                                default:
                                                                    {
                                                                        break;
                                                                    }
                                                            }
                                                            break;
                                                        }
                                                    case "MIR":
                                                        {
                                                            break;
                                                        }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            default:
                                {
                                    CustomException.ThrowNew.NetworkException("Received invalid Packet Specifier.", "[ERRNO 05] IPS");
                                    break;
                                }

                        }
                    }
                }

            }
            catch (SocketException se)
            {
                if (!GlobalVarPool.threadKilled)
                {
                    CustomException.ThrowNew.NetworkException(se.ToString(), "Socket Error:");
                }
            }
            catch (Exception e)
            {
                if (!GlobalVarPool.threadKilled)
                {
                    CustomException.ThrowNew.GenericException(e.ToString());
                }
            }
            finally
            {
                if (isSocketError)
                {
                    GlobalVarPool.clientSocket.Close();
                    GlobalVarPool.clientSocket.Dispose();
                }
                else if (isTcpFin)
                {
                    // TODO: DISPLAY NON-POP-UP DISCONNECTED MESSAGE
                    try
                    {
                        GlobalVarPool.clientSocket.Disconnect(true);
                    }
                    catch { }
                    GlobalVarPool.clientSocket.Close();
                    GlobalVarPool.clientSocket.Dispose();
                }
                else if (GlobalVarPool.threadKilled)
                {
                    // TODO: DISPLAY NON-POP-UP DISCONNECTED MESSAGE
                }
                else
                {
                    // TODO: DISPLAY NON-POP-UP DISCONNECTED MESSAGE
                    try
                    {
                        if (!isDisconnected)
                        {
                            Network.Send("FIN");
                        }
                        GlobalVarPool.clientSocket.Disconnect(true);
                        GlobalVarPool.clientSocket.Close();
                        GlobalVarPool.clientSocket.Dispose();
                    }
                    catch
                    {
                        GlobalVarPool.clientSocket.Close();
                        GlobalVarPool.clientSocket.Dispose();
                    }
                }
                string previousUser = GlobalVarPool.currentUser;
                GlobalVarPool.currentUser = "<offline>";
                if (GlobalVarPool.isRoot)
                {
                    GlobalVarPool.isRoot = false;
                }
                else if (GlobalVarPool.isUser)
                {
                    GlobalVarPool.isUser = false;
                }
                GlobalVarPool.connected = false;
                GlobalVarPool.ThreadIDs.Remove(Thread.CurrentThread.ManagedThreadId);
            }
        }
    }
}
