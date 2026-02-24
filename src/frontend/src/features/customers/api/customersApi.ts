import axiosInstance from '@/shared/api/axiosInstance';
import type { PagedResult } from '@/types/api.types';
import type { Customer, CreateCustomer, UpdateCustomer } from '../types/customer.types';

export const customersApi = {
    getAll: async (params: { page?: number; pageSize?: number }) => {
        const { data } = await axiosInstance.get<PagedResult<Customer>>('/customers', { params });
        return data;
    },
    getById: async (id: string) => {
        const { data } = await axiosInstance.get<Customer>(`/customers/${id}`);
        return data;
    },
    create: async (dto: CreateCustomer) => {
        const { data } = await axiosInstance.post<Customer>('/customers', dto);
        return data;
    },
    update: async (id: string, dto: UpdateCustomer) => {
        const { data } = await axiosInstance.put<Customer>(`/customers/${id}`, dto);
        return data;
    },
    delete: async (id: string) => {
        await axiosInstance.delete(`/customers/${id}`);
    },
};
