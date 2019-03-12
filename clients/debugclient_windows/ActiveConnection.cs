using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CE;

namespace debugclient
{
    public struct ActiveConnection
    {
        
        //public Boolean isDisconnected = false;
        public static void Start(string ip, int port)
        {
            Boolean isDisconnected = false;
            Boolean isSocketError = false;
            Boolean isTcpFin = false;
            string address = ip + ":" + port;

            //IPHostEntry entry = Dns.GetHostEntry("th3fr3d.ddns.net");
            IPAddress ipAddress = IPAddress.Parse(ip);
            //IPAddress ipAddress = entry.AddressList[0];
            IPEndPoint server = new IPEndPoint(ipAddress, port);
            GlobalVarPool.clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            GlobalVarPool.clientSocket.Connect(server);
            ConsoleExtension.PrintF("Successfully connected to " + ip + ":" + port + "!");
            ConsoleExtension.PrintF("\n");
            ConsoleExtension.PrintF("CLIENT ---> CONNECTED                  <--- " + address);
            GlobalVarPool.attemptConnection = false;
            GlobalVarPool.clientSocket.Send(Encoding.UTF8.GetBytes("\x01UINIXML\x04"));
            ConsoleExtension.PrintF("CLIENT ---> CLIENT HELLO               ---> " + address);
            //LINE 1225 IN DEBUGCLIENT.PY BELOW;
            // AWAIT PACKETS FROM SERVER
            try
            {
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
                    }
                    // ITERATE OVER EVERY SINGE PACKET THAT HAS BEEN RECEIVED
                    for (int i = 0; i < dataPackets.Count; i++)
                    {
                        List<byte> dataPacket = new List<byte>(dataPackets[i]);
                        if (dataPacket[0] != 0x01)                              // <-- SOMETHING'S F***ED UP HERE: PACKETS ARE SPLIT IN RANDOM PIECES SO THIS CHECK FAILS | NOTE: MAY ACTUALLY WORK?
                        {
                            if (!dataPacket.Contains(0x01))
                            {
                                //ConsoleExtension.PrintF("CLIENT <-#- [ERRNO 02] ISOH            -#-> " + address);
                                continue;
                            }
                            else if (dataPacket.Where(currentByte => currentByte.Equals(0x01)).Count() == 1)
                            {
                                dataPacket.RemoveRange(0, dataPacket.IndexOf(0x01));
                            }
                            else
                            {
                                ConsoleExtension.PrintF("CLIENT <-#- [ERRNO 02] ISOH            -#-> " + address);
                                continue;
                            }
                        }
                        dataPacket.RemoveAt(0);
                        string dataString = Encoding.UTF8.GetString(dataPacket.ToArray());
                        char packetSpecifier = dataString[0];
                        // FROM HERE ON USE MAIN PROTOCOL DEFINED HERE: https://github.com/Th3-Fr3d/pmdbs/blob/development/doc/CustomProtocolFinal.pdf
                        switch (packetSpecifier)
                        {
                            case 'U':
                                {
                                    string packetID = dataString.Substring(1, 3);
                                    switch (packetID)
                                    {
                                        case "FIN":
                                            {
                                                ConsoleExtension.PrintF("RECEIVED FIN");
                                                isDisconnected = true;
                                                ConsoleExtension.PrintF("REASON: " + dataString.Substring(4).Split(new string[] { "%eq" }, StringSplitOptions.None)[1].Replace("!", "").Replace(";", ""));
                                                return;
                                            }
                                        case "KEY":
                                            {
                                                ConsoleExtension.PrintF("CLIENT <--- SERVER HELLO               <--- " + address);
                                                string packetSID = dataString.Substring(4, 3);
                                                if (packetSID.Equals("XML"))
                                                {
                                                    GlobalVarPool.foreignRsaKey = dataString.Substring(7).Split('!')[1];
                                                    GlobalVarPool.nonce = CryptoHelper.RandomString();
                                                    string encNonce = CryptoHelper.RSAEncrypt(GlobalVarPool.foreignRsaKey, GlobalVarPool.nonce);
                                                    string message = "CKEkey%eq!" + GlobalVarPool.PublicKey + "!;nonce%eq!" + encNonce + "!;";
                                                    ConsoleExtension.PrintF("CLIENT ---> CLIENT KEY EXCHANGE        ---> " + address);
                                                    GlobalVarPool.clientSocket.Send(Encoding.UTF8.GetBytes("\x01K" + message + "\x04"));
                                                }
                                                else
                                                {
                                                    ConsoleExtension.PrintF("Error: Key Format not supported!");
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
                                        ConsoleExtension.PrintF("CLIENT <--- SYMMETRIC KEY EXCHANGE     <--- " + address);
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
                                            ConsoleExtension.PrintF("Provided nonce: " + providedNonce);
                                            ConsoleExtension.PrintF("Real nonce: " + GlobalVarPool.nonce);
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
                                            ConsoleExtension.PrintF("AES KEY: " + GlobalVarPool.aesKey);
                                            ConsoleExtension.PrintF("NONCE: " + GlobalVarPool.nonce);
                                            ConsoleExtension.PrintF("HMAC KEY: " + GlobalVarPool.hmac);
                                        }
                                        GlobalVarPool.nonce = CryptoHelper.RandomString();
                                        ConsoleExtension.PrintF("CLIENT <--- ACKNOWLEDGE                ---> " + address);
                                        Network.SendEncrypted("KEXACKnonce%eq!" + GlobalVarPool.nonce + "!;");
                                    }
                                    break;
                                }
                            case 'E':
                                {
                                    if (!CryptoHelper.VerifyHMAC(GlobalVarPool.hmac, dataString.Substring(1)))
                                    {
                                        ConsoleExtension.PrintF("SECURITY_EXCEPTION_INVALID_HMAC_CHECKSUM");
                                    }
                                    else if (GlobalVarPool.debugging)
                                    {
                                        ConsoleExtension.PrintF("HMAC OK!");
                                    }
                                    string decryptedData = CryptoHelper.AESDecrypt(dataString.Substring(1, dataString.Length - 45), GlobalVarPool.aesKey);
                                    if (GlobalVarPool.debugging)
                                    {
                                        ConsoleExtension.PrintF("SERVER: " + decryptedData);
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
                                                    ConsoleExtension.PrintF("CLIENT <--- KEY EXCHANGE FINISHED      ---> " + address);
                                                    if (GlobalVarPool.cookie.Equals(string.Empty))
                                                    {
                                                        ConsoleExtension.PrintF("CLIENT ---> GET COOKIE                 ---> " + address);
                                                        Commands.GetCookie(new string[] { "getcookie" });
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
                                                            ConsoleExtension.PrintF("CLIENT <--- NEW COOKIE                 <--- " + address);
                                                            GlobalVarPool.cookie = decryptedData.Substring(6).Split('!')[1];
                                                            File.WriteAllText("cookie.txt",GlobalVarPool.cookie);
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
                                    ConsoleExtension.PrintF("CLIENT <-#- [ERRNO 05] IPS             -#-> " + address);
                                    break;
                                }

                        }
                    }
                }

            }
            catch (SocketException se)
            {
                ConsoleExtension.PrintF(se.Source);
            }
            catch (Exception e)
            {
                ConsoleExtension.PrintF(e.Message);
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}
