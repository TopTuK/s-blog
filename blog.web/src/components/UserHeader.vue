<template>
    <div class="inline-flex pr-2 gap-2">
        <div>
            <router-link
                to="/posts"
                class="items-center px-4 py-2 border border-transparent text-base font-medium rounded-md text-white bg-gray-700 hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
                {{ $t('common.posts_title') }}
            </router-link>
        </div>

        <div>
            {{ userStore.isAuthenticated }}
        </div>

        <div v-if="userStore.isAuthenticated">
            <router-link
                to="/profile"
                class="items-center px-4 py-2 border border-transparent text-base font-medium rounded-md text-white bg-gray-700 hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
                {{ $t('common.profile_button_title') }}
            </router-link>
        </div>
        <div v-else>
            <router-link
                to="/login"
                class="items-center px-4 py-2 border border-transparent text-base font-medium rounded-md text-white bg-gray-700 hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
                {{ $t('common.login_button_title') }}
            </router-link>
        </div>
    </div>
</template>

<script setup>
import { useUserStore } from "@/stores/userStore";

const userStore = useUserStore();
</script>