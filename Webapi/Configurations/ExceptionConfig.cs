using System.Net;
using Common.Exception;
using Common.Utility;
using Data.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Webapi.Configurations;

  public static class ExceptionConfig
    {
        public static void UserExceptionMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseExceptionHandler(new ExceptionHandlerOptions
			{
				ExceptionHandler = async context =>
				{
					var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
					if (ex == null)
						return;

					var error = new AppErrorModel(ex.Message, ex.StackTrace);
					
					switch (ex)
					{
						case AppException appException when !string.IsNullOrEmpty(appException.Message):
							context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
							error = new AppErrorModel(appException.GetMessage(), appException.ErrorCode);
							break;
						
						// ReSharper disable once PatternAlwaysOfType
						case Exception exception when !string.IsNullOrEmpty(exception.Message):
							context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
							error.Message = exception.Message;

							break;
					}

					// Return as json
					context.Response.ContentType = "application/json";
					await using var writer = new StreamWriter(context.Response.Body);
					await writer.WriteAsync(JsonConvert.SerializeObject(error, new JsonSerializerSettings
					{
						ContractResolver = new CamelCasePropertyNamesContractResolver(),
						ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
						DateTimeZoneHandling = DateTimeZoneHandling.Local,
					}));

					await writer.FlushAsync();
				}
			});
		}
    }