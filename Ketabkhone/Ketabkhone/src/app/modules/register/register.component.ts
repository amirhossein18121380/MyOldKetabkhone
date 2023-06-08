import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AccountService } from 'src/app/shared/services/account.service';
import { AccountClient, RegisterViewModel } from 'src/app/shared/services/api.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
   
  @ViewChild('recaptcha') recaptcha;

  //SearchCountryField = SearchCountryField;
  //CountryISO = CountryISO;

  regForm: FormGroup;
  isShowPass: boolean;
  isLoading: boolean;
  captchaToken: string;

  constructor(private toaster: ToastService,
    private accountClient: AccountClient,
    private accountService: AccountService) {
  }

  ngOnInit(): void {

    this.regForm = new FormGroup({
      userName: new FormControl('', [
        Validators.required,
        Validators.minLength(5)
      ]),

      email: new FormControl('', [
        Validators.required,
        Validators.email
      ]),

      pass: new FormControl('', [
        Validators.required,
        Validators.minLength(6)
      ]),

      conPass: new FormControl('', [
        Validators.required
      ]),

      tel: new FormControl('', [
        Validators.required,
      ]),

      captcha: new FormControl(null, Validators.required),

      rules: new FormControl(false, [
        Validators.requiredTrue
      ])
    },
      {
        validators: this.matchPassword('pass', 'conPass'),
      }
    );
  }

  matchPassword(password: string, confirmPassword: string) {
    return (formGroup: FormGroup) => {
      const passwordControl = formGroup.controls[password];
      const confirmPasswordControl = formGroup.controls[confirmPassword];

      if (!passwordControl || !confirmPasswordControl) {
        return null;
      }

      if (confirmPasswordControl.errors && !confirmPasswordControl.errors['passwordMismatch']) {
        return null;
      }

      if (passwordControl.value !== confirmPasswordControl.value) {
        confirmPasswordControl.setErrors({ passwordMismatch: true });
      } else {
        confirmPasswordControl.setErrors(null);
      }
    }
  }

  get f() {
    return this.regForm.controls
  }

  toggleShowPass() {
    this.isShowPass = !this.isShowPass
  }

  onResolvedCaptcha(captchaToken: string) {
    this.captchaToken = captchaToken;
  }

  resetCaptcha() {
    this.captchaToken = null;
    this.recaptcha.reset()
  }

  onSubmit() {
    if (this.regForm.invalid || this.isLoading) {
      return;
    } else {
      this.isLoading = true;
    }

    const registerModel: RegisterViewModel = new RegisterViewModel();
    //registerModel.countryIso = this.f.tel.value.countryCode;
    //registerModel.countryCode = this.f.tel.value.dialCode;
    registerModel.mobileNumber = this.f['tel'].value.e164Number;
    registerModel.email = this.f['email'].value;
    registerModel.userName = this.f['userName'].value;
    registerModel.password = this.f['pass'].value;
    registerModel.inviteCode = localStorage.getItem('inviteCode');
    registerModel.captchaCode = this.captchaToken;
    console.log(registerModel)

    this.accountClient.register(registerModel).subscribe(value => {
      this.toaster.showSuccess2('ثبت نام با موفقیت انجام شد')
      this.accountService.setTokens(value.token, value.refreshToken);
      this.accountService.setProfileInfo(value.profileInfo);

      const targetRoute = window.location.origin + this.accountService.getLoginTargetRoute();
      window.location.replace(targetRoute)
    }, error => {
      this.isLoading = false;
      this.toaster.showError2(error.response)
      this.resetCaptcha();
    });

  }
}
