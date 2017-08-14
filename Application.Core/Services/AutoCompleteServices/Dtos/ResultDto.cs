using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Application.DAL;

namespace Application.Core.Services.CityServices.Dtos
{
    public class ResultDto
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Score { get; set; }

        internal ResultDto(City city)
        {
            Name = string.Format("{0}, {1}, {2}", city.Name, city.ProvStateCode, city.CountryCode);
            Latitude = city.Latitude;
            Longitude = city.Longitude;

        }
    }
}