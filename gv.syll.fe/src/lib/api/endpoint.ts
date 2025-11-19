export const ENDPOINTS = {
    connectToken: '/connect/token',
    getMe: '/api/app/users/me',
    getFormPaging: '/api/core/form',
    getGvLayout: '/api/core/form-layout',
    updateFormContent: (idForm: number) => `/api/core/form/${idForm}/form-content`,
    deleteRowTableData: '/api/core/form/row-table',
    downloadForm : (idForm: number) => `/api/core/form-template/form-loai/${idForm}/replace`,
    createForm: '/api/core/form',
    getToChucPaging :'/api/core/to-chuc',
    createToChuc : '/api/core/to-chuc',
    deleteToChuc : (idToChuc: number) => `/api/core/to-chuc/${idToChuc}`,
}