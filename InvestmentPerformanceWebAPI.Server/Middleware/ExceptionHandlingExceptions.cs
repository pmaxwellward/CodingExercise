// Middleware/ExceptionHandlingExtensions.cs
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentPerformanceWebAPI.Server.Middleware {
    public static class ExceptionHandlingExtensions {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env, ILogger logger) {
            app.UseExceptionHandler(appError => {
                appError.Run(async context => {
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var ex = feature?.Error;

                    logger.LogError(ex, "Unhandled exception at {Path}", feature?.Path);

                    var problem = new ProblemDetails {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Title = "An unexpected error occurred.",
                        Detail = env.IsDevelopment() ? ex?.Message : "Please contact support."
                    };

                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = problem.Status ?? 500;
                    await context.Response.WriteAsJsonAsync(problem);
                });
            });
        }
    }
}
