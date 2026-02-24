export type AppointmentStatus = 'Pending' | 'Confirmed' | 'Cancelled' | 'Completed';

export interface Appointment {
    id: string;
    ownerId: string;
    ownerName: string;
    customerId: string;
    customerName: string;
    title: string;
    startTime: string;
    endTime: string;
    status: AppointmentStatus;
    notes?: string;
    createdAt: string;
    updatedAt?: string;
    ownerEmail: string;
    customerEmail: string;
}

export interface CreateAppointment {
    ownerId: string;
    customerId: string;
    title: string;
    startTime: string;
    endTime: string;
    notes?: string;
}

export interface UpdateAppointment {
    title: string;
    startTime: string;
    endTime: string;
    status: AppointmentStatus;
    notes?: string;
}

export interface OverlapCheckResult {
    hasOverlap: boolean;
}
