using Application.Core;
using Application.Core.Services.CityServices;
using Application.Core.Shared.Web;
using AutoComplete.ViewModels.AutoComplete;
using System.Web.Http;

namespace AutoComplete.Controllers
{
    public class SuggestionsController : ApiController
    {
        private readonly IAutoCompleteService _autoCompleteService = CoreContext.Instance.Service<IAutoCompleteService>();

        //// GET api/GetAllCities
        //private ResultViewModel GetAllCities()
        //{
        //    return new ResultViewModel(_autoCompleteService.GetAllCities());
        //}

        public ResultViewModel Get()
        {
                return new ResultViewModel();
        }

        public ResultViewModel Get(string q, double? latitude = null, double? longitude = null)
        {
            if (string.IsNullOrEmpty(q))
                return new ResultViewModel();

            return new ResultViewModel(_autoCompleteService.GetCitySuggestions(WebConstants.MaximumItemReturn, q, latitude, longitude));
        }
    }
}
