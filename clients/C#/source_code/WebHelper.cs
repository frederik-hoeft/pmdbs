using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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

        public static Image GetFavIcons(string Url)
        {
            string domain = "";
            try
            {
                Uri UriToResolve = new Uri(Url);
                domain = UriToResolve.Host.Replace("www.", "");
            }
            catch
            {
                //return
            }
            string uri = "https://i.olsh.me/icons?url=" + domain + "#result";
            string HtmlCode = Get(uri).Split(new string[] { "<table" }, StringSplitOptions.None).Last();
            //bool isFavIcon = true;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlCode);
            string BestImage = doc.DocumentNode.SelectNodes("//img")[0].GetAttributeValue("src", null);
            string ImageFileExtension = BestImage.Split('.').Last().Split('?')[0];
            string localFilename = @"temp\" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() + "." + ImageFileExtension;
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(BestImage, localFilename);
            }
            Image FavIcon = Image.FromFile(localFilename);
            return FavIcon;
        }
    }
}
