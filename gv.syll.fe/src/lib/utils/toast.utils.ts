import { toast as sonnerToast } from 'svelte-sonner';

export const toast = {
    success: (message: string) => {
        sonnerToast.success(message, {
            style: 'background: #10b981; color: white; border: none;'
        });
    },
    error: (message: string) => {
        sonnerToast.error(message, {
            style: 'background: #ef4444; color: white; border: none;'
        });
    },
    info: (message: string) => {
        sonnerToast.info(message, {
            style: 'background: #3b82f6; color: white; border: none;'
        });
    },
    warning: (message: string) => {
        sonnerToast.warning(message, {
            style: 'background: #f59e0b; color: white; border: none;'
        });
    }
};