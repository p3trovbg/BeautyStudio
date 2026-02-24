import { useQuery } from '@tanstack/react-query';
import { appointmentsApi } from '../api/appointmentsApi';

export const useCheckOverlap = (params: {
    ownerId: string;
    startTime: string;
    endTime: string;
    excludeAppointmentId?: string;
}) => {
    return useQuery({
        queryKey: ['check-overlap', params],
        queryFn: () => appointmentsApi.checkOverlap(params),
        enabled: !!params.ownerId && !!params.startTime && !!params.endTime,
        refetchInterval: false,
    });
};
