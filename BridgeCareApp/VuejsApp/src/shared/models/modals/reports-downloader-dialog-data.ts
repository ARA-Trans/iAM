import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';

export interface ReportsDownloaderDialogData {
    showModal: boolean;
    names: string[];
    scenario: Scenario;
}

export const emptyReportsDownloadDialogData: ReportsDownloaderDialogData = {
    showModal: false,
    names: [],
    scenario: emptyScenario
};
