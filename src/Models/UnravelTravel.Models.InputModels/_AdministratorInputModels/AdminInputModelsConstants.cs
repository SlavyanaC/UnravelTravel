// ReSharper disable once CheckNamespace
namespace UnravelTravel.Models.InputModels.AdministratorInputModels
{
    internal class AdminInputModelsConstants
    {
        internal const string NameError = "Name must be between {2} and {1} symbols";
        internal const string AddressError = "Address must be between {2} and {1} symbols";
        internal const string NameRegex = "^[A-Z]\\D+[a-z]$";
        internal const string NameRegexError = "Name must start with upper case and end with lower case";

        internal class Activity
        {
            internal const string StartingHourError = "Activity starting hour must be at least 15 minutes from now";
            internal const string DateDisplayName = "Activity date and starting hour";
        }

        internal class Destination
        {
            internal const string InformationError = "Information must be between {2} and {1} symbols";
        }
    }
}
