using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    /// <summary>
    /// Provides methods to access the internet using the HTTP protocol.
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// Uses the HTTP protocol to download a website's HTML source code.
        /// </summary>
        /// <param name="uri">The url to get the HTML code from.</param>
        /// <returns>The website as HTML code</returns>
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

        /// <summary>
        /// Gets the domain from a url.
        /// </summary>
        /// <param name="url">the url to get the domain from.</param>
        /// <returns>The domain or null if the url is invalid.</returns>
        public static string GetDomain(string url)
        {
            try
            {
                Uri myUri = new Uri(url);
                return myUri.Host.Replace("www.", "");
            }
            catch
            {
                return null;
            }
        }
    }
}
