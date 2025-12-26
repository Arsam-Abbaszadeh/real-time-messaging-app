namespace realTimeMessagingWebAppInfra.Persistence.Entities;

public class MessageAttachment
{
    public Guid MessageAttachmentId { get; set; }
    // Table for storing message attachments access details given all attachements will be on R2
    public Guid MessageId { get; set; }
    public int AttachementBucketKey { get; set; }
    public string AttachmentKey { get; set; } // notn sure if its a url or key value thing yet
    public string AttachmentType { get; set; }
    public Message Message { get; set; }
}
