import React from 'react';

interface PaginationProps {
    page: number;
    totalPages: number;
    onPageChange: (page: number) => void;
}

export const Pagination: React.FC<PaginationProps> = ({ page, totalPages, onPageChange }) => {
    if (totalPages <= 1) return null;

    return (
        <nav className="flex items-center justify-center gap-2 py-4" aria-label="Pagination">
            <button
                className="rounded-md border border-border px-3 py-1.5 text-sm font-medium transition-colors hover:bg-accent disabled:opacity-50 disabled:cursor-not-allowed"
                onClick={() => onPageChange(page - 1)}
                disabled={page <= 1}
            >
                Previous
            </button>
            <span className="text-sm text-muted-foreground">
                Page {page} of {totalPages}
            </span>
            <button
                className="rounded-md border border-border px-3 py-1.5 text-sm font-medium transition-colors hover:bg-accent disabled:opacity-50 disabled:cursor-not-allowed"
                onClick={() => onPageChange(page + 1)}
                disabled={page >= totalPages}
            >
                Next
            </button>
        </nav>
    );
};
