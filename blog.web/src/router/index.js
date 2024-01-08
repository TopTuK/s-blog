import { createRouter, createWebHistory, createWebHashHistory } from 'vue-router';

const Home = () => import('@/views/Home.vue');
const Login = () => import('@/views/Login.vue');
const Profile = () => import('@/views/Profile.vue');

const routes = [
    {
        path: "/:catchAll(.*)",
        redirect: { name: "Home" },
    },
    {
        path: "/",
        name: "Home",
        component: Home,
        meta: {
            title: "home_route_title",
        },
    },
    {
        path: "/login",
        name: "Login",
        component: Login,
        meta: {
            title: "login_route_title",
        },
    },
    {
        path: "/profile",
        name: "Profile",
        component: Profile,
        meta: {
            title: "profile_route_title",
        },
    },
];

const router = createRouter({
    //history: createWebHashHistory(),
    history: createWebHistory(),
    routes,
    scrollBehavior(to, from, savedPosition) {
        return savedPosition || { top: 0 };
    },
});

router.beforeEach(async (to, from) => {
    console.log(`Router::beforEach: from: ${from.name} -> to: ${to.name}`);

    return true;
});

export default router;