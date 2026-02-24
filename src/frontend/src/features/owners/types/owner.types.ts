export interface Owner {
    id: string;
    fullName: string;
    email: string;
    phoneNumber?: string;
    createdAt: string;
}

export interface CreateOwner {
    fullName: string;
    email: string;
    phoneNumber?: string;
}

export interface UpdateOwner {
    fullName: string;
    email: string;
    phoneNumber?: string;
}
