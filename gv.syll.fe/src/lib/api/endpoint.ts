export const ENDPOINTS = {
    connectToken: '/connect/token',
    getMe: '/api/app/users/me',
    getGvLayout: '/api/core/form-layout',
    updateFormContent: (idForm: number) => `/api/core/form/${idForm}/form-content`,
}