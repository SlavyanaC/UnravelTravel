namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

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
        public IActionResult Create(RestaurantCreateInputModel restaurantCreateInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(restaurantCreateInputModel);
            }

            var fileType = restaurantCreateInputModel.Image.ContentType.Split('/')[1];
            if (!this.IsImageTypeValid(fileType))
            {
                return this.View(restaurantCreateInputModel);
            }

            var restaurantId = this.restaurantsService.CreateAsync(restaurantCreateInputModel).GetAwaiter().GetResult();

            return this.RedirectToAction("Details", "Restaurants", new { area = "", id = restaurantId });
        }

        public IActionResult Edit(int id)
        {
            this.ViewData["Destinations"] = this.SelectAllDestinations();

            var restaurantToEdit = this.restaurantsService.GetViewModelByIdAsync<RestaurantEditViewModel>(id)
                .GetAwaiter()
                .GetResult();
            if (restaurantToEdit == null)
            {
                return this.Redirect("/");
            }

            return this.View(restaurantToEdit);
        }

        [HttpPost]
        public IActionResult Edit(RestaurantEditViewModel restaurantEditView)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(restaurantEditView);
            }

            if (restaurantEditView.NewImage != null)
            {
                var fileType = restaurantEditView.NewImage.ContentType.Split('/')[1];
                if (!this.IsImageTypeValid(fileType))
                {
                    return this.View(restaurantEditView);
                }
            }

            this.restaurantsService.EditAsync(restaurantEditView).GetAwaiter().GetResult();

            return this.RedirectToAction("All", "Restaurants", new { area = "" });
        }

        public IActionResult Delete(int id)
        {
            var restaurantToDelete = this.restaurantsService.GetViewModelByIdAsync<RestaurantEditViewModel>(id)
                .GetAwaiter()
                .GetResult();
            if (restaurantToDelete == null)
            {
                return this.Redirect("/");
            }

            return this.View(restaurantToDelete);
        }

        [HttpPost]
        public IActionResult Delete(RestaurantEditViewModel destinationEditViewModel)
        {
            var id = destinationEditViewModel.Id;
            this.restaurantsService.DeleteByIdAsync(id).GetAwaiter().GetResult();
            return this.RedirectToAction("All", "Restaurants", new { area = "" });
        }

        private IEnumerable<SelectListItem> SelectAllDestinations()
        {
            return this.destinationsService.GetAllDestinationsAsync()
                .GetAwaiter()
                .GetResult()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                });
        }
    }
}
