import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AccountClient, ResetPasswordViewModel } from 'src/app/shared/services/api.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  
  token: string;
  message: string;
  isSend: boolean;
  isShowPass: boolean;

  userForm: FormGroup;
  passForm: FormGroup;

  constructor(private toaster: ToastService, private accountClient: AccountClient) {
    const path = location.pathname.split('/');
    if (path.length === 3) {
      this.token = path[2]
    }
  }

  ngOnInit(): void {
    this.userForm = new FormGroup({
      username: new FormControl( '', [
        Validators.required,
        Validators.minLength(1)
      ])
    });

    this.passForm = new FormGroup({

        newPass: new FormControl('', [
          Validators.required,
          Validators.minLength(6)
        ]),

        conPass: new FormControl('', [
          Validators.required
        ]),

      },
      {
        validators: this.matchPassword('newPass', 'conPass'),
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
        confirmPasswordControl.setErrors({passwordMismatch: true});
      } else {
        confirmPasswordControl.setErrors(null);
      }
    }
  }

  get userF() {
    return this.userForm.controls
  }

  get passF() {
    return this.passForm.controls
  }

  toggleShowPass() {
    this.isShowPass = !this.isShowPass
  }

  onClickSendMail() {
    const username = this.userF['username'].value;
    this.accountClient.forgetPassword(username).subscribe(value => {
      this.toaster.showSuccess2('درخواست بازیابی رمز عبور ارسال شد.');
      this.isSend = true;
      this.message = value;
    }, error => {
      this.toaster.showError2(error.response)
    })
  }

  onClickChangePassword() {
    const resetPasswordModel = new ResetPasswordViewModel()
    resetPasswordModel.forgetToken = this.token;
    resetPasswordModel.password = this.passF['newPass'].value

    this.accountClient.resetPassword(resetPasswordModel).subscribe(value => {
      this.toaster.showSuccess2('رمز عبور با موفقیت تغییر یافت.');
      this.isSend = true;
      this.message = 'رمز عبور شما با موفقیت تغییر یافت، اکنون میتوانید با استفاده از رمز عبور جدید وارد حساب خود شوید.'
    }, error => {
      this.toaster.showError2(error.response)
    })
  }

}
