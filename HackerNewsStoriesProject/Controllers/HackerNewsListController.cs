using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Runtime.CompilerServices;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HackerNewsStoriesProject.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class HackerNewsListController : ControllerBase
    {
        private readonly ILogger<HackerNewsListController> _logger;
        private IMemoryCache _cache; //Used to Cache the API data to prevent round trips
        private readonly IConfiguration _configuration;

        private const string hackernewsListCacheKey = "hackernewsList"; 
        List<HackerNewsList> hackerstoriesList = new List<HackerNewsList>();// Collection to hold the results from Hacker News Stories and pass it on to Cache

        public HackerNewsListController(ILogger<HackerNewsListController> logger, IMemoryCache cacheProvider, IConfiguration configuration)
        {
            _logger = logger;
            _cache = cacheProvider;
            _configuration = configuration;
        }

        [HttpGet]
        //Fetch the Newest stories from Hacker News API and store it in a Collection
        public async Task<IEnumerable<HackerNewsList>> Get()
        {
                HackerNewsList hackerstoryList = new HackerNewsList();
                string[] hackerstoriesId;
                string _HackerStoryIDListURL = _configuration["HackerStoryIDListURL"];
                string _HackerStoriesListURL = _configuration["HackerStoriesListURL"];

                using (var httpClient1 = new HttpClient())
                {
                    //Fetch the Ids of the Newest Stories from Hacker News API 
                    using (var responsetopstories = await httpClient1.GetAsync(_HackerStoryIDListURL))
                    {
                        string apiResponseTopStories = await responsetopstories.Content.ReadAsStringAsync();
                        hackerstoriesId = apiResponseTopStories.Split(",");
                        for (int j = 0; j < hackerstoriesId.Length; j++)
                        {
                            hackerstoriesId[j] = hackerstoriesId[j].Trim();
                        }
                    }
                    //Check if Cache has the data of the Newest Stories. If present fetch from cache else fetch from API and store it in Cache                
                    List<HackerNewsList> hackerstoriesCacheList = new List<HackerNewsList>();
                    if (_cache.TryGetValue(hackernewsListCacheKey, out hackerstoriesCacheList))
                    {
                        _logger.Log(LogLevel.Information, "Retrieving Hacker Newest Stories from Cache");
                        hackerstoriesList = new List<HackerNewsList>(hackerstoriesCacheList);
                    }
                    else
                    {
                        for (int i = 0; i < hackerstoriesId.Length; i++)
                        {
                            using (var httpClient = new HttpClient())
                            {
                                using (var response = await httpClient.GetAsync(_HackerStoriesListURL + hackerstoriesId[i] + ".json?print=pretty"))
                                {
                                    string apiResponse = await response.Content.ReadAsStringAsync();
                                    hackerstoryList = JsonConvert.DeserializeObject<HackerNewsList>(apiResponse);
                                    hackerstoriesList.Add(hackerstoryList);
                                }
                            }
                        }
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(300))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                .SetPriority(CacheItemPriority.Normal)
                .SetSize(1024);
                        _cache.Set(hackernewsListCacheKey, hackerstoriesList, cacheEntryOptions);
                    }
                }
                return Enumerable.Range(1, hackerstoriesId.Length).Select(index => new HackerNewsList
                {
                    Id = hackerstoriesList[index - 1].Id,
                    Title = hackerstoriesList[index - 1].Title,
                    Time = hackerstoriesList[index - 1].Time,
                    URL = hackerstoriesList[index - 1].URL,
                    By = hackerstoriesList[index - 1].By,
                    Text = hackerstoriesList[index - 1].Text,
                    Type = hackerstoriesList[index - 1].Type
                })
                .ToArray();
        }
    }
}

   