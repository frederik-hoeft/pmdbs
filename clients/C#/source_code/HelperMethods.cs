using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq.Expressions;
using System.Threading;

namespace pmdbs
{
    public struct HelperMethods
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

        public static MethodInfo CompileFunction(string config_file)
        {
            string code = @"
        using System;
            
        namespace pmdbs
        {                
            public class BinaryFunction
            {                
                public static bool Condition()
                {
                    return place_holder;
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
            parameters.ReferencedAssemblies.Add(Assembly.GetEntryAssembly().Location);
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, finalCode);
            Type binaryFunction = results.CompiledAssembly.GetType("pmdbs.BinaryFunction");
            return binaryFunction.GetMethod("Condition");
        }

        public static string GetOS()
        {
            return Environment.OSVersion.VersionString;
        }

        public static void Prompt(string promptMain, string promptAction)
        {
            GlobalVarPool.promptAction.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.promptAction.Text = promptAction;
            });
            GlobalVarPool.promptEMail.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.promptEMail.Text = "An email containing a verification code has been sent to " + GlobalVarPool.email + ".";
            });
            GlobalVarPool.promptMain.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.promptMain.Text = promptMain;
            });
            GlobalVarPool.promptPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.promptPanel.BringToFront();
            });
        }

        public static void InvokeOutputLabel(string text)
        {
            if (GlobalVarPool.outputLabelIsValid)
            {
                GlobalVarPool.outputLabel.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    GlobalVarPool.outputLabel.Text = text;
                });
            }
        }

        public static void LoadingHelper(object parameters)
        {
            GlobalVarPool.loadingSpinner.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingSpinner.Visible = true;
            });
            GlobalVarPool.loadingLogo.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingLogo.Visible = true;
            });
            GlobalVarPool.loadingLabel.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingLabel.Visible = true;
            });
            GlobalVarPool.settingsAbort.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.settingsAbort.Visible = false;
            });
            GlobalVarPool.settingsSave.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.settingsSave.Visible = false;
            });
            GlobalVarPool.loadingPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingPanel.BringToFront();
            });
            GlobalVarPool.settingsPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.settingsPanel.BringToFront();
            });

            // PARSE PARAMETER OBJECT TO LIST
            List<object> paramsList = (List<object>)parameters;
            // GET PARAMETERS AND PARSE THEM TO CORRESPONDING DATA TYPES
            System.Windows.Forms.Panel finalPanel = (System.Windows.Forms.Panel)paramsList[0];
            System.Windows.Forms.Label output = (System.Windows.Forms.Label)paramsList[1];
            bool showBackendOutput = (bool)paramsList[2];
            string finishCondition = (string)paramsList[3];

            // SET GLOBAL VARIABLES
            GlobalVarPool.outputLabelIsValid = showBackendOutput;
            GlobalVarPool.outputLabel = output;

            // COMPILE CONDITION AT RUNTIME BECAUSE IT'S IMPOSSIBLE TO PASS A LAMBDA EXPRESSION AS PARAMETER WHEN USING ParameterizedThreadStart()
            // STUPID, OVERCOMPLICATED, OFFICIALLY CONSIDERED "UGLY" BUT IT WORKS ... SOMEHOW *sigh*
            MethodInfo condition = CompileFunction(finishCondition);

            // WAIT FOR LOADING PROCEDURE TO COMPLETE
            while (!(bool)condition.Invoke(null,null))
            {
                Thread.Sleep(1000);
            }
            // INVOKE UI AND HIDE LOADING SCREEN
            finalPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                finalPanel.BringToFront();
            });
            GlobalVarPool.loadingSpinner.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingSpinner.Visible = false;
            });
            GlobalVarPool.loadingLogo.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingLogo.Visible = false;
            });
            GlobalVarPool.loadingLabel.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.loadingLabel.Visible = false;
            });
            GlobalVarPool.settingsAbort.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.settingsAbort.Visible = true;
            });
            GlobalVarPool.settingsSave.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                GlobalVarPool.settingsSave.Visible = true;
            });
        }
    }
}
