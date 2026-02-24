import { format, parseISO, differenceInMinutes, addHours, isAfter, isBefore } from 'date-fns';

export const formatDate = (date: string | Date): string =>
    format(typeof date === 'string' ? parseISO(date) : date, 'PPP');

export const formatTime = (date: string | Date): string =>
    format(typeof date === 'string' ? parseISO(date) : date, 'p');

export const formatDateTime = (date: string | Date): string =>
    format(typeof date === 'string' ? parseISO(date) : date, 'PPPp');

export const getDurationMinutes = (start: string | Date, end: string | Date): number =>
    differenceInMinutes(
        typeof end === 'string' ? parseISO(end) : end,
        typeof start === 'string' ? parseISO(start) : start
    );

export const toISOString = (date: Date): string => date.toISOString();

export { addHours, isAfter, isBefore, parseISO };
