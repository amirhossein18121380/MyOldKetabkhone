import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { BehaviorSubject, catchError, filter, Observable, switchMap, take, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AccountService } from './account.service';
import { AccountClient, Token } from './api.service';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  
  private refreshTokenInProgress = false;
  // Refresh Token Subject tracks the current token, or is null if no token is currently
  // available (e.g. refresh pending).
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(public accountService: AccountService, private router: Router, private accountClient: AccountClient) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    // add assessToken to request
    // some request like login & register & refreshToken dont need accessToken
    if (!request.url.includes("api/Account/Register") &&
      !request.url.includes("api/Account/Login") &&
      !request.url.includes("api/Account/RefreshToken")) {
      request = this.addToken(request);
    }

    // handle errors
    return next.handle(request).pipe(catchError<any, any>(error => {

      // If error status is different than 401 we want to skip refresh token
      // So we check that and throw the error if it's the case
      if (error.status !== 401) {
        return throwError(error);
      }

      // We do another check to see if refresh token failed
      if (request.url.includes("api/Account/RefreshToken")) {
        this.redirectToLogin();
        return throwError(error)
      }

      // check refreshToken existence
      const refreshToken = this.accountService.getRefreshToken();
      if (refreshToken == null || refreshToken.length === 0) {
        this.redirectToLogin();
        return throwError(error)
      }

      if (this.refreshTokenInProgress) {
        // If refreshTokenInProgress is true, we will wait until refreshTokenSubject has a non-null value
        // – which means the new token is ready and we can retry the request again
        return this.refreshTokenSubject.pipe(
          filter(result => result !== null),
          take(1),
          switchMap(() => next.handle(this.addToken(request)))
        );
      } else {
        this.refreshTokenInProgress = true;

        // Set the refreshTokenSubject to null so that subsequent API calls will wait until the new token has been retrieved
        this.refreshTokenSubject.next(null);

        // Call auth.refreshAccessToken(this is an Observable that will be returned)
        const token = new Token();
        token.refreshToken = this.accountService.getRefreshToken();
        return this.accountClient.refreshToken(token).pipe(
          switchMap(token => {
            //When the call to refreshToken completes we reset the refreshTokenInProgress to false
            // for the next time the token needs to be refreshed
            this.accountService.setTokens(token.token, token.refreshToken)
            this.refreshTokenInProgress = false;
            this.refreshTokenSubject.next(token);

            return next.handle(this.addToken(request));
          }),
          catchError<any, any>(err => {
            // checked refreshToken if changed with other request ignore error
            if (err.status === 401) {
              const refreshToken = token.refreshToken;
              const lastRefreshToken = this.accountService.getRefreshToken();
              if (refreshToken != null && refreshToken.length > 0
                && lastRefreshToken != null && lastRefreshToken.length > 0
                && refreshToken != lastRefreshToken) {
                this.refreshTokenInProgress = false;
                this.refreshTokenSubject.next(token);
                return next.handle(this.addToken(request));
              }
            }

            this.refreshTokenInProgress = false;
            return throwError(err);
          })
        );
      }
    }));
  }

  redirectToLogin() {
    // checked if current route needed authenticated redirect to login else just remove expired token
    const route = this.router.url;
    if (route.includes('panel') || route.includes('lobby') || route.includes('table')) {
      this.accountService.login();
    } else {
      this.accountService.setTokens(null, null)
    }
  }

  addToken(request) {
    // Get access token from Local Storage
    const token = this.accountService.getToken();
    if (token) {
      // We clone the request, because the original request is immutable
      return request.clone({
        setHeaders: {
          'Authorization': `Bearer ${token}`,
        }
      });
    } else {
      return request;
    }
  }
}
