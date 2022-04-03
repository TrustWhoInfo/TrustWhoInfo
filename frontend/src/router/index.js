import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/about',
    name: 'About',
    component: () => import(/* webpackChunkName: "about" */ '../views/About.vue')
  },
  {
    path: '/post',
    name: 'Post News',
    component: () => import(/* webpackChunkName: "post" */ '../views/PostNews.vue')
  }, 
  {
    path: '/create-author',
    name: 'Create author',
    component: () => import(/* webpackChunkName: "newAuthor" */ '../views/CreateAuthor.vue')
  },
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
