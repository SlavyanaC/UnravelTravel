namespace UnravelTravel.Services.Data.Common
{
    public class ServicesDataConstants
    {
        public const string JpgFormat = "jpg";

        public const string JpegFormat = "jpeg";

        public const string PngFormat = "png";

        public const string InvalidRestaurantType = "Restaurant type {0} is invalid.";

        public const string InvalidActivityType = "Activity type {0} is invalid.";

        public const string NullReferenceActivityId = "Activity with id {0} not found.";

        public const string NullReferenceUsername = "User with username {0} not found.";

        public const string NullReferenceDestinationId = "Destination with id {0} not found.";

        public const string NullReferenceCountryId = "Country with id {0} not found.";

        public const string NullReferenceRestaurantId = "Restaurant with id {0} not found.";

        public const string ZeroOrNegativeQuantity = "Quantity cannot be negative or zero";

        public const string NullReferenceShoppingCartActivityId = "Shopping cart activity with id {0} not found.";

        public const string NullReferenceGuestShoppingCartActivityId = "Session does not contain shopping cart activity with id {0}.";

        public const string NullReferenceReservationId = "Reservation with id {0} not found.";

        public const string NullReferenceTicketId = "Ticket with id {0} not found.";

        public const string RestaurantReviewAlreadyAdded = "User with username {0} has already reviewed restaurant with id {1} and name {2}";

        public const string ActivityReviewAlreadyAdded = "User with username {0} has already reviewed activity with id {1} and name {2}";

        public const string NullReferenceShoppingCartForUser = "User with id {0} and username {1} does not have a shopping cart.";

        public const string BookingEmailSubject = "Booking confirmation";

        public const string TicketsReceiptEmailHtmlPath = "Views/Emails/tickets-receipt.html";

        public const string TicketsInfoPlaceholder = "@ticketsInfo";

        public const string TicketActivityReceiptHtmlInfo = @"<tr>
                     <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{0}</td>
                     <td align=""left"" width=""50%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{1}</td>
                     <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{2}</td>
                     <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">${3:F2}</td>
                     </tr>";

        public const string TotalReceiptInfoPlaceholder = "@totalInfo";

        public const string TotalReceiptHtmlInfo = @"<tr>
				  <td align=""left"" width=""50%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>Total</strong></td>
				  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
				  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>${0:F2}</strong></td>
                </tr>";

        public const string PaymentMethodPlaceholder = "@paymentMethod";

        public const string OnlinePaymentEmailString = "Payed online";

        public const string CashPaymentEmailString = "Pay when yuo get there";

        public const string ReservationReceiptEmailHtmlPath = "Views/Emails/reservation-receipt.html";

        public const string ReservationInfoPlaceholder = "@reservationInfo";

        public const string ReservationHtmlInfo =
            @"<td align=""left"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-    serif; font-size: 16px; line-height: 24px;"">{0}</td>
              <td align = ""left"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{1}</td>
              <td align = ""left"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{2}</td>
              <td align = ""left"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{3}</td>";

        public const string RestaurantInfoPlaceholder = "@restaurantInfo";

        public const string RestaurantHtmlInfo = @"<p>{0}<br>{1}, {2}</p>";

        public const string EmailRegex = @"[^@]+@[^\.]+\..+";
    }
}
