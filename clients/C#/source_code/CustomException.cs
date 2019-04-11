using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroFramework.Forms;
namespace pmdbs
{
    public static class CustomException
    {
        public struct ThrowNew
        {
            public static void NetworkException()
            {
                new ErrorForm("An unknown network error has occurred.", "Network Exception").ShowDialog();
            }
            public static void NetworkException(string message)
            {
                new ErrorForm(message, "Network Exception").ShowDialog();
            }
            public static void NetworkException(string message, string code)
            {
                new ErrorForm(message, "Network Exception").ShowDialog();
            }
            public static void GenericException()
            {
                new ErrorForm("An unknown error has occurred.", "Generic Exception").ShowDialog();
            }
            public static void GenericException(string message)
            {
                new ErrorForm(message, "Generic Exception").ShowDialog();
            }
            public static void CryptographicException()
            {
                new ErrorForm("An unknown cryptographic error has occurred.", "Cryptographic Exception").ShowDialog();
            }
            public static void CryptographicException(string message)
            {
                new ErrorForm(message, "Cryptographic Exception").ShowDialog();
            }
            public static void FormatException()
            {
                new ErrorForm("An unknown format error has occured.", "Format Exception").ShowDialog();
            }
            public static void FormatException(string message)
            {
                new ErrorForm(message, "Format Exception").ShowDialog();
            }
            public static void NotImplementedException()
            {
                new ErrorForm("This feature is not available yet.", "Not Implemented Exception").ShowDialog();
            }
            public static void NotImplementedException(string message)
            {
                new ErrorForm(message, "Not Implemented Exception").ShowDialog();
            }
        }
    }
}
