using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core.Utils
{
    public static class GeolocalizationDistance
    {
        const double PIx = 3.141592653589793;
        const double RADIUS = 6378.16;

        private static double Radians(double x)
        {
            return x * PIx / 180;
        }

        public static double GetBirdDistanceInKms(double startLatitude, double startLongitude, double latitude, double longitude)
        {
            double dlon = Radians(longitude - startLongitude);
            double dlat = Radians(latitude - startLatitude);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(Radians(startLatitude)) * Math.Cos(Radians(latitude)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return (double)Math.Round(Convert.ToDecimal(angle * RADIUS), 2);
        }
    }
}
