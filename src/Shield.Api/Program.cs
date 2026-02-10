using System.Text.Json.Serialization;
using Shield.Api.Services;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();
builder.Services.AddScoped<IPaymentFraudAssessmentService, PaymentFraudAssessmentService>();
builder.Services.AddSingleton<IHumanReviewService, HumanReviewService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();

builder.Host.UseWolverine();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
