import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { inject } from '@angular/core';
import { map, catchError, of } from 'rxjs';

export const guestGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);

  return userService.getUserInfo().pipe(
    map((res) => {
      if (res.id) {
        userService.currentUserSig.set(res);
        return false;
      } else {
        userService.currentUserSig.set(null);
        return true;
      }
    }),
    catchError(() => {
      userService.currentUserSig.set(null);
      return of(true);
    })
  );
}