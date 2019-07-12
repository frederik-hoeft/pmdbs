using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    /// <summary>
    /// Responsible for providing an icon for an account.
    /// </summary>
    public static class Icon
    {
        private class GetResponse
        {
            public string url = string.Empty;
            public Favicon[] icons = new Favicon[] { };
        }
        private class Favicon
        {
            public string url = string.Empty, format = string.Empty, error = string.Empty, sha1sum = string.Empty;
            public int height = -1, width = -1;
        }
        
        /// <summary>
        /// Downloads the favicon of the given url.
        /// </summary>
        /// <param name="url">The url to get the favicon for.</param>
        /// <returns>The best favicon available.</returns>
        public static string Get(string url, bool checkUrl)
        {
            if (checkUrl)
            {
                bool urlIsValid = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (!urlIsValid)
                {
                    throw new ArgumentException("Invalid url.");
                }
            }
            string uri = "https://besticon-demo.herokuapp.com/allicons.json?url=" + url;
            string HtmlCode = HttpHelper.Get(uri);
            GetResponse response = JsonConvert.DeserializeObject<GetResponse>(HtmlCode);
            Favicon[] favicons = response.icons;
            string base64Image = string.Empty;
            bool successful = false;
            foreach (Favicon icon in favicons)
            {
                string iconLink = icon.url;
                ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
                SecurityProtocolType[] protocolTypes = new SecurityProtocolType[] { SecurityProtocolType.Ssl3, SecurityProtocolType.Tls, SecurityProtocolType.Tls11, SecurityProtocolType.Tls12 };
                
                for (int i = 0; i < protocolTypes.Length; i++)
                {
                    ServicePointManager.SecurityProtocol = protocolTypes[i];
                    try
                    {
                        using (WebClient client = new WebClient())
                        using (MemoryStream stream = new MemoryStream(client.DownloadData(iconLink)))
                        {
                            Bitmap bmpIcon = new Bitmap(Image.FromStream(stream, true, true));
                            if (bmpIcon.Width < 60 || bmpIcon.Height < 60)
                            {
                                break;
                            }
                            bmpIcon = (Bitmap)bmpIcon.GetThumbnailImage(350, 350, null, new IntPtr());
                            using (MemoryStream ms = new MemoryStream())
                            {
                                bmpIcon.Save(ms, ImageFormat.Png);
                                base64Image = Convert.ToBase64String(ms.ToArray());
                            }
                        }
                        successful = true;
                        break;
                    }
                    catch { }
                }

                if (successful)
                {
                    break;
                }
                
            }
            if (!successful)
            {
                throw new Exception("No Icon found");
            }
            return base64Image;
        }

        public static Image GetFromUrl(string url)
        {
            Image image = new Bitmap(1,1);
            bool successful = false;
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
            //SecurityProtocolType[] protocolTypes = new SecurityProtocolType[] { SecurityProtocolType.Ssl3, SecurityProtocolType.Tls, SecurityProtocolType.Tls11, SecurityProtocolType.Tls12 };

            //for (int i = 0; i < protocolTypes.Length; i++)
            //{
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                try
                {
                    using (WebClient client = new WebClient())
                    using (MemoryStream stream = new MemoryStream(client.DownloadData(url)))
                    {
                        Bitmap bmpIcon = new Bitmap(Image.FromStream(stream, true, true));
                        bmpIcon = (Bitmap)bmpIcon.GetThumbnailImage(350, 350, null, new IntPtr());
                        image = bmpIcon;
                    }
                    successful = true;
                    //break;
                }
                catch { }
            //}
            if (successful)
            {
                return image;
            }
            return null;
        }

        public static unsafe bool IsWhiteOnly(Image image)
        {
            using (Bitmap bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(image, 0, 0);
                }

                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

                int* pt = (int*)data.Scan0;
                bool res = true;
                for (int i = 0; i < data.Height * data.Width; i++)
                {
                    Color color = Color.FromArgb(pt[i]);

                    if (color.A == 255 && (color.R < 200 || color.G < 200 || color.B < 200))
                    {
                        res = false;
                        break;
                    }
                }

                bmp.UnlockBits(data);

                return res;
            }
        }

        public static Image RemoveTransparency(Bitmap bmp, Color newColor)
        {
            Bitmap target = new Bitmap(bmp.Size.Width, bmp.Size.Height);
            Graphics g = Graphics.FromImage(target);

            g.Clear(newColor);
            g.CompositingMode = CompositingMode.SourceOver;
            g.DrawImage(bmp, 0, 0);

            return target;
        }

        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (error == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine("X509Certificate [{0}] Policy Error: '{1}'",
                cert.Subject,
                error.ToString());

            return false;
        }
        /// <summary>
        /// Generates an icon for a given hostname.
        /// </summary>
        /// <param name="Hostname">The hostname to generate the icon for.</param>
        /// <returns>An icon of the first letter of the hostname.</returns>
        public static string Generate(string Hostname)
        {
            char[] alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char letter = Hostname.ToUpper()[0];
            // Create Optional Icons
            using (Bitmap bmp = new Bitmap(alphabet.Contains(letter) ? @"Resources\Icons\" + letter + ".png" : @"Resources\Icons\_UNKNOWN.png"))
            {
                Graphics g = Graphics.FromImage(bmp);
                // Set the image attribute's color mappings
                ColorMap[] colorMap = new ColorMap[1];
                Random rng = new Random();
                colorMap[0] = new ColorMap
                {
                    OldColor = Color.FromArgb(255, 102, 51),
                    NewColor = ColorExtensions.HSBToRGBConversion((float)rng.NextDouble(), (float)rng.Next(50, 90) / 100, 0.5f)
                };
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(colorMap);
                // Draw using the color map
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                g.DrawImage(bmp, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
                // string name = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }
}
