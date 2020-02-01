using PaymentProcessor.Models.DTO;
using System;

namespace PaymentProcessor.Gateways
{
    public class CheapPaymentGateway : ICheapPaymentGateway
    {
        public PaymentStateDto ProcessPayment(PaymentRequestDto paymentRequest)
        {
            Random rnd = new Random();
            var num = rnd.Next(1,12);
            if (num % 4 == 0 || num % 6 == 0)
                throw new Exception("Call failed");
            if(num % 2 == 0)
                return new PaymentStateDto() { PaymentState = PaymentStateEnum.Failed, PaymentStateDate = DateTime.UtcNow };
            else return new PaymentStateDto() { PaymentState = PaymentStateEnum.Processed, PaymentStateDate = DateTime.UtcNow };
        }
    }
}
