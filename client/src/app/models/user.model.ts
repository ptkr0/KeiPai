import { IDeveloperWithInfo } from "./developer.model";
import { IInfluencerWithInfo } from "./influencer.model";

export interface IUserInfo {
    developer?: IDeveloperWithInfo;
    influencer?: IInfluencerWithInfo;
}

export interface IBasicUserInfo {
    userId: string;
    username: string;
    about: string;
    contactEmail?: string;
}