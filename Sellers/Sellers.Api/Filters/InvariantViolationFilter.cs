using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sellers.CommandModel;

namespace Sellers.Filters;

public class InvariantViolationFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is InvariantViolationException)
        {
            context.Result = new BadRequestResult();
        }
    }
}