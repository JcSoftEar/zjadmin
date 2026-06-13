import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/login/index.vue'),
    meta: { title: '登录', noAuth: true }
  },
  {
    path: '/',
    component: () => import('../layouts/AppLayout.vue'),
    redirect: '/dashboard',
    children: [
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('../views/dashboard/index.vue'),
        meta: { title: '首页', icon: 'HomeFilled' }
      },
      {
        path: 'system/users',
        name: 'Users',
        component: () => import('../views/system/user/index.vue'),
        meta: { title: '用户管理', icon: 'User', permission: 'system:user:query' }
      },
      {
        path: 'system/roles',
        name: 'Roles',
        component: () => import('../views/system/role/index.vue'),
        meta: { title: '角色管理', icon: 'Avatar', permission: 'system:role:query' }
      },
      {
        path: 'system/permissions',
        name: 'Permissions',
        component: () => import('../views/system/permission/index.vue'),
        meta: { title: '权限管理', icon: 'Lock', permission: 'system:permission:query' }
      },
      {
        path: 'system/settings',
        name: 'Settings',
        component: () => import('../views/system/settings/index.vue'),
        meta: { title: '系统设置', icon: 'Tools', permission: 'system:config:query' }
      },
      {
        path: 'logs/operation',
        name: 'OperationLogs',
        component: () => import('../views/logs/operation/index.vue'),
        meta: { title: '操作日志', icon: 'Edit', permission: 'logs:operation:query' }
      },
      {
        path: 'logs/exception',
        name: 'ExceptionLogs',
        component: () => import('../views/logs/exception/index.vue'),
        meta: { title: '异常日志', icon: 'Warning', permission: 'logs:exception:query' }
      },
      {
        path: 'profile',
        name: 'Profile',
        component: () => import('../views/profile/index.vue'),
        meta: { title: '个人中心', icon: 'UserFilled' }
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem('token')
  if (to.meta.noAuth) {
    next()
  } else if (!token) {
    next('/login')
  } else {
    next()
  }
})

export default router
