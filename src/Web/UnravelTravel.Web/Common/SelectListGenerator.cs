namespace UnravelTravel.Web.Common
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Services.Data.Contracts;

    public static class SelectListGenerator
    {
        public static IEnumerable<SelectListItem> GetAllDestinations(IDestinationsService destinationsService)
        {
            var destinations = destinationsService.GetAllDestinationsAsync().GetAwaiter().GetResult();
            var groups = new List<SelectListGroup>();
            foreach (var destinationViewModel in destinations)
            {
                if (groups.All(g => g.Name != destinationViewModel.CountryName))
                {
                    groups.Add(new SelectListGroup { Name = destinationViewModel.CountryName });
                }
            }

            return destinations.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
                Group = groups.FirstOrDefault(g => g.Name == x.CountryName),
            }).OrderBy(x => x.Group.Name);
        }

        public static IEnumerable<SelectListItem> GetAllCountries(ICountriesService countriesService)
        {
            var countries = countriesService.GetAllAsync().GetAwaiter().GetResult();
            var groups = new List<SelectListGroup>();
            foreach (var country in countries)
            {
                if (groups.All(g => g.Name != country.Name.Substring(0, 1)))
                {
                    groups.Add(new SelectListGroup { Name = country.Name.Substring(0, 1) });
                }
            }

            return countries.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
                Group = groups.FirstOrDefault(g => g.Name == x.Name.Substring(0, 1)),
            }).OrderBy(x => x.Group.Name);
        }
    }
}
