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
    // idk if I should create nested structures for the final DTO or something like that
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
