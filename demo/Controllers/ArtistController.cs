using demo.Models;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;

namespace demo.Controllers
{
    public class ArtistController : Controller
    {
        public ActionResult Index()
        {
            var artists = LoadArtistsFromXml();
            var summaries = new List<ArtistSummary>();

            artists.ForEach(a => summaries.Add(new ArtistSummary { ID = a.djid, Name = a.name }));

            return View(summaries);
        }

        [HttpGet]
        public ActionResult GetArtist(int id)
        {
            var artists = LoadArtistsFromXml();
            var selectedArtist = artists.FirstOrDefault(a => a.djid == id);
            
            return Json(selectedArtist, JsonRequestBehavior.AllowGet);
        }

        private List<Models.ArtistDetails> LoadArtistsFromXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("/resources/artists.xml"));

            var artists = new List<Models.ArtistDetails>();
            foreach (XmlNode node in doc.SelectNodes("RA/artist"))
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
    }
}