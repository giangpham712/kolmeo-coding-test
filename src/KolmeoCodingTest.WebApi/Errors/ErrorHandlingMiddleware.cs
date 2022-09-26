

using System.Net;
using System.Text.Json;
using KolmeoCodingTest.Application.Errors;
using KolmeoCodingTest.Core.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace KolmeoCodingTest.WebApi.Errors;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            ProblemDetails responseModel;

            switch (error)
            {
                
                case DomainException e:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel = new ProblemDetails()
                    {
                        Title = "Validation error occurred.",
                        Detail = e.Message
                    };
                    
                    break;
                

                case EntityNotFoundException e:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    responseModel = new ProblemDetails()
                    {
                        Title = e.Message
                    };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel = new ProblemDetails();
                    break;
            }
            
            var result = JsonSerializer.Serialize(responseModel, responseModel.GetType());
            await response.WriteAsync(result);
        }
    }
}