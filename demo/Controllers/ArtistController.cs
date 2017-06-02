using demo.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace demo.Controllers
{
    public class ArtistController : Controller
    {
        public ActionResult Index(String lineup)
        {
            return View(ParseLineup(lineup));
        }

        public List<string> ParseLineup(string lineup)
        {
            return lineup.Split(',').Select(a => a.Trim()).ToList();
        }

        [HttpGet]
        async public Task<ActionResult> GetArtist(string artistName)
        {
            System.Diagnostics.Debug.WriteLine("Get artist called");
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

        async public Task<String> GetArtistDetailsByName(string artistName)
        {
            return await GetRequest("https://www.residentadvisor.net/api/dj.asmx/getartist", "2090605", 
                "a465bde8-ba57-4ce0-95af-923cb856ab9d", "", artistName, "");
        }

        async static Task<String> GetRequest(string url, string userID, string accessKey, string djID,
            string artistName, string artistURL)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestString = url+"?UserID="+userID+"&AccessKey="+accessKey
                    +"&DJID="+djID+"&ArtistName="+artistName+"&URL="+url;

                using (HttpResponseMessage response = await client.GetAsync(requestString))
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