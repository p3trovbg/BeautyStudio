export interface Customer {
    id: string;
    fullName: string;
    email: string;
    phoneNumber?: string;
    createdAt: string;
}

export interface CreateCustomer {
    fullName: string;
    email: string;
    phoneNumber?: string;
}

export interface UpdateCustomer {
    fullName: string;
    email: string;
    phoneNumber?: string;
}
