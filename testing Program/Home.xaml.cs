using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Syncfusion.DocIO.DLS;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using testing_Program.Entities;


namespace testing_Program
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private HttpClient httpClient;

        public Home()
        {
            InitializeComponent();
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast?latitude=-33.91&longitude=18.4304&current=temperature_2m,relative_humidity_2m,precipitation,rain,weather_code,cloud_cover,wind_speed_10m,wind_direction_10m,wind_gusts_10m&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_sum,precipitation_probability_max,wind_speed_10m_max,wind_gusts_10m_max&timezone=Africa%2FCairo");
            CallApiAndHandleResponse();
            LoadWeather();
        }

        public async void CallApiAndHandleResponse()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync("");
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                WeatherResult responseObject = JsonConvert.DeserializeObject<WeatherResult>(jsonResponse);
                using (var cont = new TableContext())
                {
                    for (int i = 0; i < responseObject.Daily.time.Count; i++)
                    {
                        int day = 1 + i;
                        var existingWeather = cont.Weather.FirstOrDefault(w => w.Day == day);

                        if (existingWeather == null)
                        {
                            Weather weather = new Weather
                            {
                                Day = i + 1, // Assuming day starts from 1
                                Date = DateTime.Parse(responseObject.Daily.time[i]),
                                MaxTemp = Convert.ToDecimal(responseObject.Daily.temperature_2m_Max[i]),
                                MinTemp = Convert.ToDecimal(responseObject.Daily.temperature_2m_Min[i]),
                                RainChance = responseObject.Daily.precipitation_probability_max[i],
                                RainAmt = Convert.ToDecimal(responseObject.Daily.precipitation_sum[i]),
                                WindSpeed = Convert.ToDecimal(responseObject.Daily.wind_speed_10m_max[i]),
                                GustSpeed = Convert.ToDecimal(responseObject.Daily.wind_gusts_10m_max[i]),
                                Condition = responseObject.Daily.weather_code[i]
                            };
                            cont.Weather.Add(weather);
                        }
                        else
                        {
                            existingWeather.Date = DateTime.Parse(responseObject.Daily.time[i]);
                            existingWeather.MaxTemp = Convert.ToDecimal(responseObject.Daily.temperature_2m_Max[i]);
                            existingWeather.MinTemp = Convert.ToDecimal(responseObject.Daily.temperature_2m_Min[i]);
                            existingWeather.RainChance = responseObject.Daily.precipitation_probability_max[i];
                            existingWeather.RainAmt = Convert.ToDecimal(responseObject.Daily.precipitation_sum[i]);
                            existingWeather.WindSpeed = Convert.ToDecimal(responseObject.Daily.wind_speed_10m_max[i]);
                            existingWeather.GustSpeed = Convert.ToDecimal(responseObject.Daily.wind_gusts_10m_max[i]);
                            existingWeather.Condition = responseObject.Daily.weather_code[i];
                        }
                    }

                    var currWeath = cont.CurrentWeather
                        .Where(c => c.Entry == 1)
                        .FirstOrDefault();

                    if (currWeath != null)
                    {
                        currWeath.Temperature = Convert.ToDecimal(responseObject.Current.temperature_2m);
                        currWeath.Humidity = responseObject.Current.relative_humidity_2m;
                        currWeath.Precipitation = Convert.ToDecimal(responseObject.Current.precipitation);
                        currWeath.Rain = Convert.ToDecimal(responseObject.Current.rain);
                        currWeath.Condition = responseObject.Current.weather_code;
                        currWeath.CloudCover = responseObject.Current.cloud_cover;
                        currWeath.WindSpeed = Convert.ToDecimal(responseObject.Current.wind_speed_10m);
                        currWeath.WindDirection = responseObject.Current.wind_direction_10m;
                        currWeath.WindGusts = Convert.ToDecimal(responseObject.Current.wind_gusts_10m);
                    }
                    else
                    {
                        CurrentWeather cWeath = new CurrentWeather {
                            Entry = 1,
                            Temperature = Convert.ToDecimal(responseObject.Current.temperature_2m),
                            Humidity = responseObject.Current.relative_humidity_2m,
                            Precipitation = Convert.ToDecimal(responseObject.Current.precipitation),
                            Rain = Convert.ToDecimal(responseObject.Current.rain),
                            Condition = responseObject.Current.weather_code,
                            CloudCover = responseObject.Current.cloud_cover,
                            WindSpeed = Convert.ToDecimal(responseObject.Current.wind_speed_10m),
                            WindDirection = responseObject.Current.wind_direction_10m,
                            WindGusts = Convert.ToDecimal(responseObject.Current.wind_gusts_10m),
                        };
                        cont.CurrentWeather.Add(cWeath);
                    }

                    await cont.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadWeather()
        {
            using(var cont = new TableContext())
            {
                var curr = cont.CurrentWeather
                    .Include(w => w.WeatherConditions)
                    .FirstOrDefault();

                lblCloud.Content = curr.CloudCover + "%";
                lblWind.Content = curr.WindSpeed + "km/h";
                lblcont.Content = curr.WeatherConditions.Description;
                lblTemp.Content = curr.Temperature + "°C";
                lblHum.Content = curr.Humidity + "%";
                lblDirect.Content = DegreesToDirection(curr.WindDirection);
            }
        }

        public static string DegreesToDirection(int degrees)
        {
            degrees = (degrees + 360) % 360;

            string[] directions = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
            int[] degreeRanges = { 11, 34, 56, 79, 101, 124, 146, 169, 191, 214, 236, 259, 281, 304, 326, 349 };

            for (int i = 0; i < directions.Length; i++)
            {
                if (degrees < degreeRanges[i])
                {
                    return directions[i];
                }
            }
            return "N";
        }
    }
}
