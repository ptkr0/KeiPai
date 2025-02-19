export interface IAddReview {
    revieweeId: string;
    rating: number;
    comment?: string;
    isAnonymous: boolean;
}

export interface IReview {
    id: number;
    reviewerId?: string;
    reviewerName?: string;
    rating: number;
    comment?: string;
    reviewDate: string;
    isAnonymous: boolean;
}

export interface IUpdateReview {
    updatedReview: IReview;
    oldRating: number;
}

export interface IUserRating {
    totalRating: number;
    numberOfRatings: number;
    userWasRated: boolean;
}