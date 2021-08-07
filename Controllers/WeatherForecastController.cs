using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Amazon.DynamoDBv2.DataModel;

namespace weatherforecast.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        public readonly IDynamoDBContext _dynamodbcontext;

        public WeatherForecastController( IDynamoDBContext context,
            ILogger<WeatherForecastController> logger)
        {
            _dynamodbcontext = context;
            _logger = logger;
        }

       

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get(string city = "Bangalore")
        {
             return await _dynamodbcontext.QueryAsync<WeatherForecast>(city).GetRemainingAsync();
         //   return GenerateDummmyForecast(city);
        }

        [HttpPost]
        public async void Post(string city)
        {
            //var data = GenerateDummmyForecast(city);
            //foreach(var item in data)
            //{
            //    await _dynamodbcontext.SaveAsync(item);
            //}

            var item = await _dynamodbcontext.LoadAsync<WeatherForecast>(city, DateTime.Now.Date.AddDays(1));
            item.Summary = "Test";
           await _dynamodbcontext.SaveAsync(item);           
        }

        [HttpDelete]
        public  async void Delete(string city)
        {
            var item = await _dynamodbcontext.LoadAsync<WeatherForecast>(city, DateTime.Now.Date.AddDays(1));
            await _dynamodbcontext.DeleteAsync(item);
        }

        private static IEnumerable<WeatherForecast> GenerateDummmyForecast(string city)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                City = city,
                Date = DateTime.Now.Date.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
