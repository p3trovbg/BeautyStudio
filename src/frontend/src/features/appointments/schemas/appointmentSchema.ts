import { z } from 'zod';

export const appointmentSchema = z.object({
    ownerId: z.string().uuid('Please select an owner'),
    customerId: z.string().uuid('Please select a customer'),
    title: z.string().min(1, 'Title is required').max(200, 'Title must not exceed 200 characters'),
    startTime: z.string().min(1, 'Start time is required'),
    endTime: z.string().min(1, 'End time is required'),
    notes: z.string().max(1000).optional(),
}).refine((data) => new Date(data.endTime) > new Date(data.startTime), {
    message: 'End time must be after start time',
    path: ['endTime'],
});

export const updateAppointmentSchema = z.object({
    title: z.string().min(1, 'Title is required').max(200),
    startTime: z.string().min(1, 'Start time is required'),
    endTime: z.string().min(1, 'End time is required'),
    status: z.enum(['Pending', 'Confirmed', 'Cancelled', 'Completed']),
    notes: z.string().max(1000).optional(),
}).refine((data) => new Date(data.endTime) > new Date(data.startTime), {
    message: 'End time must be after start time',
    path: ['endTime'],
});

export type AppointmentFormData = z.infer<typeof appointmentSchema>;
export type UpdateAppointmentFormData = z.infer<typeof updateAppointmentSchema>;
