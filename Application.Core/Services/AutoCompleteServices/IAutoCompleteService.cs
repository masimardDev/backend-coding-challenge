using System.Collections.Generic;
using Application.Core.Services.CityServices.Dtos;
using Application.Core.Shared.Services;

namespace Application.Core.Services.CityServices
{
    public interface IAutoCompleteService : IServiceBase 
    {
        IEnumerable<ResultDto> GetCitySuggestions(int take, string searchTerm, double? latitude, double? longitude);
        IEnumerable<ResultDto> GetAllCities();
    }
}
