import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { ownersApi } from '../api/ownersApi';
import type { CreateOwner } from '../types/owner.types';
import { toast } from 'sonner';

export const useOwners = (params: { page?: number; pageSize?: number } = {}) =>
    useQuery({ queryKey: ['owners', params], queryFn: () => ownersApi.getAll(params) });

export const useOwner = (id: string) =>
    useQuery({ queryKey: ['owners', id], queryFn: () => ownersApi.getById(id), enabled: !!id });

export const useCreateOwner = () => {
    const qc = useQueryClient();
    return useMutation({
        mutationFn: (dto: CreateOwner) => ownersApi.create(dto),
        onSuccess: () => { qc.invalidateQueries({ queryKey: ['owners'] }); toast.success('Owner created'); },
        onError: (e: any) => toast.error(e.response?.data?.detail || 'Failed to create owner'),
    });
};

export const useDeleteOwner = () => {
    const qc = useQueryClient();
    return useMutation({
        mutationFn: (id: string) => ownersApi.delete(id),
        onSuccess: () => { qc.invalidateQueries({ queryKey: ['owners'] }); toast.success('Owner deleted'); },
        onError: () => toast.error('Failed to delete owner'),
    });
};
