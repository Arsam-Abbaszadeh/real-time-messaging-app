import { fetchJson } from './httpRequests.ts';
import { HEADERS, HEADER_VALUES } from './httpRequestHeaderConstants.ts';
import type { chatSummaryDto } from './dtos/chatDtos.ts';

export async function getChatSummaries(): Promise<Array<chatSummaryDto>> {
    return await fetchJson<Array<chatSummaryDto>>('/chat/chatsummaries', {
        method: 'GET',
    });
}

// TODO: this needs to be paginated and what not, cant make a get request for entire history
export async function getChatHistory(id: string) {
    // need to get create actualy tyhpe for chat history
    return await fetchJson<Array<chatSummaryDto>>('/chat/chatsummaries', {
        method: 'GET',
    });
}
