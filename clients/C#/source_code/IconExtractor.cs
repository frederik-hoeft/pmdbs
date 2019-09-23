using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Xml;
using Svg;

namespace pmdbs
{
    /// <summary>
    /// Provides functionality to extract or generate favicons from / for web addresses.
    /// </summary>
    public class IconExtractor
    {
        private readonly string _url = string.Empty;
        private List<Icon> allIcons = new List<Icon>();
        private IconExtractor(string url)
        {
            this._url = url;
        }

        /// <summary>
        /// Loads the specified url.
        /// </summary>
        /// <param name="url">The url to extract icons from.</param>
        /// <returns>The IconExtractor object.</returns>
        public static IconExtractor Load(string url)
        {
            bool isValid = Uri.TryCreate(url, UriKind.Absolute, out Uri request);
            if (!isValid)
            {
                throw new UriFormatException();
            }
            url = request.GetLeftPart(UriPartial.Authority);
            return new IconExtractor(url);
        }

        /// <summary>
        /// Extracts all icons from the loaded url.
        /// </summary>
        /// <returns></returns>
        public Task Extract() => Task.Run(() => extractSynchronous());

        private void extractSynchronous()
        {
            List<string> possibleIcons = new List<string> { "/favicon.ico", "/apple-touch-icon.png", "/apple-touch-icon-precomposed.png" };
            HtmlWeb web = new HtmlWeb();
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
            SecurityProtocolType[] protocolTypes = new SecurityProtocolType[] { SecurityProtocolType.Ssl3, SecurityProtocolType.Tls, SecurityProtocolType.Tls11, SecurityProtocolType.Tls12 };

            for (int i = 0; i < protocolTypes.Length; i++)
            {
                ServicePointManager.SecurityProtocol = protocolTypes[i];
                try
                {
                    HtmlDocument doc = web.Load(_url);
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//link");

                    for (int j = 0; j < nodes.Count; j++)
                    {
                        HtmlNode node = nodes[j];
                        HtmlAttributeCollection attributes = node.Attributes;
                        HtmlAttribute rel = attributes["rel"];
                        if (rel.Value.ToLower().Contains("icon"))
                        {
                            possibleIcons.Add(attributes["href"].Value);
                        }
                    }
                }
                catch (WebException e)
                {
                    if (e.Status != WebExceptionStatus.SecureChannelFailure)
                    {
                        return;
                    }
                }
            }
                
            for (int i = 0; i < possibleIcons.Count; i++)
            {
                string iconLink = possibleIcons[i];
                if (!Uri.TryCreate(iconLink, UriKind.Absolute, out Uri result))
                {
                    possibleIcons[i] = _url + iconLink;
                }
            }
            List<string> downloadedIcons = new List<string>();
            for (int i = 0; i < possibleIcons.Count; i++)
            {
                string iconLink = possibleIcons[i];
                if (downloadedIcons.Contains(iconLink))
                {
                    // DON'T WANT TO DOWNLOAD THE SAME ICON TWICE
                    continue;
                }
                downloadedIcons.Add(iconLink);
                Icon icon = DownloadImageSynchronous(iconLink);
                if (icon != null)
                {
                    allIcons.Add(icon);
                }
            }
        }

        /// <summary>
        /// Filters and sorts all extracted icons.
        /// </summary>
        /// <param name="minimumWidth">The minimum allowed icon width.</param>
        public void ApplyFilter(int minimumWidth)
        {
            List<Icon> iconsTemp = new List<Icon>();
            foreach (Icon icon in allIcons)
            {
                if (icon.Width >= minimumWidth)
                {
                    iconsTemp.Add(icon);
                }
            }
            allIcons = iconsTemp.OrderByDescending(icon => icon.Width).ToList();
        }

        /// <summary>
        /// Checks wether icons have been found on this website.
        /// </summary>
        public bool IconsAvailable
        {
            get
            {
                return allIcons.Count != 0;
            }
        }

        /// <summary>
        /// Directly download an image from a given url.
        /// </summary>
        /// <param name="imageLink">The url to download the image from.</param>
        /// <returns>The downloaded image as an Icon object.</returns>
        public static Task<Icon> DownloadImage(string imageLink)
        {
            return Task.Run(() => DownloadImageSynchronous(imageLink));
        }
        
