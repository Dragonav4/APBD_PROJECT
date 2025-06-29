using System.Net;
using System.Text.Json;

namespace APBD_PROJECT.Exceptions;

public class BadRequestException : Exception
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public BadRequestException(string message, HttpStatusCode badRequest) : base(message) { }

    public override string ToString()
    {
        var payload = new
        {
            ExceptionType = GetType().Name,
            Message = Message,
            StatusCode = StatusCode
        };
        return JsonSerializer.Serialize(payload);
    }
}

[Serializable]
public class InternalServerErrorException : Exception
{
    public HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
    public InternalServerErrorException(string message) : base(message) { }

    public override string ToString()
    {
        var payload = new
        {
            ExceptionType = GetType().Name,
            Message = Message,
            StatusCode = StatusCode
        };
        return JsonSerializer.Serialize(payload);
    }
}