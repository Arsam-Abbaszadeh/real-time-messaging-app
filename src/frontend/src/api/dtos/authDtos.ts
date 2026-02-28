export type LoginRequestDto = {
    username: string;
    password: string;
};

export type LoginResponseDto = {
    userId: string;
    username: string;
    message: string;
    accessToken: string;
    accessTokenExpiration: Date;
};

export type CreateAccountRequestDto = {
    username: string;
    password: string;
};

export type UserSummaryDto = {
    userId: string;
    username: string;
    signIpDate: Date;
};
