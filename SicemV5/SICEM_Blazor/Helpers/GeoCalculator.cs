using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SICEM_Blazor.Models;

namespace SICEM_Blazor.Helpers {
    public class GeoCalculator {
        
        public static MapMark GetCenterPoint(IEnumerable<MapMark> points)
        {
            if (points == null || !points.Any()) {
                throw new ArgumentException("List of points cannot be null or empty.");
            }
            
            double totalLatitude = 0;
            double totalLongitude = 0;

            foreach (var point in points) {
                totalLatitude += point.Latitude;
                totalLongitude += point.Longitude;
            }

            double centerLatitude = totalLatitude / points.Count();
            double centerLongitude = totalLongitude / points.Count();

            return new MapMark{
                Latitude = centerLatitude,
                Longitude = centerLongitude,
                Zoom = 8.3
            };
        }
    }
    
}
