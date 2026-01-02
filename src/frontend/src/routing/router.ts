import { createRouter, createWebHistory } from 'vue-router'
import { LoginPage, CreateAccountPage } from '../views'
import { ROUTE_NAMES } from '../utils/routeNames'

const routes = [
    { 
      path: '/login',
      name: ROUTE_NAMES.LOGIN,
      component: LoginPage,
    },
    {
      path: '/create-account',
      name: ROUTE_NAMES.ACCOUNT,
      component: CreateAccountPage
    }
]    

export const router = createRouter({
  history: createWebHistory(),
  routes
})
