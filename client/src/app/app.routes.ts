import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { MainPageComponent } from './pages/main-page/main-page.component';
import { RegisterComponent } from './pages/register/register.component';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';
import { YoutubeSuccessComponent } from './pages/youtube-success/youtube-success.component';
import { GamesComponent } from './pages/games/games.component';
import { AddGameComponent } from './pages/add-game/add-game.component';
import { GameComponent } from './pages/game/game.component';
import { EditGameComponent } from './pages/edit-game/edit-game.component';
import { TwitchSuccessComponent } from './pages/twitch-success/twitch-success.component';
import { ManageKeysComponent } from './pages/manage-keys/manage-keys.component';
import { AddCampaignComponent } from './pages/add-campaign/add-campaign.component';
import { CampaignComponent } from './pages/campaign/campaign.component';
import { CampaignsComponent } from './pages/campaigns/campaigns.component';
import { RequestsPendingComponent } from './pages/requests-pending/requests-pending.component';
import { RequestsAcceptedComponent } from './pages/requests-accepted/requests-accepted.component';
import { DeveloperRequestsComponent } from './pages/developer-requests/developer-requests.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { SettingsComponent } from './pages/settings/settings.component';
import { RequestsCompletedComponent } from './pages/requests-completed/requests-completed.component';
import { CampaignDetailsComponent } from './pages/campaign-details/campaign-details.component';
import { MessagesComponent } from './pages/messages/messages.component';

export const routes: Routes = [
    {
        path: '',
        component: MainPageComponent,
        canActivate: [guestGuard],
        children: []
    },
    {
        path: 'login',
        component: LoginComponent,
        canActivate: [guestGuard],
        children: []
    },
    {
        path: 'register/:type',
        component: RegisterComponent,
        canActivate: [guestGuard],
        children: []
    },
    {
        path: 'youtube-success',
        component: YoutubeSuccessComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'games',
        component: GamesComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'add-game',
        component: AddGameComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'game/:id',
        component: GameComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'edit-game/:id',
        component: EditGameComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'twitch-success',
        component: TwitchSuccessComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'manage-keys/:id',
        component: ManageKeysComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'campaigns',
        component: CampaignsComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'add-campaign',
        component: AddCampaignComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'campaign/:id',
        component: CampaignComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'campaign-details/:id',
        component: CampaignDetailsComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'pending',
        component: RequestsPendingComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'accepted',
        component: RequestsAcceptedComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'completed',
        component: RequestsCompletedComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'requests',
        component: DeveloperRequestsComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'profile/:username',
        component: ProfileComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'settings',
        component: SettingsComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'messages',
        component: MessagesComponent,
        canActivate: [authGuard],
        children: []
    },
    {
        path: 'messages/:id',
        component: MessagesComponent,
        canActivate: [authGuard],
        children: []
    }
];
