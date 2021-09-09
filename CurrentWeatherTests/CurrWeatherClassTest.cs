using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace CurrentWeatherTests
{
    public class Tests
    {
        
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("Salem", "Oregon", true, HttpStatusCode.OK)]
        [TestCase("", "", false, HttpStatusCode.BadRequest)]
        [TestCase("sanfransisco", "newyork", false, HttpStatusCode.NotFound)]
        [TestCase("><><>","<><><>", false, HttpStatusCode.NotFound)]
        [TestCase("salem", "Massachusetts", true, HttpStatusCode.OK)]
        public async Task GetWeather(string city, string state, bool expectedToPass, HttpStatusCode expectedStatusCode)
        {
            HttpClient _client = new HttpClient();
            string url = "https://localhost:44328/Weather/bycity";

            _client.BaseAddress = new Uri(url);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await _client.GetAsync($"?city={HttpUtility.UrlEncode(city)}&state={HttpUtility.UrlEncode(state)}");
            Assert.AreEqual(expectedToPass, response.IsSuccessStatusCode);
            Assert.AreEqual(expectedStatusCode, response.StatusCode);
        }
    }
}