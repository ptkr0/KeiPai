import { IBasicGame } from "./game.model";

export interface IYoutubeInfo {
    username: string;
    url: string;
    subscriberCount: number;
    viewCount: number;
    averageViewCount: number;
    lastCrawlDate?: string;
}

export interface IYoutubeVideo {
    id: string;
    title: string;
    url: string;
    description: string;
    viewCount: number;
    thumbnail: string;
    uploadDate?: string;
}

export interface IGetYoutubeVideo {
    id: number;
    games: IBasicGame[];
    video: IYoutubeVideo;
}

export interface IVideoPage {
    videos: IGetYoutubeVideo[];
    pageSize: number;
    totalPages: number;
    currentPage: number;
    totalCount: number;
}