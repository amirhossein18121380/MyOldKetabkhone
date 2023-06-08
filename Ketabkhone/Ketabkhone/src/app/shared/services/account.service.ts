import {Injectable} from '@angular/core';
import {ProfileInfoViewModel} from "./api.service";
import {Router} from "@angular/router";
import {ToastService} from "./toast.service";

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  public profileInfo: ProfileInfoViewModel;

  constructor(private toaster: ToastService, private router: Router) {
    this.profileInfo = new ProfileInfoViewModel();
  }

  isAuthenticated() {
    return !!this.getToken()
  }

  setTokens(token: string, refreshToken: string) {
    // save token and refreshToken
    if (token == null || refreshToken == null) {
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      this.setProfileInfo(new ProfileInfoViewModel())
    } else {
      localStorage.setItem('token', token);
      localStorage.setItem('refreshToken', refreshToken);
    }
  }

  getToken() {
    return localStorage.getItem('token');
  }

  getRefreshToken() {
    return localStorage.getItem('refreshToken')
  }

  setProfileInfo(profileInfo: ProfileInfoViewModel) {
    this.profileInfo.userId = profileInfo.userId;
    this.profileInfo.firstName = profileInfo.firstName;
    this.profileInfo.lastName = profileInfo.lastName;
    this.profileInfo.email = profileInfo.email;
    this.profileInfo.userName = profileInfo.userName;
    this.profileInfo.displayName = profileInfo.displayName;
   //this.profileInfo.countryIso = profileInfo.countryIso;
  //this.profileInfo.countryCode = profileInfo.countryCode;
    this.profileInfo.mobileNumber = profileInfo.mobileNumber;
    this.profileInfo.genderType = profileInfo.genderType;
    this.profileInfo.profileImageName = profileInfo.profileImageName;
    this.profileInfo.lastBalance = profileInfo.lastBalance;
    this.profileInfo.birthDay = profileInfo.birthDay;
    //this.profileInfo.playedHands = profileInfo.playedHands;
    //this.profileInfo.level = profileInfo.level;
    this.profileInfo.unreadNotificationCount = profileInfo.unreadNotificationCount;
    this.profileInfo.inviteCode = profileInfo.inviteCode;
    this.profileInfo.emailVerified = profileInfo.emailVerified;
    this.profileInfo.mobileVerified = profileInfo.mobileVerified;
  }

  setLoginTargetRoute(loginTargetRoute: string) {
    if (loginTargetRoute != null) {
      sessionStorage.setItem('loginTargetRoute', loginTargetRoute)
    } else {
      sessionStorage.removeItem('loginTargetRoute')
    }
  }

  getLoginTargetRoute() {
    // get target route
    let targetRoute = sessionStorage.getItem('loginTargetRoute')
    if (!targetRoute || targetRoute === '/') targetRoute = '/panel'
    // reset target route
    this.setLoginTargetRoute(null);
    // return target route
    return targetRoute;
  }

  login(loginTargetRoute?: string) {
    this.setTokens(null, null);
    this.setLoginTargetRoute(loginTargetRoute ? loginTargetRoute : this.router.url);
    this.router.navigate(['/login']).then();
  }

  logOut() {
    localStorage.removeItem('token');
    localStorage.removeItem("refreshToken");

    this.router.navigate(["login"]);
    console.log("logged out");
  }
}
