import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { catchError, map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);
  const router = inject(Router);
  
  return userService.getUserInfo().pipe(
    map((res) => {
      if(res.id) {
        userService.currentUserSig.set(res);
        return true;
      }
      else
      {
        userService.currentUserSig.set(null);
        return false;
      }
    }),
    catchError(() => {
      return of(false);
    })
  );
}