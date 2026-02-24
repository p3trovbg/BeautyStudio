import axiosInstance from '@/shared/api/axiosInstance';
import type { PagedResult } from '@/types/api.types';
import type { Owner, CreateOwner, UpdateOwner } from '../types/owner.types';

export const ownersApi = {
    getAll: async (params: { page?: number; pageSize?: number }) => {
        const { data } = await axiosInstance.get<PagedResult<Owner>>('/owners', { params });
        return data;
    },
    getById: async (id: string) => {
        const { data } = await axiosInstance.get<Owner>(`/owners/${id}`);
        return data;
    },
    create: async (dto: CreateOwner) => {
        const { data } = await axiosInstance.post<Owner>('/owners', dto);
        return data;
    },
    update: async (id: string, dto: UpdateOwner) => {
        const { data } = await axiosInstance.put<Owner>(`/owners/${id}`, dto);
        return data;
    },
    delete: async (id: string) => {
        await axiosInstance.delete(`/owners/${id}`);
    },
};
