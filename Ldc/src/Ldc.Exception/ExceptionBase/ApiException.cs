namespace Ldc.Exception.ExceptionBase;

public abstract class ApiException : SystemException
{
    protected ApiException(string message) : base(message)
    {

    }

    public abstract int StatusCode { get; }
    public abstract List<string> GetErrors();
}