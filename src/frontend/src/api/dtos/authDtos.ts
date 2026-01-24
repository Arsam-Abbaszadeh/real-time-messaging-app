export type LoginRequestDto = {
  username: string;
  password: string;
};

export type LoginResponseDto = {
  accessToken: string;
  userId: string;
  username: string;
  message: string;
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
