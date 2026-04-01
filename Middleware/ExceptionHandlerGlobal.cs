using Microsoft.AspNetCore.Diagnostics;

namespace FinanceAPI.Middleware
{
    public static class ExceptionHandlerGlobal
    {
        public static void UseGlobalExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    if (exceptionHandlerPathFeature?.Error != null)
                    {
                        if (app.Environment.IsDevelopment())
                        {
                            await context.Response.WriteAsJsonAsync(new
                            {
                                Message = "An unexpected error occurred.",
                                Details = exceptionHandlerPathFeature.Error.Message,
                                StackTrace = exceptionHandlerPathFeature.Error.StackTrace
                            });
                        }
                        else
                        {
                            await context.Response.WriteAsJsonAsync(new
                            {
                                Message = "An unexpected error occurred. Please try again later."
                            });
                        }
                    }
                });
            });
        }

    }
}