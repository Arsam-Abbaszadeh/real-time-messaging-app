import { fetchJson } from './httpRequests.ts';

import { HEADERS, HEADER_VALUES } from './httpRequestHeaderConstants.ts';

export function getChatSummaries(): Promise<string>
    return fetchJson<>('/chat/chatsummaries')