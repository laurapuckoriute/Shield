using System.Text.Json.Serialization;
using Shield.Api.Configuration;
using Shield.Api.Services;
using Shield.Api.Services.Downstream;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddScoped<IPaymentFraudAssessmentService, PaymentFraudAssessmentService>();
builder.Services.AddSingleton<IHumanReviewService, HumanReviewService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddSingleton<IPaymentFraudCheckRequestValidator, PaymentFraudCheckRequestValidator>();
builder.Services
    .AddOptions<ConfigOptions>()
    .Bind(builder.Configuration.GetSection(ConfigOptions.SectionName))
    .PostConfigure(options =>
    {
        if (string.IsNullOrWhiteSpace(options.API_KEY))
        {
            options.API_KEY = ConfigOptions.DefaultApiKey;
        }
    });

builder.Host.UseWolverine();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
