import axios from 'axios';

const apiUrl = 'https://localhost:5001';

export const setAuthHeader = (req: { isLoggedIn: boolean, accessToken: string }) => {
    if (req.accessToken) {
        axios.defaults.headers.common['Authorization'] = `Bearer ${req.accessToken}`;
    }
}

export const deleteAuthHeader = () => {
    delete axios.defaults.headers.common['Authorization'];
}

export const register = (registerUserRequest: { password: string; encryptionKeyHash: string; email: string; username: string }) => {
    return axios.post(apiUrl + '/api/accounts/register', registerUserRequest);
};

export const login = (loginUserRequest: { password: string; identifier: string }) => {
    return axios.post(apiUrl + '/api/accounts/login', loginUserRequest);
};

export const verifyEmailAddress = (verifyEmailAddressRequest: { username: string; token: string }) => {
    return axios.post(apiUrl + '/api/accounts/verify-email-address', verifyEmailAddressRequest);
}

export const requestResetPassword = (requestResetPasswordRequest: { identifier: string }) => {
    return axios.post(apiUrl + '/api/accounts/request-password-reset', requestResetPasswordRequest);
}

export const requestEmailVerification = (requestEmailVerificationRequest: { identifier: string }) => {
    return axios.post(apiUrl + '/api/accounts/request-email-verification', requestEmailVerificationRequest);
}

export const resetPassword = (resetPasswordRequest: { username: string, resetPasswordToken: string, newPassword: string }) => {
    return axios.post(apiUrl + '/api/accounts/reset-password', resetPasswordRequest);
}

export const verifyEncryptionKey = (verifyEncryptionKeyRequest: { encryptionKeyHash: string }) => {
    return axios.post(apiUrl + '/api/accounts/verify-encryption-key', verifyEncryptionKeyRequest);
}

export const getVault = () => {
    return axios.get(apiUrl + '/api/vault');
}

export const addVaultItem = (addVaultItemRequest: {
    encryptionKeyHash: string, websiteUrl: string, encryptedPassword: string, identifier: string
}) => {
    return axios.post(apiUrl + '/api/vault', addVaultItemRequest);
}

export const changePassword = (changePasswordRequest: { newPassword: string, oldPassword: string }) => {
    return axios.post(apiUrl + '/api/accounts/change-password', changePasswordRequest);
}