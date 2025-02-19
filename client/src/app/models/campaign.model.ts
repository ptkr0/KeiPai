import { IGame, IGameDetail } from "./game.model";
import { IAssignKeys, IKeysLeftForCampaign, IRequestsSentAndKeysLeft } from "./key.model";

export interface ICampaign {
    id: number;
    game: IGame;
    startDate: string;
    endDate: string;
    isClosed: boolean;
    keysLeftForCampaign: IKeysLeftForCampaign[];
    didJoin?: boolean;
}

export interface ICampaignPage {
    campaigns: ICampaign[];
    pageSize: number;
    totalPages: number;
    currentPage: number;
    totalCount: number;
}

export interface IAddCampaign {
    gameId: number;
    endDate?: string;
    startDate: string;
    description?: string;
    minimumYoutubeSubscribers?: number;
    minimumYoutubeAvgViews?: number;
    minimumTwitchFollowers?: number;
    minimumTwitchAvgViewers?: number;
    autoCodeDistribution: boolean;
    embargoDate?: string;
    areThirdPartyWebsitesAllowed: number;
    keys: IAssignKeys[];
}

export interface ICampaignDetail {
    id: number;
    game: IGameDetail;
    startDate: string;
    endDate: string;
    description: string;
    minimumYoutubeSubscribers: number;
    minimumYoutubeAvgViews: number;
    minimumTwitchFollowers: number;
    minimumTwitchAvgViewers: number;
    autoCodeDistribution: boolean;
    embargoDate: string;
    areThirdPartyWebsitesAllowed: number;
    isClosed: boolean;
    keysLeftForCampaigns: IKeysLeftForCampaign[];
}

export interface ICanRequest {
    canRequest: boolean;
    reasonCode: number;
    reasonMessage?: string;
}

export interface IActiveCampaign {
    id: number;
    gameId: number;
    gameName: string;
    keys: IRequestsSentAndKeysLeft[];
}

export interface ICampaignStats {
    campaign: ICampaign;
    keys: IRequestsSentAndKeysLeft[];
}