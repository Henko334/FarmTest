using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testing_Program.Entities
{
    public class TableContext : DbContext
    {
        #region DbSets
        public DbSet<Weather> Weather { get; set; }
        public DbSet<WeatherConditions> WeatherConditions { get; set; }
        public DbSet<CurrentWeather> CurrentWeather { get; set; }
        public DbSet<FruitType> FruitType { get; set; }
        public DbSet<FruitVariant> FruitVariant { get; set; }
        public DbSet<BlockData> BlockData { get; set; }
        public DbSet<BlockDetails> BlockDetails { get; set; }
        public DbSet<Tractors> Tractors { get; set; }
        public DbSet<Sprayers> Sprayers { get; set; }
        public DbSet<Trailers> Trailers { get; set; }
        public DbSet<TractorSprayer> TractorSprayer { get; set; }
        public DbSet<TractorTrailer> TractorTrailer { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=D:\\testDB\\test.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Keys
            modelBuilder.Entity<TractorSprayer>()
                .HasKey(s => new { s.TractorID, s.SprayerID });

            modelBuilder.Entity<TractorTrailer>()
                .HasKey(s => new { s.TractorID, s.TrailerID });
            #endregion

            #region Relationships
            modelBuilder.Entity<Weather>()
                .HasOne(c => c.WeatherConditions)
                .WithMany(w => w.Weather)
                .HasForeignKey(w => w.Condition);

            modelBuilder.Entity<CurrentWeather>()
                .HasOne(c=> c.WeatherConditions)
                .WithMany(w => w.CurrentWeather)
                .HasForeignKey(w => w.Condition);

            modelBuilder.Entity<FruitVariant>()
                .HasOne(f => f.FruitType)
                .WithMany(f => f.FruitVariant)
                .HasForeignKey(f => f.FruitID);

            modelBuilder.Entity<BlockData>()
                .HasOne(f => f.FruitType)
                .WithMany(b => b.BlockData)
                .HasForeignKey(f=> f.FruitID);

            modelBuilder.Entity<BlockData>()
                .HasOne(v => v.FruitVariant)
                .WithMany(b => b.BlockData)
                .HasForeignKey(v => v.VarID);

            modelBuilder.Entity<TractorSprayer>()
                .HasMany(t => t.Tractors)
                .WithMany(ts => ts.TractorSprayer);

            modelBuilder.Entity<TractorSprayer>()
                .HasMany(s => s.Sprayers)
                .WithMany(ts => ts.TractorSprayer);

            #endregion
        }
    }

    #region DB Entities
    public class Weather
    {
        [Key]
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public decimal MaxTemp { get; set; }
        public decimal MinTemp { get; set; }
        public decimal RainChance { get; set; }
        public decimal RainAmt { get; set; }
        public decimal WindSpeed { get; set; }
        public decimal GustSpeed { get; set; }
        public int Condition { get; set; }
        public WeatherConditions WeatherConditions { get; set; }
    }

    public class CurrentWeather
    {
        [Key]
        public int Entry { get; set; }
        public decimal Temperature { get; set; }
        public int Humidity { get; set; }
        public decimal Precipitation { get; set; }
        public decimal Rain { get; set; }
        public int Condition { get; set; }
        public int CloudCover { get; set; }
        public decimal WindSpeed { get; set; }
        public int WindDirection { get; set; }
        public decimal WindGusts { get; set; }
        public WeatherConditions WeatherConditions { get; set; }
    }

    public class WeatherConditions
    {
        [Key]
        public int Code { get; set; }
        public string Description { get; set; }
        public ICollection<Weather> Weather { get; set; }
        public ICollection<CurrentWeather> CurrentWeather { get; set; }
    }

    public class FruitType
    {
        [Key]
        public int FruitID { get; set; }
        public string FruitName { get; set; }
        public ICollection<FruitVariant> FruitVariant { get; set; }
        public ICollection<BlockData> BlockData { get; set; }
    }

    public class FruitVariant
    {
        [Key]
        public int VarID { get; set; }
        public int FruitID { get; set; }
        public string VarName { get; set; }
        public FruitType FruitType { get; set; }
        public ICollection<BlockData> BlockData { get; set; }
    }

    public class BlockData
    {
        [Key]
        public int BlockID { get; set; }
        public string BlockName { get; set; }
        public decimal Size { get; set; }
        public int FruitID { get; set; }
        public int VarID { get; set; }
        public FruitType FruitType { get; set; }
        public FruitVariant FruitVariant { get; set; }
    }

    public class BlockDetails
    {
        [Key]
        public int BlockID { get; set; }
        public DateTime PlantingDate { get; set; }
        public int Age { get; set; }
        public decimal PlantSpacing { get; set; }
        public decimal RowSpacing { get; set; }
        public int PlantsHA { get; set; }
    }

    public class Tractors
    {
        [Key]
        public int TractorID { get; set;}
        public string? TractorModel { get; set; }
        public string? TractorMake { get; set; }
        public string? Name { get; set;}
        public string? Registration { get; set; }
        public string? Remarks { get; set; }
        public ICollection<TractorSprayer> TractorSprayer { get; set; }
    }

    public class Sprayers
    {
        [Key]
        public int SprayerID { get; set; }
        public string? SprayerModel { get; set; }
        public string? SprayerMake { get; set; }
        public string? Name { get; set; }
        public string? Remarks { get; set;}
        public int Volume { get; set; }
        public int ReqTractor { get; set; }
        public ICollection<TractorSprayer> TractorSprayer { get; set; }
    }

    public class Trailers
    {
        [Key]
        public int TrailerID { get; set; }
        public string? TrailerModel { get; set; } 
        public string? TrailerMake { get; set; }
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public string? Remarks { get; set; }
        public int ReqTractor { get; set; }
    }

    public class TractorSprayer
    {
        public int TractorID { get; set; }
        public int SprayerID { get; set; }
        public ICollection<Tractors> Tractors { get; set; }
        public ICollection<Sprayers> Sprayers { get; set; }
    }

    public class TractorTrailer
    {
        public int TractorID { get; set; }
        public int TrailerID { get; set; }
    }
    #endregion

    #region Api Entities
    public class WeatherResult
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double GenerationTimeMs { get; set; }
        public int UtcOffsetSeconds { get; set; }
        public string Timezone { get; set; }
        public string TimezoneAbbreviation { get; set; }
        public double Elevation { get; set; }
        public DailyUnits DailyUnits { get; set; }
        public DailyData Daily { get; set; }
        public CurrentWeatherResults Current { get; set; }
    }

    public class DailyUnits
    {
        public string Time { get; set; }
        public string temperature_2m_max { get; set; }
        public string temperature_2m_min { get; set; }
        public string precipitation_sum { get; set; }
        public string precipitation_probability_max { get; set; }
        public string wind_speed_10m_max { get; set; }
        public string wind_gusts_10m_max { get; set; }
    }

    public class DailyData
    {
        public List<string> time { get; set; }
        public List<int> weather_code { get; set; }
        public List<double> temperature_2m_Max { get; set; }
        public List<double> temperature_2m_Min { get; set; }
        public List<double> precipitation_sum { get; set; }
        public List<int> precipitation_probability_max { get; set; }
        public List<double> wind_speed_10m_max { get; set; }
        public List<double> wind_gusts_10m_max { get; set; }
    }

    public class CurrentWeatherResults
    {
        public DateTime time { get; set; }
        public int interval { get; set; }
        public double temperature_2m { get; set; }
        public int relative_humidity_2m { get; set; }
        public double precipitation { get; set; }
        public double rain { get; set; }
        public int weather_code { get; set; }
        public int cloud_cover { get; set; }
        public double wind_speed_10m { get; set; }
        public int wind_direction_10m { get; set; }
        public double wind_gusts_10m { get; set; }
    }
    #endregion


    #region cross use Entities
    public class WeatherGrid
    {
        public int Day { get; set; }
        public string Date { get; set; }
        public decimal MaxTemp { get; set; }
        public decimal MinTemp { get; set; }
        public decimal RainChance { get; set; }
        public decimal RainAmt { get; set; }
        public decimal WindSpeed { get; set; }
        public decimal GustSpeed { get; set; }
        public string Condition { get; set; }
    }

    public class TractInfo
    {
        public int TractorID { get; set; }
        public string Tractor { get; set;}
    }
    #endregion
}
