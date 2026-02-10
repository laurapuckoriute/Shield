using Microsoft.AspNetCore.Mvc;
using Shield.Api.Models;
using Shield.Api.Services;

namespace Shield.Api.Controllers;

[ApiController]
[Route("payment")]
public class RiskAssessmentController(IPaymentFraudAssessmentService fraudAssessmentService) : ControllerBase
{
	[HttpPost("fraud-check")]
	public async Task<ActionResult<PaymentFraudAssessmentResult>> CheckPaymentFraud([FromBody] PaymentFraudCheckRequest request, CancellationToken cancellationToken)
	{
		var result = await fraudAssessmentService.Assess(request, cancellationToken);
		return Ok(result);
	}
}
