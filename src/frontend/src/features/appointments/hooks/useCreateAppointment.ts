import { useMutation, useQueryClient } from '@tanstack/react-query';
import { appointmentsApi } from '../api/appointmentsApi';
import type { CreateAppointment } from '../types/appointment.types';
import { toast } from 'sonner';

export const useCreateAppointment = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (dto: CreateAppointment) => appointmentsApi.create(dto),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['appointments'] });
            toast.success('Appointment created successfully');
        },
        onError: (error: any) => {
            const message = error.response?.data?.detail || 'Failed to create appointment';
            toast.error(message);
        },
    });
};
