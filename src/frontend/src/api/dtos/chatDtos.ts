export type chatSummaryDto = {
    chatId: string;
    chatName: string;
    chatImageUrl: string;
};

export type chatMessageDto = {
    messageSenderId: string;
    messageId: string;
    messageIsEdited: boolean;
    messageSentAt: Date;
    messageSequenceNumber: Number;
    messageContent: string;
    // idk if I should create nested structures for the final DTO or something like that
    //
};
