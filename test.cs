string domain = "";
try
{
	Uri UriToResolve = new Uri(Url);
	domain = UriToResolve.Host.Replace("www.", "");
}
catch
{
	//TODO: return error message
}
string uri = "https://i.olsh.me/icons?url=" + domain + "#result";
string HtmlCode = Get(uri).Split(new string[] { "<table" }, StringSplitOptions.None).Last();
HtmlDocument doc = new HtmlDocument();
doc.LoadHtml(HtmlCode);
//string BestImage = doc.DocumentNode.SelectNodes("//img")[0].GetAttributeValue("src", null);
HtmlNodeCollection Icons = doc.DocumentNode.SelectNodes("//img");
string localFilename = "";
bool successful = false;
foreach (HtmlNode Icon in Icons)
{
	try
	{
		string iconLink = Icon.GetAttributeValue("src", null);
		string imageFileExtension = iconLink.Split('.').Last().Split('?')[0];
		localFilename = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() + "." + imageFileExtension;
		using (WebClient client = new WebClient())
		{
			client.DownloadFile(iconLink, localFilename);
		}
		successful = true;
		break;
	}
	catch (Exception e)
	{
		Console.WriteLine(e.Message.ToUpper() + "\n" + e.ToString());
		continue;
	}
}
if (!successful)
{
	throw new Exception("No icons available or SSL/TLS Certificate Check failed @$#*!");
}