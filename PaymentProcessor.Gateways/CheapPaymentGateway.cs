using PaymentProcessor.Models.DTO;
using System;

namespace PaymentProcessor.Gateways
{
    public class CheapPaymentGateway : ICheapPaymentGateway
    {
        public PaymentStateDto ProcessPayment(PaymentRequestDto paymentRequest)
        {
            return new PaymentStateDto() { PaymentState = PaymentStateEnum.Failed, PaymentStateDate = DateTime.UtcNow };
        }
    }
}
