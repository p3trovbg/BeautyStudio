import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuthStore } from '@/shared/hooks/useAuth';

interface ProtectedRouteProps {
    children: React.ReactNode;
    allowedRoles?: ('Owner' | 'Customer')[];
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, allowedRoles }) => {
    const { isAuthenticated, role } = useAuthStore();

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    if (allowedRoles && role && !allowedRoles.includes(role)) {
        return <Navigate to="/unauthorized" replace />;
    }

    return <>{children}</>;
};
