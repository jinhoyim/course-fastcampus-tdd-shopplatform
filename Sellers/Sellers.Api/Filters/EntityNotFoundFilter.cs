using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sellers.CommandModel;

namespace Sellers.Filters;

public class EntityNotFoundFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is EntityNotFoundException)
        {
            context.Result = new NotFoundResult();
        }
    }
}