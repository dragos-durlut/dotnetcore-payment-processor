using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PaymentProcessor.Models.DTO.Validation
{
    /// <summary>
    /// https://stackoverflow.com/questions/8844747/restrict-datetime-value-with-data-annotations
    /// </summary>
    public class DateNotInPastValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime d = Convert.ToDateTime(value);
            return d >= DateTime.Now; 

        }
    }
}
