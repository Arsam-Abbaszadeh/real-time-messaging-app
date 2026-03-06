export type chatSummaryDto = {
    chatId: string;
    chatName: string;
    chatImageUrl: string;
};

export type chatMessageDto = {
    chatId: string;
    messageSenderId: string;
    messageId: string;
    messageIsEdited: boolean;
    messageSentAt: Date;
    messageSequenceNumber: Number;
    messageContent: string;
    // we should create nested structures for these DTOs
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
