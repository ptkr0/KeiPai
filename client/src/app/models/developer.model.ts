export interface IDeveloper {
    id: string;
    username: string;
    about: string;
    contactEmail: string;
    websiteUrl: string;
}

export interface IDeveloperWithInfo {
    developer: IDeveloper;
    campaignsCreated: number;
    gamesAdded: number;
}

export interface IUpdateDeveloper {
    username?: string;
    contactEmail: string;
    about: string;
    url: string;
}