import { fetchJson } from './httpRequests.ts';
import type {
    LoginRequestDto,
    LoginResponseDto,
    CreateAccountRequestDto,
    UserSummaryDto,
} from './dtos';

export function requestlogin(dto: LoginRequestDto): Promise<LoginResponseDto> {
    return fetchJson<LoginResponseDto>('/user/login', {
        method: 'POST',
        body: JSON.stringify(dto),
    });
}

export function requestCreateAccount(dto: CreateAccountRequestDto): Promise<UserSummaryDto> {
    return fetchJson<UserSummaryDto>('/user/createnewuser', {
        method: 'POST',
        body: JSON.stringify(dto),
    });
}

export function refreshAccessTokenRequest(): Promise<LoginResponseDto> {
    return fetchJson<LoginResponseDto>('/user/refreshaccesstokentoken', {
        method: 'GET',
    });
}