using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PaymentProcessor.Models.DTO.Validation
{
    public static class CreditCardInfoValidator
    {
        /// <summary>
        /// <1> Credit card number must be 16 digits; the first 4 digits must be one of these: 1298, 1267, 4512, 4567, 8901, 8933
        /// <2> Security code must be 3 digit numeric(assumed)
        /// <3> Expiration date must be 2 digits for the month, 
        /// <4> 4 digits for the year. 
        /// <5> The month must be 01 - 12. 
        /// <6> The year must begin with "20". 
        /// <7> The date must be greater than the current actual month and year, and
        /// <8> less than 6 years from the current actual month and year.
        /// https://stackoverflow.com/a/32965555/249895
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="expiryDate"></param>
        /// <param name="cvv"></param>
        /// <returns></returns>
        public static bool IsCreditCardInfoValid(string cardNo, string expiryDate, string cvv)
        {
            var cardCheck = new Regex(@"^(1298|1267|4512|4567|8901|8933)([\-\s]?[0-9]{4}){3}$");
            var monthCheck = new Regex(@"^(0[1-9]|1[0-2])$");
            var yearCheck = new Regex(@"^20[0-9]{2}$");
            var cvvCheck = new Regex(@"^\d{3}$");

            if (!cardCheck.IsMatch(cardNo)) // <1>check card number is valid
                return false;
            if (!cvvCheck.IsMatch(cvv)) // <2>check cvv is valid as "999"
                return false;

            var dateParts = expiryDate.Split('/'); //expiry date in from MM/yyyy            
            if (!monthCheck.IsMatch(dateParts[0]) || !yearCheck.IsMatch(dateParts[1])) // <3 - 6>
                return false; // ^ check date format is valid as "MM/yyyy"

            var year = int.Parse(dateParts[1]);
            var month = int.Parse(dateParts[0]);
            var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month); //get actual expiry date
            var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);

            //check expiry greater than today & within next 6 years <7, 8>>
            return (cardExpiry > DateTime.Now && cardExpiry < DateTime.Now.AddYears(6));
        }
    }
}
