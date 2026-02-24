import React from 'react';

export const LoadingSpinner: React.FC<{ className?: string }> = ({ className = '' }) => (
    <div className={`flex items-center justify-center ${className}`}>
        <div className="h-8 w-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
    </div>
);
