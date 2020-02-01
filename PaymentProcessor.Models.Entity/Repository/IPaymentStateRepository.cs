using System.Threading.Tasks;

namespace PaymentProcessor.Models.Entity.Repository
{
    public interface IPaymentStateRepository
    {
        Task<PaymentState> GetById(long id);
    }
}