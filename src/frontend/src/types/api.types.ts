/// <summary>Shared API response types matching the backend Result and PagedResult patterns.</summary>

export interface ApiResult<T> {
    value?: T;
    isSuccess: boolean;
    isFailure: boolean;
    errors: string[];
    errorCode?: string;
}

export interface PagedResult<T> {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}

export interface ProblemDetails {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    instance?: string;
}
