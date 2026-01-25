import { fetchJson } from './httpRequests.ts';
import type {
    LoginRequestDto,
    LoginResponseDto,
    CreateAccountRequestDto,
    UserSummaryDto,
} from './dtos';
import { HEADERS, HEADER_VALUES } from './httpRequestHeaderConstants.ts';

export function requestlogin(dto: LoginRequestDto): Promise<LoginResponseDto> {
    return fetchJson<LoginResponseDto>('/user/login', {
        method: 'POST',
        body: JSON.stringify(dto),
        headers: {
            [HEADERS.CONTENT_TYPE]: HEADER_VALUES.APPLICATION_JSON,
        },
    });
}

export function requestCreateAccount(dto: CreateAccountRequestDto): Promise<UserSummaryDto> {
    return fetchJson<UserSummaryDto>('/user/createnewuser', {
        method: 'POST',
        body: JSON.stringify(dto),
        headers: {
            [HEADERS.CONTENT_TYPE]: HEADER_VALUES.APPLICATION_JSON,
        },
    });
}

export function refreshAccessTokenRequest(): Promise<LoginResponseDto> {
    return fetchJson<LoginResponseDto>('/user/refreshaccesstokentoken', {
        method: 'GET',
    });
}
