import { IOtherPlatformInfo } from "./otherPlatform.model";
import { IUserRating } from "./review.model";
import { ITwitchInfo } from "./twitch.model";
import { IYoutubeInfo } from "./youtube.model";

export interface IInfluencerInfo {
    youtube?: IYoutubeInfo;
    otherMedia?: IOtherPlatformInfo;
    twitch?: ITwitchInfo;
}

export interface IInfluencer {
    id: string;
    username: string;
    about: string;
    contactEmail: string;
    language: string;
}

export interface IInfluencerWithInfo {
    influencer: IInfluencer;
    media: IInfluencerInfo;
    requestsSent: number;
    requestsDone: number;
    rating: IUserRating;
}

export interface IUpdateInfluencer {
    username?: string;
    contactEmail: string;
    about: string;
    language: string;
}