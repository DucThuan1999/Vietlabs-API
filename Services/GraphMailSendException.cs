namespace VietLab.Services;

public class GraphMailSendException : Exception
{
    public int StatusCode { get; }

    public string? GraphErrorCode { get; }

    public GraphMailSendException(
        string message,
        int statusCode = 0,
        string? graphErrorCode = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        GraphErrorCode = graphErrorCode;
    }
}
