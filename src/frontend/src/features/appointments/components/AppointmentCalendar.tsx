import React, { useMemo } from 'react';
import { Calendar, dateFnsLocalizer, type Event } from 'react-big-calendar';
import { format, parse, startOfWeek, getDay } from 'date-fns';
import { enUS } from 'date-fns/locale/en-US';
import { useAppointments } from '../hooks/useAppointments';
import { LoadingSpinner } from '@/shared/components/LoadingSpinner';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import type { AppointmentStatus } from '../types/appointment.types';

const locales = { 'en-US': enUS };

const localizer = dateFnsLocalizer({
    format,
    parse,
    startOfWeek,
    getDay,
    locales,
});

const statusColorMap: Record<AppointmentStatus, string> = {
    Pending: '#eab308',
    Confirmed: '#22c55e',
    Cancelled: '#ef4444',
    Completed: '#3b82f6',
};

export const AppointmentCalendar: React.FC = () => {
    const { data, isLoading } = useAppointments({ pageSize: 100 });

    const events: Event[] = useMemo(() => {
        if (!data?.items) return [];
        return data.items.map((apt) => ({
            title: `${apt.title} (${apt.ownerName})`,
            start: new Date(apt.startTime),
            end: new Date(apt.endTime),
            resource: apt,
        }));
    }, [data]);

    if (isLoading) return <LoadingSpinner className="py-12" />;

    return (
        <div className="h-[700px] rounded-lg border border-border bg-card p-4">
            <Calendar
                localizer={localizer}
                events={events}
                startAccessor="start"
                endAccessor="end"
                defaultView="week"
                views={['month', 'week', 'day']}
                eventPropGetter={(event) => {
                    const apt = event.resource;
                    return {
                        style: {
                            backgroundColor: statusColorMap[apt.status as AppointmentStatus] ?? '#6b7280',
                            border: 'none',
                            borderRadius: '4px',
                            color: '#fff',
                        },
                    };
                }}
            />
        </div>
    );
};
