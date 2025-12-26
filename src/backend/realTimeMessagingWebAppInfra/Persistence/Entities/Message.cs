namespace realTimeMessagingWebAppInfra.Persistence.Entities
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public Guid SenderId { get; set; } // fk to user
        public Guid GroupChatId { get; set; } // this can be either group chat or private chat id
        public string MessageContent { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsEdited { get; set; } = false;
        public DateTime? EditedAt { get; set; } = null;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;
        public ulong SequenceNumber { get; set; }

        // Nav properties
        public User Sender { get; set; }
        public GroupChat Chat { get; set; }
        public ICollection<MessageAttachment> Attachments { get; set; }
    }
}
