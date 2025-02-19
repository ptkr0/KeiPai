import { IBasicGame } from "./game.model";

export interface IOtherMedia {
    id: number;
    title: string;
    description: string;
    url: string;
    thumbnail: string;
    games: IBasicGame[];
}

export interface IConnectOtherMedia {
    name: string;
    url: string;
    role: string;
    medium: string;
    sampleContent: string;
}