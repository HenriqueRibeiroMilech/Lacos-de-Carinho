using Ldc.Communication.Responses;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ldc.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ApiException)
        {
            HandleProjectException(context);
        }
        else
        {   
            ThrowUnkowError(context);
        }
    }
    
    private void HandleProjectException(ExceptionContext context)
    {
        var coreException = (ApiException)context.Exception;
        var errorResponse = new ResponseErrorJson(coreException.GetErrors());
        
        context.HttpContext.Response.StatusCode = coreException.StatusCode;
        context.Result = new ObjectResult(errorResponse);
    }
    
    private void ThrowUnkowError(ExceptionContext context)
    {
        var errorResponse = new ResponseErrorJson(ResourceErrorMessages.UNKNOWN_ERROR);
        
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(errorResponse);
    }
}