using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentProcessor.Models.DTO;
using PaymentProcessor.Services;

namespace PaymentProcessor.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {
        private readonly ILogger<PayController> _logger;
        private readonly IPaymentRequestService _paymentRequestService;

        public PayController(IPaymentRequestService paymentRequestService, ILogger<PayController> logger)
        {
            _logger = logger;
            _paymentRequestService = paymentRequestService;
        }

        [HttpGet]
        public string Get()
        {
            return "Payment Processor is online";
        }


        [HttpPost]        
        public async Task<IActionResult> Post(PaymentRequestDto paymentRequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var paymentState = await _paymentRequestService.Pay(paymentRequest);
                    var paymentResponse = new PaymentResponseDto()
                    {
                        IsProcessed = paymentState.PaymentState == PaymentStateEnum.Processed
                        ,
                        PaymentState = paymentState
                    };

                    if (!paymentResponse.IsProcessed)
                        return StatusCode(500, new { error = "Payment could not be processed" });
                    return Ok(paymentResponse);
                }
                else
                    return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }
    }
}