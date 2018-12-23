// ReSharper disable once CheckNamespace
namespace UnravelTravel.Models.InputModels.AdministratorInputModels
{
    internal class AdminInputModelsConstants
    {
        public const string NameError = "Name must be between {2} and {1} symbols";
        public const string AddressError = "Address must be between {2} and {1} symbols";
        public const string NameRegex = "^[A-Z]\\D+[a-z]$";
        public const string NameRegexError = "Name must start with upper case and end with lower case";

        internal class Activity
        {
            public const string StartingHourError = "Activity starting hour must be at least 15 minutes from now";
            public const string DateDisplayName = "Activity date and starting hour";
        }

        internal class Destination
        {
            public const string InformationError = "Information must be between {2} and {1} symbols";
        }
    }
}
