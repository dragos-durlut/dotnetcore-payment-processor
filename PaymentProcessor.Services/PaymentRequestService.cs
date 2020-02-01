using PaymentProcessor.Gateways;
using PaymentProcessor.Models.DTO;
using System;

namespace PaymentProcessor.Services
{
    public class PaymentRequestService : IPaymentRequestService
    {
        private readonly ICheapPaymentGateway _cheapPaymentGateway;
        private readonly IExpensivePaymentGateway _expensivePaymentGateway;
        public PaymentRequestService(ICheapPaymentGateway cheapPaymentGateway, IExpensivePaymentGateway expensivePaymentGateway)
        {
            _cheapPaymentGateway = cheapPaymentGateway;
            _expensivePaymentGateway = expensivePaymentGateway;
        }
        public PaymentStateDto Pay(PaymentRequestDto paymentRequestDto)
        {
            //map here
            //save to db here
            //send request to various gateway here
            if (paymentRequestDto.Amount <= 20)
            {
                return _cheapPaymentGateway.ProcessPayment(paymentRequestDto);
            }
            else if (paymentRequestDto.Amount > 20 && paymentRequestDto.Amount <= 500)
            {
                int tryCount = 0;
                while(tryCount < 3)
                {
                    try
                    {
                        return _cheapPaymentGateway.ProcessPayment(paymentRequestDto);
                    }
                    catch(Exception ex)
                    {
                        //log error
                    }
                    finally
                    {
                        tryCount++;
                    }
                }
                return _expensivePaymentGateway.ProcessPayment(paymentRequestDto);
            }
            else
            {
                return _expensivePaymentGateway.ProcessPayment(paymentRequestDto);
            }            
        }
    }
}
