namespace realTimeMessagingWebApp.Controllers.QureyParamObjects;

public class PaginatedChatHistoryOptionsQuery
{
    public int? StartMessageSequenceNumber { get; init; }
    public int? EndMessageSequenceNumber { get; init; }
    public bool? EndFallBackToMaxInt { get; init; }

    // TOOD consider just inherting from IvalidateObject
    public bool validate()
    {
        if (StartMessageSequenceNumber is not null && EndMessageSequenceNumber is not null)
        {
            return true;
        }

        if (StartMessageSequenceNumber is not null && EndMessageSequenceNumber is not null)
        {
            return true;
        }

        return false;
    }
}