export const ENDPOINTS = {
    connectToken: '/connect/token',
    getMe: '/api/app/users/me',
    getFormPaging: '/api/core/form',
    getGvLayout: '/api/core/form-layout',
    updateFormContent: (idForm: number) => `/api/core/form/${idForm}/form-content`,
    deleteRowTableData: '/api/core/form/row-table',
    createForm: '/api/core/form'
}