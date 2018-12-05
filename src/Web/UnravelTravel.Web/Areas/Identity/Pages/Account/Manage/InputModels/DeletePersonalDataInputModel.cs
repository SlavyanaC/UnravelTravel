﻿namespace UnravelTravel.Web.Areas.Identity.Pages.Account.Manage.InputModels
{
    using System.ComponentModel.DataAnnotations;

    public class DeletePersonalDataInputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
