import { z } from 'zod';

export const customerSchema = z.object({
    fullName: z.string().min(1, 'Full name is required').max(150),
    email: z.string().email('A valid email is required'),
    phoneNumber: z.string().max(20).optional(),
});

export type CustomerFormData = z.infer<typeof customerSchema>;
