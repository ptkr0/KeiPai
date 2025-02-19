import { IKeysPerPlatform } from "./key.model";
import { IScreenshot } from "./screenshot.model";
import { ITag } from "./tag.model";

export interface IGame {
    id: number;
    developerId: string;
    name: string;
    coverPhoto?: string;
    releaseDate: string;
    tags: ITag[];
}

export interface IGamePage {
    games: IGame[];
    pageSize: number;
    totalPages: number;
    currentPage: number;
    totalCount: number;
}

export interface IIGDB {
    id: number;
    name: string;
}

export interface IGameDetail {
    id: number;
    name: string;
    description: string;
    coverPhoto?: string;
    releaseDate: string;
    youtubeTrailer: string;
    youtubeTag: string;
    twitchTagId: number;
    twitchTagName: string;
    minimumCPU: string;
    minimumGPU: string;
    minimumRAM: string;
    minimumStorage: string;
    minimumOS: string;
    pressKit: string;
    tags: ITag[];
    screenshots: IScreenshot[];
    developerId: string;
    developerName: string;
}

export interface IUpdateGame {
    Name: string;
    Description: string;
    ReleaseDate: string;
    YoutubeTrailer: string;
    YoutubeTag: string;
    TwitchTagId: number;
    TwitchTagName: string;
    MinimumCPU: string;
    MinimumGPU: string;
    MinimumRAM: string;
    MinimumStorage: string;
    MinimumOS: string;
    PressKit: string;
    Tags: number[];
}

export interface IAddCampaignGame {
    id: number;
    name: string;
    coverPhoto: string;
    releaseDate: string;
    youtubeTag: string;
    twitchTagId: number;
    twitchTagName: string;
    keysPerPlatform: IKeysPerPlatform[];
}

export interface IBasicGame {
    id: number;
    name: string;
    cover?: string;
}