using System;
using System.ComponentModel.DataAnnotations;

namespace CCWFM.Helpers.InputValidators
{
    public class NotEqualNumberAttribute : ValidationAttribute
    {
        decimal notEqualVaule = 0;
        public NotEqualNumberAttribute(long value)
        {
            notEqualVaule = value;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //return base.IsValid(value, validationContext);
            var dt = (decimal)value;
           
            if (dt != notEqualVaule)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(string.Format("Value Cannot be equal to {0}.", notEqualVaule));
        }
    }
}
