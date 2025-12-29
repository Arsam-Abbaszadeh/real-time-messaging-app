using System.Net;

namespace realTimeMessagingWebAppInfra.Storage.Services.ResponseModel;

public class ObjectResponseEtag
{
    public string? ETag { get; init; }
    public HttpStatusCode statusCode { get; init; }
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
}
