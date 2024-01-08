import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import axios from 'axios';
import Cookies from 'js-cookie';
import { COOKIE_NAME, DEBUG  } from '@/config';

export const useUserStore = defineStore('userStore', () => {
    const USER_STORAGE_KEY = "userInfoKey";
    const GET_USER_INFO_ACTION = "user/getuserinfo";

    // store state
    const user = ref(JSON.parse(localStorage.getItem(USER_STORAGE_KEY)));
    const isLoading = ref(false);
    const error = ref(null);

    const getUser = async (forceRefresh = false) => {
        console.log("UserStore::getUser: start getting user information");

        let userInfo = null;

        if (!forceRefresh) {
            if (DEBUG) {
                console.log("UserStore::getUser: try to get user from local storage");
            }

            userInfo = JSON.parse(localStorage.getItem(USER_STORAGE_KEY));
            if (userInfo != null) {
                console.log("UserStore::getUser: found user in local storage");
                user.value = userInfo;

                return userInfo;
            }

            if (DEBUG) {
                console.log("UserStore::getUser: user is not found in local storage");
            }
        }

        try {
            error.value = null;
            isLoading.value = true; // Setting loading state to true

            const response = await axios.get(GET_USER_INFO_ACTION);

            if (DEBUG) {
                console.log(`UserStore::getUser: got response with status=${response.status}`);
            }

            // Check for bad request status
            if (response.status !== 200) {
                console.log("UserStore::getUser: ERROR: can't get user info");

                error.value = { isError: true, message: "" };
                user.value = null;
            }
            else {
                if (DEBUG) {
                    console.log("UserStore::getUser: userInfo=", response.data);
                }

                userInfo = response.data; // Setting user info

                user.value = userInfo; // Setting user info
                localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(userInfo));
            }

            isLoading.value = false; // Setting loading state to false
            return userInfo;
        }
        catch (ex) {
            console.log("UserStore::getUser: EXCEPTION: ", ex);

            isLoading.value = false; // Setting loading state to false in case of error
            error.value = ex;
        }
    }

    const isAuthenticated = computed(() => {
        return Boolean(Cookies.get(COOKIE_NAME));
    });

    return {
        isAuthenticated,
        user, getUser,
    }
});