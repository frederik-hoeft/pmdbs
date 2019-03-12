using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using CE;
using System.Text.RegularExpressions;

namespace debugclient
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string ip = "192.168.178.46";
            int port = 4444;
            GlobalVarPool.ip = ip;
            GlobalVarPool.port = port;
            DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] files = d.GetFiles().Where(file => (new[] { "client.privatekey", "client.publickey" }).Contains(file.Name)).ToArray();
            if (files.Length < 2)
            {
                ConsoleExtension.PrintF("Generating 4096 bit RSA key pair...");
                string[] RSAKeyPair = CryptoHelper.RSAKeyPairGenerator();
                GlobalVarPool.PublicKey = RSAKeyPair[0];
                GlobalVarPool.PrivateKey = RSAKeyPair[1];
                File.WriteAllText(d.FullName + "\\client.privatekey", GlobalVarPool.PrivateKey);
                File.WriteAllText(d.FullName + "\\client.publickey", GlobalVarPool.PublicKey);
            }
            else
            {
                ConsoleExtension.PrintF("Reading RSA keys from files...");
                // KINDA LAZY BUT IT WORKS
                GlobalVarPool.PrivateKey = File.ReadAllText(files.Where(file => file.Name.Equals("client.privatekey")).ToArray()[0].FullName);
                GlobalVarPool.PublicKey = File.ReadAllText(files.Where(file => file.Name.Equals("client.publickey")).ToArray()[0].FullName);
            }
            ConsoleExtension.PrintF("Done!");
            try
            {
                GlobalVarPool.cookie = File.ReadAllText(d.GetFiles().Where(file => file.Name.Equals("cookie.txt")).First().FullName);
            }
            catch
            {
                ConsoleExtension.PrintF("FAILED_TO_READ_COOKIE_FROM_FILE");
            }
            Thread ioThread = new Thread(new ThreadStart(IO.Start))
            {
                IsBackground = true
            };
            ioThread.Start();
            int n = 0;
            GlobalVarPool.attemptConnection = true;
            while (GlobalVarPool.attemptConnection)
            {
                n++;
                ConsoleExtension.PrintF("Trying to connect to " + ip + ":" + port + " ... Attempt #" + n);
                try
                {
                    ActiveConnection.Start(ip, port);
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}
