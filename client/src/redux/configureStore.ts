import {createStore} from 'redux';
import * as api from "../api/apiCalls";
import authenticationReducer, {AuthState} from "./authenticationReducer";

const notLoggedInUser: AuthState = {
    email: '',
    userId: 0,
    username: '',
    isLoggedIn: false,
    accessToken: '',
    encryptionKey: '',
    refreshToken: ''
}

export const configureStore = () => {
    let loadedUserFromLocalStorage = checkIfUserDataAreStoredInLocalStorage();
    const state = loadedUserFromLocalStorage ? {...loadedUserFromLocalStorage} : {...notLoggedInUser};
    // @ts-ignore
    const store = createStore(authenticationReducer, state, window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__());

    store.subscribe(() => {
        localStorage.setItem('secure-password-manager-user', JSON.stringify(store.getState()));
        const {isLoggedIn, accessToken} = store.getState();
        api.setAuthHeader({isLoggedIn, accessToken});
    });

    window.addEventListener('storage', function (event) {
        if (event.storageArea === localStorage) {
            let a = localStorage.getItem('secure-password-manager-user');
            if (a) {
                const payload: AuthState = JSON.parse(a);
                if (payload.isLoggedIn){
                    store.dispatch({type: "login", payload})
                }
                else {
                    store.dispatch({type: "logout"})
                }
            }
        }
    }, false);

    return store;
};

const checkIfUserDataAreStoredInLocalStorage = (): AuthState | null => {
    let loadedUserFromLocalStorage = localStorage.getItem('secure-password-manager-user');
    if (loadedUserFromLocalStorage) {
        try {
            const temp: AuthState = JSON.parse(loadedUserFromLocalStorage);
            api.setAuthHeader({isLoggedIn: true, accessToken: temp.accessToken});
            return temp;
        } catch {
            return null;
        }
    }
    return null;
}