using demo.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Threading.Tasks;

namespace demo.Controllers
{
    public class ArtistController : Controller
    {
        public ActionResult Index()
        {
            return View(lineup());
        }

        [HttpGet]
        async public Task<ActionResult> GetArtist(string artistName)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(await GetArtistDetailsByName(artistName));
            var artistDetails = LoadArtistsFromXml(xd);
            return Json(artistDetails, JsonRequestBehavior.AllowGet);
        }

        private ArtistDetails LoadArtistsFromXml(XmlDocument xd)
        {
            var artistDetails = new ArtistDetails();

            foreach (XmlNode node in xd.SelectNodes("RA/artist"))
            {
                artistDetails = new ArtistDetails
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
                };
            }
            return artistDetails;
        }

        private List<ArtistDetails> LoadAllArtistsFromXml(XmlDocument xd)
        {
            var artists = new List<ArtistDetails>();
            foreach (XmlNode node in xd.SelectNodes("RA/artist"))
            {
                artists.Add(new ArtistDetails
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

            return lineup;
        }

        async public Task<String> GetArtistDetailsByName(string artistName)
        {
            return await PostRequest("https://www.residentadvisor.net/api/dj.asmx/getartist", "2090605", 
                "a465bde8-ba57-4ce0-95af-923cb856ab9d", "", artistName, "");
        }

        async static Task<String> PostRequest(string url, string userID, string accessKey, string djID, 
            string artistName, string artistURL)
        {
            IEnumerable <KeyValuePair<string, string>> queries = new List<KeyValuePair<string, string>>()
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
                        HttpContentHeaders headers = content.Headers;
                        return await content.ReadAsStringAsync();
                        
                    }

                }
            }
        }
    }
}