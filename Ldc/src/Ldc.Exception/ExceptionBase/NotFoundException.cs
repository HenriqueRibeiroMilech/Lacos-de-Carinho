using System.Net;

namespace Ldc.Exception.ExceptionBase;

public class NotFoundException : ApiException
{
    public NotFoundException(string message) : base(message)
    {
        
    }
    
    public override int StatusCode => (int)HttpStatusCode.NotFound;
    
    public override List<string> GetErrors()
    {
        return [Message];
    }
}