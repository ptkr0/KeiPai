import { IUserRating } from "./review.model";

export interface ISendRequest {
    campaignId: number;
    platformId: number;
    contentPlatformType: string;
}

export interface ISendRequestResponse {
    id: number;
    status: number;
}

export interface IRequestInfluencer {
    id: number;
    campaignId: number;
    gameId: number;
    gameName: string;
    gameCover: string;
    platform: number;
    media: string;
    requestDate: string;
    considerationDate?: string;
    key?: string;
    status: number;
    content?: number;
}

export interface IRequestDeveloper {
    id: number;
    campaignId: number;
    gameId: number;
    gameName: string;
    influencerId: string;
    influencerName: string;
    influencerRating: IUserRating;
    platform: number;
    media: string;
    requestDate: string;
    status: number;
    considerationDate?: string;
    language?: string;
    content?: number;
}