using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    public struct Network
    {
        public static void Send(string data)
        {
            try
            {
                if (GlobalVarPool.debugging)
                {
                    Console.WriteLine("SENDING: U" + data);
                }
                GlobalVarPool.clientSocket.Send(Encoding.UTF8.GetBytes("\x01U" + data + "\x04"));
            }
            catch (Exception e)
            {
                if (!GlobalVarPool.threadKilled)
                {
                    CustomException.ThrowNew.NetworkException(e.ToString());
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
                    Console.WriteLine("SENDING: E" + data);
                    Console.WriteLine("SENDINGE: E" + encryptedData);
                    Console.WriteLine("CALCULATED HMAC: " + hmac);
                }
                GlobalVarPool.clientSocket.Send(Encoding.UTF8.GetBytes("\x01" + "E" + encryptedData + hmac + "\x04"));
            }
            catch (Exception e)
            {
                if (!GlobalVarPool.threadKilled)
                {
                    CustomException.ThrowNew.NetworkException(e.ToString());
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
