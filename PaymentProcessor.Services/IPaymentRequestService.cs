using PaymentProcessor.Models.DTO;

namespace PaymentProcessor.Services
{
    public interface IPaymentRequestService
    {
        PaymentStateDto Pay(PaymentRequestDto paymentRequestDto);
    }
}