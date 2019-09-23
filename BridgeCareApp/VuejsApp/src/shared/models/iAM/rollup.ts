export interface Rollup {
    networkId: number;
    networkName: string;
    createdDate?: Date;
    lastModifiedDate?: Date;
    rollupStatus?: string;
    id: number | string;
}

export const emptyRollup: Rollup = {
    networkId: 0,
    networkName: '',
    createdDate: new Date(),
    lastModifiedDate: new Date(),
    rollupStatus: '',
    id: 0
};
