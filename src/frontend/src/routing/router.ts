import { createRouter, createWebHistory } from 'vue-router';
import { LoginPage, CreateAccountPage, ChatLayout } from '../views';
import { ROUTE_NAMES } from './routeNames';

// does it makes sense to actually use a store here?
const redirects = [
    {
        path: ROUTE_NAMES.BASE,
        redirect: () => {
            // later we need more complex for to check auth as well to see if we are logged in.
            // may as well redirect to chats straight away
            return { name: ROUTE_NAMES.LOGIN };
        },
    },
];

const routes = [
    {
        path: '/login',
        name: ROUTE_NAMES.LOGIN,
        component: LoginPage,
    },
    {
        path: '/create-account',
        name: ROUTE_NAMES.ACCOUNT,
        component: CreateAccountPage,
    },
    {
        path: '/chat/:chatid?',
        name: ROUTE_NAMES.CHAT_LAYOUT,
        component: ChatLayout,
    },
    ...redirects,
];

export const router = createRouter({
    history: createWebHistory(),
    routes,
});
