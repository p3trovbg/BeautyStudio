import axiosInstance from '@/shared/api/axiosInstance';
import type { PagedResult } from '@/types/api.types';
import type { Appointment, CreateAppointment, UpdateAppointment, OverlapCheckResult } from '../types/appointment.types';

export const appointmentsApi = {
    getAll: async (params: { ownerId?: string; customerId?: string; page?: number; pageSize?: number }) => {
        const { data } = await axiosInstance.get<PagedResult<Appointment>>('/appointments', { params });
        return data;
    },

    getById: async (id: string) => {
        const { data } = await axiosInstance.get<Appointment>(`/appointments/${id}`);
        return data;
    },

    create: async (dto: CreateAppointment) => {
        const { data } = await axiosInstance.post<Appointment>('/appointments', dto);
        return data;
    },

    update: async (id: string, dto: UpdateAppointment) => {
        const { data } = await axiosInstance.put<Appointment>(`/appointments/${id}`, dto);
        return data;
    },

    cancel: async (id: string) => {
        await axiosInstance.delete(`/appointments/${id}`);
    },

    checkOverlap: async (params: {
        ownerId: string;
        startTime: string;
        endTime: string;
        excludeAppointmentId?: string;
    }) => {
        const { data } = await axiosInstance.get<OverlapCheckResult>('/appointments/check-overlap', { params });
        return data;
    },
};
