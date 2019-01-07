namespace UnravelTravel.Models.ViewModels.Activities
{
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using GoogleMaps.LocationServices;
    using RestSharp;
    using UnravelTravel.Common;
    using UnravelTravel.Common.Extensions;
    using UnravelTravel.Models.ViewModels.Reviews;

    public class ActivityDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Type { get; set; }

        public string SplitWordsType => this.Type.SplitWords();

        public DateTime Date { get; set; }

        public DateTime LocalDate => this.GetLocalDate(this.DestinationName, this.DestinationCountryName, this.Date);

        public string DateString => this.LocalDate.ToString(GlobalConstants.DateFormat + " " + GlobalConstants.HourFormat,
            CultureInfo.InvariantCulture);

        public bool IsPassed => this.Date < DateTime.Now;

        public string Description { get; set; }

        public string AdditionalInfo { get; set; }

        public int DestinationId { get; set; }

        public string DestinationName { get; set; }

        public string DestinationCountryName { get; set; }

        public string Address { get; set; }

        public string LocationName { get; set; }

        public decimal Price { get; set; }

        public IEnumerable<ReviewViewModel> Reviews { get; set; }

        private DateTime GetLocalDate(string destinationName, string countryName, DateTime date)
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

            var utcDate = date.Add(new TimeSpan(0, 0, (int)rawOffsetInSeconds));
            return utcDate;
        }
    }
}