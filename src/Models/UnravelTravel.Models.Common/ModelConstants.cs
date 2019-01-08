namespace UnravelTravel.Models.Common
{
    public class ModelConstants
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 6;
        public const int AddressMinLength = 3;
        public const int AddressMaxLength = 50;
        public const int DestinationActivitiesToDisplay = 6;
        public const int DestinationRestaurantsToDisplay = 6;
        public const int ShoppingCartActivityNameLength = 25;

        public const string NameLengthError = "Name must be between {2} and {1} symbols";
        public const string AddressLengthError = "Address must be between {2} and {1} symbols";
        public const string NameRegex = "^[A-Z]\\D+[a-z]$";
        public const string NameRegexError = "Name must start with upper case and end with lower case";
        public const string ImageUrlDisplay = "Current image";
        public const string NewImageDisplay = "New Image";
        public const string UserFullNameDisplay = "User full name";
        public const string TicketIdDisplay = "#";
        public const string ShoppingCartActivityNameCut = "...";

        public class Activity
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 70;
            public const int DescriptionMaxLength = 1950;
            public const int DescriptionMinLength = 10;

            public const string AdminDateDisplay = "Activity date and starting hour";
            public const string NameDisplay = "Activity name";
            public const string DateDisplay = "Activity date";
            public const string StartingHourDisplay = "Activity starting hour";
            public const string AdditionalInfoDisplay = "Know before you go";
            public const string DescriptionLengthError = "Description must be between {2} and {1} symbols";
        }

        public class Destination
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;

            public const int InformationMinLength = 10;
            public const int InformationMaxLength = 850;

            public const string InformationError = "Information must be between {2} and {1} symbols";
        }

        public class Restaurant
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;

            public const string NameDisplay = "Restaurant name";
        }

        public class Reservation
        {
            public const string IdDisplay = "#";
            public const string DateDisplay = "Reservation date and time";
            public const string PeopleCountDisplay = "Table for";
            public const string ReservationDay = "Reservation day";
            public const string ReservationHour = "Reservation hour";
        }

        public class Search
        {
            public const string StartDateDisplay = "From";
            public const string EndDateDisplay = "To";
            public const string EndDateError = "End date cannot be before start date";
        }

        public class Review
        {
            public const int RatingMin = 1;
            public const int RatingMax = 5;
            public const int ContentMinLength = 5;
            public const int ContentMaxLength = 400;

            public const string ContentError = "Content must be between {2} and {1} symbols";
        }
    }
}
