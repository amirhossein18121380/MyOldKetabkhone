import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AccountService } from 'src/app/shared/services/account.service';
import { AccountClient, ChangePasswordViewModel, FileParameter, ProfileInfoViewModel, UserClient } from 'src/app/shared/services/api.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  //SearchCountryField = SearchCountryField;
  //CountryISO = CountryISO;


  

  
  @ViewChild('avatarInput') avatarInput: ElementRef;

  playerImagePath = environment.profileThumbnailPicturePath

  logoutModalRef: BsModalRef;
  imageModalRef: BsModalRef;






  profileForm: FormGroup;
  passForm: FormGroup;

  isChangeProfileInfo: boolean;
  isShowPass: boolean;
  public ff: ProfileInfoViewModel

  constructor(private toaster: ToastService,
     public accountService: AccountService,
     private userClient: UserClient,
     private modalService: BsModalService,
     private accountClient: AccountClient, 
     private router: Router
     ) {
  }
  public profileInfo: ProfileInfoViewModel;

  ngOnInit(): void {

    this.profileForm = new FormGroup({

      // firstName: new FormControl(this.accountService.profileInfo.firstName),

      // lastName: new FormControl(this.accountService.profileInfo.lastName),

      // gender: new FormControl(this.accountService.profileInfo.genderType),

      // birthDay: new FormControl(this.accountService.profileInfo.birthDay),

      // userName: new FormControl(this.accountService.profileInfo.userName, [
      //   Validators.required,
      //   Validators.minLength(5)
      // ]),

      // displayName: new FormControl(this.accountService.profileInfo.displayName),

      // tel: new FormControl(this.accountService.profileInfo.mobileNumber.replace('+98', ''), [
      //   Validators.required,
      // ]),

      firstName: new FormControl(),

      lastName: new FormControl(),

      gender: new FormControl(),

      birthDay: new FormControl(),

      userName: new FormControl('', [
        Validators.required,
        Validators.minLength(5)
      ]),

      displayName: new FormControl(),

      tel: new FormControl('', [
        Validators.required,
      ]),

    });
    

 
    setTimeout(() => {
      this.profileForm.valueChanges.subscribe(x => {
        this.isChangeProfileInfo = true;
      });
    }, 500)

    this.passForm = new FormGroup({

        pass: new FormControl('', [
          Validators.required,
          Validators.minLength(1)
        ]),

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

    // this.profileForm.setValue({
    //   firstName: this.profileInfo.firstName,
    //   lastName: this.profileInfo.lastName,
    //   gender: this.profileInfo.genderType,
    //   birthDay: this.profileInfo.birthDay,
    //   userName: this.profileInfo.userName,
    //   displayName: this.profileInfo.displayName,
    //   tel: this.profileInfo.mobileNumber.replace('+98', '')
    // });


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

  get profileF() {
    return this.profileForm.controls
  }

  get passF() {
    return this.passForm.controls
  }

  toggleShowPass() {
    this.isShowPass = !this.isShowPass
  }

  onClickUpdateProfile() {
    const profileModel = new ProfileInfoViewModel();
    profileModel.userName = this.profileF['userName'].value;
    profileModel.firstName = this.profileF['firstName'].value;
    profileModel.lastName = this.profileF['lastName'].value;
    profileModel.displayName = this.profileF['displayName'].value;
    //profileModel.countryIso = this.profileF.tel.value.countryCode
    //profileModel.countryCode = this.profileF.tel.value.dialCode
    profileModel.mobileNumber = this.profileF['tel'].value.e164Number
    profileModel.birthDay = this.profileF['birthDay'].value;
    profileModel.genderType = this.profileF['gender'].value
    if (profileModel.genderType != 1 && profileModel.genderType != 0) {
      profileModel.genderType = undefined;
    } else {
      // cast to integer
      profileModel.genderType = +profileModel.genderType;
    }

    console.log(profileModel)
    this.userClient.setProfileOtherProfileInfo(profileModel).subscribe(value => {
      this.toaster.showSuccess2('اطلاعات به موفقیت به روز رسانی شد.')
      this.isChangeProfileInfo = false;
      this.accountService.setProfileInfo(value)
    }, error => {
      this.toaster.showError2(error.response)
    })
  }

  onClickUpdatePassword() {
    const changePasswordModel = new ChangePasswordViewModel();
    changePasswordModel.currentPassword = this.passF['pass'].value;
    changePasswordModel.newPassword = this.passF['newPass'].value;
    this.userClient.changePassword(changePasswordModel).subscribe(value => {
      this.toaster.showSuccess2('رمز عبور با موفقیت تغییر یافت')
      this.passF['pass'].reset('');
      this.passF['newPass'].reset('');
      this.passF['conPass'].reset('');
    }, error => {
      this.toaster.showError2(error.response);
    })
  }





  onClickLogout(template: TemplateRef<any>) {
    this.logoutModalRef = this.modalService.show(
      template,
      Object.assign({}, { class: 'modal-md' })
    );
  }

  onClickChangeImage(template: TemplateRef<any>) {
    this.imageModalRef = this.modalService.show(template,
      Object.assign({}, { class: 'modal-md' })
    );
  }

  logout(confirm: boolean) {
    this.logoutModalRef.hide();
    if (confirm) {
      this.accountClient.logout(this.accountService.getRefreshToken()).subscribe(value => {
        this.accountService.setTokens(null, null);
        this.toaster.showSuccess2('با موفقیت از حساب خود خارج شده اید.');
        this.router.navigate(['/'], {replaceUrl: true}).then()
      }, error => {
        this.accountService.setTokens(null, null);
        this.toaster.showSuccess2('با موفقیت از حساب خود خارج شده اید.');
        this.router.navigate(['/'], {replaceUrl: true}).then()
      })
    }
  }

  editProfileImage() {
    this.imageModalRef.hide();
    this.avatarInput.nativeElement.click();
  }

  deleteProfileImage() {
    this.imageModalRef.hide();
    this.userClient.deleteImage(this.accountService.profileInfo.userId).subscribe(value => {
      this.accountService.profileInfo.profileImageName = undefined;
      this.toaster.showSuccess2('تصویر پروفایل با موفقیت حذف شد.')
    }, error => {
      this.toaster.showError2(error.response)
    })
  }

  onChangeAvatarInput(imageInput: any) {
    const file: File = imageInput.files[0];
    const fileParameter: FileParameter = {data: file, fileName: file.name}
    this.userClient.setProfileImage(fileParameter).subscribe(value => {
      this.toaster.showSuccess2('تصویر پروفایل با موفقیت آپلود شد.')
      this.accountService.profileInfo.profileImageName = value;
    }, error => {
      this.toaster.showError2(error.response);
    });
  }
  
}
