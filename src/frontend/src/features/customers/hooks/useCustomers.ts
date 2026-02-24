import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { customersApi } from '../api/customersApi';
import type { CreateCustomer } from '../types/customer.types';
import { toast } from 'sonner';

export const useCustomers = (params: { page?: number; pageSize?: number } = {}) =>
    useQuery({ queryKey: ['customers', params], queryFn: () => customersApi.getAll(params) });

export const useCustomer = (id: string) =>
    useQuery({ queryKey: ['customers', id], queryFn: () => customersApi.getById(id), enabled: !!id });

export const useCreateCustomer = () => {
    const qc = useQueryClient();
    return useMutation({
        mutationFn: (dto: CreateCustomer) => customersApi.create(dto),
        onSuccess: () => { qc.invalidateQueries({ queryKey: ['customers'] }); toast.success('Customer created'); },
        onError: (e: any) => toast.error(e.response?.data?.detail || 'Failed to create customer'),
    });
};

export const useDeleteCustomer = () => {
    const qc = useQueryClient();
    return useMutation({
        mutationFn: (id: string) => customersApi.delete(id),
        onSuccess: () => { qc.invalidateQueries({ queryKey: ['customers'] }); toast.success('Customer deleted'); },
        onError: () => toast.error('Failed to delete customer'),
    });
};
