namespace UnravelTravel.Web.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public sealed class DateAfter : ValidationAttribute
    {
        private readonly string startDate;

        public DateAfter(string startDate)
        {
            this.startDate = startDate;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is DateTime))
            {
                return new ValidationResult("Invalid DateTime object.");
            }

            var currentValue = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(this.startDate);
            if (property == null)
            {
                throw new ArgumentException("Property with this name not found");
            }

            var startDateValue = (DateTime)property.GetValue(validationContext.ObjectInstance);
            if (currentValue < startDateValue)
            {
                return new ValidationResult(this.ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