        private static Icon DownloadImageSynchronous(string imageLink)
        {
            string extension = imageLink.Split('.').Last().Split('?').First();
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
            SecurityProtocolType[] protocolTypes = new SecurityProtocolType[] { SecurityProtocolType.Ssl3, SecurityProtocolType.Tls, SecurityProtocolType.Tls11, SecurityProtocolType.Tls12 };

            for (int i = 0; i < protocolTypes.Length; i++)
            {
                ServicePointManager.SecurityProtocol = protocolTypes[i];
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        using (MemoryStream stream = new MemoryStream(client.DownloadData(imageLink)))
                        {
                            Image image = null;
                            switch (extension.ToLower())
                            {
                                case "ico":
                                    // THE BITMAP OBJECT DOES NOT PARSE ICONS TO THE BEST QUALITY AVAILABLE BY DEFAULT
                                    // BY CREATING A NEW ICON WITH A VERY LARGE SIZE IT TAKES THE BEST QUALITY AVAILABLE TO THEN BE CONVERTED TO A BITMAP
                                    System.Drawing.Icon originalIcon = new System.Drawing.Icon(stream);
                                    System.Drawing.Icon icon = new System.Drawing.Icon(originalIcon, new Size(1024, 1024));
                                    image = icon.ToBitmap();
                                    break;
                                case "svg":
                                    XmlDocument xml = new XmlDocument();
                                    xml.Load(stream);
                                    SvgDocument svgDocument = SvgDocument.Open(xml);
                                    using (Bitmap bitmap = svgDocument.Draw(1024, 1024))
                                    {
                                        image = bitmap.GetThumbnailImage(1024, 1024, null, new IntPtr());
                                    }
                                    break;
                                default:
                                    image = Image.FromStream(stream, true, true);
                                    break;
                            }
                            // ICON DOWNLOADED SUCCESSFULLY --> RETURN CUSTOM ICON OBJECT
                            return new Icon(image, imageLink, extension);
                        }
                    }
                    catch (WebException e)
                    {
                        if (e.Status != WebExceptionStatus.SecureChannelFailure)
                        {
                            return null;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the best available Icon for the specified filter.
        /// </summary>
        /// <returns>The best available Icon.</returns>
        public Icon GetBestIcon()
        {
            return allIcons.FirstOrDefault();
        }

        /// <summary>
        /// Gets all icons that passed the filter.
        /// </summary>
        /// <returns></returns>
        public List<Icon> GetIconsAsList()
        {
            return allIcons;
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
        /// A representation of a favicon.
        /// </summary>
        public class Icon
        {
            /// <summary>
            /// The icon as an Image object.
            /// </summary>
            public readonly Image Image = null;
            /// <summary>
            /// The size in pixels of the icon.
            /// </summary>
            public readonly int Height = -1, Width = -1;
            /// <summary>
            /// The source url of the icon.
            /// </summary>
            public readonly string Url = string.Empty;
            /// <summary>
            /// A Base64 SHA-256 Checksum.
            /// </summary>
            public readonly string Sha256Sum = string.Empty;
            /// <summary>
            /// The original file format of the icon.
            /// </summary>
            public readonly string Extension = string.Empty;
            /// <summary>
            /// Creates a new Icon object.
            /// </summary>
            /// <param name="image">The image the icon object is created from.</param>
            /// <param name="url">The source url of the icon.</param>
            /// <param name="extension">The original file format of the icon.</param>
            public Icon(Image image, string url, string extension)
            {
                Height = image.Height;
                Width = image.Width;
                Image = image;
                Extension = extension;
                Url = url;
                using (System.Security.Cryptography.SHA256Cng ShaHashFunction = new System.Security.Cryptography.SHA256Cng())
                {
                    ImageConverter converter = new ImageConverter();
                    Sha256Sum = Convert.ToBase64String(ShaHashFunction.ComputeHash((byte[])converter.ConvertTo(image, typeof(byte[]))));
                }
            }

            /// <summary>
            /// Converts the icon to a base64 string representation.
            /// </summary>
            /// <returns>The base64 string representation of the icon.</returns>
            public string ToBase64String()
            {
                Bitmap bmpIcon = (Bitmap)Image.GetThumbnailImage(350, 350, null, new IntPtr());
                using (MemoryStream ms = new MemoryStream())
                {
                    bmpIcon.Save(ms, ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
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
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }
}
