using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pmdbs
{
    /// <summary>
    /// This class hosts all sorts of extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Checks wether a string is a valid pmdbs username
        /// </summary>
        /// <param name="username">The string to check.</param>
        /// <param name="showErrors">Display error messages.</param>
        /// <param name="setGlobal">Set the corresponding global variable in the GlobalVarPool.</param>
        /// <returns>Returns true if the username is valid.</returns>
        public static bool IsValidUsername(this string username, bool showErrors = true, bool setGlobal = true)
        {
            if (string.IsNullOrEmpty(username))
            {
                if (showErrors)
                {
                    CustomException.ThrowNew.FormatException("Please enter your username.");
                }
                return false;
            }
            if (username.Contains("__"))
            {
                if (showErrors)
                {
                    CustomException.ThrowNew.FormatException("The username may not contain double underscores.");
                }
                return false;
            }
            if (new string[] { " ", "\"", "'" }.Any(username.Contains))
            {
                if (showErrors)
                {
                    CustomException.ThrowNew.FormatException("The username may not contain spaces, single or double quotes.");
                }
                return false;
            }
            if (setGlobal)
            {
                GlobalVarPool.username = username;
            }
            return true;
        }
        /// <summary>
        /// Checks wether a string is a valid piot
        /// </summary>
        /// <param name="port">The string to check.</param>
        /// <param name="showErrors">Display error messages.</param>
        /// <param name="setGlobal">Set the corresponding global variable in the GlobalVarPool.</param>
        /// <returns>Returns true if the port is valid.</returns>
        public static bool IsValidPort(this string port, bool showErrors = true, bool setGlobal = true)
        {
            if (string.IsNullOrEmpty(port))
            {
                if (showErrors)
                {
                    CustomException.ThrowNew.FormatException("Please enter the port of the server you'd like to connect to.");
                }
                return false;
            }
            int portNumber = Convert.ToInt32(port);
            if (portNumber < 1 || portNumber > 65536)
            {
                if (showErrors)
                {
                    CustomException.ThrowNew.FormatException("Please enter a valid port number.");
                }
                return false;
            }
            GlobalVarPool.REMOTE_PORT = portNumber;
            return true;
        }
        /// <summary>
        /// Checks wether a string is a ip address or domain.
        /// </summary>
        /// <param name="ip">The string to check.</param>
        /// <param name="showErrors">Display error messages.</param>
        /// <param name="setGlobal">Set the corresponding global variable in the GlobalVarPool.</param>
        /// <returns>Returns true if the ip is valid.</returns>
        public static async Task<bool> IsValidIp(this string ip, bool showErrors = true, bool setGlobal = true)
        {
            if (string.IsNullOrEmpty(ip))
            {
                if (showErrors)
                {
                    CustomException.ThrowNew.FormatException("Please enter the IPv4 address or domain of the server you'd like to connect to.");
                }
                return false;
            }
            // TODO: MORE DISGUSTING REGEXES. THIS ONE DOESN'T EVEN WORK PROPERLY AS IT ALLOWS STUFF LIKE 1.1.1 AS IPv4 ADDRESSES --> TODO: LINQ <3
            if (Regex.IsMatch(ip, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]).){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$"))
            {
                if (setGlobal)
                {
                    GlobalVarPool.REMOTE_ADDRESS = ip;
                }
                return true;
            }
            try
            {
                Task<IPHostEntry> ipTask = Dns.GetHostEntryAsync(ip);
                IPHostEntry ipAddress = await ipTask;
                string ipv4String = ipAddress.AddressList.First().MapToIPv4().ToString();
                if (setGlobal)
                {
                    GlobalVarPool.REMOTE_ADDRESS = ipv4String;
                }
            }
            catch
            {
                if (showErrors)
                {
                    CustomException.ThrowNew.FormatException("Please enter a valid IPv4 address or domain.");
                }
                return false;
            }
            return true;
        }
        /// <summary>
        /// Checks wether a string is a valid email address
        /// </summary>
        /// <param name="email">The string to check.</param>
        /// <param name="showErrors">Display error messages.</param>
        /// <param name="setGlobal">Set the corresponding global variable in the GlobalVarPool.</param>
        /// <returns>Returns true if the email address is valid.</returns>
        public static bool IsValidEmail(this string email, bool showErrors = true, bool setGlobal = true)
        {
            if (string.IsNullOrEmpty(email))
            {
                if (showErrors)
                {
                    CustomException.ThrowNew.FormatException("Please enter your email address.");
                }
                return false;
            }
            // DAMN REGEX SYNTAX SUCKS ... TODO: REPLACE THIS WITH SOME ACTUALLY READABLE LINQ QUERY
            if (!Regex.IsMatch(email, @"^[^.][0-9a-zA-z\.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-\.]+\.[a-z]+$"))
            {
                if (showErrors)
                {
                    CustomException.ThrowNew.FormatException("Please enter a valid email address.");
                }
                return false;
            }
            if (new string[] { " ", "\"", "'" }.Any(email.Contains))
            {
                if (showErrors)
                {
                    CustomException.ThrowNew.FormatException("The email address may not contain spaces, single or double quotes.");
                }
                return false;
            }
            if (setGlobal)
            {
                GlobalVarPool.email = email;
            }
            return true;
        }
    }
}
