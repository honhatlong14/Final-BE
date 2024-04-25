using Common.Constants;
using Service.SingalR;
using Stripe;
using Webapi.Configurations;
using Webapi.Extensions;
using Webapi.Initializer;
using Webapi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure the application's configuration settings
builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
builder.Configuration.AddEnvironmentVariables();
// Map AppSettings section in appsettings.json file value to AppSetting model
builder.Configuration.GetSection("AppSettings").Get<AppSettings>(options => options.BindNonPublicProperties = true);


// Add services to the container.
builder.Services.AddControllers();
builder.Services
    .AddAutoMapper()
    .AddService()
    .AddChatService()
    .AddSwagger()
    .AddCustomCors()
    .BackgroundService()
    .MailSenderService(builder.Configuration)
    .DatabaseService(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddSignalR();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

var app = builder.Build();
StripeConfiguration.ApiKey = "sk_test_51OxKACRpntJeZywLB6y1dKyJ4MAy5DYN8jwYaVoLBYzEBZUd7o6A1UDCUReFI7Vzoz7w3zzqJXycKFJCuVUgchSv00WKQMB1r2";


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Final project v1");
    });
}
// app.UserExceptionMiddleware(app.Environment);
app.UseRouting();

app.UseCors("Policy");

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseMiddleware<JwtMiddleware>();

// custom jwt auth middleware
//app.UseMiddleware<JwtMiddleware>();
app.MapHub<ChatHub>("/chat");
app.MapControllers();
DbInitializer.Initialize(app);

app.Run();