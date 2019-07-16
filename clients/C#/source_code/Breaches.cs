using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    /// <summary>
    /// Provides required methods to interact with the breach info web API.
    /// </summary>
    public static class Breaches
    {
        private class GetResponse
        {
            public Breach[] breaches = new Breach[] { };
        }

        /// <summary>
        /// Holds information about a specific breach.
        /// </summary>
        public sealed class Breach
        {
            /// <summary>
            /// Breach information
            /// </summary>
            public string Name = string.Empty, Title = string.Empty, Domain = string.Empty, BreachDate = string.Empty, AddedDate = string.Empty, ModifiedDate = string.Empty, Description = string.Empty, LogoPath = string.Empty;
            /// <summary>
            /// The number of affected accounts.
            /// </summary>
            public int PwnCount = -1;
            /// <summary>
            /// The leaked information.
            /// </summary>
            public string[] DataClasses = new string[] { };
            /// <summary>
            /// Further information about this breach.
            /// </summary>
            public bool IsVerified = false, IsFabricated = false, IsSensitive = false, IsRetired = false, IsSpamList = false;
        }

        /// <summary>
        /// Get all documented data breaches from the web API.
        /// </summary>
        /// <returns>A list of data breaches.</returns>
        public static List<Breach> FetchAll()
        {
            string uri = "https://haveibeenpwned.com/api/v2/breaches";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                string HtmlCode = HttpHelper.Get(uri);
                GetResponse response = JsonConvert.DeserializeObject<GetResponse>("{\"breaches\":" + HtmlCode + "}");
                Breach[] breaches = response.breaches;
                return breaches.ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get all documented data breaches from the web API using async Tasks.
        /// </summary>
        /// <returns>Awaitable list of data breaches.</returns>
        public static Task<List<Breach>> FetchAllAsync()
        {
            return Task.Run(() => FetchAll());
        }
    }
}
