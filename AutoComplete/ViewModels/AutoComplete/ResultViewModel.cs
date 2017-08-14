using System;
using System.Collections.Generic;
using Application.Core.Services.CityServices;
using System.Linq;
using System.Web;
using Application.Core.Services.CityServices.Dtos;

namespace AutoComplete.ViewModels.AutoComplete
{
    public class ResultViewModel
    {
        public IEnumerable<ResultDto> Suggestions { get; set; }

        public ResultViewModel()
        {
        }

        public ResultViewModel(IEnumerable<ResultDto> suggestions)
        {
            Suggestions = suggestions;
        }
    }
}