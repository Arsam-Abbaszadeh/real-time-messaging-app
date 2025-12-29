namespace realTimeMessagingWebAppInfra.Persistence.Entities;

public class MessageAttachment
{
    public Guid MessageAttachmentId { get; set; }
    public Guid MessageId { get; set; }
    public int AttachementBucketKey { get; set; }
    public string AttachmentObjectKey { get; set; }
    public string AttachmentMimeType { get; set; }
    public string AttachmentFileExtension { get; set; }
    public DateTime UploadedAtUtc { get; set; }

    public Message Message { get; set; }
}
