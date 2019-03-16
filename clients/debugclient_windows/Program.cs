using System.Linq;
using System.Threading;
using System.IO;
using CE;
using System.Reflection;
using System;

namespace debugclient
{
    class Program
    {

        static void Main(string[] args)
        {
            string configFile = string.Empty;
            DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
            ConsoleExtension.PrintF(HelperMethods.Check() + "Searching for config file ...");
            try
            {
                configFile = File.ReadAllText(d.GetFiles().Where(file => (file.Name.Equals("config.cs"))).First().FullName);
            }
            catch
            {
                ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Config file not found ... Aborting.");
                return;
            }
            ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Found config file .");
            ConsoleExtension.PrintF(HelperMethods.Check() + "Compiling config file ...");
            Func<object[]> CompiledConfig;
            try
            {
                MethodInfo Config = HelperMethods.CreateFunction(configFile);
                CompiledConfig = (Func<object[]>)Delegate.CreateDelegate(typeof(Func<object[]>), Config);
            }
            catch (Exception e)
            {
                ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Config compiler error: " + e.ToString());
                return;
            }
            ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Config compiled successfully!");
            ConsoleExtension.PrintF(HelperMethods.Check() + "Reading config ...");
            object[] ConfigVariables = CompiledConfig();
            try
            {
                GlobalVarPool.CONFIG_VERSION = (string)ConfigVariables[0];
                GlobalVarPool.CONFIG_BUILD = (string)ConfigVariables[1];
            }
            catch
            {
                ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Invalid config file.");
                return;
            }
            ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Config seems ok.");
            ConsoleExtension.PrintF(HelperMethods.Check() + "Checking version info ...");
            if (GlobalVarPool.CONFIG_VERSION.Equals(GlobalVarPool.CLIENT_VERSION))
            {
                ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Version info checked. You're running pmdbsUtil v" + GlobalVarPool.CLIENT_VERSION + " x86-64 (" + HelperMethods.GetOS() + ")");
            }
            else
            {
                ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "FATAL: Client is on version " + GlobalVarPool.CLIENT_VERSION + " but config file is for version  " + GlobalVarPool.CONFIG_VERSION + ".");
                return;
            }
            ConsoleExtension.PrintF(HelperMethods.Check() + "Checking build info ...");
            if (GlobalVarPool.CONFIG_BUILD.Equals(GlobalVarPool.CLIENT_BUILD))
            {
                ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Build info checked. Current build: " + GlobalVarPool.CONFIG_BUILD + "-build");
            }
            else
            {
                ConsoleExtension.PrintF(HelperMethods.CheckWarning() + "Client is on " + GlobalVarPool.CLIENT_BUILD + "-build but config file is for " + GlobalVarPool.CONFIG_BUILD + "-build.");
            }
            ConsoleExtension.PrintF(HelperMethods.Check() + "Reading variables from config file ...");
            try
            {
                GlobalVarPool.REMOTE_ADDRESS = (string)ConfigVariables[2];
                GlobalVarPool.REMOTE_PORT = (int)ConfigVariables[3];
                GlobalVarPool.ADDRESS_IS_DNS = (bool)ConfigVariables[4];
                GlobalVarPool.USE_PERSISTENT_RSA_KEYS = (bool)ConfigVariables[5];
            }
            catch (IndexOutOfRangeException e)
            {
                ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Fatal read error: " + e.ToString());
                return;
            }
            catch (Exception e)
            {
                ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Fatal read error: " + e.ToString());
                return;
            }
            ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Successfully extracted variables.");
            ConsoleExtension.PrintF(HelperMethods.Check() + "Config file seems ok ...");
            ConsoleExtension.PrintF(HelperMethods.Check() + "Checking for NULL values ...");
            if (new object[] { GlobalVarPool.REMOTE_ADDRESS, GlobalVarPool.REMOTE_PORT, GlobalVarPool.ADDRESS_IS_DNS, GlobalVarPool.USE_PERSISTENT_RSA_KEYS }.Contains(null))
            {
                ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Found uninitialized variables.");
                return;
            }
            ConsoleExtension.PrintF(HelperMethods.CheckOk() + "All variables initialized!");
            ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Config checks complete!");
            if (GlobalVarPool.USE_PERSISTENT_RSA_KEYS)
            {
                bool retry = true;
                ConsoleExtension.PrintF(HelperMethods.Check() + "Looking for RSA key pair in " + d.FullName + "\\keys ...");
                if (Directory.Exists(d.FullName + "\\keys"))
                {
                    ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Directory \"keys\" exists.");
                }
                else
                {
                    ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Directory \"keys\" does not exist in " + d.FullName + ".");
                    ConsoleExtension.PrintF(HelperMethods.Check() + "Creating directory \"keys\" ...");
                    try
                    {
                        Directory.CreateDirectory(d.FullName + "\\keys");
                    }
                    catch (Exception e)
                    {
                        ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Could not create directory: " + e.ToString());
                        return;
                    }
                    ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Directory \"keys\" created successfully.");
                }
                while (retry)
                {
                    d = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\keys");
                    FileInfo[] files = d.GetFiles().Where(file => (new[] { "client.privatekey", "client.publickey" }).Contains(file.Name)).ToArray();
                    if (files.Length < 2)
                    {
                        bool selected = false;
                        while (!selected)
                        {
                            ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "No key file found!");
                            ConsoleExtension.PrintF(HelperMethods.CheckManual() + "What to do next?");
                            ConsoleExtension.PrintF(HelperMethods.Check() + "[R] = Retry");
                            ConsoleExtension.PrintF(HelperMethods.Check() + "[G] = Generate");
                            string choice = ConsoleExtension.Input(" > ");
                            if (choice.ToUpper().Equals("G"))
                            {
                                ConsoleExtension.PrintF(HelperMethods.Check() + "Generating 4096 bit RSA key pair...");
                                string[] RSAKeyPair = CryptoHelper.RSAKeyPairGenerator();
                                GlobalVarPool.PublicKey = RSAKeyPair[0];
                                GlobalVarPool.PrivateKey = RSAKeyPair[1];
                                ConsoleExtension.PrintF(HelperMethods.Check() + "Exporting RSA private key ...");
                                File.WriteAllText(d.FullName + "\\client.privatekey", GlobalVarPool.PrivateKey);
                                ConsoleExtension.PrintF(HelperMethods.CheckOk() + "RSA private key exported successfully.");
                                ConsoleExtension.PrintF(HelperMethods.Check() + "Exporting RSA public key ...");
                                File.WriteAllText(d.FullName + "\\client.publickey", GlobalVarPool.PublicKey);
                                ConsoleExtension.PrintF(HelperMethods.CheckOk() + "RSA public key exported successfully.");
                                ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Generated RSA key pair.");
                                retry = false;
                                selected = true;
                            }
                            else if (choice.ToUpper().Equals("R"))
                            {
                                selected = true;
                            }
                            else
                            {
                                ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Invalid option! Please try again.");
                            }
                        }
                    }
                    else
                    {
                        ConsoleExtension.PrintF(HelperMethods.Check() + "Reading RSA keys ...");
                        // KINDA LAZY BUT IT WORKS
                        GlobalVarPool.PrivateKey = File.ReadAllText(files.Where(file => file.Name.Equals("client.privatekey")).ToArray()[0].FullName);
                        GlobalVarPool.PublicKey = File.ReadAllText(files.Where(file => file.Name.Equals("client.publickey")).ToArray()[0].FullName);
                        ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Successfully set up RSA keys.");
                        retry = false;
                    }
                }
            }
            else
            {
                ConsoleExtension.PrintF(HelperMethods.Check() + "Generating 4096 bit RSA key pair...");
                string[] RSAKeyPair = CryptoHelper.RSAKeyPairGenerator();
                GlobalVarPool.PublicKey = RSAKeyPair[0];
                GlobalVarPool.PrivateKey = RSAKeyPair[1];
                ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Generated RSA key pair.");
                ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Successfully set up RSA keys.");
            }
            ConsoleExtension.PrintF(HelperMethods.Check() + "Looking for cookie ...");
            d = new DirectoryInfo(Directory.GetCurrentDirectory());
            try
            {
                GlobalVarPool.cookie = File.ReadAllText(d.GetFiles().Where(file => file.Name.Equals("cookie.txt")).First().FullName);
                ConsoleExtension.PrintF(HelperMethods.CheckOk() + "Found cookie.");
            }
            catch
            {
                ConsoleExtension.PrintF(HelperMethods.CheckWarning() + "Cookie not found.");
            }
            ConsoleExtension.PrintF(HelperMethods.Check() + "Trying to connect to " + GlobalVarPool.REMOTE_ADDRESS + ":" + GlobalVarPool.REMOTE_PORT + " ... ");
            try
            {
                Thread connectionThread = new Thread(new ThreadStart(ActiveConnection.Start))
                {
                    IsBackground = true
                };
                connectionThread.Start();

            }
            catch
            {
                ConsoleExtension.PrintF(HelperMethods.CheckFailed() + "Failed to connect to " + GlobalVarPool.REMOTE_ADDRESS + ":" + GlobalVarPool.REMOTE_PORT + " ... ");
            }
            ConsoleExtension.PrintF(HelperMethods.Check() + "Starting IO Thread ...");
            IO.Start();
        }
    }
}
