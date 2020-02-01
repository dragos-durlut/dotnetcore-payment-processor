using PaymentProcessor.Models.DTO;
using System.Threading.Tasks;

namespace PaymentProcessor.Services
{
    public interface IPaymentRequestService
    {
        Task<PaymentStateDto> Pay(PaymentRequestDto paymentRequestDto);
    }
}