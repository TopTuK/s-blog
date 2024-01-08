<template>
    <div class="relative py-16 sm:max-w-xl sm:mx-auto">
        <div class="absolute top-14 max-h-96 inset-0 bg-gradient-to-r from-blue-300 to-blue-600 shadow-lg transform -skew-y-6 sm:skew-y-0 sm:-rotate-6 sm:rounded-3xl">
        </div>

        <div class="relative mt-10 px-4 py-10 bg-white shadow-lg sm:rounded-3xl sm:p-30">
            <div class="max-w-md mx-auto">
                <div>
                    <h1 class="text-2xl font-semibold text-cyan-900">
                        {{ $t('login.login_title') }}
                    </h1>
                </div>

                <div class="divide-y divide-gray-200">
                    <div class="py-8 text-base leading-6 space-y-4 text-gray-700 sm:text-lg sm:leading-7">
                        <div class="relative">
                            <button
                                class="group h-12 px-6 border-2 border-gray-300 rounded-full transition duration-300 hover:border-blue-400 focus:bg-blue-50 active:bg-blue-100"
                            >
                                <div class="relative flex items-center space-x-4 justify-center">
                                    <span class="block w-max font-semibold tracking-wide text-gray-700 text-sm transition duration-300 group-hover:text-blue-600 sm:text-base">
                                        {{ $t('login.auth_google_title') }}
                                    </span>
                                </div>
                            </button>
                        </div>

                        <div class="relative">
                            <button 
                                class="group h-12 px-6 border-2 border-gray-300 rounded-full transition duration-300 hover:border-blue-400 focus:bg-blue-50 active:bg-blue-100"
                                @click="signInOidc"
                            >
                                <div class="relative flex items-center space-x-4 justify-center">
                                    <span class="block w-max font-semibold tracking-wide text-gray-700 text-sm transition duration-300 group-hover:text-blue-600 sm:text-base">
                                        {{ $t('login.auth_vas3k_title') }}
                                    </span>
                                </div>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
const signInGoogle = () => {
    window.location.href = "/auth/signingoogle";
}

const signInOidc = () => {
    window.location.href = "/auth/SignInVas3k";
}
</script>