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
        // using AES with:
        // Key hash algorithm: SHA-256
        // Key Size: 256 Bit
        // Block Size: 128 Bit
        // Input Vector (IV): 128 Bit
        // Mode of Operation: Cipher-Block Chaining (CBC)
        public static string AESEncrypt(string plainText, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedPasswordBytes = SHA256Managed.Create().ComputeHash(passwordBytes);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
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
            byte[] iv = new byte[16];
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] hashedPasswordBytes = SHA256Managed.Create().ComputeHash(passwordBytes);
            Array.Copy(cipherBytes, iv, 16);
            byte[] decryptedBytes = null;
            using (AesCng AES = new AesCng())
            {
                AES.IV = iv;
                AES.KeySize = 256;
                AES.Mode = CipherMode.CBC;
                AES.Key = hashedPasswordBytes;
                using (ICryptoTransform decryptor = AES.CreateDecryptor())
                {
                    using (MemoryStream msDecrypted = new MemoryStream())
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypted, decryptor, CryptoStreamMode.Write))
                        {
                            csDecrypt.Write(cipherBytes, 16, cipherBytes.Length - 16);
                            csDecrypt.Close();
                        }
                        decryptedBytes = msDecrypted.ToArray();
                    }
                }
            }
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        public static byte[] GetRandomBytes(int saltLength)
        {
            byte[] randomBytes = new byte[saltLength];
            RNGCryptoServiceProvider.Create().GetBytes(randomBytes);
            return randomBytes;
        }
    }
}
