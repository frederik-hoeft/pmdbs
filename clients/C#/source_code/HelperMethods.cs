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
        public static string GetOS()
        {
            return Environment.OSVersion.VersionString;
        }
        public static void InvokeOutputLabel(string text)
        {
            if (GlobalVarPool.outputLabelIsValid)
            {
                GlobalVarPool.OutputLabel.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    GlobalVarPool.OutputLabel.Text = text;
                });
            }
        }
        public abstract class RoundedRectangle
        {
            public enum RectangleCorners
            {
                None = 0, TopLeft = 1, TopRight = 2, BottomLeft = 4, BottomRight = 8,
                All = TopLeft | TopRight | BottomLeft | BottomRight
            }

            public static GraphicsPath Create(int x, int y, int width, int height,
                                              int radius, RectangleCorners corners)
            {
                int xw = x + width;
                int yh = y + height;
                int xwr = xw - radius;
                int yhr = yh - radius;
                int xr = x + radius;
                int yr = y + radius;
                int r2 = radius * 2;
                int xwr2 = xw - r2;
                int yhr2 = yh - r2;

                GraphicsPath p = new GraphicsPath();
                p.StartFigure();

                //Top Left Corner
                if ((RectangleCorners.TopLeft & corners) == RectangleCorners.TopLeft)
                {
                    p.AddArc(x, y, r2, r2, 180, 90);
                }
                else
                {
                    p.AddLine(x, yr, x, y);
                    p.AddLine(x, y, xr, y);
                }

                //Top Edge
                p.AddLine(xr, y, xwr, y);

                //Top Right Corner
                if ((RectangleCorners.TopRight & corners) == RectangleCorners.TopRight)
                {
                    p.AddArc(xwr2, y, r2, r2, 270, 90);
                }
                else
                {
                    p.AddLine(xwr, y, xw, y);
                    p.AddLine(xw, y, xw, yr);
                }

                //Right Edge
                p.AddLine(xw, yr, xw, yhr);

                //Bottom Right Corner
                if ((RectangleCorners.BottomRight & corners) == RectangleCorners.BottomRight)
                {
                    p.AddArc(xwr2, yhr2, r2, r2, 0, 90);
                }
                else
                {
                    p.AddLine(xw, yhr, xw, yh);
                    p.AddLine(xw, yh, xwr, yh);
                }

                //Bottom Edge
                p.AddLine(xwr, yh, xr, yh);

                //Bottom Left Corner
                if ((RectangleCorners.BottomLeft & corners) == RectangleCorners.BottomLeft)
                {
                    p.AddArc(x, yhr2, r2, r2, 90, 90);
                }
                else
                {
                    p.AddLine(xr, yh, x, yh);
                    p.AddLine(x, yh, x, yhr);
                }

                //Left Edge
                p.AddLine(x, yhr, x, yr);

                p.CloseFigure();
                return p;
            }

            public static GraphicsPath Create(Rectangle rect, int radius, RectangleCorners c)
            { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius, c); }

            public static GraphicsPath Create(int x, int y, int width, int height, int radius)
            { return Create(x, y, width, height, radius, RectangleCorners.All); }

            public static GraphicsPath Create(Rectangle rect, int radius)
            { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius); }

            public static GraphicsPath Create(int x, int y, int width, int height)
            { return Create(x, y, width, height, 5); }

            public static GraphicsPath Create(Rectangle rect)
            { return Create(rect.X, rect.Y, rect.Width, rect.Height); }
        }
    }
}
