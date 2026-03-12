export type chatSummaryDto = {
    chatId: string;
    chatName: string;
    chatImageUrl: string;
};

// DTO for getting chat message history
export type chatMessageDto = {
    chatId: string;
    messageSenderId: string;
    messageId: string;
    messageIsEdited: boolean;
    messageSentAt: Date;
    messageSequenceNumber: Number;
    messageContent: string;
    messageAttachments: chatMessageAttachment[];
};

// DTO for attachement retrieval
export type chatMessageAttachment = {
    attachementId: string;
    messageId: string; // not sure I need this
    attachmentUrl: string;
    attachmentMimeType: string;
};

// DTO for sending message, not getting history
export type sendChatMessageDto = {
    chatId: string;
    messageSenderId: string;
    messageSentAt: Date;
    messageContent: string;
};

export type imageDetailsForUploadUrlDto = {
    userId: string;
    chatId: string;
    fileExtension: string;
    fileType: string;
};

export type ChatHistoryOptionsPaginatedDto = {};
