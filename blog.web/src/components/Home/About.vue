<template>
    <div class="flex flex-row h-44 justify-center items-center gap-8">
        <VaAvatar
            size="120px"
            src="/images/ava.jpg"
        />

        <div class="flex flex-col">
            <span class="text-3xl">
                {{ $t('home.about_author_name') }}
            </span>
            <span class="text-xl">
                {{ $t('home.about_blog_description') }}
            </span>
        </div>
    </div>
</template>

<script setup>
</script>