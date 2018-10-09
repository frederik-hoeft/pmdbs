using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
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
        
        static void Main(string[] args)
        {
            
        } 
    }
}
