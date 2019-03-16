using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CE;
using System.Management;

namespace debugclient
{
    class HelperMethods
    {

        public static List<byte[]> Separate(byte[] source, byte[] separator)
        {
            var Parts = new List<byte[]>();
            var Index = 0;
            byte[] Part;
            for (var i = 0; i < source.Length; ++i)
            {
                for (int j = 0; j < separator.Length; j++)
                {
                    if (source[i].Equals(separator[j]))
                    {
                        Part = new byte[i - Index];
                        Array.Copy(source, Index, Part, 0, Part.Length);
                        Parts.Add(Part);
                        Index = i + separator.Length;
                        i += separator.Length - 1;
                    }
                }
                
            }
            Part = new byte[source.Length - Index];
            Array.Copy(source, Index, Part, 0, Part.Length);
            Parts.Add(Part);
            return Parts;
        }

        public static MethodInfo CreateFunction(string config_file)
        {
            string code = @"
        using System;
            
        namespace UserFunctions
        {                
            public class BinaryFunction
            {                
                public static object[] Config()
                {
                    place_holder
                    return new object[]{CONFIG_VERSION, CONFIG_BUILD, REMOTE_ADDRESS, REMOTE_PORT, ADDRESS_IS_DNS, USE_PERSISTENT_RSA_KEYS};
                }
            }
        }
    ";
            string[] finalCode = new string[] { code.Replace("place_holder", config_file) };
            
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters
            {
                // True - memory generation, false - external file generation
                GenerateInMemory = true
            };
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, finalCode);
            Type binaryFunction = results.CompiledAssembly.GetType("UserFunctions.BinaryFunction");
            return binaryFunction.GetMethod("Config");
        }
        public static string CheckOk()
        {
            return "[" + ConsoleColorExtension.Green.ToString() + "  OK  " + ConsoleColorExtension.Default.ToString() + "] ";
        }
        public static string CheckFailed()
        {
            return "[" + ConsoleColorExtension.Red.ToString() + "FAILED" + ConsoleColorExtension.Default.ToString() + "] ";
        }
        public static string CheckWarning()
        {
            return "[" + ConsoleColorExtension.Yellow.ToString() + "WARNING" + ConsoleColorExtension.Default.ToString() + "] ";
        }
        public static string Check()
        {
            return "         ";
        }
        public static string GetOS()
        {
            return Environment.OSVersion.VersionString;
        }
        public static string CheckManual()
        {
            return "[" + ConsoleColorExtension.Yellow.ToString() + "MANUAL" + ConsoleColorExtension.Default.ToString() + "] ";
        }
    }
}
