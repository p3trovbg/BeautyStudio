import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { appointmentSchema, type AppointmentFormData } from '../schemas/appointmentSchema';
import { useCreateAppointment } from '../hooks/useCreateAppointment';
import { useCheckOverlap } from '../hooks/useCheckOverlap';
import { OverlapWarning } from './OverlapWarning';

interface AppointmentFormProps {
    onSuccess?: () => void;
}

export const AppointmentForm: React.FC<AppointmentFormProps> = ({ onSuccess }) => {
    const createMutation = useCreateAppointment();

    const {
        register,
        handleSubmit,
        watch,
        formState: { errors, isSubmitting },
    } = useForm<AppointmentFormData>({
        resolver: zodResolver(appointmentSchema),
    });

    const ownerId = watch('ownerId');
    const startTime = watch('startTime');
    const endTime = watch('endTime');

    const { data: overlapResult } = useCheckOverlap({
        ownerId,
        startTime,
        endTime,
    });

    const hasOverlap = overlapResult?.hasOverlap ?? false;

    const onSubmit = async (data: AppointmentFormData) => {
        if (hasOverlap) return;
        await createMutation.mutateAsync(data);
        onSuccess?.();
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            <h2 className="text-xl font-semibold">New Appointment</h2>

            <OverlapWarning show={hasOverlap} />

            <div className="grid gap-4 sm:grid-cols-2">
                <div>
                    <label className="mb-1.5 block text-sm font-medium">Owner ID</label>
                    <input
                        type="text"
                        {...register('ownerId')}
                        className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring"
                        placeholder="Owner UUID"
                    />
                    {errors.ownerId && (
                        <p className="mt-1 text-sm text-destructive">{errors.ownerId.message}</p>
                    )}
                </div>

                <div>
                    <label className="mb-1.5 block text-sm font-medium">Customer ID</label>
                    <input
                        type="text"
                        {...register('customerId')}
                        className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring"
                        placeholder="Customer UUID"
                    />
                    {errors.customerId && (
                        <p className="mt-1 text-sm text-destructive">{errors.customerId.message}</p>
                    )}
                </div>
            </div>

            <div>
                <label className="mb-1.5 block text-sm font-medium">Title</label>
                <input
                    type="text"
                    {...register('title')}
                    className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring"
                    placeholder="Appointment title"
                />
                {errors.title && (
                    <p className="mt-1 text-sm text-destructive">{errors.title.message}</p>
                )}
            </div>

            <div className="grid gap-4 sm:grid-cols-2">
                <div>
                    <label className="mb-1.5 block text-sm font-medium">Start Time</label>
                    <input
                        type="datetime-local"
                        {...register('startTime')}
                        className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus:outline-none focus:ring-2 focus:ring-ring"
                    />
                    {errors.startTime && (
                        <p className="mt-1 text-sm text-destructive">{errors.startTime.message}</p>
                    )}
                </div>

                <div>
                    <label className="mb-1.5 block text-sm font-medium">End Time</label>
                    <input
                        type="datetime-local"
                        {...register('endTime')}
                        className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus:outline-none focus:ring-2 focus:ring-ring"
                    />
                    {errors.endTime && (
                        <p className="mt-1 text-sm text-destructive">{errors.endTime.message}</p>
                    )}
                </div>
            </div>

            <div>
                <label className="mb-1.5 block text-sm font-medium">Notes</label>
                <textarea
                    {...register('notes')}
                    rows={3}
                    className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring"
                    placeholder="Optional notes"
                />
            </div>

            <button
                type="submit"
                disabled={isSubmitting || hasOverlap}
                className="w-full rounded-md bg-primary px-4 py-2.5 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90 disabled:cursor-not-allowed disabled:opacity-50"
            >
                {isSubmitting ? 'Creating...' : 'Create Appointment'}
            </button>
        </form>
    );
};
