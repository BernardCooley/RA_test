using demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace demo.Controllers
{
    public class EventController : Controller
    {
        async public Task<ActionResult> Index(DateInput model)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(await GetEventsByDate(model.date));
            var events = LoadEventsFromXml(xd);
            return View(LoadEventsFromXml(xd));
        }

        public ActionResult Input()
        {
            return View(new DateInput());
        }

        [HttpPost]
        public ActionResult Date(DateInput model)
        {
            return View();
        }

        private List<EventDetails> LoadEventsFromXml(XmlDocument xd)
        {
            var events = new List<EventDetails>();

            foreach (XmlNode node in xd.SelectNodes("RA/events/event"))
            {
                events.Add(new EventDetails
                {
                    eventName = node["title"].InnerText,
                    lineup = node["lineup"].InnerText,
                });
            }
            return events;
        }

        async public Task<String> GetEventsByDate(string date)
        {
            return await GetRequest("https://www.residentadvisor.net/api/events.asmx/GetEventsByDay", 
                "2090605", "a465bde8-ba57-4ce0-95af-923cb856ab9d", "", "13", date);
        }

        async static Task<String> GetRequest(string url, string userID, string accessKey, string countryID,
            string areaID, string date)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestString = url + "?UserID=" + userID + "&AccessKey=" + accessKey + "&CountryID=" + countryID + "&AreaID=" + areaID + "&Date=" + date;
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