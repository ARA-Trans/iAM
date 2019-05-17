export interface AlertData {
    showDialog: boolean;
    heading: string;
    message: string;
    choice: boolean;
}

export const emptyAlertData: AlertData = {
    showDialog: false,
    heading: '',
    message: '',
    choice: false
};