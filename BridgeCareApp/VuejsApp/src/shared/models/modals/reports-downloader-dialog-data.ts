export interface ReportsDownloaderDialogData {
    showModal: boolean;
    names: string[];
    networkId: number;
    networkName: string;
    simulationId: number;
    simulationName: string;
}

export const emptyReportsDownloadDialogData: ReportsDownloaderDialogData = {
    showModal: false,
    names: [],
    networkId: 0,
    networkName: '',
    simulationId: 0,
    simulationName: ''
};