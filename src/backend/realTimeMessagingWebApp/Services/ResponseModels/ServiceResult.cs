namespace realTimeMessagingWebApp.Services.ResponseModels;

public class ServiceResult
{
    public bool IsSuccess { get; set; }
 
    public string Message { get; set; }
}

public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
 
    public string Message { get; set; }

    public T? Data { get; set; }
}
