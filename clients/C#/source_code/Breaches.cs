using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    public static class Breaches
    {
        private class GetResponse
        {
            public Breach[] breaches = new Breach[] { };
        }

        public sealed class Breach
        {
            public string Name = string.Empty, Title = string.Empty, Domain = string.Empty, BreachDate = string.Empty, AddedDate = string.Empty, ModifiedDate = string.Empty, Description = string.Empty, LogoPath = string.Empty;
            public int PwnCount = -1;
            public string[] DataClasses = new string[] { };
            public bool IsVerified = false, IsFabricated = false, IsSensitive = false, IsRetired = false, IsSpamList = false;
        }

        public static List<Breach> FetchAll()
        {
            string uri = "https://haveibeenpwned.com/api/v2/breaches";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string HtmlCode = HttpHelper.Get(uri);
            GetResponse response = JsonConvert.DeserializeObject<GetResponse>("{\"breaches\":" + HtmlCode + "}");
            Breach[] breaches = response.breaches;
            return breaches.ToList();
        }

        public static Task<List<Breach>> FetchAllAsync()
        {
            return Task.Run(() => FetchAll());
        }
    }
}
