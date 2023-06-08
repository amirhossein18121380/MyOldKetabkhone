import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AccountService } from 'src/app/shared/services/account.service';
import { AccountClient, LoginViewModel } from 'src/app/shared/services/api.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }
  
  // @ViewChild('recaptcha') recaptcha;

  // loginForm = new FormGroup({
  //   userName: new FormControl( '', [
  //     // Validators.required,
  //     // Validators.minLength(1)
  //   ]),

  //   pass: new FormControl('', [
  //     // Validators.required,
  //     // Validators.minLength(1)
  //   ]),

  //   //captcha: new FormControl(null, Validators.required)

  // });
  // isShowPass: boolean;
  // isLoading: boolean;
  // captchaToken: string;

  // constructor(private toaster: ToastService,
  //             private accountClient: AccountClient,
  //             private accountService: AccountService) { }

  // ngOnInit(): void {
  // }

  // get f() {
  //   return this.loginForm.controls
  // }

  // toggleShowPass() {
  //   this.isShowPass = !this.isShowPass
  // }

  // // onResolvedCaptcha(captchaToken: string) {
  // //   this.captchaToken = captchaToken;
  // // }

  // // resetCaptcha() {
  // //   this.captchaToken = null;
  // //   this.recaptcha.reset()
  // // }

  // onSubmit() {
  //   if (this.loginForm.invalid || this.isLoading) {
  //     return;
  //   } else {
  //     this.isLoading = true;
  //   }

  //   const loginModel = new LoginViewModel()
  //   loginModel.userName = this.f['userName'].value;
  //   loginModel.password = this.f['pass'].value;
  //   loginModel.captchaId = 'null';
  //   loginModel.captchaCode = 'null';
  //   //loginModel.captchaCode = this.captchaToken;


  //   this.accountClient.login(loginModel).subscribe(value => {
  //     console.log(value)
  //     this.toaster.showSuccess2('ورود با موفقیت انجام شد')
  //     this.accountService.setTokens(value.token, value.refreshToken);
  //     this.accountService.setProfileInfo(value.profileInfo);

  //     const targetRoute = window.location.origin + this.accountService.getLoginTargetRoute();
  //     window.location.replace(targetRoute)
  //   }, error => {
  //     this.isLoading = false;
  //     this.toaster.showError2(error.response)
  //     //this.resetCaptcha();
  //   })
  // }
}
