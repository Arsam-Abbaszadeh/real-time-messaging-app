import { fetchJson } from './httpRequests.ts';
import { HEADERS, HEADER_VALUES } from './httpRequestHeaderConstants.ts';
import type { chatSummaryDto } from '../dtos/chatDtos.ts';

export async function getChatSummaries(): Promise<chatSummaryDto[]> {
    return await fetchJson<chatSummaryDto[]>('/chat/summaries', {
        method: 'GET',
        headers: {
            [HEADERS.CONTENT_TYPE]: HEADER_VALUES.APPLICATION_JSON,
        },
    });
}
