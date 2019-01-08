namespace UnravelTravel.Common.Extensions
{
    using System;

    using GoogleMaps.LocationServices;
    using RestSharp;

    public static class DateTimeExtensions
    {
        public static DateTime GetUtcDate(this DateTime localDateTime, string cityName, string countryName)
        {
            var address = $"{cityName}, {countryName}";
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

            var utcDate = localDateTime.Subtract(new TimeSpan(0, 0, (int)rawOffsetInSeconds));
            return utcDate;
        }

        public static DateTime GetLocalDate(this DateTime date, string cityName, string countryName)
        {
            var address = $"{cityName}, {countryName}";
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

            var localDate = date.Add(new TimeSpan(0, 0, (int)rawOffsetInSeconds));
            return localDate;
        }
    }
}
