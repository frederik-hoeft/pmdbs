using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptSharp;
using CryptSharp.Utility;

namespace pmdbs
{
    public static class CryptoHelper
    {
        #region AES
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
                using (AesCng AES = new AesCng())
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
        #endregion
        #region RSA
        public static string[] RSAKeyPairGenerator()
        {
            //lets take a new CSP with a new 4096 bit rsa key pair
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(4096);

            //how to get the private key
            var privKey = csp.ExportParameters(true);

            //and the public key ...
            var pubKey = csp.ExportParameters(false);

            //converting the public key into a string representation
            string pubKeyString;
            {
                //we need some buffer
                StringWriter sw = new StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, pubKey);
                //get the string from the stream
                pubKeyString = sw.ToString();
            }
            string privKeyString;
            {
                //we need some buffer
                StringWriter sw = new StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, privKey);
                //get the string from the stream
                privKeyString = sw.ToString();
            }
            string[] keyPair = new string[] { pubKeyString, privKeyString };
            return keyPair;
        }
        public static string RSAEncrypt(string ForeignKeyString, string content)
        {

            //get a stream from the string
            StringReader sr = new StringReader(ForeignKeyString);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            RSAParameters ForeignKey = (RSAParameters)xs.Deserialize(sr);

            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters(ForeignKey);

            //for encryption, always handle bytes...
            byte[] bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(content); // <-- ENCODING???

            //apply pkcs OAEP padding and encrypt our data 
            byte[] bytesCipherText = csp.Encrypt(bytesPlainTextData, true);

            //we might want a string representation of our cypher text... base64 will do
            string cipherText = Convert.ToBase64String(bytesCipherText);
            return cipherText;
        }
        public static string RSADecrypt(string PrivateKeyString, string cipherText)
        {
            byte[] bytesCipherText = Convert.FromBase64String(cipherText);
            //get a stream from the string
            StringReader sr = new StringReader(PrivateKeyString);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            RSAParameters PrivateKey = (RSAParameters)xs.Deserialize(sr);
            //we want to decrypt, therefore we need a csp and load our private key
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters(PrivateKey);

            //decrypt and strip pkcs OAEP padding
            byte[] bytesPlainTextData = csp.Decrypt(bytesCipherText, true);

            //get our original plainText back...
            //string plainText = UTF8Encoding.Unicode.GetString(bytesPlainTextData);// <-- DOES NOT WORK :P
            string plainText = Encoding.UTF8.GetString(bytesPlainTextData);// <-- Encoding.UTF8 WORKS!!!! \(^_^)/
            return plainText;
        }
        #endregion
        #region SCrypt
        public static String SCryptHash(string password, string salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            byte[] hashedPassword = SCrypt.ComputeDerivedKey(passwordBytes, saltBytes, 262144, 8, 1, null, 128);
            StringBuilder hex = new StringBuilder(hashedPassword.Length * 2);
            foreach (byte b in hashedPassword)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        #endregion
        #region SHA1
        public static string SHA1Hash(string plaintext)
        {
            using (SHA1 sha = new SHA1Cng())
            {
                byte[] hashedBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plaintext));

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                string hashedPassword = string.Empty;
                foreach (byte b in hashedBytes)
                {
                    hashedPassword += string.Format("{0:x2}", b);
                }
                return hashedPassword;
            }
        }
        #endregion
        #region SHA256
        public static string SHA256Hash(string plainPassword)
        {
            SHA256Cng ShaHashFunction = new SHA256Cng();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainPassword);
            byte[] hashedBytes = ShaHashFunction.ComputeHash(plainBytes);
            string hashedPassword = string.Empty;
            foreach (byte b in hashedBytes)
            {
                hashedPassword += string.Format("{0:x2}", b);
            }
            return hashedPassword;
        }
        public static string SHA256HashBase64(string text)
        {
            SHA256Cng ShaHashFunction = new SHA256Cng();
            byte[] plainBytes = Encoding.UTF8.GetBytes(text);
            byte[] hashedBytes = ShaHashFunction.ComputeHash(plainBytes);
            return Convert.ToBase64String(hashedBytes);
        }
        #endregion
        #region HMAC
        public static string CalculateHMAC(string hmacKey, string message)
        {
            return SHA256HashBase64(hmacKey.Substring(32, 32) + SHA256HashBase64(hmacKey.Substring(0, 32) + message));
        }

        public static bool VerifyHMAC(string hmacKey, string fullMessage)
        {
            string hmac = fullMessage.Substring(fullMessage.Length - 44, 44);
            string message = fullMessage.Substring(0, fullMessage.Length - 44);
            string actualHmac = CalculateHMAC(hmacKey, message);
            if (hmac.Equals(actualHmac))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region SecureRandomGenerator
        public static string RandomString()
        {
            byte[] randomBytes = new Byte[64];
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            SHA256Cng ShaHashFunction = new SHA256Cng();
            byte[] hashedBytes = ShaHashFunction.ComputeHash(randomBytes);
            string randomString = string.Empty;
            foreach (byte b in hashedBytes)
            {
                randomString += string.Format("{0:x2}", b);
            }
            return randomString;
        }
        #endregion
    }
}
