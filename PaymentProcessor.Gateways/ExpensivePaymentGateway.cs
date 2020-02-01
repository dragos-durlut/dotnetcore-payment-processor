using PaymentProcessor.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentProcessor.Gateways
{
    public class ExpensivePaymentGateway : IExpensivePaymentGateway
    {
        public PaymentStateDto ProcessPayment(PaymentRequestDto paymentRequest)
        {
            return new PaymentStateDto() { PaymentState = PaymentStateEnum.Failed, PaymentStateDate = DateTime.UtcNow };
        }
    }
}
