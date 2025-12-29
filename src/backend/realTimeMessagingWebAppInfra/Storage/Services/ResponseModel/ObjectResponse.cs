using System.Net;

namespace realTimeMessagingWebAppInfra.Storage.Services.ResponseModel;

public class ObjectResponse
{
    public bool IsSuccess { get; init; }
    public HttpStatusCode StatusCode { get; init; }
    public string? ErrorMessage { get; init; }
}
