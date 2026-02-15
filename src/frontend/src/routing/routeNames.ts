export const ROUTE_NAMES = {
    HOME: 'home',
    LOGIN: 'login',
    ACCOUNT: 'createAccount',
    CHAT_LAYOUT: 'chat',
    BASE: '/',
} as const;

// type to allow for compile time safety and intellisense for routenames
export type RouteName = (typeof ROUTE_NAMES)[keyof typeof ROUTE_NAMES];
