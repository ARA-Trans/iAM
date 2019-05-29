import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';

export interface ReportsDownloaderDialogData {
    showModal: boolean;
    scenario: Scenario;
}

export const emptyReportsDownloadDialogData: ReportsDownloaderDialogData = {
    showModal: false,
    scenario: emptyScenario
};
