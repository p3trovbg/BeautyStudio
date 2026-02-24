import React, { useState } from 'react';
import { useAppointments } from '../hooks/useAppointments';
import { appointmentsApi } from '../api/appointmentsApi';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Pagination } from '@/shared/components/Pagination';
import { LoadingSpinner } from '@/shared/components/LoadingSpinner';
import { formatDateTime } from '@/shared/utils/dateUtils';
import { toast } from 'sonner';
import type { AppointmentStatus } from '../types/appointment.types';

const statusColors: Record<AppointmentStatus, string> = {
    Pending: 'bg-yellow-100 text-yellow-800',
    Confirmed: 'bg-green-100 text-green-800',
    Cancelled: 'bg-red-100 text-red-800',
    Completed: 'bg-blue-100 text-blue-800',
};

export const AppointmentList: React.FC = () => {
    const [page, setPage] = useState(1);
    const { data, isLoading, error } = useAppointments({ page, pageSize: 10 });
    const queryClient = useQueryClient();

    const cancelMutation = useMutation({
        mutationFn: (id: string) => appointmentsApi.cancel(id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['appointments'] });
            toast.success('Appointment cancelled');
        },
        onError: () => toast.error('Failed to cancel appointment'),
    });

    if (isLoading) return <LoadingSpinner className="py-12" />;
    if (error) return <p className="text-destructive">Failed to load appointments.</p>;

    return (
        <div className="space-y-4">
            <div className="overflow-hidden rounded-lg border border-border">
                <table className="w-full text-sm">
                    <thead className="bg-muted/50">
                        <tr>
                            <th className="px-4 py-3 text-left font-medium">Title</th>
                            <th className="px-4 py-3 text-left font-medium">Owner</th>
                            <th className="px-4 py-3 text-left font-medium">Customer</th>
                            <th className="px-4 py-3 text-left font-medium">Start</th>
                            <th className="px-4 py-3 text-left font-medium">End</th>
                            <th className="px-4 py-3 text-left font-medium">Status</th>
                            <th className="px-4 py-3 text-left font-medium">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-border">
                        {data?.items.map((apt) => (
                            <tr key={apt.id} className="hover:bg-muted/30 transition-colors">
                                <td className="px-4 py-3 font-medium">{apt.title}</td>
                                <td className="px-4 py-3 text-muted-foreground">{apt.ownerName}</td>
                                <td className="px-4 py-3 text-muted-foreground">{apt.customerName}</td>
                                <td className="px-4 py-3 text-muted-foreground">{formatDateTime(apt.startTime)}</td>
                                <td className="px-4 py-3 text-muted-foreground">{formatDateTime(apt.endTime)}</td>
                                <td className="px-4 py-3">
                                    <span className={`inline-flex rounded-full px-2.5 py-0.5 text-xs font-medium ${statusColors[apt.status]}`}>
                                        {apt.status}
                                    </span>
                                </td>
                                <td className="px-4 py-3">
                                    {apt.status !== 'Cancelled' && apt.status !== 'Completed' && (
                                        <button
                                            onClick={() => cancelMutation.mutate(apt.id)}
                                            className="text-sm text-destructive hover:underline"
                                        >
                                            Cancel
                                        </button>
                                    )}
                                </td>
                            </tr>
                        ))}
                        {data?.items.length === 0 && (
                            <tr>
                                <td colSpan={7} className="px-4 py-8 text-center text-muted-foreground">
                                    No appointments found.
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            {data && (
                <Pagination
                    page={data.page}
                    totalPages={data.totalPages}
                    onPageChange={setPage}
                />
            )}
        </div>
    );
};
