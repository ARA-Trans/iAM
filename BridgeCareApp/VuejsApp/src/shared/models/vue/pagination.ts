export interface Pagination {
    descending: boolean;
    page: number;
    rowsPerPage: number;
    sortBy: string;
    totalItems: number;
}

export const emptyPagination: Pagination = {
    descending: false,
    page: 1,
    rowsPerPage: 5,
    sortBy: '',
    totalItems: 0
};
