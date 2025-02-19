import { IBasicGame } from "./game.model";

export interface ITwitchInfo {
    username: string;
    url: string;
    followerCount: number;
    isAffiliateOrPartner: boolean;
    averageViewers: number;
    lastCrawlDate?: string
}

export interface ITwitchStream {
    id: string;
    title: string;
    url: string;
    averageViewers: number;
    peakViewers: number;
    thumbnail: string;
    startDate: string;
    endDate: string;
}

export interface IGetTwitchStream {
    id: number;
    games: IBasicGame[];
    stream: ITwitchStream;
}

export interface IStreamPage {
    streams: IGetTwitchStream[];
    pageSize: number;
    totalPages: number;
    currentPage: number;
    totalCount: number
}