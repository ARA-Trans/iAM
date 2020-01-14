export interface Announcement {
    id: string;
    title: string;
    content: string;
    creationDate: number;
}

export const emptyAnnouncement: Announcement = {
    id: '0',
    title: '',
    content: '',
    creationDate: 0
};