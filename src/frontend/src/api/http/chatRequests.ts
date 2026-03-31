import { fetchJson } from './httpRequests.ts';
import { HEADERS, HEADER_VALUES } from './httpRequestHeaderConstants.ts';
import type { chatMessageDto, chatSummaryDto, PaginatedChatHistoryOptionsDto } from '../dtos/chatDtos.ts';

export async function getChatSummaries(): Promise<chatSummaryDto[]> {
    return await fetchJson<chatSummaryDto[]>('/chat/summaries', {
        method: 'GET',
        headers: {
            [HEADERS.CONTENT_TYPE]: HEADER_VALUES.APPLICATION_JSON,
        },
    });
}

// TODO: are the \n going to effect the query params?
export async function getMessages(
    chatId: string,
    messageCount?: number,
    chatHistoryOptions?: PaginatedChatHistoryOptionsDto
): Promise<chatMessageDto[]> {
    return await fetchJson<chatMessageDto[]>(`/chat/${chatId}/messages${provideGetMessageQureyParams(messageCount, chatHistoryOptions)}`, {
        method: 'GET',
        headers: {
            [HEADERS.CONTENT_TYPE]: HEADER_VALUES.APPLICATION_JSON,
        },
    });
}

function provideGetMessageQureyParams(messageCount?: number, chatHistoryOptions?: PaginatedChatHistoryOptionsDto): string {
    let params = '';

    if (messageCount) {
        params += `messageCount=${messageCount}`;
    }

    if (chatHistoryOptions) {
        if (chatHistoryOptions.startMessageSequence) {
            params = params ? `${params}&` : params;
            params += `startMessageSequence=${chatHistoryOptions.startMessageSequence}`;
        }
        if (chatHistoryOptions.endMessageSequence) {
            params = params ? `${params}&` : params;
            params += `endMessageSequence=${chatHistoryOptions.endMessageSequence}`;
        }
        if (chatHistoryOptions.EndFallBackToMaxInt) {
            params = params ? `${params}&` : params;
            params += `EndFallBackToMaxInt=${chatHistoryOptions.EndFallBackToMaxInt}`;
        }
    }

    return params ? `?${params}` : '';
}
