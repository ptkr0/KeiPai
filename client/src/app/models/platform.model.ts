export interface IPlatform {
    id: number;
    name: string;
}

export interface IPlatformCampaign {
    id: number;
    name: string;
    canBeRequested: boolean;
}