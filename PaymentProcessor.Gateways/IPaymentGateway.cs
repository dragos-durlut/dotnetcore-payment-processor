using PaymentProcessor.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentProcessor.Gateways
{
    public interface IPaymentGateway
    {
        PaymentStateDto ProcessPayment(PaymentRequestDto paymentRequest);
    }
}
