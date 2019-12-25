using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    /// <summary>
    /// Provides methods for data transmission using the PMDBS Packet Handling System protocol
    /// </summary>
    public static class Network
    {
        /// <summary>
        /// Sends data using the unencrypted PPHS protocol. Used for initial key exchange in PPHSS.
        /// </summary>
        /// <param name="data">The data to be sent.</param>
        public static void Send(string data)
        {
            try
            {
                HelperMethods.Debug("Network:  SENDING: U" + data);
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

        /// <summary>
        /// Sends data using the encrypted PPHSS protocol.
        /// </summary>
        /// <param name="data">The data to be sent.</param>
        public static void SendEncrypted(string data)
        {
            try
            {
                string encryptedData = CryptoHelper.AESEncrypt(data, GlobalVarPool.aesKey);
                string hmac = CryptoHelper.CalculateHMAC(GlobalVarPool.hmac, encryptedData);
                HelperMethods.Debug("Network:  SENDING: E" + data);
                HelperMethods.Debug("Network:  SENDINGE: E" + encryptedData);
                HelperMethods.Debug("Network:  CALCULATED HMAC: " + hmac);
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
    }
}
