namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Restaurants;
    using UnravelTravel.Models.ViewModels.Restaurants;
    using UnravelTravel.Services.Data.Contracts;

    public class RestaurantsController : AdministratorController
    {
        private readonly IRestaurantsService restaurantsService;
        private readonly IDestinationsService destinationsService;

        public RestaurantsController(IRestaurantsService restaurantsService, IDestinationsService destinationsService)
        {
            this.restaurantsService = restaurantsService;
            this.destinationsService = destinationsService;
        }

        public IActionResult Create()
        {
            this.ViewData["Destinations"] = this.SelectAllDestinations();
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RestaurantCreateInputModel restaurantCreateInputModel)
        {
            var fileType = restaurantCreateInputModel.Image.ContentType.Split('/')[1];
            if (!this.IsImageTypeValid(fileType))
            {
                return this.View(restaurantCreateInputModel);
            }

            var restaurant = await this.restaurantsService.CreateAsync(restaurantCreateInputModel);
            return this.RedirectToAction("Details", "Restaurants", new { area = "", id = restaurant.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            this.ViewData["Destinations"] = this.SelectAllDestinations();
            var restaurantToEdit = await this.restaurantsService.GetViewModelByIdAsync<RestaurantEditViewModel>(id);
            return this.View(restaurantToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RestaurantEditViewModel restaurantEditView)
        {
            if (restaurantEditView.NewImage != null)
            {
                var fileType = restaurantEditView.NewImage.ContentType.Split('/')[1];
                if (!this.IsImageTypeValid(fileType))
                {
                    return this.View(restaurantEditView);
                }
            }

            await this.restaurantsService.EditAsync(restaurantEditView);
            return this.RedirectToAction("All", "Restaurants", new { area = "" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var restaurantToDelete = await this.restaurantsService.GetViewModelByIdAsync<RestaurantEditViewModel>(id);
            return this.View(restaurantToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RestaurantEditViewModel destinationEditViewModel)
        {
            var id = destinationEditViewModel.Id;
            await this.restaurantsService.DeleteByIdAsync(id);
            return this.RedirectToAction("All", "Restaurants", new { area = "" });
        }

        private IEnumerable<SelectListItem> SelectAllDestinations()
        {
            return this.destinationsService.GetAllDestinationsAsync().GetAwaiter().GetResult()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                });
        }
    }
}
