namespace UnravelTravel.Models.InputModels
{
    internal class InputModelsConstants
    {
        internal const string ReviewContentError = "Content must be between {2} and {1} symbols";

        internal class Reservation
        {
            internal const string HourError = "Reservation time must be at least 30 minutes from now";
            internal const string DateDisplay = "Reservation date and time";
            internal const string PeopleCountDisplay = "Table for";
        }
    }
}
