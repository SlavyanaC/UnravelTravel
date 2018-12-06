namespace UnravelTravel.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using UnravelTravel.Services.Data.Models.Restaurants;

    public interface IRestaurantsService
    {
        Task<AllRestaurantsViewModel> GetAllRestaurantsAsync();
    }
}
