import type { PaginatedChatHistoryOptionsDto, chatMessageDto, chatSummaryDto } from '@/api/dtos/chatDtos';
import SignalRConnection from './connection';

export class ChatHubClient {
    private hubConnection: SignalRConnection;

    constructor() {
        this.hubConnection = new SignalRConnection();
    }

    public async getRecentChatHistory(chatId: string, range: number): Promise<chatMessageDto[]> {
        return await this.hubConnection.connection.invoke('GetRecentChatHistoryAsync', chatId, range);
    }

    public async getChatHistoryPaginated(options: PaginatedChatHistoryOptionsDto): Promise<chatMessageDto[]> {
        return await this.hubConnection.connection.invoke('GetChatHistoryPaginatedAsync', options);
    }
    public async joinChat(chatId: string): Promise<void> {
        await this.hubConnection.connection.invoke('JoinChatAsync', chatId);
    }

    // TODO: are we ever going to need this
    public async leaveChat(chatId: string): Promise<void> {
        await this.hubConnection.connection.invoke('LeaveChatAsync', chatId);
    }
}
