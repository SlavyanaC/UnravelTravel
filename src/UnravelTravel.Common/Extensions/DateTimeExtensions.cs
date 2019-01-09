namespace UnravelTravel.Common.Extensions
{
    using System;

    using GoogleMaps.LocationServices;
    using RestSharp;

    public static class DateTimeExtensions
    {
        public static GoogleServiceInfo GetGoogleServiceInfo(string destinationName, string countryName)
        {
            var address = $"{destinationName}, {countryName}";
            var locationService = new GoogleLocationService(apikey: GoogleUtilitiess.ApiKey);
            var point = locationService.GetLatLongFromAddress(address);
            var latitude = point.Latitude;
            var longitude = point.Longitude;

            var client = new RestClient(GoogleUtilitiess.BaseUrl);
            var requestTime = new RestRequest(GoogleUtilitiess.TimeZoneResource, Method.GET);
            requestTime.AddParameter("location", $"{latitude},{longitude}");
            requestTime.AddParameter("timestamp", GoogleUtilitiess.TimeStamp);
            requestTime.AddParameter("key", GoogleUtilitiess.ApiKey);
            var responseTime = client.Execute<GoogleTimeZone>(requestTime);
            var rawOffsetInSeconds = responseTime.Data.RawOffset;

            var googleServiceInfo = new GoogleServiceInfo
            {
                Latitude = latitude,
                Longitude = longitude,
                UtcRawOffset = rawOffsetInSeconds,
            };

            return googleServiceInfo;
        }

        public static DateTime CalculateUtcDateTime(this DateTime localDateTime, double destinationRawOffset)
        {
            var utcDate = localDateTime.Subtract(new TimeSpan(0, 0, (int)destinationRawOffset));
            return utcDate;
        }

        public static DateTime CalculateLocalDate(this DateTime utcDateTime, double destinationRawOffset)
        {
            var localDate = utcDateTime.Add(new TimeSpan(0, 0, (int)destinationRawOffset));
            return localDate;
        }
    }
}
