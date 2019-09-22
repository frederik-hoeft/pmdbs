using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace pmdbs
{
    //TODO:Support .svg images
    public class IconExtractor
    {
        private readonly string url = string.Empty;
        private List<Icon> allIcons = new List<Icon>();
        private IconExtractor(string _url)
        {
            url = _url;
        }

        public static IconExtractor Load(string _url)
        {
            bool isValid = Uri.TryCreate(_url, UriKind.Absolute, out Uri request);
            if (!isValid)
            {
                throw new UriFormatException();
            }
            _url = request.GetLeftPart(UriPartial.Authority);
            return new IconExtractor(_url);
        }

        public async Task Extract()
        {
            List<string> possibleIcons = new List<string> { "/favicon.ico", "/apple-touch-icon.png", "/apple-touch-icon-precomposed.png" };
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//link");

            for (int i = 0; i < nodes.Count; i++)
            {
                HtmlNode node = nodes[i];
                HtmlAttributeCollection attributes = node.Attributes;
                HtmlAttribute rel = attributes["rel"];
                if (rel.Value.ToLower().Contains("icon"))
                {
                    possibleIcons.Add(attributes["href"].Value);
                }
            }
            for (int i = 0; i < possibleIcons.Count; i++)
            {
                string iconLink = possibleIcons[i];
                if (!Uri.TryCreate(iconLink, UriKind.Absolute, out Uri result))
                {
                    possibleIcons[i] = url + iconLink;
                }
            }
            for (int i = 0; i < possibleIcons.Count; i++)
            {
                string iconLink = possibleIcons[i];
                DownloadIcon(iconLink);
            }
            Console.WriteLine("Getting all icons for " + url);
            for (int i = 0; i < allIcons.Count; i++)
            {
                Icon icon = allIcons[i];
                Console.WriteLine(icon.Url + "(" + icon.Width.ToString() + " x " + icon.Height.ToString() + ")");
            }
            Console.WriteLine("DONE!!!");
        }

        public void ApplyFilter(int minimumSize)
        {

        }

        public bool Available
        {
            get
            {
                return allIcons.Count != 0;
            }
        }

        private bool DownloadIcon(string iconLink)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri result))
            {
                iconLink = url + iconLink;
            }
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
                        Image image = Image.FromStream(stream, true, true);
                        allIcons.Add(new Icon(image, iconLink));
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

        public Icon GetBestIcon()
        {
            return allIcons.FirstOrDefault();
        }

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

        public class Icon
        {
            public readonly Image Image = null;
            public readonly int Height = -1, Width = -1;
            public readonly string Url = string.Empty, Sha256Sum = string.Empty;
            public Icon(Image image, string url)
            {
                Height = image.Height;
                Width = image.Width;
                Image = image;
                Url = url;
                using (System.Security.Cryptography.SHA256Cng ShaHashFunction = new System.Security.Cryptography.SHA256Cng())
                {
                    ImageConverter converter = new ImageConverter();
                    Sha256Sum = Convert.ToBase64String(ShaHashFunction.ComputeHash((byte[])converter.ConvertTo(image, typeof(byte[]))));
                }
            }
            public string ToBase64()
            {
                Bitmap bmpIcon = (Bitmap)Image.GetThumbnailImage(350, 350, null, new IntPtr());
                using (MemoryStream ms = new MemoryStream())
                {
                    bmpIcon.Save(ms, ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }
}
