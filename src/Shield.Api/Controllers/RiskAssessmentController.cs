using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shield.Api.Configuration;
using Shield.Api.Models;
using Shield.Api.Services;

namespace Shield.Api.Controllers;

[ApiController]
[Route("payment")]
public class RiskAssessmentController(
	IPaymentFraudAssessmentService fraudAssessmentService,
	IOptions<ConfigOptions> configurationValues) : ControllerBase
{
	[HttpPost("fraud-check")]
	public async Task<ActionResult<PaymentFraudAssessmentResult>> CheckPaymentFraud([FromBody] PaymentFraudCheckRequest request, CancellationToken cancellationToken)
	{
		if (!Request.Headers.TryGetValue("x-api-key", out var providedApiKey))
		{
			return Unauthorized();
		}

		var expectedApiKey = configurationValues.Value.API_KEY ?? ConfigOptions.DefaultApiKey;
		if (!string.Equals(providedApiKey.ToString(), expectedApiKey, StringComparison.Ordinal))
		{
			return Unauthorized();
		}

		var result = await fraudAssessmentService.Assess(request, cancellationToken);
		return Ok(result);
	}
}
