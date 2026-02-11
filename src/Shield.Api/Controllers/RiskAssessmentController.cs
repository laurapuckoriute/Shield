using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shield.Api.Configuration;
using Shield.Api.Models;
using Shield.Api.Services;

namespace Shield.Api.Controllers;

[ApiController]
[Route("payment")]
public class RiskAssessmentController(
	IPaymentFraudCheckRequestValidator requestValidator,
	IPaymentFraudAssessmentService fraudAssessmentService,
	IOptions<ConfigOptions> configurationValues,
	ILogger<RiskAssessmentController> logger) : ControllerBase
{

	[HttpPost("fraud-check")]
	public async Task<ActionResult<PaymentFraudAssessmentResult>> CheckPaymentFraud([FromBody] PaymentFraudCheckRequest request, CancellationToken cancellationToken)
	{
		if (!Request.Headers.TryGetValue("x-api-key", out var providedApiKey))
		{
			logger.LogWarning("Fraud check request missing API key header.");
			return Unauthorized();
		}

		var expectedApiKey = configurationValues.Value.API_KEY ?? ConfigOptions.DefaultApiKey;
		if (!string.Equals(providedApiKey.ToString(), expectedApiKey, StringComparison.Ordinal))
		{
			logger.LogWarning("Fraud check request supplied invalid API key.");
			return Unauthorized();
		}

		var validationResult = requestValidator.Validate(request);
		if (!validationResult.IsValid)
		{
			logger.LogWarning("Fraud check validation failed: {Error}", validationResult.ErrorMessage);
			return BadRequest(validationResult.ErrorMessage);
		}

		var result = await fraudAssessmentService.Assess(request, cancellationToken);
		logger.LogInformation(
			"Fraud check succeeded for transaction {TransactionId} with risk score {RiskScore}.",
			request.TransactionId,
			result.RiskScore);
		return Ok(result);
	}
}
