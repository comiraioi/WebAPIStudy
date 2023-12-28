using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.HttpSys;
using NzWalks.Web.Models;
using NzWalks.Web.Models.DTO;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace NzWalks.Web.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public RegionsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        #region // Regions List
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<RegionDto> response = new List<RegionDto>();

            try
            {
                //Get All Regions from API
                var client = httpClientFactory.CreateClient();

                var httpResponseMessage = await client.GetAsync("https://localhost:7202/api/regions");
                httpResponseMessage.EnsureSuccessStatusCode();

                //var stringResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                //ViewBag.Response = stringResponseBody;

                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());

            }
            catch (Exception ex)
            {
                // 예외 로그 기록

            }


            //return View();
            return View(response);
        }
        #endregion

        #region // Add Region Form Page
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        #endregion

        #region // Add Region
        [HttpPost]
        public async Task<IActionResult> Add(AddRegionViewModel model)
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7202/api/regions"),
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if(response != null)
            {
                return RedirectToAction("Index", "Regions");
            }

            return View();
        }
        #endregion

        #region // Edit Region Form Page
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)  // 매개변수명은 asp-route- 뒤에 붙는 이름(id)과 일치해야 함
        {
            //ViewBag.Id = id;

            var client = httpClientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<RegionDto>($"https://localhost:7202/api/regions/{id.ToString()}");
            
            if (response != null)
            {
                return View(response);
            }

            return View(null);
        }
        #endregion

        #region // Edit Region Information
        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto region)
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7202/api/regions/{region.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(region), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if (response != null)
            {
                return RedirectToAction("Edit","Regions");
            }

            return View();
        }
        #endregion

        #region // Delete Region
        [HttpPost]
        public async Task<IActionResult> Delete(RegionDto region)
        {
            try
            {
                var client = httpClientFactory.CreateClient();

                var httpResponseMessage = await client.DeleteAsync($"https://localhost:7202/api/regions/{region.Id}");
                httpResponseMessage.EnsureSuccessStatusCode();

                return RedirectToAction("Index", "Regions");
            }
            catch(Exception ex)
            {
                // Console
            }

            return View("Edit");
        }
        #endregion
    }
}
