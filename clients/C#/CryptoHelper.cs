using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class CryptoHelper
    {
        public static string AESEncrypt(string plainText, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            // Hash the password with SHA256
            byte[] hashedPasswordBytes = SHA256Managed.Create().ComputeHash(passwordBytes);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 256;

                    AES.Key = hashedPasswordBytes;
                    AES.IV = GetRandomBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cs.Close();
                    }
                    encryptedBytes = new byte[AES.IV.Length + ms.ToArray().Length];
                    AES.IV.CopyTo(encryptedBytes, 0);
                    ms.ToArray().CopyTo(encryptedBytes, AES.IV.Length);
                }
            }
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string AESDecrypt(string cipherText, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            // Hash the password with SHA256
            byte[] hashedPasswordBytes = SHA256Managed.Create().ComputeHash(passwordBytes);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            byte[] decryptedBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 256;

                    AES.Key = hashedPasswordBytes;
                    AES.IV = cipherBytes.Take(AES.BlockSize / 8).ToArray();

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes.Skip(AES.BlockSize / 8).ToArray(), 0, cipherBytes.Skip(AES.BlockSize / 8).ToArray().Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        public static byte[] GetRandomBytes(int saltLength)
        {
            byte[] ba = new byte[saltLength];
            RNGCryptoServiceProvider.Create().GetBytes(ba);
            return ba;
        }
    }
}
