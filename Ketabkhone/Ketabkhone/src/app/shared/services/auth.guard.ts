import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  
  constructor(private accountService:AccountService, private router: Router) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):
    Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const isAuthenticated = this.accountService.isAuthenticated();
    if (state.url === '/login' || state.url === '/register' || state.url.includes('/resetPassword')) {
      if (isAuthenticated) {
        this.router.navigate(['/']).then()
        return false;
      } else {
        return true;
      }
    } else {
      if (isAuthenticated) {
        return true;
      } else {
        this.accountService.login(state.url);
        return false;
      }
    }

  }
}
