using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    class WebHelper
    {
        public static async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetFavIcons(string Url)
        {
            string domain = "";
            try
            {
                Uri UriToResolve = new Uri(Url);
                domain = UriToResolve.Host.Replace("www.", "");
            }
            catch
            {
                throw new ArgumentException("Invalid url"); // TODO : CATCH EXCEPTION IN SCOPE ABOVE AND HANDLE ERROR MESSAGE RETURN
            }
            if (!IsValidDomainName(domain))
            {
                throw new InvalidDomainException(); // TODO : CATCH EXCEPTION IN SCOPE ABOVE AND HANDLE ERROR MESSAGE RETURN
            }
            string uri = "https://i.olsh.me/icons?url=" + domain + "#result";
            string HtmlCode = Get(uri).Split(new string[] { "<table" }, StringSplitOptions.None).Last();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlCode);
            //string BestImage = doc.DocumentNode.SelectNodes("//img")[0].GetAttributeValue("src", null);
            HtmlNodeCollection Icons = doc.DocumentNode.SelectNodes("//img");
            string base64Image = string.Empty;
            bool successful = false;
            foreach (HtmlNode Icon in Icons)
            {
                string iconLink = Icon.GetAttributeValue("src", null);
                string imageFileExtension = iconLink.Split('.').Last().Split('?')[0];
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
                            if (bmpIcon.Width < 64 || bmpIcon.Height < 64)
                            {
                                break;
                            }
                            bmpIcon = (Bitmap)bmpIcon.GetThumbnailImage(350, 350, null, new IntPtr());
                            using (MemoryStream ms = new MemoryStream())
                            {
                                bmpIcon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                base64Image = Convert.ToBase64String(ms.ToArray());
                            }
                        }
                        successful = true;
                        break;
                    }
                    catch (Exception e)
                    {
                        //CustomException.ThrowNew.NetworkException(e.ToString());
                    }
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


        public static bool IsValidDomainName(string name)
        {
            return Uri.CheckHostName(name) != UriHostNameType.Unknown;
        }
    }
}
