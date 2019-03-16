using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CE;

namespace debugclient
{
    public struct Network
    {
        public static void Send(string data)
        {
            try
            {
                if (GlobalVarPool.debugging)
                {
                    ConsoleExtension.PrintF("SENDING: U" + data);
                }
                GlobalVarPool.clientSocket.Send(Encoding.UTF8.GetBytes("\x01U" + data + "\x04"));
            }
            catch (Exception e)
            {
                if (!GlobalVarPool.threadKilled)
                {
                    ConsoleExtension.PrintF(e.ToString());
                }
            }
        }

        public static void SendEncrypted(string data)
        {
            try
            {
                string encryptedData = CryptoHelper.AESEncrypt(data, GlobalVarPool.aesKey);
                string hmac = CryptoHelper.CalculateHMAC(GlobalVarPool.hmac, encryptedData);
                if (GlobalVarPool.debugging)
                {
                    ConsoleExtension.PrintF("SENDING: E" + data);
                    ConsoleExtension.PrintF("SENDINGE: E" + encryptedData);
                    ConsoleExtension.PrintF("CALCULATED HMAC: " + hmac);
                }
                GlobalVarPool.clientSocket.Send(Encoding.UTF8.GetBytes("\x01" + "E" + encryptedData + hmac + "\x04"));
            }
            catch (Exception e)
            {
                if (!GlobalVarPool.threadKilled)
                {
                    ConsoleExtension.PrintF(e.ToString());
                }
            }
        }

        public static string GetHost(string ip)
        {
            IPAddress hostIPAddress = IPAddress.Parse(ip);
            IPHostEntry hostInfo = Dns.GetHostEntry(hostIPAddress);
            return hostInfo.HostName;
        }
    }
}
