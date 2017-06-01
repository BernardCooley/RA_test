using demo.Models;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;

namespace demo.Controllers
{
    public class ArtistController : Controller
    {
        XmlDocument xd = new XmlDocument();
        String xmlString = "";
        StringBuilder sb = new StringBuilder();
        List<ArtistDetails> artists = new List<ArtistDetails>();

        async public Task<ActionResult> Index()
        {
            await RunRequests(lineup(), xmlString);
            xmlString = parseXMLString(sb);
            xd.LoadXml(xmlString);
            System.Diagnostics.Debug.WriteLine(xd);
            artists = LoadArtistsFromXml(xd);
            var summaries = new List<ArtistSummary>();

            artists.ForEach(a => summaries.Add(new ArtistSummary { ID = a.djid, Name = a.name }));

            return View(summaries);
        }

        [HttpGet]
        async public Task<ActionResult> GetArtist(int id)
        {
            await RunRequests(lineup(), xmlString);
            xmlString = parseXMLString(sb);
            xd.LoadXml(xmlString);
            artists = LoadArtistsFromXml(xd);
            var selectedArtist = artists.FirstOrDefault(a => a.djid == id);
            return Json(selectedArtist, JsonRequestBehavior.AllowGet);
        }

        private List<Models.ArtistDetails> LoadArtistsFromXml(XmlDocument xd)
        {
            System.Diagnostics.Debug.WriteLine("load artists from xml called");
            var artists = new List<Models.ArtistDetails>();
            foreach (XmlNode node in xd.SelectNodes("RA/artist"))
            {
                artists.Add(new Models.ArtistDetails
                {
                    djid = int.Parse(node["djid"].InnerText),
                    name = node["artistname"].InnerText,
                    country = node["countryname"].InnerText,
                    labels = node["labels"].InnerText,
                    website = node["website"].InnerText,
                    raProfile = node["raprofile"].InnerText,
                    twitter = node["twitter"].InnerText,
                    facebook = node["facebook"].InnerText,
                    discogs = node["discogs"].InnerText,
                    profileImageURL = node["profileimage"].InnerText,
                    bioSmall = node["biointro"].InnerText,
                });
            }
            return artists;
        }

        async static void GetRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        string myContent = await content.ReadAsStringAsync();
                    }
                        
                }
            }
        }

        public List<string> lineup()
        {
            List<string> lineup = new List<string>();
            lineup.Add("Laurent Garnier");
            lineup.Add("Paul Woolford");
            lineup.Add("Carl Craig");
            lineup.Add("Henry Saiz");
            lineup.Add("Jeff Mills");
            lineup.Add("Cooley");
            lineup.Add("ghsertsjh");

            return lineup;
        }

        async public Task RunRequests(List<string> lineup, string xmlString)
        {
            foreach(string s in lineup)
            {
                await PostRequest("https://www.residentadvisor.net/api/dj.asmx/getartist", "2090605", "a465bde8-ba57-4ce0-95af-923cb856ab9d", "", s, "", xd, sb);
            }
        }

        async static Task PostRequest(string url, string userID, string accessKey, string djID, string artistName, string artistURL, XmlDocument xd, StringBuilder sb)
        {
            IEnumerable<KeyValuePair<string, string>> queries = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("UserID", userID),
                new KeyValuePair<string, string>("AccessKey", accessKey),
                new KeyValuePair<string, string>("DJID", djID),
                new KeyValuePair<string, string>("ArtistName", artistName),
                new KeyValuePair<string, string>("URL", artistURL)
            };
            HttpContent q = new FormUrlEncodedContent(queries);

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.PostAsync(url, q))
                {
                    using (HttpContent content = response.Content)
                    {
                        string myContent = await content.ReadAsStringAsync();
                        HttpContentHeaders headers = content.Headers;

                        sb.Append(myContent);
                    }
                }
            }
        }

        public String parseXMLString(StringBuilder sb)
        {
            String xmlString = sb.ToString();
            xmlString = xmlString.Replace("</RA>", "");
            xmlString = xmlString.Replace("<RA>", "");
            xmlString = xmlString.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            xmlString = xmlString.Insert(0, "\n<RA>");
            xmlString = xmlString.Insert(0, "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlString = xmlString.Insert(xmlString.Length, "</RA>");

            return xmlString;
        }
    }
}