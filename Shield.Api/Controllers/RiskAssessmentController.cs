using Microsoft.AspNetCore.Mvc;
using Shield.Api.Models;
using Shield.Api.Services;

namespace Shield.Api.Controllers;

[ApiController]
[Route("payment")]
public class RiskAssessmentController(IPaymentFraudAssessmentService fraudAssessmentService) : ControllerBase
{
	[HttpPost("fraud-check")]
	public ActionResult<PaymentFraudAssessmentResult> CheckPaymentFraud([FromBody] PaymentFraudCheckRequest request)
	{
		var result = fraudAssessmentService.Assess(request);
		return Ok(result);
	}
}
