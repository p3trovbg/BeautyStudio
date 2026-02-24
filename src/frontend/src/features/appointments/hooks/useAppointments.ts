import { useQuery } from '@tanstack/react-query';
import { appointmentsApi } from '../api/appointmentsApi';

export const useAppointments = (params: {
    ownerId?: string;
    customerId?: string;
    page?: number;
    pageSize?: number;
}) => {
    return useQuery({
        queryKey: ['appointments', params],
        queryFn: () => appointmentsApi.getAll(params),
    });
};

export const useAppointment = (id: string) => {
    return useQuery({
        queryKey: ['appointments', id],
        queryFn: () => appointmentsApi.getById(id),
        enabled: !!id,
    });
};
