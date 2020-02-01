using NUnit.Framework;
using PaymentProcessor.Services;
using Moq;
using PaymentProcessor.Gateways;
using AutoMapper;
using PaymentProcessor.Models.Entity.Repository;
using PaymentProcessor.Models.DTO;
using PaymentProcessor.Models.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace PaymentProcessor.UnitTests
{
    public class PaymentRequestServiceTests
    {
        IPaymentRequestService _paymentRequestService;

        Mock<ICheapPaymentGateway> _cheapPaymentGateway;
        Mock<IExpensivePaymentGateway> _expensivePaymentGateway;
        Mock<IMapper> _mapper;
        Mock<IPaymentRepository> _paymentRepository;
        Mock<IPaymentStateRepository> _paymentStateRepository;
        [SetUp]
        public void Setup()
        {
            //_cheapPaymentGateway.Setup(s=>s.ProcessPayment(It.IsAny<PaymentRequestDto>()))
            _mapper = new Mock<IMapper>();
            _paymentRepository = new Mock<IPaymentRepository>();
            _paymentStateRepository = new Mock<IPaymentStateRepository>();
            
        }

        [Test, TestCaseSource(typeof(PaymentRequestServiceTestCaseSource),  nameof(PaymentRequestServiceTestCaseSource.Tests))]
        public async Task Test_PaymentRequestService_Pay(PaymentRequestDto paymentRequestDto, PaymentStateDto cheapGatewayResponseDto, PaymentStateDto expensiveGatewayResponseDto)
        {
            //arrange
            _mapper.Setup(s => s.Map<PaymentRequestDto, Payment>(It.IsAny<PaymentRequestDto>())).Returns((PaymentRequestDto paymentRequestDto) => { return new Payment(); });
            _cheapPaymentGateway.Setup(s => s.ProcessPayment(paymentRequestDto)).Returns(cheapGatewayResponseDto);
            _expensivePaymentGateway.Setup(s => s.ProcessPayment(paymentRequestDto)).Returns(expensiveGatewayResponseDto);
            _paymentRequestService = new PaymentRequestService(_cheapPaymentGateway.Object, _expensivePaymentGateway.Object, _mapper.Object, _paymentRepository.Object, _paymentStateRepository.Object);
            //act
            var paymentStateDto = await _paymentRequestService.Pay(paymentRequestDto);
            //assert
            Assert.IsNotNull(paymentStateDto);

        }
    }

    public static class PaymentRequestServiceTestCaseSource
    {
        public static PaymentStateDto FailedPaymentStateDto { get { return new PaymentStateDto() { PaymentState = PaymentStateEnum.Failed, PaymentStateDate = DateTime.Now }; } }
        public static PaymentStateDto ProcessedPaymentStateDto { get { return new PaymentStateDto() { PaymentState = PaymentStateEnum.Processed, PaymentStateDate = DateTime.Now }; } }

        public static PaymentRequestDto FirstTierPaymentRequestDto { get { return new PaymentRequestDto() { Amount = 19, CardHolder = "Jon Smith", CreditCardNumber = "5402 6326 4830 4155", ExpirationDate = DateTime.Now.AddYears(2), SecurityCode = "123" }; }  }
        public static PaymentRequestDto SecondTierPaymentRequestDto { get { return new PaymentRequestDto() { Amount = 21, CardHolder = "Jon Smith", CreditCardNumber = "5402 6326 4830 4155", ExpirationDate = DateTime.Now.AddYears(2), SecurityCode = "123" }; } }
        public static PaymentRequestDto LastTierPaymentRequestDto { get { return new PaymentRequestDto() { Amount = 501, CardHolder = "Jon Smith", CreditCardNumber = "5402 6326 4830 4155", ExpirationDate = DateTime.Now.AddYears(2), SecurityCode = "123" }; } }

        public static IEnumerable<TestCaseData> Tests 
        { 
            get 
            {
                yield return new TestCaseData(FirstTierPaymentRequestDto, ProcessedPaymentStateDto, ProcessedPaymentStateDto)       .SetName("FirstTier_CheapProcessed_ExpensiveProcessed");
                yield return new TestCaseData(FirstTierPaymentRequestDto, FailedPaymentStateDto, ProcessedPaymentStateDto)          .SetName("FirstTier_CheapFailed_ExpensiveProcessed");
                yield return new TestCaseData(FirstTierPaymentRequestDto, FailedPaymentStateDto, FailedPaymentStateDto)             .SetName("FirstTier_CheapFailed_ExpensiveFailed");

                yield return new TestCaseData(SecondTierPaymentRequestDto, ProcessedPaymentStateDto, ProcessedPaymentStateDto)  .SetName("SecondTier_CheapProcessed_ExpensiveProcessed");
                yield return new TestCaseData(SecondTierPaymentRequestDto, FailedPaymentStateDto, ProcessedPaymentStateDto)     .SetName("SecondTier_CheapFailed_ExpensiveProcessed");
                yield return new TestCaseData(SecondTierPaymentRequestDto, FailedPaymentStateDto, FailedPaymentStateDto)        .SetName("SecondTier_CheapFailed_ExpensiveFailed");

                yield return new TestCaseData(LastTierPaymentRequestDto, ProcessedPaymentStateDto, ProcessedPaymentStateDto)      .SetName("LastTier_CheapProcessed_ExpensiveProcessed");
                yield return new TestCaseData(LastTierPaymentRequestDto, FailedPaymentStateDto, ProcessedPaymentStateDto)         .SetName("LastTier_CheapFailed_ExpensiveProcessed");
                yield return new TestCaseData(LastTierPaymentRequestDto, FailedPaymentStateDto, FailedPaymentStateDto).SetName("LastTier_CheapFailed_ExpensiveFailed");
            }  
        }
    }
}