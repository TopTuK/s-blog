<template>
    <div class="flex flex-col h-full">
        <div class="flex-none">
            <va-navbar color="#314155" class="h-14 p-2">
                <template #center>
                    <div class="text-white font-extrabold">
                        <span class="text-2xl">
                            {{ $t('common.header_title') }}
                        </span>
                    </div>
                </template>

                <template #right>
                    <user-header />
                </template>
            </va-navbar>
        </div>

        <div class="flex-1 bg-slate-300">
            <router-view />
        </div>

        <div class="flex-none">
            <Footer />
        </div>
    </div>
</template>

<script setup>
import UserHeader from '@/components/UserHeader.vue';
import Footer from '@/components/Footer.vue';
</script>