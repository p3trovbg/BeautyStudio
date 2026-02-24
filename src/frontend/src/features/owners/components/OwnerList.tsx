import React, { useState } from 'react';
import { useOwners, useDeleteOwner } from '../hooks/useOwners';
import { Pagination } from '@/shared/components/Pagination';
import { LoadingSpinner } from '@/shared/components/LoadingSpinner';
import { formatDate } from '@/shared/utils/dateUtils';

export const OwnerList: React.FC = () => {
    const [page, setPage] = useState(1);
    const { data, isLoading, error } = useOwners({ page, pageSize: 10 });
    const deleteMutation = useDeleteOwner();

    if (isLoading) return <LoadingSpinner className="py-12" />;
    if (error) return <p className="text-destructive">Failed to load owners.</p>;

    return (
        <div className="space-y-4">
            <div className="overflow-hidden rounded-lg border border-border">
                <table className="w-full text-sm">
                    <thead className="bg-muted/50">
                        <tr>
                            <th className="px-4 py-3 text-left font-medium">Name</th>
                            <th className="px-4 py-3 text-left font-medium">Email</th>
                            <th className="px-4 py-3 text-left font-medium">Phone</th>
                            <th className="px-4 py-3 text-left font-medium">Created</th>
                            <th className="px-4 py-3 text-left font-medium">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-border">
                        {data?.items.map((owner) => (
                            <tr key={owner.id} className="hover:bg-muted/30 transition-colors">
                                <td className="px-4 py-3 font-medium">{owner.fullName}</td>
                                <td className="px-4 py-3 text-muted-foreground">{owner.email}</td>
                                <td className="px-4 py-3 text-muted-foreground">{owner.phoneNumber || '—'}</td>
                                <td className="px-4 py-3 text-muted-foreground">{formatDate(owner.createdAt)}</td>
                                <td className="px-4 py-3">
                                    <button onClick={() => deleteMutation.mutate(owner.id)} className="text-sm text-destructive hover:underline">
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))}
                        {data?.items.length === 0 && (
                            <tr><td colSpan={5} className="px-4 py-8 text-center text-muted-foreground">No owners found.</td></tr>
                        )}
                    </tbody>
                </table>
            </div>
            {data && <Pagination page={data.page} totalPages={data.totalPages} onPageChange={setPage} />}
        </div>
    );
};
