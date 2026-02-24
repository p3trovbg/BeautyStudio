import { createBrowserRouter, Navigate } from 'react-router-dom';
import { Layout } from '@/shared/components/Layout';
import { ProtectedRoute } from '@/shared/components/ProtectedRoute';
import { AppointmentList } from '@/features/appointments/components/AppointmentList';
import { AppointmentForm } from '@/features/appointments/components/AppointmentForm';
import { AppointmentCalendar } from '@/features/appointments/components/AppointmentCalendar';
import { OwnerList } from '@/features/owners/components/OwnerList';
import { CustomerList } from '@/features/customers/components/CustomerList';

export const router = createBrowserRouter([
    {
        path: '/',
        element: <Layout />,
        children: [
            { index: true, element: <Navigate to="/appointments" replace /> },
            {
                path: 'appointments',
                element: <ProtectedRoute><AppointmentList /></ProtectedRoute>,
            },
            {
                path: 'appointments/new',
                element: <ProtectedRoute><AppointmentForm /></ProtectedRoute>,
            },
            {
                path: 'appointments/calendar',
                element: <ProtectedRoute><AppointmentCalendar /></ProtectedRoute>,
            },
            {
                path: 'owners',
                element: <ProtectedRoute allowedRoles={['Owner']}><OwnerList /></ProtectedRoute>,
            },
            {
                path: 'customers',
                element: <ProtectedRoute><CustomerList /></ProtectedRoute>,
            },
            {
                path: 'login',
                element: <div className="flex h-[60vh] items-center justify-center"><p className="text-muted-foreground">Login page placeholder — integrate with your auth provider.</p></div>,
            },
            {
                path: 'unauthorized',
                element: <div className="flex h-[60vh] items-center justify-center"><p className="text-destructive">You do not have permission to view this page.</p></div>,
            },
        ],
    },
]);
