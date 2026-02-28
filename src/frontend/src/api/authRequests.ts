import { fetchJson } from './httpRequests.ts';
import type {
    LoginRequestDto,
    LoginResponseDto,
    CreateAccountRequestDto,
    UserSummaryDto,
} from './dtos';
import { HEADERS, HEADER_VALUES } from './httpRequestHeaderConstants.ts';

export async function requestlogin(dto: LoginRequestDto): Promise<LoginResponseDto> {
    return await fetchJson<LoginResponseDto>('/user/login', {
        method: 'POST',
        body: JSON.stringify(dto),
        headers: {
            [HEADERS.CONTENT_TYPE]: HEADER_VALUES.APPLICATION_JSON,
        },
    });
}

export async function requestCreateAccount(dto: CreateAccountRequestDto): Promise<UserSummaryDto> {
    return await fetchJson<UserSummaryDto>('/user/createnewuser', {
        method: 'POST',
        body: JSON.stringify(dto),
        headers: {
            [HEADERS.CONTENT_TYPE]: HEADER_VALUES.APPLICATION_JSON,
        },
    });
}

export async function requestNewAccessToken(): Promise<LoginResponseDto> {
    return await fetchJson<LoginResponseDto>('/user/refreshaccesstokentoken', {
        method: 'GET',
    });
}
