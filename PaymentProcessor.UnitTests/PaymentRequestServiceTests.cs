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

            _cheapPaymentGateway = new Mock<ICheapPaymentGateway>();
            _expensivePaymentGateway = new Mock<IExpensivePaymentGateway>();
            _mapper = new Mock<IMapper>();
            _paymentRepository = new Mock<IPaymentRepository>();
            _paymentStateRepository = new Mock<IPaymentStateRepository>();

            _paymentRequestService = new PaymentRequestService(_cheapPaymentGateway.Object, _expensivePaymentGateway.Object, _mapper.Object, _paymentRepository.Object, _paymentStateRepository.Object);

            _mapper.Setup(s => s.Map<PaymentRequestDto, Payment>(It.IsAny<PaymentRequestDto>())).Returns((PaymentRequestDto pr) => new Payment() { Amount = pr.Amount, CardHolder = pr.CardHolder, CreditCardNumber = pr.CreditCardNumber, ExpirationDate = pr.ExpirationDate, SecurityCode = pr.SecurityCode });
            _paymentRepository.Setup(s => s.Create(It.IsAny<Payment>())).Returns((Payment paymentEntity) => Task.FromResult(paymentEntity));
            _paymentStateRepository.Setup(s => s.Create(It.IsAny<PaymentState>())).Returns((PaymentState paymentStateEntity) => Task.FromResult(paymentStateEntity));
        }

        [Test, TestCaseSource(typeof(PaymentRequestServiceTestCaseSource),  nameof(PaymentRequestServiceTestCaseSource.Tests))]
        public async Task Test_PaymentRequestService_Pay(PaymentRequestDto paymentRequestDto, PaymentStateDto cheapGatewayResponseDto, int timesCheapGatewayCalled, PaymentStateDto expensiveGatewayResponseDto, int timesExpensiveGatewayCalled, PaymentStateEnum expectedPaymentStateEnum)
        {
            //arrange
            
            _cheapPaymentGateway.Setup(s => s.ProcessPayment(paymentRequestDto)).Returns(cheapGatewayResponseDto);
            _expensivePaymentGateway.Setup(s => s.ProcessPayment(paymentRequestDto)).Returns(expensiveGatewayResponseDto);
            

            //act
            var paymentStateDto = await _paymentRequestService.Pay(paymentRequestDto);
            //assert
            Assert.IsNotNull(paymentStateDto);
            Assert.AreEqual(paymentStateDto.PaymentState, expectedPaymentStateEnum);
            _cheapPaymentGateway.Verify(s => s.ProcessPayment(paymentRequestDto), Times.Exactly(timesCheapGatewayCalled));
            _expensivePaymentGateway.Verify(s => s.ProcessPayment(paymentRequestDto), Times.Exactly(timesExpensiveGatewayCalled));

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
                yield return new TestCaseData(FirstTierPaymentRequestDto, ProcessedPaymentStateDto, 1, ProcessedPaymentStateDto, 0, PaymentStateEnum.Processed)       .SetName("FirstTier_CheapProcessed_ExpensiveProcessed");
                yield return new TestCaseData(FirstTierPaymentRequestDto, FailedPaymentStateDto, 1, ProcessedPaymentStateDto, 0, PaymentStateEnum.Failed)          .SetName("FirstTier_CheapFailed_ExpensiveProcessed");
                yield return new TestCaseData(FirstTierPaymentRequestDto, FailedPaymentStateDto, 1, FailedPaymentStateDto, 0, PaymentStateEnum.Failed)               .SetName("FirstTier_CheapFailed_ExpensiveFailed");

                yield return new TestCaseData(SecondTierPaymentRequestDto, ProcessedPaymentStateDto, 0, ProcessedPaymentStateDto, 1, PaymentStateEnum.Processed)    .SetName("SecondTier_CheapProcessed_ExpensiveProcessed");
                yield return new TestCaseData(SecondTierPaymentRequestDto, FailedPaymentStateDto, 0, ProcessedPaymentStateDto, 1, PaymentStateEnum.Processed)       .SetName("SecondTier_CheapFailed_ExpensiveProcessed");
                yield return new TestCaseData(SecondTierPaymentRequestDto, FailedPaymentStateDto, 1, FailedPaymentStateDto, 1, PaymentStateEnum.Failed)          .SetName("SecondTier_CheapFailed_ExpensiveFailed");

                yield return new TestCaseData(LastTierPaymentRequestDto, ProcessedPaymentStateDto, 0, ProcessedPaymentStateDto, 1, PaymentStateEnum.Processed)      .SetName("LastTier_CheapProcessed_ExpensiveProcessed");
                yield return new TestCaseData(LastTierPaymentRequestDto, FailedPaymentStateDto, 0, ProcessedPaymentStateDto, 1, PaymentStateEnum.Processed)         .SetName("LastTier_CheapFailed_ExpensiveProcessed");
                yield return new TestCaseData(LastTierPaymentRequestDto, FailedPaymentStateDto, 0, FailedPaymentStateDto, 3, PaymentStateEnum.Failed)            .SetName("LastTier_CheapFailed_ExpensiveFailed");
            }  
        }
    }
}