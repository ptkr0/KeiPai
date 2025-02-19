export interface IKey {
    id: number;
    value: string;
    gameId: number;
    platformId: number;
    platformName: string;
}

export interface IKeyPagination {
    pageSize: number;
    totalPages: number;
    currentPage: number;
    totalCount: number;
    keys: IKey[];
}

export interface IAddKeys {
    keys: IKeyForm[];
    platformId: number;
    gameId: number;
}

export interface IKeyForm {
    keys: string;
}

export interface IKeysPerPlatform {
    platformId: number;
    platformName: string;
    count: number;
}

export interface IAssignKeys {
    platformId: number;
    numberOfKeys: number;
    isUnlimited: boolean;
}

export interface IKeysLeftForCampaign {
    id: number;
    name: string;
    canBeRequested: boolean;
}

export interface IRequestsSentAndKeysLeft {
    id: number;
    name: string;
    acceptedRequests: number;
    keysLeft: number;
    keysForCampaign: number;
}