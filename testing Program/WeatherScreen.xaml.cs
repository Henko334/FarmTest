using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Syncfusion.ExcelToPdfConverter;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.UI.Xaml.Diagram.Stencil;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static testing_Program.Blocks;
using static testing_Program.Entities.TableContext;

namespace testing_Program
{
    /// <summary>
    /// Interaction logic for Weather.xaml
    /// </summary>
    public partial class WeatherScreen : Page
    { 
        public WeatherScreen()
        {
            InitializeComponent();

            //https://api.open-meteo.com/v1/forecast?latitude=-33.91&longitude=18.4304&current=temperature_2m,relative_humidity_2m,precipitation,rain,weather_code,cloud_cover,wind_speed_10m,wind_direction_10m,wind_gusts_10m&timezone=Africa%2FCairo

            CallApiAndHandleResponse();
            GetCurrentWeather();
        }

        public async void CallApiAndHandleResponse()
        {
            try
            {
                using (var cont = new TableContext())
                {
                    var weatherData = cont.Weather
                        .Select(w => new WeatherGrid
                        {
                            Date = w.Date.ToShortDateString(),
                            MaxTemp = w.MaxTemp,
                            MinTemp = w.MinTemp,
                            Condition = w.WeatherConditions.Description,
                            WindSpeed = w.WindSpeed,
                            GustSpeed = w.GustSpeed,
                            RainChance = w.RainChance,
                            RainAmt = w.RainAmt,
                        })
                        .ToList();
                    chart1.DataContext = weatherData;

                    var lineSeries1 = new LineSeries
                    {
                        Label = "Max Temperature",
                        ItemsSource = weatherData,
                        XBindingPath = "Date",
                        YBindingPath = "MaxTemp",
                        StrokeThickness = 3,
                        ToolTip = "Max Temperature",

                    };

                    var lineSeries2 = new LineSeries
                    {
                        Label = "Min Temperature",
                        ItemsSource = weatherData,
                        XBindingPath = "Date",
                        YBindingPath = "MinTemp",
                        StrokeThickness = 3,
                        ToolTip = "Min Temperature",

                    };

                    var lineSeries3 = new LineSeries
                    {
                        Label = "Precipitation Probability",
                        ItemsSource = weatherData,
                        XBindingPath = "Date",
                        YBindingPath = "RainChance",
                        ToolTip = "Precipitation Probability",
                        StrokeThickness = 3,
                    };

                    var lineSeries4 = new LineSeries
                    {
                        Label = "Wind Speed",
                        ItemsSource = weatherData,
                        XBindingPath = "Date",
                        YBindingPath = "WindSpeed",
                        ToolTip = "Max Wind Speed",
                        StrokeThickness = 3,
                    };

                    var lineSeries5 = new LineSeries
                    {
                        Label = "Total Precipitation",
                        ItemsSource = weatherData,
                        XBindingPath = "Date",
                        YBindingPath = "RainAmt",
                        ToolTip = "Total Precipitation",
                        StrokeThickness = 3,
                    };

                    var lineSeries6 = new LineSeries
                    {
                        Label = "Gust Speeds",
                        ItemsSource = weatherData,
                        XBindingPath = "Date",
                        YBindingPath = "GustSpeed",
                        ToolTip = "Gust Speeds",
                        StrokeThickness = 3,
                    };

                    ChartAdornmentInfo adornmentInfo1 = new ChartAdornmentInfo()
                    {
                        ShowLabel = true,
                        SegmentLabelFormat = "0.00"
                    };

                    ChartAdornmentInfo adornmentInfo2 = new ChartAdornmentInfo()
                    {
                        ShowLabel = true,
                        SegmentLabelFormat = "0.00",

                    };

                    ChartAdornmentInfo adornmentInfo3 = new ChartAdornmentInfo()
                    {
                        ShowLabel = true,
                        SegmentLabelFormat = "0.00",

                    };

                    ChartAdornmentInfo adornmentInfo4 = new ChartAdornmentInfo()
                    {
                        ShowLabel = true,
                        SegmentLabelFormat = "0.00",

                    };

                    ChartAdornmentInfo adornmentInfo5 = new ChartAdornmentInfo()
                    {
                        ShowLabel = true,
                        SegmentLabelFormat = "0.00",
                    };

                    ChartAdornmentInfo adornmentInfo6 = new ChartAdornmentInfo()
                    {
                        ShowLabel = true,
                        SegmentLabelFormat = "0.00",
                    };

                    var legend = new ChartLegend
                    {
                        DockPosition = ChartDock.Top,
                        IconHeight = 25,
                        IconWidth = 25,
                        IconVisibility = Visibility.Visible
                    };

                    lineSeries1.AdornmentsInfo = adornmentInfo1;
                    lineSeries2.AdornmentsInfo = adornmentInfo2;
                    lineSeries3.AdornmentsInfo = adornmentInfo3;
                    lineSeries4.AdornmentsInfo = adornmentInfo4;
                    lineSeries5.AdornmentsInfo = adornmentInfo5;
                    lineSeries6.AdornmentsInfo = adornmentInfo6;

                    chart1.Legend = legend;

                    chart1.Series.Add(lineSeries1);
                    chart1.Series.Add(lineSeries2);
                    chart1.Series.Add(lineSeries3);
                    chart1.Series.Add(lineSeries4);
                    chart1.Series.Add(lineSeries5);
                    chart1.Series.Add(lineSeries6);

                    DisplayData(weatherData);
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error calling API: {ex.Message}");
            }
        }


        private void DisplayData(List<WeatherGrid> weathers)
        {
            var dataSource = weathers
                .Select(w => new
                {
                    Date = w.Date,
                    MinTemp = w.MinTemp + "°C",
                    MaxTemp =w.MaxTemp + "°C",
                    WeatherCondition = w.Condition,
                    PrecipitationChance = w.RainChance + "%",
                    TotalPrecipitation = w.RainAmt + "mm" 
                })
                .ToList();

            grdWeather.ItemsSource = dataSource;
        }

        private void GetCurrentWeather()
        {
            using (var cont = new TableContext())
            {
                var curr = cont.CurrentWeather
                    .Include(w => w.WeatherConditions)
                    .FirstOrDefault();

                lblTemp.Content = curr.Temperature + "°C";
                lblHum.Content = curr.Humidity + "%";
                lblcont.Content = curr.WeatherConditions.Description;
                lblCloud.Content = curr.CloudCover + "%";
                lblDirect.Content = DegreesToDirection(curr.WindDirection);
                lblWind.Content = curr.WindSpeed + "km/h";
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
