using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentProcessor.Models.DTO
{
    public class PaymentStateDto
    {
        public PaymentStateEnum PaymentState { get; set; }
        public DateTime PaymentStateDate { get; set; }
    }
}
