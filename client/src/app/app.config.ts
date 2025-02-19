import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './services/auth.interceptor';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';

export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes), 
    provideClientHydration(), 
    provideHttpClient(withInterceptors([authInterceptor]), withFetch()),
      provideAnimations(), // required animations providers
      provideToastr({positionClass: 'toast-bottom-right', tapToDismiss: true, progressBar: true, timeOut: 3000, progressAnimation: 'increasing', closeButton: true}), // Toastr providers
  ],
};
