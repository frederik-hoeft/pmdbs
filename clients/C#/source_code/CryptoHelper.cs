using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CryptSharp;
using CryptSharp.Utility;

namespace pmdbs
{
    /// <summary>
    /// Provides a pool of cryptographic functions to be used throughout pmdbs.
    /// </summary>
    public static class CryptoHelper
    {
        public sealed class CertificateInformation
        {
            public readonly string Issuer = string.Empty;
            public readonly string Subject = string.Empty;
            public readonly string FriendlyName = string.Empty;
            public readonly string PublicKey = string.Empty;
            public readonly string NotValidBefore = string.Empty;
            public readonly string NotValidAfter = string.Empty;
            public readonly string SignatureAlgorithm = string.Empty;
            public readonly bool IsSelfSigned = false;
            public readonly bool IsValid = false;
            public readonly string Checksum = string.Empty;
            public string Status = string.Empty;

            public CertificateInformation(X509Certificate2 cert)
            {
                Issuer = cert.IssuerName.Name;
                Subject = cert.SubjectName.Name;
                FriendlyName = cert.FriendlyName;
                NotValidAfter = cert.NotAfter.ToString();
                NotValidBefore = cert.NotBefore.ToString();
                PublicKey = cert.PublicKey.Key.ToXmlString(false).Replace("RSAKeyValue", "RSAParameters");
                SignatureAlgorithm = cert.SignatureAlgorithm.FriendlyName;
                IsSelfSigned = cert.Issuer == cert.Subject;
                Checksum = Convert.ToBase64String(cert.GetCertHash());
                DateTime currentTime = DateTime.Now;
                int result = DateTime.Compare(cert.NotBefore, currentTime);
                if (result > 0)
                {
                    Status = "Invalid";
                    return;
                }
                result = DateTime.Compare(currentTime, cert.NotAfter);
                if (result > 0)
                {
                    Status = "Expired";
                    return;
                }
                if (IsSelfSigned)
                {
                    Status = "Untrusted";
                    return;
                }
                if (!VerifyCertificate(cert))
                {
                    Status = "Invalid";
                    return;
                }
                Status = "Trusted";
            }
        }
        #region X.509
        public static CertificateInformation CreateCertificateFromString(string pemCertificate)
        {
            return new CertificateInformation(new X509Certificate2(Convert.FromBase64String(pemCertificate.Replace("-----BEGIN CERTIFICATE-----", "").Replace("-----END CERTIFICATE-----", "").Replace("\n", ""))));
        }

        public static bool VerifyCertificate(X509Certificate2 cert)
        {
            return cert.Verify();
        }
        #endregion
        #region AES
        // using AES with:
        // Key hash algorithm: SHA-256
        // Key Size: 256 Bit
        // Block Size: 128 Bit
        // Input Vector (IV): 128 Bit
        // Mode of Operation: Cipher-Block Chaining (CBC)
        /// <summary>
        /// Encrypts a plain text with a password using AES-256 CBC with SHA-256.
        /// </summary>
        /// <param name="plainText">The text to be encrypted.</param>
        /// <param name="password">The password to encrypt the text with.</param>
        /// <returns>The encrypted text.</returns>
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
        /// <summary>
        /// Decrypts a cipher text with a password using AES-256 CBC with SHA-256.
        /// </summary>
        /// <param name="cipherText">The cipher text to be decrypted.</param>
        /// <param name="password">The password to decrypt the cipher text with.</param>
        /// <returns>The decrypted text.</returns>
        public static string AESDecrypt(string cipherText, string password)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }
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
        /// <summary>
        /// Generates cryptographically secure random bytes.
        /// </summary>
        /// <param name="saltLength">The number of bytes to be generated.</param>
        /// <returns>Cryptographically secure random bytes.</returns>
        public static byte[] GetRandomBytes(int saltLength)
        {
            byte[] randomBytes = new byte[saltLength];
            RandomNumberGenerator.Create().GetBytes(randomBytes);
            return randomBytes;
        }
        #endregion
        #region RSA
        /// <summary>
        /// Generates a 4096 bit RSA key pair.
        /// </summary>
        /// <returns>RSA public and private key.</returns>
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
        /// <summary>
        /// Encrypts a string using a provided RSA public key in XML format.
        /// </summary>
        /// <param name="ForeignKeyString">The public key as XML string.</param>
        /// <param name="content">The text to be encrypted.</param>
        /// <returns>The encrypted text as base64 string.</returns>
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
        /// <summary>
        /// Decrypts a cipher string using a provided RSA private key in XML format.
        /// </summary>
        /// <param name="PrivateKeyString">The private key as XML string.</param>
        /// <param name="cipherText">The cipher text to be decrypted.</param>
        /// <returns>The decrypted plain text.</returns>
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
            string plainText = Encoding.UTF8.GetString(bytesPlainTextData);// <-- Encoding.UTF8 WORKS!!!! \(^_^)/
            return plainText;
        }
        #endregion
        #region SCrypt
        /// <summary>
        /// Creates the scrypt hash of a password and salt.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <param name="salt">The salt ta be used for the password hash.</param>
        /// <returns>The hex encoded salted scrypt hash of the password.</returns>
        public static string ScryptHash(string password, string salt)
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
        /// <summary>
        /// Creates the SHA-1 hash of a string.
        /// </summary>
        /// <param name="plaintext">The string to be hashed.</param>
        /// <returns>The hax encoded SHA-1 hash of the plain text.</returns>
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
        /// <summary>
        /// Creates the SHA-256 hash of a password.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>The hex encoded SHA-256 hashed password.</returns>
        public static string SHA256Hash(string password)
        {
            SHA256Cng ShaHashFunction = new SHA256Cng();
            byte[] plainBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = ShaHashFunction.ComputeHash(plainBytes);
            string hashedPassword = string.Empty;
            foreach (byte b in hashedBytes)
            {
                hashedPassword += string.Format("{0:x2}", b);
            }
            return hashedPassword;
        }
        /// <summary>
        /// Creates the SHA-256 hash of a password.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>The base64 encoded SHA-256 hashed password.</returns>
        public static string SHA256HashBase64(string password)
        {
            SHA256Cng ShaHashFunction = new SHA256Cng();
            byte[] plainBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = ShaHashFunction.ComputeHash(plainBytes);
            return Convert.ToBase64String(hashedBytes);
        }
        #endregion
        #region HMAC
        /// <summary>
        /// Calculates the Hashed keyed Message Authentication Code of a message.
        /// </summary>
        /// <param name="hmacKey">The key to be used in the HMAC.</param>
        /// <param name="message">The message to be HMAC'd.</param>
        /// <returns>The </returns>
        public static string CalculateHMAC(string hmacKey, string message)
        {
            return SHA256HashBase64(hmacKey.Substring(32, 32) + SHA256HashBase64(hmacKey.Substring(0, 32) + message));
        }
        /// <summary>
        /// Verifies the Hashed keyed Message Authentication Code of a message.
        /// </summary>
        /// <param name="hmacKey">The key to be used in the validation process.</param>
        /// <param name="fullMessage">The complete message including HMAC.</param>
        /// <returns>true if the HMAC is valid.</returns>
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
        /// <summary>
        /// Generates a cryptographically secure random hex string.
        /// </summary>
        /// <returns>The random hex string.</returns>
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
