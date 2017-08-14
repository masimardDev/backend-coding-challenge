using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Application.Core.Services.CityServices.Dtos;
using Application.Core.Shared.Services;
using Application.DAL;
using Application.Core.Utils;
using Application.Core.Shared.Web;

namespace Application.Core.Services.CityServices
{
    public class CityService : ServiceBase, IAutoCompleteService
    {

        internal IQueryable<City> GetCityBase(string searchString = null)
        {
            IQueryable<City> cities = Entities.Cities;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                cities = cities.Where(b => b.Name.Contains(searchString));
            }

            return cities.OrderBy(e => e.Name);
        }

        public IEnumerable<ResultDto> GetCitySuggestions(int take, string searchTerm, double? searchLatitude, double? searchLongitude)
        {

            IEnumerable<ResultDto> results = GetCityBase(searchTerm).AsEnumerable()
                    .Select(e => new ResultDto(e)
                            {
                                Score = CalculateSimilarity(searchTerm, e.Name, searchLatitude, searchLongitude, e.Latitude, e.Longitude)
                            }
                    ).ToList();


            return results.OrderByDescending(o => o.Score).Take((int)take);
        }

        public IEnumerable<ResultDto> GetAllCities()
        {
            return
                GetCityBase()
                    .AsEnumerable()
                    .Select(e => new ResultDto(e));
        }

        double CalculateSimilarity(string source, string target, double? searchLatitude, double? searchLongitude, double? latitude, double? longitude)
        {

            if (string.IsNullOrEmpty(source)) 
                return string.IsNullOrEmpty(target) ? 1 : 0;

            if (string.IsNullOrEmpty(target))
                return string.IsNullOrEmpty(source) ? 1 : 0;

            int levenshteinDistance = StringDistance.LevenshteinDistance(source.ToLower(), target.ToLower());
            double textScore = (1.0 - (levenshteinDistance / (double)Math.Max(source.Length, target.Length)));


            if (searchLatitude != null && searchLongitude != null && latitude != null && longitude != null)
            {
                var distance = GeolocalizationDistance.GetBirdDistanceInKms((double)searchLatitude, (double)searchLongitude, (double)latitude, (double)longitude);
                double distanceScore = 1 - (distance / WebConstants.DistanceMaximumInKm);
                return (textScore + distanceScore) / 2;
            }
            else
            {
                return textScore;
            }
        }

    }
}
